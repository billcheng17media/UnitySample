using UnityEngine;
/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    // Check to see if we're about to be destroyed.
    private static bool _ShuttingDown = false;
    private static readonly object _Lock = new object();
    private static T _Instance;
    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance {
        get {
#if UNITY_EDITOR
            // Allow support for singletons in editor scripts
            if (!Application.isPlaying) {
                _ShuttingDown = false;
            }
#endif
            if (_ShuttingDown) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed. Returning null.");
                return null;
            }
            lock (_Lock) {
                if (_Instance == null) {
                    // Search for existing instance.
                    _Instance = (T)FindObjectOfType(typeof(T));
                    // Create new instance if one doesn't already exist.
                    if (_Instance == null) {
                        // Need to create a new GameObject to attach the singleton to.
                        GameObject singletonObject = new GameObject();
                        _Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                        // Make instance persistent.
                        if (Application.isPlaying) {
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
#if UNITY_EDITOR
                    // Allow support for singletons in editor scripts
                    if (!Application.isPlaying) {
                        System.Type t = _Instance.GetType();
                        System.Reflection.MethodInfo awake = t.GetMethod("Awake", System.Reflection.BindingFlags.Instance);
                        if (awake != null) {
                            awake.Invoke(_Instance, null);
                        }
                        System.Reflection.MethodInfo start = t.GetMethod("Start", System.Reflection.BindingFlags.Instance);
                        if (start != null) {
                            start.Invoke(_Instance, null);
                        }
                    }
#endif
                }
                return _Instance;
            }
        }
    }
    private void OnApplicationQuit()
    {
        _ShuttingDown = true;
    }
    private void OnDestroy()
    {
        _ShuttingDown = true;
    }
}
