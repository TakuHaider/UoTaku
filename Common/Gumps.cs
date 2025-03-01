using System;
using System.Collections.Generic;
using System.Linq;

namespace RazorEnhanced
{
    public static class UoTGumps
    {
		public static uint GetGumpId(List<string> textList, int timeout = 10000)
		{
		    for (int i = 0; i < timeout / 10; i++)
		    {
			    foreach (string text in textList)
			    {
				    if (Gumps.LastGumpTextExist(text))
				    {
					    return Gumps.CurrentGump();
				    }
			    }
			    Misc.Pause(10);
		    }
		    return 0;
		}

		public static bool CloseGump(uint gumpId, int timeout = 5000)
		{
			if (!Gumps.HasGump(gumpId)) return false;
			Gumps.CloseGump(gumpId);
			for (var i = 0; i < timeout/10; i++)
			{
				Misc.SendMessage("CloseGump: Closing Gump");
				if (!Gumps.HasGump(gumpId))
				{
					Misc.SendMessage("CloseGump: Gump closed");
					return true;
				}
				Misc.Pause(10);
			}
			return !Gumps.HasGump(gumpId);
		}

		public static bool WaitForGump(uint gumpId = 0, int timeout = 5000)
		{
			if (gumpId == 0)
			{
				if (Gumps.HasGump()) return true;
				for (var i = 0; i < timeout/10; i++)
				{
					if (Gumps.HasGump(gumpId))
					{
						return true;
					}
					Misc.Pause(10);
				}
				return false;
			}
		    var found = false;
		    // If gump is open then wait for it to close
		    if (Gumps.HasGump(gumpId))
		    {
			    for (var i = 0; i < timeout/10; i++)
			    {
				    //Misc.SendMessage("WaitForGump: Gump is already open");
				    if (!Gumps.HasGump(gumpId))
				    {
					    Misc.SendMessage("WaitForGump: Gump closed");
					    break;
				    }
				    Misc.Pause(10);
			    }
		    }
		    // Wait for gump to open
		    for (var i = 0; i < timeout/10; i++)
		    {
			    if (Gumps.HasGump(gumpId) || Gumps.CurrentGump() == gumpId)
			    {
				    found = true;
				    break;
			    }
			    Misc.Pause(10);
		    }
		    Misc.Pause(40);
		    if (!found) Misc.SendMessage("WaitForGump: Gump " + gumpId + " not found. Current: " + Gumps.CurrentGump());
		    return found;
		}
	    
