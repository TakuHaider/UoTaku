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

//#assembly <Newtonsoft.Json.dll>

namespace RazorEnhanced
{
	/*
	 using (var storedData = new UoTJson("MyDataFile"))
{
    storedData.storedData.StoreData("some value", "key", UoTJson.StoreType.Global);
    var value = storedData.GetData<string>("key", UoTJson.StoreType.Global);
}

	 */
    public partial class UoTJson : IDisposable
    {
	    private bool _disposed = false;
	    private static readonly object FileLock = new object();
	    private readonly FileInfo _fileName;
	    private JSONRoot _cachedData; // Cached in-memory JSON data
	    public static string FileName; // Static field to hold the file name
	    private readonly string _serverName;		// Name of the server
	    private readonly string _characterName;		// Name of the account
	    public readonly JsonSerializerSettings _settings;	// Settings for deserializing JSON data


	    private T ConvertJTokenToType<T>(JToken token)
	    {
		    var serializer = JsonSerializer.Create(_settings); // Create a JsonSerializer from _settings
		    return token.ToObject<T>(serializer);
	    }
	    
        public enum StoreType
        {
            Global,
            Server,
            Character
        }

        private class Server
        {
            public Dictionary<string, object> ServerData { get; set; }
            public Dictionary<string, Character> Characters { get; set; }
            public Server()
            {
                ServerData = new Dictionary<string, object>();
                Characters = new Dictionary<string, Character>();
            }
        }

        private class Character : Dictionary<string, object>
        {
            public Character() { }
            public void AddSetting(string key, object value)
            {
                this[key] = value;
            }
        }
        private class JSONRoot
        {
            public Dictionary<string, object> Global { get; set; }
            public Dictionary<string, Server> Servers { get; set; }
            public JSONRoot()
            {
                Global = new Dictionary<string, object>();
                Servers = new Dictionary<string, Server>();
            }
        }
        public class CustomSerializationBinder : ISerializationBinder
        {
	        public Type BindToType(string assemblyName, string typeName)
	        {
		        // Handle Dictionary types
		        if (typeName.StartsWith("System.Collections.Generic.Dictionary"))
		        {
			        if (typeName.Contains("UoTJson+Server"))
			        {
				        return typeof(Dictionary<string, Server>);
			        }
			        if (typeName.Contains("UoTJson+Character"))
			        {
				        return typeof(Dictionary<string, Character>);
			        }
			        // Default dictionary of string to object
			        return typeof(Dictionary<string, object>);
		        }

		        // Handle the specific case for MiningDatabase
		        if (typeName.Contains("GatherOre+MiningDatabase"))
		        {
			        return typeof(UoTiles.MiningDatabase);
		        }
		        if (typeName.Contains("UoTRecipes+CraftProfession") ||
		            typeName.Contains("UoTRecipes+CraftCategory") ||
		            typeName.Contains("UoTRecipes+CraftRecipe"))
		        {
			        // Strip any assembly name that might be in the typeName
			        string cleanTypeName = typeName.Contains(",") 
				        ? typeName.Substring(0, typeName.IndexOf(',')) 
				        : typeName;
                
			        return Type.GetType($"{cleanTypeName}, {Assembly.GetExecutingAssembly().FullName}");
		        }

		        // Handle specific types
		        if (typeName.Contains("UoTJson+JSONRoot"))
		        {
			        return typeof(JSONRoot);
		        }
		        if (typeName.Contains("UoTJson+Server"))
		        {
			        return typeof(Server);
		        }
		        if (typeName.Contains("UoTJson+Character"))
		        {
			        return typeof(Character);
		        }

			        
		        // For other types, try to resolve normally
		        string typeToFind = typeName.Contains(",") ? typeName.Substring(0, typeName.IndexOf(',')) : typeName;
		        
		        
		        return Type.GetType($"{typeToFind}, {Assembly.GetExecutingAssembly().FullName}", false, true) 
		               ?? Type.GetType(typeToFind);
	        }
	        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
	        {
		        //assemblyName = null; //serializedType.Assembly.FullName;
		        // Use the current assembly name when serializing
		        assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
		        typeName = serializedType.FullName;

	        }
        }

