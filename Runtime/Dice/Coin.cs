using UnityEngine;
using System.Collections;

namespace Utilities.Coin
{
    /// <summary>
    /// A usable coin for flips.
    /// It could be made more modular to support dice.
    /// </summary>
    public class Coin : MonoBehaviour
    {
        Rigidbody rb;
        public bool flipping = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        [ContextMenu("Flip Coin")]
        public void Flip(float upwardForce = 5f, float torqueAmount = 10f)
        {
            if (flipping || rb == null)
            {
                return;
            }
            
            StartCoroutine(FlipRoutine(upwardForce, torqueAmount));
        }

        IEnumerator FlipRoutine(float upwardForce, float torqueAmount)
        {
            flipping = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Add upward force
            rb.AddForce(Vector3.up * (upwardForce + Random.Range(0f, 2f)), ForceMode.Impulse);

            // Add random torque
            float torqueX;
            torqueX = Random.Range(1f, 5f);
            // if (Random.value < 0.5f) torqueX = Random.Range(0.5f, 1.25f);   // positive range
            // else torqueX = Random.Range(-1.25f, -0.5f); // negative range

            Vector3 randomTorque = new Vector3(torqueX, 0f, 0f) * torqueAmount;
            // Vector3 randomTorque = new Vector3(
            //     Random.Range(-1f, 1f),
            //     Random.Range(-1f, 1f),
            //     Random.Range(-1f, 1f)
            // ) * torqueAmount;
            rb.AddTorque(randomTorque, ForceMode.Impulse);

            // Wait until it's stationary
            yield return new WaitUntil(() => rb.IsSleeping());

            flipping = false;

            bool isUpright = Vector3.Dot(transform.up, Vector3.up) > 0.8f;
            // Debug.Log("Landed upright? " + isUpright);

            yield return new WaitForSeconds(2f);
            gameObject.SetActive(false);
        }

        public bool IsUpright()
        {
            return Vector3.Dot(transform.up, Vector3.up) > 0.8f;
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
