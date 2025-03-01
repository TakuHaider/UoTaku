using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


// Reminder on differences between .clear and new List assignment
// Clear resets the data, not the object. This can cause thread issues.
// Clear ensures the list is no longer accessed by other methods or threads
// Clear prevents open threads from using stale list objects.
// Expect InvalidOperationException and possible race conditions during list access from .Clear
// Clearing lists of open threads before they are properly awaited, canceled, or disposed may cause issues.
// The tasks themselves aren't yet stopped—you're just removing their references from the list.
// This could lead to tasks being improperly finalized or even forgotten.
// Safely clear and dispose any other shared resources 


namespace RazorEnhanced
{
	/// <summary>
	/// Provides an infrastructure for managing and monitoring asynchronous tasks using a thread-safe
	/// dictionary. Features include task lifecycle management, cancellation handling, and debugging tools.
	/// </summary>
	public class UoTasks
	{
		// Thread-safe collection for tracking ongoing tasks
	    private static readonly ConcurrentDictionary<string, TaskInfo> _tasks
	        = new ();
	    /// <summary>
	    /// Represents information about a single tracked task, such as its status, task object, and cancellation handling.
	    /// </summary>
	    public class TaskInfo
	    {
		    /// <summary>
		    /// The asynchronous task being tracked.
		    /// </summary>
	        public Task Task { get; set; }
		    
		    /// <summary>
		    /// Retrieves the cancellation token for this task.
		    /// </summary>
		    public CancellationToken Token => CancelSource.Token;
		    
		    /// <summary>
		    /// A cancellation token source for managing the lifecycle of the task.
		    /// </summary>
	        public CancellationTokenSource CancelSource;
	        
	        /// <summary>
	        /// Initializes a new instance of the <c>TaskInfo</c> class.
	        /// </summary>
	        public TaskInfo()
	        {
		        CancelSource =  new CancellationTokenSource();
	        }
		    /// <summary>
		    /// Cancels the task.
		    /// </summary>
		    /// <returns>True if cancellation was successful, false otherwise.</returns>
		    public void Cancel()
		    {
			    var tokenSourceToCancel = CancelSource;
			    try
			    {
				    if (tokenSourceToCancel == null || tokenSourceToCancel.Token == null)
				    {
						UoTLogger.LogErrorToFile($"Task is already canceled or disposed.");
					    return;
				    }
				    if (!tokenSourceToCancel.Token.IsCancellationRequested)
				    {
					    UoTLogger.LogErrorToFile($"Signaling cancellation of task.");
					    tokenSourceToCancel.Cancel(); 
				    }
			    }
			    catch (Exception ex)
			    {
				    UoTLogger.LogErrorToFile($"Error in Cancel(): {ex.Message}");
			    }
		    }

		    /// <summary>
		    /// Determines whether the task is currently running, !_.Iscompleted.
		    /// </summary>
		    public bool IsRunning => Task != null && !Task.IsCompleted;
		    
		    /// <summary>
		    /// Checks whether cancellation has been requested for this task or globally.
		    /// </summary>
		    public bool IsCancellationRequested => 
			    (MainCancellationTokenSource?.Token.IsCancellationRequested ?? false) ||
			    (CancelSource?.Token.IsCancellationRequested ?? false);


		}
	    
	    /// <summary>
	    /// A synchronization object used to ensure thread-safe operations on the
	    /// underlying tasks collection (e.g., adding, removing, or modifying tasks).
	    /// </summary>
	    private static readonly object TasksLock = new ();
	    
	    /// <summary>
	    /// A global CancellationTokenSource used to manage cancellation across all tasks.
	    /// </summary>
	    public static CancellationTokenSource MainCancellationTokenSource = new DebugCancellationTokenSource();

	    /// <summary>
	    /// Cancels and clears all tracked tasks. Resets the global CancellationTokenSource.
	    /// Should be called before starting new tasks to ensure the system is in a clean state.
	    /// </summary>
	    public static void InitializeTasks()
	    {
		    try
		    {
			    var taskCache = _tasks.ToArray();
			    foreach (var taskInfo in _tasks.Values.ToArray())  // ToArray creates a snapshot for safe iteration
			    {
				    taskInfo.Cancel(); // this calls Cancel on the linked token source
			    }
			    _tasks.Clear();
			    MainCancellationTokenSource?.Cancel();
			    MainCancellationTokenSource?.Dispose();
			    MainCancellationTokenSource = new DebugCancellationTokenSource();
			    UoTLogger.LogErrorToFile("UoTasks initialized successfully.");
		    }
		    catch (Exception ex)
		    {
			    UoTLogger.LogErrorToFile($"Error initializing UoTasks: {ex.Message}");
		    }
	    }

