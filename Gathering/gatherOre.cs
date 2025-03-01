using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using IronPython.Compiler;
using Newtonsoft.Json;



//#assembly <IronPython.dll>
//#assembly <IronPython.Modules.dll>
//#import <../Common/Bod.cs>
//#import <../Common/BodData.cs>
//#import <../Common/Gumps.cs>
//#import <../Common/Items.cs>
//#import <../Common/Journal.cs>
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

namespace RazorEnhanced
{
    public class GatherOre
    {
        
        private static Journal _journal = new();
        private const int MaxWeightRange = 100; // How far from max before smelting
        private const int PurgeAfterMins = 10; // Remove node from depleted list after N minutes
        private const int FindRadius = 16; // Radius around player to search for ore
        private static UoTiles.MiningDatabase _depletedOre;
        private static (int x, int y, int z) _lastLocationMined;
        
        public void Run()
        {
            try
            {
                var mainToken = UoTasks.GetTaskCancellationToken();
                InitilizeDatabases(mainToken);
                RunMiningLoop(mainToken);
            }
            catch (ThreadAbortException)
            {
                UoTLogger.LogErrorToFile("ThreadAbortException in Run().");
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile(e.Message, e);
                //throw;
            } 
            finally
            {
                SyncFeluccaTrammel();

                var mapCount = typeof(UoTiles.MapType)
                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .Count();
                for (var i = 0; i < mapCount; i++)
                { 
                    UoTiles.MapType map = i;
                    using var storedData = new UoTJson("Ore_" + Misc.ShardName() + "-" + map.ToName());
                    if (UoTiles.OreNodeLists[i] != null && UoTiles.OreNodeLists[i].Count > 1)
                        storedData.StoreData(UoTiles.OreNodeLists[map], "Ore_" + Misc.ShardName() + "-" + map.ToName(),
                            UoTJson.StoreType.Server);
                }
                using (var storedData = new UoTJson("Ore_depleted"))
                {
                    storedData.StoreData(_depletedOre, "OreMaps", UoTJson.StoreType.Server);
                }
            }
        }
 
        private static void InitilizeDatabases(CancellationToken token = default)
        {
            var mapCount = typeof(UoTiles.MapType)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Count();
            
            UoTJson.FileName = "gatherOre";
            
            using (var storedData = new UoTJson("Ore_depleted"))
            { 
                _depletedOre = storedData.GetData<UoTiles.MiningDatabase>(
                    "OreMaps", typeof(UoTiles.MiningDatabase), UoTJson.StoreType.Server);
                if (_depletedOre == null) _depletedOre = new UoTiles.MiningDatabase();
                if (_depletedOre.Maps == null) _depletedOre.Maps = new Dictionary<int, UoTiles.Map>();
                
                for (var i = 0; i < mapCount; i++)
                {
                    if (!_depletedOre.Maps.ContainsKey(i))
                    {
                        _depletedOre.Maps.Add(i, new UoTiles.Map());
                    }
                }
                storedData.StoreData(_depletedOre, "OreMaps", UoTJson.StoreType.Server);
            }

            foreach (var entry in _journalEntries)
            {
                UoTItems.journal.Clear(entry);
            }
        }
        private static HashSet<string> _processedEntries = new HashSet<string>();

        private static void PrintNewJournalEntries()
        {
            var entry = UoTItems.journal.GetJournalEntry(DateTimeOffset.UtcNow.AddHours(-8).AddSeconds(5).ToUnixTimeSeconds()
            );
            if (entry != null && !_processedEntries.Contains(entry.ToString()))
            {
                Misc.SendMessage($"New Journal Entry: {entry}");
                _processedEntries.Add(entry.ToString());
            }
    
            Thread.Sleep(10);  // Add small delay to prevent excessive CPU usage
        
        }

