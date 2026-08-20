using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities.SimpleRegistry
{
    /// <summary>
    /// Using LINQ to find the
    /// 1. closest item
    /// 2. furthest item
    /// 3. list of items in a radius
    /// 4. list of nearest N items
    /// 5. random item from a list
    /// </summary>
    public static class Selector
    {
        public static T GetClosest<T>(IEnumerable<T> items, Vector3 from)
        {
            return items
                .Select(item => (item, component: item as Component))
                .Where(x => x.component != null)
                .OrderBy(x => (x.component.transform.position - from).sqrMagnitude)
                .Select(x => x.item)
                .FirstOrDefault();
        }

        public static T GetFarthest<T>(IEnumerable<T> items, Vector3 from)
        {
            return items
                .Select(item => (item, component: item as Component))
                .Where(x => x.component != null)
                .OrderByDescending(x => (x.component.transform.position - from).sqrMagnitude)
                .Select(x => x.item)
                .FirstOrDefault();
        }

        public static IEnumerable<T> GetWithinRadius<T>(IEnumerable<T> items, Vector3 from, float radius)
        {
            float sqrRadius = radius * radius;
            return items
                .Select(item => (item, component: item as Component))
                .Where(x => x.component != null)
                .Where(x => (x.component.transform.position - from).sqrMagnitude <= sqrRadius)
                .Select(x => x.item);
        }

        public static IEnumerable<T> GetNearestN<T>(IEnumerable<T> items, Vector3 from, int n)
        {
            return items
                .Select(item => (item, component: item as Component))
                .Where(x => x.component != null)
                .OrderBy(x => (x.component.transform.position - from).sqrMagnitude)
                .Take(n)
                .Select(x => x.item);
        }

        public static T GetRandom<T>(IEnumerable<T> items)
        {
            return items.OrderBy(_ => Random.value).FirstOrDefault();
        }

        public static T GetFirstActive<T>(IEnumerable<T> items)
        {
            List<T> active = items
                .Select(item => (item, component: item as Component))
                .Where(x => x.component != null &&
                            x.component.gameObject.activeInHierarchy)
                .Select(x => x.item)
                .ToList(); // materialize once to access count
            
            if (active.Count != 1)
            {
                Debug.LogWarning($"Expected exactly 1 active {typeof(T).Name}, but found {active.Count} active");
            }

            return active.FirstOrDefault();
        }
    }
}