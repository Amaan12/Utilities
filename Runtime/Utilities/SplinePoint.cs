using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    [System.Serializable]
    public struct SplinePoint
    {
        public Vector3 position;
        public Vector3 tangent; // spline forward direction
    }
}