using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public enum GameType
{
    NONE = 0,
    CUBE
}

public class GameSceneController : Singleton<GameSceneController>
{
    GameType prevGameType = GameType.NONE;

    private static class Constants
    {
        public static string EmptySceneName = "InitScene";
        public static string CubeSceneName = "cubegame";
    }

    #region Life Cycle
    private void OnEnable()
    {
        
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        
        // Register Callback
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unregister Callback
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start() {
#if UNITY_ANDROID
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;

            Debug.Log("Start");
            Screen.fullScreen = false;
            // To get the EGLContext on the native side
            GL.IssuePluginEvent(AndroidNativeAPI.GetRenderEventFunc(), 0);
#endif

    }

    void Update()
    {
        
    }
    #endregion

    #region SceneEvent Callback
    private void OnSceneLoaded(Scene current, LoadSceneMode mode)
    {
        Debug.Log("[Bill] OnSceneLoaded");
        GameType gameType = CurrentGameType();
        Debug.Log("Success Loaded: <" + current.name + ">");
        SceneManager.SetActiveScene(current);

        bool initSceneDidLoaded = (gameType == GameType.NONE && current.name == Constants.EmptySceneName);
        if (initSceneDidLoaded)
        {
            if (prevGameType == GameType.NONE)
            {
                return;
            }
            NotifyNativeGameUnload((int)prevGameType);
        } else
        {
            NotifyNativeGameLoad((int)gameType);
        }
        
        prevGameType = gameType;
    }
    #endregion

    #region Private Functions
    private GameType CurrentGameType()
    {
        var gameObject = GameObject.FindGameObjectWithTag("GameDescriber");
        
        var type = gameObject.GetComponent<GameDescriber>().GameType;
        Debug.Log("Current GameType is " + type);

        return type;
    }

    private void UnloadScene()
    {
#if UNITY_ANDROID
        // Clear the texture because the texture contains black content
        // between the scene unloaded and the native unload callback called.
        if (Application.platform == RuntimePlatform.Android) 
        {
            AndroidNativeAPI.SetTexturePtrFromUnity(System.IntPtr.Zero);
        }
#endif        
        /// Load EmptyScene to unload actived scene.
        SceneManager.LoadSceneAsync(Constants.EmptySceneName);
    }

    private string GetSceneNameByGameType(GameType type)
    {
        string result = Constants.EmptySceneName;
        switch (type)
        {
            case GameType.NONE: break;
            case GameType.CUBE:
                result = Constants.CubeSceneName;
                break;
        }

        return result;
    }
    #endregion

    private void NotifyNativeGameLoad(int gameType) 
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidNativeAPI.OnGameLoad(gameType);
        }
#elif UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            iOSNativeAPI.DidLoadGame(gameType);
        }
#endif
    }

    private void NotifyNativeGameUnload(int gameType) 
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidNativeAPI.OnGameUnload(gameType);
        }
#elif UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            iOSNativeAPI.DidUnloadGame(gameType);
        }
#endif
    }

    #region Public Functions
    public void UpdateScoreToNative(int score)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidNativeAPI.OnScoreUpdate(score);
        }
#elif UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            iOSNativeAPI.UpdateScore(score);
        }
#endif
    }

    public void EndGame()
    {
        if (SceneManager.GetActiveScene().name == Constants.EmptySceneName)
        {
            Debug.Log("Sould Not Unload Init Scene");
            return;
        }

        Debug.Log("End Game, SceneName : " + SceneManager.GetActiveScene().name);
        UnloadScene();
    }

    #endregion

    #region Called by Native
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
    public void StartGame(string gameStartJsonString)
    {
        var json = SimpleJSON.JSON.Parse(gameStartJsonString);

        var gameType = json["gameType"];
        var locale = json["locale"];

        Localization.Language lang = (Localization.Language)Enum.Parse(typeof(Localization.Language), locale);
        Services.Localization.CurrentLanguage = lang;

        GameType type = (GameType)int.Parse(gameType);
        string sceneName = GetSceneNameByGameType(type);
        Debug.Log("currentType = " + type);
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void CloseGame()
    {
        UnloadScene();
        Debug.Log("[Unity] Close Game by Native");
    }
#endif
    #endregion
}