		public static void SaveSkillGumpId(string skillName, Mobile target = null, Mobile secondTarget = null, bool waitSkill = true, int waitGump = 2000)
		{
		    var oldGumpIdsByKeys = Gumps.AllGumpIDs();
		    var currentGumpIdsByWorld = new List<uint>();
		    var oldGumpIdByWorld = Gumps.CurrentGump();
		    uint firstSkillGumpId = 0;
			    
		    // Use Skill
		    if (target == null) Player.UseSkill(skillName, waitSkill);
		    else Player.UseSkill(skillName, target, waitSkill);

		    // Wait for Gumps
		    for (var i = 0; i < waitGump/10; i++)
		    {
			    if (Gumps.CurrentGump() != oldGumpIdByWorld && !currentGumpIdsByWorld.Contains(Gumps.CurrentGump()))
				    currentGumpIdsByWorld.Add(Gumps.CurrentGump());
			    Misc.Pause(10);
		    }
		    
		    var currentGumpIdsByKeys = Gumps.AllGumpIDs();
		    currentGumpIdsByKeys = currentGumpIdsByKeys.Except(oldGumpIdsByKeys).ToList();
		    
		    // Process both lists: remove 0 and duplicates
		    var processedKeys = currentGumpIdsByKeys.Where(id => id != 0).Distinct();
		    var processedWorld = currentGumpIdsByWorld.Where(id => id != 0).Distinct();
		    if (processedKeys.Count() == 0 && processedWorld.Count() == 0)
		    {
			    UoTLogger.LogErrorToFile("No gump found");
		    }
		    else if (processedKeys.Count() == 0 && processedWorld.Count() > 0)
		    {
			    UoTLogger.LogErrorToFile("processedKeys failed!");
			    processedKeys = processedWorld;
		    }
		    else if (processedWorld.Count() == 0 && processedKeys.Count() > 0)
		    {
			    UoTLogger.LogErrorToFile("processedWorld failed!");
			    processedWorld = processedKeys;
		    }

		    // Compare the processed lists
		    if (!processedKeys.OrderBy(x => x).SequenceEqual(processedWorld.OrderBy(x => x)))
		    {
			    UoTLogger.LogErrorToFile("Gump capture mismatch!");
			    // Print the original lists since they are not equivalent
			    UoTLogger.LogErrorToFile("currentGumpIdsByKeys:");
			    foreach (var id in currentGumpIdsByKeys)
			    {
				    UoTLogger.LogErrorToFile(id+"");
			    }
			    UoTLogger.LogErrorToFile("currentGumpIdsByWorld:");
			    foreach (var id in currentGumpIdsByWorld)
			    {
				    UoTLogger.LogErrorToFile(id+"");
			    }
		    }

		    // Process results
		    if (processedKeys.Count() == 0)
		    {
			    Misc.SendMessage("Skill gump: none found");
			    UoTLogger.LogErrorToFile("Skill gump: none found");
			    firstSkillGumpId = 0;
		    }
		    if (processedKeys.Count() == 1)
		    {
			    Misc.SendMessage("Skill gump found");
			    UoTLogger.LogErrorToFile("Skill gump: none found");
			    firstSkillGumpId = processedKeys.FirstOrDefault();
		    }
		    else if (processedKeys.Count() > 1)
		    {
			    Misc.SendMessage("Skill Gump: Multiple gumps found");
			    UoTLogger.LogErrorToFile("Skill gump: none found");
			    firstSkillGumpId = processedKeys.FirstOrDefault();
		    }
		    
		    // Save results
		    var fileName = UoTJson.FileName;
		    if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
		    if (firstSkillGumpId != 0)
		    {
			    using (var storedData = new UoTJson(fileName))
			    {
				    storedData.StoreData(firstSkillGumpId, "skillGump:"+skillName+"_secondTarget:");
			    }
		    }
		    
		    // There's another?
		    if (secondTarget == null) 
		    {
			    if (firstSkillGumpId != 0) Gumps.CloseGump(firstSkillGumpId);
			    Gumps.ResetGump();
			    return;
		    }
		    // reinitialize 
			oldGumpIdsByKeys = Gumps.AllGumpIDs();
		    currentGumpIdsByWorld = new List<uint>();
		    oldGumpIdByWorld = Gumps.CurrentGump();
		    uint secondSkillGumpId = 0;
		    
		    // Use Skill
		    Target.WaitForTarget(2000, true);
		    Target.TargetExecute(secondTarget);

		    // Wait for Gumps
		    for (var i = 0; i < waitGump/10; i++)
		    {
			    if (Gumps.CurrentGump() != oldGumpIdByWorld && !currentGumpIdsByWorld.Contains(Gumps.CurrentGump()))
				    currentGumpIdsByWorld.Add(Gumps.CurrentGump());
			    Misc.Pause(10);
		    }
		    
		    // Filter old keys
		    currentGumpIdsByKeys = Gumps.AllGumpIDs();
		    currentGumpIdsByKeys = currentGumpIdsByKeys.Except(oldGumpIdsByKeys).ToList();
		    
		    // Process both lists: remove 0 and duplicates
		    processedKeys = currentGumpIdsByKeys.Where(id => id != 0).Distinct();
		    processedWorld = currentGumpIdsByWorld.Where(id => id != 0).Distinct();
		    if (processedKeys.Count() == 0 && processedWorld.Count() == 0)
		    {
			    UoTLogger.LogErrorToFile("No gump found");
		    }
		    else if (processedKeys.Count() == 0 && processedWorld.Count() > 0)
		    {
			    UoTLogger.LogErrorToFile("processedKeys failed!");
			    processedKeys = processedWorld;
		    }
		    else if (processedWorld.Count() == 0 && processedKeys.Count() > 0)
		    {
			    UoTLogger.LogErrorToFile("processedWorld failed!");
			    processedWorld = processedKeys;
		    }

		    // Compare the processed lists
		    if (!processedKeys.OrderBy(x => x).SequenceEqual(processedWorld.OrderBy(x => x)))
		    {
			    UoTLogger.LogErrorToFile("Gump capture mismatch!");
			    // Print the original lists since they are not equivalent
			    UoTLogger.LogErrorToFile("currentGumpIdsByKeys:");
			    foreach (var id in currentGumpIdsByKeys)
			    {
				    UoTLogger.LogErrorToFile(id+"");
			    }
			    UoTLogger.LogErrorToFile("currentGumpIdsByWorld:");
			    foreach (var id in currentGumpIdsByWorld)
			    {
				    UoTLogger.LogErrorToFile(id+"");
			    }
		    }

		    // Process results
		    if (processedKeys.Count() == 0)
		    {
			    Misc.SendMessage("Skill gump: none found");
			    UoTLogger.LogErrorToFile("Skill gump: none found");
			    secondSkillGumpId = 0;
		    }
		    if (processedKeys.Count() == 1)
		    {
			    Misc.SendMessage("Skill gump found");
			    UoTLogger.LogErrorToFile("Skill gump: none found");
			    secondSkillGumpId = processedKeys.FirstOrDefault();
		    }
		    else if (processedKeys.Count() > 1)
		    {
			    Misc.SendMessage("Skill Gump: Multiple gumps found");
			    UoTLogger.LogErrorToFile("Skill gump: none found");
			    secondSkillGumpId = processedKeys.FirstOrDefault();
		    }
		    fileName = UoTJson.FileName;
		    if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
		    if (secondSkillGumpId != 0)
		    {
			    using (var storedData = new UoTJson(fileName))
			    {
				    storedData.StoreData(secondSkillGumpId, "skillGump:"+skillName+"_secondTarget:");
			    }
		    }
		}
        public static uint GetSkillGumpId(string skillName)
        {
	        var fileName = UoTJson.FileName;
	        if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
	        return UoTJson.StoreOrGetJson<uint>("skillGump:"+skillName, default, fileName);
        }
        public static uint GetSkillGumpIdSecondTarget(string skillName)
        {
	        var fileName = UoTJson.FileName;
	        if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
	        return UoTJson.StoreOrGetJson<uint>("skillGump:" + skillName + "_secondTarget:", default, fileName);
        }
        