        /// <summary>
        /// Initializes a new instance of the StoredData class.
        /// </summary>
        /// <param name="storedDataFolder">The folder path where the JSON file will be stored.</param>
        /// <param name="serverName">The name of the server.</param>
        /// <param name="characterName">The name of the character.</param>
        public UoTJson(string fileName = null)
        {
	        var dataFolder = Path.GetFullPath(Path.Combine(Assistant.Engine.RootPath, "Data"));

            _fileName = new FileInfo(Path.Combine(dataFolder, MakeValidFileName(fileName+".json"))) ??
                        new FileInfo(Path.Combine(dataFolder, MakeValidFileName(FileName+".json")));
            _serverName = Misc.ShardName();
            _characterName = Player.Name;
            _settings = new JsonSerializerSettings
            {
	            TypeNameHandling = TypeNameHandling.Auto, //Auto
	            Formatting = Formatting.Indented, 
	            NullValueHandling = NullValueHandling.Ignore,  // Skips null values
	            DefaultValueHandling = DefaultValueHandling.Ignore,  // Skips default values
	            Converters = new List<JsonConverter>
	            {
		            new Newtonsoft.Json.Converters.KeyValuePairConverter(),
		            new Newtonsoft.Json.Converters.StringEnumConverter(),
                    new ConcurrentDictionaryValueTupleDateTimeConverter(),
                    new ValueTupleJsonConverter<int, int, int>(),
                    new ValueTupleJsonConverter<int, int>(),
                    new MiningDatabaseConverter(),
                    new MapConverter(),
                    new HashSetConverter()
	            },
	            SerializationBinder = new CustomSerializationBinder(),
	            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
	            DateParseHandling = DateParseHandling.None

            };

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            if (!File.Exists(_fileName.FullName))
            {
                File.Create(_fileName.FullName).Close();
            }

            // Initialize the cached JSON data structure.
            InitializeJsonStructure();
        }

