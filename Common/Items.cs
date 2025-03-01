using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace RazorEnhanced
{
    public class UoTItems
    {
        public static readonly Target target;
        public static readonly Journal journal;
        
        static UoTItems()
        {
            target = new Target();
            journal = new();
        }
        
        private const int DELAY_USE_ITEM_MS = 600;

        
        
        public static readonly Dictionary<string, Dictionary<string, int>> CraftingTools = new()
        {
            ["Alchemy"] = new Dictionary<string, int>
            {
				["mortar and pestle"] = 0x0E9B
			},
            ["Blacksmithing"] = new Dictionary<string, int>
            {
                ["smith's hammer"] = 0x13E3,
                ["sledge hammer"] = 0x0FB5,
                ["tongs"] = 0x0FBB
            },
            ["Bowcraft and Fletching"] = new Dictionary<string, int>
            {
                ["fletcher's tools"] = 0x1022,
                ["arrow fletching"] = 0x1022
            },
            ["Carpentry"] = new Dictionary<string, int>
            {
                ["saw"] = 0x1034,
                ["draw_knife"] = 0x10E4,
                ["froe"] = 0x10E5,
                ["inshave"] = 0x10E6,
                ["scorp"] = 0x10E7,
                ["dovetail_saw"] = 0x1028,
                ["hammer"] = 0x102A
            },
            ["Cartography"] = new Dictionary<string, int>
            {
                ["mapmaker's pen"] = 0x0FBF
            },
            ["Cooking"] = new Dictionary<string, int>
            {
                ["skillet"] = 0x097F,
                ["flour sifter"] = 0x103E,
                ["rolling pin"] = 0x1043
            },
            ["Inscription"]  = new Dictionary<string, int>
            {
                ["scribe's pen"] = 0x0FBF
            },
            ["Tailoring"] = new Dictionary<string, int>
            {
                ["sewing kit"] = 0x0F9D
            },
            ["Tinkering"] = new Dictionary<string, int>
            {
                ["tinker's tools"] = 0x1EBC,
                ["tool kit"] = 0x1EB8
            }
        };
      
        
        
        /// <summary>
        /// Returns a list of all items from a list of the itemIDs. It can look recursiverly inside a container
        /// </summary>
        /// <param name="itemIDs"> List of itemsID</param>
        /// <param name="container"> Container where look into</param>
        /// <param name="itemProperties"> Finds specific items</param>
        /// <param name="recursive"> Look recursively into containers inside the main container</param>
        /// <param name="stopAtFirst"> Returns only the first found. Optimization if you need only one item</param>
        /// <param name="ignoreItems"> Ignores this item</param>
        /// <returns>List of found Items</returns>
        public static List<Item> FindItems(List<int> itemIDs, Item container, List<string> itemProperties = null,
            bool recursive = true, bool stopAtFirst = false, List<int> ignoreItems = null)
        {
            var itemList = new List<Item>();

            foreach (var item in container.Contains.Where(item => itemIDs.Contains(item.ItemID)))
            { 
                if (ignoreItems != null && ignoreItems.Contains(item.Serial)) continue;
                //Items.WaitForProps(item.Serial, delayWaitForProprs);
                if (itemProperties == null || itemProperties.Count == 0)
                {
                    itemList.Add(item);
                    if (stopAtFirst)
                    {
                        return itemList;
                    }
                }
                else
                {
                    var stop = false;
                    foreach (var prop in item.Properties.Where(
                                 prop => itemProperties.Any(
                                     itemProperty => prop.ToString().ToLower().Contains(
                                         itemProperty.ToLower()))))
                    {
                        itemList.Add(item);
                        if (stopAtFirst) stop = true;
                    } 
                    if (stop) break;
                }
            }

            if (recursive)
            {
                var subcontainers = container.Contains.Select(
                        sublist => sublist).Where( item => item.IsContainer)
                        .ToList();

                foreach (var bag in subcontainers.Where(bag => bag.ItemID != 0x2259))
                {
                    Items.UseItem(bag);
                    Misc.Pause(DELAY_USE_ITEM_MS);
                    var itemInSubContainer = FindItems(itemIDs, bag, itemProperties, true, stopAtFirst);
                    itemList.AddRange(itemInSubContainer);
                    if (stopAtFirst)
                    {
                        return itemList;
                    }
                }
            }

            return itemList;
        }

        /// <summary>
        /// Returns a list of all items with the provided itemID. It can look recursiverly inside a container
        /// </summary>
        /// <param name="itemId">ItemID of the item you want look for</param>
        /// <param name="container"> Container where look into</param>
        /// <param name="itemProperties"> Finds specific items</param>
        /// <param name="recursive"> Look recursively into containers inside the main container</param>
        /// <param name="stopAtFirst"> Returns only the first found. Optimization if you need only one item</param>
        /// <param name="ignoreItems"> Ignores this item</param>
        /// <returns>List of found Items</returns>
        public static List<Item> FindItems(int itemId, Item container, List<string> itemProperties = null,
            bool recursive = true, bool stopAtFirst = false, List<int> ignoreItems = null)
        {
            return FindItems(new List<int>() { itemId }, container, itemProperties, recursive, stopAtFirst, ignoreItems);
        }
        /// <summary>
        /// Returns a list of all items from a list of the itemIDs. It can look recursiverly inside a container
        /// </summary>
        /// <param name="itemIdsWithColor">ItemID and ItemColor of the items you want</param>
        /// <param name="container"> Container where look into</param>
        /// <param name="itemProperties"> Finds specific items</param>
        /// <param name="recursive"> Look recursively into containers inside the main container</param>
        /// <param name="stopAtFirst"> Returns only the first found. Optimization if you need only one item</param>
        /// <param name="ignoreItems"> Ignores this item</param>
        /// <returns>List of found Items</returns>
        public static List<Item> FindItems(List<(int, int)> itemIdsWithColor, Item container, List<string> itemProperties = null,
            bool recursive = true, bool stopAtFirst = false, List<int> ignoreItems = null)
        {
            var itemList = new List<Item>();
            foreach (var item in container.Contains)
            {
                if (ignoreItems != null && ignoreItems.Contains(item.Serial)) continue;
                if (!itemIdsWithColor.Contains((item.ItemID, item.Color))) continue;

                //Items.WaitForProps(item.Serial, delayWaitForProprs);
                if (itemProperties == null || itemProperties.Count == 0)
                {
                    itemList.Add(item);
                    if (stopAtFirst)
                    {
                        return itemList;
                    }
                }
                else
                {
                    var stop = false;
                    foreach (var prop in item.Properties)
                    {
                        foreach (var itemProperty in itemProperties.Where(
                                     itemProperty => prop.ToString().ToLower().Contains(itemProperty)))
                        {
                            itemList.Add(item);
                            if (stopAtFirst) stop = true;
                        }
                    }
                    if (stop) break;
                }
            }

            if (recursive)
            {//(itemIdsWithColor.Contains((item.ItemID,item.Color)))
                var subcontainers 
                    = container.Contains.Select(sublist => sublist).
                        Where(item => item.IsContainer)
                    .ToList();

                foreach (var bag in subcontainers)
                {
                    // If is a Book of BOD skip
                    if (bag.ItemID == 0x2259)
                    {
                        continue;
                    }

                    Items.UseItem(bag);
                    Misc.Pause(DELAY_USE_ITEM_MS);
                    var itemInSubContainer = FindItems(itemIdsWithColor, bag, itemProperties, true, stopAtFirst);
                    itemList.AddRange(itemInSubContainer);
                    if (stopAtFirst)
                    {
                        return itemList;
                    }
                }
            }

            return itemList;
        }

        /// <summary>
        /// Returns a list of all items with the provided itemID. It can look recursiverly inside a container
        /// </summary>
        /// <param name="itemIdWithColor">ItemID and ItemColor of the item you want</param>
        /// <param name="container"> Container where look into</param>
        /// <param name="itemProperties"> Finds specific items</param>
        /// <param name="recursive"> Look recursively into containers inside the main container</param>
        /// <param name="stopAtFirst"> Returns only the first found. Optimization if you need only one item</param>
        /// <param name="ignoreItems"> Ignores this item</param>
        /// <returns>List of found Items</returns>
        public static List<Item> FindItems((int, int) itemIdWithColor, Item container, List<string> itemProperties = null,
            bool recursive = true, bool stopAtFirst = false, List<int> ignoreItems = null)
        {
            var tuple = new List<(int,int)> { itemIdWithColor};
            return FindItems(tuple, container, itemProperties, recursive, stopAtFirst, ignoreItems);
        }

        /// <summary>
        /// Looks for all not exceptional items with itemID in player backpack. It's not a recursive find.
        /// </summary>
        /// <param name="itemID">ItemID of the items to be found</param>
        /// <returns>List of all not exceptional items</returns>
        public static List<Item> FindItemsNotExceptionalInBackpack(int itemID)
        {
            List<Item> itemList = new List<Item>();

            foreach (Item item in Player.Backpack.Contains)
            {
                if (item.ItemID == itemID)
                {
                    //Items.WaitForProps(item.Serial, delayWaitForProprs);
                    bool isExceptional = false;
                    foreach (Property prop in item.Properties)
                    {
                        if (prop.ToString().ToLower().Contains("exceptional"))
                        {
                            isExceptional = true;
                            break;
                        }
                    }

                    if (!isExceptional)
                    {
                        itemList.Add(item);
                    }
                }
            }

            return itemList;
        }

        public static Item FindItemAnywhere(string itemName)
        {
	        return FindItemsAnywhere(new List<string>() { itemName }).FirstOrDefault();
        }
        public static Item FindItemAnywhere(int itemId)
        {
	        return FindItemsAnywhere(new List<int>() { itemId }).FirstOrDefault();
        }
        public static List<Item> FindItemsAnywhere(List<string> itemNames)
        {
	        var foundItems = new List<Item>();
	        try
	        {
		        foreach (var itemName in itemNames)
		        {
			        Items.Filter itemFilter = new()
			        {
				        Enabled = true
			        };
			        //foreach (var itemID in itemids)
			        itemFilter.Name = itemName;
			        itemFilter.Movable = 1;
			        //itemFilter.RangeMax = 2;
			        
			        foreach (Item item in Items.ApplyFilter(itemFilter))
			        {
				        if (item == null) continue;
				        foundItems.Add(item);
			        }
		        }
		        UoTLogger.LogErrorToFile("FindItemsAnywhere: " + foundItems.Count);
		        return foundItems;
	        }
	        catch (Exception e)
	        {
		        UoTLogger.LogErrorToFile("FindItemsAnywhere: ", e);
	        }
	        return foundItems;
        }
        public static List<Item> FindItemsAnywhere(List<int> itemIds)
        {
            Items.Filter itemFilter = new Items.Filter
                {
                    //Whatever filters, check with new Items.Filter().filterProperty
                    //Enabled
                    //Serials
                    //Graphics
                    //Name
                    //Hues
                    //RangeMin
                    //RangeMax
                    //ZRangeMin
                    //ZRangeMax
                    //Movable
                    //Multi
                    //CheckIgnoreObject
                    //Layers
                    //OnGround
                    //IsCorpse
                    //IsContainer
                    //IsDoor
                    Enabled = true
                };

            foreach (var itemId in itemIds) itemFilter.Graphics.Add(itemId);
            
            return Items.ApplyFilter(itemFilter);
        }


        public static List<Item> FindItemsAnywhere(string itemName)
        {
	        return FindItemsAnywhere(new List<string>() { itemName });
        }
        public static List<Item> FindItemsAnywhere(int itemId)
        {
            return FindItemsAnywhere(new List<int>() { itemId });
        }

        public static Item FindAndEquipWeapon(int weaponId, bool oneHanded = true)
        {
            return FindAndEquipWeapon(new List<int>() {weaponId}, oneHanded);
        }

        public static Item FindAndEquipWeapon(List<int> weaponIds, bool oneHanded = true, bool recursive = true)
        {
            var rightHand = Player.GetItemOnLayer("RightHand");
            var leftHand = Player.GetItemOnLayer("LeftHand");
            switch (oneHanded)
            {
                case false when leftHand != null && weaponIds.Contains(leftHand.ItemID):
                    return leftHand;
                case true when rightHand != null && weaponIds.Contains(rightHand.ItemID):
                    return rightHand;
            }
            var weapons = FindItems(weaponIds, Player.Backpack, null, recursive);
            if (weapons == null || weapons.Count == 0) return null;
            var weapon = weapons.FirstOrDefault();
            if (weapon == null) return null;
            if (rightHand != null)
            {
	            Player.UnEquipItemByLayer("RightHand");
	            Misc.Pause(700);
	            if (leftHand.IsTwoHanded)
	            {
		            Player.UnEquipItemByLayer("LeftHand");
		            Misc.Pause(700);
	            }
            }
            if (!oneHanded)
            {
	            if (leftHand != null)
	            {
		            Player.UnEquipItemByLayer("LeftHand");
					Misc.Pause(700);
	            } 
            }

            Player.EquipItem(weapon);
            Misc.SendMessage("Equipping " + weapon.Name);
            Misc.Pause(2500);
            return weapon;

        }

        public static string BookName(int index)
        {
            return "book_" + index.ToString();
        }

        public static void RestockSupplies(string agentRestock)
        {
            if (agentRestock == null) return;
            
            // Restock.RunOnce(agentRestock, UoTLogger.JsonStoredData.GetData<int>("storage_chest"), Player.Backpack, 650);
            Restock.ChangeList(agentRestock);
            Restock.FStart();

            while (Restock.Status())
            {
                Player.HeadMessage(33, "Attempting to restock with " + agentRestock);
                Misc.Pause(250);
            }
            // Might want to add a timeout timer
            Restock.FStop();
        
        }
        
        public static void RestockSupplies(Item item, int amount)
        {
            RestockSupplies(item.ItemID, amount);
        }
        public static void RestockSupplies(int itemId, int amount)
        {
            RestockSupplies((itemId, amount));
        }
        public static void RestockSupplies(List<(int, int)> supplies) 
        {
            foreach (var item in supplies)
            {
                RestockSupplies(item);
            } 
        }
        //change to find all
        public static void RestockSupplies((int, int) supplies)
        {
            var currentCount = Items.ContainerCount(Player.Backpack.Serial, supplies.Item1, -1, true);
            //Misc.SendMessage($"Items in bag: {currentCount}", 33);
            if (currentCount >= supplies.Item2) return; 
            
            var fileName = UoTJson.FileName;
            if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
            var chestSerial = UoTJson.StoreOrGetJson<int>("Restock", default, fileName);
            var chest = Items.FindBySerial(chestSerial);
            if (chest == null)
            {
	            UoTJson.ClearData("Restock", fileName);
	            chestSerial = UoTJson.StoreOrGetJson<int>("Restock", default, fileName);
	            chest = Items.FindBySerial(chestSerial);
	            if (chest == null)
	            {
		            Misc.SendMessage("Restock");
		            return;
	            }
            }
            //var chestGump = UoTGumps.GetItemGumpId(chestSerial);
            

            var itemsInBag = FindItems(supplies.Item1, chest, null, true, false );
            Misc.SendMessage($"Nearby: {itemsInBag.Count}", 33);
            Misc.SendMessage($"Stocking: {supplies.Item2}", 33);

            foreach (var item in itemsInBag.Where
                     (item => Items.ContainerCount(
                                  Player.Backpack.Serial, supplies.Item1, -1, true) < supplies.Item2 
                              && item.Movable))
            {
                Items.Move(item, Player.Backpack, 0, 0, 0);
                Misc.Pause(650);
            }
        }

        public static void BagTransfer()
        {
	        var tar = new Target();
	        var sourceSerial = tar.PromptTarget("Select Source bag");
	        var sourceBag = Items.FindBySerial(sourceSerial);
	        var targetSerial = tar.PromptTarget("Select Target bag");
	        var targetBag = Items.FindBySerial(targetSerial);
	        var targetItemSerial = tar.PromptTarget("Select Target item");
	        var targetItem = Items.FindBySerial(targetItemSerial);
	        if (targetItem == null)
	        {
		        Items.WaitForContents(sourceBag,3000);
		        foreach (var item in sourceBag.Contains.Where(i => i.IsLootable))
		        {
			        Items.Move(item.Serial, targetBag, item.Amount);
			        Misc.Pause(200);
		        }
		        return;
	        }
	        var targetItemId = targetItem.ItemID;
	        Items.WaitForContents(sourceBag,3000);
	        foreach (var item in sourceBag.Contains.Where(i => i.IsLootable && i.ItemID == targetItemId))
	        {
		        Items.Move(item.Serial, targetBag, item.Amount);
		        Misc.Pause(200);
	        }
        }
        
        public static void DropItemsOnGroud(List<Item> items, CancellationToken token = default)
        {
	        if (items.Count == 0) return;
	        Misc.Pause(1600);
	        foreach (var item in items)
	        {
		        token.ThrowIfCancellationRequested();
		        var xOffset = UoTMath.randomGen.Next(-1, 2); // Random value between -1 and 1
		        var yOffset = UoTMath.randomGen.Next(-1, 2); // Random value between -1 and 1
            
		        Items.MoveOnGround(item, item.Amount, 
			        Player.Position.X + xOffset, 
			        Player.Position.Y + yOffset, 
			        Player.Position.Z);
		        Misc.Pause(1600);
	        }
        }

        public static void MoveItemsToContainer(List<int> items, int container, CancellationToken token)
        {
	        var chest = Items.FindBySerial(container);
	        if (chest == null) return;
            var moveItems = UoTItems.FindItems(items, Player.Backpack, null, false, false);
            if (moveItems.Count == 0) return;
            UoTMovement.PathingTask(chest.Position.X, chest.Position.Y, default, 1, token);
            Misc.Pause(300);
            UoTasks.StopTask("Pathing");
            Misc.Pause(300);
            //var gump = UoTGumps.GetItemGumpId(container);
            //if (gump != 0) Gumps.CloseGump(gump);
            foreach (var item in moveItems.Where(item => item is { Movable: true }))
            {
                Items.Move(item.Serial, Items.FindBySerial(container), -1);
                Misc.Pause(800);
            }
        }
        
        public static void MoveItemsToContainerFromBeetle(List<Item> moveItems, int container, CancellationToken token)
        {
	        if(token.IsCancellationRequested) return;
	        if(moveItems == null || !moveItems.Any()) return;
	        var chest = Items.FindBySerial(container);
	        if (chest == null) return;
	        if (UoTasks.StopTask("Pathing")) Misc.Pause(600);
	        UoTMovement.PathingTask(chest.Position.X, chest.Position.Y, default, default, token);
	        if (UoTasks.StopTask("Pathing")) Misc.Pause(600);
	        var beetle = UoTMobiles.FindBeetle();
	        if (beetle == null && Player.Mount != null)
	        {
				Misc.SendMessage("Dismounting to move items from beetle to chest", 33);
		        Mobiles.UseMobile(Player.Serial);
	        }
	        if (UoTasks.StopTask("Pathing")) Misc.Pause(600);
	        beetle = UoTMobiles.FindBeetle();
	        if (beetle == null) return;
	        Misc.SetSharedValue("Lootmaster:Pause", true);
	        //MoveItemsFromAnimal(moveItems);
	        Items.OpenAt(beetle.Backpack.Serial, 0, 0);
	        //Items.UseItem(beetle.Backpack);
	        //Items.OpenContainerAt(beetle.Backpack, 0,0);
	        Misc.Pause(600);
	        Items.UseItem(chest);
	        Misc.Pause(600);
	        foreach (var item in moveItems)
	        {
		        if(token.IsCancellationRequested) return;
		        if (item is not { Movable: true }) continue;
		        Items.Move(item.Serial, Items.FindBySerial(container), -1);
		        Misc.Pause(800);
	        }
	        
	        Mobiles.UseMobile(beetle);
	        Misc.Pause(800);
	        Misc.SetSharedValue("Lootmaster:Pause", false);
        }
        
        //change order to look, beetle first.
        public static void MoveItemsToAnimal(List<Item> items, CancellationToken token)
        {
            if (items.Count == 0) return;
            Player.HeadMessage(33, "Looking for pack animals");
            var packList = UoTMobiles.FindPackAnimals();
            //possible add CanRename check
            var beetle = UoTMobiles.FindBeetle();
            if (beetle != null) packList.Add(beetle);
            if (packList.Count == 0)
            {
                Player.HeadMessage(33, "None Found");
            }
            else
            {
                foreach (var p in packList)
                {
                    if (p?.Backpack == null) continue;
                    Misc.SendMessage("Found " + p.Name, 33);
                    foreach (var item in items)
                    {
	                    token.ThrowIfCancellationRequested();
                        if (p.Backpack.Weight > 1400) break;
                        if (item is not { Movable: true }) continue;
                        Items.Move(item.Serial, p, -1);
                        Misc.Pause(800);
                    }

                    Misc.Pause(800);
                    if (Player.Mount == null & p.Body == 0x0317)
                    {
                        Mobiles.UseMobile(p.Serial);

                        Misc.Pause(1800);
                    }
                }
            }
        }
        public static Item CreatePickaxe()
        {
	        try
	        {
				if (Player.GetSkillValue("Tinkering") < 40) return null;
				var toolCount = Items.ContainerCount(Player.Backpack, 0x1EB8, -1) + Items.ContainerCount(Player.Backpack, 0x1EBC, -1);
				if (toolCount == 1) CreateTinkerTools();
			    var tinkertools = Items.FindByID(0x1EB8, -1, Player.Backpack.Serial);
			    if (tinkertools == null) tinkertools = Items.FindByID(0x1EBC, -1, Player.Backpack.Serial);
			    if (tinkertools == null)
			    {
				    Player.HeadMessage((int)UoTLists.Colors.Yellow, "We are out of tinker tools.");
				    return null;
			    }
			    //Player.HeadMessage((int)UoTLists.Colors.Yellow, "Creating new pickaxe...");
			    
			    var gumpid = UoTGumps.GetItemGumpId(tinkertools);
			    if (gumpid == 0) UoTGumps.SaveItemGumpId(tinkertools);
			    gumpid = UoTGumps.GetItemGumpId(tinkertools);
			    
			    Misc.Pause(2000);
			    Items.UseItem(tinkertools);
			    if (!UoTGumps.WaitForGump(gumpid))
			    {
				    Items.UseItem(tinkertools);
				    UoTGumps.WaitForGump(gumpid);
			    }
			    Gumps.SendAction(gumpid, 15); // UOAlive: 41
			    UoTGumps.WaitForGump(gumpid);
			    Misc.Pause(200);
			    Gumps.SendAction(gumpid, 114); // UOAlive: 323
			    UoTGumps.WaitForGump(gumpid);
			    Misc.Pause(200);
			    Gumps.SendAction(gumpid, 0);
			    //Gumps.CloseGump(gumpid);
			    //Gumps.ResetGump();
			    var pickaxe = Items.FindByID(0x0E86, -1, Player.Backpack.Serial);
			    return pickaxe;
	        }
	        catch (Exception e)
	        {
		        UoTLogger.LogErrorToFile("Creating pickaxe errored", e);
		        throw;
	        }
	        
		}

		public static bool FindShovel()
		{
		    var tinkertools = Items.FindByID(0x1EB8, -1, Player.Backpack.Serial);
		    var shovel = Items.FindByID(0x0E86, -1, Player.Backpack.Serial);
		    
		    if (tinkertools == null)
		    {
		        Player.HeadMessage((int)UoTLists.Colors.Red, "No tinker tools found!");
		        return false;
		    }

		    if (Items.ContainerCount(Player.Backpack.Serial, 0x0F39, -1) < 1 && tinkertools != null)
		    {
		        Items.UseItem(tinkertools);
		        var gumpid = Gumps.CurrentGump();
		        Misc.Pause(500);
		        UoTGumps.WaitForGump(gumpid, 1500);
		        Gumps.SendAction(gumpid, 202);
		        UoTGumps.WaitForGump(gumpid, 1500);
		        Gumps.SendAction(gumpid, 0);
		        UoTGumps.WaitForGump(gumpid, 2000);
		        Gumps.CloseGump(gumpid);
		        Misc.Pause(2000);
		    }
		    
		    return true;
		}

		public static void CreateTinkerTools()
		{
			var tinkertools = Items.FindByID(0x1EB8, -1, Player.Backpack.Serial);
			if (tinkertools == null) tinkertools = Items.FindByID(0x1EBC, -1, Player.Backpack.Serial);
		    if (tinkertools == null || Player.GetSkillValue("Tinkering") < 10)
		    {
		        Player.HeadMessage((int)UoTLists.Colors.Red, "No tinker tools or skill found!");
		        return;
		    }
		    
		    var gumpid = UoTGumps.GetItemGumpId(tinkertools);
		    if (gumpid == 0) UoTGumps.SaveItemGumpId(tinkertools);
		    gumpid = UoTGumps.GetItemGumpId(tinkertools);
		    
		    Misc.Pause(2000);
		    Items.UseItem(tinkertools);
		    if (!UoTGumps.WaitForGump(gumpid))
		    {
			    Items.UseItem(tinkertools);
			    UoTGumps.WaitForGump(gumpid);
		    }
		    Gumps.SendAction(gumpid, 15); // UOAlive: 41
		    UoTGumps.WaitForGump(gumpid);
		    Misc.Pause(200);
		    Gumps.SendAction(gumpid, 23); // UOAlive: 63
		    UoTGumps.WaitForGump(gumpid, 1000);
		    Misc.Pause(200);
		    Gumps.SendAction(gumpid, 0);
		    //Gumps.CloseGump(gumpid);
		    //Gumps.ResetGump();
		    Misc.Pause(500);
		    
		}

		public static Item GetItem(string itemName)
		{
			var item = FindItemAnywhere(itemName);
			if (item == null)
			{
				Misc.SendMessage("Item not found: " + itemName, 33);
				return null;
			}
			TrackItem(item.Serial, item.ItemID, item.Hue, item.Container, item.RootContainer,
				(item.Position.X, item.Position.Y));
			Items.Move(item, Player.Backpack, -1);
			Misc.Pause(600);
			return item;
		}
		public static bool Cut(Item item)
		{
			var scissors = GetItem("Scissors");
			if (scissors == null || item == null) return false;
			Items.UseItem(scissors);
			if (Target.WaitForTarget(5000)) Target.TargetExecute(item);
			else return false;
			Misc.Pause(300);
			if (Target.HasTarget()) Target.Cancel();
			Misc.Pause(300);
			return true;
		}

		public static int CountItemsInBag(string itemName, Item bag = null)
		{
			bag ??= Player.Backpack;
			return !bag.IsContainer ? 0 : bag.Contains.Where(item => item.Name == itemName).Sum(item => item.Amount);
		}

		
		//could return list of needed or collected items
		public static bool GetItems(List<(string,int)> items)
		{
			if (items == null) return false;
			foreach (var item in items)
			{
				GetItem(item.Item1, item.Item2);
			}
			return true;
		}

		public static int GetItem(string ingredientName, int amount)
		{
			var foundItems = UoTItems.FindItemsAnywhere(ingredientName);
			foreach (var foundItem in foundItems)
			{
				if (foundItem.Amount > amount)
				{
					Items.Move(foundItem, Player.Backpack, amount);
					amount = 0;
				}
				else
				{
					Items.Move(foundItem, Player.Backpack, foundItem.Amount);
					amount -= foundItem.Amount;
				}
			}
			return amount;
		}

		public class ToReturn
		{
			public int ItemSerial { get; }
			public int ItemId { get; }
			public ushort Hue { get; }
			public int ContainerSerial { get; }
			public int RootContainerSerial { get; }
			public (int X, int Y) LocationInContainer { get; }

			public ToReturn(int itemSerial, int itemId, ushort hue, int containerSerial, int rootContainerSerial, (int X, int Y) locInContainer)
			{
				ItemSerial = itemSerial;
				ItemId = itemId;
				Hue = hue;
				ContainerSerial = containerSerial;
				RootContainerSerial = rootContainerSerial;
				LocationInContainer = locInContainer;
			}
		}
		private static readonly Dictionary<int, ToReturn> _trackedItems = new();

		public static void TrackItem(int itemSerial, int itemId, ushort hue, int containerSerial, int rootContainerSerial, (int X, int Y) locInContainer)
		{
			var item = new ToReturn(itemSerial, itemId, hue, containerSerial, rootContainerSerial, locInContainer);
			_trackedItems[itemSerial] = item;
		}

		public static ToReturn GetItemInfo(int itemSerial)
		{
			return _trackedItems.TryGetValue(itemSerial, out var item) ? item : null;
		}

		public static void StopTrackingItem(int itemSerial)
		{
			_trackedItems.Remove(itemSerial);
		}

		
		/*
		public void ReturnItems()
		{
			var newToReturn = new List<Dictionary<string, object>>();
			var itemsInUse = new List<Item>();

			if (excludeBod != null)
			{
				var toolCodList = SkillInfo[excludeBod.Skill]["tools"].Values.ToList();

				if (excludeBod.Skill == "Cartography" || excludeBod.Skill == "Inscription")
				{
					toolCodList = SkillInfo[excludeBod.Skill]["tools"].Keys.ToList();
				}

				itemsInUse.AddRange(FindItemsList(toolCodList, Player.Backpack, -1, true));

				foreach (var resourceData in excludeBod.GetResourcesData())
				{
					itemsInUse.AddRange(FindItemsList(new List<int> { resourceData["id"] }, Player.Backpack, resourceData["color"], true));
				}
			}

			foreach (var itemData in to_return)
			{
				var item = Items.FindByID((int)itemData["id"], (int)itemData["color"], Player.Backpack.Serial, true, true);

				if (item == null)
					continue;

				if (excludeBod != null && itemsInUse.Any(i => i.Serial == item.Serial))
				{
					newToReturn.Add(itemData);
					continue;
				}

				if (!(bool)itemData["tool"] && Items.FindByID((int)itemData["id"], (int)itemData["color"], (int)itemData["container"], true, true) != null)
				{
					ThrowItems(new List<Item> { item }, (int)itemData["container"]);
				}
				else
				{
					ThrowItems(new List<Item> { item }, (int)itemData["container"], -1, (int)itemData["x"], (int)itemData["y"]);
				}
			}

			to_return = newToReturn;
		}*/
		public static List<Item> CraftTool(string skill, int amount)
		{
			var tools = UoTBods.BodSkillInfo[skill].Tools.ToList();
			var toolsName = UoTBods.BodSkillInfo[skill].Tools.Keys.ToList();
			var toolsId = UoTBods.BodSkillInfo[skill].Tools.Values.ToList();
			List<(UoTBods.Profession, UoTBods.Category, UoTBods.Recipe)> toolRecipes = new();
			//Might have range issue, could check range of container=
			var craftedTools = new List<Item>();
			while (craftedTools.Count < amount)
			{
				foreach (var tool in tools)
				{
					var toolRecipe = UoTBods.GetRecipeInfo(tool.Key);
					var toolForRecipe = GetTool(toolRecipe.Item1.Name);
					if (toolForRecipe == null) continue; //or recursive call?
					//Is there a string I can check?
					var gumpid = UoTGumps.OpenCraftGump(toolRecipe.Item1.Name);
					if (gumpid == 0) UoTGumps.OpenCraftGump(toolRecipe.Item1.Name);
					if (gumpid == 0)
					{
						UoTGumps.SaveItemGumpId(toolForRecipe);
						Items.UseItem(toolForRecipe);
						Misc.SendMessage("OpenCraftGump: failed with " + toolRecipe.Item1.Name, 33);
					}

					if (gumpid == 0)
					{
						Misc.SendMessage("No craft gump found for " + toolRecipe.Item1.Name, 33);
						continue;
					}
					if (!UoTGumps.WaitForGump(gumpid)) Items.UseItem(toolForRecipe);
					UoTGumps.AnswerGump(gumpid, toolRecipe.Item2.ButtonId, toolRecipe.Item3.ButtonId);
					//check count
				}
			}
			return craftedTools;
		}

		public static List<Item> GetTools(string skill, bool lookOnGround = true)
		{
			// Access dictionary with case-insensitive skill name
			var skillKey = CraftingTools.Keys.FirstOrDefault(k => k.Equals(skill, StringComparison.OrdinalIgnoreCase)) ?? skill;
			var toolsName = CraftingTools[skillKey].Keys.ToList();
			var toolsId = CraftingTools[skillKey].Values.ToList();
			
			
	        //Might have range issue, could check range of container
			var foundToolsRaw = Items.FindAllByID(toolsId, -1, -1, 2); 
			UoTLogger.LogErrorToFile("GetTools: " + skill + ":" + skillKey + " " + foundToolsRaw.Count);
	        var foundTools = new List<Item>();
	        foreach (var toolRaw in foundToolsRaw)
	        {
		        if (!toolRaw.Movable) continue;
		        foundTools.Add(toolRaw);
	        }
	        var foundToolsAnywhere = FindItemsAnywhere(toolsName);
	        UoTLogger.LogErrorToFile("Found tools: " + foundTools.Count);
	        return foundTools.Union(foundToolsAnywhere).ToList();
		}
        public static Item GetTool(string skill, bool lookOnGround = true)
        {
	        var skillKey = CraftingTools.Keys.FirstOrDefault(k => k.Equals(skill, StringComparison.OrdinalIgnoreCase)) ?? skill;
	        var tools = CraftingTools[skillKey].Keys;
	        var rightHand = Player.GetItemOnLayer("RightHand");
	        if (rightHand != null && tools.Any(t => t.Equals(rightHand.Name, StringComparison.OrdinalIgnoreCase)))
		        return rightHand;
	        var leftHand = Player.GetItemOnLayer("LeftHand");
	        if (leftHand != null && tools.Any(t => t.Equals(leftHand.Name, StringComparison.OrdinalIgnoreCase)))
		        return leftHand;

	        return GetTools(skill, lookOnGround).FirstOrDefault();
            /*if (lookOnGround)
            {
                var tool = GetGroundTool(skill);
                if (tool != null)
                    return tool;
            }*/

            /*
             UoT.FindItems();
            var salvageBag = GetFirst(FindItemsList(new List<string> { "Salvage Bag" }, Player.Backpack));
            var toolCodList = tools.ToList();
            var containerList = new List<long>();

            if (salvageBag != null && 
                new[] { "Blacksmithing", "Tailoring", "Tinkering" }.Contains(skill))
            {
                containerList.Add(salvageBag.Serial);
                to_container = salvageBag.Serial;
            }
            
            containerList.Add(Player.Backpack.Serial);
            if (nearbyChests != null)
                containerList.AddRange(nearbyChests.Select(chest => chest.Serial));

            for (int attempt = 0; attempt < 3; attempt++)
            {
                for (int i = 0; i < containerList.Count; i++)
                {
                    var toolList = FindItemsList(toolCodList, containerList[i], -1, true);

                    if (toolList.Count > 0)
                    {
                        if (i > 0)
                        {
                            cont_xy["tools"]["y"] = (cont_xy["tools"]["y"] + 10) % 160 + 40;
                            RefreshToReturn(GetFirst(toolList), true);
                            ThrowItems(new List<Item> { GetFirst(toolList) }, to_container, 1, 
                                cont_xy["tools"]["x"], cont_xy["tools"]["y"]);
                            break;
                        }
                        else
                        {
                            var tool = GetFirst(toolList);
                            to_container = Items.FindBySerial(DotContainer(tool));
                            return tool;
                        }
                    }
                }
            }*/

            return null;
        }
        public static bool CompareName(string name, string secondName)
        {
	        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(secondName))
	        {
		        return false;
	        }

	        var nameLower = name.ToLowerInvariant();
	        var secondNameLower = secondName.ToLowerInvariant();
    
	        // Direct comparison
	        if (nameLower == secondNameLower || nameLower == secondNameLower + "s")
	        {
		        return true;
	        }

	        // Try to get the part after the first space
	        var spaceIndex = secondNameLower.IndexOf(' ');
	        if (spaceIndex <= 0)
	        {
		        return false;
	        }
	        var altName = secondNameLower.Substring(spaceIndex + 1);
	        return nameLower == altName || nameLower == altName + "s";
        }

        public static int ItemIdByName(string itemName)
        {
	        return Items.FindByName(itemName, -1, -1, 2)?.ItemID ?? 0;
        }
        
        public static string ItemNameById(int itemId)
        {
	        return Items.FindByID(itemId, -1, -1, 2)?.Name ?? "NotFound";
        }

    }
    
}