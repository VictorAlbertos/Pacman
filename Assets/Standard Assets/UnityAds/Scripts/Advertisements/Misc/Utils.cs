#if UNITY_ANDROID || UNITY_IOS

namespace UnityEngine.Advertisements {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using DebugLevel = Advertisement.DebugLevel;

  internal static class Utils {
    private static void Log(DebugLevel debugLevel, string message) {
      if((Advertisement.debugLevel & debugLevel) != DebugLevel.NONE) {
        Debug.Log(message);
      }
    }

    public static void LogDebug(string message) {
      Log (DebugLevel.DEBUG,"Debug: " + message);
    }
    
    public static void LogInfo(string message) {
      Log (DebugLevel.INFO, "Info:" + message);
    }
    
    public static void LogWarning(string message) {
      Log (DebugLevel.WARNING,"Warning:" + message);
    }
    
    public static void LogError(string message) {
      Log (DebugLevel.ERROR, "Error: " + message);
    }
  }
}

#endif
