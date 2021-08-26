using UnityEngine;

public class SceneSingletonDestroyableOnLoad<T> : MonoBehaviour where T : SceneSingletonDestroyableOnLoad<T>
{
    private static T _instance = null;
    public static T Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<T>();
                // fallback, might not be necessary.
                if (_instance == null) {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
