using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assistant;
using IronPython.Modules;
using Ultima;

namespace RazorEnhanced
{
    public partial class Bod2
    {
        public string Skill { get; set; }
        public int BodSerial { get; set; }
        public string BaseMaterial { get; set; } = "";
        public bool Exceptional { get; set; } = false;
        public string Type { get; set; } = "small";

        public Dictionary<string, object> Item { get; set; } = new Dictionary<string, object>
            { { "name", "" }, { "done_amount", 0 } };

        public string MadeName { get; set; } = "";
        public List<Dictionary<string, object>> ItemsList { get; set; } = new List<Dictionary<string, object>>();
        public int TotalAmount { get; set; } = 20;
        public bool Filled { get; set; } = false;
        public List<object> OldItems { get; set; } = new List<object>();

        public Bod(string skill, object serial)
        {
            Skill = skill;
            BodSerial = serial;

            if (skill == "Blacksmithing" || skill == "Tinkering")
            {
                BaseMaterial = "Iron";
            }
            else if (skill == "Tailoring")
            {
                BaseMaterial = "Leather/Hides";
            }
            else if (skill == "Bowcraft and Fletching" || skill == "Carpentry")
            {
                BaseMaterial = "Wood";
            }

            Refresh(true);
        }

        public void Refresh(bool createList = false)
        {
            if (BodSerial == null) return;

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    string textList = BodSerial.Properties?.ToString()?.Replace("[", "|").Replace("]", "|").ToLower() ??
                                      "";

                    if (textList.Contains("exceptional"))
                    {
                        Exceptional = true;
                    }

                    if (textList.Contains("large bulk"))
                    {
                        Type = "large";
                        Misc.IgnoreObject(BodSerial);
                    }

                    if (BaseMaterial == "Iron" && textList.Contains("all items must be made with "))
                    {
                        foreach (var ingot in SkillInfo[Skill].Materials)
                        {
                            if (textList.Contains($"all items must be made with {ingot.ToLower()} ingots."))
                            {
                                BaseMaterial = ingot;
                            }
                        }
                    }

                    if (BaseMaterial == "Leather/Hides" && textList.Contains("all items must be made with "))
                    {
                        foreach (var leather in SkillInfo[Skill].Materials)
                        {
                            if (textList.Contains(
                                    $"all items must be made with {leather.ToLower().Replace("hides", "leather.")}"))
                            {
                                BaseMaterial = leather;
                            }
                        }
                    }

                    if (BaseMaterial == "Wood")
                    {
                        foreach (var wood in SkillInfo[Skill].Materials)
                        {
                            if (textList.Contains($"|{wood.ToLower()}|"))
                            {
                                BaseMaterial = wood;
                            }
                        }
                    }

                    foreach (var prop in textList.Split('|'))
                    {
                        if (!prop.Contains(":") || prop.Contains("weight")) continue;

                        if (prop.Contains("amount to make:"))
                        {
                            TotalAmount = Math.Min(TotalAmount, int.Parse(prop.Split("amount to make:")[1]));
                        }
                        else
                        {
                            var itemData = prop.Split(':');
                            if (createList)
                            {
                                Items.Add(new Dictionary<string, object>
                                {
                                    { "name", itemData[0] },
                                    { "done_amount", int.Parse(itemData[1]) }
                                });
                                Item = Items[0];
                            }
                            else
                            {
                                Item = new Dictionary<string, object>
                                {
                                    { "name", itemData[0] },
                                    { "done_amount", int.Parse(itemData[1]) }
                                };
                            }
                        }
                    }

                    return;
                }
                catch (Exception)
                {
                    Misc.Pause(10);
                }
            }

