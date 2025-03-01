using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assistant;
using IronPython.Compiler;

//#assembly <IronPython.dll>
//#assembly <IronPython.Modules.dll>
//#import <../Common/Bod.cs>
//#import <../Common/BodData.cs>
//#import <../Common/Gumps.cs>
//#import <../Common/Items.cs>
//#import <../Common/Json.cs>
//#import <../Common/JsonConverters.cs>
//#import <../Common/Logger.cs> 
//#import <../Common/Math.cs>
//#import <../Common/Mobiles.cs>
//#import <../Common/Movement.cs>
//#import <../Common/Movement_Antistuck.cs>
//#import <../Common/Player.cs>
//#import <../Common/System.cs>
//#import <../Common/Tasks.cs>
//#import <../Common/Tiles.cs>
//#import <../Lists/Ore.cs> 
//#import <../Lists/Misc.cs> 
//#import <../Lists/Mobiles.cs>
//#import <../Lists/Tools.cs>
//#import <../Lists/Wood.cs>


//check for you can't use an axe on that.
namespace RazorEnhanced
{
    internal class GatherWood
    {
        private const bool DropCommonLogs = false; // Drop on ground commons logs
        private const int MaxWeightRange = 100; // How far from max before chop and move
        private const int PurgeAfterMins = 45; // Remove tree from the list after N minutes
        private const int FindRadius = 16; // Radius around player to search for trees, max 16

        public GatherWood()
        {
            UoTJson.FileName = "gatherWood";
            UoTasks.InitializeTasks();
            
            
            using (var storedData = new UoTJson("gatherWood"))
            {
                _ignoredTrees = storedData.GetData<ConcurrentDictionary<(int,int),DateTime>>(
                    "ignoredTrees", typeof(ConcurrentDictionary<(int,int),DateTime>));

            }
            PurgeExpiredEntries();
            if (_ignoredTrees != null) Misc.SendMessage("ignoredTrees: " + _ignoredTrees.Count, 33);
        }
  