        private static void RunMiningLoop(CancellationToken token = default)
        {
            try
            {
                (int X, int Y, int Z) closestLocation = default;
                while (!token.IsCancellationRequested)
                {
                    BackupData(token);
                    //PrintNewJournalEntries();
                    UoTMovement.PlayerMoved(); //Update last move time
                    if (closestLocation == default)
                        closestLocation = FindClosestValidLocation(Player.Map);
                    if (ShouldSkipLocation(closestLocation, Player.Map)) closestLocation = default;
                    if (closestLocation != default)
                    {
                        if (Misc.Distance(Player.Position.X, Player.Position.Y,
                                closestLocation.X, closestLocation.Y) > FindRadius)
                            closestLocation = default;
                        UoTSystem.CheckTimer(0, "Distance to node: " + Misc.Distance(Player.Position.X, Player.Position.Y,  closestLocation.X, closestLocation.Y), "", UoTSystem.MessageOutput.None);
                    }
                    //use seperate stopwatches for idle, local starts pathing and movement in pathing
                    UoTMovement.PlayerMovementStopwatch.Start();
                    if (!MineAttempt(token) && closestLocation != default)
                    {
                        //UoTSystem.CheckTimer(0, "Pathing to node", "", UoTSystem.MessageOutput.Misc);
                        UoTMovement.PathingTask(closestLocation.X, closestLocation.Y, closestLocation.Z, 0, token);
                    }
                    UoTMovement.PlayerMovementStopwatch.Stop();
                    CheckPlayerOverloaded(token);
                    PurgeExpiredEntries(token);
                    while (UoTSystem.CheckTimer(0, "Slowdown", "", UoTSystem.MessageOutput.None, 33, 200))
                        Misc.Pause(10);
                }
            }
            catch (ThreadAbortException)
            {
                UoTLogger.LogErrorToFile($"ThreadAbortException in RunMiningLoop.");
                //Thread.ResetAbort(); // Prevents abort crash.
                throw;
            }
            catch (OperationCanceledException)
            {
                UoTLogger.LogErrorToFile("RunMiningLoop was canceled.");
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile($"Unexpected error in mining loop: {e.Message}", e);
            }
            finally
            {
                UoTMovement.PlayerMovementStopwatch.Stop();
            }
            
        }
        private static (int X, int Y, int Z) _lastDepletedLocation = default;
 
        private static Item EquipPickaxe(CancellationToken token = default)
        {
            try
            {
                //Items.ContainerCount(Player.Backpack, 0x1EB8, -1)
                var pickaxe = UoTItems.FindAndEquipWeapon(UoTools.Pickaxes, true, false);
                if (Player.GetSkillValue("Tinkering") > 30)
                { 
                    if (Items.FindAllByID(UoTools.Pickaxes, -1, Player.Backpack.Serial, -1).Count == 0)
                        SmeltWithBeetle(token);
                    while (Items.FindAllByID(UoTools.Pickaxes, -1, Player.Backpack.Serial, -1).Count == 0 &&
                           Items.ContainerCount(Player.Backpack, 0x1BF2, 0) > 3) //move to item and support crafting other hues
                    {
                        //Misc.SendMessage("Ingots: " + Items.ContainerCount(Player.Backpack, 0x1BF2));
                        Misc.Pause(1000);
                        UoTItems.CreatePickaxe();
                        Misc.Pause(1000);
                    }
                    if (pickaxe == null) pickaxe = UoTItems.FindAndEquipWeapon(UoTools.Pickaxes, true, false);
                }
                if (pickaxe == null)
                {
                    UoTLogger.LogErrorToFile("No pickaxe found");
                    Misc.SendMessage("No pickaxe found");
                    Player.HeadMessage(33, "No pickaxe found");
                    Misc.Pause(600);
                    MoveOreToStorage(token); // Go restock
                    return null;
                }
                return pickaxe;
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile(e.Message, e);
            }
            return null;
        }

