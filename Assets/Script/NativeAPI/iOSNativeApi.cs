using System;
using System.Runtime.InteropServices;

#if UNITY_IOS || UNITY_TVOS
public class iOSNativeAPI
{
    #region Common
    [DllImport("__Internal")]
    public static extern void DidLoadScene(string sceneName);
    [DllImport("__Internal")]
    public static extern void DidUnloadScene(string sceneName);
    #endregion

    #region Render
    [DllImport("__Internal")]
    public static extern void SetRenderTextureFromUnity(System.IntPtr texture);
    #endregion

    #region Game
    [DllImport("__Internal")]
    public static extern void DidLoadGame(int gameType);
    [DllImport("__Internal")]
    public static extern void DidUnloadGame(int gameType);
    [DllImport("__Internal")]
    public static extern void UpdateScore(int score);
    #endregion
}
#endif
