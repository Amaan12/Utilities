using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Splines;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

namespace Utilities
{
    public static class Helpers
    {
        #region Optimization
        /// <summary>
        /// Camera.main cache
        /// Redundant now as this value is cached anyways, but useful to know.
        /// </summary>   
        // static Camera _camera;
        // public static Camera Camera => _camera ??= Camera.main;

        /// <summary>
        /// WaitForSeconds cache
        /// </summary>
        static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }
        #endregion

        #region UI
        /// <summary>
        /// Is the cursor or finger click over any UI element
        /// </summary>
        static PointerEventData _eventDataCurrentPosition;
        static List<RaycastResult> _results;
        public static bool IsOverUI()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
            return _results.Count > 0;
        }

        /// <summary>
        /// Was used in project drift to make a ghost panel that enabled the TurnTable dragging
        /// </summary>
        public static bool IsPointerOverUI(RectTransform panel)
        {
            Vector2 localMousePos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panel, Input.mousePosition, null, out localMousePos))
            {
                return panel.rect.Contains(localMousePos);
            }
            return false;
        }

        /// <summary>
        /// Get World Position of Canvas element (used with camera canvas mode, e.g. to place an object and animate it to give 3D feel)
        /// </summary>
        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera.main, out var result);
            return result;
        }

        /// <summary>
        /// For menu transitions, should use DOTween, but that failed, so this is for now.
        /// </summary>
        public static IEnumerator Fade(CanvasGroup cg, float from, float to, float duration)
        {
            if (cg == null) yield break;

            cg.alpha = from;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }

            cg.alpha = to;
            cg.interactable = (to > 0.9f);
            cg.blocksRaycasts = (to > 0.9f);
        }

        public static void Cut(CanvasGroup cg, float to)
        {
            if (cg == null) return;

            cg.alpha = to;
            cg.interactable = (to > 0.9f);
            cg.blocksRaycasts = (to > 0.9f);
        }

        public static IEnumerator FadeEaseOut(CanvasGroup cg, float from, float to, float duration)
        {
            if (cg == null) yield break;

            cg.alpha = from;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;

                float normalized = Mathf.Clamp01(t / duration);
                float eased = 1f - Mathf.Pow(1f - normalized, 2f); // ease-out

                cg.alpha = Mathf.Lerp(from, to, eased);
                yield return null;
            }

            cg.alpha = to;
            cg.interactable = (to > 0.9f);
            cg.blocksRaycasts = (to > 0.9f);
        }

        /// <summary>Sets alpha on a SpriteRenderer.</summary>
        public static void SetAlpha(this SpriteRenderer sr, float alpha)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        /// <summary>Sets alpha on a UI Graphic.</summary>
        public static void SetAlpha(this Graphic g, float alpha)
        {
            Color c = g.color;
            c.a = alpha;
            g.color = c;
        }
        #endregion

        #region CSV
        /// <summary>
        /// Extract .csv file content
        /// </summary>
        /// <param name="filePath">Probably relative to Assets</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async UniTask<List<string[]>> ReadCsv(string filePath, string fileName)
        {
            string path = Path.Combine(filePath, fileName);

            List<string[]> table = new List<string[]>();

            // For platforms like Android, StreamingAssets requires UnityWebRequest
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var www = UnityEngine.Networking.UnityWebRequest.Get(path))
            {
                await www.SendWebRequest();
                string content = www.downloadHandler.text;
                string[] lines = content.Split('\n');
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        table.Add(line.Trim().Split(','));
                }
            }
#else
            string[] lines = await UniTask.RunOnThreadPool(() => File.ReadAllLines(path));
            foreach (string line in lines)
            {
                // Split the line by commas
                string[] cells = line.Split(',');

                // Trim each cell individually
                for (int i = 0; i < cells.Length; i++)
                {
                    cells[i] = cells[i].Trim();
                }

                table.Add(cells);
            }
