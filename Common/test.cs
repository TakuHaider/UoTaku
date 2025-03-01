

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
//#import <../Common/Recipes.cs>
//#import <../Common/Player.cs>
//#import <../Common/System.cs>
//#import <../Common/Tasks.cs>
//#import <../Common/Tiles.cs>
//#import <../Lists/Ore.cs> 
//#import <../Lists/Misc.cs> 
//#import <../Lists/Mobiles.cs>
//#import <../Lists/Tools.cs>
//#import <../Lists/Wood.cs>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.Runtime.CompilerServices
{
    internal static partial class IsExternalInit {}
}
namespace RazorEnhanced
{
    public class Test
    { 
        public static void Run()
        {
            try
            {
                
                var recipes = new UoTRecipes(); 
                recipes.SearchProfessionsAll();

                //while (!Player.IsGhost)
               
            }
            catch (Exception e)
            {
                UoTLogger.LogErrorToFile($"Unexpected error: {e.Message}", e);
            }
            
        }


        public static void ProcessGump()
        {
            
        }

        public static void PrintGumpDump(Gumps.GumpData gumpData)
        {
            
            UoTLogger.LogErrorToFile("GumpData Id: " + gumpData.gumpId);
            UoTLogger.LogErrorToFile("GumpData serial: " + gumpData.serial);
            UoTLogger.LogErrorToFile("GumpData buttonId: " + gumpData.buttonid);
            UoTLogger.LogErrorToFile("GumpData hasResponse: " + gumpData.hasResponse);
            UoTLogger.LogErrorToFile("GumpData Definition: " + gumpData.gumpDefinition);
            UoTLogger.LogErrorToFile("GumpData textID: ");
            foreach (var line in gumpData.textID)
            {
                UoTLogger.LogErrorToFile(line + "");
            }
            
            UoTLogger.LogErrorToFile("GumpData switches: ");
            foreach (var line in gumpData.switches)
            {
                UoTLogger.LogErrorToFile(line + "");
            }
            
            
            
            if (gumpData.stringList != null)
            {
                UoTLogger.LogErrorToFile("gumpData.stringList");
                foreach (var line in gumpData.stringList)
                {
                    if (line != null)
                        UoTLogger.LogErrorToFile(line);
                }
            }
            
            
            if (gumpData.gumpStrings != null)
            {
                UoTLogger.LogErrorToFile("gumpData.gumpStrings");
                foreach (var line in gumpData.gumpStrings)
                {
                    if (line != null)
                        UoTLogger.LogErrorToFile(line);
                }
            }
            

            /*
            if (gumpData.layoutPieces != null)
            {
                UoTLogger.LogErrorToFile("gumpData.layoutPieces ");
                foreach (var line in gumpData.layoutPieces)
                    UoTLogger.LogErrorToFile(line);
            }
            */
            
            if (gumpData.gumpLayout != null)
            {
                UoTLogger.LogErrorToFile("gumpData.gumpLayout ");
                UoTLogger.LogErrorToFile(gumpData.gumpLayout);
            }
             
            if (gumpData.gumpText != null)
            {
                UoTLogger.LogErrorToFile("gumpData.gumpText");
                foreach (var line in gumpData.gumpText)
                {
                    if (line != null)
                        UoTLogger.LogErrorToFile(line);
                }
            }
            
            if (gumpData.gumpData != null)
            {
                UoTLogger.LogErrorToFile("gumpData.gumpData");
                foreach (var line in gumpData.gumpData)
                {
                    if (line != null)
                        UoTLogger.LogErrorToFile(line);
                }
            }

            /*
            UoTLogger.LogErrorToFile("All gumps dump");
            var gumpIds = Gumps.AllGumpIDs();
            foreach (var id in gumpIds)
            {
                UoTLogger.LogErrorToFile("Gump ID: " + id);
                var rawText = Gumps.GetGumpRawText(id);
                foreach (var line in rawText)
                {
                    if (line != null)
                        UoTLogger.LogErrorToFile(line);
                }
            }
            */
            
                        
        }

        public static Dictionary<string, int> ReadMaterialAmounts(Gumps.GumpData gumpData)
        {
            
            var amounts = ParseAmounts(gumpData.gumpLayout);
            var materials = ParseMaterials(gumpData.gumpText);
            return CombineMaterialsAndAmounts(materials, amounts);
            
        }

        
        public static Dictionary<string, int> CombineMaterialsAndAmounts(List<string> materials, List<int> amounts)
        {
            var result = new Dictionary<string, int>();

            for (int i = 0; i < materials.Count && i < amounts.Count; i++)
            {
                if (!result.ContainsKey(materials[i]))
                {
                    result[materials[i]] = 0;
                }

                result[materials[i]] += amounts[i];
            }

            return result;
        }

        public static List<string> ParseMaterials(List<string> text)
        {
            var materials = new List<string>();
            foreach (var line in text)
            {
                // Look for lines containing material names with (~1_AMT~)
                if (line.Contains("(~1_AMT~)"))
                {
                    // Extract the material name by removing the (~1_AMT~) part
                    var material = line.Replace("(~1_AMT~)", "").Trim();
                    materials.Add(material);
                }
            }
            return materials;
        }


        public static List<int> ParseAmounts(string layout)
        {
            var amounts = new List<int>();
            var pattern = @"@(\d+)@";

            var matches = Regex.Matches(layout, pattern);
            foreach (Match match in matches)
            {
                if (int.TryParse(match.Groups[1].Value, out int amount))
                {
                    amounts.Add(amount);
                }
            }
            return amounts;
        }
        
