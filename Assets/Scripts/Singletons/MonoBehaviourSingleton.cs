using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (!_canBeAccessed)
                return null;
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<MonoBehaviourSingleton<T>>();
                if (_instance == null)
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>() as MonoBehaviourSingleton<T>;
            }
            _instance.Awake();
            return _instance as T;
        }
    }

    private static MonoBehaviourSingleton<T> _instance;

    private static bool _wasInitialized;
    private static bool _canBeAccessed = true;

    protected virtual bool Awake()
    {
        if ( _wasInitialized)
            return false;
        if (_instance != this)
        {
            if (_instance == null)
                _instance = this;
            else
            {
                Destroy(gameObject);
                return false;
            }
        }
        return _wasInitialized = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _canBeAccessed = false;
    }
}
