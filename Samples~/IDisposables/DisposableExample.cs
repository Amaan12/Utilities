using System;
using UnityEngine;

namespace Utilities.IDisposableUtils
{
    public class DisposableExample : MonoBehaviour
    {
        void Start()
        {
            using (new DisposableLogStopwatch())
            {
                Log1();
                Log2();
            }
        }

        void Log1()
        {
            Debug.Log("Log1");
        }
        
        void Log2()
        {
            Debug.Log("Log2");
        }
    }
}