using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;


/*
WorldMapVeinData GetPositionInfo(int x, int y, int facet)
{
	int landID1 = Statics.GetLandID(x, y, facet);
	int landID2 = Statics.GetLandID(x, y - 1, facet);
	int landID3 = Statics.GetLandID(x + 1, y - 1, facet);
	int landID4 = Statics.GetLandID(x + 1, y, facet);
	if (landID1 == 240
	    && landID2 == 241
	    && landID3 == 242
	    && landID4 == 243)
	{
		int z = Statics.GetLandZ(x, y, facet);
		if (z == 0)
		{
			WorldMapVeinData worldMapVeinDataObject = new WorldMapVeinData();
			worldMapVeinDataObject.posX = x;
			worldMapVeinDataObject.posY = y;
			worldMapVeinDataObject.facet = facet;
			worldMapVeinDataObject.description = "";
			worldMapVeinDataObject.cursor = "";
			worldMapVeinDataObject.colorMarker = "white";
			worldMapVeinDataObject.zoomlevel = 9;

			return worldMapVeinDataObject;
		}
	}
	return null;
}*/
namespace RazorEnhanced
{
	public class UoTiles
	{
		
		// Thread-safe data structure for storing TileInfo with their coordinates
		public static ConcurrentDictionary<(int X, int Y), List<Statics.TileInfo>> CachedTiles = new();
		
		public struct MapType : IEquatable<MapType>
		{
			private readonly int _value;
    
			private MapType(int value) => _value = value;
    
			public static implicit operator int(MapType map) => map._value;
			public static implicit operator MapType(int value) => new MapType(value);
    
			// Constants instead of enum values
			public static MapType Felucca => 0;
			public static MapType Trammel => 1;
			public static MapType Ilshenar => 2;
			public static MapType Malas => 3;
			public static MapType Tokuno => 4;
			public static MapType TerMur => 5;
			public static MapType Eodon => 6;

			// Add these methods for proper dictionary key support
			public override bool Equals(object obj) => 
				obj is MapType other && Equals(other);

			public bool Equals(MapType other) => 
				_value == other._value;

			public override int GetHashCode() => 
				_value.GetHashCode();
			
