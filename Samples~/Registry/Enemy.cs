using UnityEngine;

namespace Utilities.SimpleRegistry.Samples
{
    public class Enemy : MonoBehaviour
    {
        void Start()
        {
            Registry<Enemy>.TryAdd(this);
        }

        void OnDestroy()
        {
            Registry<Enemy>.Remove(this);
        }
    }
}