	    /// <summary>
	    /// A custom cancellation token source used for debugging. Logs a stack trace whenever cancellation is triggered.
	    /// </summary>
	    public class DebugCancellationTokenSource : CancellationTokenSource
	    {
		    /// <summary>
		    /// Cancels the token source and logs a stack trace for debugging purposes.
		    /// </summary>
		    public new void Cancel()
		    {
			    var stackTrace = Environment.StackTrace;
			    UoTLogger.LogErrorToFile($"Cancellation triggered. Stack trace: {stackTrace}");
			    base.Cancel();
		    }
	    }
	    /// <summary>
	    /// Cancels all running tasks, disposes of the global CancellationTokenSource, and cleans up resources.
	    /// This method is designed for proper cleanup during application shutdown.
	    /// </summary>
	    public static void Shutdown()
	    {
		    try
		    {
			    var stackTrace = Environment.StackTrace;
			    UoTLogger.LogErrorToFile($"Shutdown() triggered. Stack trace: {stackTrace}");
			    MainCancellationTokenSource.Cancel();
			    foreach (var taskName in _tasks.Keys.ToArray())
			    {
				    StopTask(taskName);
			    }

			    // Dispose of the main CancellationTokenSource
			    MainCancellationTokenSource.Dispose();
			    MainCancellationTokenSource = null;
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine("Exception in Stop(): " + ex.Message);
			    // Log and handle other exceptions to ensure stability.
		    }
	    }
	    /// <summary>
	    /// Starts a named asynchronous task if there isn't one already running with the same name.
	    /// Ensures that tasks with unique names can be managed independently.
	    /// </summary>
	    /// <param name="taskName">Unique identifier for the task (e.g., "pathing", "combat").</param>
	    /// <param name="taskAction">The asynchronous function to execute for the task.</param>
	    /// <returns>
	    /// True if the task starts successfully. Returns false if an error occurs 
	    /// or if a task is already running under the specified name.
	    /// </returns>
	    public static bool StartTask(string taskName, Func<CancellationToken, Task> taskAction)
	    {
		    try
		    {
			    TaskInfo existingTaskInfo = null;
			    TaskInfo newTaskInfo = null;
			    if (_tasks.TryGetValue(taskName, out existingTaskInfo) && existingTaskInfo != null)
			    {
				    if (existingTaskInfo.IsRunning)
				    {
					    //UoTLogger.LogErrorToFile($"Task '{taskName}' is still running and cannot be started again.");
					    return false;
				    }
				    if (existingTaskInfo.Task != null && 
				        (!existingTaskInfo.Task.IsCompleted || 
				         existingTaskInfo.Task.Status == TaskStatus.WaitingToRun || 
				         existingTaskInfo.Task.Status == TaskStatus.Created || 
				         existingTaskInfo.Task.Status == TaskStatus.WaitingForActivation))
				    {
					    // Prevent starting a new task if the existing one is still running
					    UoTLogger.LogErrorToFile($"Task '{taskName}' is !IsCompleted and cannot be started again.");
					    return false;
				    }
				    // Force new task?
				    //if (existingTaskInfo._cancelSource.Token.CanBeCanceled) existingTaskInfo.CancelAndDispose();
				    //else return false;
			    }
			   // Create the new TaskInfo
			    newTaskInfo = new TaskInfo();
	    
			    // Create new TaskInfo if none exists
			    if (!_tasks.TryAdd(taskName, newTaskInfo))
			    {
				    UoTLogger.LogErrorToFile($"Failed to add task '{taskName}' to the task list.");
				    return false;
			    }
		    
			    // Create linked token source
			    var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(
				    newTaskInfo.CancelSource.Token, 
				    MainCancellationTokenSource.Token
			    );

			    newTaskInfo.Task = Task.Run(async () =>
			    {
				    try
				    {
					    await taskAction(linkedSource.Token);
				    }
				    catch (Exception ex)
				    {
					    UoTLogger.LogErrorToFile($"StartTask encountered an exception: {ex}");
					    //throw; 
				    }
			    }, linkedSource.Token);
			    newTaskInfo.Task.ContinueWith(t =>
				{
					try
					{
						linkedSource.Cancel();
						switch (t.Status)
						{
							case TaskStatus.RanToCompletion:
								UoTLogger.LogErrorToFile($"Task '{taskName}' completed successfully.");
								break;
							case TaskStatus.Faulted:
								UoTLogger.LogErrorToFile($"Task '{taskName}' faulted: {t.Exception?.InnerException?.Message}");
								break;
							case TaskStatus.Canceled:
								UoTLogger.LogErrorToFile($"Task '{taskName}' was canceled.");
								break;
							default:
								UoTLogger.LogErrorToFile($"Task '{taskName}' finished in unexpected state: {t.Status}");
								break;
						}
						linkedSource.Dispose();
						lock (TasksLock)
							if (_tasks.TryRemove(taskName, out var completedTaskInfo))
							{
								UoTLogger.LogErrorToFile($"Task '{taskName}' has been removed from the task list.");
							}
					}
					catch (Exception ex)
					{
						UoTLogger.LogErrorToFile($"Error in TaskManager.StartTask.ContinueWith: {ex.Message}");
					}
			    } , TaskContinuationOptions.ExecuteSynchronously);
		
			    return true;
		    }
		    catch (Exception ex)
		    {
			    UoTLogger.LogErrorToFile($"Unexpected exception in StartTask: {ex}");
			   // return false;
		    }

		    return true;
	    }


