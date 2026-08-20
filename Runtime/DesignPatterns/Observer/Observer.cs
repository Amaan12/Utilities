using System;
using UnityEngine;

namespace Utilities.Observer
{
    [Serializable]

    public class Observer<T>
    {
        [SerializeField] T value;
        public event Action<T> OnValueChanged;

        public T Value
        {
            get => value;
            set => Set(value);
        }

        public static implicit operator T(Observer<T> observer) => observer.value;

        public Observer(T initialValue)
        {
            value = initialValue;
        }

        void Set(T newValue)
        {
            if (Equals(value, newValue))
                return;

            value = newValue;
            OnValueChanged?.Invoke(value);
        }
    }
}


