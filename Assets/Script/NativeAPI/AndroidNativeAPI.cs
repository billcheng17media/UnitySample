#if UNITY_ANDROID
using System;
using System.Runtime.InteropServices;

public class AndroidNativeAPI
{
    #region Render
    [DllImport ("NativeUnityPlugin")]
    public static extern void SetTexturePtrFromUnity(System.IntPtr texturePtr);
    [DllImport ("NativeUnityPlugin")]
    public static extern IntPtr GetRenderEventFunc();
    #endregion

    #region Game
    [DllImport ("NativeUnityPlugin")]
    public static extern void OnGameLoad(int gameType);
    [DllImport ("NativeUnityPlugin")]
    public static extern void OnGameUnload(int gameType);
    [DllImport ("NativeUnityPlugin")]
    public static extern void OnScoreUpdate(int score);
    #endregion
}
#endif