	    /// <summary>
	    /// Stops and cancels all tasks currently tracked by <c>UoTasks</c>.
	    /// </summary>
	    public static void StopAllTasks()
	    {
		    // Make a snapshot of the task names
		    var taskNames = _tasks.Keys.ToArray();
    
		    // Cancel each one in turn
		    foreach (var taskName in taskNames)
		    {
			    StopTask(taskName);
		    }
	    }

	    /// <summary>
	    /// Attempts to stop (cancel) a named task via its CancellationToken.
	    /// </summary>
	    /// <param name="taskName">The name of the task to be canceled.</param>
	    /// <returns>
	    /// True if the task was successfully canceled; false otherwise.
	    /// </returns>

	    public static bool StopTask(string taskName)
	    {
		    try
		    {
			    TaskInfo taskInfo;
			    if (!_tasks.TryGetValue(taskName, out taskInfo))
			    {
				    //UoTLogger.LogErrorToFile($"StopTask: Task '{taskName}' was not in _tasks.");
				    return false;
			    }
			    if (taskInfo == null)
			    {
				    UoTLogger.LogErrorToFile($"StopTask: Task '{taskName}'s taskInfo was null");
				    return false;
			    }
			    UoTLogger.LogErrorToFile($"StopTask: Task Canceling '{taskName}'");
			    taskInfo.Cancel();
			    return true;
		    }
		    catch (Exception e)
		    {
			    UoTLogger.LogErrorToFile($"Error stopping task: {e.Message}");
			    return false;
		    }
	    }

	    /// <summary>
	    /// Checks whether a particular task is running (by name).
	    /// </summary>
	    /// <param name="taskName">The unique name of the task to check.</param>
	    /// <returns>
	    /// True if the task exists and is actively running; false otherwise.
	    /// </returns>

