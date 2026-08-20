using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

namespace Utilities
{
    public static class EnumUtility
    {
        public static T Random<T>() where T : struct, Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        public static T[] Values<T>() where T : struct, Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static int Count<T>() where T : struct, Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        public static bool IsDefined<T>(T value) where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }

        public static bool TryParse<T>(string value, out T result) where T : struct, Enum
        {
            return Enum.TryParse(value, true, out result);
        }

        public static T Next<T>(T value) where T : struct, Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, value);
            return values[(index + 1) % values.Length];
        }

        public static T Previous<T>(T value) where T : struct, Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, value);
            return values[(index - 1 + values.Length) % values.Length];
        }

        public static bool IsFirst<T>(T value) where T : struct, Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return EqualityComparer<T>.Default.Equals(value, values[0]);
        }

        public static bool IsLast<T>(T value) where T : struct, Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return EqualityComparer<T>.Default.Equals(value, values[values.Length - 1]);
        }
    }

}