			/*  // ToName() usage example
				UoTiles.MapType map = Player.Map;
				string result = map.ToName();
			*/
			public string ToName()
			{
				int currentValue = this;  // Uses the implicit conversion to int
				var propertyInfo = typeof(MapType)
					.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
					.FirstOrDefault(p => 
					{
						var value = p.GetValue(null);
						return value is MapType mapType && (int)mapType == currentValue;
					});
				return propertyInfo?.Name ?? $"Unknown({currentValue})";
			}
		}
		private static readonly int[] _serials = Enumerable.Range(800000, 2000).ToArray();
		private static int _serialIndex = 0;
		private void CreateFakeObjectsAtOreNodes(int range = 16)
		{
		    // Reset serial index
		    _serialIndex = 0;

		    foreach ((int X, int Y, int Z) node in OreNodeLists[Player.Map])
		    {
			    if (Misc.Distance(Player.Position.X, Player.Position.Y, node.X, node.Y) > range) continue;
		        int serial = _serials[_serialIndex++ % _serials.Length];
		        
		        // Using similar parameters to the Python example
		        // 0x1766 is the item ID used in the example for walkable tiles
		        // 1152 is a hue value used in the example for path marking
		        // Adding +1 to Z position to make it visible above ground
		        var itemId = 0x1766;  // You can change this to any item ID you prefer
		        var hue = 1152;       // You can adjust the color/hue as needed
		        var z = node.Z + 1;   // Lifting it slightly above ground level
		        PlaceFakeObject(itemId, hue, node.X, node.Y, z, serial);
		    }
		}
		private void PlaceFakeObject(int itemId, int hue, int x, int y, int z, int serial)
		{
		    // Convert parameters to proper format
		    string itemIdHex = $"{itemId:X4}";  // Convert to 4-digit hex
		    string itemIdFormatted = $"{itemIdHex.Substring(0, 2)} {itemIdHex.Substring(2, 2)}";
		    string hueHex = $"{hue:X4}";
		    string hueFormatted = $"{hueHex.Substring(0, 2)} {hueHex.Substring(2, 2)}";
		    // Call the existing UOT method to create the item
		    CreateItemAtLocation(x, y, z, itemIdFormatted, hueFormatted, serial);
		}
		private void CreateItemAtLocation(int x, int y, int z, string itemId, string hue, int serial)
		{
		    string serialHex = serial.ToString("X6");
		    // Base packet format from the Python example
		    string basePacket = "F3 00 01 00 48 FC BB 12 00 00 00 00 01 00 01 05 88 06 88 0A 00 04 15 20 00 00";
		    var packetParts = basePacket.Split(' ').ToList();
		    // Update packet with our values
		    packetParts[5] = serialHex.Substring(0, 2);
		    packetParts[6] = serialHex.Substring(2, 4);
		    packetParts[7] = serialHex.Substring(4, 2);
		    // Add item ID
		    var itemIdParts = itemId.Split(' ');
		    packetParts[8] = itemIdParts[0];
		    packetParts[9] = itemIdParts[1];
		    // Add coordinates
		    packetParts[15] = $"{x:X4}".Substring(0, 2);
		    packetParts[16] = $"{x:X4}".Substring(2, 2);
		    packetParts[17] = $"{y:X4}".Substring(0, 2);
		    packetParts[18] = $"{y:X4}".Substring(2, 2);
		    packetParts[19] = ConvertToUnsignedHex(z - 1, 1);
		    // Add hue
		    var hueParts = hue.Split(' ');
		    packetParts[21] = hueParts[0];
		    packetParts[22] = hueParts[1];
		    // Convert and send packet
		    byte[] packet = packetParts.Select(p => Convert.ToByte(p, 16)).ToArray();
		    PacketLogger.SendToClient(packet);
		}

		private string ConvertToUnsignedHex(int value, int byteSize)
		{
		    if (value < 0)
		        value += 1 << (byteSize * 8);
		    return value.ToString("X").PadLeft(byteSize * 2, '0');
		}
		// Method to remove fake objects when needed
		public void RemoveFakeObjects()
		{
		    foreach (var serial in _serials)
		    {
		        // Implement hide/remove logic based on your UOT framework
		        Items.Hide(serial);
		    }
		}
		/// <summary>
		/// Tile: Return a list of TileInfo representing the Tile items for a given X,Y, map.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <param name="map">
		///     0 = Felucca
		///     1 = Trammel
		///     2 = Ilshenar
		///     3 = Malas
		///     4 = Tokuno
		///     5 = TerMur
		///		6 = Eodon
		/// </param>
		/// <returns>A list of TileInfo related to Tile items.</returns>
		public static List<Statics.TileInfo> GetTileInfo(int x, int y, int map)
		{
			// Check if the tile information already exists in the _tiles dictionary
			if (CachedTiles.TryGetValue((x, y), out var cachedTileInfo) && cachedTileInfo != null && cachedTileInfo.Any())
			{
				//UoTLogger.LogErrorToFile("Tile Info Found in Cache:");
				return cachedTileInfo; 
			}
			
			// Fetch tile information from Statics.GetStaticsTileInfo
			var refList = Statics.GetStaticsTileInfo(x, y, map);
			
			// If Statics.GetStaticsTileInfo is valid, save it to _tiles
			if (refList != null && refList.Any())
			{
				// Create a new list to store the new TileInfo objects
				var cacheList = new List<Statics.TileInfo>();
				//UoTLogger.LogErrorToFile("Saving Tile Info to Cache:");
				for (var i = 0; i < refList.Count; i++)
				{ 
					var tileInfo = refList[i]; // Access the current TileInfo object

					// Create a new instance of Statics.TileInfo using its constructor
					var newTile = new Statics.TileInfo(
						tileInfo.ID,    // Copy ID from TileInfo
						tileInfo.Hue,   // Copy Hue from TileInfo
						tileInfo.Z      // Copy Z from TileInfo
					);
					// Add the new Statics.TileInfo to the destination list
					cacheList.Add(newTile);
				}
				
				//UoTLogger.LogErrorToFile("Adding to cache:");
				// Cache and return the converted copies
				CachedTiles.AddOrUpdate((x, y), cacheList, (_, _) => cacheList);

				return cacheList;

			}
			//TileDataFailed.AddOrUpdate((x, y), DateTime.Now, (_, _) => DateTime.Now.AddMinutes(2));
			return null;
		}