#endif

            return table;
        }

        public static List<string[]> ReadTsv(string filePath, string fileName)
        {
            List<string[]> table = new List<string[]>();
            return table;
        }
        #endregion

        #region 2D Array
        /// <summary>
        /// Removes writing redundant double for loops each time
        /// </summary>
        public static void ForEach2D<T>(T[,] array, System.Action<int, int> action)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    action(x, y);
                }
            }
        }

        public static void ForEachJagged<T>(List<List<T>> lists, System.Action<int, int> action)
        {
            for (int x = 0; x < lists.Count; x++)
            {
                var inner = lists[x];
                if (inner == null)
                    continue;

                for (int y = 0; y < inner.Count; y++)
                {
                    action(x, y);
                }
            }
        }
        #endregion

        #region List
        /// <summary>
        /// Returns a random element from an array.
        /// Logs a warning and returns default(T) if empty or null.
        /// list.Random() was present in git-amend's utilities, however for array it was O(n) while this is O(1)
        /// </summary>
        public static T RandomFromArray<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
            {
                Debug.LogWarning($"GetRandom: Tried to get a random element from an empty or null array of type {typeof(T)}.");
                return default;
            }

            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        /// <summary>
        /// if list has 0 active gameobject or more than 1 active gameobject, activate the first one
        /// check if list has one active gameobject, if so, deactivate it and activate the next one
        /// </summary>
        public static void Cycle(this IList<GameObject> list)
        {
            if (list == null || list.Count == 0)
                return;

            int activeIndex = -1;
            int activeCount = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var go = list[i];
                if (go != null && go.activeSelf)
                {
                    activeIndex = i;
                    activeCount++;
                    if (activeCount > 1)
                        break;
                }
            }

            // 0 or multiple active → toggle first valid
            if (activeCount != 1)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        list.Toggle(i);
                        return;
                    }
                }
                return;
            }

            // exactly one active → toggle next valid
            for (int i = 1; i <= list.Count; i++)
            {
                int next = (activeIndex + i) % list.Count;
                if (list[next] != null)
                {
                    list.Toggle(next);
                    return;
                }
            }
        }

        public static void Toggle(this IList<GameObject> objects, int index)
        {
            if (objects == null || objects.Count == 0)
                return;

            if (index < 0 || index >= objects.Count)
                return;

            for (int i = 0; i < objects.Count; i++)
                objects[i]?.SetActive(i == index);
        }
        #endregion

        #region Splines
        /// <summary>
        /// Extracts all spline points and their tangents (world space) from the given list of spline containers.
        /// Ideally bezier knots should be used, but this is a temporary solution.
        /// </summary>
        public static List<SplinePoint> ExtractSplinePoints(this List<SplineContainer> containers)
        {
            List<SplinePoint> result = new List<SplinePoint>();

            foreach (var container in containers)
            {
                if (container == null) continue;

                foreach (var spline in container.Splines)
                {
                    if (spline == null || spline.Count < 2) continue;

                    for (int i = 0; i < spline.Count; i++)
                    {
                        float t = spline.Count == 1 ? 0f : i / (float)(spline.Count - 1);

                        Vector3 localPos = (Vector3)SplineUtility.EvaluatePosition(spline, t);
                        Vector3 localTan = (Vector3)SplineUtility.EvaluateTangent(spline, t);

                        Vector3 worldPos = container.transform.TransformPoint(localPos);
                        Vector3 worldTan = container.transform.TransformDirection(localTan).normalized;

                        result.Add(new SplinePoint
                        {
                            position = worldPos,
                            tangent = worldTan
                        });
                    }
                }
            }

            return result;
        }

        public static SplinePoint FindNearestSplinePoint(this List<SplinePoint> allSplinePoints, Transform target)
        {
            return allSplinePoints
                .OrderBy(p => (p.position - target.position).sqrMagnitude)
                .First();
        }

        public static Vector3 FindAlignedTangent(this List<SplinePoint> allSplinePoints, Transform target)
        {
            SplinePoint nearest = allSplinePoints.FindNearestSplinePoint(target);

            Vector3 tangent = nearest.tangent.normalized;
            Vector3 forward = target.forward;

            // Flip if facing opposite direction
            if (Vector3.Dot(tangent, forward) < 0f)
                tangent = -tangent;

            return tangent;
        }
        #endregion

        #region Transform
        // <summary>Resets world position, rotation, and scale.</summary>
        public static void ResetTransform(this Transform t)
        {
            t.position = Vector3.zero;
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        /// <summary>Resets local position, rotation, and scale.</summary>
        public static void ResetLocalTransform(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        /// <summary>Invokes an action on all children recursively.</summary>
        public static void ForEachChildRecursive(this Transform root, System.Action<Transform> action, bool includeRoot = false)
        {
            if (includeRoot)
                action(root);

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                action(child);
                child.ForEachChildRecursive(action);
            }
        }

        /// <summary>Sets the layer for all children recursively.</summary>
        public static void SetLayerRecursive(this Transform root, int layer, bool includeRoot = true)
        {
            root.ForEachChildRecursive(
                t => t.gameObject.layer = layer,
                includeRoot);
        }
        #endregion

        #region Vector3
        /// <summary>Rounds Vector3 to Vector3Int.</summary>
        public static Vector3Int RoundToInt(this Vector3 v)
        {
            return new Vector3Int(
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y),
                Mathf.RoundToInt(v.z)
            );
        }
        #endregion

        #region AI
        /// <summary>
        /// Generates a random point on a navmesh
        /// </summary>
        /// <param name="origin">The origin to generate the random position from</param>
        /// <param name="range">How far from the origin to generate the point</param>
        /// <param name="areaMask">Navmesh surface area mask</param>
        /// <returns>A Vector3 with the random position</returns>
        public static Vector3 RandomNavmeshPoint(Vector3 origin, float range, int areaMask)
        {
            var randomDirection = UnityEngine.Random.insideUnitSphere * range;

            randomDirection += origin;

            NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, range, areaMask);

            return navHit.position;
        }
        #endregion
    }
}