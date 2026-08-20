using UnityEngine;

namespace Utilities.SimpleRegistry.Samples
{
    public class Player : MonoBehaviour
    {
        void Attack()
        {
            Enemy closest = Registry<Enemy>.Get(item => Selector.GetClosest(item, transform.position));
            if (closest != null)
            {
                Destroy(closest);
            }
        }
    }
}