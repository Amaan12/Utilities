using System;
using UnityEngine;

namespace Utilities.IDisposableUtils
{
    public class DisposableFreezeTime : IDisposable
    {
        private readonly float _previousTimeScale;

        public DisposableFreezeTime()
        {
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        public void Dispose()
        {
            Time.timeScale = _previousTimeScale;
        }
    }
}