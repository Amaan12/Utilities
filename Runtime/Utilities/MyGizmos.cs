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
    public static class MyGizmos
    {
        public static void DrawLine(IList<Transform> points, Color color)
        {
            if (points == null || points.Count < 2)
                return;

            Color prevColor = Gizmos.color;
            Gizmos.color = color;

            Transform prev = null;

            for (int i = 0; i < points.Count; i++)
            {
                var current = points[i];
                if (current == null)
                    continue;

                if (prev != null)
                {
                    Gizmos.DrawLine(prev.position, current.position);
                }

                prev = current;
            }

            Gizmos.color = prevColor;
        }

        public static void DrawLine(IList<Transform> points)
        {
            DrawLine(points, Color.white);
        }
    }

}