//C#
// Stored Data Library - CaporaleSimone 2021-2024
// This library allows you to store/read data in a JSON file based on the storage type (Global, Server, or Character).
//
// To edit this script without VS errors add referene to Newtonsoft.Json.dll from RE folder
// Edited by Taku to include ClearData method

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace RazorEnhanced
{
	public partial class UoTJson : IDisposable
	{
		
		public class HashSetConverter : JsonConverter<HashSet<(int, int, int)>>
		{
			public override void WriteJson(JsonWriter writer, HashSet<(int, int, int)> value, JsonSerializer serializer)
			{
				// Temporarily change formatting to None
				var originalFormatting = writer.Formatting;
				writer.Formatting = Formatting.None;
				
				writer.WriteStartArray();
				foreach (var (x, y, z) in value)
				{
					writer.WriteStartArray();
					writer.WriteValue(x);
					writer.WriteValue(y);
					writer.WriteValue(z);
					writer.WriteEndArray();
				}
				writer.WriteEndArray();
				
				// Restore original formatting
				writer.Formatting = originalFormatting;

			}

			public override HashSet<(int, int, int)> ReadJson(JsonReader reader, Type objectType, HashSet<(int, int, int)> existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var result = new HashSet<(int, int, int)>();
				if (reader.TokenType == JsonToken.StartArray)
				{
					while (reader.Read())
					{
						if (reader.TokenType == JsonToken.EndArray) break;
						var arr = serializer.Deserialize<int[]>(reader);
						result.Add((arr[0], arr[1], arr[2]));
					}
				}
				return result;
			}
		}

		public class MiningDatabaseConverter : JsonConverter<UoTiles.MiningDatabase>
		{

		    public override UoTiles.MiningDatabase ReadJson(JsonReader reader, Type objectType, UoTiles.MiningDatabase existingValue, bool hasExistingValue, JsonSerializer serializer)
		    {
		        try
		        {
		            var jObject = JObject.Load(reader);
		            var database = new UoTiles.MiningDatabase();

		            if (jObject["Maps"] != null)
		            {
		                database.Maps = new Dictionary<int, UoTiles.Map>();
		                var mapsObj = jObject["Maps"];

		                foreach (var prop in mapsObj.Children<JProperty>())
		                {
		                    if (int.TryParse(prop.Name, out int mapId))
		                    {
		                        var map = new UoTiles.Map
		                        {
		                            Locations = new Dictionary<(int, int, int), DateTime>()
		                        };

		                        if (prop.Value["Locations"] != null)
		                        {
		                            foreach (var locProp in prop.Value["Locations"].Children<JProperty>())
		                            {
		                                var coords = locProp.Name.Trim('(', ')').Split(',');
		                                if (coords.Length == 3 &&
		                                    int.TryParse(coords[0], out int x) &&
		                                    int.TryParse(coords[1], out int y) &&
		                                    int.TryParse(coords[2], out int z))
		                                {
		                                    map.Locations[(x, y, z)] = locProp.Value.ToObject<DateTime>();
		                                }
		                            }
		                        }

		                        database.Maps[mapId] = map;
		                    }
		                }
		            }

		            return database;
		        }
		        catch (Exception ex)
		        {
		            throw new JsonSerializationException($"Error deserializing MiningDatabase: {ex.Message}", ex);
		        }
		    }

		    public override void WriteJson(JsonWriter writer, UoTiles.MiningDatabase value, JsonSerializer serializer)
		    {
		        try
		        {
		            writer.WriteStartObject();

		            writer.WritePropertyName("Maps");
		            writer.WriteStartObject();

		            if (value.Maps != null)
		            {
		                foreach (var mapKvp in value.Maps)
		                {
		                    writer.WritePropertyName(mapKvp.Key.ToString());
		                    writer.WriteStartObject();

		                    writer.WritePropertyName("Locations");
		                    writer.WriteStartObject();
		                    foreach (var locKvp in mapKvp.Value.Locations)
		                    {
		                        writer.WritePropertyName($"({locKvp.Key.Item1},{locKvp.Key.Item2},{locKvp.Key.Item3})");
		                        serializer.Serialize(writer, locKvp.Value);
		                    }
		                    writer.WriteEndObject();
		                    writer.WriteEndObject();
		                }
		            }

		            writer.WriteEndObject();
		            writer.WriteEndObject();
		        }
		        catch (Exception ex)
		        {
		            throw new JsonSerializationException($"Error serializing MiningDatabase: {ex.Message}", ex);
		        }
		    }
		}



		public class MapConverter : JsonConverter<UoTiles.Map>
		{
			public override UoTiles.Map ReadJson(JsonReader reader, Type objectType, UoTiles.Map existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var jObject = JObject.Load(reader);
				var result = new UoTiles.Map
				{
					Locations = new Dictionary<(int, int, int), DateTime>()
				};

				if (jObject["Locations"] != null)
				{
					foreach (var prop in jObject["Locations"].Children<JProperty>())
					{
						var coords = prop.Name.Trim('(', ')').Split(',');
						if (coords.Length == 3 &&
						    int.TryParse(coords[0], out int x) &&
						    int.TryParse(coords[1], out int y) &&
						    int.TryParse(coords[2], out int z))
						{
							result.Locations[(x, y, z)] = prop.Value.ToObject<DateTime>();
						}
					}
				}

				return result;
			}

			public override void WriteJson(JsonWriter writer, UoTiles.Map value, JsonSerializer serializer)
			{
				writer.WriteStartObject();
				writer.WritePropertyName("Locations");
				writer.WriteStartObject();

				foreach (var kvp in value.Locations)
				{
					writer.WritePropertyName($"({kvp.Key.Item1},{kvp.Key.Item2},{kvp.Key.Item3})");
					serializer.Serialize(writer, kvp.Value);
				}

				writer.WriteEndObject();
				writer.WriteEndObject();
			}
		}

		
		public class ConcurrentDictionaryValueTupleDateTimeConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(ConcurrentDictionary<ValueTuple<int, int>, DateTime>);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				var dict = new ConcurrentDictionary<ValueTuple<int, int>, DateTime>();
				var jObject = JObject.Load(reader); // Read the JSON object

				foreach (var property in jObject.Properties())
				{
					// Each key is stored as a string, deserialize it into ValueTuple<int, int>
					var key = JsonConvert.DeserializeObject<ValueTuple<int, int>>(property.Name);
					// The value is DateTime
					var value = property.Value.ToObject<DateTime>();
					dict[key] = value;
				}
				return dict;
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				var dict = (ConcurrentDictionary<ValueTuple<int, int>, DateTime>)value;

				writer.WriteStartObject();
				foreach (var kvp in dict)
				{
					var keySerialized = JsonConvert.SerializeObject(kvp.Key); // Serialize key as tuple string
					writer.WritePropertyName(keySerialized);
					serializer.Serialize(writer, kvp.Value); // Serialize DateTime
				}
				writer.WriteEndObject();
			}
		}

		public class ValueTupleJsonConverter<T1, T2> : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				// Check if the object type is a ValueTuple with the correct generics
				return objectType == typeof(ValueTuple<T1, T2>);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				if (reader.TokenType == JsonToken.String)
				{
					var stringValue = reader.Value.ToString();
					var elements = stringValue.Trim('(', ')', ' ').Split(',');
					if (elements.Length == 2)
					{
						var item1 = Convert.ChangeType(elements[0].Trim(), typeof(T1));
						var item2 = Convert.ChangeType(elements[1].Trim(), typeof(T2));
						return ValueTuple.Create((T1)item1, (T2)item2);
					}
				}
				else if (reader.TokenType == JsonToken.StartObject)
				{
					var obj = JObject.Load(reader);
					var item1 = obj["Item1"].ToObject<T1>();
					var item2 = obj["Item2"].ToObject<T2>();
					return ValueTuple.Create(item1, item2);
				}
				throw new JsonSerializationException("Invalid ValueTuple format.");
			}
			

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				var tuple = (ValueTuple<T1, T2>)value;
				writer.WriteStartObject();
				writer.WritePropertyName("Item1");
				serializer.Serialize(writer, tuple.Item1);
				writer.WritePropertyName("Item2");
				serializer.Serialize(writer, tuple.Item2);
				writer.WriteEndObject();

			}
		}
		
		public class ValueTupleJsonConverter<T1, T2, T3> : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(ValueTuple<T1, T2, T3>);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				if (reader.TokenType == JsonToken.String)
				{
					var stringValue = reader.Value.ToString();
					var elements = stringValue.Trim('(', ')', ' ').Split(',');
					if (elements.Length == 3)
					{
						var item1 = Convert.ChangeType(elements[0].Trim(), typeof(T1));
						var item2 = Convert.ChangeType(elements[1].Trim(), typeof(T2));
						var item3 = Convert.ChangeType(elements[2].Trim(), typeof(T3));
						return ValueTuple.Create((T1)item1, (T2)item2, (T3)item3);
					}
				}
				else if (reader.TokenType == JsonToken.StartObject)
				{
					var obj = JObject.Load(reader);
					var item1 = obj["Item1"].ToObject<T1>();
					var item2 = obj["Item2"].ToObject<T2>();
					var item3 = obj["Item3"].ToObject<T3>();
					return ValueTuple.Create(item1, item2, item3);
				}

				throw new JsonSerializationException("Invalid ValueTuple format.");
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				var tuple = (ValueTuple<T1, T2, T3>)value;
				writer.WriteStartObject();
				writer.WritePropertyName("Item1");
				serializer.Serialize(writer, tuple.Item1);
				writer.WritePropertyName("Item2");
				serializer.Serialize(writer, tuple.Item2);
				writer.WritePropertyName("Item3");
				serializer.Serialize(writer, tuple.Item3);
				writer.WriteEndObject();
			}
		}
	}
}