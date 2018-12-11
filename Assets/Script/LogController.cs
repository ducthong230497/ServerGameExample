using System.Diagnostics;
using UnityEditor;
using UnityEngine;

// Prevent Type conflict with System.Diagnostics.Log
using Debug = UnityEngine.Debug;

public class LogController : MonoBehaviour {
	//public void Start()
 //   {
 //       string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
 //       str += ";ENABLE_LOG";
 //       PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, str);
 //       Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));
 //   }

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
