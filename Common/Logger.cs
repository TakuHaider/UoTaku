//
// Simple logger library. To be extended
// 
// Developed by SimonSoft on Demise Server - 2021
//

using System;
using System.IO;
using Engine = Assistant.Engine;


namespace RazorEnhanced
{
    class UoTLogger
    {
	    
	    
	    // Determine the directory of the currently executing assembly
	    //private static readonly string CurrentFileDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
	    public static string LogFilePath = Path.Combine(Path.Combine(Engine.RootPath, "Data"), "UoT.log");
	    //private static readonly string LogFilePath2 = Path.Combine(CurrentFileDirectory, "application.log");
	    //private static readonly string LogFilePath3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "application.log");

	    
	    
	    
	    
        /// <summary>
        /// Enum for razor colors
        /// </summary>
        public enum COLORS
        {
            GREY = 0,
            BLACK = 1,
            BLUE = 2,
            LIGHT_BLUE = 5,
            VIOLET = 9,
            LIGHT_VIOLET = 10,
            PURPLE = 14,
            LIGHT_PURPLE = 15,
            PINK = 23,
            LIGHT_PINK = 24,
            RED = 33,
            LIGHT_RED = 34,
            ORANGE = 43,
            LIGHT_ORANGE = 44,
            YELLOW = 48,
            LIGHT_YELLOW = 49,
            GREEN = 63,
            LIGHT_GREEEN = 66,
            AZURE = 93,
            LIGHT_AZURE = 96,
        }
        
        /// <summary>
        /// Logs an error message to a Console.
        /// </summary>
        public static void LogErrorToConsole(string errorMessage, Exception ex = null)
        {
	        try
	        {
		        // Capture the timestamp
		        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

		        // Create the log message
		        string logMessage = $"[{timestamp}] ERROR: {errorMessage}";

		        // Add exception details if provided
		        if (ex != null)
		        {
			        logMessage += Environment.NewLine + $"Exception: {ex.Message}" + Environment.NewLine + $"StackTrace: {ex.StackTrace}";
		        }
		        Console.WriteLine(logMessage);

		        // Write to the log file
		        //File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
		        
	        }
	        catch (Exception loggingEx)
	        {
		        // Handle logging errors without disrupting the main application
		        System.Diagnostics.Debug.WriteLine($"Logging failed: {loggingEx.Message}");
	        }
        }
        
        /// <summary>
        /// Logs an error message to a file.
        /// </summary>
        public static void LogErrorToFile(string errorMessage, Exception ex = null, bool timeStamp = true)
        {
            try
            {
                // Capture the timestamp
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string logMessage;
                
                // Create the log message
                if (timeStamp)
	                logMessage = $"[{timestamp}]: {errorMessage}";
                else 
	                logMessage = $"{errorMessage}";

                // Add exception details if provided
                if (ex != null)
                {
                    logMessage += Environment.NewLine + $"Exception: {ex.Message}" + Environment.NewLine + $"StackTrace: {ex.StackTrace}";
                }

                // Write to the log file
                //(`outputConsole`) is defined as a `RichTextBox` in `EnhancedScriptEditor.Designer.cs` and is manipulated in `EnhancedScriptEditor.cs`
                //RazorEnhanced.Logger.MESSAGEBOX_TYPE;
                //RazorEnhanced.Logger.MessageBox(logMessage, RazorEnhanced.Logger.MESSAGEBOX_TYPE.INFORMATION);
                //Console.Out.Write(logMessage);
                //Console.WriteLine(errorMessage);
                //System.Diagnostics.Debug.WriteLine(logMessage);
                //print(logMessage);
                //RazorEnhanced.EnhancedScriptService.Instance
                if (timeStamp)
					File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
                else
	                File.AppendAllText(LogFilePath, logMessage + @" ");
            }
            catch (Exception loggingEx)
            {
                // Handle logging errors without disrupting the main application
                System.Diagnostics.Debug.WriteLine($"Logging failed: {loggingEx.Message}");
            }
        }
    }
}