        private static bool ClearJournalEntry(string entry)
        {
            UoTItems.journal.Clear(entry);
            return true;
        }
        private static List<string> _journalEntries = new List<string>
        {
            "You have moved too far away to continue mining",
            "You can't mine there.",
            "Target cannot be seen.",
            "is no metal here to mine.",
            "ore and put it in your backpack",
            "you loosen some rocks but fail to find any usable ore",
            "you cannot mine" //while standing inside a house
        };
        private static int _attemptCount = 0;
        private static bool MineAttempt(CancellationToken token = default)
        {
            try
            {
                UoTMovement.PlayerMovementStopwatch.Stop();
                if (Player.Mount != null)  Mobiles.UseMobile(Player.Serial);
                var pickaxe = EquipPickaxe(token);
                if (Target.HasTarget()) Target.Cancel();
                if (_lastDepletedLocation == (Player.Position.X, Player.Position.Y, Player.Position.Z)) return false;
                _attemptCount++;
                Target.TargetResource(pickaxe, "ore");
                var entry = UoTItems.journal.WaitJournal(_journalEntries, 100);
                if (!string.IsNullOrEmpty(entry))
                {
                    if (entry.Contains("You can't mine there.") ||
                        entry.Contains("Target cannot be seen.") ||
                        entry.Contains("is no metal here to mine.") ||
                        entry.Contains("you cannot mine"))
                    {
                        if (_attemptCount > 3)
                        {
                            UoTSystem.CheckTimer(0, "", "Keep looking", UoTSystem.MessageOutput.Misc, 63, 13500);
                            UpdateLastMinedTime((Player.Position.X, Player.Position.Y, Player.Position.Z));
                            _attemptCount = 0;
                            _lastDepletedLocation = (Player.Position.X, Player.Position.Y, Player.Position.Z);
                            QuickSmeltWithBeetle(token);
                            return false;
                        }
                        UoTItems.journal.Clear(entry);
                        return true;
                    } 

                    if (entry.Contains("ore and put it in your backpack") ||
                        entry.Contains("you loosen some rocks but fail to find any useable ore"))
                    {
                        //UoTSystem.CheckTimer(0, "", "Found ore", UoTSystem.MessageOutput.Misc, 33, 3500);
                        UoTasks.StopTask("Pathing");
                        UoTMovement.ClearPointOfInterest();
                        FoundNode((Player.Position.X, Player.Position.Y, Player.Position.Z));
                        UoTItems.journal.Clear(entry);
                        return true;
                    }
                    if (entry.Contains("You have moved too far away to continue mining"))
                    {
                        Player.HeadMessage(33, "Stop to mark node!");
                        UoTItems.journal.Clear(entry);
                        var loc = Player.Position;
                        UoTasks.StopTask("Pathing");
                        UoTMovement.ClearPointOfInterest();
                        Misc.Pause(200);
                        for (var i = 0; i < 1000; i++)
                        {
                            if (!UoTasks.IsTaskRunning("Pathing")) break;
                            Misc.Pause(10);
                        }

                        if (MineAttempt()) return true;
                        if (MineAttempt()) return true;
                        Misc.Pause(600);
                        var direction = UoTMovement.GetDirectionToTarget(loc.X, loc.Y);
                        var (newX, newY) = UoTMovement.GetCoordinateFromDirection(loc.X, loc.Y, direction);
                        UoTMovement.PathingTask(newX, newY, loc.Z, 0, token);

                        Misc.Pause(200);
                        for (var i = 0; i < 1000; i++)
                        {
                            if (!UoTasks.IsTaskRunning("Pathing")) break;
                            Misc.Pause(10);
                        }
                        Misc.Pause(200);
                        MineAttempt();
                        Misc.Pause(600);
                        MineAttempt();
                        UoTItems.journal.Clear(entry);
                        return true;
                    }
                }

                return true;

            }
            catch (ThreadAbortException)
            {
                UoTLogger.LogErrorToFile($"ThreadAbortException in MineAttempt.");
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile(e.Message, e);
                //throw;
            }
            finally
            {
                UoTMovement.PlayerMovementStopwatch.Start();
            }

            return false;
        }

