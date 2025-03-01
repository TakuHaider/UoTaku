using System.Collections.Generic;
using System.Linq;

namespace RazorEnhanced
{
    public class UoTBod
    {
        public string Skill { get; set; }
        public int BodSerial { get; set; }
        public Item BodItem { get; set; }
        public uint BodGump { get; set; }
        public UoTBods.Recipe Recipe { get; set; }
        public string ResourceType { get; set; }
        public string Resource { get; set; }
        public bool Exceptional { get; set; } = false;
        public bool SmallOrder { get; set; } = false;
        public List<(string, int)> IngredientsNeeded { get; set; } 
        public List<((UoTBods.Profession, UoTBods.Category, UoTBods.Recipe), int)> FailedList  { get; set; } = new ();

        public List<((UoTBods.Profession, UoTBods.Category, UoTBods.Recipe), int)> IngredientsToMake { get; set; } = new ();
        public Dictionary<string, int> CurrentAmountList { get; set; } = new ();
        public int TotalAmount { get; set; }
        public int FilledAmount { get; set; }
        public bool Filled { get; set; } = false;

        public UoTBod(string skill, Item bodItem)
        {
            Skill = skill;
            BodSerial = bodItem.Serial;
            BodItem = bodItem;
            ResourceType = UoTBods.BodSkillInfo[skill].ResourceType;
            Resource = ResourceType;
            var allMaterials = UoTBods.ResourceInfo.FirstOrDefault(m => m.Value.FirstOrDefault().Key == ResourceType).Value.Keys.ToList();
            var allRecipes = UoTBods.Professions
                .SelectMany(profession => 
                    profession.Categories.SelectMany(category => 
                        category.Recipes.Select(recipe => 
                            (Profession: profession, 
                                Category: category, 
                                Recipe: recipe))))
                .ToList();
            var professionInfo = UoTBods.Professions.First(p => p.Name == Skill);
            var categories = professionInfo.Categories.ToList();
            var recipesWithCategories = professionInfo.Categories
                .SelectMany(category => category.Recipes.Select(recipe => (Category: category, Recipe: recipe)))
                .ToList();
            foreach (var prop in bodItem.Properties)
            { 
                if (prop.ToString().Contains("small bulk order")) SmallOrder = true;
                if (prop.ToString().Contains("all items must be exceptional")) Exceptional = true;
                if (prop.ToString().Contains("Amount To Make: ")) TotalAmount = int.Parse(prop.ToString().Replace("Amount To Make: ", "").Trim());
                foreach (var recipeWithCategory in recipesWithCategories)
                {
                    Recipe = recipeWithCategory.Recipe;
                    if (!prop.ToString().StartsWith(Recipe.Name)) continue;
                    FilledAmount = int.Parse(prop.ToString().Replace(Recipe.Name+": ", "").Trim());
                    IngredientsNeeded = Recipe.Ingredients.ToList();
                    foreach (var ingredientNeeded in IngredientsNeeded)
                    {
                        foreach (var (profession, category, recipe) in allRecipes)
                        {
                            var message = string.Format(@"               Recipe requirement:
                                 - Material: {0}
                                 - Amount needed: {1}
                               Can be crafted in:
                                 - Profession: {2}
                                 - Category: {3}
                                 - Recipe: {4}",
                                ingredientNeeded.Item1,
                                ingredientNeeded.Item2,
                                profession,
                                category,
                                recipe);
                            Misc.SendMessage(message);
                            IngredientsToMake.Add(((profession, category, recipe), ingredientNeeded.Item2));
                        }
                    }
                }
                foreach (var material in allMaterials)
                {
                    if (!prop.ToString().Contains("All items must be made with") || !prop.ToString().Contains(material)) continue;
                    Resource = material;
                    break;
                }
               
            }
            //Refresh(true);
        }