            Error($"Failed to retrieve data from {Skill} BOD.");
        }

        public int ToMakeQuantity()
        {
            return TotalAmount - Convert.ToInt32(Item["done_amount"]);
        }

        public bool OpenGump()
        {
            if (_ast.Global.BodGumpId > 0 && Gumps.WaitForGump(_ast.Global.BodGumpId, 100) > 0) return true;

            for (int i = 0; i < 5; i++)
            {
                Items.UseItem(BodSerial);
                int newBodGumpId = GetGumpNumber(new[] { "A bulk order", "A large bulk order" }, 4000);
                if (newBodGumpId > 0)
                {
                    _ast.Global.BodGumpId = SetSharedGump("bod_gump_id", newBodGumpId);
                    return true;
                }
            }

            return false;
        }

        public void SetOldItems()
        {
            OldItems = Items.Contains(Player.Backpack).Select(i => i.Serial).ToList();
        }

        public void SetMadeItemName()
        {
            if (!string.IsNullOrEmpty(MadeName)) return;

            var newItem = GetFirst(Items.Contains(Player.Backpack)
                .Where(i => !OldItems.Contains(i.Serial) &&
                            !SkillInfo[Skill].Tools.Keys.Contains(i.Name)).ToList());
            if (newItem != null)
            {
                MadeName = newItem.Name;
            }
        }

        public void Use()
        {
            bool success = false;

            for (int i = 0; i < 3; i++)
            {
                var fillingContainers = GetFillingContainers();
                if (!fillingContainers.Any())
                {
                    success = true;
                    break;
                }

                foreach (var container in fillingContainers)
                {
                    OpenGump();
                    if (_ast.Global.BodGumpId > 0 && Gumps.WaitForGump(_ast.Global.BodGumpId, 4000) > 0)
                    {
                        success = true;
                        Gumps.SendAction(_ast.Global.BodGumpId, 4);

                        if (Target.WaitForTarget(4000, false) > 0)
                        {
                            Target.TargetExecute(container);
                            Misc.Pause(300);
                            if (Gumps.WaitForGump(_ast.Global.BodGumpId, 4000) > 0)
                            {
                                Target.WaitForTarget(1000, false);
                                Target.Cancel();
                            }
                        }
                    }
                }
            }

            if (!success)
            {
                Error($"Failed to open {Skill} BOD gump.");
            }

            Gumps.CloseGump(Gumps.CurrentGump());
            Misc.Pause(500);
            Gumps.CloseGump(_ast.Global.BodGumpId);
            Target.Cancel();
        }

        private List<object> GetFillingContainers()
        {
            if (Type == "large")
            {
                return Containers.GetFromId(new[] { 0x2258 }, SkillInfo[Skill].BodColor, null, new[] { BodSerial })
                    .ToList();
            }

            var searchList = Items.FindList(GetMaterialPrefix(BaseMaterial) + Item["name"],
                Player.Backpack, -1, true, Exceptional);

            if (!string.IsNullOrEmpty(MadeName) && !searchList.Any())
            {
                searchList = Items.FindList(MadeName, Player.Backpack, -1, true, Exceptional);
            }

            return searchList;
        }
        
        public bool GetItemsToFill()
    {
        var (button, itemData) = SearchButton(SkillInfo[Skill]["gump"], Item["name"].ToString());

        if (!(bool)itemData["stack"])
        {
            return false;
        }

        if (GetResource(new[] { GetMaterialPrefix(BaseMaterial) + Item["name"] }, -1, 1, ToMakeQuantity(), (bool)itemData["stack"]))
        {
            var madeItem = GetFirst(FindItemsList(new[] { GetMaterialPrefix(BaseMaterial) + Item["name"] }, Player.Backpack));
            if (madeItem != null)
            {
                if (madeItem.Amount <= ToMakeQuantity())
                {
                    return true;
                }
                else
                {
                    // Separate stacked items for filling the BOD
                    ThrowItems(new[] { madeItem }, Player.Backpack, ToMakeQuantity(), 100, 100);
                    return true;
                }
            }
        }

        return false;
    }

    public List<Dictionary<string, object>> GetResourcesData()
    {
        var commonNames = new Dictionary<string, Dictionary<string, object>>
        {
            { "Cloth", new Dictionary<string, object> { { "id", new[] { "Cut Cloth", "Cloth", "Bolt Of Cloth" } }, { "color", 0x0000 } } },
            { "Yards of Cloth", new Dictionary<string, object> { { "id", new[] { "Cut Cloth", "Cloth", "Bolt Of Cloth" } }, { "color", 0x0000 } } },
            { "Blank Maps or Scrolls", new Dictionary<string, object> { { "id", new[] { "Blank Scroll", "Blank Map" } }, { "color", -1 } } },
            { "Sea Serpent or Dragon Scales", new Dictionary<string, object> { { "id", new[] { "Sea Serpent Scales", "Dragon Scales" } }, { "color", -1 } } },
            { "Water", new Dictionary<string, object> { { "id", new[] { "Endless Decanter of Water", "Glass Pitcher", "A Pitcher Of Water" } }, { "color", -1 } } }
        };

        var (button, itemData) = SearchButton(SkillInfo[Skill]["gump"], Item["name"].ToString());
        var resourcesData = new List<Dictionary<string, object>>();

        foreach (var resource in (List<object>)itemData["mats"])
        {
            var resourceData = new Dictionary<string, object>();

            foreach (var nameList in MaterialInfo.Keys)
            {
                if (resource[0].ToString() == nameList)
                {
                    resourceData = MaterialInfo[nameList][BaseMaterial];
                    resourceData["name"] = BaseMaterial.Replace("Leather/Hides", "").Replace(" Hides", "") + " " + resource[0].ToString();
                    break;
                }
            }

            if (!resourceData.Any())
            {
                foreach (var name in commonNames.Keys)
                {
                    if (resource[0].ToString() == name)
                    {
                        resourceData = commonNames[name];
                        resourceData["name"] = name;
                        break;
                    }
                }
            }

            if (!resourceData.Any())
            {
                resourceData = new Dictionary<string, object> { { "id", new[] { resource[0] } }, { "color", -1 } };
            }

            resourceData["name"] = resource[0];
            resourceData["amount"] = resource[1];
            resourceData["stack"] = itemData["stack"];
            resourcesData.Add(resourceData);
        }

        return resourcesData;
    }

        public bool GetResources(List<Dictionary<string, object>> resourcesData)
        {
            var grabbingResults = new List<bool>();
            foreach (var resourceData in resourcesData)
            {
                grabbingResults.Add(GetResource(
                    (string[])resourceData["id"],
                    (int)resourceData["color"],
                    (int)resourceData["amount"],
                    ToMakeQuantity(),
                    (bool)resourceData["stack"]
                ));
            }

            return !grabbingResults.Contains(false);
        }

        public void Start()
        {
            Misc.SendMessage("\r\n\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", SkillInfo[Skill]["bod_color"]);
            Misc.SendMessage($"{Skill} Small BOD | {Item["name"].ToString().ToUpper()}\r\n*** Starting ***\r\n\r\n", 48);
        }

        public bool Finished()
        {
            if ((Type == "small" && ToMakeQuantity() <= 0) || (Type == "large" && Filled))
            {
                Misc.IgnoreObject(BodSerial);
                Misc.SendMessage("\r\n\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", SkillInfo[Skill]["bod_color"]);

                if (Type == "small" && ToMakeQuantity() <= 0)
                {
                    Misc.SendMessage($"{Skill} Small BOD | {Item["name"].ToString().ToUpper()}\r\n*** Finished ***\r\n\r\n", 68);
                }
                else
                {
                    var itemsList = string.Join("\r\n", Items.Select(i => i["name"].ToString().ToUpper()));
                    Misc.SendMessage($"{Skill} Large BOD | Items: \r\n{itemsList}\r\n*** Finished ***\r\n\r\n", 68);
                }

                return true;
            }

            return false;
        }

        // Placeholder helper methods for undefined functionality
        private (object, dynamic) SearchButton(object gump, string itemName)
        {
            // Simulate searching based on the gump
            return (null, new { mats = new List<object>(), stack = true });
        }

        private bool GetResource(string[] resourceIds, int color, int resourceNeeded, int toMakeAmount, bool stack)
        {
            // Simulate resource fetching logic
            return true;
        }

        private void ThrowItems(IEnumerable<dynamic> items, dynamic container, int amount, int x, int y)
        {
            // Simulate item movement
        }

        private dynamic GetFirst(IEnumerable<dynamic> collection)
        {
            // Retrieve the first item from the collection
            return collection.FirstOrDefault();
        }

        private string GetMaterialPrefix(string material)
        {
            if (new[] { "Iron", "Leather/Hides", "Wood", "" }.Contains(material))
            {
                return string.Empty;
            }
            return material.Replace(" Hides", "").Replace("Gold", "Golden") + " ";
        }

        // Other placeholders for undefined data structures
        private Dictionary<string, dynamic> SkillInfo => new Dictionary<string, dynamic>();
        private Dictionary<string, dynamic> MaterialInfo => new Dictionary<string, dynamic>();

        private static void Error(string message) => throw new Exception(message);

        private static int GetGumpNumber(string[] textList, int timeout) => 0;
        private static int SetSharedGump(string name, int gumpNum) => gumpNum;
                
        public void CheckSave()
        {
            if (Journal.Search("The world is saving, please wait."))
            {
                Misc.Pause(3000);
                Journal.Clear();
            }
        }
        
        public T GetFirst<T>(List<T> itemList)
        {
            if (itemList != null && itemList.Count > 0)
            {
                return itemList[0];
            }
            return default(T); // Return default value for the generic type (e.g., null for reference types)
        }
        public int Dist(dynamic pos1, dynamic pos2 = null)
        {
            pos2 = pos2 ?? Player; // Default to Player if pos2 is null
            var pos = new[] { pos1, pos2 };

            for (int i = 0; i < pos.Length; i++)
            {
                if (!(pos[i] is Dictionary<string, object>))
                {
                    try
                    {
                        var _ = pos[i].Serial;
                    }
                    catch
                    {
                        pos[i] = Items.FindBySerial(pos[i]);
                    }
                }

                try
                {
                    if (pos[i].Serial == Player.Backpack.Serial)
                    {
                        pos[i] = Player;
                    }

                    if (DotContainer(pos[i]) > 0)
                    {
                        pos[i] = Items.FindBySerial(DotContainer(pos[i]));
                        return Dist(pos[0], pos[1]); // Recursive call
                    }
                }
                catch { }

                if (!(pos[i] is Dictionary<string, object>))
                {
                    try
                    {
                        pos[i] = new Dictionary<string, int>
                        {
                            { "X", pos[i].Position.X },
                            { "Y", pos[i].Position.Y },
                            { "Map", pos[i].Map }
                        };
                    }
                    catch
                    {
                        pos[i] = new Dictionary<string, int>
                        {
                            { "X", pos[i].Position.X },
                            { "Y", pos[i].Position.Y },
                            { "Map", Player.Map }
                        };
                    }
                }
            }

            try
            {
                if (pos[0]["Map"] != pos[1]["Map"])
                {
                    return 10000;
                }
            }
            catch { }

            return Math.Max(
                Math.Abs(pos[0]["X"] - pos[1]["X"]),
                Math.Abs(pos[0]["Y"] - pos[1]["Y"])
            );
        }
        
        public List<Item> Contains(Item container, int color = -1, bool recursive = true, bool exceptional = false)
        {
            var foundItems = new List<Item>();

            foreach (var item in container.Contains)
            {
                if ((exceptional && item.Properties.ToLower().Contains("exceptional")) || !exceptional)
                {
                    if (color >= 0)
                    {
                        if (color == item.Hue)
                        {
                            foundItems.Add(item);
                        }
                    }
                    else
                    {
                        foundItems.Add(item);
                    }
                }

                if (item.IsContainer && recursive)
                {
                    foundItems.AddRange(Contains(item, color, recursive, exceptional));
                }
            }

            return foundItems;
        }
        public bool NameMatsCompare(string matName, Item item)
        {
            if (matName == null || string.IsNullOrWhiteSpace(matName))
            {
                return false; // Handle null or non-string inputs
            }

            try
            {
                int idx = item.Name.IndexOf(" ", StringComparison.Ordinal) + 1;
                string altName = item.Name.Substring(idx).ToLower(); // Extract alternate name after the first space

                // Compare material name (case-insensitive) with variations of the item's name
                return matName.ToLower() == item.Name.ToLower()
                       || matName.ToLower() == altName
                       || matName.ToLower() == item.Name.ToLower() + "s"
                       || matName.ToLower() == altName + "s";
            }
            catch
            {
                return matName.ToLower() == item.Name.ToLower(); // Return direct match if an exception occurs (e.g., on altName extraction)
            }
        }
        public void DocumentChest(Item container)
        {
            // Helper function to handle container contents with timeout
            List<Item> ContainerContains(Item cont, int timeout = 1000)
            {
                for (int i = 0; i < timeout / 100; i++)
                {
                    CheckSave(); // Ensure the world is not saving
                    Misc.Pause(100);
                    var contents = cont.Contains;

                    if (Journal.Search("You must wait to perform another action."))
                    {
                        Journal.Clear();
                        Misc.Pause(1000);
                        Items.UseItem(cont); // Open the container again
                        return ContainerContains(cont, timeout); // Recursive call
                    }

                    if (contents != null && contents.Count > 0)
                    {
                        return contents;
                    }
                }

                return new List<Item>(); // Return an empty list if no contents found
            }

            // Convert container serial (int) to Item if necessary
            if (container is int serial)
            {
                container = Items.FindBySerial(serial);
            }

            // Clear journal and start checking contents
            Journal.Clear();
            CheckSave();
            Items.UseItem(container);

            foreach (var item in ContainerContains(container))
            {
                if (item.IsContainer)
                {
                    DocumentChest(item); // Recursive call for nested containers
                }
            }

            Items.Close(container); // Close the container when done
        }
        public List<Item> FindItemsList(List<object> itemIdList, Item container, int color = -1, bool recursive = true, bool exceptional = false)
        {
            var foundItems = new List<Item>();

            try
            {
                // Ensure container has a valid serial
                var _ = container.Serial;
            }
            catch
            {
                // If container is not Item, find it by serial
                container = Items.FindBySerial(container);
            }

            foreach (var itemId in itemIdList)
            {
                foreach (var item in container.Contains)
                {
                    // Match item by ID or material name comparison
                    if ((itemId.Equals(item.ItemID) || NameMatsCompare(itemId.ToString(), item)) &&
                        ((exceptional && item.Properties.ToLower().Contains("exceptional")) || !exceptional))
                    {
                        // Check color if specified
                        if (color >= 0)
                        {
                            if (color == item.Hue)
                            {
                                foundItems.Add(item);
                            }
                        }
                        else
                        {
                            foundItems.Add(item);
                        }
                    }

                    // Recursively check nested containers
                    if (recursive && item.IsContainer)
                    {
                        foundItems.AddRange(FindItemsList(itemIdList, item, color, true, exceptional));
                    }
                }
            }

            return foundItems;
        }
        
        
        public int GetRootContainer(object item)
        {
            // If the item is passed as a serial (integer), find it by serial.
            if (item is int serial)
            {
                item = Items.FindBySerial(serial);
            }

            // If the item doesn't exist, return the player's backpack serial.
            if (item == null)
            {
                return Player.Backpack.Serial;
            }

            // Recursively find the root container if it exists.
            if (DotContainer(item) > 0)
            {
                item = Items.FindBySerial(DotContainer(item));
                return GetRootContainer(item);
            }

            return (item as Item)?.Serial ?? 0; // Return item serial or 0 if item is null.
        }
        
        public List<Item> GetContainersFromId(
            List<int> itemIdList, 
            int color = -1, 
            List<Item> itemList = null, 
            List<int> excludedSerials = null)
        {
            excludedSerials ??= new List<int>(); // Initialize excludedSerials if null.
            var foundContainerSerials = new List<int>();

            // Determine the search list.
            var searchList = itemList ?? 
                             FindItemsList(itemIdList, Player.Backpack, color, true)
                                 .Where(i => !excludedSerials.Contains(i.Serial))
                                 .ToList();

            foreach (var item in searchList)
            {
                var containerSerial = DotContainer(item);
                if (containerSerial > 0 && !foundContainerSerials.Contains(containerSerial))
                {
                    foundContainerSerials.Add(containerSerial);
                }
            }

            // Return the containers as a list of Item objects.
            return foundContainerSerials.Select(serial => Items.FindBySerial(serial)).ToList();
        }
        
        public List<Item> GetItemsByFilter(
            List<int> itemIdList = null, 
            string name = null, 
            int radius = 2, 
            bool ground = false, 
            bool onlyContainers = false)
        {
            var idList = new List<int>();
            var selectedList = new List<Item>();

            var itemFilter = new Items.Filter
            {
                OnGround = ground ? 1 : 0,
                RangeMax = radius
            };

            if (itemIdList != null)
            {
                foreach (var itemId in itemIdList)
                {
                    var item = Items.FindBySerial(itemId);
                    if (item != null && !idList.Contains(item.ItemID))
                    {
                        idList.Add(item.ItemID);
                    }
                }

                // If idList remains empty, use the provided itemIdList.
                if (!idList.Any())
                {
                    idList = itemIdList;
                }

                itemFilter.Graphics = idList;
            }

            if (!string.IsNullOrEmpty(name))
            {
                itemFilter.Name = name;
            }

            if (onlyContainers)
            {
                itemFilter.IsContainer = 1;
            }

            var itemList = Items.ApplyFilter(itemFilter);

            if (itemList != null)
            {
                foreach (var it in itemList)
                {
                    if (Dist(it) <= radius)
                    {
                        selectedList.Add(it);
                    }
                }
            }

            return selectedList;
        }
        public int DotContainer(object item)
        {
            if (item is int serial)
            {
                item = Items.FindBySerial(serial);
            }
            else
            {
                item = Items.FindBySerial((item as Item)?.Serial ?? 0);
            }

            return item != null ? item.Container : 0;
        }
        public Item InContainer(Item item, Item container, bool recursive = false)
        {
            foreach (var i in container.Contains)
            {
                if (i.Serial == item.Serial)
                {
                    return container;
                }

                if (recursive && i.IsContainer)
                {
                    var cont = InContainer(item, i, true);
                    if (cont != null)
                    {
                        return cont;
                    }
                }
            }

            return null;
        }
        public void ThrowItems(List<Item> itemList, Item container, int amount = 0, int x = -1, int y = -1, int wait = 1000)
        {
            try
            {
                var _ = container.Serial; // Check if container has a serial
            }
            catch
            {
                container = Items.FindBySerial(container.Serial);
            }

            foreach (var item in itemList)
            {
                try
                {
                    var _ = item.Serial; // Check if item has a serial
                }
                catch
                {
                    item = Items.FindBySerial(item.Serial);
                }

                while (!Player.IsGhost)
                {
                    item = Items.FindBySerial(item.Serial); // Refresh item state
                    if (item == null || DotContainer(item) == container.Serial || DotContainer(item) == 0)
                    {
                        break;
                    }

                    Items.Move(item, container, amount, x, y);
                    Misc.Pause(wait);
                }
            }
        }
        
        
        public Item GetSharedItem(string name)
        {
            return Items.FindBySerial((int)Misc.ReadSharedValue(name));
        }
        public int GetShared(string name)
        {
            return Convert.ToInt32(Misc.ReadSharedValue(name));
        }
        public Item SetSharedItem(string name, string message)
        {
            Misc.SendMessage(message, 68);
            var item = Items.FindBySerial(Target.PromptTarget(""));
            if (item != null)
            {
                Misc.SetSharedValue(name, item.Serial);
            }
            return item;
        }
        
        
        public int SetSharedGump(string name, int gumpNum)
        {
            if (gumpNum != GetShared(name))
            {
                Misc.SetSharedValue(name, gumpNum);
            }
            return gumpNum;
        }

        public Item GetAndSetSharedContainer(string varName, string message)
        {
            var container = GetSharedItem(varName);

            while (container == null || !container.IsContainer || container.Serial == Player.Backpack.Serial || Dist(container) > 2)
            {
                container = SetSharedItem(varName, message);

                if (container == null || (container.IsContainer && Dist(container) <= 2))
                {
                    break;
                }

                Error("Is that a container?\r\nIs that not too far away?\r\n(2 tiles maximum radius).");
            }

            return container;
        }
        public bool InCap(string skill)
        {
            if (Player.GetSkillValue(skill) >= Player.GetSkillCap(skill))
            {
                Misc.SendMessage($"{skill} is at its cap. Finishing.", 68);
                return true;
            }
            return false;
        }
        public void Error(string text, string skill = "", string itemName = "", bool finish = true)
        {
            string msg = "";

            if (!string.IsNullOrEmpty(skill))
            {
                Misc.SendMessage("\r\n\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", skillInfo[skill]["bod_color"]);
                msg += skill;
            }

            if (!string.IsNullOrEmpty(itemName))
            {
                msg += " | item: " + itemName;
            }

            msg += "\r\n" + text + "\r\n";

            if (finish)
            {
                msg += "FINISHING.\r\n\r\n";
            }

            Misc.SendMessage(msg, 38);

            if (finish)
            {
                Environment.Exit(0); // Ends the application
            }
        }
        public Item GetWorkableContainer(int chestSerial, string chestName)
        {
            var chest = Items.FindBySerial(chestSerial);

            if (chest == null || Dist(chest) > 2)
            {
                if (chest != null)
                {
                    Misc.SendMessage($"\r\n!!! You are too far away from the {chestName.Replace("_", " ")} you set on top of the code !!!\r\n", 68);
                }

                chest = GetSharedItem(chestName);

                if (chest == null || Dist(chest) > 2)
                {
                    return null;
                }
            }

            return chest;
        }
        public bool OutOfResources(string name)
        {
            if (lookIntoNearbyChests)
            {
                Misc.SendMessage($"Can't find {name}. Skipping this BOD.\r\n", 38);
                return true;
            }

            var aux = auxiliaryChest;

            if (auxiliaryChestQuestions)
            {
                auxiliaryChest = GetAndSetSharedContainer("auxiliary_chest",
                    $"Can't find {name}, please select the container where the {name} are (Or ESC to skip this bod.)\r\n");

                nearbyChests = new List<Item> { auxiliaryChest };

                if (auxiliaryChest != null && auxiliaryChest == aux)
                {
                    return true;
                }

                if (auxiliaryChest != null)
                {
                    DocumentChest(auxiliaryChest);
                    return false;
                }
            }

            auxiliaryChest = aux;
            nearbyChests = new List<Item> { auxiliaryChest };

            return true;
        }
        public void SetMacroContainers()
        {
            trashBarrel = GetWorkableContainer(trashBarrel, "trash_barrel");

            if (!lookIntoNearbyChests)
            {
                auxiliaryChest = GetWorkableContainer(auxiliaryChest, "auxiliary_chest");

                if (auxiliaryChest == null && auxiliaryChestQuestions)
                {
                    auxiliaryChest = GetAndSetSharedContainer("auxiliary_chest", "Target an auxiliary container,"
                        + " if you want to grab resources from it.\r\n(Hit ESC to use only backpack resources).");
                }
            }

            if (trashBarrel == null && trashBarrelQuestions)
            {
                foreach (var skill in skillInfo.Keys)
                {
                    if (Items.FindByID(0x2258, skillInfo[skill]["bod_color"], Player.Backpack.Serial, false, true) != null)
                    {
                        if (new List<string> { "Bowcraft and Fletching", "Carpentry" }.Contains(skill))
                        {
                            trashBarrel = GetAndSetSharedContainer("trash_barrel",
                                "Target a trash container.\r\n(Hit ESC to trash no items).\r\nTarget some bag if you just want to organize the leftovers there.");
                            break;
                        }
                    }
                }
            }

            nearbyChests = lookIntoNearbyChests
                ? GetItemsByFilter(null, null, 2, true, true)
                : new List<Item> { auxiliaryChest };

            if (nearbyChests.Any())
            {
                Misc.SendMessage("\r\n!!! Please Wait !!!\r\n...Acknowledging containers...\r\n", 78);

                foreach (var chest in nearbyChests)
                {
                    DocumentChest(chest);
                }

                Misc.SendMessage("\r\n...Finished...\r\n", 78);
            }
        }
        
                
        public (int, int) SearchButton(Dictionary<string, Dictionary<string, int>> gump, string itemName)
        {
            foreach (var menuButton in gump)
            {
                if (menuButton.Key == "categories")
                    continue;

                foreach (var gumpItem in menuButton.Value)
                {
                    if (itemName.TitleCase() == gumpItem.Key || 
                        itemName.TitleCase() == gumpItem.Key.Replace("(", "").Replace(")", ""))
                    {
                        return (menuButton.Key[1], gumpItem.Value);
                    }
                }
            }

            foreach (var menuButton in gump)
            {
                if (menuButton.Key == "categories")
                    continue;

                foreach (var gumpItem in menuButton.Value)
                {
                    if (gumpItem.Key.Contains(itemName.TitleCase()) || 
                        gumpItem.Key.Replace("(", "").Replace(")", "").Contains(itemName.TitleCase()))
                    {
                        return (menuButton.Key[1], gumpItem.Value);
                    }
                }
            }

            Error("Failed to retrieve data. Item name didn't match.");
            return (0, 0); // Default return in case of error
        }
        public void Meditate()
        {
            while (Player.Mana != Player.ManaMax)
            {
                Player.UseSkill("Meditation");
                Misc.Pause(1000);

                while (Player.BuffsExist("Meditation") && Player.Mana != Player.ManaMax)
                {
                    Misc.Pause(1000);
                }
            }
        }
        public List<string> LastGumpLines(int timeout = 5000)
        {
            for (int i = 0; i < timeout / 10; i++)
            {
                if (Gumps.CurrentGump() == craftingGumpId)
                {
                    var lineList = Gumps.LastGumpGetLineList();

                    try
                    {
                        foreach (var line in lineList)
                        {
                            // Iterate over lines just for consistency
                        }

                        if (lineList.Any())
                        {
                            return lineList;
                        }
                    }
                    catch
                    {
                        // Ignore exceptions and continue
                    }
                }

                Misc.Pause(10);
            }

            return new List<string>();
        }
        
        public string WaitCraftGump(int gumpNum, int timeout)
        {
            bool gumpOpen = false;

            for (int i = 0; i < timeout / 100; i++)
            {
                if (Journal.Search("You have worn out"))
                {
                    string msg = "worn";

                    if (Journal.Search("You create "))
                        msg += " create";

                    if (Journal.Search("You create an exceptional"))
                        msg += " exceptional";

                    Journal.Clear();
                    return msg;
                }

                if (Journal.Search("You must wait to perform another action."))
                {
                    Misc.Pause(2000);
                    Journal.Clear();
                    return "wait";
                }

                if (Gumps.WaitForGump(gumpNum, 100) > 0)
                {
                    gumpOpen = true;
                    break;
                }
            }

            if (gumpOpen)
            {
                var msgList = new List<(string, int)>
                {
                    ("You failed ", 40),
                    ("You create an exceptional", 68),
                    ("You create the", 90),
                    ("You do not have ", 28),
                    ("You don't have ", 28),
                    ("You must be near ", 28)
                };

                foreach (var line in LastGumpLines())
                {
                    foreach (var msg in msgList)
                    {
                        if (line.Contains(msg.Item1))
                        {
                            if (msg.Item2 != 28)
                                Misc.SendMessage(line, msg.Item2);

                            return line;
                        }
                    }
                }

                return "";
            }
            else
            {
                return "timeout";
            }
        }
        public int GetGumpNum(List<string> textList, int timeout = 10000)
        {
            for (int i = 0; i < timeout / 100; i++)
            {
                foreach (var text in textList)
                {
                    if (Gumps.LastGumpTextExist(text))
                    {
                        return Gumps.CurrentGump();
                    }
                }

                Misc.Pause(100);

                if (Journal.Search("You must wait to perform another action."))
                {
                    Misc.Pause(2000);
                    Journal.Clear();
                    return 0;
                }
            }

            return 0;
        }
        public Item GetScissors()
        {
            var scissors = GetFirst(FindItemsList(new List<int> { 0x0F9F }, Player.Backpack));

            if (scissors == null && nearbyChests != null)
            {
                foreach (var chest in nearbyChests)
                {
                    scissors = GetFirst(FindItemsList(new List<int> { 0x0F9F }, chest));

                    if (scissors != null)
                    {
                        RefreshToReturn(scissors);
                        ThrowItems(new List<Item> { scissors }, Player.Backpack, 0);
                        Misc.Pause(500);
                        break;
                    }
                }
            }

            return scissors;
        }
        
        public bool Cut(Item item)
        {
            var scissors = GetScissors();
            if (scissors != null)
            {
                Items.UseItem(scissors);
                if (Target.WaitForTarget(5000, false) > 0)
                {
                    Target.TargetExecute(item);
                }
                Misc.Pause(600);
                return true;
            }
            return false;
        }
        public bool CheckMoreActions(string resourceId)
        {
            if (resourceId == "Glass Pitcher")
            {
                var emptyPitcher = Items.FindByID(0x0FF6, 0x0000, Player.Backpack.Serial, -1, true);
                var waterTrough = GetFirst(GetItemsByFilter(new List<int> { 0x0B41, 0x0B42, 0x0B43, 0x0B44 }));

                if (waterTrough == null)
                {
                    Misc.SendMessage("\r\nYou have to be near a Water Trough to fill the glass of water.\r\n");
                    return false;
                }

                if (emptyPitcher != null)
                {
                    Items.UseItem(emptyPitcher);

                    if (Target.WaitForTarget(4000, false) > 0)
                    {
                        Target.TargetExecute(waterTrough);
                        Misc.Pause(600);
                        return true;
                    }
                }
            }

            if (resourceId == "Bolt Of Cloth")
            {
                var bolt = GetFirst(FindItemsList(new List<string> { "Bolt Of Cloth" }, Player.Backpack));
                if (bolt != null && Cut(bolt))
                {
                    return true;
                }
            }
            return false;
        }
        
        
        public bool GetResource(List<int> resourceIdList, int color = -1, int resourceNeeded = 1, int toMakeAmount = 1, bool stack = false)
        {
            int totalAmountFound = 0;

            foreach (var resourceId in resourceIdList)
            {
                int amountFound = 0, grabbed = 0, toGrab = 0;
                string name = "resources";
                var containerList = new List<Item> { Player.Backpack };

                if (nearbyChests != null)
                    containerList.AddRange(nearbyChests);

                for (int attempt = 0; attempt < 5; attempt++)
                {
                    foreach (var container in containerList)
                    {
                        var resourceList = FindItemsList(new List<int> { resourceId }, container, color, true);

                        if (resourceList != null && resourceList.Count > 0)
                        {
                            if (container.Serial != Player.Backpack.Serial)
                            {
                                foreach (var resource in resourceList)
                                {
                                    if (resource.Container > 0)
                                    {
                                        if (amountFound == 0 && grabbed == 0)
                                        {
                                            name = "resources";
                                            cont_xy["resources"]["y"] = (cont_xy["resources"]["y"] + 10) % 160 + 40;
                                        }
                                        else
                                        {
                                            name = "stack";
                                        }
                                    }

                                    if (resourceId == "Bolt Of Cloth" && GetScissors() != null)
                                    {
                                        toGrab = GetQuantityToGrab(resource, 
                                            (int)(resourceNeeded * toMakeAmount / 50) + 1, 1, amountFound + grabbed, stack);
                                    }
                                    else
                                    {
                                        toGrab = GetQuantityToGrab(resource, resourceNeeded, toMakeAmount, amountFound + grabbed, stack);
                                    }

                                    if (toGrab > 0)
                                    {
                                        RefreshToReturn(resource);
                                        ThrowItems(new List<Item> { resource }, Player.Backpack, toGrab, 
                                            cont_xy[name]["x"], cont_xy[name]["y"]);

                                        grabbed += Math.Min(toGrab, resource.Amount);
                                        toGrab -= grabbed;
                                    }

                                    if (toGrab <= 0)
                                        break;
                                }
                            }
                            else // Container is the player's backpack
                            {
                                if (CheckMoreActions(resourceId))
                                    return true;

                                amountFound = 0;
                                foreach (var resource in resourceList)
                                    amountFound += resource.Amount;

                                if (GetQuantityToGrab(resourceList[0], resourceNeeded, toMakeAmount, amountFound, stack) <= 0)
                                    return true;
                            }
                        }
                    }
                }

                totalAmountFound += amountFound;
            }

            return totalAmountFound >= resourceNeeded;
        }
        
        public int GetQuantityToGrab(Item resource, int resourceNeeded = 1, int toMakeAmount = 1, int amountFound = 0, bool stack = false)
        {
            double weightAvailable = (Player.MaxWeight - 70) - Player.Weight;
            double weightPerPiece = resource.Weight / resource.Amount;
            double multiplier = stack ? 1 : 1.5;

            return (int)Math.Min(weightAvailable / weightPerPiece, (multiplier * resourceNeeded * toMakeAmount)) - amountFound;
        }
        
        
        public void CheckMoreIncludes(Item resource)
        {
            if (resource.ItemID == 0x1F9D && resource.Hue == 0x0000)
            {
                to_return.Add(new Dictionary<string, object>
                {
                    { "id", 0x0FF6 },
                    { "color", 0x0000 },
                    { "container", resource.Container },
                    { "x", resource.Position.X },
                    { "y", resource.Position.Y },
                    { "serial", resource.Serial },
                    { "tool", false }
                });
            }

            if (resource.ItemID == 0x0FF6 && resource.Hue == 0x0000)
            {
                to_return.Add(new Dictionary<string, object>
                {
                    { "id", 0x1F9D },
                    { "color", 0x0000 },
                    { "container", resource.Container },
                    { "x", resource.Position.X },
                    { "y", resource.Position.Y },
                    { "serial", resource.Serial },
                    { "tool", false }
                });
            }
        }
        public bool RefreshToReturn(Item resource, bool tool = false)
        {
            foreach (var item in to_return)
            {
                if (item["id"] == resource.ItemID && item["color"] == resource.Hue)
                    return false;
            }

           int itemSerial, int itemID, ushort hue, int containerSerial, (int X, int Y) locInContainer, 
            
            to_return.Add(new Dictionary<string, object>
            {
                { "id", resource.ItemID },
                { "color", resource.Hue },
                { "container", resource.Container },
                { "x", resource.Position.X },
                { "y", resource.Position.Y },
                { "serial", resource.Serial },
                { "tool", tool },
            });

            CheckMoreIncludes(resource);
            return true;
        }
        public void ReturnItems(BOD excludeBod = null)
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
        }
        public bool Trash(List<int> itemIdList, int color = -1, bool recursive = true)
        {
            if (itemIdList != null && itemIdList.Count > 0)
            {
                if (trashBarrel != null && Dist(trashBarrel) <= 2)
                {
                    Misc.SendMessage("Throwing items away.");
                    ThrowItems(FindItemsList(itemIdList, Player.Backpack, color, recursive), trashBarrel);
                }
                return true;
            }
            return false;
        }
        public string GetMaterialPrefix(string itemMaterial)
        {
            if (string.IsNullOrEmpty(itemMaterial) || 
                itemMaterial == "Iron" || 
                itemMaterial == "Leather/Hides" || 
                itemMaterial == "Wood")
            {
                return string.Empty;
            }
            return itemMaterial.Replace(" Hides", "").Replace("Gold", "Golden") + " ";
        }
            
        public void Recycle(string skill, string itemName, string itemMaterial)
        {
            Func<List<Item>> toRecycle = () =>
                FindItemsList(new List<string> { GetMaterialPrefix(itemMaterial) + itemName }, Player.Backpack)
                    .Where(i => !i.Properties.ToLower().Contains("exceptional"))
                    .ToList();

            if (!toRecycle().Any())
                return;

            var bsTool = GetTool("Blacksmithing", false);
            var forge = GetFirst(GetItemsByFilter(new List<int> { 0x00A9, 0x0FB1, 0x1986, 0x19A2, 0x197A, 0x197E, 0x198A, 0x199E, 0x1992, 0x1996 }));
            var salvageBag = GetFirst(FindItemsList(new List<string> { "Salvage Bag" }, Player.Backpack));

            if (bsTool != null && forge != null && salvageBag != null && (skill == "Blacksmithing" || skill == "Tailoring"))
            {
                ThrowItems(new List<Item> { bsTool }, Player.Backpack, -1, cont_xy["tools"]["x"], cont_xy["tools"]["y"]);
                Misc.SendMessage("\r\nSalvaging remaining pieces\r\n", 78);
                ThrowItems(toRecycle(), salvageBag);

                Misc.WaitForContext(salvageBag.Serial, 10000);

                if (skill == "Blacksmithing")
                {
                    Misc.ContextReply(salvageBag.Serial, 0);
                }
                else
                {
                    Misc.ContextReply(salvageBag.Serial, 1);
                }

                Misc.Pause(1000);
            }

            if (bsTool != null)
            {
                ThrowItems(new List<Item> { bsTool }, bsTool.Container, -1, cont_xy["tools"]["x"], cont_xy["tools"]["y"]);
            }
        }
        public Item GetGroundTool(string skill)
        {
            var names = new Dictionary<string, string>
            {
                { "Alchemy", "Alchemy Station" },
                { "Blacksmithing", "Smithing Press" },
                { "Carpentry", "Spinning Lathe" },
                { "Cooking", "BBQ Smoker" },
                { "Tailoring", "Sewing Machine" },
                { "Tinkering", "Tinker Bench" },
                { "Inscription", "Writing Desk" },
                { "Masonry", "Sculpture" },
                { "Glassblowing", "Kiln" }
            };

            var tool = GetFirst(GetItemsByFilter(null, names[skill]));

            if (tool != null)
            {
                var properties = Items.GetPropStringByIndex(tool.Serial, 1).ToLower();
                if (properties.Contains("uses remaining: "))
                {
                    var usesRemaining = int.Parse(properties.Split(new[] { "uses remaining: " }, StringSplitOptions.None)[1]);
                    if (usesRemaining > 0)
                        return tool;
                }
            }

            return null;
        }
        
        public Item GetTool(string skill, bool lookOnGround = true)
        {
            foreach (var item in new[] 
                     { 
                         Player.GetItemOnLayer("RightHand"), 
                         Player.GetItemOnLayer("LeftHand") 
                     })
            {
                if (item != null && SkillInfo[skill]["tools"].Keys.Contains(item.Name))
                    return item;
            }

            if (lookOnGround)
            {
                var tool = GetGroundTool(skill);
                if (tool != null)
                    return tool;
            }

            var salvageBag = GetFirst(FindItemsList(new List<string> { "Salvage Bag" }, Player.Backpack));
            var toolCodList = SkillInfo[skill]["tools"].Keys.ToList();
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
            }

            return null;
        }

        public bool OpenCraftGump(string skill)
        {
            if (crafting_gump_id > 0 && Gumps.WaitForGump(crafting_gump_id, 100) > 0)
                return true;

            Journal.Clear();
            var tool = GetTool(skill);

            if (tool != null)
            {
                while (true)
                {
                    Items.UseItem(tool);
                    var craftingGumpNewId = GetGumpNum(new List<string> { skill.ToUpper() + " MENU" });

                    if (craftingGumpNewId > 0)
                    {
                        crafting_gump_id = SetSharedGump("crafting_gump_id", craftingGumpNewId);
                        return true;
                    }
                }
            }

            return false;
        }
        public bool AnswerCraftGump(int menuButton, int? itemButton = null)
        {
            if (crafting_gump_id > 0 && Gumps.WaitForGump(crafting_gump_id, 100) > 0)
            {
                Gumps.SendAction(crafting_gump_id, menuButton);

                if (itemButton.HasValue)
                {
                    Gumps.WaitForGump(crafting_gump_id, 4000);
                    Gumps.SendAction(crafting_gump_id, itemButton.Value);
                }

                return true;
            }

            return false;
        }
        public string MakeOneItem(string skill, int menuButton, int itemButton)
        {
            if (OpenCraftGump(skill))
            {
                if (AnswerCraftGump(menuButton, itemButton))
                    return WaitCraftGump(crafting_gump_id, 8000);
            }
            else
            {
                return "OUT OF TOOLS.";
            }

            return "timeout";
        }
        
        public string MakeBodItems(Bod bod)
        {
            int GetMadeCount(int last)
            {
                var count = FindItemsList(new List<string> 
                { 
                    GetMaterialPrefix(bod.BaseMaterial) + bod.Item["name"] 
                }, Player.Backpack, -1, true, bod.Exceptional).Count;

                if (count == 0)
                {
                    count = FindItemsList(new List<string> 
                    { 
                        bod.MadeName 
                    }, Player.Backpack, -1, true, bod.Exceptional).Count;
                }

                return count + last;
            }

            int menuButton;
            Dictionary<string, object> itemData;
            (menuButton, itemData) = SearchButton(SkillInfo[bod.Skill]["gump"], bod.Item["name"]);

            bod.SetOldItems();
            int lastMade = 0, totalMade = 0;

            while (!Player.IsGhost)
            {
                bod.Refresh();

                if (Player.Weight >= Player.MaxWeight - 50)
                {
                    Misc.SendMessage("Dealing with overweight.", 78);
                    return "overweight";
                }

                bod.SetMadeItemName();

                if (Math.Max(totalMade, GetMadeCount(lastMade)) >= bod.ToMakeQuantity())
                {
                    Gumps.CloseGump(crafting_gump_id);
                    return string.Empty;
                }

                if (bod.Skill == "INSCRIPTION" && Player.Mana <= 40)
                    Meditate();

                string craftResult = MakeOneItem(bod.Skill, menuButton, (int)itemData["btn"]);

                if (craftResult.Contains("You must be near ") || 
                    craftResult.Contains("You do not have ") || 
                    craftResult.Contains("OUT OF TOOLS."))
                {
                    Gumps.CloseGump(crafting_gump_id);
                    return craftResult;
                }

                lastMade = 0;

                if (craftResult.Contains("create") && 
                    ((bod.Exceptional && craftResult.Contains("exceptional")) || (!bod.Exceptional)))
                {
                    totalMade++;
                    lastMade = 1;
                }

                if (craftResult == "timeout" || (bool)itemData["stack"])
                    return string.Empty;
            }

            return string.Empty;
        }
        
        public void ReturnGumpToDefault(string skill, string lastMaterial)
        {
            if (!string.IsNullOrEmpty(lastMaterial))
            {
                if ((skill == "Blacksmithing" || skill == "Tinkering") && lastMaterial != "Iron")
                    ChooseGumpMaterial(skill, "Iron");

                if (skill == "Tailoring" && lastMaterial != "Leather/Hides")
                    ChooseGumpMaterial(skill, "Leather/Hides");

                if ((skill == "Carpentry" || skill == "Bowcraft and Fletching") && lastMaterial != "Wood")
                    ChooseGumpMaterial(skill, "Wood");
            }
        }
        
        public string problem ChooseGumpMaterial(string skill, string materialName)
        {
            if (string.IsNullOrEmpty(materialName))
                return (string.Empty);

            try
            {
                UoTBods.SkillDetails materialData = SkillInfo[skill];
                //var materialData = SkillInfo[skill]["material"][materialName];

                if (OpenCraftGump(skill))
                {
                    if (!Gumps.LastGumpGetLineList().Contains($"|{materialName.ToUpper()}"))
                    {
                        AnswerCraftGump(7, (int)materialData["btn"]);
                        Gumps.WaitForGump(crafting_gump_id, 4000);
                    }

                    Gumps.CloseGump(crafting_gump_id);
                    return (string.Empty);
                }
                else
                {
                    return ("OUT OF TOOLS.");
                }
            }
            catch (Exception)
            {
                return (string.Empty);
            }
        }
            
        public void CombineBods(List<Bod> lbods, List<Bod> sbods)
        {
            if (lbods.Count > 0)
            {
                var sbodItemList = sbods.Select(sbod => sbod.Item).ToList();

                foreach (var lbod in lbods)
                {
                    bool ready = true;

                    foreach (var item in lbod.Items)
                    {
                        if (item["done_amount"] == 0 && 
                            !sbodItemList.Any(sbodItem => sbodItem["name"] == item["name"] && sbodItem["done_amount"] == lbod.TotalAmount))
                        {
                            ready = false;
                            break;
                        }
                    }

                    if (ready)
                    {
                        Misc.ClearIgnore();
                        lbod.Use();
                        lbod.Filled = true;
                        lbod.Finished();
                    }
                }
            }
        }
        public void MakeBod()
        {
            foreach (var skill in SkillInfo.Keys)
            {
                var sbods = new List<Bod>();
                var lbods = new List<Bod>();
                string material = string.Empty;
                string problem = string.Empty;

                while (!Player.IsGhost)
                {
                    var bod = new Bod(skill, Items.FindByID(0x2258,SkillInfo[skill]["bod_color"], Player.Backpack.Serial));
                    

                    if (Items.FindBySerial(bod.BodSerial) == null || problem == "OUT OF TOOLS.")
                    {
                        if (problem != "OUT OF TOOLS.")
                            ReturnGumpToDefault(skill, material);

                        break;
                    }

                    problem = string.Empty;

                    if (bod.Type == "large")
                    {
                        lbods.Add(bod);
                        continue;
                    }

                    if (bod.Finished())
                    {
                        sbods.Add(bod);
                        continue;
                    }

                    bod.Start();
                    ReturnItems(bod);
                    (material, problem) = ChooseGumpMaterial(skill, bod.BaseMaterial);

                    bod.GetItemsToFill();

                    while (!Player.IsGhost)
                    {
                        bod.Use();

                        if (!string.IsNullOrEmpty(problem))
                        {
                            if (problem != "OUT OF TOOLS." || 
                                (skill != "Blacksmithing" && skill != "Tailoring"))
                            {
                                Recycle(skill, bod.Item["name"], bod.BaseMaterial);
                            }

                            if (problem == "overweight")
                                problem = string.Empty;

                            if ((problem == "OUT OF TOOLS." && !OutOfResources("tools") && GetTool(skill) != null) || 
                                bod.GetResources(bod.GetResourcesData()))
                            {
                                problem = string.Empty;
                            }
                            else
                            {
                                Error(problem, skill, bod.Item["name"], false);
                                Misc.IgnoreObject(bod.BodSerial);
                                break;
                            }
                        }

                        bod.Refresh();

                        if (bod.Finished())
                        {
                            sbods.Add(bod);
                            Recycle(skill, bod.Item["name"], bod.BaseMaterial);
                            break;
                        }

                        problem = MakeBodItems(bod);
                    }
                }

                CombineBods(lbod, sbods);
            }

            ReturnItems();
            Gumps.CloseGump(crafting_gump_id);
            Gumps.CloseGump(GetGumpNum(new List<string> { "A bulk order", "A large bulk order" }, 500));
        }


                
                