        private static void FoundNode((int X, int Y, int Z) location)
        {
            var currentMap = Player.Map;
            UoTiles.OreNodeLists[Player.Map].Add(_lastLocationMined);
            // Initialize map if it doesn't exist
            if (!_depletedOre.Maps.ContainsKey(currentMap))
            {
                _depletedOre.Maps.Add(currentMap, new UoTiles.Map());
            } 
            // Add or update the location with current time
            _depletedOre.Maps[currentMap].Locations[location] = DateTime.Now + TimeSpan.FromMinutes(45);
        } 
        private static void UpdateLastMinedTime((int X, int Y, int Z) loc)
        {
            var currentMap = Player.Map; 
            var depletedTime = DateTime.Now.AddMinutes(PurgeAfterMins);
    
            // Initialize map if it doesn't exist
            if (!_depletedOre.Maps.ContainsKey(currentMap))
            {
                _depletedOre.Maps.Add(currentMap, new UoTiles.Map());
            } 
            
            if (UoTiles.OreNodeLists[currentMap].Contains(loc))
                _depletedOre.Maps[currentMap].Locations[loc] = depletedTime;
            
            /*// Update the center location and all 8 surrounding tiles if they exist
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    var location = (
                        X: loc.X + xOffset,
                        Y: loc.Y + yOffset,
                        Z: loc.Z
                    );
            
                    // Only update if the location already exists in the dictionary
                    if (UoTiles.OreNodeLists[currentMap].Contains(location))
                        _depletedOre.Maps[currentMap].Locations[location] = depletedTime;

                }
            }*/
        }
        private static Mobile _smeltingBeetle;
        private static bool SmeltWithBeetle(CancellationToken token = default)
        {
            if (_smeltingBeetle == null) _smeltingBeetle = UoTMobiles.FindSmeltingBeetle();
            var beetle = _smeltingBeetle;
            if (beetle == null) return false;
            //UoTSystem.CheckTimer(0, "Found Beetle! Smelting with " + beetle.Name, "", UoTSystem.MessageOutput.Misc);
            var smelted = false;
            List<Item> ores = null; 
            if (Target.HasTarget()) Target.Cancel();
            Target.ClearQueue();
            ores = Items.FindAllByID(UoTLists.OreChunks, -1, Player.Backpack.Serial, 0, false);
            var singleOres = new List<Item>();
            foreach (var ore in ores)
            {
                //Eventine
                if (ore.Hue == 0x08ab) continue;
                if (ore.ItemID == 0x19B9 && ore.Hue == 0x0979)
                {
                    continue;
                }
                if (ore.ItemID == 0x19B7 && ore.Amount == 1)
                {
                    singleOres.Add(ore);
                    continue;
                }
                if (ore.ItemID == 0x19B8 && ore.Hue == 0x0965)
                {
                    continue;
                }
                Items.UseItem(ore);
                Target.WaitForTarget(2000);
                Target.TargetExecute(beetle);
                Misc.Pause(150);
                smelted = true;
            }
            foreach (var ore in singleOres)
            {
                if (ore == null)
                    continue;

                foreach (var ore2 in singleOres)
                {
                    if (ore2 == null || ore.Serial == ore2.Serial)
                        continue;

                    if (ore.Hue == ore2.Hue)
                    {
                        Items.Move(ore2, ore, -1);
                        Misc.Pause(250);
                    }
                }
            }
            singleOres.Clear();

            if (Target.HasTarget()) Target.Cancel();
            Target.ClearQueue();
            ores = Items.FindAllByID(UoTLists.OreChunks, -1, Player.Backpack.Serial, 0, false);
            foreach (var ore in ores)
            {
                if (ore.ItemID == 0x19B7 && ore.Amount == 1) continue;
                Items.UseItem(ore);
                Target.WaitForTarget(2000);
                Target.TargetExecute(beetle);
                Misc.Pause(150);
                smelted = true;
            }
            return smelted;
        }
        private static bool QuickSmeltWithBeetle(CancellationToken token = default)
        {
            if (_smeltingBeetle == null) _smeltingBeetle = UoTMobiles.FindSmeltingBeetle();
            var beetle = _smeltingBeetle;
            if (beetle == null) return false;
            //UoTSystem.CheckTimer(0, "Found Beetle! Smelting with " + beetle.Name, "", UoTSystem.MessageOutput.Misc);
            var smelted = false;
            List<Item> ores = null; 
            if (Target.HasTarget()) Target.Cancel();
            Target.ClearQueue();
            ores = Items.FindAllByID(UoTLists.OreChunks, -1, Player.Backpack.Serial, 0, false);
            var singleOres = new List<Item>();
            foreach (var ore in ores)
            {
                //Eventine
                if (ore.Hue == 0x08ab) continue;
                if (ore.ItemID == 0x19B9 && ore.Hue == 0x0979)
                {
                    continue;
                }
                if (ore.ItemID == 0x19B8 && ore.Hue == 0x0965)
                {
                    continue;
                }
                if (ore.ItemID == 0x19B7 && ore.Amount == 1)
                {
                    singleOres.Add(ore);
                    continue;
                }
                Items.UseItem(ore);
                Target.WaitForTarget(2000);
                Target.TargetExecute(beetle);
                Misc.Pause(250);
                smelted = true;
            }
            return smelted;
        }