        public void CraftIngredientsToMake()
        {
            var craftedList = new List<((UoTBods.Profession, UoTBods.Category, UoTBods.Recipe), int)>();
            foreach (var item in IngredientsToMake)
            {
                if (FailedList.Contains(item)) continue;
                var profession = item.Item1.Item1;
                var category = item.Item1.Item2;
                var recipe = item.Item1.Item3;
                // add check for current amount?
                var amountTotal = item.Item2;
                if (UoTItems.GetItem(recipe.Name, item.Item2) < 1)
                {
                    //already got them
                    continue;
                }
                //check tools, create tools
                var tool = UoTItems.GetTool(profession.Name) ?? UoTItems.CraftTool(profession.Name, 2).FirstOrDefault();
                if (tool == null) continue;
                //need to check hue ?
                var craft = true;
                UoTBods.ChooseGumpMaterial(profession.Name, Resource);
                var professionGump = UoTGumps.OpenCraftGump(profession.Name);
                while (craft && UoTItems.CountItemsInBag(recipe.Name) < amountTotal)
                {
                    Misc.Pause(10);
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        // Are resources the only hues that matter?
                        var material = ingredient.Material;
                        if (ingredient.Material == ResourceType) material = Resource;
                        var ingredientsNeeded = UoTItems.GetItem(material,ingredient.Quantity); //need to check hue
                        if (ingredientsNeeded > 0)
                        {
                            Misc.SendMessage("Need " + ingredientsNeeded + " " + material + " to craft " + recipe.Name);
                            //lookup if craftable
                            craft = false;
                        }
                    }
                    //UoTGumps.OpenCraftGump(profession.Name);
                    UoTGumps.AnswerGump(professionGump, category.ButtonId, recipe.ButtonId);
                }

                //return mats on fail?
                if (!craft) FailedList.Add(item);
                else craftedList.Add(item);
            }
            IngredientsToMake = IngredientsToMake.Except(craftedList).ToList();
        }

        public bool GetIngredients()
        {
            return UoTItems.GetItems(IngredientsNeeded);
        }

