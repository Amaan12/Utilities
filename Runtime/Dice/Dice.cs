using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utilities.Dice
{
    /// <summary>
    /// Generalized dice that supports any number of sides >= 2.
    /// Side transforms represent the outward normal of each face.
    /// Index 0 = Side 1, Index 1 = Side 2, etc.
    /// </summary>
    public class Dice : MonoBehaviour
    {
        Rigidbody rb;

        [Header("State")]
        public bool rolling = false;

        [Header("Sides")]
        [Tooltip("Each transform's up vector represents that face's normal")]
        public List<Transform> sides = new List<Transform>();

        [Header("Detection")]
        [Range(0.5f, 0.999f)]
        public float uprightThreshold = 0.9f;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        [ContextMenu("Roll Dice")]
        public void Roll(float upwardForce = 5f, float torqueAmount = 10f)
        {
            if (rolling || rb == null)
            {
                return;
            }

            StartCoroutine(RollRoutine(upwardForce, torqueAmount));
        }

        IEnumerator RollRoutine(float upwardForce, float torqueAmount)
        {
            rolling = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Upward impulse
            rb.AddForce(Vector3.up * (upwardForce + Random.Range(0f, 2f)), ForceMode.Impulse);

            // Fully random torque direction
            Vector3 randomTorque = Random.onUnitSphere * Random.Range(0.5f, 1.5f) * torqueAmount;

            rb.AddTorque(randomTorque, ForceMode.Impulse);

            // Wait until stopped
            yield return new WaitUntil(() => rb.IsSleeping());

            rolling = false;

            int result = GetUpSide();

            Debug.Log("Dice result: " + result);

            yield return new WaitForSeconds(2f);

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Returns side number (1-based)
        /// Returns -1 if none found
        /// </summary>
        public int GetUpSide()
        {
            float bestDot = -1f;
            int bestIndex = -1;

            for (int i = 0; i < sides.Count; i++)
            {
                Vector3 normal = sides[i].up;

                float dot = Vector3.Dot(normal, Vector3.up);

                if (dot > uprightThreshold && dot > bestDot)
                {
                    bestDot = dot;
                    bestIndex = i;
                }
            }

            return bestIndex + 1;
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}