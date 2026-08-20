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
    public static class MyDebug
    {
        public static void DrawLine(IList<Transform> points, Color color, float duration = 0f, bool depthTest = true)
        {
            if (points == null || points.Count < 2)
                return;

            Transform prev = null;

            for (int i = 0; i < points.Count; i++)
            {
                var current = points[i];
                if (current == null)
                    continue;

                if (prev != null)
                {
                    Debug.DrawLine(
                        prev.position,
                        current.position,
                        color,
                        duration,
                        depthTest
                    );
                }

                prev = current;
            }
        }

        public static void DrawLine(IList<Transform> points)
        {
            DrawLine(points, Color.white);
        }

        public static void DrawLine(IList<Transform> points, Color color)
        {
            DrawLine(points, color, 0f);
        }

        public static void DrawLine(IList<Transform> points, Color color, float duration)
        {
            DrawLine(points, color, duration, true);
        }
    }

}