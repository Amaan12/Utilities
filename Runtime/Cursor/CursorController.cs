using System.Collections.Generic;
using UnityEngine;

namespace Utilities.CursorControl
{
    public class CursorController : MonoBehaviour
    {
        [Header("Initial Cursor State")]
        [SerializeField] CursorLockMode initialLockMode = CursorLockMode.Locked;
        [SerializeField] bool initialVisible = false;

        private static List<object> requestStack = new List<object>();
        private static CursorLockMode defaultLockMode = CursorLockMode.Locked;
        private static bool defaultVisible = false;

        void Awake()
        {
            defaultLockMode = initialLockMode;
            defaultVisible = initialVisible;
        }

        void OnEnable()
        {
            UpdateCursorState();
        }

        public static void RequestCursor(object requester)
        {
            if (requester == null) return;
            if (!requestStack.Contains(requester))
            {
                requestStack.Add(requester);
            }
            UpdateCursorState();
        }

        public static void ReleaseCursor(object requester)
        {
            if (requester == null) return;
            requestStack.Remove(requester);
            UpdateCursorState();
        }

        public static void ClearStack()
        {
            requestStack.Clear();
        }

        public static void DisableCursorGlobal()
        {
            ClearStack();
            ApplyState(defaultLockMode, defaultVisible);
        }

        public static void EnableCursorGlobal()
        {
            ClearStack();
            ApplyState(CursorLockMode.None, true);
        }

        private static void UpdateCursorState()
        {
            if (requestStack.Count > 0)
            {
                ApplyState(CursorLockMode.None, true);
            }
            else
            {
                ApplyState(defaultLockMode, defaultVisible);
            }
        }

        public static void ApplyState(CursorLockMode mode, bool isVisible)
        {
            Cursor.lockState = mode;
            Cursor.visible = isVisible;
        }

        // Keep compatibility helpers but base them on a generic static key
        private static readonly object fallbackKey = new object();

        public static void DisableCursor()
        {
            ReleaseCursor(fallbackKey);
        }

        public static void EnableCursor()
        {
            RequestCursor(fallbackKey);
        }
    }
}
