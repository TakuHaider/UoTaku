using System;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    internal static partial class IsExternalInit {}
}

namespace RazorEnhanced
{
	public static partial class UoTBods
	{
        public record Profession(string Name, IReadOnlyCollection<Category> Categories);
        public record Category(string Name, int ButtonId, IReadOnlyCollection<Recipe> Recipes);
        public record Recipe(string Name, int ButtonId, bool IsQuestItem, IReadOnlyList<(string Material, int Quantity)> Ingredients);
        
        
        
        public static readonly IReadOnlyCollection<Profession> Professions = new[]
        {
            new Profession("Alchemy", new[]
            {
                new Category("Exit", 0, new Recipe[0]), // No recipes, empty category
                new Category("Cancel Make", 227, new Recipe[0]), // No recipes, placeholder
                new Category("Non Quest Item", 207, new Recipe[0]), // Placeholder
                new Category("Make Last", 47, new Recipe[0]), // Placeholder, no recipes specified
                new Category("Last Ten", 67, new Recipe[0]), // Placeholder

                new Category("Healing And Curative", 1, new[]
                {
                    new Recipe(
                        "Refresh",
                        2,
                        false,
                        new[] 
                        { 
                            ("Black Pearl", 1), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Greater Refreshment",
                        22,
                        false,
                        new[] 
                        { 
                            ("Black Pearl", 5), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Lesser Heal",
                        42,
                        false,
                        new[] 
                        { 
                            ("Ginseng", 1), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Heal",
                        62,
                        false,
                        new[] 
                        { 
                            ("Ginseng", 3), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Greater Heal",
                        82,
                        false,
                        new[] 
                        { 
                            ("Ginseng", 7), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Lesser Cure",
                        102,
                        false,
                        new[] 
                        { 
                            ("Garlic", 1), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Cure",
                        122,
                        false,
                        new[] 
                        { 
                            ("Garlic", 3), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Greater Cure",
                        142,
                        false,
                        new[] 
                        { 
                            ("Garlic", 6), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Elixir Of Rebirth",
                        162,
                        false,
                        new[] 
                        { 
                            ("Medusa Blood", 1), 
                            ("Spiders' Silk", 3), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Barrab Hemolymph Concentrate",
                        182,
                        false,
                        new[] 
                        { 
                            ("Empty Bottles", 1), 
                            ("Ginseng", 20), 
                            ("Plant Clippings", 5), 
                            ("Myrmidex Eggsac", 5) 
                        }
                    )
                }),
                new Category("Enhancement", 21, new[]
                {
                    new Recipe(
                        "Agility",
                        2,
                        false,
                        new[] 
                        { 
                            ("Blood Moss", 1), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    new Recipe(
                        "Greater Agility",
                        22,
                        false,
                        new[] 
                        { 
                            ("Blood Moss", 3), 
                            ("Empty Bottles", 1) 
                        }
                    ),
                    
                    new Recipe(
                        "Night Sight",
                        42,
                        false,
                        new[] { ("Spiders' Silk", 1), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Strength",
                        62,
                        false,
                        new[] { ("Mandrake Root", 2), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Greater Strength",
                        82,
                        false,
                        new[] { ("Mandrake Root", 5), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Invisibility",
                        102,
                        false,
                        new[] { ("Empty Bottles", 1), ("Blood Moss", 4), ("Nightshade", 3) }
                    ),
                    new Recipe(
                        "Jukari Burn Poultice",
                        122,
                        false,
                        new[] { ("Empty Bottles", 1), ("Black Pearl", 20), ("vanilla", 10), ("Lava Berry", 5) }
                    ),
                    new Recipe(
                        "Kurak Ambusher'S Essence",
                        142,
                        false,
                        new[] { ("Empty Bottles", 1), ("Blood Moss", 20), ("Blue Diamond", 1), ("Lava Berry", 10) }
                    ),
                    new Recipe(
                        "Barako Draft Of Might",
                        162,
                        false,
                        new[] { ("Empty Bottles", 1), ("Spiders' Silk", 20), ("bottle of liquor", 10), ("Perfect Bananas", 5) }
                    ),
                    new Recipe(
                        "Urali Trance Tonic",
                        182,
                        false,
                        new[] { ("Empty Bottles", 1), ("Mandrake Root", 20), ("Sea Serpent or Dragon Scales", 10), ("River Moss", 5) }
                    ),
                    new Recipe(
                        "Sakkhra Prophylaxis Potion",
                        202,
                        false,
                        new[] { ("Empty Bottles", 1), ("Nightshade", 20), ("bottle of wine", 10), ("Blue Corn", 5) }
                    )
                }),

                new Category("Toxic", 41, new[]
                {
                    new Recipe(
                        "Lesser Poison",
                        2,
                        false,
                        new[] { ("Nightshade", 1), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Poison",
                        22,
                        false,
                        new[] { ("Nightshade", 2), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Greater Poison",
                        42,
                        false,
                        new[] { ("Nightshade", 4), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Deadly Poison",
                        62,
                        false,
                        new[] { ("Nightshade", 8), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Parasitic",
                        82,
                        false,
                        new[] { ("Empty Bottles", 1), ("parasitic plant", 5) }
                    ),
                    new Recipe(
                        "Darkglow",
                        102,
                        false,
                        new[] { ("Empty Bottles", 1), ("luminescent fungi", 5) }
                    ),
                    new Recipe(
                        "Scouring Toxin",
                        122,
                        false,
                        new[] { ("toxic venom sac", 1), ("Empty Bottles", 1) }
                    )
                        }),
                
                new Category("Explosive", 61, new[]
                {
                    new Recipe(
                        "Lesser Explosion",
                        2,
                        false,
                        new[] { ("Sulfurous Ash", 3), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Explosion",
                        22,
                        false,
                        new[] { ("Sulfurous Ash", 5), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Greater Explosion",
                        42,
                        false,
                        new[] { ("Sulfurous Ash", 10), ("Empty Bottles", 1) }
                    ),
                    new Recipe(
                        "Conflagration",
                        62,
                        false,
                        new[] { ("Empty Bottles", 1), ("Grave Dust", 5) }
                    ),
                    new Recipe(
                        "Greater Conflagration",
                        82,
                        false,
                        new[] { ("Empty Bottles", 1), ("Grave Dust", 10) }
                    ),
                    new Recipe(
                        "Confusion Blast",
                        102,
                        false,
                        new[] { ("Empty Bottles", 1), ("Pig Iron", 5) }
                    ),
                    new Recipe(
                        "Greater Confusion Blast",
                        122,
                        false,
                        new[] { ("Empty Bottles", 1), ("Pig Iron", 10) }
                    ),
                    new Recipe(
                        "Black Powder",
                        142,
                        true,
                        new[] { ("Sulfurous Ash", 1), ("saltpeter", 6), ("charcoal", 1) }
                    ),
                    new Recipe(
                        "Fuse Cord",
                        162,
                        false,
                        new[] { ("ball of yarn", 1), ("black powder", 1), ("potash", 1) }
                    )
                }),

                new Category("Strange Brew", 81, new[]
                {
                    new Recipe(
                        "Smoke Bomb",
                        2,
                        false,
                        new[] { ("Eggs", 1), ("Ginseng", 3) }
                    ),
                    new Recipe(
                        "Hovering Wisp",
                        22,
                        false,
                        new[] { ("Captured Essence", 4) }
                    ),
                    new Recipe(
                        "Natural Dye",
                        42,
                        false,
                        new[] { ("plant pigment", 1), ("color fixative", 1) }
                    ),
                    new Recipe(
                        "Nexus Core",
                        62,
                        false,
                        new[] { ("Mandrake", 10), ("Spider's Silk", 10), ("Dark Sapphire", 5), ("crushed glass", 5) }
                    )
                }),

                new Category("Ingredients", 101, new[]
                {
                    new Recipe(
                        "Plant Pigment",
                        2,
                        false,
                        new[] { ("Plant Clippings", 1), ("empty bottle", 1) }
                    ),
                    new Recipe(
                        "Color Fixative",
                        22,
                        false,
                        new[] { ("silver serpent venom", 1), ("bottle of wine", 1) }
                    ),
                    new Recipe(
                        "Crystal Granules",
                        42,
                        false,
                        new[] { ("Shimmering Crystals", 1) }
                    ),
                    new Recipe(
                        "Crystal Dust",
                        62,
                        false,
                        new[] { ("broken crystals", 4) }
                    ),
                    new Recipe(
                        "Softened Reeds",
                        82,
                        false,
                        new[] { ("dry reeds", 1), ("scouring toxin", 2) }
                    ),
                    new Recipe(
                        "Vial Of Vitriol",
                        102,
                        false,
                        new[] { ("Parasitic Poison", 1), ("Nightshade", 30) }
                    ),
                    new Recipe(
                        "Bottle Of Ichor",
                        122,
                        false,
                        new[] { ("Darkglow Poison", 1), ("Spiders' Silk", 1) }
                    ),
                    new Recipe(
                        "Potash",
                        142,
                        true,
                        new[] { ("Boards or Logs", 1) }
                    ),
                    new Recipe(
                        "Gold Dust",
                        162,
                        false,
                        new[] { ("Gold", 1000) }
                    )
                })
            }),
            
            new Profession("Blacksmithing", new[]
            {
                
                new Category("Exit", 0, new Recipe[0]), // No recipes, empty category
                new Category("Cancel Make", 227, new Recipe[0]), // No recipes, placeholder
                new Category("Repair Item", 107, new Recipe[0]), // Empty
                new Category("Mark Item", 127, new Recipe[0]), // Empty
                new Category("Enhance Item", 167, new Recipe[0]), // Placeholder
                new Category("Non Quest Item", 207, new Recipe[0]), // Placeholder
                new Category("Make Last", 47, new Recipe[0]), // Placeholder
                new Category("Smelt Item", 27, new Recipe[0]), // Empty
                new Category("Last Ten", 67, new Recipe[0]), // Placeholder
                new Category("Miscellaneous", 181, new Recipe[0]), // Placeholder, refer to previous data

                
                new Category("Metal Armor", 1, new[]
                {
                    new Recipe(
                        "Ringmail Gloves",
                        2,
                        false,
                        new[] { ("Ingots", 10) }
                    ),
                    new Recipe(
                        "Ringmail Leggings",
                        22,
                        false,
                        new[] { ("Ingots", 16) }
                    ),
                    new Recipe(
                        "Ringmail Sleeves",
                        42,
                        false,
                        new[] { ("Ingots", 14) }
                    ),
                    new Recipe(
                        "Ringmail Tunic",
                        62,
                        false,
                        new[] { ("Ingots", 18) }
                    ),
                    new Recipe(
                        "Chainmail Coif",
                        82,
                        false,
                        new[] { ("Ingots", 10) }
                    ),
                    new Recipe(
                        "Chainmail Leggings",
                        102,
                        false,
                        new[] { ("Ingots", 18) }
                    ),
                    new Recipe(
                        "Chainmail Tunic",
                        122,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Platemail Arms",
                        142,
                        false,
                        new[] { ("Ingots", 18) }
                    ),
                    new Recipe(
                        "Platemail Gloves",
                        162,
                        false,
                        new[] { ("Ingots", 12) }
                    ),
                    new Recipe(
                        "Platemail Gorget",
                        182,
                        false,
                        new[] { ("Ingots", 10) }
                    ),
                    new Recipe(
                        "Platemail Legs",
                        202,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Platemail (Tunic)",
                        222,
                        false,
                        new[] { ("Ingots", 25) }
                    ),
                    new Recipe(
                        "Platemail (Female) Plate",
                        242,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Platemail Mempo",
                        282,
                        false,
                        new[] { ("Ingots", 18) }
                    ),
                    new Recipe(
                        "Platemail Do",
                        302,
                        false,
                        new[] { ("Ingots", 28) }
                    ),
                    new Recipe(
                        "Platemail Hiro Sode",
                        322,
                        false,
                        new[] { ("Ingots", 16) }
                    ),
                    new Recipe(
                        "Platemail Suneate",
                        342,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Platemail Haidate",
                        362,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Gargish Platemail Arms",
                        382,
                        false,
                        new[] { ("Ingots", 18) }
                    ),
                    new Recipe(
                        "Gargish Platemail Chest",
                        402,
                        false,
                        new[] { ("Ingots", 25) }
                    ),
                    new Recipe(
                        "Gargish Platemail Leggings",
                        422,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Gargish Platemail Kilt",
                        442,
                        false,
                        new[] { ("Ingots", 12) }
                    ),
                    new Recipe(
                        "Gargish Platemail Arms 2",
                        462,
                        false,
                        new[] { ("Ingots", 18) }
                    ),
                    new Recipe(
                        "Gargish Platemail Chest 2",
                        482,
                        false,
                        new[] { ("Ingots", 25) }
                    ),
                    new Recipe(
                        "Gargish Platemail Leggings 2",
                        502,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Gargish Platemail Kilt 2",
                        522,
                        false,
                        new[] { ("Ingots", 12) }
                    ),
                    new Recipe(
                        "Gargish Amulet",
                        542,
                        false,
                        new[] { ("Ingots", 3) }
                    ),
                    new Recipe(
                        "Britches Of Warding",
                        562,
                        false,
                        new[] { ("Ingots", 18), ("Leggings of Bane", 1), ("Turquoise", 4), ("Blood of the Dark Father", 5) }
                    )
                }),
                // Add these categories to the Blacksmithing profession array:
                new Category("Helmets", 21, new[]
                {
                    new Recipe(
                        "Bascinet",
                        2,
                        false,
                        new[] { ("Ingots", 15) }
                    ),
                    new Recipe(
                        "Close Helmet",
                        22,
                        false,
                        new[] { ("Ingots", 15) }
                    ),
                    new Recipe(
                        "Helmet",
                        42,
                        false,
                        new[] { ("Ingots", 15) }
                    ),
                    new Recipe(
                        "Norse Helm",
                        62,
                        false,
                        new[] { ("Ingots", 15) }
                    ),
                    new Recipe(
                        "Plate Helm",
                        82,
                        false,
                        new[] { ("Ingots", 15) }
                    ),
                    new Recipe(
                        "Chainmail Hatsuburi",
                        102,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Platemail Hatsuburi",
                        122,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Heavy Platemail Jingasa",
                        142,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Light Platemail Jingasa",
                        162,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Small Platemail Jingasa",
                        182,
                        false,
                        new[] { ("Ingots", 20) }
                    ),
                    new Recipe(
                        "Decorative Platemail Kabuto",
                        202,
                        false,
                        new[] { ("Ingots", 25) }
                    ),
                    new Recipe(
                        "Platemail Battle Kabuto",
                        222,
                        false,
                        new[] { ("Ingots", 25) }
                    ),
                    new Recipe(
                        "Standard Platemail Kabuto",
                        242,
                        false,
                        new[] { ("Ingots", 25) }
                    ),
                    new Recipe(
                        "Circlet",
                        262,
                        false,
                        new[] { ("Ingots", 6) }
                    ),
                    new Recipe(
                        "Royal Circlet",
                        282,
                        false,
                        new[] { ("Ingots", 6) }
                    ),
                    new Recipe(
                        "Gemmed Circlet",
                        302,
                        false,
                        new[] { ("Ingots", 6), ("Tourmalines", 1), ("Amethysts", 1), ("Blue Diamond", 1) }
                    )
                }),

                new Category("Shields", 41, new[]
                {
                    new Recipe(
                        "Buckler",  // Removed trailing space
                        2,
                        false,
                        new[] { ("Ingots", 10) }
                    ),
                    new Recipe(
                        "Bronze Shield",
                        22,
                        false,
                        new[] { ("Ingots", 12) }
                    ),
                    new Recipe(
                        "Heater Shield",
                        42,
                        false,
                        new[] { ("Ingots", 18) }
                    ),
                    new Recipe(
                        "Metal Shield",
                        62,
                        false,
                        new[] { ("Ingots", 14) }
                    ),
                    new Recipe(
                        "Metal Kite Shield",
                        82,
                        false,
                        new[] { ("Ingots", 16) }
                    ),
                    new Recipe(
                        "Tear Kite Shield",
                        102,
                        false,
                        new[] { ("Ingots", 8) }
                    ),
                    new Recipe(
                        "Chaos Shield",
                        122,
                        false,
                        new[] { ("Ingots", 25) }
                    ),
                    new Recipe(
                        "Order Shield",
                        142,
                        false,
                        new[] { ("Ingots", 25) }
                    ),
                    new Recipe(
                        "Small Plate Shield",
                        162,
                        false,
                        new[] { ("Ingots", 12) }
                    ),
                    new Recipe(
                        "Gargish Kite Shield",
                        182,
                        false,
                        new[] { ("Ingots", 16) }
                    ),
                    new Recipe(
                        "Large Plate Shield",
                        202,
                        false,
                        new[] { ("Ingots", 18) }
                    ),
                    new Recipe(
                        "Medium Plate Shield",
                        222,
                        false,
                        new[] { ("Ingots", 14) }
                    ),
                    new Recipe(
                        "Gargish Chaos Shield",
                        242,
                        false,
                        new[] { ("Ingots", 25) }
                    ),
                    new Recipe(
                        "Gargish Order Shield",
                        262,
                        false,
                        new[] { ("Ingots", 25) }
                    )
                }),
                new Category("Bladed", 61, new[]
                {
                    new Recipe("Bone Harvester", 2, false, new[] { ("Ingots", 10) }),
                    new Recipe("Broadsword", 22, false, new[] { ("Ingots", 10) }),
                    new Recipe("Crescent Blade", 42, false, new[] { ("Ingots", 14) }),
                    new Recipe("Cutlass", 62, false, new[] { ("Ingots", 8) }),
                    new Recipe("Dagger", 82, false, new[] { ("Ingots", 3) }),
                    new Recipe("Katana", 102, false, new[] { ("Ingots", 8) }),
                    new Recipe("Kryss", 122, false, new[] { ("Ingots", 8) }),
                    new Recipe("Longsword", 142, false, new[] { ("Ingots", 12) }),
                    new Recipe("Scimitar", 162, false, new[] { ("Ingots", 10) }),
                    new Recipe("Viking Sword", 182, false, new[] { ("Ingots", 14) }),
                    new Recipe("No-Dachi", 222, false, new[] { ("Ingots", 18) }),
                    new Recipe("Wakizashi", 242, false, new[] { ("Ingots", 8) }),
                    new Recipe("Lajatang", 262, false, new[] { ("Ingots", 25) }),
                    new Recipe("Daisho", 282, false, new[] { ("Ingots", 15) }),
                    new Recipe("Tekagi", 302, false, new[] { ("Ingots", 12) }),
                    new Recipe("Shuriken", 322, false, new[] { ("Ingots", 5) }),
                    new Recipe("Kama", 342, false, new[] { ("Ingots", 14) }),
                    new Recipe("Sai", 362, false, new[] { ("Ingots", 12) }),
                    new Recipe("Radiant Scimitar", 382, false, new[] { ("Ingots", 15) }),
                    new Recipe("War Cleaver", 402, false, new[] { ("Ingots", 18) }),
                    new Recipe("Elven Spellblade", 422, false, new[] { ("Ingots", 14) }),
                    new Recipe("Assassin Spike", 442, false, new[] { ("Ingots", 9) }),
                    new Recipe("Leafblade", 462, false, new[] { ("Ingots", 12) }),
                    new Recipe("Rune Blade", 482, false, new[] { ("Ingots", 15) }),
                    new Recipe("Elven Machete", 502, false, new[] { ("Ingots", 14) }),
                    new Recipe("Rune Carving Knife", 522, false, new[] { ("Ingots", 9), ("Dread Horn Mane", 1), ("Putrefaction", 10), ("Muculent", 10) }),
                    new Recipe("Cold Forged Blade", 542, false, new[] { ("Ingots", 18), ("Grizzled Bones", 1), ("Grizzled Bones", 10), ("Blight", 10) }),
                    new Recipe("Overseer Sundered Blade", 562, false, new[] { ("Ingots", 15), ("Grizzled Bones", 1), ("Blight", 10), ("Scourge", 10) }),
                    new Recipe("Luminous Rune Blade", 582, false, new[] { ("Ingots", 15), ("Grizzled Bones", 1), ("Corruption", 10), ("Putrefaction", 10) }),
                    new Recipe("True Spellblade", 602, false, new[] { ("Ingots", 14), ("Blue Diamond", 1) }),
                    new Recipe("Icy Spellblade", 622, false, new[] { ("Ingots", 14), ("Turquoise", 1) }),
                    new Recipe("Fiery Spellblade", 642, false, new[] { ("Ingots", 14), ("Fire Ruby", 1) }),
                    new Recipe("Spellblade Of Defense", 662, false, new[] { ("Ingots", 18), ("White Pearl", 1) }),
                    new Recipe("True Assassin Spike", 682, false, new[] { ("Ingots", 9), ("Dark Sapphire", 1) }),
                    new Recipe("Charged Assassin Spike", 702, false, new[] { ("Ingots", 9), ("Ecru Citrine", 1) }),
                    new Recipe("Magekiller Assassin Spike", 722, false, new[] { ("Ingots", 9), ("Brilliant Amber", 1) }),
                    new Recipe("Wounding Assassin Spike", 742, false, new[] { ("Ingots", 9), ("Perfect Emerald", 1) }),
                    new Recipe("True Leafblade", 762, false, new[] { ("Ingots", 12), ("Blue Diamond", 1) }),
                    new Recipe("Luckblade", 782, false, new[] { ("Ingots", 12), ("White Pearl", 1) }),
                    new Recipe("Magekiller Leafblade", 802, false, new[] { ("Ingots", 12), ("Fire Ruby", 1) }),
                    new Recipe("Leafblade Of Ease", 822, false, new[] { ("Ingots", 12), ("Perfect Emerald", 1) }),
                    new Recipe("Knight's War Cleaver", 842, false, new[] { ("Ingots", 18), ("Perfect Emerald", 1) }),
                    new Recipe("Butcher's War Cleaver", 862, false, new[] { ("Ingots", 18), ("Turquoise", 1) }),
                    new Recipe("Serrated War Cleaver", 882, false, new[] { ("Ingots", 18), ("Ecru Citrine", 1) }),
                    new Recipe("True War Cleaver", 902, false, new[] { ("Ingots", 18), ("Brilliant Amber", 1) }),
                    new Recipe("Adventurer's Machete", 922, false, new[] { ("Ingots", 14), ("White Pearl", 1) }),
                    new Recipe("Orcish Machete", 942, false, new[] { ("Ingots", 14), ("Scourge", 1) }),
                    new Recipe("Machete Of Defense", 962, false, new[] { ("Ingots", 14), ("Brilliant Amber", 1) }),
                    new Recipe("Diseased Machete", 982, false, new[] { ("Ingots", 14), ("Blight", 1) }),
                    new Recipe("Runesabre", 1002, false, new[] { ("Ingots", 15), ("Turquoise", 1) }),
                    new Recipe("Mage's Rune Blade", 1022, false, new[] { ("Ingots", 15), ("Blue Diamond", 1) }),
                    new Recipe("Rune Blade Of Knowledge", 1042, false, new[] { ("Ingots", 15), ("Ecru Citrine", 1) }),
                    new Recipe("Corrupted Rune Blade", 1062, false, new[] { ("Ingots", 15), ("Corruption", 1) }),
                    new Recipe("True Radiant Scimitar", 1082, false, new[] { ("Ingots", 15), ("Brilliant Amber", 1) }),
                    new Recipe("Darkglow Scimitar", 1102, false, new[] { ("Ingots", 15), ("Dark Sapphire", 1) }),
                    new Recipe("Icy Scimitar", 1122, false, new[] { ("Ingots", 15), ("Dark Sapphire", 1) }),
                    new Recipe("Twinkling Scimitar", 1142, false, new[] { ("Ingots", 15), ("Dark Sapphire", 1) }),
                    new Recipe("Bone Machete", 1162, false, new[] { ("Ingots", 20), ("Bones", 6) }),
                    new Recipe("Gargish Katana", 1182, false, new[] { ("Ingots", 8) }),
                    new Recipe("Gargish Kryss", 1202, false, new[] { ("Ingots", 8) }),
                    new Recipe("Gargish Bone Harvester", 1222, false, new[] { ("Ingots", 10) }),
                    new Recipe("Gargish Tekagi", 1242, false, new[] { ("Ingots", 12) }),
                    new Recipe("Gargish Daisho", 1262, false, new[] { ("Ingots", 15) }),
                    new Recipe("Dread Sword", 1282, false, new[] { ("Ingots", 14) }),
                    new Recipe("Gargish Talwar", 1302, false, new[] { ("Ingots", 18) }),
                    new Recipe("Gargish Dagger", 1322, false, new[] { ("Ingots", 3) }),
                    new Recipe("Bloodblade", 1342, false, new[] { ("Ingots", 8) }),
                    new Recipe("Shortblade", 1362, false, new[] { ("Ingots", 12) })
                }),

                new Category("Axes", 81, new[]
                {
                    new Recipe("Axe", 2, false, new[] { ("Ingots", 14) }),
                    new Recipe("Battle Axe", 22, false, new[] { ("Ingots", 14) }),
                    new Recipe("Double Axe", 42, false, new[] { ("Ingots", 12) }),
                    new Recipe("Executioner's Axe", 62, false, new[] { ("Ingots", 14) }),
                    new Recipe("Large Battle Axe", 82, false, new[] { ("Ingots", 12) }),
                    new Recipe("Two Handed Axe", 102, false, new[] { ("Ingots", 16) }),
                    new Recipe("War Axe", 122, false, new[] { ("Ingots", 16) }),
                    new Recipe("Ornate Axe", 142, false, new[] { ("Ingots", 18) }),
                    new Recipe("Guardian Axe", 162, false, new[] { ("Ingots", 15), ("Blue Diamond", 1) }),
                    new Recipe("Singing Axe", 182, false, new[] { ("Ingots", 15), ("Brilliant Amber", 1) }),
                    new Recipe("Thundering Axe", 202, false, new[] { ("Ingots", 15), ("Ecru Citrine", 1) }),
                    new Recipe("Heavy Ornate Axe", 222, false, new[] { ("Ingots", 15), ("Turquoise", 1) }),
                    new Recipe("Gargish Battle Axe", 242, false, new[] { ("Ingots", 14) }),
                    new Recipe("Gargish Axe", 262, false, new[] { ("Ingots", 14) }),
                    new Recipe("Dual Short Axes", 282, false, new[] { ("Ingots", 24) })
                }),

                new Category("Polearms", 101, new[]
                {
                    new Recipe("Bardiche", 2, false, new[] { ("Ingots", 18) }),
                    new Recipe("Bladed Staff", 22, false, new[] { ("Ingots", 12) }),
                    new Recipe("Double Bladed Staff", 42, false, new[] { ("Ingots", 16) }),
                    new Recipe("Halberd", 62, false, new[] { ("Ingots", 20) }),
                    new Recipe("Lance", 82, false, new[] { ("Ingots", 20) }),
                    new Recipe("Pike", 102, false, new[] { ("Ingots", 12) }),
                    new Recipe("Short Spear", 122, false, new[] { ("Ingots", 6) }),
                    new Recipe("Scythe", 142, false, new[] { ("Ingots", 14) }),
                    new Recipe("Spear", 162, false, new[] { ("Ingots", 12) }),
                    new Recipe("War Fork", 182, false, new[] { ("Ingots", 12) }),
                    new Recipe("Gargish Bardiche", 202, false, new[] { ("Ingots", 18) }),
                    new Recipe("Gargish War Fork", 222, false, new[] { ("Ingots", 12) }),
                    new Recipe("Gargish Scythe", 242, false, new[] { ("Ingots", 14) }),
                    new Recipe("Gargish Pike", 262, false, new[] { ("Ingots", 12) }),
                    new Recipe("Gargish Lance", 282, false, new[] { ("Ingots", 20) }),
                    new Recipe("Dual Pointed Spear", 302, false, new[] { ("Ingots", 16) })
                }),
                new Category("Bashing", 121, new[]
                {
                    new Recipe("Hammer Pick", 2, false, new[] { ("Ingots", 16) }),
                    new Recipe("Mace", 22, false, new[] { ("Ingots", 6) }),
                    new Recipe("Maul", 42, false, new[] { ("Ingots", 10) }),
                    new Recipe("Scepter", 62, false, new[] { ("Ingots", 10) }),
                    new Recipe("War Mace", 82, false, new[] { ("Ingots", 14) }),
                    new Recipe("War Hammer", 102, false, new[] { ("Ingots", 16) }),
                    new Recipe("Tessen", 122, false, new[] { ("Ingots", 16), ("Cloth", 10) }),
                    new Recipe("Diamond Mace", 142, false, new[] { ("Ingots", 20) }),
                    new Recipe("Shard Thrasher", 162, false, new[] { ("Ingots", 20), ("Eye of the Travesty", 1), ("Muculent", 10), ("Corruption", 10) }),
                    new Recipe("Ruby Mace", 182, false, new[] { ("Ingots", 20), ("Fire Ruby", 1) }),
                    new Recipe("Emerald Mace", 202, false, new[] { ("Ingots", 20), ("Perfect Emerald", 1) }),
                    new Recipe("Sapphire Mace", 222, false, new[] { ("Ingots", 20), ("Dark Sapphire", 1) }),
                    new Recipe("Silver-Etched Mace", 242, false, new[] { ("Ingots", 20), ("Blue Diamond", 1) }),
                    new Recipe("Gargish War Hammer", 262, false, new[] { ("Ingots", 16) }),
                    new Recipe("Gargish Maul", 282, false, new[] { ("Ingots", 10) }),
                    new Recipe("Gargish Tessen", 302, false, new[] { ("Ingots", 16), ("Cloth", 10) }),
                    new Recipe("Disc Mace", 322, false, new[] { ("Ingots", 20) })
                }),
                new Category("Cannons", 141, new[]
                {
                    new Recipe("Cannonball", 2, true, new[] { ("Ingots", 12) }),
                    new Recipe("Grapeshot", 22, true, new[] { ("Ingots", 12), ("Cloth", 2) }),
                    new Recipe("Culverin", 42, false, new[] { ("Ingots", 900), ("Boards or Logs", 50) }),
                    new Recipe("Carronade", 62, false, new[] { ("Ingots", 1800), ("Boards or Logs", 75) })
                }),
                new Category("Throwing", 161, new[]
                {
                    new Recipe("Boomerang", 2, false, new[] { ("Ingots", 5) }),
                    new Recipe("Cyclone", 22, false, new[] { ("Ingots", 9) }),
                    new Recipe("Soul Glaive", 42, false, new[] { ("Ingots", 9) })
                }),
                new Category("Miscellaneous", 181, new[]
                {
                    new Recipe("Dragon Gloves", 2, false, new[] { ("Dragon Scales", 16) }),
                    new Recipe("Dragon Helm", 22, false, new[] { ("Dragon Scales", 20) }),
                    new Recipe("Dragon Leggings", 42, false, new[] { ("Dragon Scales", 28) }),
                    new Recipe("Dragon Sleeves", 62, false, new[] { ("Dragon Scales", 24) }),
                    new Recipe("Dragon Breastplate", 82, false, new[] { ("Dragon Scales", 36) }),
                    new Recipe("Crushed Glass", 102, false, new[] { ("Blue Diamond", 1), ("glass sword", 5) }),
                    new Recipe("Powdered Iron", 122, false, new[] { ("white pearl", 1), ("Ingots", 20) }),
                    new Recipe("Metal Keg", 142, false, new[] { ("Ingots", 25) }),
                    new Recipe("Exodus Sacrificial Dagger", 162, false, new[] { ("Ingots", 12), ("Blue Diamond", 2), ("Fire Ruby", 2), ("a small piece of blackrock", 10) }),
                    new Recipe("Gloves Of Feudal Grip", 182, false, new[] { ("Dragon Scales", 18), ("Blue Diamond", 4), ("Gauntlets of Nobility", 1), ("Blood of the Dark Father", 5) })
                })
            }),
            new Profession("Bowcraft", new[]
            {
                new Category("Exit", 0, new Recipe[0]), // No recipes, empty category
                new Category("Cancel Make", 227, new Recipe[0]), // Placeholder
                new Category("Repair Item", 107, new Recipe[0]), // No recipes
                new Category("Mark Item", 127, new Recipe[0]), // No recipes
                new Category("Enhance Item", 167, new Recipe[0]), // No recipes
                new Category("Non Quest Item", 207, new Recipe[0]), // Placeholder
                new Category("Make Last", 47, new Recipe[0]), // No recipes
                new Category("Last Ten", 67, new Recipe[0]), // No recipes
                new Category("Materials", 1, new[]
                {
                    new Recipe("Elven Fletching", 2, false, new[] { ("Feathers", 20), ("faery dust", 1) }),
                    new Recipe("Kindling", 22, false, new[] { ("Boards or Logs", 1) }),
                    new Recipe("Shaft", 42, true, new[] { ("Boards or Logs", 1) })
                }),
                new Category("Ammunition", 21, new[]
                {
                    new Recipe("Arrow", 2, true, new[] { ("Arrow Shafts", 1), ("Feathers", 1) }),
                    new Recipe("Crossbow Bolt", 22, true, new[] { ("Arrow Shafts", 1), ("Feathers", 1) }),
                    new Recipe("Fukiya Dart", 42, true, new[] { ("Boards or Logs", 1) })
                }),
                new Category("Weapons", 41, new[]
                {
                    new Recipe("Bow", 2, false, new[] { ("Boards or Logs", 7) }),
                    new Recipe("Crossbow", 22, false, new[] { ("Boards or Logs", 7) }),
                    new Recipe("Heavy Crossbow", 42, false, new[] { ("Boards or Logs", 10) }),
                    new Recipe("Composite Bow", 62, false, new[] { ("Boards or Logs", 7) }),
                    new Recipe("Repeating Crossbow", 82, false, new[] { ("Boards or Logs", 10) }),
                    new Recipe("Yumi", 102, false, new[] { ("Boards or Logs", 10) }),
                    new Recipe("Elven Composite Longbow", 122, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Magical Shortbow", 142, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Blight Gripped Longbow", 162, false, new[] { ("Boards or Logs", 20), ("Lard of Paroxysmus", 1), ("Blight", 10), ("Corruption", 10) }),
                    new Recipe("Faerie Fire", 182, false, new[] { ("Boards or Logs", 20), ("Lard of Paroxysmus", 1), ("Putrefaction", 10), ("Taint", 10) }),
                    new Recipe("Silvani'S Feywood Bow", 202, false, new[] { ("Boards or Logs", 20), ("Lard of Paroxysmus", 1), ("Scourge", 10), ("Muculent", 10) }),
                    new Recipe("Mischief Maker", 222, false, new[] { ("Boards or Logs", 15), ("Dread Horn Mane", 1), ("Corruption", 10), ("Putrefaction", 10) }),
                    new Recipe("The Night Reaper", 242, false, new[] { ("Boards or Logs", 10), ("Dread Horn Mane", 1), ("Blight", 10), ("Scourge", 10) }),
                    new Recipe("Barbed Longbow", 262, false, new[] { ("Boards or Logs", 20), ("fire ruby", 1) }),
                    new Recipe("Slayer Longbow", 282, false, new[] { ("Boards or Logs", 20), ("brilliant amber", 1) }),
                    new Recipe("Frozen Longbow", 302, false, new[] { ("Boards or Logs", 20), ("turquoise", 1) }),
                    new Recipe("Longbow Of Might", 322, false, new[] { ("Boards or Logs", 10), ("blue diamond", 1) }),
                    new Recipe("Ranger'S Shortbow", 342, false, new[] { ("Boards or Logs", 15), ("perfect emerald", 1) }),
                    new Recipe("Lightweight Shortbow", 362, false, new[] { ("Boards or Logs", 15), ("white pearl", 1) }),
                    new Recipe("Mystical Shortbow", 382, false, new[] { ("Boards or Logs", 15), ("ecru citrine", 1) }),
                    new Recipe("Assassin'S Shortbow", 402, false, new[] { ("Boards or Logs", 15), ("dark sapphire", 1) })
                })
            }),
            new Profession("Carpentry", new[]
            {
                new Category("Exit", 0, new Recipe[0]), // No recipes, empty category
                new Category("Cancel Make", 227, new Recipe[0]), // No recipes, placeholder
                new Category("Repair Item", 107, new Recipe[0]), // No recipes
                new Category("Mark Item", 127, new Recipe[0]), // Placeholder
                new Category("Enhance Item", 167, new Recipe[0]), // No recipes
                new Category("Non Quest Item", 207, new Recipe[0]), // Placeholder
                new Category("Make Last", 47, new Recipe[0]), // Placeholder
                new Category("Last Ten", 67, new Recipe[0]), // Placeholder
                new Category("Other", 1, new[]
                {
                    new Recipe("Barrel Staves", 2, false, new[] { ("Boards or Logs", 5) }),
                    new Recipe("Barrel Lid", 22, false, new[] { ("Boards or Logs", 4) }),
                    new Recipe("Short Music Stand (Left)", 42, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Short Music Stand (Right)", 62, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Tall Music Stand (Left)", 82, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Tall Music Stand (Right)", 102, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Easel (South)", 122, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Easel (East)", 142, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Easel (North)", 162, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Red Hanging Lantern", 182, false, new[] { ("Boards or Logs", 5), ("Blank Scrolls", 10) }),
                    new Recipe("White Hanging Lantern", 202, false, new[] { ("Boards or Logs", 5), ("Blank Scrolls", 10) }),
                    new Recipe("Shoji Screen", 222, false, new[] { ("Boards or Logs", 75), ("Cloth", 60) }),
                    new Recipe("Bamboo Screen", 242, false, new[] { ("Boards or Logs", 75), ("Cloth", 60) }),
                    new Recipe("Fishing Pole", 262, false, new[] { ("Boards or Logs", 5), ("Cloth", 5) }),
                    new Recipe("Wooden Container Engraving Tool", 282, false, new[] { ("Boards or Logs", 4), ("Ingots", 2) }),
                    new Recipe("Runed Switch", 302, false, new[] { ("Boards or Logs", 2), ("Enchanted Switch", 1), ("Runed Prism", 1), ("jeweled filigree", 1) }),
                    new Recipe("Arcanist Statue (South)", 322, false, new[] { ("Boards or Logs", 250) }),
                    new Recipe("Arcanist Statue (East)", 342, false, new[] { ("Boards or Logs", 250) }),
                    new Recipe("Warrior Statue (South)", 362, false, new[] { ("Boards or Logs", 250) }),
                    new Recipe("Warrior Statue (East)", 382, false, new[] { ("Boards or Logs", 250) }),
                    new Recipe("Squirrel Statue (South)", 402, false, new[] { ("Boards or Logs", 250) }),
                    new Recipe("Squirrel Statue (East)", 422, false, new[] { ("Boards or Logs", 250) }),
                    new Recipe("Giant Replica Acorn", 442, false, new[] { ("Boards or Logs", 35) }),
                    new Recipe("Mounted Dread Horn", 462, false, new[] { ("Boards or Logs", 50), ("Pristine Dread Horn", 1) }),
                    new Recipe("Acid Proof Rope", 482, false, new[] { ("Greater Strength Potion", 2), ("Protection", 1), ("switch", 1) }),
                    new Recipe("Gargish Banner", 502, false, new[] { ("Boards or Logs", 50), ("Cloth", 50) }),
                    new Recipe("An Incubator", 522, false, new[] { ("Boards or Logs", 100) }),
                    new Recipe("A Chicken Coop", 542, false, new[] { ("Boards or Logs", 150) }),
                    new Recipe("Exodus Summoning Altar", 562, false, new[] { ("Boards or Logs", 100), ("high quality granite", 10), ("a small piece of blackrock", 10), ("nexus core ", 1) }),
                    new Recipe("Dark Wooden Sign Hanger", 582, false, new[] { ("Boards or Logs", 5) }),
                    new Recipe("Light Wooden Sign Hanger", 602, false, new[] { ("Boards or Logs", 5) })
                }), // Placeholder for "Other"
                new Category("Furniture", 21, new[]
                {
                    new Recipe("Foot Stool", 2, false, new[] { ("Boards or Logs", 9) }),
                    new Recipe("Stool", 22, false, new[] { ("Boards or Logs", 9) }),
                    new Recipe("Straw Chair", 42, false, new[] { ("Boards or Logs", 13) }),
                    new Recipe("Wooden Chair", 62, false, new[] { ("Boards or Logs", 13) }),
                    new Recipe("Vesper-Style Chair", 82, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Trinsic-Style Chair", 102, false, new[] { ("Boards or Logs", 13) }),
                    new Recipe("Wooden Bench", 122, false, new[] { ("Boards or Logs", 17) }),
                    new Recipe("Wooden Throne", 142, false, new[] { ("Boards or Logs", 17) }),
                    new Recipe("Magincia-Style Throne", 162, false, new[] { ("Boards or Logs", 19) }),
                    new Recipe("Small Table", 182, false, new[] { ("Boards or Logs", 17) }),
                    new Recipe("Writing Table", 202, false, new[] { ("Boards or Logs", 17) }),
                    new Recipe("Yew-Wood Table", 222, false, new[] { ("Boards or Logs", 27) }),
                    new Recipe("Large Table", 242, false, new[] { ("Boards or Logs", 23) }),
                    new Recipe("Elegant Low Table", 262, false, new[] { ("Boards or Logs", 35) }),
                    new Recipe("Plain Low Table", 282, false, new[] { ("Boards or Logs", 35) }),
                    new Recipe("Ornate Table (South)", 302, false, new[] { ("Boards or Logs", 60) }),
                    new Recipe("Ornate Table (East)", 322, false, new[] { ("Boards or Logs", 60) }),
                    new Recipe("Hardwood Table (South)", 342, false, new[] { ("Boards or Logs", 50) }),
                    new Recipe("Hardwood Table (East)", 362, false, new[] { ("Boards or Logs", 50) }),
                    new Recipe("Elven Podium", 382, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Ornate Elven Chair", 402, false, new[] { ("Boards or Logs", 30) }),
                    new Recipe("Cozy Elven Chair", 422, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Reading Chair", 442, false, new[] { ("Boards or Logs", 30) }),
                    new Recipe("Ter-Mur Style Chair", 462, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Ter-Mur Style Table", 482, false, new[] { ("Boards or Logs", 50) }),
                    new Recipe("Upholstered Chair", 502, false, new[] { ("Boards or Logs", 40), ("Cloth", 12) })
                }),
                new Category("Containers", 41, new[]
                {
                    new Recipe("Wooden Box", 2, false, new[] { ("Boards or Logs", 10) }),
                    new Recipe("Small Crate", 22, false, new[] { ("Boards or Logs", 8) }),
                    new Recipe("Medium Crate", 42, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Large Crate", 62, false, new[] { ("Boards or Logs", 18) }),
                    new Recipe("Wooden Chest", 82, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Wooden Shelf", 102, false, new[] { ("Boards or Logs", 25) }),
                    new Recipe("Armoire (Red)", 122, false, new[] { ("Boards or Logs", 35) }),
                    new Recipe("Armoire", 142, false, new[] { ("Boards or Logs", 35) }),
                    new Recipe("Plain Wooden Chest", 162, false, new[] { ("Boards or Logs", 30) }),
                    new Recipe("Ornate Wooden Chest", 182, false, new[] { ("Boards or Logs", 30) }),
                    new Recipe("Gilded Wooden Chest", 202, false, new[] { ("Boards or Logs", 30) }),
                    new Recipe("Wooden Footlocker", 222, false, new[] { ("Boards or Logs", 30) }),
                    new Recipe("Finished Wooden Chest", 242, false, new[] { ("Boards or Logs", 30) }),
                    new Recipe("Tall Cabinet", 262, false, new[] { ("Boards or Logs", 35) }),
                    new Recipe("Short Cabinet", 282, false, new[] { ("Boards or Logs", 35) }),
                    new Recipe("Red Armoire", 302, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Elegant Armoire", 322, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Maple Armoire", 342, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Cherry Armoire", 362, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Keg", 382, false, new[] { ("Barrel Staves", 3), ("Barrel Hoops", 1), ("Barrel Lids", 1) }),
                    new Recipe("Arcane Bookshelf (South)", 402, false, new[] { ("Boards or Logs", 80) }),
                    new Recipe("Arcane Bookshelf (East)", 422, false, new[] { ("Boards or Logs", 80) }),
                    new Recipe("Ornate Elven Chest (South)", 442, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Ornate Elven Chest (East)", 462, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Elven Wash Basin (South)", 482, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Elven Wash Basin (East)", 502, false, new[] { ("Boards or Logs", 40) }),
                    new Recipe("Elven Dresser (South)", 522, false, new[] { ("Boards or Logs", 45) }),
                    new Recipe("Elven Dresser (East)", 542, false, new[] { ("Boards or Logs", 45) }),
                    new Recipe("Elven Armoire (Fancy)", 562, false, new[] { ("Boards or Logs", 60) }),
                    new Recipe("Elven Armoire (Simple)", 582, false, new[] { ("Boards or Logs", 60) }),
                    new Recipe("Rarewood Chest", 602, false, new[] { ("Boards or Logs", 30) }),
                    new Recipe("Decorative Box", 622, false, new[] { ("Boards or Logs", 25) }),
                    new Recipe("Academic Bookcase", 642, false, new[] { ("Boards or Logs", 25), ("academic books", 1) }),
                    new Recipe("Gargish Chest", 662, false, new[] { ("Boards or Logs", 30) }),
                    new Recipe("Empty Liquor Barrel", 682, false, new[] { ("Boards or Logs", 50) })
                }),
                new Category("Weapons", 61, new[]
                {
                    new Recipe("Shepherd'S Crook", 2, false, new[] { ("Boards or Logs", 7) }),
                    new Recipe("Quarter Staff", 22, false, new[] { ("Boards or Logs", 6) }),
                    new Recipe("Gnarled Staff", 42, false, new[] { ("Boards or Logs", 7) }),
                    new Recipe("Bokuto", 62, false, new[] { ("Boards or Logs", 6) }),
                    new Recipe("Fukiya", 82, false, new[] { ("Boards or Logs", 6) }),
                    new Recipe("Tetsubo", 102, false, new[] { ("Boards or Logs", 10) }),
                    new Recipe("Wild Staff", 122, false, new[] { ("Boards or Logs", 16) }),
                    new Recipe("Phantom Staff", 142, false, new[] { ("Boards or Logs", 16), ("Diseased Bark", 1), ("Putrefaction", 10), ("Taint", 10) }),
                    new Recipe("Arcanist'S Wild Staff", 162, false, new[] { ("Boards or Logs", 16), ("white pearl", 1) }),
                    new Recipe("Ancient Wild Staff", 182, false, new[] { ("Boards or Logs", 16), ("perfect emerald", 1) }),
                    new Recipe("Thorned Wild Staff", 202, false, new[] { ("Boards or Logs", 16), ("fire ruby", 1) }),
                    new Recipe("Hardened Wild Staff", 222, false, new[] { ("Boards or Logs", 16), ("turquoise", 1) }),
                    new Recipe("Serpentstone Staff", 242, false, new[] { ("Boards or Logs", 16), ("ecru citrine", 1) }),
                    new Recipe("Gargish Gnarled Staff", 262, false, new[] { ("Boards or Logs", 16), ("ecru citrine", 1) }),
                    new Recipe("Club", 282, false, new[] { ("Boards or Logs", 9) }),
                    new Recipe("Black Staff", 302, false, new[] { ("Boards or Logs", 9) }),
                    new Recipe("Kotl Black Rod", 322, false, new[] { ("Boards or Logs", 20), ("Black Moonstone", 1), ("Staff of the Magi", 1) })
                }),
                new Category("Armor", 81, new[]
                {
                    new Recipe("Wooden Shield", 2, false, new[] { ("Boards or Logs", 9) }),
                    new Recipe("Woodland Chest", 22, false, new[] { ("Boards or Logs", 20), ("Bark Fragment", 6) }),
                    new Recipe("Woodland Arms", 42, false, new[] { ("Boards or Logs", 15), ("Bark Fragment", 4) }),
                    new Recipe("Woodland Gauntlets", 62, false, new[] { ("Boards or Logs", 15), ("Bark Fragment", 4) }),
                    new Recipe("Woodland Leggings", 82, false, new[] { ("Boards or Logs", 15), ("Bark Fragment", 4) }),
                    new Recipe("Woodland Gorget", 102, false, new[] { ("Boards or Logs", 15), ("Bark Fragment", 4) }),
                    new Recipe("Raven Helm", 122, false, new[] { ("Boards or Logs", 10), ("Bark Fragment", 4), ("feathers", 25) }),
                    new Recipe("Vulture Helm", 142, false, new[] { ("Boards or Logs", 10), ("Bark Fragment", 4), ("feathers", 25) }),
                    new Recipe("Winged Helm", 162, false, new[] { ("Boards or Logs", 10), ("Bark Fragment", 4), ("feathers", 60) }),
                    new Recipe("Ironwood Crown", 182, false, new[] { ("Boards or Logs", 10), ("Diseased Bark", 1), ("Corruption", 10), ("Putrefaction", 10) }),
                    new Recipe("Bramble Coat", 202, false, new[] { ("Boards or Logs", 10), ("Diseased Bark", 1), ("Taint", 10), ("Scourge", 10) }),
                    new Recipe("Darkwood Crown", 222, false, new[] { ("Boards or Logs", 10), ("Lard of Paroxysmus", 1), ("Blight", 10), ("Taint", 10) }),
                    new Recipe("Darkwood Chest", 242, false, new[] { ("Boards or Logs", 20), ("Dread Horn Mane", 1), ("Corruption", 10), ("Muculent", 10) }),
                    new Recipe("Darkwood Gorget", 262, false, new[] { ("Boards or Logs", 15), ("Diseased Bark", 1), ("Blight", 10), ("Scourge", 10) }),
                    new Recipe("Darkwood Leggings", 282, false, new[] { ("Boards or Logs", 15), ("Grizzled Bones", 1), ("Corruption", 10), ("Putrefaction", 10) }),
                    new Recipe("Darkwood Pauldrons", 302, false, new[] { ("Boards or Logs", 15), ("Eye of the Travesty", 1), ("Scourge", 10), ("Taint", 10) }),
                    new Recipe("Darkwood Gauntlets", 322, false, new[] { ("Boards or Logs", 15), ("Captured Essence", 1), ("Putrefaction", 10), ("Muculent", 10) }),
                    new Recipe("Gargish Wooden Shield", 342, false, new[] { ("Boards or Logs", 9) }),
                    new Recipe("Pirate Shield", 362, false, new[] { ("Boards or Logs", 12), ("Ingots", 8) })
                }),
                new Category("Instruments", 101, new[]
                {
                    new Recipe("Lap Harp", 2, false, new[] { ("Boards or Logs", 20), ("Cloth", 10) }),
                    new Recipe("Standing Harp", 22, false, new[] { ("Boards or Logs", 35), ("Cloth", 15) }),
                    new Recipe("Drum", 42, false, new[] { ("Boards or Logs", 20), ("Cloth", 10) }),
                    new Recipe("Lute", 62, false, new[] { ("Boards or Logs", 25), ("Cloth", 10) }),
                    new Recipe("Tambourine", 82, false, new[] { ("Boards or Logs", 15), ("Cloth", 10) }),
                    new Recipe("Tambourine (Tassel)", 102, false, new[] { ("Boards or Logs", 15), ("Cloth", 15) }),
                    new Recipe("Bamboo Flute", 122, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Aud-Char", 142, false, new[] { ("Boards or Logs", 35), ("Stones", 3) }),
                    new Recipe("Snake Charmer Flute", 162, false, new[] { ("Boards or Logs", 15), ("luminescent fungi", 3) }),
                    new Recipe("Cello", 182, false, new[] { ("Boards or Logs", 15), ("Cloth", 5) }),
                    new Recipe("Wall Mounted Bell (South)", 202, false, new[] { ("Boards or Logs", 50), ("Ingots", 50) }),
                    new Recipe("Wall Mounted Bell (East)", 222, false, new[] { ("Boards or Logs", 50), ("Ingots", 50) }),
                    new Recipe("Trumpet", 242, false, new[] { ("Boards or Logs", 10), ("Ingots", 15) }),
                    new Recipe("Cowbell", 262, false, new[] { ("Boards or Logs", 10), ("Ingots", 15) })
                }),
                new Category("Misc. Add-Ons", 121, new[]
                {
                    new Recipe("Bulletin Board", 2, false, new[] { ("Boards or Logs", 50) }),
                    new Recipe("Bulletin Board 2", 22, false, new[] { ("Boards or Logs", 50) }),
                    new Recipe("Parrot Perch", 42, false, new[] { ("Boards or Logs", 100) }),
                    new Recipe("Arcane Circle", 62, false, new[] { ("Boards or Logs", 100), ("blue diamond", 2), ("perfect emerald", 2), ("fire ruby", 2) }),
                    new Recipe("Tall Elven Bed (South)", 82, false, new[] { ("Boards or Logs", 200), ("Cloth", 100) }),
                    new Recipe("Tall Elven Bed (East)", 102, false, new[] { ("Boards or Logs", 200), ("Cloth", 100) }),
                    new Recipe("Elven Bed (South)", 122, false, new[] { ("Boards or Logs", 100), ("Cloth", 100) }),
                    new Recipe("Elven Bed (East)", 142, false, new[] { ("Boards or Logs", 100), ("Cloth", 100) }),
                    new Recipe("Elven Loveseat (East)", 162, false, new[] { ("Boards or Logs", 50) }),
                    new Recipe("Elven Loveseat (South)", 182, false, new[] { ("Boards or Logs", 50) }),
                    new Recipe("Alchemist Table (South)", 202, false, new[] { ("Boards or Logs", 70) }),
                    new Recipe("Alchemist Table (East)", 222, false, new[] { ("Boards or Logs", 70) }),
                    new Recipe("Small Bed (South)", 242, false, new[] { ("Boards or Logs", 100), ("Cloth", 100) }),
                    new Recipe("Small Bed (East)", 262, false, new[] { ("Boards or Logs", 100), ("Cloth", 100) }),
                    new Recipe("Large Bed (South)", 282, false, new[] { ("Boards or Logs", 150), ("Cloth", 150) }),
                    new Recipe("Large Bed (East)", 302, false, new[] { ("Boards or Logs", 150), ("Cloth", 150) }),
                    new Recipe("Dartboard (South)", 322, false, new[] { ("Boards or Logs", 5) }),
                    new Recipe("Dartboard (East)", 342, false, new[] { ("Boards or Logs", 5) }),
                    new Recipe("Ballot Box", 362, false, new[] { ("Boards or Logs", 5) }),
                    new Recipe("Pentagram", 382, false, new[] { ("Boards or Logs", 100), ("Ingots", 40) }),
                    new Recipe("Abbatoir", 402, false, new[] { ("Boards or Logs", 100), ("Ingots", 40) }),
                    new Recipe("Gargish Couch (East)", 422, false, new[] { ("Boards or Logs", 75) }),
                    new Recipe("Gargish Couch (South)", 442, false, new[] { ("Boards or Logs", 75) }),
                    new Recipe("Long Table (South)", 482, false, new[] { ("Boards or Logs", 80) }),
                    new Recipe("Long Table (East)", 502, false, new[] { ("Boards or Logs", 80) }),
                    new Recipe("Ter-Mur Style Dresser (East)", 522, false, new[] { ("Boards or Logs", 60) }),
                    new Recipe("Ter-Mur Style Dresser (South)", 542, false, new[] { ("Boards or Logs", 60) }),
                    new Recipe("Rustic Bench (South)", 562, false, new[] { ("Boards or Logs", 35) }),
                    new Recipe("Rustic Bench (East)", 582, false, new[] { ("Boards or Logs", 35) }),
                    new Recipe("Plain Wooden Shelf (South)", 602, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Plain Wooden Shelf (East)", 622, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Fancy Wooden Shelf (South)", 642, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Fancy Wooden Shelf (East)", 662, false, new[] { ("Boards or Logs", 15) }),
                    new Recipe("Fancy Loveseat (South)", 682, false, new[] { ("Boards or Logs", 80), ("Cloth", 24) }),
                    new Recipe("Fancy Loveseat (East)", 702, false, new[] { ("Boards or Logs", 80), ("Cloth", 24) }),
                    new Recipe("Plush Loveseat (South)", 722, false, new[] { ("Boards or Logs", 80), ("Cloth", 24) }),
                    new Recipe("Plush Loveseat (East)", 742, false, new[] { ("Boards or Logs", 80), ("Cloth", 24) }),
                    new Recipe("Plant Tapestry (South)", 762, false, new[] { ("Boards or Logs", 12), ("Cloth", 50) }),
                    new Recipe("Plant Tapestry (East)", 782, false, new[] { ("Boards or Logs", 12), ("Cloth", 50) }),
                    new Recipe("Metal Table (South)", 802, false, new[] { ("Boards or Logs", 20), ("Ingots", 15) }),
                    new Recipe("Metal Table (East)", 822, false, new[] { ("Boards or Logs", 20), ("Ingots", 15) }),
                    new Recipe("Long Metal Table (South)", 842, false, new[] { ("Boards or Logs", 40), ("Ingots", 30) }),
                    new Recipe("Long Metal Table (East)", 862, false, new[] { ("Boards or Logs", 40), ("Ingots", 30) }),
                    new Recipe("Wooden Table (South)", 882, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Wooden Table (East)", 902, false, new[] { ("Boards or Logs", 20) }),
                    new Recipe("Long Wooden Table (South)", 922, false, new[] { ("Boards or Logs", 80) }),
                    new Recipe("Long Wooden Table (East)", 942, false, new[] { ("Boards or Logs", 80) }),
                    new Recipe("Small Display Case (South)", 962, false, new[] { ("Boards or Logs", 40), ("Ingots", 10) }),
                    new Recipe("Small Display Case (East)", 982, false, new[] { ("Boards or Logs", 40), ("Ingots", 10) }),
                    new Recipe("Fancy Loveseat (North)", 1002, false, new[] { ("Boards or Logs", 80), ("Cloth", 48) }),
                    new Recipe("Fancy Loveseat (West)", 1022, false, new[] { ("Boards or Logs", 80), ("Cloth", 48) }),
                    new Recipe("Fancy Couch (North)", 1042, false, new[] { ("Boards or Logs", 80), ("Cloth", 48) }),
                    new Recipe("Fancy Couch (West)", 1062, false, new[] { ("Boards or Logs", 80), ("Cloth", 48) }),
                    new Recipe("Fancy Couch (South)", 1082, false, new[] { ("Boards or Logs", 80), ("Cloth", 48) }),
                    new Recipe("Fancy Couch (East)", 1102, false, new[] { ("Boards or Logs", 80), ("Cloth", 48) }),
                    new Recipe("Small Elegant Aquarium", 1122, false, new[] { ("Boards or Logs", 20), ("workable glass", 2), ("Sand", 5), ("Live Rock", 1) }),
                    new Recipe("Wall Mounted Aquarium", 1142, false, new[] { ("Boards or Logs", 50), ("workable glass", 4), ("Sand", 10), ("Live Rock", 3) }),
                    new Recipe("Large Elegant Aquarium", 1162, false, new[] { ("Boards or Logs", 100), ("workable glass", 8), ("Sand", 20), ("Live Rock", 5) })
                }),
                new Category("Tailoring And Cooking", 141, new[]
                {
                    new Recipe("Dressform (Front)", 2, false, new[] { ("Boards or Logs", 25), ("Cloth", 10) }),
                    new Recipe("Dressform (Side)", 22, false, new[] { ("Boards or Logs", 25), ("Cloth", 10) }),
                    new Recipe("Elven Spinning Wheel (East)", 42, false, new[] { ("Boards or Logs", 60), ("Cloth", 40) }),
                    new Recipe("Elven Spinning Wheel (South)", 62, false, new[] { ("Boards or Logs", 60), ("Cloth", 40) }),
                    new Recipe("Elven Oven (South)", 82, false, new[] { ("Boards or Logs", 80) }),
                    new Recipe("Elven Oven (East)", 102, false, new[] { ("Boards or Logs", 80) }),
                    new Recipe("Spinning Wheel (East)", 122, false, new[] { ("Boards or Logs", 75), ("Cloth", 25) }),
                    new Recipe("Spinning Wheel (South)", 142, false, new[] { ("Boards or Logs", 75), ("Cloth", 25) }),
                    new Recipe("Loom (East)", 162, false, new[] { ("Boards or Logs", 85), ("Cloth", 25) }),
                    new Recipe("Loom (South)", 182, false, new[] { ("Boards or Logs", 85), ("Cloth", 25) }),
                    new Recipe("Stone Oven (East)", 202, false, new[] { ("Boards or Logs", 85), ("Ingots", 125) }),
                    new Recipe("Stone Oven (South)", 222, false, new[] { ("Boards or Logs", 85), ("Ingots", 125) }),
                    new Recipe("Flour Mill (East)", 242, false, new[] { ("Boards or Logs", 100), ("Ingots", 50) }),
                    new Recipe("Flour Mill (South)", 262, false, new[] { ("Boards or Logs", 100), ("Ingots", 50) }),
                    new Recipe("Water Trough (East)", 282, false, new[] { ("Boards or Logs", 150) }),
                    new Recipe("Water Trough (South)", 302, false, new[] { ("Boards or Logs", 150) })
                }),
                new Category("Anvils And Forges", 161, new[]
                {
                    new Recipe("Elven Forge", 2, false, new[] { ("Boards or Logs", 200) }),
                    new Recipe("Soulforge", 22, false, new[] { ("Boards or Logs", 150), ("Ingots", 150), ("Relic Fragment", 1) }),
                    new Recipe("Small Forge", 42, false, new[] { ("Boards or Logs", 5), ("Ingots", 75) }),
                    new Recipe("Large Forge (East)", 62, false, new[] { ("Boards or Logs", 5), ("Ingots", 100) }),
                    new Recipe("Large Forge (South)", 82, false, new[] { ("Boards or Logs", 5), ("Ingots", 100) }),
                    new Recipe("Anvil (East)", 102, false, new[] { ("Boards or Logs", 5), ("Ingots", 150) }),
                    new Recipe("Anvil (South)", 122, false, new[] { ("Boards or Logs", 5), ("Ingots", 150) })
                }),
                new Category("Training", 181, new[]
                {
                    new Recipe("Training Dummy (East)", 2, false, new[] { ("Boards or Logs", 55), ("Cloth", 60) }),
                    new Recipe("Training Dummy (South)", 22, false, new[] { ("Boards or Logs", 55), ("Cloth", 60) }),
                    new Recipe("Pickpocket Dip (East)", 42, false, new[] { ("Boards or Logs", 65), ("Cloth", 60) }),
                    new Recipe("Pickpocket Dip (South)", 62, false, new[] { ("Boards or Logs", 65), ("Cloth", 60) })
                })
            }),
            new Profession("Cartography", new[]
            {
                new Category("Exit", 0, Array.Empty<Recipe>()),
                new Category("Cancel Make", 227, Array.Empty<Recipe>()),
                new Category("Non Quest Item", 207, Array.Empty<Recipe>()),
                new Category("Make Last", 47, Array.Empty<Recipe>()),
                new Category("Last Ten", 67, Array.Empty<Recipe>()),
                new Category("Maps", 1, new[]
                {
                    new Recipe("Local Map", 2, false, new[] { ("Blank Maps or Scrolls", 1) }),
                    new Recipe("City Map", 22, false, new[] { ("Blank Maps or Scrolls", 1) }),
                    new Recipe("Sea Chart", 42, false, new[] { ("Blank Maps or Scrolls", 1) }),
                    new Recipe("World Map", 62, false, new[] { ("Blank Maps or Scrolls", 1) }),
                    new Recipe("Tattered Wall Map (South)", 82, false, new[]
                    {
                        ("Level 1 Treasure Maps", 10),
                        ("Level 3 Treasure Maps", 5),
                        ("Level 4 Treasure Maps", 3),
                        ("Level 5 Treasure Maps", 1)
                    }),
                    new Recipe("Tattered Wall Map (East)", 102, false, new[]
                    {
                        ("Level 1 Treasure Maps", 10),
                        ("Level 3 Treasure Maps", 5),
                        ("Level 4 Treasure Maps", 3),
                        ("Level 5 Treasure Maps", 1)
                    }),
                    new Recipe("Wall Map Of Eodon", 122, false, new[]
                    {
                        ("Blank Maps or Scrolls", 50),
                        ("Unabridged Atlas of Eodon", 1)
                    }),
                    new Recipe("Star Chart", 142, false, new[] { ("Blank Maps or Scrolls", 1) })
                })
                
                
                }),
            new Profession("Cooking", new[]
            {
                new Category("Exit", 0, Array.Empty<Recipe>()),
                new Category("Cancel Make", 227, Array.Empty<Recipe>()),
                new Category("Mark Item", 127, Array.Empty<Recipe>()),
                new Category("Non Quest Item", 207, Array.Empty<Recipe>()),
                new Category("Make Last", 47, Array.Empty<Recipe>()),
                new Category("Last Ten", 67, Array.Empty<Recipe>()),
                new Category("Ingredients", 1, new[]
                {
                    new Recipe("Sack Of Flour", 2, false, new[] { ("Wheat", 2) }),
                    new Recipe("Dough", 22, false, new[] { ("Flour", 1), ("Water", 1) }),
                    new Recipe("Sweet Dough", 42, false, new[] { ("Dough", 1), ("Honey", 1) }),
                    new Recipe("Cake Mix", 62, false, new[] { ("Flour", 1), ("Sweet Dough", 1) }),
                    new Recipe("Cookie Mix", 82, false, new[] { ("Honey", 1), ("Sweet Dough", 1) }),
                    new Recipe("Cocoa Butter", 102, false, new[] { ("cocoa pulp", 1) }),
                    new Recipe("Cocoa Liquor", 122, false, new[] { ("cocoa pulp", 1), ("pewter bowl", 1) }),
                    new Recipe("Wheat Wort", 142, false, new[] { ("empty bottle", 1), ("Water", 1), ("Flour", 1) }),
                    new Recipe("Fish Oil Flask", 162, true, new[] { ("Coffee Grounds", 1), ("Raw Fish Steaks", 8) }),
                    new Recipe("Fresh Seasoning", 182, false, new[] { ("salt", 4) })
                }),
                
                new Category("Preparations", 21, new[]
                {
                    new Recipe("Unbaked Quiche", 2, false, new[] { ("Dough", 1), ("Eggs", 1) }),
                    new Recipe("Unbaked Meat Pie", 22, false, new[] { ("Dough", 1), ("Raw Meat", 1) }),
                    new Recipe("Uncooked Sausage Pizza", 42, false, new[] { ("Dough", 1), ("Sausage", 1) }),
                    new Recipe("Uncooked Cheese Pizza", 62, false, new[] { ("Dough", 1), ("Cheese", 1) }),
                    new Recipe("Unbaked Fruit Pie", 82, false, new[] { ("Dough", 1), ("Pears", 1) }),
                    new Recipe("Unbaked Peach Cobbler", 102, false, new[] { ("Dough", 1), ("Peaches", 1) }),
                    new Recipe("Unbaked Apple Pie", 122, false, new[] { ("Dough", 1), ("Apples", 1) }),
                    new Recipe("Unbaked Pumpkin Pie", 142, false, new[] { ("Dough", 1), ("Pumpkin", 1) }),
                    new Recipe("Green Tea", 162, false, new[] { ("Green Tea", 1), ("Water", 1) }),
                    new Recipe("Wasabi Clumps", 182, false, new[] { ("Water", 1), ("bowl of peas", 3) }),
                    new Recipe("Sushi Rolls", 202, false, new[] { ("Water", 1), ("Raw Fish Steaks", 10) }),
                    new Recipe("Sushi Platter", 222, false, new[] { ("Water", 1), ("Raw Fish Steaks", 10) }),
                    new Recipe("Savage Kin Paint", 242, false, new[] { ("Flour", 1), ("Tribal Berries", 1) }),
                    new Recipe("Egg Bomb", 262, false, new[] { ("Eggs", 1), ("Flour", 3) }),
                    new Recipe("Parrot Wafer", 282, false, new[] { ("Dough", 1), ("Honey", 1), ("Raw Fish Steaks", 10) }),
                    new Recipe("Plant Pigment", 302, false, new[] { ("Plant Clippings", 1), ("empty bottle", 1) }),
                    new Recipe("Natural Dye", 322, false, new[] { ("plant pigment", 1), ("color fixative", 1) }),
                    new Recipe("Color Fixative", 342, false, new[] { ("bottle of wine", 1), ("silver serpent venom", 1) }),
                    new Recipe("Wood Pulp", 362, false, new[] { ("Bark Fragment", 1), ("Water", 1) }),
                    new Recipe("Charcoal", 382, true, new[] { ("Boards or Logs", 1) }),
                    new Recipe("Charybdis Bait", 402, false, new[] { ("Samuel's Secret Sauce", 1), ("Raw Fish Steaks", 50), ("salted serpent steak", 3) })
                }),
                new Category("Baking", 41, new[]
                {
                    new Recipe("Bread Loaf", 2, false, new[] { ("Dough", 1) }),
                    new Recipe("Pan Of Cookies", 22, false, new[] { ("Cookie Mix", 1) }),
                    new Recipe("Cake", 42, false, new[] { ("Cake Mix", 1) }),
                    new Recipe("Muffins", 62, false, new[] { ("Sweet Dough", 1) }),
                    new Recipe("Baked Quiche", 82, false, new[] { ("Uncooked Quiches", 1) }),
                    new Recipe("Baked Meat Pie", 102, false, new[] { ("Uncooked Meat Pies", 1) }),
                    new Recipe("Sausage Pizza", 122, false, new[] { ("Uncooked Sausage Pizzas", 1) }),
                    new Recipe("Cheese Pizza", 142, false, new[] { ("Uncooked Cheese Pizzas", 1) }),
                    new Recipe("Baked Fruit Pie", 162, false, new[] { ("Uncooked Fruit Pies", 1) }),
                    new Recipe("Baked Peach Cobbler", 182, false, new[] { ("Uncooked Peach Cobblers", 1) }),
                    new Recipe("Baked Apple Pie", 202, false, new[] { ("Uncooked Apple Pies", 1) }),
                    new Recipe("Baked Pumpkin Pie", 222, false, new[] { ("Unbaked Pumpkin Pies", 1) }),
                    new Recipe("Miso Soup", 242, false, new[] { ("Raw Fish Steaks", 1), ("Water", 1) }),
                    new Recipe("White Miso Soup", 262, false, new[] { ("Raw Fish Steaks", 1), ("Water", 1) }),
                    new Recipe("Red Miso Soup", 282, false, new[] { ("Raw Fish Steaks", 1), ("Water", 1) }),
                    new Recipe("Awase Miso Soup", 302, false, new[] { ("Raw Fish Steaks", 1), ("Water", 1) }),
                    new Recipe("Gingerbread Cookie", 322, false, new[] { ("Cookie Mix", 1), ("Fresh Ginger", 1) }),
                    new Recipe("Three Tiered Cake", 342, false, new[] { ("Cake Mix", 3) })
                }),

                new Category("Barbecue", 61, new[]
                {
                    new Recipe("Cooked Bird", 2, true, new[] { ("Raw Birds", 1) }),
                    new Recipe("Chicken Leg", 22, true, new[] { ("Raw Chicken Legs", 1) }),
                    new Recipe("Fish Steak", 42, true, new[] { ("Raw Fish Steaks", 1) }),
                    new Recipe("Fried Eggs", 62, true, new[] { ("Eggs", 1) }),
                    new Recipe("Leg Of Lamb", 82, true, new[] { ("Raw Legs of Lamb", 1) }),
                    new Recipe("Cut Of Ribs", 102, true, new[] { ("Raw Ribs", 1) }),
                    new Recipe("Bowl Of Rotworm Stew", 122, true, new[] { ("raw rotworm meat", 1) }),
                    new Recipe("Blackrock Stew", 142, false, new[] { ("bowl of rotworm stew", 1), ("a piece of blackrock", 1) }),
                    new Recipe("Khaldun Tasty Treat", 162, true, new[] { ("Raw Fish Steaks", 40) }),
                    new Recipe("Hamburger", 182, false, new[] { ("bread loaf", 1), ("Raw Ribs", 1), ("head of lettuce", 1) }),
                    new Recipe("Hot Dog", 202, false, new[] { ("bread loaf", 1), ("sausage", 1) }),
                    new Recipe("Sausage", 222, false, new[] { ("ham", 1), ("dried herbs", 1) }),
                    new Recipe("Grilled Serpent Steak", 242, true, new[] { ("raw serpent steak", 1), ("fresh seasoning", 1) }),
                    new Recipe("Bbq Dino Ribs", 262, true, new[]
                    {
                        ("raw dino ribs", 1), ("fresh seasoning", 1), ("sack of sugar", 1), ("Samuel's Secret Sauce", 1)
                    }),
                    new Recipe("Waku Chicken", 282, true, new[] { ("Raw Birds", 1), ("dried herbs", 1), ("Samuel's Secret Sauce", 1) })
                }),
                new Category("Enchanted", 81, new[]
                {
                    new Recipe("Food Decoration Tool", 2, false, new[] { ("Dough", 1), ("Honey", 1) }),
                    new Recipe("Enchanted Apple", 42, false, new[] { ("Apples", 1), ("Greater Heal Potion", 1) }),
                    new Recipe("Grapes Of Wrath", 62, false, new[] { ("Grapes", 1), ("Greater Strength Potion", 1) }),
                    new Recipe("Fruit Bowl", 82, false, new[]
                    {
                        ("Wooden Bowl", 1), ("Pears", 3), ("Apples", 3), ("Bananas", 3)
                    })
                }),
                new Category("Chocolatiering", 101, new[]
                {
                    new Recipe("Sweet Cocoa Butter", 2, false, new[] { ("sack of sugar", 1), ("cocoa butter", 1) }),
                    new Recipe("Dark Chocolate", 22, false, new[] { ("sack of sugar", 1), ("cocoa butter", 1), ("cocoa liquor", 1) }),
                    new Recipe("Milk Chocolate", 42, false, new[]
                    {
                        ("sack of sugar", 1), ("cocoa butter", 1), ("cocoa liquor", 1), ("milk", 1)
                    }),
                    new Recipe("White Chocolate", 62, false, new[]
                    {
                        ("sack of sugar", 1), ("cocoa butter", 1), ("vanilla", 1), ("milk", 1)
                    }),
                    new Recipe("Dark Chocolate Nutcracker", 82, false, new[]
                    {
                        ("sweet cocoa butter", 1), ("foil sheet", 1), ("cocoa liquor", 1)
                    }),
                    new Recipe("Milk Chocolate Nutcracker", 102, false, new[]
                    {
                        ("sweet cocoa butter", 1), ("foil sheet", 1), ("cocoa liquor", 1)
                    }),
                    new Recipe("White Chocolate Nutcracker", 122, false, new[]
                    {
                        ("sweet cocoa butter", 1), ("foil sheet", 1), ("cocoa liquor", 1)
                    })
                }),
                new Category("Magical Fish Pies", 121, new[]
                {
                    new Recipe("Great Barracuda Pie", 2, false, new[]
                    {
                        ("great barracuda steak", 1), ("Mento Seasoning", 1), ("zoogi fungus", 1)
                    }),
                    new Recipe("Giant Koi Pie", 22, false, new[]
                    {
                        ("You don't have the components needed to make that.", 1), ("Mento Seasoning", 1), ("bowl of peas", 1), ("dough", 1)
                    }),
                    new Recipe("Fire Fish Pie", 42, false, new[]
                    {
                        ("fire fish steak", 1), ("dough", 1), ("carrot", 1), ("Samuel's Secret Sauce", 1)
                    }),
                    new Recipe("Stone Crab Pie", 62, false, new[]
                    {
                        ("stone crab meat", 1), ("dough", 1), ("head of cabbage", 1), ("Samuel's Secret Sauce", 1)
                    }),
                    new Recipe("Blue Lobster Pie", 82, false, new[]
                    {
                        ("blue lobster meat", 1), ("dough", 1), ("tribal berry", 1), ("Samuel's Secret Sauce", 1)
                    }),
                    new Recipe("Reaper Fish Pie", 102, false, new[]
                    {
                        ("reaper fish steak", 1), ("dough", 1), ("pumpkin", 1), ("Samuel's Secret Sauce", 1)
                    }),
                    new Recipe("Crystal Fish Pie", 122, false, new[]
                    {
                        ("crystal fish steak", 1), ("dough", 1), ("apple", 1), ("Samuel's Secret Sauce", 1)
                    }),
                    new Recipe("Bull Fish Pie", 142, false, new[]
                    {
                        ("bull fish steak", 1), ("dough", 1), ("squash", 1), ("Mento Seasoning", 1)
                    }),
                    new Recipe("Summer Dragonfish Pie", 162, false, new[]
                    {
                        ("summer dragonfish steak", 1), ("dough", 1), ("onion", 1), ("Mento Seasoning", 1)
                    }),
                    new Recipe("Fairy Salmon Pie", 182, false, new[]
                    {
                        ("fairy salmon steak", 1), ("dough", 1), ("ear of corn", 1), ("dark truffle", 1)
                    }),
                    new Recipe("Lava Fish Pie", 202, false, new[]
                    {
                        ("lava fish steak", 1), ("dough", 1), ("Cheese", 1), ("dark truffle", 1)
                    }),
                    new Recipe("Autumn Dragonfish Pie", 222, false, new[]
                    {
                        ("autumn dragonfish steak", 1), ("dough", 1), ("pear", 1), ("Mento Seasoning", 1)
                    }),
                    new Recipe("Spider Crab Pie", 242, false, new[]
                    {
                        ("spider crab meat", 1), ("dough", 1), ("head of lettuce", 1), ("Mento Seasoning", 1)
                    }),
                    new Recipe("Yellowtail Barracuda", 262, false, new[]
                    {
                        ("yellowtail barracuda steak", 1), ("dough", 1), ("bottle of wine", 1), ("Mento Seasoning", 1)
                    }),
                    new Recipe("Holy Mackerel Pie", 282, false, new[]
                    {
                        ("holy mackerel steak", 1), ("dough", 1), ("jar of honey", 1), ("Mento Seasoning", 1)
                    }),
                    new Recipe("Unicorn Fish Pie", 302, false, new[]
                    {
                        ("unicorn fish steak", 1), ("dough", 1), ("Fresh Ginger", 1), ("Mento Seasoning", 1)
                    })
                }),
                new Category("Beverages", 141, new[]
                {
                    new Recipe("Coffee", 2, false, new[] { ("Coffee Grounds", 1), ("Water", 1) }),
                    new Recipe("Green Tea", 22, false, new[] { ("Basket of Green Tea", 1), ("Water", 1) }),
                    new Recipe("Hot Cocoa", 42, false, new[]
                    {
                        ("Cocoa liquor", 1), ("Sack of sugar", 1), ("Pitcher of Milk", 1)
                    })
                })

                    }),
                    
                    new Profession("Inscription", new[]
            {
                new Category("Exit", 0, Array.Empty<Recipe>()),
                new Category("Cancel Make", 227, Array.Empty<Recipe>()),
                new Category("Mark Item", 127, Array.Empty<Recipe>()),
                new Category("Non Quest Item", 207, Array.Empty<Recipe>()),
                new Category("Make Last", 47, Array.Empty<Recipe>()),
                new Category("Last Ten", 67, Array.Empty<Recipe>()),
                new Category("First - Second Circle", 1, new[]
                {
                    new Recipe("Reactive Armor", 2, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Spiders' Silk", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Clumsy", 22, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Nightshade", 1) }),
                    new Recipe("Create Food", 42, false, new[] { ("Blank Scroll", 1), ("Garlic", 1), ("Ginseng", 1), ("Mandrake Root", 1) }),
                    new Recipe("Feeblemind", 62, false, new[] { ("Blank Scroll", 1), ("Nightshade", 1), ("Ginseng", 1) }),
                    new Recipe("Heal", 82, false, new[] { ("Blank Scroll", 1), ("Garlic", 1), ("Ginseng", 1), ("Spiders' Silk", 1) }),
                    new Recipe("Magic Arrow", 102, false, new[] { ("Blank Scroll", 1), ("Sulfurous Ash", 1) }),
                    new Recipe("Night Sight", 122, false, new[] { ("Blank Scroll", 1), ("Spiders' Silk", 1), ("Sulfurous Ash", 1) }),
                    new Recipe("Weaken", 142, false, new[] { ("Blank Scroll", 1), ("Garlic", 1), ("Nightshade", 1) }),
                    new Recipe("Agility", 162, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1) }),
                    new Recipe("Cunning", 182, false, new[] { ("Blank Scroll", 1), ("Nightshade", 1), ("Mandrake Root", 1) }),
                    new Recipe("Cure", 202, false, new[] { ("Blank Scroll", 1), ("Garlic", 1), ("Ginseng", 1) }),
                    new Recipe("Harm", 222, false, new[] { ("Blank Scroll", 1), ("Nightshade", 1), ("Spiders' Silk", 1) }),
                    new Recipe("Magic Trap", 242, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Spiders' Silk", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Magic Untrap", 262, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Sulfurous Ash", 1) }),
                    new Recipe("Protection", 282, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Ginseng", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Strength", 302, false, new[] { ("Blank Scroll", 1), ("Nightshade", 1), ("Mandrake Root", 1) })
                }),

                new Category("Third - Fourth Circle", 21, new[]
                {
                    new Recipe("Bless", 2, false, new[] { ("Blank Scroll", 1), ("Garlic", 1), ("Mandrake Root", 1) }),
                    new Recipe("Fireball", 22, false, new[] { ("Blank Scroll", 1), ("Black Pearl", 1) }),
                    new Recipe("Magic Lock", 42, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Garlic", 1), ("Sulfurous Ash", 1) }),
                    new Recipe("Poison", 62, false, new[] { ("Blank Scroll", 1), ("Nightshade", 1) }),
                    new Recipe("Telekinesis", 82, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1) }),
                    new Recipe("Teleport", 102, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1) }),
                    new Recipe("Unlock", 122, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Sulfurous Ash", 1) }),
                    new Recipe("Wall Of Stone", 142, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Garlic", 1) }),
                    new Recipe("Arch Cure", 162, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Ginseng", 1), ("Mandrake Root", 1)
                    }),
                    new Recipe("Arch Protection", 182, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Ginseng", 1), ("Mandrake Root", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Curse", 202, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Nightshade", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Fire Field", 222, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Spiders' Silk", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Greater Heal", 242, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Spiders' Silk", 1), ("Mandrake Root", 1), ("Ginseng", 1)
                    }),
                    new Recipe("Lightning", 262, false, new[] { ("Blank Scroll", 1), ("Mandrake Root", 1), ("Sulfurous Ash", 1) }),
                    new Recipe("Mana Drain", 282, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Spiders' Silk", 1), ("Mandrake Root", 1)
                    }),
                    new Recipe("Recall", 302, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Blood Moss", 1), ("Mandrake Root", 1)
                    })
                }),
                new Category("Fifth - Sixth Circle", 41, new[]
                {
                    new Recipe("Blade Spirits", 2, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Nightshade", 1), ("Mandrake Root", 1)
                    }),
                    new Recipe("Dispel Field", 22, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Garlic", 1), ("Spiders' Silk", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Incognito", 42, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Garlic", 1), ("Nightshade", 1) }),
                    new Recipe("Magic Reflection", 62, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Mandrake Root", 1), ("Spiders' Silk", 1)
                    }),
                    new Recipe("Mind Blast", 82, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Mandrake Root", 1), ("Nightshade", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Paralyze", 102, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Mandrake Root", 1), ("Spiders' Silk", 1)
                    }),
                    new Recipe("Poison Field", 122, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Nightshade", 1), ("Spiders' Silk", 1)
                    }),
                    new Recipe("Summon Creature", 142, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1), ("Spiders' Silk", 1)
                    }),
                    new Recipe("Dispel", 162, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Mandrake Root", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Energy Bolt", 182, false, new[] { ("Blank Scroll", 1), ("Black Pearl", 1), ("Nightshade", 1) }),
                    new Recipe("Explosion", 202, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1) }),
                    new Recipe("Invisibility", 222, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Nightshade", 1) }),
                    new Recipe("Mark", 242, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Black Pearl", 1), ("Mandrake Root", 1)
                    }),
                    new Recipe("Mass Curse", 262, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Mandrake Root", 1), ("Nightshade", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Paralyze Field", 282, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Ginseng", 1), ("Spiders' Silk", 1)
                    }),
                    new Recipe("Reveal", 302, false, new[] { ("Blank Scroll", 1), ("Blood Moss", 1), ("Sulfurous Ash", 1) })
                }),
                
                new Category("Seventh - Eighth Circle", 61, new[]
                {
                    new Recipe("Chain Lightning", 2, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Blood Moss", 1),
                        ("Mandrake Root", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Energy Field", 22, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Mandrake Root", 1),
                        ("Spiders' Silk", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Flamestrike", 42, false, new[]
                    {
                        ("Blank Scroll", 1), ("Spiders' Silk", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Gate Travel", 62, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Mandrake Root", 1),
                        ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Mana Vampire", 82, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Blood Moss", 1),
                        ("Mandrake Root", 1), ("Spiders' Silk", 1)
                    }),
                    new Recipe("Mass Dispel", 102, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Garlic", 1),
                        ("Mandrake Root", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Meteor Swarm", 122, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1),
                        ("Sulfurous Ash", 1), ("Spiders' Silk", 1)
                    }),
                    new Recipe("Polymorph", 142, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1),
                        ("Spiders' Silk", 1)
                    }),
                    new Recipe("Earthquake", 162, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1),
                        ("Ginseng", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Energy Vortex", 182, false, new[]
                    {
                        ("Blank Scroll", 1), ("Black Pearl", 1), ("Blood Moss", 1),
                        ("Mandrake Root", 1), ("Nightshade", 1)
                    }),
                    new Recipe("Resurrection", 202, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Garlic", 1),
                        ("Ginseng", 1)
                    }),
                    new Recipe("Summon Air Elemental", 222, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1),
                        ("Spiders' Silk", 1)
                    }),
                    new Recipe("Summon Daemon", 242, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1),
                        ("Spiders' Silk", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Summon Earth Elemental", 262, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1),
                        ("Spiders' Silk", 1)
                    }),
                    new Recipe("Summon Fire Elemental", 282, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1),
                        ("Spiders' Silk", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Summon Water Elemental", 302, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Mandrake Root", 1),
                        ("Spiders' Silk", 1)
                    })
                }),
                new Category("Spells Of Necromancy", 81, new[]
                {
                    new Recipe("Animate Dead", 2, false, new[]
                    {
                        ("Blank Scroll", 1), ("Grave Dust", 1), ("Daemon Blood", 1)
                    }),
                    new Recipe("Blood Oath", 22, false, new[]
                    {
                        ("Blank Scroll", 1), ("Daemon Blood", 1)
                    }),
                    new Recipe("Corpse Skin", 42, false, new[]
                    {
                        ("Blank Scroll", 1), ("Batwing", 1), ("Grave Dust", 1)
                    }),
                    new Recipe("Curse Weapon", 62, false, new[]
                    {
                        ("Blank Scroll", 1), ("Pig Iron", 1)
                    }),
                    new Recipe("Evil Omen", 82, false, new[]
                    {
                        ("Blank Scroll", 1), ("Batwing", 1), ("Nox Crystal", 1)
                    }),
                    new Recipe("Horrific Beast", 102, false, new[]
                    {
                        ("Blank Scroll", 1), ("Batwing", 1), ("Daemon Blood", 1)
                    }),
                    new Recipe("Lich Form", 122, false, new[]
                    {
                        ("Blank Scroll", 1), ("Grave Dust", 1), ("Daemon Blood", 1), ("Nox Crystal", 1)
                    }),
                    new Recipe("Mind Rot", 142, false, new[]
                    {
                        ("Blank Scroll", 1), ("Batwing", 1), ("Daemon Blood", 1), ("Pig Iron", 1)
                    }),
                    new Recipe("Pain Spike", 162, false, new[]
                    {
                        ("Blank Scroll", 1), ("Grave Dust", 1), ("Pig Iron", 1)
                    }),
                    new Recipe("Poison Strike", 182, false, new[]
                    {
                        ("Blank Scroll", 1), ("Nox Crystal", 1)
                    }),
                    new Recipe("Strangle", 202, false, new[]
                    {
                        ("Blank Scroll", 1), ("Daemon Blood", 1), ("Nox Crystal", 1)
                    }),
                    new Recipe("Summon Familiar", 222, false, new[]
                    {
                        ("Blank Scroll", 1), ("Batwing", 1), ("Grave Dust", 1), ("Daemon Blood", 1)
                    }),
                    new Recipe("Vampiric Embrace", 242, false, new[]
                    {
                        ("Blank Scroll", 1), ("Batwing", 1), ("Nox Crystal", 1), ("Pig Iron", 1)
                    }),
                    new Recipe("Vengeful Spirit", 262, false, new[]
                    {
                        ("Blank Scroll", 1), ("Batwing", 1), ("Grave Dust", 1), ("Pig Iron", 1)
                    }),
                    new Recipe("Wither", 282, false, new[]
                    {
                        ("Blank Scroll", 1), ("Grave Dust", 1), ("Nox Crystal", 1), ("Pig Iron", 1)
                    }),
                    new Recipe("Wraith Form", 302, false, new[]
                    {
                        ("Blank Scroll", 1), ("Nox Crystal", 1), ("Pig Iron", 1)
                    }),
                    new Recipe("Exorcism", 322, false, new[]
                    {
                        ("Blank Scroll", 1), ("Nox Crystal", 1), ("Grave Dust", 1)
                    })
                }),
                new Category("Other", 101, new[]
                {
                    new Recipe("Enchanted Switch", 2, false, new[]
                    {
                        ("Blank Scrolls", 1), ("Spiders' Silk", 1), ("Black Pearl", 1), ("Switch", 1)
                    }),
                    new Recipe("Runed Prism", 22, false, new[]
                    {
                        ("Blank Scrolls", 1), ("Spiders' Silk", 1), ("Black Pearl", 1), ("hollow prism", 1)
                    }),
                    new Recipe("Runebook", 42, false, new[]
                    {
                        ("Blank Scrolls", 8), ("Recall Scrolls", 1), ("Gate Scrolls", 1), ("Unmarked Runes", 1)
                    }),
                    new Recipe("Bulk Order Book", 62, false, new[]
                    {
                        ("Blank Scrolls", 10)
                    }),
                    new Recipe("Spellbook", 82, false, new[]
                    {
                        ("Blank Scrolls", 10)
                    }),
                    new Recipe("Scrapper'S Compendium", 102, false, new[]
                    {
                        ("Blank Scrolls", 100), ("Dread Horn Mane", 1), ("Taint", 10), ("Corruption", 10)
                    }),
                    new Recipe("Spellbook Engraving Tool", 122, false, new[]
                    {
                        ("Feathers", 1), ("Black Pearl", 7)
                    }),
                    new Recipe("Mysticism Spellbook", 142, false, new[]
                    {
                        ("Blank Scrolls", 10)
                    }),
                    new Recipe("Necromancer Spellbook", 162, false, new[]
                    {
                        ("Blank Scrolls", 10)
                    }),
                    new Recipe("Exodus Summoning Rite", 182, false, new[]
                    {
                        ("Daemon Blood", 5), ("Taint", 1), ("Daemon Bone", 5), ("A Daemon Summoning Scroll", 1)
                    }),
                    new Recipe("Prophetic Manuscript", 202, false, new[]
                    {
                        ("Ancient Parchment", 10), ("Antique Documents Kit", 1), ("wood pulp", 10), ("Beeswax", 5)
                    }),
                    new Recipe("Blank Scroll", 222, false, new[]
                    {
                        ("wood pulp", 1)
                    }),
                    new Recipe("Scroll Binder", 242, false, new[]
                    {
                        ("wood pulp", 1)
                    }),
                    new Recipe("Book (100 Pages)", 262, false, new[]
                    {
                        ("Blank Scrolls", 40), ("Beeswax", 2)
                    }),
                    new Recipe("Book (200 Pages)", 282, false, new[]
                    {
                        ("Blank Scrolls", 40), ("Beeswax", 2)
                    }),
                    new Recipe("Runic Atlas", 302, false, new[]
                    {
                        ("Blank Scrolls", 24), ("Recall Scrolls", 3), ("Gate Scrolls", 3)
                    })
                }),
                new Category("Spells Of Mysticism", 121, new[]
                {
                    new Recipe("Nether Bolt", 2, false, new[]
                    {
                        ("Blank Scroll", 1), ("Sulfurous Ash", 1), ("Black Pearl", 1)
                    }),
                    new Recipe("Healing Stone", 22, false, new[]
                    {
                        ("Blank Scroll", 1), ("Bone", 1), ("Garlic", 1), ("Ginseng", 1), ("Spiders' Silk", 1)
                    }),
                    new Recipe("Purge Magic", 42, false, new[]
                    {
                        ("Blank Scroll", 1), ("Fertile Dirt", 1), ("Garlic", 1), ("Mandrake Root", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Enchant", 62, false, new[]
                    {
                        ("Blank Scroll", 1), ("Spiders' Silk", 1), ("Mandrake Root", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Sleep", 82, false, new[]
                    {
                        ("Blank Scroll", 1), ("Spiders' Silk", 1), ("Black Pearl", 1), ("Nightshade", 1)
                    }),
                    new Recipe("Eagle Strike", 102, false, new[]
                    {
                        ("Blank Scroll", 1), ("Spiders' Silk", 1), ("Blood Moss", 1), ("Mandrake Root", 1), ("Bone", 1)
                    }),
                    new Recipe("Animated Weapon", 122, false, new[]
                    {
                        ("Blank Scroll", 1), ("Bone", 1), ("Black Pearl", 1), ("Mandrake Root", 1), ("Nightshade", 1)
                    }),
                    new Recipe("Stone Form", 142, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Fertile Dirt", 1), ("Garlic", 1)
                    }),
                    new Recipe("Spell Trigger", 162, false, new[]
                    {
                        ("Blank Scroll", 1), ("Spiders' Silk", 1), ("Mandrake Root", 1), ("Garlic", 1), ("Dragon's Blood", 1)
                    }),
                    new Recipe("Mass Sleep", 182, false, new[]
                    {
                        ("Blank Scroll", 1), ("Spiders' Silk", 1), ("Nightshade", 1), ("Ginseng", 1)
                    }),
                    new Recipe("Cleansing Winds", 202, false, new[]
                    {
                        ("Blank Scroll", 1), ("Ginseng", 1), ("Garlic", 1), ("Dragon's Blood", 1), ("Mandrake Root", 1)
                    }),
                    new Recipe("Bombard", 222, false, new[]
                    {
                        ("Blank Scroll", 1), ("Garlic", 1), ("Dragon's Blood", 1), ("Sulfurous Ash", 1), ("Blood Moss", 1)
                    }),
                    new Recipe("Spell Plague", 242, false, new[]
                    {
                        ("Blank Scroll", 1), ("Daemon Bone", 1), ("Dragon's Blood", 1), ("Nightshade", 1), ("Sulfurous Ash", 1)
                    }),
                    new Recipe("Hail Storm", 262, false, new[]
                    {
                        ("Blank Scroll", 1), ("Dragon's Blood", 1), ("Black Pearl", 1), ("Mandrake Root", 1), ("Blood Moss", 1)
                    }),
                    new Recipe("Nether Cyclone", 282, false, new[]
                    {
                        ("Blank Scroll", 1), ("Blood Moss", 1), ("Nightshade", 1), ("Sulfurous Ash", 1), ("Mandrake Root", 1)
                    }),
                    new Recipe("Rising Colossus", 302, false, new[]
                    {
                        ("Blank Scroll", 1), ("Daemon Bone", 1), ("Fertile Dirt", 1), ("Dragon's Blood", 1), ("Nightshade", 1)
                    })
                }),
                }),
            new Profession("Tailoring", new[]
            {
                new Category("Exit", 0, Array.Empty<Recipe>()),
                new Category("Cancel Make", 227, Array.Empty<Recipe>()),
                new Category("Repair Item", 107, Array.Empty<Recipe>()),
                new Category("Mark Item", 127, Array.Empty<Recipe>()),
                new Category("Enhance Item", 167, Array.Empty<Recipe>()),
                new Category("Non Quest Item", 207, Array.Empty<Recipe>()),
                new Category("Make Last", 47, Array.Empty<Recipe>()),
                new Category("Last Ten", 67, Array.Empty<Recipe>()),
                new Category("Materials", 1, new[]
                {
                    new Recipe("Cut-Up Cloth", 2, false, new[]
                    {
                        ("Bolts of Cloth", 1)
                    }),
                    new Recipe("Combine Cloth", 22, false, new[]
                    {
                        ("Yards of Cloth", 1)
                    }),
                    new Recipe("Powder Charge", 42, true, new[]
                    {
                        ("Yards of Cloth", 1), ("black powder", 4)
                    }),
                    new Recipe("Abyssal Cloth", 62, false, new[]
                    {
                        ("Yards of Cloth", 50), ("Crystalline Blackrock", 1)
                    })
                }),
                new Category("Hats", 21, new[]
                {
                    new Recipe("Skullcap", 2, false, new[]
                    {
                        ("Yards of Cloth", 2)
                    }),
                    new Recipe("Bandana", 22, false, new[]
                    {
                        ("Yards of Cloth", 2)
                    }),
                    new Recipe("Floppy Hat", 42, false, new[]
                    {
                        ("Yards of Cloth", 11)
                    }),
                    new Recipe("Cap", 62, false, new[]
                    {
                        ("Yards of Cloth", 11)
                    }),
                    new Recipe("Wide-Brim Hat", 82, false, new[]
                    {
                        ("Yards of Cloth", 12)
                    }),
                    new Recipe("Straw Hat", 102, false, new[]
                    {
                        ("Yards of Cloth", 10)
                    }),
                    new Recipe("Tall Straw Hat", 122, false, new[]
                    {
                        ("Yards of Cloth", 13)
                    }),
                    new Recipe("Wizard's Hat", 142, false, new[]
                    {
                        ("Yards of Cloth", 15)
                    }),
                    new Recipe("Bonnet", 162, false, new[]
                    {
                        ("Yards of Cloth", 11)
                    }),
                    new Recipe("Feathered Hat", 182, false, new[]
                    {
                        ("Yards of Cloth", 12)
                    }),
                    new Recipe("Tricorne Hat", 202, false, new[]
                    {
                        ("Yards of Cloth", 12)
                    }),
                    new Recipe("Jester Hat", 222, false, new[]
                    {
                        ("Yards of Cloth", 15)
                    }),
                    new Recipe("Flower Garland", 242, false, new[]
                    {
                        ("Yards of Cloth", 5)
                    }),
                    new Recipe("Cloth Ninja Hood", 262, false, new[]
                    {
                        ("Yards of Cloth", 13)
                    }),
                    new Recipe("Kasa", 282, false, new[]
                    {
                        ("Yards of Cloth", 12)
                    }),
                    new Recipe("Orc Mask", 302, false, new[]
                    {
                        ("Yards of Cloth", 12)
                    }),
                    new Recipe("Bear Mask", 322, false, new[]
                    {
                        ("Yards of Cloth", 15)
                    }),
                    new Recipe("Deer Mask", 342, false, new[]
                    {
                        ("Yards of Cloth", 15)
                    }),
                    new Recipe("Tribal Mask", 362, false, new[]
                    {
                        ("Yards of Cloth", 12)
                    }),
                    new Recipe("Tribal Mask 2", 382, false, new[]
                    {
                        ("Yards of Cloth", 12)
                    }),
                    new Recipe("Chef's Toque", 402, false, new[]
                    {
                        ("Yards of Cloth", 11)
                    }),
                    new Recipe("Krampus Minion Hat", 422, false, new[]
                    {
                        ("Yards of Cloth", 8)
                    }),
                    new Recipe("Assassin's Cowl", 442, false, new[]
                    {
                        ("Yards of Cloth", 5), ("Leather or Hides", 5), ("vile tentacles", 5)
                    }),
                    new Recipe("Mage's Hood", 462, false, new[]
                    {
                        ("Yards of Cloth", 5), ("Leather or Hides", 5), ("void core", 5)
                    }),
                    new Recipe("Cowl Of The Mace & Shield", 482, false, new[]
                    {
                        ("Yards of Cloth", 5), ("Leather or Hides", 5),
                        ("Mace and Shield Reading Glasses", 1), ("vile tentacles", 10)
                    }),
                    new Recipe("Mage's Hood Of Scholarly Insight", 502, false, new[]
                    {
                        ("Yards of Cloth", 5), ("Leather or Hides", 5),
                        ("the scholar's halo", 1), ("void core", 10)
                    })
                }),
                new Category("Shirts And Pants", 41, new[]
            {
                new Recipe("Doublet", 2, false, new[] { ("Yards of Cloth", 8) }),
                new Recipe("Shirt", 22, false, new[] { ("Yards of Cloth", 8) }),
                new Recipe("Fancy Shirt", 42, false, new[] { ("Yards of Cloth", 8) }),
                new Recipe("Tunic", 62, false, new[] { ("Yards of Cloth", 12) }),
                new Recipe("Surcoat", 82, false, new[] { ("Yards of Cloth", 14) }),
                new Recipe("Plain Dress", 102, false, new[] { ("Yards of Cloth", 10) }),
                new Recipe("Fancy Dress", 122, false, new[] { ("Yards of Cloth", 12) }),
                new Recipe("Cloak", 142, false, new[] { ("Yards of Cloth", 14) }),
                new Recipe("Robe", 162, false, new[] { ("Yards of Cloth", 16) }),
                new Recipe("Jester Suit", 182, false, new[] { ("Yards of Cloth", 24) }),
                new Recipe("Fur Cape", 202, false, new[] { ("Yards of Cloth", 13) }),
                new Recipe("Gilded Dress", 222, false, new[] { ("Yards of Cloth", 16) }),
                new Recipe("Formal Shirt", 242, false, new[] { ("Yards of Cloth", 16) }),
                new Recipe("Cloth Ninja Jacket", 262, false, new[] { ("Yards of Cloth", 12) }),
                new Recipe("Kamishimo", 282, false, new[] { ("Yards of Cloth", 15) }),
                new Recipe("Hakama-Shita", 302, false, new[] { ("Yards of Cloth", 14) }),
                new Recipe("Male Kimono", 322, false, new[] { ("Yards of Cloth", 16) }),
                new Recipe("Female Kimono", 342, false, new[] { ("Yards of Cloth", 16) }),
                new Recipe("Jin-Baori", 362, false, new[] { ("Yards of Cloth", 12) }),
                new Recipe("Short Pants", 382, false, new[] { ("Yards of Cloth", 6) }),
                new Recipe("Long Pants", 402, false, new[] { ("Yards of Cloth", 8) }),
                new Recipe("Kilt", 422, false, new[] { ("Yards of Cloth", 8) }),
                new Recipe("Skirt", 442, false, new[] { ("Yards of Cloth", 10) }),
                new Recipe("Fur Sarong", 462, false, new[] { ("Yards of Cloth", 12) }),
                new Recipe("Hakama", 482, false, new[] { ("Yards of Cloth", 16) }),
                new Recipe("Tattsuke-Hakama", 502, false, new[] { ("Yards of Cloth", 16) }),
                new Recipe("Elven Shirt", 522, false, new[] { ("Yards of Cloth", 10) }),
                new Recipe("Elven Shirt 2", 542, false, new[] { ("Yards of Cloth", 10) }),
                new Recipe("Elven Pants", 562, false, new[] { ("Yards of Cloth", 12) }),
                new Recipe("Elven Robe", 582, false, new[] { ("Yards of Cloth", 30) }),
                new Recipe("Female Elven Robe", 602, false, new[] { ("Yards of Cloth", 30) }),
                new Recipe("Woodland Belt", 622, false, new[] { ("Yards of Cloth", 10) }),
                new Recipe("Gargish Robe", 642, false, new[] { ("Yards of Cloth", 16) }),
                new Recipe("Gargish Fancy Robe", 662, false, new[] { ("Yards of Cloth", 16) }),
                new Recipe("Robe Of Rite", 682, false, new[] { ("Leather or Hides", 6), ("Fire Ruby", 1), ("Gold Dust", 5), ("abyssal cloth", 6) }),
                new Recipe("Gilded Kilt", 702, false, new[] { ("Yards of Cloth", 8) }),
                new Recipe("Checkered Kilt", 722, false, new[] { ("Yards of Cloth", 8) }),
                new Recipe("Fancy Kilt", 742, false, new[] { ("Yards of Cloth", 8) }),
                new Recipe("Flowered Dress", 762, false, new[] { ("Yards of Cloth", 18) }),
                new Recipe("Evening Gown", 782, false, new[] { ("Yards of Cloth", 18) })
            }),
                new Category("Miscellaneous", 61, new[]
            {
                new Recipe("Body Sash", 2, false, new[] { ("Yards of Cloth", 4) }),
                new Recipe("Half Apron", 22, false, new[] { ("Yards of Cloth", 6) }),
                new Recipe("Full Apron", 42, false, new[] { ("Yards of Cloth", 10) }),
                new Recipe("A Bag", 62, false, new[] { ("Yards of Cloth", 6) }),
                new Recipe("Pouch", 82, false, new[] { ("Yards of Cloth", 8) }),
                new Recipe("Backpack", 102, false, new[] { ("Leather or Hides", 15) }),
                new Recipe("Obi", 122, false, new[] { ("Yards of Cloth", 6) }),
                new Recipe("Elven Quiver", 142, false, new[] { ("Leather or Hides", 28) }),
                new Recipe("Quiver Of Fire", 162, false, new[] { ("Leather or Hides", 28), ("Fire Ruby", 15) }),
                new Recipe("Quiver Of Ice", 182, false, new[] { ("Leather or Hides", 28), ("White Pearl", 15) }),
                new Recipe("Quiver Of Blight", 202, false, new[] { ("Leather or Hides", 28), ("Blight", 10) }),
                new Recipe("Quiver Of Lightning", 222, false, new[] { ("Leather or Hides", 28), ("Corruption", 10) }),
                new Recipe("Leather Container Engraving Tool", 242, false, new[] { ("Bones", 1), ("Leather or Hides", 6), ("Spools of Thread", 2), ("dyes", 1) }),
                new Recipe("Gargish Half Apron", 262, false, new[] { ("Yards of Cloth", 6) }),
                new Recipe("Gargish Sash", 282, false, new[] { ("Yards of Cloth", 4) }),
                new Recipe("Oil Cloth", 302, false, new[] { ("Yards of Cloth", 1) }),
                new Recipe("Goza (East)", 322, false, new[] { ("Yards of Cloth", 25) }),
                new Recipe("Goza (South)", 342, false, new[] { ("Yards of Cloth", 25) }),
                new Recipe("Square Goza (East)", 362, false, new[] { ("Yards of Cloth", 25) }),
                new Recipe("Square Goza (South)", 382, false, new[] { ("Yards of Cloth", 25) }),
                new Recipe("Brocade Goza (East)", 402, false, new[] { ("Yards of Cloth", 25) }),
                new Recipe("Brocade Goza (South)", 422, false, new[] { ("Yards of Cloth", 25) }),
                new Recipe("Square Brocade Goza (East)", 442, false, new[] { ("Yards of Cloth", 25) }),
                new Recipe("Square Brocade Goza (South)", 462, false, new[] { ("Yards of Cloth", 25) }),
                new Recipe("Square Goza", 482, false, new[] { ("Yards of Cloth", 25) }),
                new Recipe("Mace Belt", 502, false, new[] { ("Yards of Cloth", 5), ("Leather or Hides", 5), ("lodestone", 5) }),
                new Recipe("Sword Belt", 522, false, new[] { ("Yards of Cloth", 5), ("Leather or Hides", 5), ("lodestone", 5) }),
                new Recipe("Dagger Belt", 542, false, new[] { ("Yards of Cloth", 5), ("Leather or Hides", 5), ("lodestone", 5) }),
                new Recipe("Elegant Collar", 562, false, new[] { ("Yards of Cloth", 5), ("Leather or Hides", 5), ("fey wings", 5) }),
                new Recipe("Crimson Mace Belt", 582, false, new[] { ("Yards of Cloth", 5), ("Leather or Hides", 5), ("Crimson Cincture", 1), ("lodestone", 10) }),
                new Recipe("Crimson Sword Belt", 602, false, new[] { ("Yards of Cloth", 5), ("Leather or Hides", 5), ("Crimson Cincture", 1), ("lodestone", 10) }),
                new Recipe("Crimson Dagger Belt", 622, false, new[] { ("Yards of Cloth", 5), ("Leather or Hides", 5), ("Crimson Cincture", 1), ("lodestone", 10) }),
                new Recipe("Elegant Collar Of Fortune", 642, false, new[] { ("Yards of Cloth", 5), ("Leather or Hides", 5), ("Leurocian's Mempo of Fortune", 1), ("fey wings", 10) })
            }),
                new Category("Footwear", 81, new[]
                {
                    new Recipe("Elven Boots", 2, false, new[] { ("Leather or Hides", 15) }),
                    new Recipe("Fur Boots", 22, false, new[] { ("Yards of Cloth", 12) }),
                    new Recipe("Ninja Tabi", 42, false, new[] { ("Yards of Cloth", 10) }),
                    new Recipe("Waraji And Tabi", 62, false, new[] { ("Yards of Cloth", 6) }),
                    new Recipe("Sandals", 82, false, new[] { ("Leather or Hides", 4) }),
                    new Recipe("Shoes", 102, false, new[] { ("Leather or Hides", 6) }),
                    new Recipe("Boots", 122, false, new[] { ("Leather or Hides", 8) }),
                    new Recipe("Thigh Boots", 142, false, new[] { ("Leather or Hides", 10) }),
                    new Recipe("Gargish Leather Talons", 162, false, new[] { ("Leather or Hides", 6) }),
                    new Recipe("Jester Shoes", 182, false, new[] { ("Yards of Cloth", 6) }),
                    new Recipe("Krampus Minion Boots", 202, false, new[] { ("Leather or Hides", 6), ("Yards of Cloth", 4) }),
                    new Recipe("Krampus Minion Talons", 222, false, new[] { ("Leather or Hides", 6), ("Yards of Cloth", 4) })
                }),
                new Category("Leather Armor", 101, new[]
            {
                new Recipe("Spell Woven Britches", 2, false, new[] { ("Leather or Hides", 15), ("Eye of the Travesty", 1), ("Putrefaction", 10), ("Scourge", 10) }),
                new Recipe("Song Woven Mantle", 22, false, new[] { ("Leather or Hides", 15), ("Eye of the Travesty", 1), ("Blight", 10), ("Muculent", 10) }),
                new Recipe("Stitcher's Mittens", 42, false, new[] { ("Leather or Hides", 15), ("Captured Essence", 1), ("Corruption", 10), ("Taint", 10) }),
                new Recipe("Leather Gorget", 62, false, new[] { ("Leather or Hides", 4) }),
                new Recipe("Leather Cap", 82, false, new[] { ("Leather or Hides", 2) }),
                new Recipe("Leather Gloves", 102, false, new[] { ("Leather or Hides", 3) }),
                new Recipe("Leather Sleeves", 122, false, new[] { ("Leather or Hides", 4) }),
                new Recipe("Leather Leggings", 142, false, new[] { ("Leather or Hides", 10) }),
                new Recipe("Leather Tunic", 162, false, new[] { ("Leather or Hides", 12) }),
                new Recipe("Leather Jingasa", 182, false, new[] { ("Leather or Hides", 4) }),
                new Recipe("Leather Mempo", 202, false, new[] { ("Leather or Hides", 8) }),
                new Recipe("Leather Do", 222, false, new[] { ("Leather or Hides", 12) }),
                new Recipe("Leather Hiro Sode", 242, false, new[] { ("Leather or Hides", 5) }),
                new Recipe("Leather Suneate", 262, false, new[] { ("Leather or Hides", 12) }),
                new Recipe("Leather Haidate", 282, false, new[] { ("Leather or Hides", 12) }),
                new Recipe("Leather Ninja Pants", 302, false, new[] { ("Leather or Hides", 13) }),
                new Recipe("Leather Ninja Jacket", 322, false, new[] { ("Leather or Hides", 13) }),
                new Recipe("Leather Ninja Belt", 342, false, new[] { ("Leather or Hides", 5) }),
                new Recipe("Leather Ninja Mitts", 362, false, new[] { ("Leather or Hides", 12) }),
                new Recipe("Leather Ninja Hood", 382, false, new[] { ("Leather or Hides", 14) }),
                new Recipe("Leaf Tunic", 402, false, new[] { ("Leather or Hides", 15) }),
                new Recipe("Leaf Arms", 422, false, new[] { ("Leather or Hides", 12) }),
                new Recipe("Leaf Gloves", 442, false, new[] { ("Leather or Hides", 10) }),
                new Recipe("Leaf Leggings", 462, false, new[] { ("Leather or Hides", 15) }),
                new Recipe("Leaf Gorget", 482, false, new[] { ("Leather or Hides", 12) }),
                new Recipe("Leaf Tonlet", 502, false, new[] { ("Leather or Hides", 12) }),
                new Recipe("Gargish Leather Arms", 522, false, new[] { ("Leather or Hides", 8) }),
                new Recipe("Gargish Leather Chest", 542, false, new[] { ("Leather or Hides", 8) }),
                new Recipe("Gargish Leather Leggings", 562, false, new[] { ("Leather or Hides", 10) }),
                new Recipe("Gargish Leather Kilt", 582, false, new[] { ("Leather or Hides", 6) }),
                new Recipe("Gargish Leather Arms 2", 602, false, new[] { ("Leather or Hides", 8) }),
                new Recipe("Gargish Leather Chest 2", 622, false, new[] { ("Leather or Hides", 8) }),
                new Recipe("Gargish Leather Leggings 2", 642, false, new[] { ("Leather or Hides", 10) }),
                new Recipe("Gargish Leather Kilt 2", 662, false, new[] { ("Leather or Hides", 6) }),
                new Recipe("Gargish Leather Wing Armor", 682, false, new[] { ("Leather or Hides", 12) }),
                new Recipe("Tiger Pelt Chest", 702, false, new[] { ("Tiger Pelt", 4), ("Leather or Hides", 8) }),
                new Recipe("Tiger Pelt Leggings", 722, false, new[] { ("Tiger Pelt", 8), ("Leather or Hides", 4) }),
                new Recipe("Tiger Pelt Shorts", 742, false, new[] { ("Tiger Pelt", 4), ("Leather or Hides", 2) }),
                new Recipe("Tiger Pelt Helm", 762, false, new[] { ("Tiger Pelt", 2), ("Leather or Hides", 2) }),
                new Recipe("Tiger Pelt Collar", 782, false, new[] { ("Tiger Pelt", 2), ("Leather or Hides", 1) }),
                new Recipe("Dragon Turtle Hide Chest", 802, false, new[] { ("Leather or Hides", 8), ("Dragon Turtle Scute", 2) }),
                new Recipe("Dragon Turtle Hide Leggings", 822, false, new[] { ("Leather or Hides", 8), ("Dragon Turtle Scute", 4) }),
                new Recipe("Dragon Turtle Hide Helm", 842, false, new[] { ("Leather or Hides", 2), ("Dragon Turtle Scute", 1) }),
                new Recipe("Dragon Turtle Hide Arms", 862, false, new[] { ("Leather or Hides", 4), ("Dragon Turtle Scute", 2) })
            }),
                new Category("Cloth Armor", 121, new[]
                {
                    new Recipe("Gargish Cloth Arms", 2, false, new[] { ("Yards of Cloth", 8) }),
                    new Recipe("Gargish Cloth Chest", 22, false, new[] { ("Yards of Cloth", 8) }),
                    new Recipe("Gargish Cloth Leggings", 42, false, new[] { ("Yards of Cloth", 10) }),
                    new Recipe("Gargish Cloth Kilt", 62, false, new[] { ("Yards of Cloth", 6) }),
                    new Recipe("Gargish Cloth Arms 2", 82, false, new[] { ("Yards of Cloth", 8) }),
                    new Recipe("Gargish Cloth Chest 2", 102, false, new[] { ("Yards of Cloth", 8) }),
                    new Recipe("Gargish Cloth Leggings 2", 122, false, new[] { ("Yards of Cloth", 10) }),
                    new Recipe("Gargish Cloth Kilt 2", 142, false, new[] { ("Yards of Cloth", 6) }),
                    new Recipe("Gargish Cloth Wing Armor", 162, false, new[] { ("Yards of Cloth", 12) })
                }),
                new Category("Studded Armor", 141, new[]
                {
                    new Recipe("Studded Gorget", 2, false, new[] { ("Leather or Hides", 6) }),
                    new Recipe("Studded Gloves", 22, false, new[] { ("Leather or Hides", 8) }),
                    new Recipe("Studded Sleeves", 42, false, new[] { ("Leather or Hides", 10) }),
                    new Recipe("Studded Leggings", 62, false, new[] { ("Leather or Hides", 12) }),
                    new Recipe("Studded Tunic", 82, false, new[] { ("Leather or Hides", 14) }),
                    new Recipe("Studded Mempo", 102, false, new[] { ("Leather or Hides", 8) }),
                    new Recipe("Studded Do", 122, false, new[] { ("Leather or Hides", 14) }),
                    new Recipe("Studded Hiro Sode", 142, false, new[] { ("Leather or Hides", 8) }),
                    new Recipe("Studded Suneate", 162, false, new[] { ("Leather or Hides", 14) }),
                    new Recipe("Studded Haidate", 182, false, new[] { ("Leather or Hides", 14) }),
                    new Recipe("Hide Tunic", 202, false, new[] { ("Leather or Hides", 15) }),
                    new Recipe("Hide Pauldrons", 222, false, new[] { ("Leather or Hides", 12) }),
                    new Recipe("Hide Gloves", 242, false, new[] { ("Leather or Hides", 10) }),
                    new Recipe("Hide Pants", 262, false, new[] { ("Leather or Hides", 15) }),
                    new Recipe("Hide Gorget", 282, false, new[] { ("Leather or Hides", 12) })
                }),
                new Category("Female Armor", 161, new[]
                {
                    new Recipe("Leather Shorts", 2, false, new[] { ("Leather or Hides", 8) }),
                    new Recipe("Leather Skirt", 22, false, new[] { ("Leather or Hides", 6) }),
                    new Recipe("Leather Bustier", 42, false, new[] { ("Leather or Hides", 6) }),
                    new Recipe("Studded Bustier", 62, false, new[] { ("Leather or Hides", 8) }),
                    new Recipe("Female Leather Armor", 82, false, new[] { ("Leather or Hides", 8) }),
                    new Recipe("Studded Armor", 102, false, new[] { ("Leather or Hides", 10) }),
                    new Recipe("Tiger Pelt Bustier", 122, false, new[] { ("Tiger Pelt", 3), ("Leather or Hides", 6) }),
                    new Recipe("Tiger Pelt Long Skirt", 142, false, new[] { ("Tiger Pelt", 2), ("Leather or Hides", 4) }),
                    new Recipe("Tiger Pelt Skirt", 162, false, new[] { ("Tiger Pelt", 2), ("Leather or Hides", 4) }),
                    new Recipe("Dragon Turtle Hide Bustier", 182, false, new[] { ("Leather or Hides", 6), ("Dragon Turtle Scute", 3) })
                }),
                new Category("Bone Armor", 181, new[]
                {
                    new Recipe("Bone Helmet", 2, false, new[] { ("Leather or Hides", 4), ("Bones", 2) }),
                    new Recipe("Bone Gloves", 22, false, new[] { ("Leather or Hides", 6), ("Bones", 2) }),
                    new Recipe("Bone Arms", 42, false, new[] { ("Leather or Hides", 8), ("Bones", 4) }),
                    new Recipe("Bone Leggings", 62, false, new[] { ("Leather or Hides", 10), ("Bones", 6) }),
                    new Recipe("Bone Armor", 82, false, new[] { ("Leather or Hides", 12), ("Bones", 10) }),
                    new Recipe("Orc Helm", 102, false, new[] { ("Leather or Hides", 6), ("Bones", 4) }),
                    new Recipe("Cuffs Of The Archmage", 122, false, new[] { ("Yards of Cloth", 8), ("Midnight Bracers", 1), ("Blood of the Dark Father", 5), ("Dark Sapphire", 4) })
                }),
            }),
            new Profession("Tinkering", new[]
            {
                new Category("Exit", 0, Array.Empty<Recipe>()),
                new Category("Cancel Make", 227, Array.Empty<Recipe>()),
                new Category("Repair Item", 107, Array.Empty<Recipe>()),
                new Category("Mark Item", 127, Array.Empty<Recipe>()),
                new Category("Enhance Item", 167, Array.Empty<Recipe>()),
                new Category("Non Quest Item", 207, Array.Empty<Recipe>()),
                new Category("Make Last", 47, Array.Empty<Recipe>()),
                new Category("Last Ten", 67, Array.Empty<Recipe>()),
                new Category("Jewelry", 1, new[]
            {
                new Recipe("Ring", 2, false, new[] { ("Ingots", 3) }),
                new Recipe("Bracelet", 22, false, new[] { ("Ingots", 3) }),
                new Recipe("Gargish Necklace", 42, false, new[] { ("Ingots", 3) }),
                new Recipe("Gargish Bracelet", 62, false, new[] { ("Ingots", 3) }),
                new Recipe("Gargish Ring", 82, false, new[] { ("Ingots", 3) }),
                new Recipe("Gargish Earrings", 102, false, new[] { ("Ingots", 3) }),
                new Recipe("Star Sapphire Ring", 122, false, new[] { ("Ingots", 2), ("Star Sapphires", 1) }),
                new Recipe("Star Sapphire Necklace (Silver)", 142, false, new[] { ("Ingots", 2), ("Star Sapphires", 1) }),
                new Recipe("Star Sapphire Necklace (Jewelled)", 162, false, new[] { ("Ingots", 2), ("Star Sapphires", 1) }),
                new Recipe("Star Sapphire Earrings", 182, false, new[] { ("Ingots", 2), ("Star Sapphires", 1) }),
                new Recipe("Star Sapphire Necklace (Golden)", 202, false, new[] { ("Ingots", 2), ("Star Sapphires", 1) }),
                new Recipe("Star Sapphire Bracelet", 222, false, new[] { ("Ingots", 2), ("Star Sapphires", 1) }),
                new Recipe("Emerald Ring", 242, false, new[] { ("Ingots", 2), ("Emeralds", 1) }),
                new Recipe("Emerald Necklace (Silver)", 262, false, new[] { ("Ingots", 2), ("Emeralds", 1) }),
                new Recipe("Emerald Necklace (Jewelled)", 282, false, new[] { ("Ingots", 2), ("Emeralds", 1) }),
                new Recipe("Emerald Earrings", 302, false, new[] { ("Ingots", 2), ("Emeralds", 1) }),
                new Recipe("Emerald Necklace (Golden)", 322, false, new[] { ("Ingots", 2), ("Emeralds", 1) }),
                new Recipe("Emerald Bracelet", 342, false, new[] { ("Ingots", 2), ("Emeralds", 1) }),
                new Recipe("Sapphire Ring", 362, false, new[] { ("Ingots", 2), ("Sapphires", 1) }),
                new Recipe("Sapphire Necklace (Silver)", 382, false, new[] { ("Ingots", 2), ("Sapphires", 1) }),
                new Recipe("Sapphire Necklace (Jewelled)", 402, false, new[] { ("Ingots", 2), ("Sapphires", 1) }),
                new Recipe("Sapphire Earrings", 422, false, new[] { ("Ingots", 2), ("Sapphires", 1) }),
                new Recipe("Sapphire Necklace (Golden)", 442, false, new[] { ("Ingots", 2), ("Sapphires", 1) }),
                new Recipe("Sapphire Bracelet", 462, false, new[] { ("Ingots", 2), ("Sapphires", 1) }),
                new Recipe("Ruby Ring", 482, false, new[] { ("Ingots", 2), ("Rubies", 1) }),
                new Recipe("Ruby Necklace (Silver)", 502, false, new[] { ("Ingots", 2), ("Rubies", 1) }),
                new Recipe("Ruby Necklace (Jewelled)", 522, false, new[] { ("Ingots", 2), ("Rubies", 1) }),
                new Recipe("Ruby Earrings", 542, false, new[] { ("Ingots", 2), ("Rubies", 1) }),
                new Recipe("Ruby Necklace (Golden)", 562, false, new[] { ("Ingots", 2), ("Rubies", 1) }),
                new Recipe("Ruby Bracelet", 582, false, new[] { ("Ingots", 2), ("Rubies", 1) }),
                new Recipe("Citrine Ring", 602, false, new[] { ("Ingots", 2), ("Citrines", 1) }),
                new Recipe("Citrine Necklace (Silver)", 622, false, new[] { ("Ingots", 2), ("Citrines", 1) }),
                new Recipe("Citrine Necklace (Jewelled)", 642, false, new[] { ("Ingots", 2), ("Citrines", 1) }),
                new Recipe("Citrine Earrings", 662, false, new[] { ("Ingots", 2), ("Citrines", 1) }),
                new Recipe("Citrine Necklace (Golden)", 682, false, new[] { ("Ingots", 2), ("Citrines", 1) }),
                new Recipe("Citrine Bracelet", 702, false, new[] { ("Ingots", 2), ("Citrines", 1) }),
                new Recipe("Amethyst Ring", 722, false, new[] { ("Ingots", 2), ("Amethysts", 1) }),
                new Recipe("Amethyst Necklace (Silver)", 742, false, new[] { ("Ingots", 2), ("Amethysts", 1) }),
                new Recipe("Amethyst Necklace (Jewelled)", 762, false, new[] { ("Ingots", 2), ("Amethysts", 1) }),
                new Recipe("Amethyst Earrings", 782, false, new[] { ("Ingots", 2), ("Amethysts", 1) }),
                new Recipe("Amethyst Necklace (Golden)", 802, false, new[] { ("Ingots", 2), ("Amethysts", 1) }),
                new Recipe("Amethyst Bracelet", 822, false, new[] { ("Ingots", 2), ("Amethysts", 1) }),
                new Recipe("Tourmaline Ring", 842, false, new[] { ("Ingots", 2), ("Tourmalines", 1) }),
                new Recipe("Tourmaline Necklace (Silver)", 862, false, new[] { ("Ingots", 2), ("Tourmalines", 1) }),
                new Recipe("Tourmaline Necklace (Jewelled)", 882, false, new[] { ("Ingots", 2), ("Tourmalines", 1) }),
                new Recipe("Tourmaline Earrings", 902, false, new[] { ("Ingots", 2), ("Tourmalines", 1) }),
                new Recipe("Tourmaline Necklace (Golden)", 922, false, new[] { ("Ingots", 2), ("Tourmalines", 1) }),
                new Recipe("Tourmaline Bracelet", 942, false, new[] { ("Ingots", 2), ("Tourmalines", 1) }),
                new Recipe("Amber Ring", 962, false, new[] { ("Ingots", 2), ("Amber", 1) }),
                new Recipe("Amber Necklace (Silver)", 982, false, new[] { ("Ingots", 2), ("Amber", 1) }),
                new Recipe("Amber Necklace (Jewelled)", 1002, false, new[] { ("Ingots", 2), ("Amber", 1) }),
                new Recipe("Amber Earrings", 1022, false, new[] { ("Ingots", 2), ("Amber", 1) }),
                new Recipe("Amber Necklace (Golden)", 1042, false, new[] { ("Ingots", 2), ("Amber", 1) }),
                new Recipe("Amber Bracelet", 1062, false, new[] { ("Ingots", 2), ("Amber", 1) }),
                new Recipe("Diamond Ring", 1082, false, new[] { ("Ingots", 2), ("Diamonds", 1) }),
                new Recipe("Diamond Necklace (Silver)", 1102, false, new[] { ("Ingots", 2), ("Diamonds", 1) }),
                new Recipe("Diamond Necklace (Jewelled)", 1122, false, new[] { ("Ingots", 2), ("Diamonds", 1) }),
                new Recipe("Diamond Earrings", 1142, false, new[] { ("Ingots", 2), ("Diamonds", 1) }),
                new Recipe("Diamond Necklace (Golden)", 1162, false, new[] { ("Ingots", 2), ("Diamonds", 1) }),
                new Recipe("Diamond Bracelet", 1182, false, new[] { ("Ingots", 2), ("Diamonds", 1) }),
                new Recipe("Krampus Minion Earrings", 1202, false, new[] { ("Ingots", 3) })
            }),
                new Category("Wooden Items", 21, new[]
                {
                    new Recipe("Nunchaku", 2, false, new[] { ("Ingots", 3), ("Boards or Logs", 8) }),
                    new Recipe("Jointing Plane", 22, false, new[] { ("Boards or Logs", 4) }),
                    new Recipe("Moulding Planes", 42, false, new[] { ("Boards or Logs", 4) }),
                    new Recipe("Smoothing Plane", 62, false, new[] { ("Boards or Logs", 4) }),
                    new Recipe("Clock Frame", 82, false, new[] { ("Boards or Logs", 6) }),
                    new Recipe("Axle", 102, false, new[] { ("Boards or Logs", 2) }),
                    new Recipe("Rolling Pin", 122, false, new[] { ("Boards or Logs", 5) }),
                    new Recipe("Ramrod", 142, false, new[] { ("Boards or Logs", 8) }),
                    new Recipe("Swab", 162, false, new[] { ("Cloth", 1), ("Boards or Logs", 4) }),
                    new Recipe("Softened Reeds", 182, false, new[] { ("dry reeds", 1), ("scouring toxin", 2) }),
                    new Recipe("Round Basket", 202, false, new[] { ("softened reeds", 2), ("shafts", 3) }),
                    new Recipe("Bushel", 222, false, new[] { ("softened reeds", 2), ("shafts", 3) }),
                    new Recipe("Small Bushel", 242, false, new[] { ("softened reeds", 1), ("shafts", 2) }),
                    new Recipe("Picnic Basket", 262, false, new[] { ("softened reeds", 1), ("shafts", 2) }),
                    new Recipe("Winnowing Basket", 282, false, new[] { ("softened reeds", 2), ("shafts", 3) }),
                    new Recipe("Square Basket", 302, false, new[] { ("softened reeds", 2), ("shafts", 3) }),
                    new Recipe("Basket", 322, false, new[] { ("softened reeds", 2), ("shafts", 3) }),
                    new Recipe("Tall Round Basket", 342, false, new[] { ("softened reeds", 3), ("shafts", 4) }),
                    new Recipe("Small Square Basket", 362, false, new[] { ("softened reeds", 1), ("shafts", 2) }),
                    new Recipe("Tall Basket", 382, false, new[] { ("softened reeds", 3), ("shafts", 4) }),
                    new Recipe("Small Round Basket", 402, false, new[] { ("softened reeds", 1), ("shafts", 2) }),
                    new Recipe("Enchanted Picnic Basket", 422, false, new[] { ("softened reeds", 2), ("shafts", 3) })
                }),
                new Category("Tools", 41, new[]
                {
                    new Recipe("Scissors", 2, false, new[] { ("Ingots", 2) }),
                    new Recipe("Mortar And Pestle", 22, false, new[] { ("Ingots", 3) }),
                    new Recipe("Scorp", 42, false, new[] { ("Ingots", 2) }),
                    new Recipe("Tinker'S Tools (Tool Kit)", 62, false, new[] { ("Ingots", 2) }),
                    new Recipe("Hatchet", 82, false, new[] { ("Ingots", 4) }),
                    new Recipe("Draw Knife", 102, false, new[] { ("Ingots", 2) }),
                    new Recipe("Sewing Kit", 122, false, new[] { ("Ingots", 2) }),
                    new Recipe("Saw", 142, false, new[] { ("Ingots", 4) }),
                    new Recipe("Dovetail Saw", 162, false, new[] { ("Ingots", 4) }),
                    new Recipe("Froe", 182, false, new[] { ("Ingots", 2) }),
                    new Recipe("Shovel", 202, false, new[] { ("Ingots", 4) }),
                    new Recipe("Hammer", 222, false, new[] { ("Ingots", 1) }),
                    new Recipe("Tongs", 242, false, new[] { ("Ingots", 1) }),
                    new Recipe("Smith'S Hammer", 262, false, new[] { ("Ingots", 4) }),
                    new Recipe("Sledge Hammer", 282, false, new[] { ("Ingots", 4) }),
                    new Recipe("Inshave", 302, false, new[] { ("Ingots", 2) }),
                    new Recipe("Pickaxe", 322, false, new[] { ("Ingots", 4) }),
                    new Recipe("Lockpick", 342, false, new[] { ("Ingots", 1) }),
                    new Recipe("Skillet", 362, false, new[] { ("Ingots", 4) }),
                    new Recipe("Flour Sifter", 382, false, new[] { ("Ingots", 3) }),
                    new Recipe("Fletcher'S Tools", 402, false, new[] { ("Ingots", 3) }),
                    new Recipe("Mapmaker'S Pen", 422, false, new[] { ("Ingots", 1) }),
                    new Recipe("Scribe'S Pen", 442, false, new[] { ("Ingots", 1) }),
                    new Recipe("Clippers", 462, false, new[] { ("Ingots", 4) }),
                    new Recipe("Metal Container Engraving Tool", 482, false, new[] { ("Ingots", 4), ("Springs", 1), ("Gears", 2), ("diamond", 1) }),
                    new Recipe("Pitchfork", 502, false, new[] { ("Ingots", 4) })
                }),
                new Category("Parts", 61, new[]
                {
                    new Recipe("Gears", 2, false, new[] { ("Ingots", 2) }),
                    new Recipe("Clock Parts", 22, false, new[] { ("Ingots", 1) }),
                    new Recipe("Barrel Tap", 42, false, new[] { ("Ingots", 2) }),
                    new Recipe("Springs", 62, false, new[] { ("Ingots", 2) }),
                    new Recipe("Sextant Parts", 82, false, new[] { ("Ingots", 4) }),
                    new Recipe("Barrel Hoops", 102, false, new[] { ("Ingots", 5) }),
                    new Recipe("Hinge", 122, false, new[] { ("Ingots", 2) }),
                    new Recipe("Bola Balls", 142, false, new[] { ("Ingots", 10) }),
                    new Recipe("Jeweled Filigree", 162, false, new[] { ("Ingots", 2), ("Star Sapphires", 1), ("Rubies", 1) })
                }),
                new Category("Utensils", 81, new[]
                {
                    new Recipe("Butcher Knife", 2, false, new[] { ("Ingots", 2) }),
                    new Recipe("Spoon", 22, false, new[] { ("Ingots", 1) }),
                    new Recipe("Spoon (Right)", 42, false, new[] { ("Ingots", 1) }),
                    new Recipe("Plate", 62, false, new[] { ("Ingots", 2) }),
                    new Recipe("Fork", 82, false, new[] { ("Ingots", 1) }),
                    new Recipe("Fork (Right)", 102, false, new[] { ("Ingots", 1) }),
                    new Recipe("Cleaver", 122, false, new[] { ("Ingots", 3) }),
                    new Recipe("Knife", 142, false, new[] { ("Ingots", 1) }),
                    new Recipe("Knife (Right)", 162, false, new[] { ("Ingots", 1) }),
                    new Recipe("Goblet", 182, false, new[] { ("Ingots", 2) }),
                    new Recipe("Pewter Mug", 202, false, new[] { ("Ingots", 2) }),
                    new Recipe("Skinning Knife", 222, false, new[] { ("Ingots", 2) }),
                    new Recipe("Gargish Cleaver", 242, false, new[] { ("Ingots", 3) }),
                    new Recipe("Gargish Butcher'S Knife", 262, false, new[] { ("Ingots", 2) })
                }),
                new Category("Miscellaneous", 101, new[]
            {
                new Recipe("Key Ring", 2, false, new[] { ("Ingots", 2) }),
                new Recipe("Candelabra", 22, false, new[] { ("Ingots", 4) }),
                new Recipe("Scales", 42, false, new[] { ("Ingots", 4) }),
                new Recipe("Iron Key", 62, false, new[] { ("Ingots", 3) }),
                new Recipe("Globe", 82, false, new[] { ("Ingots", 4) }),
                new Recipe("Spyglass", 102, false, new[] { ("Ingots", 4) }),
                new Recipe("Lantern", 122, false, new[] { ("Ingots", 2) }),
                new Recipe("Heating Stand", 142, false, new[] { ("Ingots", 4) }),
                new Recipe("Shoji Lantern", 162, false, new[] { ("Ingots", 10), ("Boards or Logs", 5) }),
                new Recipe("Paper Lantern", 182, false, new[] { ("Ingots", 10), ("Boards or Logs", 5) }),
                new Recipe("Round Paper Lantern", 202, false, new[] { ("Ingots", 10), ("Boards or Logs", 5) }),
                new Recipe("Wind Chimes", 222, false, new[] { ("Ingots", 15) }),
                new Recipe("Fancy Wind Chimes", 242, false, new[] { ("Ingots", 15) }),
                new Recipe("Ter-Mur Style Candelabra", 262, false, new[] { ("Ingots", 4) }),
                new Recipe("Farspeaker", 282, false, new[] { ("Ingots", 20), ("emerald", 10), ("ruby", 10), ("copper wire", 1) }),
                new Recipe("Gorgon Lens", 302, false, new[] { ("Medusa scales", 2), ("crystal dust", 3) }),
                new Recipe("A Scale Collar", 322, false, new[] { ("Medusa scales", 4), ("Scourge", 1) }),
                new Recipe("Dragon Lamp", 342, false, new[] { ("Ingots", 8), ("candelabra", 1), ("workable glass", 1) }),
                new Recipe("Stained Glass Lamp", 362, false, new[] { ("Ingots", 8), ("candelabra", 1), ("workable glass", 1) }),
                new Recipe("Tall Double Lamp", 382, false, new[] { ("Ingots", 8), ("candelabra", 1), ("workable glass", 1) }),
                new Recipe("Curled Metal Sign Hanger", 402, false, new[] { ("Ingots", 8) }),
                new Recipe("Flourished Metal Sign Hanger", 422, false, new[] { ("Ingots", 8) }),
                new Recipe("Inward Curled Metal Sign Hanger", 442, false, new[] { ("Ingots", 8) }),
                new Recipe("End Curled Metal Sign Hanger", 462, false, new[] { ("Ingots", 8) }),
                new Recipe("Left Metal Door (S In)", 482, false, new[] { ("Ingots", 50) }),
                new Recipe("Right Metal Door (S In)", 502, false, new[] { ("Ingots", 50) }),
                new Recipe("Left Metal Door (E Out)", 522, false, new[] { ("Ingots", 50) }),
                new Recipe("Right Metal Door (E Out)", 542, false, new[] { ("Ingots", 50) }),
                new Recipe("Currency Wall Safe", 562, false, new[] { ("Ingots", 20) }),
                new Recipe("Left Metal Door (E In)", 582, false, new[] { ("Ingots", 50) }),
                new Recipe("Right Metal Door (E In)", 602, false, new[] { ("Ingots", 50) }),
                new Recipe("Left Metal Door (S Out)", 622, false, new[] { ("Ingots", 50) }),
                new Recipe("Right Metal Door (S Out)", 642, false, new[] { ("Ingots", 50) }),
                new Recipe("Kotl Power Core", 662, false, new[] { ("workable glass", 5), ("copper wire", 5), ("Ingots", 100), ("Moonstone Crystal Shards", 5) }),
                new Recipe("Weathered Bronze Globe Sculpture", 682, false, new[] { ("bronze ingots", 200) }),
                new Recipe("Weathered Bronze Man On A Bench Sculpture", 702, false, new[] { ("bronze ingots", 200) }),
                new Recipe("Weathered Bronze Fairy Sculpture", 722, false, new[] { ("bronze ingots", 200) }),
                new Recipe("Weathered Bronze Archer Sculpture", 742, false, new[] { ("bronze ingots", 200) }),
                new Recipe("Barbed Whip", 762, false, new[] { ("Ingots", 5), ("Leather or Hides", 10) }),
                new Recipe("Spiked Whip", 782, false, new[] { ("Ingots", 5), ("Leather or Hides", 10) }),
                new Recipe("Bladed Whip", 802, false, new[] { ("Ingots", 5), ("Leather or Hides", 10) })
            }),
                new Category("Assemblies", 121, new[]
            {
                new Recipe("Axle With Gears", 2, false, new[] { ("Axles", 1), ("Gears", 1) }),
                new Recipe("Clock Parts", 22, false, new[] { ("Axles with Gears", 1), ("Springs", 1) }),
                new Recipe("Sextant Parts", 42, false, new[] { ("Axles with Gears", 1), ("Hinges", 1) }),
                new Recipe("Clock", 62, false, new[] { ("Clock Frames", 1), ("Clock Parts", 1) }),
                new Recipe("Clock (Left)", 82, false, new[] { ("Clock Frames", 1), ("Clock Parts", 1) }),
                new Recipe("Sextant", 102, false, new[] { ("Sextant Parts", 1) }),
                new Recipe("Bola", 122, false, new[] { ("Bola Balls", 4), ("Leather or Hides", 3) }),
                new Recipe("Potion Keg", 142, false, new[] { ("Empty Kegs", 1), ("Empty Bottles", 10), ("Barrel Lids", 1), ("Keg Taps", 1) }),
                new Recipe("Leather Wolf Assembly", 162, false, new[] { ("Clockwork Assembly", 1), ("power crystal", 1), ("Void Essence", 1) }),
                new Recipe("Clockwork Scorpion Assembly", 182, false, new[] { ("Clockwork Assembly", 1), ("power crystal", 1), ("Void Essence", 2) }),
                new Recipe("Vollem Assembly", 202, false, new[] { ("Clockwork Assembly", 1), ("power crystal", 1), ("Void Essence", 3) }),
                new Recipe("Hitching Rope", 222, false, new[] { ("rope", 1), ("Resolve's Bridle", 1) }),
                new Recipe("Hitching Post (Replica)", 242, false, new[] { ("Ingots", 50), ("animal pheromone", 1), ("hitching rope", 2), ("Phillip's Wooden Steed", 1) }),
                new Recipe("Arcanic Rune Stone", 262, false, new[] { ("Crystal Shards", 1), ("power crystal", 5) }),
                new Recipe("Void Orb", 282, false, new[] { ("Dark Sapphire", 1), ("Black Pearl", 50) }),
                new Recipe("Advanced Training Dummy (South)", 302, false, new[] { ("training dummy (south)", 1), ("platemail tunic", 1), ("close helmet", 1), ("broadsword", 1) }),
                new Recipe("Advanced Training Dummy (East)", 322, false, new[] { ("training dummy (east)", 1), ("platemail tunic", 1), ("close helmet", 1), ("broadsword", 1) }),
                new Recipe("Distillery (South)", 342, false, new[] { ("metal keg", 2), ("heating stand", 4), ("copper wire", 1) }),
                new Recipe("Distillery (East)", 362, false, new[] { ("metal keg", 2), ("heating stand", 4), ("copper wire", 1) }),
                new Recipe("Kotl Automaton", 382, false, new[] { ("Ingots", 300), ("Automaton Actuator", 1), ("Stasis Chamber Power Core", 1), ("Inoperative Automaton Head", 1) }),
                new Recipe("Telescope", 402, false, new[] { ("Ingots", 25), ("workable glass", 1), ("Sextant Parts", 1) }),
                new Recipe("Oracle Of The Sea", 422, false, new[] { ("Ingots", 3), ("workable glass", 2), ("ocean sapphire", 3) })
            }),
                new Category("Traps", 141, new[]
                {
                    new Recipe("Dart Trap", 2, false, new[] { ("Ingots", 1), ("Crossbow Bolts", 1) }),
                    new Recipe("Poison Trap", 22, false, new[] { ("Ingots", 1), ("Green Potions", 1) }),
                    new Recipe("Explosion Trap", 42, false, new[] { ("Ingots", 1), ("Purple Potions", 1) })
                }),
                new Category("Magic Jewelry", 161, new[]
                {
                    new Recipe("Brilliant Amber Bracelet", 2, false, new[] { ("Ingots", 5), ("amber", 20), ("Brilliant Amber", 10) }),
                    new Recipe("Fire Ruby Bracelet", 22, false, new[] { ("Ingots", 5), ("ruby", 20), ("Fire Ruby", 10) }),
                    new Recipe("Dark Sapphire Bracelet", 42, false, new[] { ("Ingots", 5), ("sapphire", 20), ("Dark Sapphire", 10) }),
                    new Recipe("White Pearl Bracelet", 62, false, new[] { ("Ingots", 5), ("tourmaline", 20), ("White Pearl", 10) }),
                    new Recipe("Ecru Citrine Ring", 82, false, new[] { ("Ingots", 5), ("citrine", 20), ("Ecru Citrine", 10) }),
                    new Recipe("Blue Diamond Ring", 102, false, new[] { ("Ingots", 5), ("diamond", 20), ("Blue Diamond", 10) }),
                    new Recipe("Perfect Emerald Ring", 122, false, new[] { ("Ingots", 5), ("emerald", 20), ("Perfect Emerald", 10) }),
                    new Recipe("Turquoise Ring", 142, false, new[] { ("Ingots", 5), ("amethyst", 20), ("Turquoise", 10) }),
                    new Recipe("Resilient Bracer", 162, false, new[] { ("Ingots", 2), ("Captured Essence", 1), ("Blue Diamond", 10), ("diamond", 50) }),
                    new Recipe("Essence Of Battle", 182, false, new[] { ("Ingots", 2), ("Captured Essence", 1), ("Fire Ruby", 10), ("ruby", 50) }),
                    new Recipe("Pendant Of The Magi", 202, false, new[] { ("Ingots", 2), ("Eye of the Travesty", 1), ("White Pearl", 5), ("star-saphire", 50) }),
                    new Recipe("Dr. Spector'S Lenses", 222, false, new[] { ("Ingots", 20), ("Black Moonstone", 1), ("Hat of the Magi", 1) }),
                    new Recipe("Bracelet Of Primal Consumption", 242, false, new[] { ("Ingots", 3), ("Ring of the Elements", 1), ("Blood of the Dark Father", 5), ("White Pearl", 4) })
                }),
            }) 
};
        
        public class ProfessionDetails
        {
            public Dictionary<string, CategoryDetails> Categories { get; set; } = new();
        }

        public class CategoryDetails
        {
            public Dictionary<string, RecipeDetails> Recipes { get; set; } = new();
        }

        public class RecipeDetails
        {
            public int ItemID { get; set; }
            public int Gump { get; set; }
            public string Material { get; set; }
            public Dictionary<string, int> Resources { get; set; } = new();
        }
        public class ResourceDetails
        {
            public int Btn { get; init; }
            public int[] Id { get; init; }
            public int Color { get; init; }
        }

        public static Dictionary<string, SkillDetails> GetSkillInfo() => BodSkillInfo;
        public class SkillDetails
        {
            public uint Gump { get; init; }  // Replace object with your actual gump type
            public int BodColor { get; init; }
            public Dictionary<string, int> Tools { get; init; }
            public string ResourceType { get; init; }  // Add this property
            public Dictionary<string, ResourceDetails> Resources { get; init; }
        }
                
        public static readonly Dictionary<string, Dictionary<string, ResourceDetails>> ResourceInfo = new()
        {
            ["Ingots"] = new Dictionary<string, ResourceDetails>
            {
                ["Iron"] = new ResourceDetails { Btn = 6, Id = new[] { 0x1BF2 }, Color = 0x0000 },
                ["Dull Copper"] = new ResourceDetails { Btn = 26, Id = new[] { 0x1BF2 }, Color = 0x0973 },
                ["Shadow Iron"] = new ResourceDetails { Btn = 46, Id = new[] { 0x1BF2 }, Color = 0x0966 },
                ["Copper"] = new ResourceDetails { Btn = 66, Id = new[] { 0x1BF2 }, Color = 0x096D },
                ["Bronze"] = new ResourceDetails { Btn = 86, Id = new[] { 0x1BF2 }, Color = 0x0972 },
                ["Gold"] = new ResourceDetails { Btn = 106, Id = new[] { 0x1BF2 }, Color = 0x08A5 },
                ["Agapite"] = new ResourceDetails { Btn = 126, Id = new[] { 0x1BF2 }, Color = 0x0979 },
                ["Verite"] = new ResourceDetails { Btn = 146, Id = new[] { 0x1BF2 }, Color = 0x089F },
                ["Valorite"] = new ResourceDetails { Btn = 166, Id = new[] { 0x1BF2 }, Color = 0x08AB }
            },
            ["Boards or Logs"] = new Dictionary<string, ResourceDetails>
            {
                ["Wood"] = new ResourceDetails { Btn = 6, Id = new[] { 0x1BD7, 0x1BDD }, Color = 0x0000 },
                ["Oak"] = new ResourceDetails { Btn = 26, Id = new[] { 0x1BD7, 0x1BDD }, Color = 0x07DA },
                ["Ash"] = new ResourceDetails { Btn = 46, Id = new[] { 0x1BD7, 0x1BDD }, Color = 0x04A7 },
                ["Yew"] = new ResourceDetails { Btn = 66, Id = new[] { 0x1BD7, 0x1BDD }, Color = 0x04A8 },
                ["Heartwood"] = new ResourceDetails { Btn = 86, Id = new[] { 0x1BD7, 0x1BDD }, Color = 0x04A9 },
                ["Bloodwood"] = new ResourceDetails { Btn = 106, Id = new[] { 0x1BD7, 0x1BDD }, Color = 0x04AA },
                ["Frostwood"] = new ResourceDetails { Btn = 126, Id = new[] { 0x1BD7, 0x1BDD }, Color = 0x047F }
            },
            ["Leather or Hides"] = new Dictionary<string, ResourceDetails>
            {
                ["Leather/Hides"] = new ResourceDetails { Btn = 6, Id = new[] { 0x1081, 0x1079 }, Color = 0x0000 },
                ["Spined Hides"] = new ResourceDetails { Btn = 26, Id = new[] { 0x1081, 0x1079 }, Color = 0x08AC },
                ["Horned Hides"] = new ResourceDetails { Btn = 46, Id = new[] { 0x1081, 0x1079 }, Color = 0x0845 },
                ["Barbed Hides"] = new ResourceDetails { Btn = 66, Id = new[] { 0x1081, 0x1079 }, Color = 0x0851 }
            }
        };

        public static readonly Dictionary<string, SkillDetails> BodSkillInfo = new()
        {
            ["Alchemy"] = new SkillDetails
            {
                Gump = 0, // replace with actual alchemy_gump
                //Materials = MaterialDetails,
                //MaterialType = "", 
                BodColor = 0x09C9,
                Tools = new Dictionary<string, int>
                {
                    ["mortar and pestle"] = 0x0E9B
                }
            },
            ["Blacksmithing"] = new SkillDetails
            {
                Gump = 0, // replace with actual blacksmithing_gump
                Resources = ResourceInfo["Ingots"],
                ResourceType = "Ingots",
                BodColor = 0x044E,
                Tools = new Dictionary<string, int>
                {
                    ["smith's hammer"] = 0x13E3,
                    ["sledge hammer"] = 0x0FB5,
                    ["tongs"] = 0x0FBB
                }
            },
            ["Bowcraft and Fletching"] = new SkillDetails
            {
                Gump = 0, // replace with actual bowcraft_gump
                Resources = ResourceInfo["Boards or Logs"],
                ResourceType = "Wood",
                BodColor = 0x0591,
                Tools = new Dictionary<string, int>
                {
                    ["fletcher's tools"] = 0x1022
                }
            },
            ["Carpentry"] = new SkillDetails
            {
                Gump = 0, // replace with actual carpentry_gump
                Resources = ResourceInfo["Boards or Logs"],
                ResourceType = "Wood", 
                BodColor = 0x05E8,
                Tools = new Dictionary<string, int>
                {
                    ["saw"] = 0x1034,
                    ["draw_knife"] = 0x10E4,
                    ["froe"] = 0x10E5,
                    ["inshave"] = 0x10E6,
                    ["scorp"] = 0x10E7,
                    ["dovetail_saw"] = 0x1028,
                    ["hammer"] = 0x102A
                }
            },
            ["Cartography"] = new SkillDetails
            {
                Gump = 0, // replace with actual cartography_gump
                BodColor = 0x0000,
                //Materials = MaterialInfo["Boards or Logs"],
                ResourceType = "Wood",
                Tools = new Dictionary<string, int>
                {
                    ["mapmaker's pen"] = 0x0FBF
                }
            },
            ["Cooking"] = new SkillDetails
            {
                Gump = 0, // replace with actual cooking_gump
                BodColor = 0x0491,
                Tools = new Dictionary<string, int>
                {
                    ["skillet"] = 0x097F,
                    ["flour sifter"] = 0x103E,
                    ["rolling pin"] = 0x1043
                }
            },
            ["Inscription"] = new SkillDetails
            {
                Gump = 0, // replace with actual inscription_gump
                BodColor = 0x0A26,
                Tools = new Dictionary<string, int>
                {
                    ["scribe's pen"] = 0x0FBF
                }
            },
            ["Tailoring"] = new SkillDetails
            {
                Gump = 0, // replace with actual tailoring_gump
                Resources = ResourceInfo["Leather or Hides"],
                ResourceType = "Leather or Hides",
                BodColor = 0x0483,
                Tools = new Dictionary<string, int>
                {
                    ["sewing kit"] = 0x0F9D
                }
            },
            ["Tinkering"] = new SkillDetails
            {
                Gump = 0, // replace with actual tinkering_gump
                Resources = ResourceInfo["Ingots"],
                ResourceType = "Ingots",
                BodColor = 0x0455,
                Tools = new Dictionary<string, int>
                {
                    ["tinker's tools"] = 0x1EBC,
                    ["tool kit"] = 0x1EB8
                }
            }
        };
      
        
    }
}
/*   
            # developing this. still with no use.
def get_resource_from_gump(item_id, color):
    storage_refs = {(0x1BF2, 0x0000): ('IronIngot', 100), (0x1BF2, 0x0973): ('DullCopperIngot', 101),
                    (0x1BF2, 0x0966): ('ShadowIronIngot', 102), (0x1BF2, 0x096D): ('CopperIngot', 103),
                    (0x1BF2, 0x0972): ('BronzeIngot', 104), (0x1BF2, 0x08A5): ('GoldIngot', 105),
                    (0x1BF2, 0x0979): ('AgapiteIngot', 106), (0x1BF2, 0x089F): ('VeriteIngot', 107),
                    (0x1BF2, 0x08AB): ('ValoriteIngot', 108),
                    (0x1BD7, 0x0000): ('Board', 107), (0x1BD7, 0x07DA): ('OakBoard', 108),
                    (0x1BD7, 0x04A7): ('AshBoard', 109), (0x1BD7, 0x04A8): ('YewBoard', 110),
                    (0x1BD7, 0x04A9): ('HeartwoodBoard', 111), (0x1BD7, 0x04AA): ('BloodwoodBoard', 112),
                    (0x1BD7, 0x047F): ('FrostwoodBoard', 113),
                    (0x1BDD, 0x0000): ('Log', 100), (0x1BDD, 0x07DA): ('OakLog', 101),
                    (0x1BDD, 0x04A7): ('AshLog', 102), (0x1BDD, 0x04A8): ('YewLog', 103),
                    (0x1BDD, 0x04A9): ('HeartwoodLog', 104), (0x1BDD, 0x04AA): ('BloodwoodLog', 105),
                    (0x1BDD, 0x047F): ('FrostwoodLog', 106),
                    (0x1081, 0x0000): ('Leather', 100), (0x1081, 0x08AC): ('SpinedLeather', 101),
                    (0x1081, 0x0845): ('HornedLeather', 102), (0x1081, 0x0851): ('BarbedLeather', 103),
                    ("Cut Cloth", 0x0000): ('Cloth', 110), ("Bolt Of Cloth", 0x0000): ('BoltOfCloth', 111)}
            */

/*
// Examples of usage:
var alchemyProfession = CraftingDatabase.GetProfession("Alchemy");
var healingCategory = CraftingDatabase.GetCategory("Alchemy", "Healing And Curative");
var refreshRecipe = CraftingDatabase.GetRecipe("Alchemy", "Healing And Curative", "Refresh");

// Get all profession names
var professions = CraftingDatabase.GetProfessionNames();

// Get all categories for Alchemy
var alchemyCategories = CraftingDatabase.GetCategoryNames("Alchemy");

// Get all recipes in Healing category
var healingRecipes = CraftingDatabase.GetRecipeNames("Alchemy", "Healing And Curative");
*/