        public static uint GetMobileGumpId(int serial)
        {
	        var fileName = UoTJson.FileName;
	        var mobile = Mobiles.FindBySerial(serial);
	        if (mobile == null) return 0;
	        if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
	        return UoTJson.StoreOrGetJson<uint>("mobileGump:"+ mobile.ItemID + ":"+mobile.MobileID, default, fileName);
        }
        
        public static uint GetItemGumpId(Item item, List<int> buttons = null)
        {
	        if (item == null) return 0;
	        return GetItemGumpId(item.Serial, buttons);
        }
        public static uint GetItemGumpId(int itemId, List<int> buttons = null)
        {
	        var fileName = UoTJson.FileName;
	        if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
	        var item = Items.FindBySerial(itemId);
	        if (item == null) return 0;
	        if (buttons == null || buttons.Count == 0)
		        return UoTJson.StoreOrGetJson<uint>("itemGump:" + item.Name + "_" + item.ItemID);
	        return UoTJson.StoreOrGetJson<uint>("itemGump:"+ item.Name + "_" +item.ItemID + 
	                                            "-button:" + string.Join("-", buttons), default, fileName);
        }
        
        public static uint SaveMobileGumpId(int serial)
        {
	        uint currentGump = 0;
	        var mobile = Mobiles.FindBySerial(serial);
	        if (mobile == null) return 0;
	        var oldGump = Gumps.CurrentGump();
	        var newGump = oldGump;
	        Mobiles.UseMobile(mobile);
	        for (var i = 0; i < 500; i++)
	        {
		        newGump = Gumps.CurrentGump();
		        if (oldGump != newGump  && newGump != 0) break;
		        Misc.Pause(10);
	        }
	        if (oldGump == newGump) newGump = 0;
	        else Gumps.CloseGump(newGump);
	        var fileName = UoTJson.FileName;
	        if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
	        UoTJson.StoreOrGetJson<uint>("mobileGump:"+ mobile.ItemID + ":"+mobile.MobileID, newGump, fileName);
	        return newGump;
        }