        public bool GetItemsAndFill()
        {
            if (Recipe == null)
            {
                Misc.SendMessage("Lost bod item.");
                return false;
            }

            var neededAmount = UoTItems.GetItem(Recipe.Name, TotalAmount - FilledAmount);
            //Items.ContainerCount(Player.Backpack, Recipe.Name) need itemId from name
            if (neededAmount < 1)
            {
                Gumps.ResetGump();
                Items.UseItem(BodItem);
                BodGump = UoTGumps.SaveGumpIdByText("A bulk order");
                Gumps.SendAction(BodGump, 4);
                Target.WaitForTarget(2000, false);
                Items.UseItem(Player.Backpack);
                Filled = true;
                return true;
            }
            Misc.SendMessage("Not enough " + Recipe.Name + " need " + neededAmount + "more.");
            return false;
        }
        
        
    /*
    def get_resources_data(self):
        common_names = {"Cloth": {'id': ["Cut Cloth", "Cloth", "Bolt Of Cloth"], 'color': 0x0000},
                        "Yards of Cloth": {'id': ["Cut Cloth", "Cloth", "Bolt Of Cloth"], 'color': 0x0000},
                        "Blank Maps or Scrolls": {'id': ["Blank Scroll", "Blank Map"], 'color': -1},
                        "Sea Serpent or Dragon Scales": {'id': ["Sea Serpent Scales", "Dragon Scales"], 'color': -1},
                        "Water": {'id': ["Endless Decanter of Water", "Glass Pitcher", "A Pitcher Of Water"],
                                  'color': -1}}

        _, item_data = search_button(skill_info[self.skill]['gump'], self.item['name'])
        resources_data = []

        for resource in item_data['mats']:
            resource_data = {}
            for name_list in material_info:
                if resource[0] == name_list:
                    resource_data = material_info[name_list][self.material]
                    resource_data['name'] = (self.material.replace("Leather/Hides", "").replace(" Hides", "")
                                             + " " + resource[0])
                    break
            if not resource_data:
                for name in common_names:
                    if resource[0] == name:
                        resource_data = common_names[name]
                        resource_data['name'] = name
                        break
            if not resource_data:
                resource_data = {'id': [resource[0]], 'color': -1}
            resource_data['name'] = resource[0]
            resource_data['amount'] = resource[1]
            resource_data['stack'] = item_data['stack']
            resources_data.append(resource_data)
        return resources_data

    def get_resources(self, resources_data):
        grabbing_results = []
        for resource_data in resources_data:
            grabbing_results.append(get_resource(resource_data['id'], resource_data['color'], resource_data['amount'],
                                                 self.to_make_quantity(), resource_data['stack']))
        return False not in grabbing_results
        */
    


    }
    
    public static partial class UoTBods
    {
        public static uint CraftingGumpId;
        public static void MakeBod()
        {
            foreach (var skill in BodSkillInfo.Keys)
            {
                CraftingGumpId = UoTGumps.OpenCraftGump(skill);
                if (CraftingGumpId == 0) continue;
                /*var fileName = UoTJson.FileName;
                if (string.IsNullOrEmpty(fileName)) fileName = UoTSystem.GetCallingClassName();
                using (var storedData = new UoTJson(fileName))
                {
                    crafting_gump_id = storedData.GetData<uint>("skillGump:" + skill, typeof(uint));
                }*/
                var sbods = new List<UoTBod>();
                var lbods = new List<UoTBod>();
                string material = string.Empty;
                string problem = string.Empty;

                while (!Player.IsGhost)
                {
                    /*crafting_gump_id = UoTGumps.OpenCraftGump(skill);
                    if (crafting_gump_id == 0) continue;*/
                    
                    var skillDetails = BodSkillInfo[skill];
                    var skillMaterials = skillDetails.Resources;
                    var bodItems = Items.FindAllByID(new List<int> {0x2258} , skillDetails.BodColor, Player.Backpack.Serial, 1);
                    if (bodItems == null || !bodItems.Any()) break;
                    foreach (var bodItem in bodItems)
                    {
                        if (bodItem.Properties.Count == 0 ||
                            !bodItem.Properties[0].ToString().Contains("a bulk order deed")) //iterate through properties?
                        {
                            //add to ignore list
                            continue;
                        }
                        
                        //ReturnItems(bod); return old marked materials, should move to end
                        
                        
                        var bod = new UoTBod(skill,bodItem);
                        ResetGumpMaterial(bod);
                        if (!bod.SmallOrder)  lbods.Add(bod); //skip large?
                        if (!ChooseGumpMaterial(skill, bod.ResourceType)) break;
                        if (bod.GetItemsAndFill()) break;
                        while (!Player.IsGhost && !bod.Filled)
                        {
                            //Recycle(skill, bod.Item["name"], bod.BaseMaterial);
                            //Check overweight
                            //Check tools
                            //resources?
                            bod.CraftIngredientsToMake();
                            //check if make list .exclue failed has any
                        }

                        //bod.Refresh();
                        if (bod.Filled)
                        {
                            
                            sbods.Add(bod);
                            //Recycle(skill, bod.Item["name"], bod.BaseMaterial);
                            break;
                        }
                            
                    }
                }
                //CombineBods(lbod, sbods);
            }
            //ReturnItems();
            //Gumps.CloseGump(crafting_gump_id);
            //Gumps.CloseGump(GetGumpNum(new List<string> { "A bulk order", "A large bulk order" }, 500));
        }
        
        public static bool ChooseGumpMaterial(string skill, string materialName)
        {
            if (string.IsNullOrEmpty(skill) || string.IsNullOrEmpty(materialName)) return false;
            var materialData = BodSkillInfo[skill].Resources[materialName];
            var gumpId = UoTGumps.OpenCraftGump(skill);
            if (gumpId == 0) return false;
            if (!Gumps.LastGumpGetLineList().Contains($"|{materialName.ToUpper()}"))
            {
                UoTGumps.AnswerGump(gumpId, 7, materialData.Btn);
                UoTGumps.WaitForGump(gumpId, 4000);
            }
            UoTGumps.CloseGump(gumpId);
            return true;

        }
        
        
        
        public static List<(UoTBods.Profession, UoTBods.Category, UoTBods.Recipe)> GetRecipeInfo(List<string> recipeNames)
        {
            return UoTBods.Professions
                .SelectMany(profession => 
                    profession.Categories.SelectMany(category => 
                        category.Recipes.Where(recipe => recipeNames.Contains(recipe.Name))
                            .Select(recipe => (profession, category, recipe))))
                .ToList();
        }
        
        public static (UoTBods.Profession, UoTBods.Category, UoTBods.Recipe) GetRecipeInfo(string recipeName)
        {
            return UoTBods.Professions
                .SelectMany(profession => 
                    profession.Categories.SelectMany(category => 
                        category.Recipes.Where(recipe => recipe.Name == recipeName)
                            .Select(recipe => (profession, category, recipe))))
                .FirstOrDefault();
        }

        public static void ResetGumpMaterial(UoTBod bod)
        {
            var gumpId = UoTGumps.OpenCraftGump(bod.Skill);
            if (gumpId == 0) return;
            if (Gumps.LastGumpGetLineList().Contains($"|{bod.ResourceType.ToUpper()}")) return;
            UoTGumps.AnswerGump(CraftingGumpId, 7, ResourceInfo[bod.ResourceType].FirstOrDefault().Value.Btn);
            UoTGumps.WaitForGump(gumpId, 4000);
            // Button is always 6
        }
        
    }
    
}