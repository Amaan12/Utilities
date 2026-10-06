using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelManagement.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasCameraAssigner : MonoBehaviour
    {
        Canvas canvas;
        [SerializeField] float planeDistance = 5f;

        void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        void OnEnable()
        {
            UpdateCanvasCamera();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            UpdateCanvasCamera();
        }

        /// <summary>
        /// Plane Distance can't be set when Canvas is screen space.
        /// This finds the current active camera whenever a scene is changed.
        /// </summary>
        void UpdateCanvasCamera()
        {
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = planeDistance;
        }
    }
}