		[Flags]
       public enum LandFlags
       {
	       None = 0,             // No flag
	       Translucent = 1,      // 2^0
	       Wall = 2,             // 2^1
	       Damaging = 4,         // 2^2
	       Impassable = 8,       // 2^3
	       Surface = 16,         // 2^4
	       Bridge = 32,          // 2^5
	       Window = 64,          // 2^6
	       NoShoot = 128,        // 2^7
	       Foliage = 256,        // 2^8
	       HoverOver = 512,      // 2^9
	       Roof = 1024,          // 2^10
	       Door = 2048,          // 2^11
	       Wet = 4096            // 2^12
       }

       [Flags]
       public enum TileFlags
       {
	       None = 0,             // No flag
	       Translucent = 1,      // 2^0
	       Wall = 2,             // 2^1
	       Damaging = 4,         // 2^2
	       Impassable = 8,       // 2^3
	       Surface = 16,         // 2^4
	       Bridge = 32,          // 2^5
	       Window = 64,          // 2^6
	       NoShoot = 128,        // 2^7
	       Foliage = 256,        // 2^8
	       HoverOver = 512,      // 2^9
	       Roof = 1024,          // 2^10
	       Door = 2048,          // 2^11
	       Wet = 4096            // 2^12
       }
       
       public static void LogTileInfo(List<Statics.TileInfo> tileInfoList, UoTiles.UoTileInfo path = null)
       {
	       if (path != null)
	       {
		       UoTLogger.LogErrorToFile($"[Tile Info]  Valid: {path.IsValid} GetPath: {path.GetPathFailed} - {path.TargetX}, {path.TargetY}, ");
	       }
	       
	       if (tileInfoList == null || tileInfoList.Count == 0)
	       {
		       //UoTLogger.LogErrorToFile("No tile info provided.");
		       return;
	       }
	       
	       foreach (var tile in tileInfoList)
	       {
		       UoTLogger.LogErrorToFile(
			       $"[Tile Info] Tile:{tile.ToString()} ID: {tile.ID} Static ID: {tile.StaticID} ");
		       UoTLogger.LogErrorToFile(
			       $"[Tile Info] Coords at Z: {tile.Z} and Static Z: {tile.StaticZ}");
		       UoTLogger.LogErrorToFile(
			       $"[Tile Info] Hue: {tile.Hue} and Static Hue: {tile.StaticHue}");

		       /*// Land Flags
		       UoTLogger.LogErrorToFile("Land Flags:");
		       foreach (var flag in Enum.GetValues(typeof(LandFlags)))
		       {
			       if (Statics.GetLandFlag(tile.StaticID, flag.ToString()))
			       {
				       UoTLogger.LogErrorToFile($"  - {flag}");
			       }
		       }

		       // Tile Flags
		       UoTLogger.LogErrorToFile("Tile Flags:");
		       foreach (var flag in Enum.GetValues(typeof(TileFlags)))
		       {
			       if (Statics.GetTileFlag(tile.StaticID, flag.ToString()))
			       {
				       UoTLogger.LogErrorToFile($"  - {flag}");
			       }
		       }*/

		       UoTLogger.LogErrorToFile("----------");
	       }
       }