        /// <summary>
        /// Initializes the JSON structure in memory and ensures essential fields are set.
        /// </summary>
        private void InitializeJsonStructure()
        {
	        lock (FileLock)
	        {
		        try
		        {
			        var retries = 3;
			        while (retries > 0)
			        {
				        try
				        {
					        // Allow others to write while we read
					        using (var fs = new FileStream(_fileName.FullName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
					        using (var sr = new StreamReader(fs))
					        {
						        string jsonText = sr.ReadToEnd();
						        if (string.IsNullOrWhiteSpace(jsonText))
						        {
							        _cachedData = new JSONRoot();
							        SaveToFile();
							        return;
						        }
						        _cachedData = JsonConvert.DeserializeObject<JSONRoot>(jsonText, _settings) ?? new JSONRoot();
						        break; // Success - exit retry loop
					        }
				        }
				        catch (IOException ex) when (retries > 1)
				        {
					        retries--;
					        System.Threading.Thread.Sleep(100); // Wait before retry
					        continue;
				        }
				        catch (Exception ex)
				        {
					        throw new IOException($"Failed to initialize JSON structure: {ex.Message}", ex);
				        }
			        }
		        }
		        catch (Exception ex)
		        {
			        UoTLogger.LogErrorToFile($"Error initializing from file {_fileName.FullName}: {ex.Message}");
			        throw;
		        }
	        }
        }


        /// <summary>
        /// Ensures that resources are properly disposed of and cached data is saved to the JSON file.
        /// </summary>
        public void Dispose()
        {
	        Dispose(true);
	        GC.SuppressFinalize(this); // Prevents the finalizer from being called
        }

        /// <summary>
        /// Performs cleanup of resources.
        /// </summary>
        /// <param name="disposing">True if called manually; false if called by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
	        if (!_disposed)
	        {
		        if (disposing)
		        {
			        // Save cached data to file before disposing
			        SaveToFile();
		        }

		        // Free unmanaged resources here if any (e.g., native file streams)

		        // Mark the object as disposed
		        _disposed = true;
	        }
        }

        // Finalizer in case Dispose is not called explicitly
        ~UoTJson()
        {
	        Dispose(false);
        }
        public static string MakeValidFileName(string input)
        {
	        if (string.IsNullOrEmpty(input)) return "unnamed";
	        // Get invalid characters
	        var invalidChars = Path.GetInvalidFileNameChars();
	        // Replace invalid characters with underscore
	        var cleanName = new string(input.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
	        // Remove multiple consecutive underscores
	        cleanName = string.Join("_", cleanName.Split(new[] { '_' }, 
		        StringSplitOptions.RemoveEmptyEntries));
	        // Trim leading/trailing spaces and dots
	        cleanName = cleanName.Trim(' ', '.');
	        // If name would be empty after cleaning, return a default name
	        return string.IsNullOrEmpty(cleanName) ? "unnamed" : cleanName;
        }
        /// <summary>
        /// Saves the cached JSON data to the file.
        /// </summary>
        private void SaveToFile()
        {
	        lock (FileLock)
	        {
		        try
		        {
			        // Retry logic for file access
			        int retries = 3;
			        while (retries > 0)
			        {
				        try
				        {
					        
					        // Use FileShare to allow other processes to read while we write
					        using (FileStream fs = new FileStream(_fileName.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
					        using (StreamWriter sw = new StreamWriter(fs))
					        {
						        var jsonText = JsonConvert.SerializeObject(_cachedData, _settings);
						        sw.Write(jsonText);
						        break; // Success - exit retry loop
					        }
				        }
				        catch (IOException ex) when (retries > 1)
				        {
					        retries--;
					        System.Threading.Thread.Sleep(100); // Wait before retry
					        continue;
				        }
				        catch (Exception ex)
				        {
					        throw new IOException($"Failed to save JSON data to file: {ex.Message}", ex);
				        }
			        }
		        }
		        catch (Exception ex)
		        {
			        UoTLogger.LogErrorToFile($"Error saving to file {_fileName.FullName}: {ex.Message}");
			        throw;
		        }
	        }
        }

        /// <summary>
        /// Returns a string representation of the JSON data.
        /// </summary>
        /// <returns>A string representation of the JSON data.</returns>
        public override string ToString()
        {
            string jsonText = File.ReadAllText(_fileName.FullName);
            JSONRoot jsonObj = JsonConvert.DeserializeObject<JSONRoot>(jsonText, _settings);
            if (jsonObj == null) return "";

            return JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        }
        /// <summary>
        /// Stores data in the JSON file based on the specified storage type.
        /// </summary>
        /// <param name="data">The data to store.</param>
        /// <param name="keyName">The key name to associate with the data.</param>
        /// <param name="storage">The storage type (Global, Server, or Character).</param>
        public void StoreData(object data, string keyName, StoreType storage = StoreType.Character)
        {
	        lock (FileLock)
	        {
		        try
		        {
			        if (data == null) return;
			        // Ensure basic structure exists
			        if (_cachedData == null)
				        _cachedData = new JSONRoot();
            
			        if (_cachedData.Global == null)
				        _cachedData.Global = new Dictionary<string, object>();
                
			        if (_cachedData.Servers == null)
				        _cachedData.Servers = new Dictionary<string, Server>();

			        // Ensure server exists
			        if (!_cachedData.Servers.TryGetValue(_serverName, out var server))
			        {
				        server = new Server();
				        _cachedData.Servers[_serverName] = server;
			        }

			        if (_cachedData.Servers.TryGetValue(_serverName, out var currentServer))
			        {
				        // Ensure server global dictionary exists
				        if (currentServer.ServerData == null)
					        currentServer.ServerData = new Dictionary<string, object>();

				        // Ensure characters dictionary exists
				        if (currentServer.Characters == null)
					        currentServer.Characters = new Dictionary<string, Character>();

				        // Ensure character exists
				        if (!currentServer.Characters.TryGetValue(_characterName, out var character))
				        {
					        character = new Character();
					        currentServer.Characters[_characterName] = character;
				        }
			        }
			        else
			        {
				        // Handle missing server case
			        } ;


			        // Store data based on the selected storage type
			        switch (storage)
			        {
				        case StoreType.Global:
					        _cachedData.Global[keyName] = data;
					        break;
				        case StoreType.Server:
					        currentServer.ServerData[keyName] = data;
					        break;
				        case StoreType.Character:
					        currentServer.Characters[_characterName][keyName] = data;
					        break;
			        }

			        SaveToFile();
		        }
		        catch (Exception ex)
		        {
			        UoTLogger.LogErrorToFile($"Failed to store data: {ex.Message}", ex);
		        }

	        }
	        
        }
        private object InitializeMissingKey(Dictionary<string, object> dictionary, string keyName)
        {
	        object defaultValue = keyName switch
	        {
		        "OreMaps" => new UoTiles.MiningDatabase(),
		        _ => default 

	        };
	        if (!dictionary.ContainsKey(keyName))
	        {
		        dictionary.Add(keyName, defaultValue);
		        SaveToFile(); // Save the new structure
		        UoTLogger.LogErrorToFile($"Missing key '{keyName}' was initialized with default value.");
	        }
	        return dictionary[keyName];
        }

        public object GetValueFromDictionary(Dictionary<string, object> dict, string keyName)
        {
	        if (dict == null)
		        throw new ArgumentNullException(nameof(dict));
	        if (string.IsNullOrEmpty(keyName))
		        throw new ArgumentException("Key name cannot be null or empty", nameof(keyName));
        
	        return dict.TryGetValue(keyName, out var value) ? value : null;
        }


        /// <summary>
        /// Retrieves data from the JSON file based on the specified storage type.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <param name="keyName">The key name associated with the data.</param>
        /// <param name="targetType"></param>
        /// <param name="storage">The storage type (Global, Server, or Character).</param>
        /// <returns>The retrieved data or the default value if keyName doesn't exist.</returns>
        public T GetData<T>(string keyName, Type targetType, StoreType storage = StoreType.Character)
        {
	        lock (FileLock)
	        {
		        try
		        {
			        // Initialize structure if needed
			        if (_cachedData == null)
				        _cachedData = new JSONRoot();
            
			        if (_cachedData.Global == null)
				        _cachedData.Global = new Dictionary<string, object>();
                
			        if (_cachedData.Servers == null)
				        _cachedData.Servers = new Dictionary<string, Server>();

			        Server server;
			        // Ensure server's structures exist
			        if (_cachedData.Servers.TryGetValue(_serverName, out server))
			        {
				        if (server.ServerData == null)
					        server.ServerData = new Dictionary<string, object>();
            
				        if (server.Characters == null)
					        server.Characters = new Dictionary<string, Character>();

				        // Ensure character exists
				        if (!server.Characters.ContainsKey(_characterName))
					        server.Characters[_characterName] = new Character();
			        }
			        else
			        {
				        server = new Server();
				        _cachedData.Servers[_serverName] = server;
			        }

			        var retVal = storage switch
			        {
				        StoreType.Global => _cachedData.Global.TryGetValue(keyName, out var gVal) 
					        ? gVal 
					        : InitializeMissingKey(_cachedData.Global, keyName),
    
				        StoreType.Server => _cachedData.Servers.TryGetValue(_serverName, out server)
					        ? (server.ServerData.TryGetValue(keyName, out var sVal) 
						        ? sVal 
						        : InitializeMissingKey(server.ServerData, keyName))
					        : default,
    
				        StoreType.Character => _cachedData.Servers.TryGetValue(_serverName, out server)
					        ? (server.Characters.TryGetValue(_characterName, out var character)
						        ? (character.TryGetValue(keyName, out var cVal)
							        ? cVal
							        : InitializeMissingKey(character, keyName))
						        : default)
					        : default,
    
				        _ => default
			        };

			        
			        // If the value is null or missing, return default
			        if (retVal == null)
				        return default(T);
			        // Log or debug to analyze retrieved data
			        //UoTLogger.LogErrorToFile($"Retrieved key '{keyName}': {retVal.GetType().FullName},  value: {retVal.GetType()} - {JsonConvert.SerializeObject(retVal)}");


			        if (retVal is T directValue)
			        {
				        return directValue;
			        }
			        if (retVal is string serializedJson)
			        {
				        if (typeof(T) == typeof(ConcurrentDictionary<ValueTuple<int, int>, DateTime>))
				        {
					        return (T)(object)JsonConvert.DeserializeObject<ConcurrentDictionary<ValueTuple<int, int>, DateTime>>(serializedJson, _settings);
				        }
				        if (typeof(T) == typeof(ValueTuple<int, int>))
				        {
					        return (T)(object)JsonConvert.DeserializeObject<ValueTuple<int, int>>(serializedJson, _settings);
				        }

				        return JsonConvert.DeserializeObject<T>(serializedJson, _settings);
			        }
			        
			        if (retVal is JArray jArr)
				        return ConvertJTokenToType<T>(jArr);

			        if (retVal is JObject jObj)
			        {
				        // Special handling for List types when we have a wrapped format
				        if (typeof(T).IsGenericType && 
				            typeof(T).GetGenericTypeDefinition() == typeof(List<>) &&
				            jObj["$type"] != null &&
				            jObj["$values"] != null)
				        {
					        var serializer = JsonSerializer.Create(_settings);
					        return jObj["$values"].ToObject<T>(serializer);

				        }
    
				        return ConvertJTokenToType<T>(jObj);
			        }


			        return (T)Convert.ChangeType(retVal, typeof(T)); // Could use Type targetType
		        }
		        catch (Exception ex)
		        {
			        //ClearData(keyName, storage);
			        UoTLogger.LogErrorToFile($"Error retrieving key '{keyName}' in GetData: {ex.Message}");
		        }
		        return default(T);

	        }
        }
        public void ClearData(string keyName, StoreType storage = StoreType.Character)
        {
	        string jsonText = File.ReadAllText(_fileName.FullName);
	        JSONRoot jsonObj = JsonConvert.DeserializeObject<JSONRoot>(jsonText, _settings) ?? throw new Exception("Error reading JSON file");

	        bool save = false;

	        switch (storage)
	        {
		        case StoreType.Global:
			        if (jsonObj.Global.ContainsKey(keyName))
			        {
				        jsonObj.Global.Remove(keyName);
				        save = true;
			        }
			        break;
		        case StoreType.Server:
			        if (jsonObj.Servers[_serverName].ServerData.ContainsKey(keyName))
			        {
				        jsonObj.Servers[_serverName].ServerData.Remove(keyName);
				        save = true;
			        }
			        break;
		        case StoreType.Character:
		        default:
			        if (jsonObj.Servers.TryGetValue(_serverName, out var server) &&
			            server.Characters.TryGetValue(_characterName, out var character) &&
			            character.ContainsKey(keyName))
			        {
				        character.Remove(keyName);
				        save = true;
			        }
			        break;
	        }

	        if (save)
	        {
		        jsonText = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
		        File.WriteAllText(_fileName.FullName, jsonText);
	        }
        }

        public static void ClearData(string keyName, string fileName = default)
        {
	        // If no file name is provided, use the static field, or set it for the first time
	        if (fileName == default)
	        {
		        if (FileName == null)
		        {
			        FileName = UoTSystem.GetCallingClassName(); // Initialize static field
		        }
		        fileName = FileName; // Use the static field
	        }
	        using (var storedData = new UoTJson(fileName))
	        {
		        storedData.ClearData(keyName);
	        }
        }

        public static T StoreOrGetJson<T>(string key, T value = default(T), string fileName = default) where T : struct
		{
			try
			{
				if (fileName == default)
				{
					if (FileName == null)
					{
						FileName = UoTSystem.GetCallingClassName();
					}

					fileName = FileName;
				}

				using (var storedData = new UoTJson(fileName))
				{
					if (typeof(T) == typeof(int) || typeof(T) == typeof(uint) ||
					    typeof(T) == typeof(long) || typeof(T) == typeof(double))
					{
						if (!EqualityComparer<T>.Default.Equals(value, default(T)) &&
						    //value != null && 
						    !((value is uint uintValue && uintValue == 0) ||
						      (value is double doubleValue && doubleValue == 0) ||
						      (value is int intValue && intValue == 0) ||
						      (Convert.ToDouble(value) == 0)))
						{
							var stringValue = Convert.ToString(value);
							if (!string.IsNullOrWhiteSpace(stringValue) &&
							    !stringValue.Trim()
								    .Equals("null",
									    StringComparison
										    .OrdinalIgnoreCase)) // Additional null check for string conversion
							{
								return storedData.GetData<T>(key, typeof(T));
							}

							storedData.StoreData(value, key);
							return value;
						}

						var storedValue = storedData.GetData<string>(key, typeof(string));
						if (!string.IsNullOrEmpty(storedValue))
						{
							if (typeof(T) == typeof(int) && int.TryParse(storedValue, out int intResult))
								return (T)(object)intResult;
							else if (typeof(T) == typeof(long) && long.TryParse(storedValue, out long longResult))
								return (T)(object)longResult;
							else if (typeof(T) == typeof(double) &&
							         double.TryParse(storedValue, out double doubleResult))
								return (T)(object)doubleResult;
							else if (typeof(T) == typeof(uint) && uint.TryParse(storedValue, out uint uintResult))
								return (T)(object)uintResult;
						}

						// Handle the special case for int with PromptTarget
						if (typeof(T) == typeof(int))
						{
							int promptValue = UoTItems.target.PromptTarget("Select " + key);
							if (promptValue != 0) // Only store if a valid selection was made
							{
								storedData.StoreData(promptValue.ToString(), key);
								return (T)(object)promptValue;
							}
						}

					}
					else
					{
						// Original complex type handling
						if (!EqualityComparer<T>.Default.Equals(value, default))
						{
							storedData.StoreData(value, key);
						}
						else
						{
							value = storedData.GetData<T>(key, typeof(T));
						}
					}

					return value;
				}
			}
			catch (Exception e)
			{
				
				if (e is FormatException || e is InvalidCastException)
				{
				
					using (var storedData = new UoTJson(fileName))
					{
						// Original complex type handling
						if (!EqualityComparer<T>.Default.Equals(value, default))
						{
							storedData.StoreData(value, key);
						}
						else
						{
							value = storedData.GetData<T>(key, typeof(T));
						}
					}
				}
				else UoTLogger.LogErrorToFile($"Error retrieving key '{key}' in StoreOrGetJson: {e.Message}");
			}
			return default(T);
		}

		public static T StoreOrGetJsonByIndex<T>(string key, int index = 0, T value = default, string fileName = default) where T : struct
		{       
		    try
		    {
		        if (fileName == default)
		        {
		            if (FileName == null)
		            {
		                FileName = UoTSystem.GetCallingClassName();
		            }
		            fileName = FileName;
		        }

		        var dataKey = key + "_" + index;
		        using (var storedData = new UoTJson(fileName))
		        {
		            if (!EqualityComparer<T>.Default.Equals(value, default))
		            {
		                if (typeof(T) == typeof(int))
		                {
		                    // Store int as string to avoid type metadata
		                    storedData.StoreData(value.ToString(), dataKey);
		                }
		                else
		                {
		                    storedData.StoreData(value, dataKey);
		                }
		            }
		            else
		            {
		                if (typeof(T) == typeof(int))
		                {
		                    // Try to get the stored value as string first
		                    var storedValue = storedData.GetData<string>(dataKey, typeof(string));
		                    if (!string.IsNullOrEmpty(storedValue) && int.TryParse(storedValue, out int parsedValue))
		                    {
			                    if (parsedValue != 0)
									return (T)(object)parsedValue;
		                    }

		                    // If no valid stored value, prompt for new value
		                    int promptValue = UoTItems.target.PromptTarget("Select " + key);
		                    Target.WaitForTarget(5000);
		                    storedData.StoreData(promptValue.ToString(), dataKey);
		                    return (T)(object)promptValue;
		                }
		                else if (typeof(T) == typeof(ConcurrentDictionary<(int, int), string>))
		                {
		                    var result = storedData.GetData<ConcurrentDictionary<(int, int), string>>(dataKey, 
		                        typeof(ConcurrentDictionary<(int, int), string>));
		                    if (result == null)
		                    {
		                        UoTLogger.LogErrorToFile($"Unable to fetch data for key: {dataKey}");
		                        return default(T);
		                    }
		                    return (T)(object)result;
		                }
		                else
		                {
		                    value = storedData.GetData<T>(dataKey, typeof(T));
		                }
		            }
		            return value;
		        }
		    }
		    catch (Exception ex)
		    {
		        UoTLogger.LogErrorToFile($"Error in StoreOrGetJsonByIndex for key '{key}': {ex.Message}");
		        return default;
		    }
		}
    }
}
