using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.SimpleRegistry
{
    // public delegate T SelectionStrategy<T>(IEnumerable<T> items);

    public static class Registry<T> where T : class
    {
        static readonly HashSet<T> Items = new();

        public static bool TryAdd(T item)
        {
            return item != null && Items.Add(item);
        }

        public static bool Remove(T item)
        {
            return Items.Remove(item);
        }

        public static void Clear()
        {
            Items.Clear();
        }

        public static T GetAny()
        {
            return Items.FirstOrDefault();
        }

        public static T Get(Func<IEnumerable<T>, T> selector)
        {
            return selector(Items);
        }

        // public static T Get(SelectionStrategy<T> strategy) => strategy(items);

        public static IEnumerable<T> All => Items;
    }
}