        private static void BreakRocks(CancellationToken token = default)
        {
            for (var i = 0; i < 10; i++)
            {
                token.ThrowIfCancellationRequested();
                var rocks = UoTItems.FindItems(0x4B4B, Player.Backpack, null, false);
                rocks.AddRange(UoTItems.FindItems(0x4B4F, Player.Backpack, null, false));
                var hatchetExists = Items.FindByID(UoTools.Axes, -1, Player.Backpack.Serial);
                if (hatchetExists != null && rocks.Any())
                {
                    Misc.SendMessage("Rocks to chop:" + rocks.Count);
                    foreach (var rock in rocks)
                    {
                        if (rock == null) continue;
                        Items.UseItem(hatchetExists);
                        Target.WaitForTarget(2000);
                        Target.TargetExecute(rock);
                        Misc.Pause(350);
                    }
                }
                else break;
            }
        }
        private static bool CheckPlayerOverloaded(CancellationToken token = default)
        {
            try
            {
                UoTMovement.PlayerMovementStopwatch.Stop();
                BreakRocks(token);
                if (Player.Weight < Player.MaxWeight - MaxWeightRange) return false;
                if (Player.WarMode) Player.SetWarMode(false);
                QuickSmeltWithBeetle(token);
                if (SmeltWithBeetle(token) && Player.Weight < Player.MaxWeight - MaxWeightRange) return false;
                
                
                if (Player.Weight > Player.MaxWeight - MaxWeightRange)
                {
                    MoveOreToStorage();
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
                UoTMovement.PlayerMovementStopwatch.Start();
            }
        }
        private static void MoveOreToStorage(CancellationToken token = default)
        {
            try
            {
                Player.HeadMessage(33, "Recalling to drop stuff off");
                Misc.SetSharedValue("Lootmaster:Pause", true);
                UoTasks.StopTask("Pathing");
                UoTMovement.ClearPointOfInterest();
                SmeltWithBeetle(token);
                Misc.Pause(600);
                _smeltingBeetle ??= UoTMobiles.FindSmeltingBeetle();
                var beetle = _smeltingBeetle;
                if (beetle != null) Mobiles.UseMobile(beetle);
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
                if (Player.Bank != null)
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
                        }
                    }
                }
                if (chest == null) return;
                if (Player.Bank == null || !Player.Bank.ContainerOpened)
                {
                    UoTMovement.PathingTask(chest.Position.X, chest.Position.Y, default, 2, token);
                } 
                
