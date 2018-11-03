using System.Diagnostics;
using UnityEngine;

// Prevent Type conflict with System.Diagnostics.Log
using Debug = UnityEngine.Debug;

public class LogController : MonoBehaviour {
    [Conditional("ENABLE_LOG")]
    public static void Log(string message, string title = "")
    {
        Debug.Log(title + title == "" ? "" : ": " + message);
    }

    [Conditional("ENABLE_LOG")]
    public static void LogWarning(string message, string title = "")
    {
        Debug.LogWarning(title + title == "" ? "" : ": " + message);
    }

    [Conditional("ENABLE_LOG")]
    public static void LogError(object message, string title = "")
    {
        Debug.LogError(title + title == "" ? "" : ": " + message);
    }
}