        public static void SaveItemGumpId(Item item, List<int> buttons = null, int waitGump = 5000)
        {
			if (item == null) return;
		    var oldGumpIdsByKeys = Gumps.AllGumpIDs();
		    var currentGumpIdsByWorld = new List<uint>();
		    var oldGumpIdByWorld = Gumps.CurrentGump();
		    uint newGump = 0;
			    
		    // Use Item
		    Misc.Pause(2000);
		    Items.UseItem(item);

		    // Wait for Gumps
		    for (var i = 0; i < waitGump/10; i++)
		    {
			    var currentGump = Gumps.CurrentGump();
			    if (currentGump != oldGumpIdByWorld && !currentGumpIdsByWorld.Contains(currentGump))
				    currentGumpIdsByWorld.Add(currentGump);
			    Misc.Pause(10);
		    }
		    
		    var currentGumpIdsByKeys = Gumps.AllGumpIDs();
		    currentGumpIdsByKeys = currentGumpIdsByKeys.Except(oldGumpIdsByKeys).ToList();
		    
		    // Process both lists: remove 0 and duplicates
		    var processedKeys = currentGumpIdsByKeys.Where(id => id != 0).Distinct();
		    var processedWorld = currentGumpIdsByWorld.Where(id => id != 0).Distinct();
		    if (processedKeys.Count() == 0 && processedWorld.Count() == 0)
		    {
			    Misc.SendMessage("No gump found!!!");
			    UoTLogger.LogErrorToFile("No gump found");
		    }
		    else if (processedKeys.Count() == 0 && processedWorld.Count() > 0)
		    {
			    Misc.SendMessage("processedKeys failed!");
			    UoTLogger.LogErrorToFile("processedKeys failed!");
			    processedKeys = processedWorld;
		    }
		    else if (processedWorld.Count() == 0 && processedKeys.Count() > 0)
		    {
			    Misc.SendMessage("processedWorld failed!");
			    UoTLogger.LogErrorToFile("processedWorld failed!");
			    processedKeys = processedWorld;
		    }

		    // Compare the processed lists
		    if (!processedKeys.OrderBy(x => x).SequenceEqual(processedWorld.OrderBy(x => x)))
		    {
			    UoTLogger.LogErrorToFile("Gump capture mismatch!");
			    // Print the original lists since they are not equivalent
			    UoTLogger.LogErrorToFile("currentGumpIdsByKeys:");
			    foreach (var id in currentGumpIdsByKeys)
			    {
				    UoTLogger.LogErrorToFile(id+"");
			    }
			    UoTLogger.LogErrorToFile("currentGumpIdsByWorld:");
			    foreach (var id in currentGumpIdsByWorld)
			    {
				    UoTLogger.LogErrorToFile(id+"");
			    }
		    }
		    
		    // Process results
		    if (processedKeys.Count() == 0)
		    {
			    Misc.SendMessage("Item gump: none found");
			    newGump = 0;
			    Gumps.ResetGump();
		    }
		    if (processedKeys.Count() == 1)
		    {
			    Misc.SendMessage("Item gump found");
			    newGump = processedKeys.FirstOrDefault();
			    if (buttons == null || buttons.Count == 0) Gumps.CloseGump(newGump);
		    }
		    else if (processedKeys.Count() > 1)
		    {
			    Misc.SendMessage("Item Gump: Multiple gumps found");
			    newGump = processedKeys.FirstOrDefault();
			    if (buttons == null || buttons.Count == 0) Gumps.ResetGump();
		    }
		    
		    // Save results
		    var fileName = UoTJson.FileName;
		    if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
		    if (newGump != 0)
		    {
			    using (var storedData = new UoTJson(fileName))
			    {
				    storedData.StoreData(newGump, "itemGump:"+ item.Name + "_" +item.ItemID);
			    }
		    }
		    
		    // If there are no buttons, return
		    if (buttons == null || buttons.Count == 0 || newGump == 0)
		    {
			    if (newGump != 0) Gumps.CloseGump(newGump);
			    Gumps.ResetGump();
			    return;
		    }
		    var firstItemGumpId = newGump;

		    foreach (var button in buttons)
		    {
			    Misc.Pause(500);
			    if (button == buttons.Last()) continue;
			    UoTGumps.WaitForGump(firstItemGumpId);
			    Misc.Pause(500);
			    Gumps.SendAction(firstItemGumpId, button);
		    }
		    
		    
		    // Reinitialize 
			oldGumpIdsByKeys = Gumps.AllGumpIDs();
		    currentGumpIdsByWorld = new List<uint>();
		    oldGumpIdByWorld = Gumps.CurrentGump();
		    uint secondItemGumpId = 0;
		    
		    // Use Item gump
		    Gumps.SendAction(firstItemGumpId, buttons.Last());

		    // Wait for Gumps
		    for (var i = 0; i < waitGump/10; i++)
		    {
			    if (Gumps.CurrentGump() != oldGumpIdByWorld && !currentGumpIdsByWorld.Contains(Gumps.CurrentGump()))
				    currentGumpIdsByWorld.Add(Gumps.CurrentGump());
			    Misc.Pause(10);
		    }
		    
		    // Filter old keys
		    currentGumpIdsByKeys = Gumps.AllGumpIDs();
		    currentGumpIdsByKeys = currentGumpIdsByKeys.Except(oldGumpIdsByKeys).ToList();
		    
		    // Process both lists: remove 0 and duplicates
		    processedKeys = currentGumpIdsByKeys.Where(id => id != 0).Distinct();
		    processedWorld = currentGumpIdsByWorld.Where(id => id != 0).Distinct();
		    if (processedKeys.Count() == 0 && processedWorld.Count() == 0)
		    {
			    UoTLogger.LogErrorToFile("No gump found");
		    }
		    else if (processedKeys.Count() == 0 && processedWorld.Count() > 0)
		    {
			    UoTLogger.LogErrorToFile("processedKeys failed!");
			    processedKeys = processedWorld;
		    }
		    else if (processedWorld.Count() == 0 && processedKeys.Count() > 0)
		    {
			    UoTLogger.LogErrorToFile("processedWorld failed!");
			    processedWorld = processedKeys;
		    }

		    // Compare the processed lists
		    if (!processedKeys.OrderBy(x => x).SequenceEqual(processedWorld.OrderBy(x => x)))
		    {
			    UoTLogger.LogErrorToFile("Gump capture mismatch!");
			    // Print the original lists since they are not equivalent
			    UoTLogger.LogErrorToFile("currentGumpIdsByKeys:");
			    foreach (var id in currentGumpIdsByKeys)
			    {
				    UoTLogger.LogErrorToFile(id+"");
			    }
			    UoTLogger.LogErrorToFile("currentGumpIdsByWorld:");
			    foreach (var id in currentGumpIdsByWorld)
			    {
				    UoTLogger.LogErrorToFile(id+"");
			    }
		    }

		    // Results
		    if (processedKeys.Count() == 0)
		    {
			    Misc.SendMessage("Item submenu gump: none found");
			    secondItemGumpId = 0;
		    }
		    if (processedKeys.Count() == 1)
		    {
			    Misc.SendMessage("Item submenu gump found");
			    secondItemGumpId = processedKeys.FirstOrDefault();
		    }
		    else if (processedKeys.Count() > 1)
		    {
			    Misc.SendMessage("Item submenu Gump: Multiple gumps found");
			    secondItemGumpId = processedKeys.FirstOrDefault();
		    }
		    fileName = UoTJson.FileName;
		    if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
		    if (secondItemGumpId != 0)
		    {
			    using (var storedData = new UoTJson(fileName))
			    {
				    storedData.StoreData(secondItemGumpId, "itemGump:"+ item.Name + "_" +item.ItemID + 
				                                           "-button:" + string.Join("-", buttons));
			    }	
				Gumps.CloseGump(secondItemGumpId);
		    }
		    Gumps.ResetGump();
        }
        
