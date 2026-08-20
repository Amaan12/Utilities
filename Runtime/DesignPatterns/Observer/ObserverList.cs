using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Observer
{
    [Serializable]
    public class ObserverList<T> : IEnumerable<T>
    {
        [SerializeField] List<T> list = new List<T>();

        public event Action<T> OnAdded;
        public event Action<T> OnRemoved;
        public event Action OnCleared;
        public event Action OnChanged;

        public int Count => list.Count;

        public T this[int index]
        {
            get => list[index];
            set => Set(index, value);
        }

        public ObserverList() { }

        public ObserverList(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
        }

        public void Add(T item)
        {
            list.Add(item);

            OnAdded?.Invoke(item);
            OnChanged?.Invoke();
        }

        public bool Remove(T item)
        {
            int index = list.IndexOf(item);

            if (index < 0)
                return false;

            list.RemoveAt(index);

            OnRemoved?.Invoke(item);
            OnChanged?.Invoke();

            return true;
        }

        public void RemoveAt(int index)
        {
            T item = list[index];

            list.RemoveAt(index);

            OnRemoved?.Invoke(item);
            OnChanged?.Invoke();
        }

        public void Clear()
        {
            if (list.Count == 0)
                return;

            list.Clear();

            OnCleared?.Invoke();
            OnChanged?.Invoke();
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);

            OnAdded?.Invoke(item);
            OnChanged?.Invoke();
        }

        public bool Contains(T item) => list.Contains(item);

        public int IndexOf(T item) => list.IndexOf(item);

        public void Sort(Comparison<T> comparison)
        {
            list.Sort(comparison);
            OnChanged?.Invoke();
        }

        void Set(int index, T newItem)
        {
            T oldItem = list[index];

            if (Equals(oldItem, newItem))
                return;

            list[index] = newItem;

            OnRemoved?.Invoke(oldItem);
            OnAdded?.Invoke(newItem);
            OnChanged?.Invoke();
        }

        public List<T> ToList() => new List<T>(list);

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}