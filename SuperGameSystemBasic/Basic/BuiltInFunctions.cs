using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SuperGameSystemBasic.Audio;
using SuperGameSystemBasic.Utils;

namespace SuperGameSystemBasic.Basic
{
    internal class BuiltInFunctions
    {
        public static Random Random = new Random();

        public static void InstallAll(Interpreter interpreter)
        {
            interpreter.AddFunction("TOSTR", ToStr);
            interpreter.AddFunction("TODIM", ToDim);
            interpreter.AddFunction("TONUM", ToNum);

            interpreter.AddFunction("ISSTR", IsStr);
            interpreter.AddFunction("ISDIM", IsDim);
            interpreter.AddFunction("ISNUM", IsNum);

            interpreter.AddFunction("ABS", Abs);
            interpreter.AddFunction("NEG", Neg);
            interpreter.AddFunction("RAND", Rand);
            interpreter.AddFunction("MIN", Min);
            interpreter.AddFunction("MAX", Max);

            interpreter.AddFunction("SIN", Sin);
            interpreter.AddFunction("COS", Cos);
            interpreter.AddFunction("TAN", Tan);
            interpreter.AddFunction("SQRT", Sqrt);
            interpreter.AddFunction("SIGN", Sign);

            interpreter.AddFunction("ROUNDUP", RoundUp);
            interpreter.AddFunction("ROUNDDOWN", RoundDown);
            interpreter.AddFunction("ROUND", Round);

            interpreter.AddFunction("ISNAN", IsNaN);

            interpreter.AddFunction("LEN", Len);
            interpreter.AddFunction("TOUPPER", ToUpper);
            interpreter.AddFunction("TOLOWER", ToLower);
            interpreter.AddFunction("TRIM", Trim);

            interpreter.AddFunction("TRIMSTART", TrimStart);
            interpreter.AddFunction("TRIMEND", TrimEnd);
            interpreter.AddFunction("LEFT", Left);
            interpreter.AddFunction("RIGHT", Right);
            interpreter.AddFunction("SUBSTRING", SubString);
            interpreter.AddFunction("REPEAT", Repeat);
            interpreter.AddFunction("INSTR", InStr);
            interpreter.AddFunction("REPLACE", Replace);

            interpreter.AddFunction("SHUFFLE", Shuffle);
            interpreter.AddFunction("COUNT", Count);
            interpreter.AddFunction("UNIQUE", Unique);
            interpreter.AddFunction("UNION", Union);
            interpreter.AddFunction("CONCAT", Concat);
            interpreter.AddFunction("ASCENDING", Ascending);
            interpreter.AddFunction("DESCENDING", Descending);
            interpreter.AddFunction("REVERSE", Reverse);

            interpreter.AddFunction("SOUND", Sound);
            interpreter.AddFunction("SOUNDQUE", SoundQue);
            interpreter.AddFunction("SOUNDQUECLEAR", SoundQueClear);
            interpreter.AddFunction("SOUNDSIGNAL", SoundSignal);

            interpreter.AddFunction("TIMER", Timer);

            //New Functions ver 1.1
            interpreter.AddFunction("DIST", Dist);
            interpreter.AddFunction("SUM", Sum);
            interpreter.AddFunction("LERP", Lerp);
            interpreter.AddFunction("AVG", Avg);
        }

        private static Value Shuffle(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Argument Expected");
            var array = args.Count > 1 ? Concat(interpreter, args) : args[0];
            if (array.Type == ValueType.String)
                return new Value(new string(array.String.ToCharArray().Shuffle().ToArray()));
            return new Value(array.Array.Shuffle().ToArray());
        }

        private static Value Reverse(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Argument Expected");
            var array = args.Count > 1 ? Concat(interpreter, args) : args[0];
            if (array.Type == ValueType.String)
                return new Value(new string(array.String.ToCharArray().Reverse().ToArray()));
            return new Value(array.Array.Reverse().ToArray());
        }

