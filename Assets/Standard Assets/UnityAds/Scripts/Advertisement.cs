namespace UnityEngine.Advertisements {
  using System;

  public static class Advertisement {

  	public static readonly string version = "1.1.4";

    public enum DebugLevel {
      NONE = 0,
      ERROR = 1,
      WARNING = 2,
      INFO = 4,
      DEBUG = 8
    }
	
    static private DebugLevel _debugLevel = Debug.isDebugBuild ? DebugLevel.ERROR | DebugLevel.WARNING | DebugLevel.INFO | DebugLevel.DEBUG : DebugLevel.ERROR | DebugLevel.WARNING | DebugLevel.INFO;
	
    static public DebugLevel debugLevel {
      get {
        return _debugLevel;
      }
	
      set {
        _debugLevel = value;
#if UNITY_ANDROID || UNITY_IOS
        UnityEngine.Advertisements.UnityAds.setLogLevel(_debugLevel);
#endif
      }
    }

    static public bool isSupported {
      get {
        return 
          Application.isEditor ||
          Application.platform == RuntimePlatform.IPhonePlayer || 
          Application.platform == RuntimePlatform.Android;
      }
    }

    static public bool isInitialized {
      get {
#if UNITY_ANDROID || UNITY_IOS
        return UnityAds.isInitialized;
#else
        return false;
#endif
      }
    }

    static public void Initialize(string appId, bool testMode = false) {
#if UNITY_ANDROID || UNITY_IOS
      UnityAds.SharedInstance.Init(appId, testMode);
#endif
    }

    static public void Show(string zoneId = null, ShowOptions options = null) {
#if UNITY_ANDROID || UNITY_IOS
      UnityAds.SharedInstance.Show(zoneId, options);
#else
      if(options != null && options.resultCallback != null) {
        options.resultCallback(ShowResult.Failed);
      }
#endif
    }

    static public bool allowPrecache { 
      get {
#if UNITY_ANDROID || UNITY_IOS
        return UnityAds.allowPrecache;
#else
        return false;
#endif
      }
      set {
#if UNITY_ANDROID || UNITY_IOS
        UnityAds.allowPrecache = value;
#endif
      }
    }

    static public bool isReady(string zoneId = null) {
#if UNITY_ANDROID || UNITY_IOS
      return UnityAds.canShowZone(zoneId);
#else
      return false;
#endif
    }

    static public bool isShowing { 
      get {
#if UNITY_ANDROID || UNITY_IOS
        return UnityAds.isShowing;
#else
        return false;
#endif
      }
    }

    static public bool UnityDeveloperInternalTestMode {
      get; set;
    }

  }

}
