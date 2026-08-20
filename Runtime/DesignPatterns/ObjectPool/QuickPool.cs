using UnityEngine;
using UnityEngine.Pool;

namespace Utilities.QuickPool
{
    public class QuickPool<T> where T : Component
    {
        ObjectPool<T> pool;
        T prefab;
        Transform parent;

        public QuickPool(T prefab, int defaultCapacity = 10, int maxSize = 100, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            pool = new ObjectPool<T>(
                Create,
                OnGet,
                OnRelease,
                OnDestroy,
                true,
                defaultCapacity,
                maxSize
            );
        }

        T Create()
        {
            var obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            return obj;
        }

        void OnGet(T obj)
        {
            obj.gameObject.SetActive(true);
        }

        void OnRelease(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        void OnDestroy(T obj)
        {
            Object.Destroy(obj.gameObject);
        }

        public T Get()
        {
            return pool.Get();
        }

        public void Release(T obj)
        {
            pool.Release(obj);
        }

        public int CountInactive => pool.CountInactive;

        public void Clear()
        {
            pool.Clear();
        }
    }
}