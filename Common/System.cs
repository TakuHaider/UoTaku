//
// System functions library.
// 
// Uses some functions from SimonSoft - 2021
//

using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


//Misc.RemoveSharedValue("Lootmaster:Pause"); 
//Misc.ReadSharedValue("Lootmaster:Pause)
//Misc.SetSharedValue("Lootmaster:Pause", true);
//Misc.SetSharedValue("Lootmaster:Pause", false);
//Misc.SetSharedValue("Lootmaster:Pause", 2000);

namespace RazorEnhanced
{
    public static class UoTSystem
    {
        private const int DELAY_USE_ITEM_MS = 600;
        
        //await ExecuteSafelyAsync(async token => {  return await CheckConditionAsync(token); }, cancellationTokenSource.Token);
        public static async Task<T> ExecuteSafelyAsync<T>(Func<CancellationToken, Task<T>> taskFunc, CancellationToken cancellationToken = default)
        {
	        try
	        {
		        T result = await taskFunc(cancellationToken).ConfigureAwait(false);
		        //var methodInfo = taskFunc.Method; // Get method info for taskFunc
		        //Console.WriteLine($"Task {methodInfo.Name} completed successfully with result: {result}");
		        return result;

	        }
	        catch (OperationCanceledException)
	        {
		        var methodInfo = taskFunc.Method; // Get method info for taskFunc
		        // Log method name and return type
		        Console.WriteLine($"Executing task: {methodInfo.Name} Return type: {typeof(T).Name} was canceled.");
	        }
	        catch (Exception ex)
	        {
		        var methodInfo = taskFunc.Method;
		        Console.WriteLine($"Executing task: {methodInfo.Name}");
		        Console.WriteLine($"An error occurred: {ex.Message}");
		        Console.WriteLine($"Return type: {methodInfo.ReturnType.Name}");
		        Console.WriteLine($"Resolved generic type <T>: {typeof(T).Name}");
		        var parameters = methodInfo.GetParameters();
		        if (parameters.Length > 0)
		        {
			        Console.WriteLine($"Method parameters:");
			        foreach (var parameter in parameters)
			        {
				        Console.WriteLine($"  - {parameter.Name}: {parameter.ParameterType.Name}");
			        }
		        }
		        else
		        {
			        Console.WriteLine("No input parameters for the method.");
		        }
		        throw;
	        }
	        finally
	        {
		        Console.WriteLine($"Task: {taskFunc.Method.Name} execution has finished.");
		        
	        }
	        return default;
        }

        
        
        
        // Seed the random number generator, set during initialization


        /// <summary>
        /// Returns if journal entry was found and removes entry from journal.
        /// </summary>
        /// <param name="journal">Journal to be read</param>
        /// <param name="message">Journal entry to find</param>
        /// <param name="send">Send misc message if found</param>
        /// <param name="type">Type of journal entry to search</param>
        public static bool SearchJournal(Journal journal, string message, bool send = false, string type = "System")
        {
	        if (!journal.SearchByType(message, type)) return false;
	        if (send) Misc.SendMessage(message, 33);
	        journal.Clear(message);
	        return true;
        }
        
        /// <summary>
        /// Returns if timer exists, if expired it will create and print message.
        /// </summary>
        /// <param name="mS">Serial of Mobile, Item or Object. Sets context and message index</param>
        /// <param name="m1">First part of saved string, category</param>
        /// <param name="m2">Second part of saved string, details</param>
        /// <param name="mO">Message output type</param>
        /// <param name="hue">Text color</param>
        /// <param name="duration">Length of timer</param>
        public static bool CheckTimer(int mS, string m1 = "", string m2 = "", MessageOutput mO = MessageOutput.None, int hue = 33, int duration = 4000)
        {
	        if (duration != 0)
	        {
		        if (Timer.Check(m1 + m2 + mS)) return true;
		        Timer.Create(m1 + m2 + mS, duration);
	        }
	        switch (mO)
	        {
		        case MessageOutput.Mobile:
			        Mobiles.Message(mS, hue, m2);
			        break;
		        case MessageOutput.Player:
			        Player.HeadMessage(hue,  m2);
			        break;
		        case MessageOutput.Misc:
			        Misc.SendMessage(m1, hue);
			        Misc.SendMessage(m2, hue);
			        break;
		        //case MessageOutput.None:
	        }
	        return false;
        }
        
		
        /// <summary>
        /// Returns if timer exists, creates new timer and if expired it will print message.
        /// </summary>
        /// <param name="mS">Serial of Mobile, Item or Object. Sets context and message index</param>
        /// <param name="m1">First part of saved string, category</param>
        /// <param name="m2">Second part of saved string, details</param>
        /// <param name="mO">Message output type</param>
        /// <param name="hue">Text color</param>
        /// <param name="duration">Length of timer</param>
        public static bool CheckTimerRenew(int mS, string m1 = "", string m2 = "", MessageOutput mO = MessageOutput.None, int hue = 33, int duration = 4000)
        {
	        var result = Timer.Check(m1 + m2 + mS);
	        Timer.Create(m1 + m2 + mS, duration);
	        if (result) return true;
	        switch (mO)
	        {
		        case MessageOutput.Mobile:
			        Mobiles.Message(mS, hue, m2);
			        break;
		        case MessageOutput.Player:
			        Player.HeadMessage(hue,  m2);
			        break;
		        case MessageOutput.Misc:
			        Misc.SendMessage(m1, hue);
			        Misc.SendMessage(m2, hue);
			        break;
		        //case MessageOutput.None:
	        }
	        return false;
        }

        /// <summary>
        /// Enum for message output types
        /// </summary>
        public enum MessageOutput
        {
	        None,
	        Debug,
	        Misc,
	        Mobile,
	        Player,
        }  
        
        public static string GetCallingClassName()
        {
	        // Get the stack trace
	        var stackTrace = new StackTrace();
        
	        // Get the frame for the method that called this one (2nd frame in the stack trace)
	        var frame = stackTrace.GetFrame(2);

	        // Get the declaring type (class) of the calling method
	        var method = frame.GetMethod();
	        var callingClass = method.DeclaringType;

	        return callingClass?.FullName ?? "<Unknown>";
        }

        public static Stopwatch stopwatch = new Stopwatch();

        /*
        public static CancellationToken GetLinkedToken(ref CancellationTokenSource main, ref CancellationTokenSource sub)
        {
	        if (sub == null)
	        {
		        // Initialize the sub token source if it doesn't already exist
		        sub = new DebugCancellationTokenSource();
	        }

	        if (main == null)
	        {
		        UoTLogger.LogErrorToFile("Main token source is null.");
		        return sub.Token; // Return the sub token since main is null.
	        }

	        // Dispose of any previously linked token sources (if necessary)
	        CancellationTokenSource oldMain = main;

	        // Create a new linked token source incorporating both main and sub tokens
	        main = CancellationTokenSource.CreateLinkedTokenSource(
		        oldMain.Token,
		        sub.Token
	        );

	        // Dispose the old main token source to prevent memory leaks
	        oldMain.Dispose();

	        return main.Token;
        }*/
    }
}