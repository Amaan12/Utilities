using UnityEngine;

namespace Utilities.SimpleSingleton
{
    /// <summary>
    /// Generic singleton, that has an optional dontDestroyOnLoad.
    /// Now we could use git-amend's but he's broken it down into 2 classes, which I didn't like.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour // Syntax for declaring generic type class.
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        instance = singleton.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        [SerializeField] private bool dontDestroyOnLoad = false;
        public virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                if (dontDestroyOnLoad && Application.isPlaying)
                {
                    transform.parent = null;
                    DontDestroyOnLoad(gameObject); // this method only works if the gameObject is a root object (doesn't have any parent)
                }
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        public virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}

namespace SimpleSingleton
{
    // Kept for backward compatibility
    public abstract class Singleton<T> : Utilities.SimpleSingleton.Singleton<T> where T : MonoBehaviour
    {
    }
}