        public static void ReadButtons(Gumps.GumpData gumpData)
        {
            if (gumpData == null) return;
            var allTexts = gumpData.gumpText
                .Where(t => !t.StartsWith("<CENTER>") && !t.EndsWith("</CENTER>"))
                .ToList();
            
            var layout = gumpData.gumpLayout;
            var buttonTextMap = new Dictionary<int, string> ();
            
            // Locate "LAST TEN" in the gumpText list to dynamically determine the starting index for Categories
            int dynamicTextStartIndex = allTexts.IndexOf("LAST TEN");
            // If not main crafting gump then dump all buttons.
            if (dynamicTextStartIndex == -1)
            {
                var anyButtons = Regex.Matches(layout, @"button (\d+) (\d+) 40\d+ 40\d+ 1 0 (\d+)")
                    .Cast<Match>()
                    .Select(m => new { 
                        X = int.Parse(m.Groups[1].Value),
                        Y = int.Parse(m.Groups[2].Value), 
                        ButtonId = int.Parse(m.Groups[3].Value) 
                    })
                    .ToList();

                var index = 0;
                foreach (var button in anyButtons)
                {
                    if (index > allTexts.Count - 1) break;
                    UoTLogger.LogErrorToFile($"X: {button.X} Y: {button.Y}, Button ID: {button.ButtonId}, Text: {allTexts[index]}");
                    index++;
                }
                return;
            }
            
            UoTLogger.LogErrorToFile("Processing buttons for main crafting gump");
            // Handle the special "Material Type" button (Button ID: 7)
            var materialTypeButton = Regex.Match(layout, @"button 15 (\d+) 4005 4007 1 0 7");
            if (materialTypeButton.Success)
            {
                var materialAmounts = ReadMaterialAmounts(gumpData);
                var materialButtonY = int.Parse(materialTypeButton.Groups[1].Value); // Y-coordinate of the material button
                var materialText =  allTexts[dynamicTextStartIndex-1];
                var materialName = materialText.Replace("(~1_AMT~)", "").Trim();
                UoTLogger.LogErrorToFile(materialAmounts.TryGetValue(materialName, out var amount)
                    ? $"Button - Y: {materialButtonY}, Button ID: 7, Material: {materialName}, Amount: {amount}"
                    : $"Material Type Button - Y: {materialButtonY}, Button ID: 7, Text: {materialText} \n", null, false);
            }
            
            // LEFT COLUMN BUTTONS: Positions at X = 15
            var leftColumnButtons = Regex.Matches(layout, @"button 15 (\d+) 4005 4007 1 0 (\d+)")
                .Cast<Match>()
                .Select(m => new
                {
                    Y = int.Parse(m.Groups[1].Value), // Button's Y-coordinate
                    ButtonId = int.Parse(m.Groups[2].Value) // Button ID
                })
                .Where(m => m.Y <= 300 )
                .ToList();

            // RIGHT COLUMN BUTTONS: Positions at X = 220
            var rightColumnButtons = Regex.Matches(layout, @"button 220 (\d+) 4005 4007 1 0 (\d+)")
                .Cast<Match>()
                .Select(m => new
                {
                    Y = int.Parse(m.Groups[1].Value), // Button's Y-coordinate
                    ButtonId = int.Parse(m.Groups[2].Value) // Button ID
                })
                .Where(m => m.Y <= 300 )
                .ToList();

            
            // Get other buttons
            var utilityButtons = Regex.Matches(layout, @"button (\d+) (\d+) 40\d+ 40\d+ 1 0 (\d+)")
                .Cast<Match>()
                .Select(m => new { 
                    X = int.Parse(m.Groups[1].Value),
                    Y = int.Parse(m.Groups[2].Value), 
                    ButtonId = int.Parse(m.Groups[3].Value) 
                })
                .Where(b => b.ButtonId != 7 && (b.X != 15 && b.X != 220 && b.X != 480 || b.Y > 300)) // Exclude left and right column buttons
                .ToList();


            // Process category and item buttons (left and right columns)
            var dynamicTextIndex = dynamicTextStartIndex;

            UoTLogger.LogErrorToFile("\nLeft Column (X: 15) \n", null, false);
            UoTLogger.LogErrorToFile("------------------------ \n", null, false);
            foreach (var button in leftColumnButtons)
            {
                var text = dynamicTextIndex < allTexts.Count ? allTexts[dynamicTextIndex++] : "Unknown";
                UoTLogger.LogErrorToFile($"Left Column - Y: {button.Y}, Button ID: {button.ButtonId}, Text: {text} \n", null, false);
            }

            UoTLogger.LogErrorToFile("\nRight Column (X: 220) \n", null, false); //X: 480 for menu
            UoTLogger.LogErrorToFile("------------------------ \n", null, false);
            foreach (var button in rightColumnButtons)
            {
                var text = dynamicTextIndex < allTexts.Count ? allTexts[dynamicTextIndex++] : "Unknown";
                UoTLogger.LogErrorToFile($"Right Column - Y: {button.Y}, Button ID: {button.ButtonId}, Text: {text} \n", null, false);
            }
            
            // Process utility buttons with their corresponding texts
            UoTLogger.LogErrorToFile("\nOther Buttons \n", null, false);
            UoTLogger.LogErrorToFile("------------------------ \n", null, false);
            foreach (var button in utilityButtons)
            {
                int index = utilityButtons.IndexOf(button);
                // Skip the "EXIT" text and categories/items, wrap around to the start
                var textIndex = index;
                var text = textIndex < allTexts.Count 
                    ? allTexts[textIndex] 
                    : allTexts[textIndex % allTexts.Count]; // Wrap around using modulo
                UoTLogger.LogErrorToFile($"X: {button.X} Y: {button.Y}, Button ID: {button.ButtonId}, Text: {text} \n", null, false);
            }
            
            
        }
        
        
    }
}


