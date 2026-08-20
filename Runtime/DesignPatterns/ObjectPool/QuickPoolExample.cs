using System.Collections;
using UnityEngine;

namespace Utilities.QuickPool
{
    public class QuickPoolExample : MonoBehaviour
    {
        [SerializeField] Bullet bulletPrefab;
        [SerializeField] Transform firePoint;

        QuickPool<Bullet> bulletPool;

        void Awake()
        {
            bulletPool = new QuickPool<Bullet>(bulletPrefab, 20, 100);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpawnBullet();
            }
        }

        void SpawnBullet()
        {
            var bullet = bulletPool.Get();

            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;

            // Example auto return
            StartCoroutine(ReturnAfterTime(bullet, 2f));
        }

        IEnumerator ReturnAfterTime(Bullet bullet, float time)
        {
            yield return new WaitForSeconds(time);
            bulletPool.Release(bullet);
        }
    }
}