                Misc.SetSharedValue("Lootmaster:Pause", true);
                //update lists to check for hue
                UoTItems.MoveItemsToContainer(UoTLists.MiningDeposits, chest.Serial, token);
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
                Misc.Pause(5000);
                UoTPlayer.RecallWithBook(1, UoTPlayer.UpdateRecallRuneIndex());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                Misc.SetSharedValue("Lootmaster:Pause", false);
            }
        }
        private static void BackupData(CancellationToken token = default)
        {
            if (!UoTSystem.CheckTimer(Player.Serial, "Backing up data", "", UoTSystem.MessageOutput.None, 33,
                    10000))
            {
                PurgeExpiredEntries(token);
                //SyncFeluccaTrammel(token);
                
                var mapCount = typeof(UoTiles.MapType)
                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .Count();
                for (var i = 0; i < mapCount; i++)
                {
                    UoTiles.MapType map = i;
                    using (var storedData = new UoTJson("Ore_"+ Misc.ShardName() + "-" + map.ToName()))
                    { 
                        if (UoTiles.OreNodeLists[i] != null && UoTiles.OreNodeLists[i].Count>1) 
                            storedData.StoreData(UoTiles.OreNodeLists[map],  "Ore_" + Misc.ShardName() + "-" + map.ToName(), UoTJson.StoreType.Server);
                    }
                }
                using (var storedData = new UoTJson("Ore_depleted"))
                {
                    storedData.StoreData(_depletedOre, "OreMaps", UoTJson.StoreType.Server);
                }
            }
        }
        private static void PurgeExpiredEntries(CancellationToken token = default)
        {
            var currentTime = DateTime.Now;

            // Process each map
            foreach (var map in _depletedOre.Maps)
            {
                token.ThrowIfCancellationRequested();
                // Create a list to store keys that need to be removed
                var expiredLocations = map.Value.Locations
                    .Where(kvp => kvp.Value <= currentTime)
                    .Select(kvp => kvp.Key)
                    .ToList();

                // Remove all expired locations
                foreach (var location in expiredLocations)
                {
                    token.ThrowIfCancellationRequested();
                    map.Value.Locations.Remove(location);
                }
            }
        }

        public static void SyncFeluccaTrammel(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            // Get all unique locations from both maps
            var allLocations = UoTiles.OreNodeLists[0].Union(UoTiles.OreNodeLists[1]);
            // Update both maps with all known locations
            UoTiles.OreNodeLists[0].UnionWith(allLocations);
            UoTiles.OreNodeLists[1].UnionWith(allLocations);
        }
        public static (int X, int Y, int Z) FindClosestValidLocation(int currentMap, int playerX = Int32.MinValue, int playerY = Int32.MinValue, int playerZ = Int32.MinValue, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (playerX == Int32.MinValue) playerX = Player.Position.X;
            if (playerY == Int32.MinValue) playerY = Player.Position.Y;
            if (playerZ == Int32.MinValue) playerZ = Player.Position.Z;

            if (UoTiles.OreNodeLists.Count == 0)
            {
                UoTLogger.LogErrorToFile("Ore node file empty!");
                Misc.SendMessage("Ore node file empty!");
                return default;
            }

            //Misc.SendMessage(UoTiles.OreNodeLists[currentMap].Count() + " nodes");
            var now = DateTime.Now;
            var oreLocations = UoTiles.OreNodeLists[currentMap];
            // Check player's position first
            var centerLocation = (playerX, playerY, playerZ);
            if (oreLocations.Contains(centerLocation))
            {
                if (_depletedOre.Maps?[currentMap].Locations.TryGetValue(centerLocation, out var expiryTime) != true || 
                    expiryTime <= now)
                { 
                    return centerLocation;
                }
            }

            (int,int,int) firstLocation = default;
            // Search in expanding squares, starting from closest to player
            for (int radius = 1; radius <= FindRadius; radius++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        token.ThrowIfCancellationRequested();
                        var location = (playerX + dx, playerY + dy, playerZ);

                        // Check if this is a possible ore location
                        if (!oreLocations.Contains(location))
                            continue;

                        // Check if node is depleted
                        if (_depletedOre.Maps?[currentMap].Locations.TryGetValue(location, out var expiryTime) == true && 
                            expiryTime > now)
                            continue;

                        //UoTMovement.AddPointOfInterest(location.Item1, location.Item2, location.playerZ);
                        if (firstLocation == default) firstLocation = location;

                    }
                }
            }
            return firstLocation;
        }

        private static bool ShouldSkipLocation((int X, int Y, int Z) location, int currentMap)
        {
            if(!_depletedOre.Maps.ContainsKey(currentMap))
                return false;

            if (!_depletedOre.Maps[currentMap].Locations.TryGetValue(location, out var lastMined))
                return false;

            // Skip if location is within the lockout period
            return lastMined > DateTime.Now;
        }
        
    }
}