using System;
using System.Diagnostics;

namespace SuperGameSystemBasic.Basic
{
    public class StopWatchWithOffset
    {
        private readonly Stopwatch _stopwatch;
        private TimeSpan _offsetTimeSpan;

        public StopWatchWithOffset(TimeSpan offsetElapsedTimeSpan)
        {
            _offsetTimeSpan = offsetElapsedTimeSpan;
            _stopwatch = new Stopwatch();
        }

        public TimeSpan ElapsedTimeSpan
        {
            get => _stopwatch.Elapsed + _offsetTimeSpan;
            set => _offsetTimeSpan = value;
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public static StopWatchWithOffset StartNew()
        {
            var s = new StopWatchWithOffset(TimeSpan.Zero);
            s.Start();
            return s;
        }

        public static StopWatchWithOffset StartNew(TimeSpan ts)
        {
            var s = new StopWatchWithOffset(ts);
            s.Start();
            return s;
        }
    }
}