        public static uint OpenCraftGump(string skill)
        {
	        UoTLogger.LogErrorToFile("OpenCraftGump: " + skill);
	        var tool = UoTItems.GetTool(skill);
	        if (tool != null)
	        {
		        for (var i = 0; i < 10; i++)
		        {
			        Items.UseItem(tool);
                    var craftingGumpNewId = SaveGumpIdByText(new List<string> { skill.ToUpper() + " MENU" });
                    if (craftingGumpNewId > 0)
                    {
	                    try
	                    {
	                        var fileName = UoTJson.FileName;
	                        if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
	                        using (var storedData = new UoTJson(fileName))
	                        {
	                            storedData.StoreData(craftingGumpNewId, "itemGump:" + tool.Name + "_" +tool.ItemID);
	                            storedData.StoreData(craftingGumpNewId, "skillGump:" + skill);
	                        }
	                    }
	                    catch (Exception e)
	                    {
		                    UoTLogger.LogErrorToFile("Storing skill gump", e);
	                    }
                        return craftingGumpNewId;
                    }
                    Misc.Pause(600);
		        }
	        }
	        return 0;
        }
        public static uint SaveGumpIdByText(string text, int timeout = 10000)
        {
	        return SaveGumpIdByText(new List<string> { text }, timeout);
        }
        public static uint SaveGumpIdByText(List<string> textList, int timeout = 10000)
        {
	        for (int i = 0; i < timeout / 10; i++)
	        {
		        foreach (var text in textList)
		        {
			        if (Gumps.LastGumpTextExist(text))
			        {
				        return Gumps.CurrentGump();
			        }
		        }
		        Misc.Pause(10);
	        }

	        return 0;
        }
        public static void SaveItemGumpId(int serial)
        {
	        if (serial == 0) return;
            var item = Items.FindBySerial(serial);
            if (item == null) return;
            SaveItemGumpId(item);
        }
        
        public static (int, int) SearchButton(IReadOnlyCollection<UoTBods.Category> gump, string itemName)
        {
	        foreach (var category in gump)
	        {
		        foreach (var gumpItem in category.Recipes)
		        {
			        if (gumpItem.Name == itemName || gumpItem.Name.Replace("(", "").Replace(")", "") == itemName)
				        return (category.ButtonId, gumpItem.ButtonId);
		        }
	        }
	        return (0, 0); // Default return in case of error
        }
        
        //Maybe change to return new gumpid
        public static bool AnswerGump(uint gump, int menuButton, int itemButton)
        {
	        if (!WaitForGump(gump)) return false;
	        Gumps.SendAction(gump, menuButton);
	        UoTGumps.WaitForGump(gump);
	        Gumps.SendAction(gump, itemButton);
	        return true;
        }

        /// <summary>
        /// Finds a string in the last opened gump
        /// </summary>
        /// <param name="text">Message to be found</param>
        /// <returns>true if is found</returns>
        public static bool FindTextInLastGump(string text)
        {
	        foreach (string line in Gumps.LastGumpGetLineList())
	        {
		        if (line.ToLower().Contains(text))
		        {
			        return true;
		        }
	        }
	        return false;
        }
        
        
    }
}