        private class Tree
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; } 
           // public string Name { get; set; }
            public int StaticId { get; set; }
        }


        
        private static  ConcurrentDictionary<(int, int), DateTime> _ignoredTrees = new();
        private static  ConcurrentDictionary<(int,int), Tree> _treesToChop = new();
        private static readonly HashSet<string> InvalidTreeNames = new () { "o'hii tree" };
        
        public static HashSet<int> BlacklistStaticIds = new HashSet<int>
        {
            8770, 8766, 8767, 8768, 8769, 8771, 3247, 3343
        }; 

        private static void BackupData()
        {
            if (!UoTSystem.CheckTimer(Player.Serial, "Backing up data", "", UoTSystem.MessageOutput.None, 33,
                    10000))
            {
                if (_ignoredTrees == null ) _ignoredTrees = new ConcurrentDictionary<(int, int), DateTime>();
                else
                {
                    using (var storedData = new UoTJson("gatherWood"))
                    { 
                        storedData.StoreData(_ignoredTrees, "ignoredTrees");
                    }
                }
            }
        }
        private static void RunLoop(CancellationToken token)
        {
            Tree treeToChop = null;
            while (!token.IsCancellationRequested) 
            {
                try 
                {
                    BackupData();
                    treeToChop ??= TreeToChop(token);
                    if (treeToChop != null && _ignoredTrees != null &&
                        _ignoredTrees.TryGetValue((treeToChop.X, treeToChop.Y), out var expirationTime2) &&
                        DateTime.Now < expirationTime2) treeToChop = null;
                    if (treeToChop != null && ChopTree(treeToChop, token)) UoTMovement.PathingTask(treeToChop.X, treeToChop.Y, default, 1, token);
                    else treeToChop = null;
                    

                    CheckPlayerOverloaded(token);
                    
                    Misc.Pause(50);
                }
                catch (ThreadAbortException)
                {
                    UoTLogger.LogErrorToFile($"Script shutting down due to ThreadAbortException in main Run.");
                    //Thread.ResetAbort(); // Prevents abort crash.
                    throw;
                }
                catch (OperationCanceledException)
                {
                    UoTLogger.LogErrorToFile("Run()'s loop method RunLoop() was canceled.");
                    break; // Exit the loop on cancellation

                }
                catch (Exception e)
                {
                    UoTLogger.LogErrorToFile($"Unexpected error during periodic tasks: {e.Message}", e);
                }
            }
        }

        public void Run()
        {
            try
            {
                UoTLogger.LogFilePath = Path.Combine(Engine.RootPath, "UoTgatherWood.txt");
                var mainToken = UoTasks.GetTaskCancellationToken();
                UoTLogger.LogErrorToFile("Starting Tree Scan task.");
                UoTasks.StartTask("Tree Scan", async (cancellationToken) => 
                    await ScanForTrees(mainToken));
                UoTSystem.SearchJournal(UoTItems.journal, "not enough wood here to harvest"); // Clear message from journal
                RunLoop(mainToken);
                
            }
            catch (OperationCanceledException)
            {
                // Handle the graceful cancellation var stackTrace = Environment.StackTrace; UoTLogger.LogErrorToFile($"Cancellation triggered. Stack trace: {stackTrace}");
                UoTLogger.LogErrorToFile("Run() was canceled safely.");
            }
            catch (ThreadAbortException)
            {
                UoTLogger.LogErrorToFile($"Script shutting down due to ThreadAbortException in main Run.");
                Thread.ResetAbort(); // Prevents abort crash.
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    UoTLogger.LogErrorToFile(inner.Message, inner);
                }
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile(e.Message, e);
                Misc.SendMessage($"Error: {e.Message}", 33);
            }
            finally
            {
                try
                {
                    if (_treesToChop != null) UoTLogger.LogErrorToFile($"Cleared trees to chop, Count={_treesToChop.Count}");
                    UoTasks.Shutdown();
                }
                catch (OperationCanceledException)
                {
                    // Handle the graceful cancellation var stackTrace = Environment.StackTrace; UoTLogger.LogErrorToFile($"Cancellation triggered. Stack trace: {stackTrace}");
                    UoTLogger.LogErrorToFile("Run() was canceled safely.");
                }
                catch (ThreadAbortException)
                {
                    UoTLogger.LogErrorToFile($"Script shutting down due to ThreadAbortException in main Run.");
                    Thread.ResetAbort(); // Prevents abort crash.
                }
                catch (AggregateException ex)
                {
                    foreach (var innerEx in ex.InnerExceptions)
                    {
                        UoTLogger.LogErrorToFile($"Error in finally (calling Stop): {innerEx.Message}", innerEx);
                    }
                }
                catch (Exception ex)
                {
                    UoTLogger.LogErrorToFile($"Error in finally (calling Stop): {ex.Message}", ex);
                }
            }
        }
        
        // Purges expired entries from ignored lists
        private static void PurgeExpiredEntries()
        {
            var now = DateTime.Now;

            try
            {
                List<(int, int)> keysSnapshot;
                keysSnapshot = _ignoredTrees.Keys.ToList(); // Create a fresh snapshot for _ignoredTrees

                foreach (var key in keysSnapshot)
                {
                    if (_ignoredTrees.TryGetValue(key, out var expirationTime) && expirationTime < now)
                    {
                        _ignoredTrees.TryRemove(key, out _); // Remove the tree entry safely
                    }
                }
            }
  
            catch (OperationCanceledException)
            {
                UoTLogger.LogErrorToFile("PurgeExiredEntries was canceled safely.");
            }
            catch (ThreadAbortException)
            {
                UoTLogger.LogErrorToFile($"Script shutting down due to ThreadAbortException in main Run.");
                //Thread.ResetAbort(); // Prevents abort crash.
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    UoTLogger.LogErrorToFile(inner.Message, inner);
                }

            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile(e.Message, e);
                Misc.SendMessage($"Error: {e.Message}", 33);
            }
        }
  

        private static bool IsTileWorthChecking((int X, int Y) tileCoord)
        { 
            if (_ignoredTrees != null && _ignoredTrees.TryGetValue(tileCoord, out var expirationTime2) && 
                DateTime.Now < expirationTime2)
                return false; // Tree is still ignored
            if (_treesToChop != null && _treesToChop.ContainsKey(tileCoord)) 
                return false; // Tree is already queued for chopping
            return true; // Tile is worth checking
        } 
 
        private static void ResetValues()
        {
            //_ignoredTrees.Clear();
            using (var storedData = new UoTJson("gatherWood"))
            {
                storedData.ClearData("storage_chest");
                storedData.ClearData("book_current");
                storedData.ClearData("book_1");
                //storedData.ClearData("book_2");
                storedData.ClearData("ignoredTrees");
                //storedData.ClearData("TilesCache");
                //storedData.ClearData("_treesToChop");
            } 
        }
        private static void DropLogs(CancellationToken token = default)
        {
            if (!DropCommonLogs) return;
            var logs = UoTItems.FindItems(UoTLists.CommonLogs, Player.Backpack, null, false);
            //Player.HeadMessage(33, "Dropping logs");
            UoTItems.DropItemsOnGroud(logs, token);
        }
        private static void CutLogs(CancellationToken token = default)
        {
            var logs = UoTItems.FindItems(UoTLists.LogTypes, Player.Backpack, null, false);
            var hatchetExists = UoTItems.FindAndEquipWeapon(UoTools.Axes, false);
            if (hatchetExists == null) return;
            Misc.SetSharedValue("Lootmaster:Pause", true);
            foreach (var log in logs)
            {
                token.ThrowIfCancellationRequested();
                if (!log.Movable || DropCommonLogs &&  log.Hue == 0) continue;
                if (Target.HasTarget()) Target.Cancel();
                Misc.Pause(100);
                Items.UseItem(hatchetExists);
                Misc.Pause(200);
                Target.WaitForTarget(1000, true);
                Target.TargetExecute(log);
                Misc.Pause(600);
            }
            Misc.SetSharedValue("Lootmaster:Pause", false);
        }

        private static void MoveBoardsToStorage(CancellationToken token = default)
        {
            try
            {
                Player.HeadMessage(33, "Recalling to drop stuff off");
                Misc.SetSharedValue("Lootmaster:Pause", true);
                UoTasks.StopTask("Pathing");
                Misc.Pause(600);
                DropLogs(token);
                var beetle = UoTMobiles.FindBeetle();
                if (beetle != null && Player.Mount == null) Mobiles.UseMobile(beetle);
                UoTPlayer.RecallWithBook();
                Misc.SetSharedValue("Lootmaster:Pause", false);
                Misc.Pause(600);
                Player.ChatSay("bank");
                for (var i = 0; i < 50; i++)
                {
                    if (Player.Bank != null && Player.Bank.Updated) break;
                    Misc.Pause(20);
                }

                Item chest = null;
                if (Player.Bank != null && Player.Bank.ContainerOpened)
                {
                    chest = Player.Bank;
                }
                else
                {
                    var chestSerial = UoTJson.StoreOrGetJson<int>("storage_chest");
                    chest = Items.FindBySerial(chestSerial);
                    if (chest == null || !chest.IsContainer)
                    {
                        Player.HeadMessage(33, "Storage chest not found, please select another.");
                        Misc.SendMessage("Storage chest not found, please select another.");
                        chestSerial = UoTItems.target.PromptTarget("Select a new storage chest");
                        chest = Items.FindBySerial(chestSerial);
                        if (chest == null || !chest.IsContainer)
                        {
                            Player.HeadMessage(33, "New storage chest not found");
                            Misc.SendMessage("New storage chest not found");
                            return;
                        }
                    }

                    UoTMovement.PathingTask(chest.Position.X, chest.Position.Y, default, 2, token);
                }

                if (chest == null) return;
                Misc.SetSharedValue("Lootmaster:Pause", true);
                //update lists to check for hue
                UoTItems.MoveItemsToContainer(UoTLists.BoardTypes, chest.Serial, token);
                /*
                if (Player.GetSkillValue("Tinkering") > 40)
                {
                    UoTItems.RestockSupplies((0x1BF2, 0x0000), 40);
                }
                else
                {
                    UoTItems.RestockSupplies(UoTools.Pickaxes.FirstOrDefault(), 15);
                }
                */
                Misc.SetSharedValue("Lootmaster:Pause", false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                var beetle = UoTMobiles.FindBeetle();
                if (beetle != null && Player.Mount == null) Mobiles.UseMobile(beetle);
            }
        }
        private static void MoveBoardsToAnimals(CancellationToken token = default)
        {
            Misc.SetSharedValue("Lootmaster:Pause", true);
            Misc.SendMessage("Dismounting to move boards to beetle.", 33);
            Misc.Pause(700);
            if (Player.Mount != null)  Mobiles.UseMobile(Player.Serial);
            Misc.Pause(600);
            var beetle = UoTMobiles.FindBeetle();
            var boards = UoTItems.FindItems(UoTLists.BoardTypes, Player.Backpack, null, false);
            Misc.SetSharedValue("Lootmaster:Pause", false);
            if (boards == null || !boards.Any())
            {
                Misc.SendMessage("No boards found", 33);
                if (beetle != null) 
                    Mobiles.UseMobile(beetle);
                return;
            }
            Misc.SetSharedValue("Lootmaster:Pause", true);
            Misc.Pause(300);
            UoTItems.MoveItemsToAnimal(boards, token);
            Misc.Pause(600);
            if (beetle != null) 
                Mobiles.UseMobile(beetle);
            Misc.Pause(300);
            Misc.SetSharedValue("Lootmaster:Pause", false);
            
        }
        private static void MoveBoardsToChestFromBeetle(CancellationToken token = default)
        { 
            var beetle = UoTMobiles.FindBeetle();
            if (Player.Mount == null && beetle == null) return;
            Mobiles.UseMobile(Player.Serial);
            Misc.Pause(300);
            beetle = UoTMobiles.FindBeetle();
            if (beetle == null) return;
            Items.OpenAt(beetle.Backpack.Serial, 0, 0);
            Misc.Pause(300);
            var boards = UoTItems.FindItems(UoTLists.BoardTypes, beetle.Backpack, null, false);
            if (boards == null || !boards.Any()) return;
            if (UoTasks.IsTaskRunning("Pathing")) 
            {
                Misc.Pause(2000); 
            }
            Misc.SetSharedValue("Lootmaster:Pause", true);
            //boards = UoTItems.FindItems(UoTLists.BoardTypes, beetle.Backpack, null, true);
            var chestSerial = UoTJson.StoreOrGetJson<int>("storage_chest");
            var chest = Items.FindBySerial(chestSerial);
            if (Player.Bank != null && Player.Bank.ContainerOpened) chest = Player.Bank;
            if (chest == null) return;
            UoTItems.MoveItemsToContainerFromBeetle(boards, chest.Serial, token);
            Misc.Pause(6000); 
            if (Player.Mount == null) Mobiles.UseMobile(beetle);
            Misc.SetSharedValue("Lootmaster:Pause", false);
        }
        
        private static bool CheckPlayerOverloaded(CancellationToken token = default)
        {
            try
            {
                _treeTimer.Stop();
                if (Player.Weight < Player.MaxWeight - MaxWeightRange) return false;
                if (Player.WarMode) Player.SetWarMode(false);
                UoTasks.StopTask("Pathing");
                DropLogs(token);
                if (Player.Weight > Player.MaxWeight - MaxWeightRange)
                {
                    CutLogs(token);
                    DropLogs(token);
                }
                if (Player.Weight > Player.MaxWeight - MaxWeightRange) MoveBoardsToAnimals(token);
                if (Player.Weight > Player.MaxWeight - MaxWeightRange)
                {
                    MoveBoardsToStorage(token);
                    MoveBoardsToChestFromBeetle(token);
                    Misc.Pause(5000);
                    UoTPlayer.RecallWithBook(1, UoTPlayer.UpdateRecallRuneIndex());
                }
                return Player.Weight > Player.MaxWeight - MaxWeightRange;
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile(e.Message, e);
                throw;
            }
            finally
            {
                _treeTimer.Start();
            }
        }
        private enum TitleInfoState
        {
            Loading,
            Tree,
            Other
        }
        private static readonly SemaphoreSlim SemaphoreTileInfo = new SemaphoreSlim(1); // Allow only 1 thread at a time
        private static async Task< (TitleInfoState, Tree)> FindTreeAt((int X, int Y) tileCoord, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (Player.Map == 0) return (TitleInfoState.Loading,null);
                await SemaphoreTileInfo.WaitAsync(token); // Acquire access
                token.ThrowIfCancellationRequested(); // Check for cancellation early
                //var tileInfo = Statics.GetStaticsTileInfo(tileCoord.X, tileCoord.Y, Player.Map);
                var tileInfo = UoTiles.GetTileInfo(tileCoord.X, tileCoord.Y, Player.Map);
                if (tileInfo == null || !tileInfo.Any()) return (TitleInfoState.Loading,null);
                // Remove tile info without the word tree
                tileInfo.RemoveAll(tile =>
                {
                    if (tile == null) return true;
                    var tileName = Statics.GetTileName(tile.ID)?.ToLower();
                    return tileName == null 
                           || InvalidTreeNames.Contains(tileName) 
                           || !tileName.Contains("tree");
                });
                if (!tileInfo.Any()) return (TitleInfoState.Other, null);
                foreach (var tile in tileInfo)
                { 
                    if (tile == null) continue;
                    token.ThrowIfCancellationRequested();
                    return (TitleInfoState.Tree, new Tree
                    {
                        X = tileCoord.X,
                        Y = tileCoord.Y,
                        Z = Statics.GetTileHeight(tile.StaticID),
                       // Name = Statics.GetTileName(tileInfo[0].ID),
                        StaticId = tile.StaticID,
                    });
                }
                return (TitleInfoState.Other, null);
            }
            catch (NullReferenceException ex)
            {
                //_nullTiles.Add(tileCoord);
                // Handle the specific "Object reference not set to an instance of an object" error
                UoTLogger.LogErrorToFile($"NullReferenceException caught find: {ex.Message}.");
                throw;
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    UoTLogger.LogErrorToFile(inner.Message, inner);
                }
                throw;
            }
            catch (OperationCanceledException)
            {
                UoTLogger.LogErrorToFile($"FindTreeAt was canceled for tile: {tileCoord}");
                throw;
            }
            catch (ThreadAbortException)
            {
                UoTLogger.LogErrorToFile("Script shutting down due to ThreadAbortException.");
                Thread.ResetAbort(); // Prevents abort crash.
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile("Error in FindTreeAt", e);
                throw;
            }
            finally
            {
                SemaphoreTileInfo.Release();
            }

            return (TitleInfoState.Loading, null);
        }

        private static async Task PerformSearchForTile((int X, int Y) tileCoord, 
            SemaphoreSlim semaphore, CancellationToken token)
        {
            {
                try
                {
                    //UoTLogger.LogErrorToFile("Starting Search for tile.");
                    // Apply cancellation logic within the semaphore's timeout
                    await semaphore.WaitAsync(token);
                    // Check whether the tile is worth exploring
                    if (!IsTileWorthChecking(tileCoord)) return;
                    var (result, tree) = await FindTreeAt(tileCoord, token);
                    token.ThrowIfCancellationRequested();
                    switch (result)
                    {
                        case TitleInfoState.Loading:
                            //UoTLogger.LogErrorToFile($"Loading failed at: {tileCoord.X}, {tileCoord.Y}");
                            break;

                        case TitleInfoState.Tree:
                            UoTLogger.LogErrorToFile($"Tree found at: {tileCoord.X}, {tileCoord.Y}");
                            _treesToChop.TryAdd((tree.X,tree.Y), tree); // Enqueue valid tree
                            break;
                        case TitleInfoState.Other:
                           // UoTLogger.LogErrorToFile($"Ignoring tile: {tileCoord.X}, {tileCoord.Y}");
                            /*_noTrees.AddOrUpdate(tileCoord,
                                DateTime.Now.AddMinutes(PurgeAfterMins),
                                (_,_) => DateTime.Now.AddMinutes(PurgeAfterMins));*/
                            break;
                    }
                }
                catch (NullReferenceException ex)
                {
                    //_nullTiles.Add(tileCoord);
                    // Handle the specific "Object reference not set to an instance of an object" error
                    UoTLogger.LogErrorToFile($"NullReferenceException caught perform: {ex.Message}.");
                    // Optionally, you can log the stack trace as well for debugging
                    throw;
                }
                catch (AggregateException ex)
                {
                    foreach (var inner in ex.InnerExceptions)
                    {
                        UoTLogger.LogErrorToFile(inner.Message, inner);
                    }
                    throw;
                }
                catch (OperationCanceledException)
                {
                    // Log or handle cancellation
                    UoTLogger.LogErrorToFile($"FindTreeAt was canceled for tile: {tileCoord}");
                    //throw; // Propagate the cancellation exception if needed
                }
                catch (ThreadAbortException)
                {
                    UoTLogger.LogErrorToFile("Script shutting down due to ThreadAbortException.");
                    //Thread.ResetAbort(); // Prevents abort crash.
                    // Gracefully handle thread abortion
                    // Optional: Log the event or cleanup resources here
                }
                catch (Exception e)
                {
                    UoTLogger.LogErrorToFile("Error in FindTreeAt", e);
                    throw;
                }
                finally
                {
                    // Release semaphore slot to allow the next task to run
                    semaphore.Release();
                }
            }
        }
        private async Task ExecuteBatch(List<Task> tasks, CancellationToken token)
        {
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (ThreadAbortException)
            {
                UoTLogger.LogErrorToFile("Script shutting down due to ThreadAbortException.");
                //Thread.ResetAbort(); // Prevents abort crash.
                // Gracefully handle thread abortion
                // Optional: Log the event or cleanup resources here
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile("Error in FindTreeAt", e);
                throw;
            }
            finally
            {
                tasks.Clear(); // Clear the batch list after execution
            }
        }
        
        
        private IEnumerable<(int dx, int dy)> GenerateLayerOffsets(int radius) //Generate Circle Around point
        {
            if (radius == 0) yield return (0, 0); // Center point for radius 0

            // Loop through the edges of the square representing this radius
            for (int dx = -radius; dx <= radius; dx++)
            {
                yield return (dx, -radius); // Top edge
                yield return (dx, radius);  // Bottom edge
            }
            for (int dy = -radius + 1; dy <= radius - 1; dy++) // Avoid overlapping corners
            {
                yield return (-radius, dy); // Left edge
                yield return (radius, dy);  // Right edge
            }
        }
        // Main Task for tree scanning
        private async Task ScanForTrees(CancellationToken token)
        {
            const int scanInterval = 5000; // 5-second delay
            const int maxConcurrentTasks = 4; // Number of tiles to include in each batch
            using var localSemaphore = new SemaphoreSlim(maxConcurrentTasks); // Local semaphore to limit concurrency
            try
            {
                while (!UoTasks.IsCancellationRequested("Tree Scan"))
                {
                    //if (Player.Position.X == _lastQueuedPosition.X && Player.Position.Y == _lastQueuedPosition.Y)
                    if (!UoTMovement.HasPlayerMovedEnough(0,5))
                    {
                        await Task.Delay(1000, token);
                        continue;
                    }
                    const int radiusSquared = FindRadius * FindRadius;
                    // Create a list to store Task references
                    var tasks = new List<Task>();
                    
                    // Scan tiles layer-by-layer
                    for (int radius = 0; radius <= FindRadius; radius++)
                    {
                        // Only scan tiles at the edges of the current "layer"
                        foreach (var offset in GenerateLayerOffsets(radius))
                        {
                            if (UoTasks.IsCancellationRequested("Tree Scan")) return;
                            var (x, y) = (Player.Position.X + offset.dx, Player.Position.Y + offset.dy);
                            // Skip tiles outside the circular search area
                            if (offset.dx * offset.dx + offset.dy * offset.dy > radiusSquared) continue;
                            if (x < 0 || y < 0 || !IsTileWorthChecking((x,y)))
                            {
                                //look into later
                                //UoTLogger.LogErrorToFile($"Invalid tile coordinates: ({x}, {y}).");
                                continue;
                            }

                            // Add the task for this tile to the batch
                            tasks.Add(PerformSearchForTile((x,y), localSemaphore, token));

                            // If the batch size is reached, process it
                            if (tasks.Count >= maxConcurrentTasks)
                            {
                                await ExecuteBatch(tasks, token);
                            }
                        }
                    }

                    
                    // Execute any remaining tasks after the loop
                    if (tasks.Count > 0)
                    {
                        await ExecuteBatch(tasks, token);
                    }

                    // Optional: Add a small delay to avoid excessive CPU usage
                    await Task.Delay(50, token); 
                }
                await Task.Delay(scanInterval, token);
            }
            catch (NullReferenceException ex)
            {
                // Handle the specific "Object reference not set to an instance of an object" error
                UoTLogger.LogErrorToFile($"NullReferenceException caught scan: {ex.Message}.");
                // Optionally, you can log the stack trace as well for debugging
                throw;
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    UoTLogger.LogErrorToFile(inner.Message, inner);
                }
                throw;
            }
            catch (OperationCanceledException)
            {
                // Log or handle cancellation
                UoTLogger.LogErrorToFile($"ScanForTrees");
                //throw;
                //throw; // Propagate the cancellation exception if needed
            }
            catch (ThreadAbortException)
            {
                UoTLogger.LogErrorToFile("Script shutting down due to ThreadAbortException.");
                //Thread.ResetAbort(); // Prevents abort crash.
                // Gracefully handle thread abortion
                // Optional: Log the event or cleanup resources here
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile("Error in ScanForTrees", e);
                throw;
            }
        }

        private static int _lastCount;
        private static (int X, int Y) _lastTree;
        private static Stopwatch _treeTimer = new Stopwatch();
        private static Tree TreeToChop(CancellationToken token = default)
        {
            if (_treesToChop.IsEmpty) return null;
            if (_lastCount != _treesToChop.Count())
            {
                UoTSystem.CheckTimer(Player.Serial, "Tree's in queue: " + _treesToChop.Count(), "", UoTSystem.MessageOutput.None, 33, 0);
                _lastCount = _treesToChop.Count();
            }
            
            // Find the closest tree by calculating the distance to the player
            var treesCachedKeys = _treesToChop.Keys;
            var closestTreeKey = treesCachedKeys
                .OrderBy(treeCoord => Misc.Distance(Player.Position.X, Player.Position.Y, treeCoord.Item1,
                    treeCoord.Item2))
                .FirstOrDefault();
            if (_treesToChop == null || !_treesToChop.TryRemove(closestTreeKey, out var closestTree)) return null;
            if (closestTree == null) return null;
            if (closestTree.X != _lastTree.X || closestTree.Y != _lastTree.Y)
            {
                //Misc.SendMessage($"Chopping tree at: {closestTree.X}, {closestTree.Y}", 33);
                _treeTimer.Reset();
                _treeTimer.Start();
                _lastTree = (closestTree.X, closestTree.Y);
            }
            return closestTree;
        }

        //return tree if you want the tree, false if you are done with tree.
        private static bool ChopTree(Tree tree, CancellationToken token = default)
        {
            if (tree == null) return false;
            _treeTimer.Stop();
            try
            {
                Chop(tree, token);
            }
            catch (Exception e)
            {
                Misc.SendMessage(e);
            }
            _treeTimer.Start();
            return !ChopIgnore(tree, token);
        }
        private static void Chop(Tree tree, CancellationToken token = default)
        {
            if (tree == null) return;
            if (Misc.Distance(Player.Position.X, Player.Position.Y, tree.X, tree.Y) > 1) return;
            Misc.Pause(600);
            Misc.Resync();
            Misc.Pause(150);
            if (UoTasks.IsTaskRunning("Pathing") && UoTasks.StopTask("Pathing")) 
                UoTLogger.LogErrorToFile("Pathing stopped in ChopTheTree");
            Misc.Pause(600);
            var hatchet = UoTItems.FindAndEquipWeapon(UoTools.Axes, false);
            hatchet = Player.GetItemOnLayer("LeftHand");
            if (hatchet == null)
            { 
                UoTLogger.LogErrorToFile("No hatchet found");
                Player.HeadMessage(33, "No hatchet found");
                Misc.Pause(600);
                return;
            }
            if (Target.HasTarget()) Target.Cancel(); //Attempt to stop target lock
            //UoTSystem.CheckTimer(Player.Serial, "gatherWood", "Chop Chop", UoTSystem.MessageOutput.Player, 33, 1000);
            Target.TargetResource(hatchet, 2);
            //Items.UseItem(hatchet);
            //Target.WaitForTarget(2000);
            //Target.TargetExecute(tree.X, tree.Y, tree.Z, tree.StaticId);
        }

        private static bool ChopIgnore(Tree tree, CancellationToken token = default)
        {
            if (_treeTimer.Elapsed.Seconds > 15) // Change ignore timeout
            {
                IgnoreTree(tree, 2, token); //change penalty
                if (UoTasks.StopTask("Pathing")) Misc.SendMessage("Pathing stopped due to ignored POI");
                return true;
            }
            if ( Misc.Distance(Player.Position.X, Player.Position.Y, tree.X, tree.Y) > 30) 
            {
                //IgnoreTree(tree, 2, token); //change penalty;
                return true;
            }
            if (UoTSystem.SearchJournal(UoTItems.journal, "t use an axe on that"))
            {
                IgnoreTree(tree, PurgeAfterMins, token);
                Misc.Pause(2000);
                UoTItems.journal.Clear("t use an axe on that");
                return true;
            }
            if (UoTSystem.SearchJournal(UoTItems.journal, "not enough wood here to harvest"))
            {
                IgnoreTree(tree, PurgeAfterMins, token);
                Misc.Pause(2000);
                UoTItems.journal.Clear("not enough wood here to harvest");
                return true;
            }
            return false;
        }

        private static void IgnoreTree(Tree tree, int duration, CancellationToken token = default)
        {
            var treeCoord = (tree.X, tree.Y);
            var _ = DateTime.Now.AddMinutes(duration);
            _ignoredTrees?.AddOrUpdate((treeCoord.X, treeCoord.Y),
                _,
                (key, oldValue) => oldValue > DateTime.Now
                    ? oldValue.AddMinutes(duration) // Extend the current expiration
                    : _); // Set a fresh expiration
            UoTSystem.CheckTimer(Player.Serial,
                "Adding tree to ignored list:" + tree.X + ":" + tree.Y + " for " + duration + " minutes", "",
                UoTSystem.MessageOutput.None);
        }
    }
}
