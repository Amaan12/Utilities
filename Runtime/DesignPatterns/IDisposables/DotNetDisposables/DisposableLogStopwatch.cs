using System;
using System.Diagnostics;

namespace Utilities.IDisposableUtils
{
    public class DisposableLogStopwatch : IDisposable
    {
        private readonly Stopwatch _stopwatch;

        public DisposableLogStopwatch()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            var elapsedMS = _stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"took: {elapsedMS} ms");
        }
    }
}