// Helper class for Gump Items
public class GumpItem
{
    public int Button { get; }
    public bool Stack { get; }
    public List<(string Material, int Quantity)> Materials { get; }

    public GumpItem(int button, bool stack, List<(string Material, int Quantity)> materials)
    {
        Button = button;
        Stack = stack;
        Materials = materials;
    }
}


        public static class Globals
        {
            public static List<Dictionary<string, object>> ToReturn = new List<Dictionary<string, object>>();

            // Initializing container coordinates (cont_xy equivalent)
            public static Dictionary<string, Dictionary<string, int>> ContCoordinates = new Dictionary<string, Dictionary<string, int>>
            {
                { "resources", new Dictionary<string, int> { { "x", 200 }, { "y", 0 } } },
                { "tools", new Dictionary<string, int> { { "x", 50 }, { "y", 0 } } },
                { "stack", new Dictionary<string, int> { { "x", -1 }, { "y", -1 } } }
            };

            public static Serial ToContainer = Player.Backpack.Serial;
            public static List<Item> NearbyChests = new List<Item>();

            // Shared state variables (crafting_gump_id and bod_gump_id)
            public static int CraftingGumpId = (int)Misc.ReadSharedValue("crafting_gump_id");
            public static int BodGumpId = (int)Misc.ReadSharedValue("bod_gump_id");
        }

        public class Main
        {
            public static void Initialize()
            {
                // Call set_macro_containers if applicable.
                SetMacroContainers();

                // Clear ignored items.
                Misc.ClearIgnore();

                // Start processing BODs.
                MakeBod();
            }
        }
        
        
    }
    
    
}