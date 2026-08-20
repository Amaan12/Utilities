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
    public static class MyMathf
    {
        /// <summary>Formats numbers using K, M, B, T… with precision, casing, and rounding options.</summary>
        public static string ToAbbreviatedString(this int value, int precision = 1, bool capitalize = true, bool round = true) => Format(value, precision, capitalize, round);
        public static string ToAbbreviatedString(this long value, int precision = 1, bool capitalize = true, bool round = true) => Format(value, precision, capitalize, round);
        public static string ToAbbreviatedString(this float value, int precision = 1, bool capitalize = true, bool round = true) => Format(value, precision, capitalize, round);
        public static string ToAbbreviatedString(this double value, int precision = 1, bool capitalize = true, bool round = true) => Format(value, precision, capitalize, round);

        static string Format(double number, int precision, bool capitalize, bool round)
        {
            double abs = System.Math.Abs(number);

            string[] suffixes =
            {
            "", "K", "M", "B", "T", "Q", "Qi", "Sx", "Sp", "Oc", "No", "Dc"
            };

            int index = 0;
            while (abs >= 1000 && index < suffixes.Length - 1)
            {
                abs /= 1000;
                number /= 1000;
                index++;
            }

            if (!round && precision > 0)
            {
                double factor = System.Math.Pow(10, precision);
                number = System.Math.Truncate(number * factor) / factor;
            }

            string suffix = capitalize
                ? suffixes[index]
                : suffixes[index].ToLowerInvariant();

            return number.ToString(
                precision > 0 ? $"F{precision}" : "F0",
                CultureInfo.InvariantCulture
            ) + suffix;
        }

        public static int RandomRangeNoDupe(int min, int max, ref int lastChosenNumber)
        {
            int generatedNumber = lastChosenNumber;

            while (generatedNumber == lastChosenNumber)
                generatedNumber = UnityEngine.Random.Range(min, max);

            lastChosenNumber = generatedNumber;

            return generatedNumber;
        }

        public static Vector3 RandomVector(float min, float max) => new Vector3(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));

        /// <summary>
        /// Clamps an angle between a minimum and maximum value, handling wrap-around at 360 degrees.
        /// </summary>
        /// <param name="angle">The angle to clamp.</param>
        /// <param name="min">The minimum angle.</param>
        /// <param name="max">The maximum angle.</param>
        /// <returns>The clamped angle.</returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            angle = angle % 360;
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>Factorial of n (n!). Returns 1 for n <= 1.</summary>
        public static long Factorial(int n)
        {
            if (n < 0) throw new ArgumentException("n must be >= 0");
            long result = 1;
            for (int i = 2; i <= n; i++) result *= i;
            return result;
        }

        /// <summary>n choose r (combinations)</summary>
        public static long Combinations(int n, int r)
        {
            if (n < 0 || r < 0 || r > n) throw new ArgumentException();
            r = Mathf.Min(r, n - r); // symmetry
            return Factorial(n) / (Factorial(r) * Factorial(n - r));
        }

        /// <summary>n permute r (permutations)</summary>
        public static long Permutations(int n, int r)
        {
            if (n < 0 || r < 0 || r > n) throw new ArgumentException();
            return Factorial(n) / Factorial(n - r);
        }

        /// <summary>Greatest common divisor (GCD) of two integers</summary>
        public static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int t = b;
                b = a % b;
                a = t;
            }
            return Math.Abs(a);
        }

        /// <summary>Least common multiple (LCM) of two integers</summary>
        public static int LCM(int a, int b)
        {
            if (a == 0 || b == 0) return 0;
            return Math.Abs(a / GCD(a, b) * b);
        }

        /// <summary>n-th Fibonacci number (iterative)</summary>
        public static long Fibonacci(int n)
        {
            if (n < 0) throw new ArgumentException("n must be >= 0");
            if (n == 0) return 0;
            if (n == 1) return 1;
            long a = 0, b = 1;
            for (int i = 2; i <= n; i++)
            {
                long temp = a + b;
                a = b;
                b = temp;
            }
            return b;
        }

        /// <summary>Sum of integers raised to a given exponent: sum_{i=1}^{n} i^exponent</summary>
        public static long SumOfPowers(int n, int exponent)
        {
            if (n < 0) throw new ArgumentException("n must be >= 0");
            long sum = 0;
            for (int i = 1; i <= n; i++)
            {
                long term = 1;
                for (int e = 0; e < exponent; e++)
                    term *= i;
                sum += term;
            }
            return sum;
        }

        /// <summary>Checks if a number is prime (optimized trial division)</summary>
        public static bool IsPrime(int n)
        {
            if (n <= 1) return false;

            int limit = (int)Mathf.Sqrt(n);
            for (int i = 2; i <= limit; i++)
            {
                if (n % i == 0)
                    return false;
            }

            return true;
        }

        /// <summary>Generalized distance between two points using a p-norm</summary>
        public static float Distance(Vector3 a, Vector3 b, int p = 2)
        {
            if (p <= 0) throw new ArgumentException("p must be > 0");
            Vector3 diff = a - b;
            float sum = Mathf.Pow(Mathf.Abs(diff.x), p) + Mathf.Pow(Mathf.Abs(diff.y), p) + Mathf.Pow(Mathf.Abs(diff.z), p);
            return Mathf.Pow(sum, 1f / p);
        }

        public static float EuclideanDistance(Vector3 a, Vector3 b) => Distance(a, b, 2);
        public static float ManhattanDistance(Vector3 a, Vector3 b) => Distance(a, b, 1);
        public static float ChebyshevDistance(Vector3 a, Vector3 b) => Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));

        /// <summary>Normalize angle to -180..180 degrees</summary>
        public static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }

        /// <summary>Convert an integer to its ordinal string (1 → 1st, 2 → 2nd)</summary>
        public static string ToOrdinal(int number)
        {
            if (number <= 0) return number.ToString();
            int rem100 = number % 100;
            int rem10 = number % 10;
            if (rem100 - rem10 == 10) return number + "th";

            return rem10 switch
            {
                1 => number + "st",
                2 => number + "nd",
                3 => number + "rd",
                _ => number + "th"
            };
        }

        #region Lerp
        /// <summary>
        /// Lerp Smoothening that is frame-rate independent, can be used in Update() safely.
        /// </summary>
        /// <param name="k">How fast it will approach that value.</param>
        public static float LerpExp(float a, float b, float k) => Mathf.Lerp(a, b, 1f - Mathf.Exp(-k * Time.deltaTime));

        public static Vector3 LerpExp(Vector3 a, Vector3 b, float k) => Vector3.Lerp(a, b, 1f - Mathf.Exp(-k * Time.deltaTime));

        public static Quaternion LerpExp(Quaternion a, Quaternion b, float k) => Quaternion.Slerp(a, b, 1f - Mathf.Exp(-k * Time.deltaTime));
        #endregion

        #region Spring Damp
        /// <summary>
        /// This equations simulates a spring, it updates a damped spring system
        /// Can be used where you need a bounce effect, it's kind of like elastic ease, however this is the logic behind it, and this can also be used in update or coroutine.
        /// This gives more control over the ease, EaseOutElastic gives less control
        /// </summary>
        public static float SpringDamp(float value, ref float velocity, float target, float stiffness, float damping)
        {
            velocity += (target - value) * stiffness;
            velocity -= velocity * damping;
            value += velocity;

            return value;
        }

        public static Vector3 SpringDamp(Vector3 value, ref Vector3 velocity, Vector3 target, float stiffness, float damping)
        {
            velocity += (target - value) * stiffness;
            velocity -= velocity * damping;
            value += velocity;

            return value;
        }

        public static float SpringDampExp(float value, ref float velocity, float target, float deltaTime, float stiffness, float damping)
        {
            if (deltaTime <= 0f) return value;

            float x0 = value - target;
            float v0 = velocity;
            float c = damping;
            float k = stiffness;

            float d = c * c - 4f * k;

            if (d < -1e-5f)
            {
                // Underdamped
                float sigma = c / 2f;
                float omegaD = Mathf.Sqrt(k - (c * c) / 4f);
                float exp = Mathf.Exp(-sigma * deltaTime);
                float cos = Mathf.Cos(omegaD * deltaTime);
                float sin = Mathf.Sin(omegaD * deltaTime);

                float yT = exp * (x0 * cos + ((v0 + sigma * x0) / omegaD) * sin);
                velocity = exp * (v0 * cos - ((sigma * v0 + k * x0) / omegaD) * sin);
                value = target + yT;
            }
            else if (d > 1e-5f)
            {
                // Overdamped
                float sigma = c / 2f;
                float omegaD = Mathf.Sqrt((c * c) / 4f - k);
                float exp = Mathf.Exp(-sigma * deltaTime);
                float expPlus = Mathf.Exp(omegaD * deltaTime);
                float expMinus = Mathf.Exp(-omegaD * deltaTime);
                float cosh = (expPlus + expMinus) / 2f;
                float sinh = (expPlus - expMinus) / 2f;

                float yT = exp * (x0 * cosh + ((v0 + sigma * x0) / omegaD) * sinh);
                velocity = exp * (v0 * cosh - ((sigma * v0 + k * x0) / omegaD) * sinh);
                value = target + yT;
            }
            else
            {
                // Critically damped
                float sigma = c / 2f;
                float exp = Mathf.Exp(-sigma * deltaTime);

                float yT = exp * (x0 + (v0 + sigma * x0) * deltaTime);
                velocity = exp * (v0 - sigma * (v0 + sigma * x0) * deltaTime);
                value = target + yT;
            }

            return value;
        }

        public static Vector3 SpringDampExp(Vector3 value, ref Vector3 velocity, Vector3 target, float deltaTime, float stiffness, float damping)
        {
            if (deltaTime <= 0f) return value;

            Vector3 x0 = value - target;
            Vector3 v0 = velocity;
            float c = damping;
            float k = stiffness;

            float d = c * c - 4f * k;

            if (d < -1e-5f)
            {
                // Underdamped
                float sigma = c / 2f;
                float omegaD = Mathf.Sqrt(k - (c * c) / 4f);
                float exp = Mathf.Exp(-sigma * deltaTime);
                float cos = Mathf.Cos(omegaD * deltaTime);
                float sin = Mathf.Sin(omegaD * deltaTime);

                Vector3 yT = exp * (x0 * cos + ((v0 + sigma * x0) / omegaD) * sin);
                velocity = exp * (v0 * cos - ((sigma * v0 + k * x0) / omegaD) * sin);
                value = target + yT;
            }
            else if (d > 1e-5f)
            {
                // Overdamped
                float sigma = c / 2f;
                float omegaD = Mathf.Sqrt((c * c) / 4f - k);
                float exp = Mathf.Exp(-sigma * deltaTime);
                float expPlus = Mathf.Exp(omegaD * deltaTime);
                float expMinus = Mathf.Exp(-omegaD * deltaTime);
                float cosh = (expPlus + expMinus) / 2f;
                float sinh = (expPlus - expMinus) / 2f;

                Vector3 yT = exp * (x0 * cosh + ((v0 + sigma * x0) / omegaD) * sinh);
                velocity = exp * (v0 * cosh - ((sigma * v0 + k * x0) / omegaD) * sinh);
                value = target + yT;
            }
            else
            {
                // Critically damped
                float sigma = c / 2f;
                float exp = Mathf.Exp(-sigma * deltaTime);

                Vector3 yT = exp * (x0 + (v0 + sigma * x0) * deltaTime);
                velocity = exp * (v0 - sigma * (v0 + sigma * x0) * deltaTime);
                value = target + yT;
            }

            return value;
        }
        #endregion
    }
}