	    public static bool IsTaskRunning(string taskName)
	    {
		    // Weird but apparently local cache needed. 
		    if (_tasks.TryGetValue(taskName, out var taskInfo))
		    {
			    // Might want to switch for lock on taskInfo.IsRunning and Cancel()
			    var isRunning = taskInfo != null && taskInfo.IsRunning;
			    return isRunning;
		    };
		    return false;
	    }
	    /// <summary>
	    /// Checks whether cancellation has been requested for a specific task or globally.
	    /// </summary>
	    /// <param name="taskName">
	    /// The name of the task to check (optional). If no name is provided, the method checks the 
	    /// global cancellation flag.
	    /// </param>
	    /// <returns>
	    /// True if cancellation has been requested; false otherwise.
	    /// </returns>
	    public static bool IsCancellationRequested(string taskName = default)
	    {
		    try
		    {
			    if (taskName == default) return MainCancellationTokenSource.Token.IsCancellationRequested;
			    if (_tasks.TryGetValue(taskName, out var taskInfo))
			    {
				    if (taskInfo == null) return false;
				    return taskInfo.IsCancellationRequested ||
				           MainCancellationTokenSource.Token.IsCancellationRequested;
			    }
			    return false;
		    }
		    catch (Exception e)
		    {
			    UoTLogger.LogErrorToFile($"Error checking cancellation: {e.Message}");
		    }
		    return false;
	    }
	    /// <summary>
	    /// Retrieves the <see cref="CancellationToken"/> for a specific task. If the task name is not 
	    /// specified, the global cancellation token is returned.
	    /// </summary>
	    /// <param name="taskName">
	    /// The name of the task to retrieve the token for (optional).
	    /// </param>
	    /// <returns>
	    /// The <see cref="CancellationToken"/> for the task if it exists, or <see cref="CancellationToken.None"/> otherwise.
	    /// </returns>
	    public static CancellationToken GetTaskCancellationToken(string taskName = default)
	    {
		    try
		    {
			    if (string.IsNullOrEmpty(taskName))
			    {
				    var tokenSource = GetTaskCancellationSource();
				    if (tokenSource.Token == null) return CancellationToken.None;
				    return tokenSource.Token;
			    }
			    return _tasks.TryGetValue(taskName, out var taskInfo) ? taskInfo?.Token ?? CancellationToken.None : CancellationToken.None;
		    }
		    catch (Exception e)
		    {
			    var stackTrace = Environment.StackTrace; 

			    UoTLogger.LogErrorToFile($"Error getting task token: {e.Message}");
			    UoTLogger.LogErrorToFile($"Stack trace: {stackTrace}");
		    }
		    return CancellationToken.None;
	    }
	    /// <summary>
	    /// Retrieves the <see cref="CancellationTokenSource"/> for a specific task, or the global 
	    /// cancellation token source when no task name is provided.
	    /// </summary>
	    /// <param name="taskName">The name of the task (optional).</param>
	    /// <returns>
	    /// The <see cref="CancellationTokenSource"/> for the specified task, or the global token source if no name is provided.
	    /// </returns>
	    public static CancellationTokenSource GetTaskCancellationSource(string taskName = default)
	    {
		    if (taskName == default)
		    {
			    if (MainCancellationTokenSource == null || MainCancellationTokenSource.IsCancellationRequested)
			    {
				    MainCancellationTokenSource = new DebugCancellationTokenSource(); // Renew if null or canceled
			    }
			    return MainCancellationTokenSource;
		    }
		    if (_tasks.TryGetValue(taskName, out var taskInfo))
		    {
			    if (taskInfo == null) return null;
			    return taskInfo.CancelSource;
		    }
		    UoTLogger.LogErrorToFile($"Task '{taskName}' does not exist.");
		    return null;
	    }
	    
	    /// <summary>
	    /// Retrieves detailed information about a specific task by its name.
	    /// </summary>
	    /// <param name="taskName">The name of the task.</param>
	    /// <returns>
	    /// A <see cref="TaskInfo"/> object containing details about the task, or null if the task is not found.
	    /// </returns>
	    public static TaskInfo GetTaskInfo(string taskName)
	    {
		    return _tasks.TryGetValue(taskName, out var taskInfo) ? taskInfo : null;
	    }

	    /// <summary>
	    /// Prints the statuses of all currently tracked tasks for debugging purposes.
	    /// Logs the task name and its current status (e.g., <c>Running</c>, <c>Completed</c>, <c>Canceled</c>).
	    /// </summary>
	    public static void PrintAllTaskStatuses()
	    {
		    // Needed to prevent InvalidOperationException
		    var tasksCache = _tasks.ToArray();
			    foreach (var kvp in tasksCache)
			    {
				    var taskName = kvp.Key;
				    var currentTask = kvp.Value.Task;
				    if (currentTask != null)
				    {
					    UoTLogger.LogErrorToFile($"Task '{taskName}' - Status: {currentTask.Status}");
				    }
			    }
	    }
	}

}