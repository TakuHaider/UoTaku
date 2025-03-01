using System.Collections.Generic;

namespace RazorEnhanced
{
    public static class UoTools
    {
        // missing aud char, and the Fire Horn.
        public static List<int> Instruments = new()
        {
            0x0E9C, //drums
            0x2805, //flute
            0x0EB3, //lute
            0x0EB2, //lap harp
            0x0EB1, //standing harp
            0x0E9E, //tambourine
            0x0E9D //tambourine with tassles
        };
        
        public static List<int> Pickaxes = new()
        {
	        // Mining
	        0x0E85, // pickaxe (left)
	        0x0E86, // pickaxe (right)
	        0x0FB4 // prospector's tool
	        
        };
        
        //Not sure if these all work
        //0x0F49, 0x1443
        public static List<int> Axes = new()
        {
            0x0F49,
            0x13FB,
            0x0F47,
            0x1443,
            0x0F45,
            0x0F4B,
            0x0F43,
            0x48B2 //Gargoyle's Axe
        };

        public static List<(int, int)> Smithing = new()
        {
            // Blacksmithing
            (0x0FB5, 0x0000), // sledge hammer
            (0x13E3, 0x0000), // smith's hammer
            (0x0FBB, 0x0000), // tongs
        };

        public static List<(int Id1, int Id2)> RunicHammer = new()
        {
            // Runic Hammers
            (0x13E3, 0x097E), // agapite runic hammer
            (0x13E3, 0x06D8), // bronze runic hammer
            (0x13E3, 0x045F), // copper runic hammer
            (0x13E3, 0x0415), // dull copper runic hammer
            (0x13E3, 0x06B7), // golden runic hammer
            (0x13E3, 0x0455), // shadow iron runic hammer
            (0x13E3, 0x07D2), // verite runic hammer
            (0x13E3, 0x0544), // valorite runic hammer
        };

        public static List<(int Id1, int Id2)> Carpentry = new()
        {
            // Carpentry
            (0x1028, 0x0000), // dovetail saw
            (0x10E4, 0x0000), // draw knife
            (0x10E5, 0x0000), // froe
            (0x102A, 0x0000), // hammer
            (0x10E6, 0x0000), // inshave
            (0x1030, 0x0000), // jointing plane
            (0x102C, 0x0000), // moulding planes
            (0x102E, 0x0000), // nails
            (0x1034, 0x0000), // saw
            (0x10E7, 0x0000), // scorp
            (0x1032, 0x0000), // smoothing plane
        };

        public static List<(int Id1, int Id2)> Mining = new()
        {
            // Mining
            (0x0E86, 0x0430), // gargoyle's pickaxe
            (0x0E85, 0x0000), // pickaxe (left)
            (0x0E86, 0x0000), // pickaxe (right)
            (0x0FB4, 0x0000), // prospector's tool
            (0x0E86, 0x0973), // sturdy pickaxe
        };

        public static List<(int Id1, int Id2)> Tinkering = new()
        {
            // Tinkering
            (0x1EBC, 0x0000), // tinker's tools
            (0x1EB8, 0x0000), // tool kit
        };

        public static List<(int Id1, int Id2)> Tailoring = new()
        {
            // Tailoring
            (0x0F9D, 0x0000), // sewing kit
            (0x0EC3, 0x0000), // cleaver
            (0x0F52, 0x0000), // dagger
            (0x0E05, 0x0000), // disguise kit
            (0x0FA9, 0x0000), // dyes
            (0x0FAB, 0x0000), // dyeing tub
            (0x0DBF, 0x0000), // fishing pole
            (0x0FC1, 0x0000), // interior decorator
            (0x14FC, 0x0000), // lockpick
            (0x0FBF, 0x0000), // mapmaker's pen
            (0x0F9F, 0x0000), // scissors
            (0x0EC4, 0x0000), // skinning knife
        };
    }
}