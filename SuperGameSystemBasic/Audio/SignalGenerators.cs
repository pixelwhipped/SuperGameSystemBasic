using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperGameSystemBasic.Audio
{
    public static class SignalGenerators
    {
        public static Random Random = new Random();
        public static float Sine(float t) => (float) Math.Sin(2f*Math.PI* t);
        public static float Square(float t) => Math.Sign(Math.Sin(2f * Math.PI * t));
        public static float Triangle(float t) => 1f - 4f * (float)Math.Abs
                    (Math.Round(t - 0.25f) - (t - 0.25f));
        public static float Sawtooth(float t) => 2f * (t - (float)Math.Floor(t + 0.5f));
        public static float Noise(float t) => (float)Random.NextDouble();        
    }
}
