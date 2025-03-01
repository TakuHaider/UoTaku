using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static RazorEnhanced.UoTiles;

namespace RazorEnhanced
{
    public static class UoTPlayer
    {
	    public static bool CheckPlayerInDungeon()
        {
            /*
             * Uses the Player's X and Y coordinates to determine if they are in a dungeon
             */

            if (Player.Position.X < 5120)
            {
                // Player is west of the dungeon cutoff
                return false;
            }

            if (Player.Position.Y < 2305)
            {
                // Player is east of the dungeon cutoff and to the north of the Lost Lands
                return true;
            }

            if (Player.Position.X > 6140)
            {
                // Player is east of the Lost Lands
                return true;
            }
            //Player is in the Lost Lands
            return false;
        }
        //did not work for provoke
        public static void UseSkill(string skillName, Mobile target = null, Mobile secondTarget = null, bool waitSkill = true)
        {
            if (target == null) Player.UseSkill(skillName, waitSkill);
            else Player.UseSkill(skillName, target, waitSkill);
            //maybe longer wait
            Misc.Pause(600);
            if (secondTarget == null) return;
            Target.WaitForTarget(2000, false);
            Target.TargetExecute(secondTarget);
        }
        public static void UseSkillClicks(string skillName, Mobile target = null, Mobile secondTarget = null, bool waitSkill = true)
        {
            if(Target.HasTarget()) Target.Cancel();
            Misc.Pause(600);
            Player.UseSkill(skillName, waitSkill);
            if (target == null) return;
            //maybe longer wait
            //Attempt to stop target lock
            Target.WaitForTarget(2000, false);
            Target.TargetExecute(target);
            if (secondTarget == null) return;
            Misc.Pause(600);
            Target.WaitForTarget(2000, false);
            Target.TargetExecute(secondTarget);
        }
        
        public static int GetRecallRuneIndex()
        {
	        var fileName = UoTJson.FileName;
	        if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
	        using (var storedData = new UoTJson(fileName))
	        {
		        var index = storedData.GetData<int>("recall_rune_index", typeof(int));
	            if (index is < 3 or > 15) index = 2;
	            return index;
	        }

        }
        public static int UpdateRecallRuneIndex(int indexSet = int.MinValue)
        {
	        var fileName = UoTJson.FileName;
			if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
	        using (var storedData = new UoTJson(fileName))
	        {
		        if (indexSet != int.MinValue)
		        {
			        storedData.StoreData((int)indexSet, "recall_rune_index");
			        return indexSet;
		        }
				var index = storedData.GetData<int>("recall_rune_index", typeof(int));
				if (index is 0 or 1 or 16)
				{
				    index = 2;
				    //Keep track of books and switch
				    //SwitchBook();
				}
				else index += 1;
				storedData.StoreData((int)index, "recall_rune_index");
				return index;
	        }
        }
        
        public static void RecallWithBook(int bookIndex = 1, int runeIndex = 1)
        {
            // Wait until player's mana is at least 10
            while (Player.Mana < 10)
            {
                Misc.Pause(20);
            }
            UoTasks.StopTask("Pathing");
            var fileName = UoTJson.FileName;
            if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
            var bookSerial = UoTJson.StoreOrGetJsonByIndex<int>("book", bookIndex, default, fileName);
            var book = Items.FindBySerial(bookSerial);
            if (book == null)
            {
	            Misc.SendMessage("Book not found");
	            var promptValue = UoTItems.target.PromptTarget("Select runic atlas");
	            Target.WaitForTarget(5000);
	            if (promptValue == 0) return;
	            bookSerial = UoTJson.StoreOrGetJsonByIndex<int>("book", bookIndex, promptValue);
	            book = Items.FindBySerial(bookSerial);
	            if (book == null) return;
            }
            var bookGump = UoTGumps.GetItemGumpId(bookSerial);
            if (bookGump == 0) UoTGumps.SaveItemGumpId(bookSerial);
            bookGump = UoTGumps.GetItemGumpId(bookSerial);
            var loc = (Player.Position.X, Player.Position.Y);
			while (Player.Position.X == loc.X && Player.Position.Y == loc.Y)
			{
				Misc.Resync();
				Misc.Pause(650);
				Items.UseItem(bookSerial);
				if (!UoTGumps.WaitForGump(bookGump, 2000))
				{
					Items.UseItem(bookSerial);
					UoTGumps.WaitForGump(bookGump, 2000);
				}
				Misc.Pause(500);
				var bookText = Gumps.GetLineList(bookGump);
				var countEmpty = 0;
				foreach (var line in bookText)
				{
					if (string.IsNullOrEmpty(line) || line.Contains("Empty")) countEmpty++;
					UoTLogger.LogErrorToFile(line);
				}
				Misc.SendMessage("Empty lines: " + countEmpty);
				//Misc.SendMessage("index: " + runeIndex);
				if (runeIndex > 16 - countEmpty)
				{
					UpdateRecallRuneIndex(2);
					runeIndex = 2;
					Misc.SendMessage("Rune setting index 2");
					Misc.Pause(5);
				}
				Misc.SendMessage("index: " + runeIndex);
				if (Player.GetSkillValue("Magery") > 24)
				{
				    Gumps.SendAction(bookGump, 99 + runeIndex); // +1 for each rune
				    UoTGumps.WaitForGump(bookGump, 10000); // Wait for the gump to open again
				    Gumps.SendAction(bookGump, 4); // Cast recall using magery
				    Misc.Pause(3500); // Small pause after casting
				}
				else if (Player.GetSkillValue("Chivalry") > 20)
				{
					Gumps.SendAction(bookGump, 99 + runeIndex); // +1 for each rune
				    UoTGumps.WaitForGump(bookGump, 10000);
				    Gumps.SendAction(bookGump, 7); // Cast Sacred Journey using Chivalry
				    Misc.Pause(3500);
				}
				else //if (useScrollsCharges)
				{
				    Gumps.SendAction(bookGump, 99 + runeIndex); // +1 for each rune
				    UoTGumps.WaitForGump(bookGump, 10000);
				    Gumps.SendAction(bookGump, 5); // Use scroll/charge
				    Misc.Pause(3500);
				}
			}
			
        }

    }
    
}