        private static Value Descending(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Argument Expected");
            var array = args.Count > 1 ? Concat(interpreter, args) : args[0];
            if (array.Type == ValueType.String)
                return new Value(new string(array.String.ToCharArray().OrderByDescending(c => c).ToArray()));
            return new Value(array.Array.OrderByDescending(c => c).ToArray());
        }

        private static Value Ascending(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Argument Expected");
            var array = args.Count>1?Concat(interpreter, args):args[0];
            if (array.Type == ValueType.String)            
                return new Value(new string(array.String.ToCharArray().OrderBy(c => c).ToArray()));
            return new Value(array.Array.OrderBy(c=>c).ToArray());
        }

        private static Value Union(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                interpreter.Error("Expected 2 or more Arguments");
            var union = args[0].Convert(ValueType.Array).Array.ToList();
            for (var i = 1; i < args.Count; i++)
            {
                union = union.Union(args[i].Convert(ValueType.Array).Array).ToList();
            }
            return new Value(union.ToArray());
        }

        private static Value Concat(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                interpreter.Error("Expected 2 or more Arguments");
            if (args[0].Type == ValueType.String)
            {
                if (double.TryParse(args[0].String, out var n))
                {
                    var s = new List<double>{n};
                    for (var i = 1; i < args.Count; i++)
                        s.AddRange(args[i].Convert(ValueType.Array).Array);
                    return new Value(s.ToArray());
                }
                var str = new StringBuilder(args[0].String);
                for (var i = 1; i < args.Count; i++)
                {
                    str.Append(args[i].Convert(ValueType.String).String);
                }
            }
            var array = new List<double>();
            array.AddRange(args[0].Convert(ValueType.Array).Array);
            for (var i = 1; i < args.Count; i++)
            {
                array.AddRange(args[i].Convert(ValueType.Array).Array);
            }
            return new Value(array.ToArray());
        }


        private static Value Unique(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 1)
                interpreter.Error("Expected 1 Arguments");
            if(args[0].Type==ValueType.Real)return new Value(args[0].Convert(ValueType.Array).Real);
            if (args[0].Type == ValueType.Array) return new Value(args[0].Array.Distinct().ToArray());
            return new Value(args[0].String.ToCharArray().Distinct().Count());
        }

        private static Value Count(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
            interpreter.Error("Expected 1 or 2 Arguments");
            if (args.Count == 1) return Len(interpreter, args);
            if (args[0].Type == ValueType.Real)
            {
                var n = args[0].Real;
                var s = 0;
                for (var i = 1; i < args.Count; i++)
                    s += args[i].Convert(ValueType.Array).Array.Count(v => Math.Abs(v - n) < double.Epsilon);
                return new Value(s);
            }
            if (args[0].Type == ValueType.String)
            {
                if(double.TryParse(args[0].String,out var n))
                {
                    var s = 0;
                    for (var i = 1; i < args.Count; i++)
                        s += args[i].Convert(ValueType.Array).Array.Count(v => Math.Abs(v - n) < double.Epsilon);
                    return new Value(s);
                }
                var src = args[0].String;
                var count = 0;
                for (var x = 1; x < args.Count; x++)
                {
                    count += src.Select((c, i) => src.Substring(i))
                        .Count(sub => sub.StartsWith(args[x].Convert(ValueType.String).String));
                }
                return new Value(count);
            }
            interpreter.Error("Unexpected arguments");
            return Value.Zero;
        }

