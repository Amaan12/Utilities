using LitMotion;
using UnityEngine;
using Utilities;

namespace Utilities.Platformer
{
    [RequireComponent(typeof(AudioSource))]
    public class SpawnEffects : MonoBehaviour
    {
        [SerializeField] GameObject spawnVFX;
        [SerializeField] float animationDuration = 1f;

        void Start()
        {
            transform.localScale = Vector3.zero;
            LMotion.Create(Vector3.zero, Vector3.one, animationDuration)
                .WithEase(Ease.OutBack)
                .Bind(scale => transform.localScale = scale);

            if (spawnVFX != null)
            {
                Instantiate(spawnVFX, transform.position, Quaternion.identity);
            }


            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            GetComponent<AudioSource>().Play();
        }
    }
}