       public static bool IsCoordIncluded(int x, int y, HashSet<(int,int)> hash)
       {
	       return hash.Contains((x, y)); // Fast O(1) lookup
       }


       /// <summary>
       /// Tile: Wrapper for game tile info
       /// </summary>
       /// <param name="IsValid">If tile data is valid</param>
       /// <param name="TargetX">Tile x coordinate</param>
       /// <param name="TargetY">Tile y coordinate</param>
       /// <param name="TileInfo">List of TileInfo for tile</param>
       /// 
       public class UoTileInfo
       {
	       public bool IsValid { get; set; }
	       public bool GetPathFailed { get; set; }
	       public int TargetX { get; set; }
	       public int TargetY { get; set; }
	       //public int TargetZ { get; set; }
	       public List<Statics.TileInfo> TileInfo { get; set; }
       }
       public static bool IsImpassable((int ,int ) coords, List<Statics.TileInfo> tileInfo = null)
       {
	       return tileInfo != null &&
	              tileInfo.Exists(tile =>
		              Statics.GetTileFlag(tile.StaticID, "Impassable") ||
		              Statics.GetLandFlag(tile.StaticID, "Impassable"));
       }
       public static bool IsMovementPossible((int ,int ) coords, List<Statics.TileInfo> tileInfo = null)
       {
	       if (tileInfo == null) return true;
	       if (!tileInfo.Any()) return true;
	       //add check for mobiles and maybe some item type, such as 0xA854 at z = 0 on Invalid layer.
	       //UoTLogger.LogErrorToFile("IsMovementPossible: tileInfo count " + tileInfo.Count + "");
	       //if (Player.Mount == null && Statics.GetTileHeight(tileInfo[0].StaticZ) > Player.Position.Z + 1) return false;
	       if (Statics.GetTileFlag(tileInfo[0].StaticID, "Impassable") ||
	           Statics.GetLandFlag(tileInfo[0].StaticID, "Impassable")) return false;
	       return true;
       }

       [JsonConverter(typeof(UoTJson.MiningDatabaseConverter))]
       public class MiningDatabase 
       {
	       // Dictionary of maps, each containing locations
	       public Dictionary<int, Map> Maps { get; set; } = new ();

       }

       public class Map
       {
	       // Dictionary where each key is a tuple for a location, and the value is the `LastMined` DateTime
	       public Dictionary<(int X, int Y, int Z), DateTime> Locations { get; set; } = new();

       }
       private static List<HashSet<(int, int, int)>> _oreNodeLists;  // backing field
       public static List<HashSet<(int, int, int)>> OreNodeLists
       {
	       get
	       {
		       if (_oreNodeLists == null)  // check the backing field
		       {
			       var mapCount = typeof(MapType)
				       .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
				       .Count();
			       _oreNodeLists = new List<HashSet<(int, int, int)>>();  // set the backing field
			       for (int i = 0; i < mapCount; i++) 
				       _oreNodeLists.Add(new HashSet<(int, int, int)>());
			       for (var i = 0; i < mapCount; i++)
			       {
				       UoTiles.MapType map = i;
				       using var storedData = new UoTJson("Ore_"+ Misc.ShardName() + "-" + map.ToName());
				       //if (UoTiles.OreNodeLists[map] != null) storedData.StoreData(UoTiles.OreNodeLists[map],  Misc.ShardName() + "-" + map.ToName(), StoredData.StoreType.Global);
				       var loadedData = storedData.GetData<HashSet<(int,int, int)>>(
					       "Ore_"+ Misc.ShardName() + "-" + map.ToName(), typeof(HashSet<(int,int, int)>), UoTJson.StoreType.Server);
				       if (loadedData != null) UoTiles.OreNodeLists[map] = loadedData;
				       else storedData.StoreData(new HashSet<(int, int, int)>(), "Ore_"+ Misc.ShardName() + "-" + map.ToName(), UoTJson.StoreType.Server);
			       }

		       }
		       return _oreNodeLists;  // return the backing field
	       }
       }

	}
}