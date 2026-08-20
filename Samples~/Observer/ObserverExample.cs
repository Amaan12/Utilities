using UnityEngine;
using UnityEngine.InputSystem;

namespace Utilities.Observer
{
    public class ObserverExample : MonoBehaviour
    {
        Observer<int> Health = new Observer<int>(100);

        void OnEnable()
        {
            Health.OnValueChanged += Health_OnValueChanged;
        }

        void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Health.Value += 100;
            }
        }
        
        void Health_OnValueChanged(int value)
        {
            Debug.Log($"Health: {value}");
        }

        void OnDisable()
        {
            Health.OnValueChanged -= Health_OnValueChanged;
        }
    }
}