        private static Value Replace(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 3)
                interpreter.Error("Expected 3 Arguments");
            var s1 = args[0].Convert(ValueType.String).String;
            var s2 = args[1].Convert(ValueType.String).String;
            var s3 = args[2].Convert(ValueType.String).String;
            var str = s1.Replace(s2, s3);
            return new Value(str);
        }

        private static Value Right(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 2)
                interpreter.Error("Expected 2 Arguments");
            var c = (int) args[1].Convert(ValueType.Real).Real;
            var s = args[0].Convert(ValueType.String).String;
            return new Value(s.Substring(s.Length-c, c));
        }

        private static Value Left(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 2)
                interpreter.Error("Expected 2 Arguments");
            return new Value(args[0].Convert(ValueType.String).String.Substring(0, (int)args[1].Convert(ValueType.Real).Real));
        }

        private static Value Repeat(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 2)
                interpreter.Error("Expected 2 Arguments");
            var s = args[0].Convert(ValueType.String).String;
            var c = (int) args[1].Convert(ValueType.Real).Real;
            var str = new StringBuilder(s.Length * c).Insert(0, s, c).ToString();
            return new Value(str);

        }

        private static Value SubString(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 or 2 Arguments");
            if (args.Count==1)
                return new Value(args[0].Convert(ValueType.String).String.Substring((int)args[1].Convert(ValueType.Real).Real));
            return new Value(args[0].Convert(ValueType.String).String.Substring((int)args[1].Convert(ValueType.Real).Real, (int)args[2].Convert(ValueType.Real).Real));
        }

        private static Value InStr(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                interpreter.Error("Expected 2 or 3 Arguments");
            return new Value(args[0].Convert(ValueType.String).String.IndexOf(args[1].Convert(ValueType.String).String, args.Count==3? Math.Abs(args[1].Convert(ValueType.Real).Real) < double.Epsilon? StringComparison.Ordinal: StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        }

        private static Value TrimEnd(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return new Value(args[0].Convert(ValueType.String).String.TrimEnd());
        }

        private static Value TrimStart(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return new Value(args[0].Convert(ValueType.String).String.TrimStart());
        }

        private static Value IsNum(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return new Value(args[0].Type == ValueType.Real ? 1 : 0);
        }

        private static Value IsDim(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return new Value(args[0].Type == ValueType.Array ? 1 : 0);
        }

        private static Value IsStr(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return new Value(args[0].Type == ValueType.String ? 1 : 0);
        }

        private static Value Dist(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 4)
                if (args.Count == 2)
                {
                    if (args[0].Type == ValueType.Real)
                    {
                        if(args[1].Type == ValueType.Real)return new Value(Math.Abs(args[0].Real- args[1].Real));
                        if(double.TryParse(args[0].Convert(ValueType.String).String,out var n))return new Value(Math.Abs(args[0].Real - n));
                    }
                    if (args[1].Type == ValueType.Real)
                    {
                        if (double.TryParse(args[1].Convert(ValueType.String).String, out var n)) return new Value(Math.Abs(args[1].Real - n));
                    }
                    if (args[0].Type == ValueType.String && args[0].String.Length == 1 &&
                        args[1].Type == ValueType.String && args[1].String.Length == 1)
                        return new Value(Math.Abs(args[0].String.ToCharArray()[0] - args[1].String.ToCharArray()[0]));
                    return new Value(Utilities.LevenshteinDistance(args[0].Convert(ValueType.Array).Array,
                        args[1].Convert(ValueType.Array).Array));
                }
                else
                    interpreter.Error("Expected 2 or 4 Arguments");
            return new Value(Vector2.Distance(new Vector2(
                    (float) (args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real),
                    (float) (args[1].Type == ValueType.Real ? args[1].Real : args[1].Convert(ValueType.Real).Real)),
                new Vector2(
                    (float) (args[2].Type == ValueType.Real ? args[2].Real : args[2].Convert(ValueType.Real).Real),
                    (float) (args[3].Type == ValueType.Real ? args[3].Real : args[3].Convert(ValueType.Real).Real))));
        }

        

        private static Value Lerp(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 3)
                interpreter.Error("Expected 3 Arguments");
            return new Value(MathHelper.Lerp(
                (float) (args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real),
                (float) (args[1].Type == ValueType.Real ? args[1].Real : args[1].Convert(ValueType.Real).Real),
                (float) (args[2].Type == ValueType.Real ? args[2].Real : args[2].Convert(ValueType.Real).Real)));
        }

        public static Value Timer(Interpreter interpreter, List<Value> args)
        {
            if (args.Count == 0)
                return new Value(interpreter.RunTime.TotalMilliseconds);
            var index = (int) args[0].Convert(ValueType.Real).Real;
            if (args.Count == 1)
            {
                if (interpreter.Timers.ContainsKey(index))
                    return new Value(interpreter.Timers[index].ElapsedTimeSpan.TotalMilliseconds);
                interpreter.Timers.Add(index, StopWatchWithOffset.StartNew());
                return Value.Zero;
            }
            if (interpreter.Timers.ContainsKey(index))
            {
                interpreter.Timers[index] =
                    StopWatchWithOffset.StartNew(TimeSpan.FromMilliseconds((int) args[1].Convert(ValueType.Real).Real));
                return new Value(interpreter.Timers[index].ElapsedTimeSpan.TotalMilliseconds);
            }
            interpreter.Timers.Add(index,
                StopWatchWithOffset.StartNew(TimeSpan.FromMilliseconds((int) args[1].Convert(ValueType.Real).Real)));
            return new Value(interpreter.Timers[index].ElapsedTimeSpan.TotalMilliseconds);
        }

        private static Value ToUpper(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return new Value(args[0].Convert(ValueType.String).String.ToUpper());
        }

        private static Value ToLower(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return new Value(args[0].Convert(ValueType.String).String.ToLower());
        }

        private static Value Trim(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return new Value(args[0].Convert(ValueType.String).String.Trim());
        }

        private static Value Round(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return
                new Value(
                    Math.Round(args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real,
                        MidpointRounding.AwayFromZero));
        }

        private static Value RoundUp(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();
            return
                new Value(
                    Math.Ceiling(args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real));
        }

        private static Value Sin(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return
                new Value(
                    Math.Sin(args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real));
        }

        private static Value Cos(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return
                new Value(
                    Math.Cos(args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real));
        }

        private static Value Tan(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return
                new Value(
                    Math.Tan(args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real));
        }

        private static Value Sqrt(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return
                new Value(
                    Math.Sqrt(args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real));
        }

        private static Value Sign(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return
                new Value(
                    Math.Sign(args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real));
        }

        private static Value RoundDown(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return
                new Value(
                    Math.Floor(args[0].Type == ValueType.Real ? args[0].Real : args[0].Convert(ValueType.Real).Real));
        }

        private static Value Len(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            switch (args[0].Type)
            {
                case ValueType.String:
                    return new Value(args[0].String.Length);
                case ValueType.Array:
                    return new Value(args[0].Array.Length);
                default: //Real
                    return new Value(args[0].Convert(ValueType.String).String.Length);
            }
        }

        public static Value SoundQueClear(Interpreter interpreter, List<Value> args)
        {
            var c = interpreter.AudioQueue.Count;
            interpreter.AudioQueue.Clear();
            if (interpreter.WaveOut != null)
            {
                interpreter.WaveOut?.Stop();
                return new Value(c + 1);
            }
            return new Value(c);
        }
        public static SignalType DefaultSignalType = SignalType.Sine;
        public static Value SoundQue(Interpreter interpreter, List<Value> args)
        {
            return new Value(interpreter.AudioQueue.Count +
                             (interpreter.WaveOut == null || interpreter.WaveOut.State == SoundState.Stopped ? 0 : 1));
        }

        public static Value SoundSignal(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
            {
                DefaultSignalType = SignalType.Sine;
            }
            else
            {
                DefaultSignalType = GetSignalType(args[0].Real);
            }
            return new Value(GetSignalType(DefaultSignalType));
        }

        public static Value Sound(Interpreter interpreter, List<Value> args)
        {
            if (!(args.Count == 3 || (args.Count%4==0))) interpreter.Error("Expected 3 Arguments or arguments in multiples of 4");
            if (interpreter.WaveOut != null)
            {
                if (interpreter.Exit)
                {
                    interpreter.WaveOut?.Stop();
                    interpreter.WaveOut = null;
                    interpreter.AudioQueue.Clear();
                    return Value.Zero;
                }
                for (int index = 0; index < args.Count; index+=4)
                {
                    interpreter.AudioQueue.Enqueue(new Note
                    {
                        Frequency = args[index].Real,
                        Amplitude = args[index + 1].Real,
                        Duration = (int)args[index + 2].Real,
                        SignalType = args.Count==3?DefaultSignalType: GetSignalType(args[index + 3].Real)
                    });
                }
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new SongWriter(stream, 60, false))
                    {
                        for (int index = 0; index < args.Count; index += 4)
                        {
                            writer.AddNote((float)args[index].Real, args[index+2].Real / 1000,
                            (short)(Math.Max(Math.Min((int)args[index+1].Real, 16384), 0) / 16384 * short.MaxValue), args.Count == 3 ? DefaultSignalType : GetSignalType(args[index + 3].Real));
                        }
                    }
                    stream.Position = 0;
                    interpreter.WaveOut = SoundEffect.FromStream(stream).CreateInstance();
                    interpreter.WaveOut.Play();
                }
            }
            return new Value(interpreter.AudioQueue.Count);
        }
        public static double GetSignalType(SignalType signalType)
        {
            switch (signalType)
            {
                case SignalType.Sine: return 0;
                case SignalType.Sawtooth: return 1;
                case SignalType.Triangle: return 2;
                case SignalType.Square: return 3;
                default: return 4;
            }
        }

        public static SignalType GetSignalType(double t)
        {
            if (t >= 4) return SignalType.Noise;
            if (t >= 3) return SignalType.Square;
            if (t >= 2) return SignalType.Triangle;
            if (t >= 1) return SignalType.Sawtooth;
            return SignalType.Sine;
        }

        public static Value Rand(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                return new Value(Random.Next());
            if (args.Count == 2)
                return new Value(Random.Next((int) args[0].Real, (int) args[1].Real));
            if (args[0].Type == ValueType.Real) return new Value(Random.Next((int) args[0].Real));
            return new Value(args[0].Array[Random.Next(0, args[0].Array.Length)]);
        }

        public static Value ToStr(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return args[0].Convert(ValueType.String);
        }

        public static Value ToDim(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return args[0].Convert(ValueType.Array);
        }

        public static Value ToNum(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return args[0].Convert(ValueType.Real);
        }

        public static Value Abs(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            return new Value(Math.Abs(args[0].Real));
        }

        public static Value Neg(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Arguments");
            return new Value(Math.Abs(args[0].Real) * -1);
        }

        private static List<double> ToDoubleArray(List<Value> args)
        {
            var ret = new List<double>();
            foreach (var a in args)
                switch (a.Type)
                {
                    case ValueType.Real:
                        ret.Add(a.Real);
                        break;
                    case ValueType.String:
                        if (a.String.Length == 1) ret.Add(a.Convert(ValueType.Real).Real);
                        else ret.AddRange(a.Convert(ValueType.Array).Array);
                        break;
                    case ValueType.Array:
                        ret.AddRange(a.Array);
                        break;
                }
            return ret;
        }

        public static Value Min(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 or more Arguments");
            return new Value(ToDoubleArray(args).Min());
        }

        public static Value Max(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 or more Arguments");
            return new Value(ToDoubleArray(args).Max());
        }

        public static Value Avg(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 or more Arguments");
            return new Value(ToDoubleArray(args).Average());
        }

        public static Value Sum(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 or more Arguments");
            return new Value(ToDoubleArray(args).Sum());
        }

        public static Value IsNaN(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                return new Value(0);
            if (args[0].Type == ValueType.Real) return new Value(double.IsNaN(args[0].Real) ? 1 : 0);
            if (args[0].Type == ValueType.Array) return Value.One;
            if (double.TryParse(args[0].String, out var d)) return new Value(double.IsNaN(d) ? 1 : 0);
            return Value.One;
        }
    }
}