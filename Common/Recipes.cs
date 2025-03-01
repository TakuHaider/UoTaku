

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace RazorEnhanced
{
    public class UoTRecipes
    {
        private readonly string _fileName;
        /*
        public class RecipeBookData
        {
            public RecipeBookData()
            {
                Professions = new List<CraftProfession>();
                
            }
            public List<CraftProfession> Professions { get; set; }
        }
        */
        
        /*
        public record CraftProfession
        {
            public CraftProfession(string name, List<CraftCategory> categories)
            {
                Name = name;
                Categories = categories ?? new List<CraftCategory>();
            }
            public string Name { get; init; }
            public List<CraftCategory> Categories { get; init; }
        }
        */
        
        public static List<CraftProfession> RecipeBook;
        public class CraftProfession
        {
            public string Name { get; }
            public List<CraftRecipe> Recipes { get; set; }
            public CraftProfession(string name, List<CraftRecipe> recipes)
            {
                Name = name;
                Recipes = recipes;
            }
        }
        public class CraftRecipe
        {
            public string Profession { get; }
            public string Category { get; }
            public int CategoryId { get; set; }
            public string Name { get; }
            public int SkillRequired { get; }
            public int ChanceOfSuccess { get; }
            public int ExceptionalChance { get; }
            public bool Known { get; }
            public bool Color { get; }
            public List<(string Material, int Quantity)> Ingredients { get; }

            public CraftRecipe(string profession, string category, int categoryId, string name, int skillRequired,
                int chanceOfSuccess, int exceptionalChance, bool known, bool color,
                List<(string Material, int Quantity)> ingredients)
            {
                Profession = profession;
                Category = category;
                CategoryId = categoryId;
                Name = name;
                SkillRequired = skillRequired;
                ChanceOfSuccess = chanceOfSuccess;
                ExceptionalChance = exceptionalChance;
                Known = known;
                Color = color;
                Ingredients = ingredients;
            }

        }

        public UoTRecipes(string fileName = "recipes")
        {
            _fileName = fileName;
            UoTJson.FileName = fileName;
            LoadProfessions();
            if (RecipeBook == null)
            {
                RecipeBook = new();
            }
            SaveProfessions();
        }

        public void LoadProfessions()
        {
            using (var storedData = new UoTJson(_fileName))
            {
                RecipeBook = storedData.GetData<List<CraftProfession>>("RecipeBook", typeof(List<CraftProfession>));
            }
        }

        public void SaveProfessions()
        {
            using (var storedData = new UoTJson(_fileName))
            {
                storedData.StoreData(RecipeBook, "RecipeBook");
            }
        }

        public void SearchProfessionsAll()
        {
            foreach (var profession in UoTItems.CraftingTools.Keys)
            {
                UoTLogger.LogErrorToFile("Searching profession: " + profession);
                var craftingGumpId = UoTGumps.OpenCraftGump(profession);
                if (craftingGumpId == 0) continue;
                //Items.UseItem(tool);
                var gumpData = Gumps.GetGumpData(craftingGumpId);
                if (gumpData == null) continue;
                //add check to menu name
                var text = gumpData.gumpText;
                var layout = gumpData.gumpLayout;
                //ReadButtons(gumpData);
                var index = text.IndexOf("LAST TEN");
                
                var leftColumnButtons = Regex.Matches(layout, @"button 15 (\d+) 4005 4007 1 0 (\d+)")
                    .Cast<Match>()
                    .Select(m => new
                    {
                        Y = int.Parse(m.Groups[1].Value), // Button's Y-coordinate
                        ButtonId = int.Parse(m.Groups[2].Value) // Button ID
                    })
                    .Where(m => m.Y <= 300 )
                    .ToList();
                // Track category names and recipe names separately
                var recipeStartIndex = index + leftColumnButtons.Count;  // Recipes start after categories
                foreach (var category in leftColumnButtons)
                {
                    if (!Gumps.HasGump(craftingGumpId)) craftingGumpId = UoTGumps.OpenCraftGump(profession);
                    // First button is not a category
                    if (index == text.IndexOf("LAST TEN"))
                    {
                        index++;
                        continue;
                    }
                    var categoryText = index < text.Count ? text[index++] : "Unknown";
                    UoTLogger.LogErrorToFile("Searching category: " + categoryText + " in gump: " + craftingGumpId + " with button id: " + category.ButtonId + "");
                    //if (!UoTGumps.WaitForGump(craftingGumpId)) craftingGumpId = UoTGumps.OpenCraftGump(profession);
                    Gumps.SendAction(craftingGumpId, category.ButtonId);
                    //Can we detect gump display update?
                    //UoTGumps.WaitForGump(craftingGumpId);
                    
                    var rightColumnButtons = Regex.Matches(layout, @"button 220 (\d+) 4005 4007 1 0 (\d+)")
                        .Cast<Match>()
                        .Select(m => new
                        {
                            Y = int.Parse(m.Groups[1].Value), // Button's Y-coordinate
                            ButtonId = int.Parse(m.Groups[2].Value) // Button ID
                        })
                        .Where(m => m.Y <= 300 )
                        .ToList();

                    var recipeIndex = recipeStartIndex;
                    foreach (var recipe in rightColumnButtons)
                    {
                        var itemText = recipeIndex < text.Count ? text[recipeIndex++] : "Unknown";
                        if (!Gumps.HasGump(craftingGumpId)) craftingGumpId = UoTGumps.OpenCraftGump(profession);
                        //Assuming I don't need to flip pages
                        if (itemText == "NEXT PAGE" || itemText == "PREV PAGE") continue;
                        UoTGumps.AnswerGump(craftingGumpId, category.ButtonId, recipe.ButtonId+1);
                        Gumps.GumpData recipeGumpData = null;
                        uint recipeGumpId = 0;
                        for (int i = 0; i < 100; i++)
                        {
                            if (!Gumps.LastGumpTextExist("Success Chance:")) Misc.Pause(10);
                            else
                            {
                                recipeGumpId = Gumps.CurrentGump();
                                recipeGumpData = Gumps.GetGumpData(recipeGumpId);
                                if (recipeGumpData != null) break;
                            }
                        }
                        if (Gumps.HasGump(recipeGumpId)) Gumps.SendAction(recipeGumpId, 0);
                        var recipeRead = ReadRecipe(recipeGumpData, categoryText, category.ButtonId);
                        Misc.Pause(40);
                        if (Gumps.HasGump(recipeGumpId)) Gumps.SendAction(recipeGumpId, 0);
                        //Gumps.ResetGump();
                        //UoTGumps.WaitForGump(craftingGumpId);
                        if (recipeRead == null || recipeRead.Name != itemText)
                        {
                            if (recipeRead != null)
                                UoTLogger.LogErrorToFile("Recipe mismatch: " +  recipeRead.Name);
                            UoTLogger.LogErrorToFile("Recipe not found: " + itemText);
                        }
                        AddRecipe(recipeRead);
                        if (Gumps.HasGump(recipeGumpId)) Gumps.ResetGump();//Gumps.SendAction(recipeGumpId, 0);
                        //if (!UoTGumps.WaitForGump(craftingGumpId)) craftingGumpId = UoTGumps.OpenCraftGump(profession);
                        
                    }
                }
            
            }
        }
        
        public void AddRecipe(CraftRecipe recipe)
        {
            if (recipe == null) return;
            // Find or create a RecipeBook
            if (RecipeBook == null)
            {
                UoTLogger.LogErrorToFile("RecipeBook is null when adding recipe: " + recipe.Name);

                RecipeBook = new List<CraftProfession>();
            }
            
            // Find or create profession
            var craftProfession = RecipeBook.FirstOrDefault(p => p.Name == recipe.Profession);
            if (craftProfession == null)
            {
                UoTLogger.LogErrorToFile($"Profession {recipe.Profession} not found for recipe: {recipe.Name}");

                craftProfession = new CraftProfession(recipe.Profession, new List<CraftRecipe>());
                RecipeBook.Add(craftProfession);
            }
            
            // Remove existing recipe if present
            var existingRecipe = craftProfession.Recipes.FirstOrDefault(r => r.Name == recipe.Name &&
                r.Category == recipe.Category && r.CategoryId == recipe.CategoryId);
            if (existingRecipe != null)
            {
                craftProfession.Recipes.Remove(existingRecipe);
            }
            
            // Add recipe to category
            craftProfession.Recipes.Add(recipe);
            
            // Save changes
            SaveProfessions();
        }
        
        
        private CraftRecipe ReadRecipe(Gumps.GumpData gumpData, string category = "category", int categoryId = 0)
        {
            if (gumpData == null || gumpData.gumpData == null || gumpData.gumpText == null || gumpData.gumpData.Count < 4 ||
            // Assume recipe if the MATERIALS header exists, could use "ITEM"
            !gumpData.gumpText.Contains("<CENTER>MATERIALS</CENTER>"))
            {
                return null;
            }

            var data = gumpData.gumpData;
            var text = gumpData.gumpText;
            
            // Get menu name
            var menu = text
                .Where(t => t.EndsWith("MENU</CENTER>"))
                .Select(t => t
                    .Replace("<CENTER>", "")
                    .Replace("MENU</CENTER>", "")
                    .ToLower()
                    .Split(' ')
                    .Select(word => word.Length > 0 
                        ? char.ToUpper(word[0]) + word.Substring(1) 
                        : word)
                    .Aggregate((a, b) => a + " " + b))
                .FirstOrDefault() ?? string.Empty;
            
            // Clean line from text
            if (text[0] == "ITEM") text.RemoveAt(0);
            var typeCraftItem = text[0] == ("ITEM");
            
            // Should I save?
            var textMakersMark = text.FindIndex(line => line.IndexOf("This item may hold its maker", StringComparison.OrdinalIgnoreCase) >= 0);
            if (textMakersMark != -1) text.RemoveAt(textMakersMark);
            
            // Already saved from data
            var textMaterialColor =  text.FindIndex(line => line.IndexOf("The item retains the color of this material", StringComparison.OrdinalIgnoreCase) >= 0);
            if (textMaterialColor != -1) text.RemoveAt(textMaterialColor);
            
            var textKnownRecipe = text.IndexOf("You have not learned this recipe.");
            if (textKnownRecipe != -1) text.RemoveAt(textKnownRecipe);
            
            // This means material color matters
            var colorMaterial = data.IndexOf("*");
            if (colorMaterial != -1) data.RemoveAt(colorMaterial);

            // Look for item name
            var name = string.Empty;
            if (!double.TryParse(data[0], out _))
            {
                name = data[0].Trim();
                data.RemoveAt(0);
            }
            // Find index of "BACK" to find Name index
            int backIndex = text.IndexOf("BACK");
            if (backIndex == -1) return null;
            
            if (string.IsNullOrEmpty(name)) name = backIndex + 1 < text.Count 
                ? text[backIndex + 1] 
                : string.Empty;

            int skillRequired = 0;
            int chanceOfSuccess = 0;
            int exceptionalChance = 0;

            // Parse skill requirement
            if (double.TryParse(data[1], out double skill))
            {
                skillRequired = (int)skill;
            }

            // Parse success chance
            if (data[2] != null)
            {
                string chanceStr = data[2].TrimEnd('%');
                if (double.TryParse(chanceStr, out double chance))
                {
                    chanceOfSuccess = (int)chance;
                }
            }
            bool hasExceptionalChance = data[3].Contains('%');
            
            
            // Parse exceptional chance if it exists
            if (hasExceptionalChance && data.Count > 3 && data[3] != null)
            {
                string exceptionalStr = data[3].TrimEnd('%');
                if (double.TryParse(exceptionalStr, out double exceptional))
                {
                    exceptionalChance = (int)exceptional;
                }
            }
            
            // Calculate materials
            int materialCount;
            if (hasExceptionalChance)
                materialCount = data.Count - 4;
            else
                materialCount = data.Count - 3;
            
            var ingredients = new List<(string Material, int Quantity)>();

            List<int> materialAmounts;
            

            /*
            UoTLogger.LogErrorToFile("data:");
            
            foreach (var line in data)
            {
                UoTLogger.LogErrorToFile(line);
            }
            UoTLogger.LogErrorToFile("Exceptional: " + hasExceptionalChance + " Color: " + colorMaterial + " TextCount: " + text.Count +" Material Count: " + materialCount);
            */
            
            if (hasExceptionalChance)
            {
                materialAmounts = data
                    .Skip(4)
                    .Take(materialCount)
                    .Select(int.Parse)
                    .ToList();
            }
            else
            {
                materialAmounts = data
                    .Skip(3)
                    .Take(materialCount)
                    .Select(int.Parse)
                    .ToList();
            }

            // Get material names from end of gumpText
            var materialNames = text
                .Skip(text.Count - materialCount)
                .Take(materialCount)
                .ToList();
            
            // First replace items ending with "Chance:" with "Material List Corrupted"
            for (int i = 0; i < materialNames.Count; i++)
            {
                if (materialNames[i].EndsWith("Chance:"))
                {
                    materialNames[i] = "Material List Corrupted";
                }
            }

            // Move "Material List Corrupted" entries to the end of list
            var corruptedCount = materialNames.RemoveAll(x => x == "Material List Corrupted");
            materialNames.AddRange(Enumerable.Repeat("Material List Corrupted", corruptedCount));

            // Combine Material names and amounts into tuples
            for (int i = 0; i < materialCount; i++)
            {
                ingredients.Add((materialNames[i], materialAmounts[i]));
            }
            
            return new CraftRecipe(menu, category, categoryId, name, skillRequired, chanceOfSuccess, exceptionalChance, textKnownRecipe == -1, colorMaterial != -1, ingredients);
        }
        

        
        public Dictionary<string, int> CombineMaterialsAndAmounts(List<string> materials, List<int> amounts)
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

        public List<string> ParseMaterials(List<string> text)
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


        public List<int> ParseAmounts(string layout)
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
        
    }

}