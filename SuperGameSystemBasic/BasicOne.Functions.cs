using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.Basic;
using ValueType = SuperGameSystemBasic.Basic.ValueType;

namespace SuperGameSystemBasic
{
    public partial class BasicOne
    {
        public int RequiredYeildCounter { get; set; }

        public Dictionary<string, Interpreter.BasicFunction> GetOsFunctions()
        {
            return new Dictionary<string, Interpreter.BasicFunction>
            {
                ["PRINT"] = Print,
                ["PRINTG"] = PrintG,
                ["GETROWS"] = GetRows,
                ["GETCOLUMNS"] = GetColumns,
                ["GLYPHGET"] = GlyphGet,
                ["GLYPHSET"] = GlyphSet,
                ["ELLIPSE"] = Ellipse,
                ["CURSORX"] = CursorX,
                ["CURSORY"] = CursorY,
                ["CURSORXY"] = CursorXY,
                ["CLEAR"] = Clear,
                ["LINE"] = Line,
                ["FILL"] = Fill,
                ["YEILD"] = Yeild,
                ["RECT"] = Rect,
                ["RGBDACRESET"] = RgbDacReset,
                ["RGBDACSET"] = RgbDacSet,
                ["RGBDACGET"] = RgbDacGet,
                ["SETMODE"] = SetMode,
                ["MOUSEX"] = MouseX,
                ["MOUSEY"] = MouseY,
                ["MOUSEB"] = MouseB,
            };
        }

        public Value MouseX(Interpreter interpreter, List<Value> args) => new Value(MouseTerminalLocation.X);
        public Value MouseY(Interpreter interpreter, List<Value> args) => new Value(MouseTerminalLocation.Y);
        public Value MouseB(Interpreter interpreter, List<Value> args)
        {
            var s  = Mouse.GetState();
            return new Value((s.LeftButton == ButtonState.Pressed || s.RightButton == ButtonState.Pressed) ? 1 : 0);
        }       

        public Value Ellipse(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 5 && args.Count != 6 && args.Count != 7)
                interpreter.Error("Expected 4,5 or 6 Arguments");
            var x = (int) args[0].Convert(ValueType.Real).Real;
            var y = (int) args[1].Convert(ValueType.Real).Real;
            var w = (int) args[2].Convert(ValueType.Real).Real;
            var h = (int) args[3].Convert(ValueType.Real).Real;
            var c = (int) args[4].Convert(ValueType.Real).Real;
            var fill = args.Count == 6 && args[5].Convert(ValueType.Real).Real > 0;
            var clear = args.Count != 7 || args[6].Convert(ValueType.Real).Real > 0;
            if (w <= 0 || h <= 0) return Value.Zero;
            var ellipse = new bool[w, h];
            EllipseDrawer.DrawEllipse(ellipse, new Rectangle(0, 0, w, h), fill);
            for (var j = 0; j < h; j++)
            {
                var any = false;
                for (var i = 0; i < w; i++)
                    if (ellipse[i, j])
                    {
                        any = true;
                        SetBackGroundR(x + i, y + j, c, clear);
                    }
                if (!any)
                    for (var ix = 0; ix < w; ix++)
                        if (ellipse[ix, j < h / 2 ? j + 1 : j - 1])
                            SetBackGroundR(x + ix, y + j, c, clear);
            }

            return Value.Zero;
        }

        public Value RgbDacReset(Interpreter interpreter, List<Value> args)
        {
            Terminal.Pallet = null;
            return Value.Zero;
        }

        public Value RgbDacGet(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 1)
                interpreter.Error("Expected 1 Arguments");
            var p = Terminal.Pallet[MathHelper.Clamp((int) args[0].Convert(ValueType.Real).Real, 0, 15)];
            return new Value(new double[] {p.R, p.G, p.B});
        }

        public Value RgbDacSet(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 2 && args.Count != 4)
                interpreter.Error("Expected 2 or 4 Arguments");
            if (args.Count == 2 && args[1].Type != ValueType.Array)
                interpreter.Error("Expected array");
            if (args.Count == 2)
            {
                if (args[1].Array.Length != 3) interpreter.Error("Array to small");
                var p = MathHelper.Clamp((int) args[0].Convert(ValueType.Real).Real, 0, 15);
                var a = args[1].Array;
                Terminal.Pallet[p] = new Color((int) a[0], (int) a[1], (int) a[2]);
                return new Value(p);
            }
            var px = MathHelper.Clamp((int) args[0].Convert(ValueType.Real).Real, 0, 15);
            Terminal.Pallet[px] = new Color((int) args[1].Convert(ValueType.Real).Real,
                (int) args[2].Convert(ValueType.Real).Real, (int) args[3].Convert(ValueType.Real).Real);
            return new Value(px);
        }

        public Value Line(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 5 && args.Count != 6)
                interpreter.Error("Expected 4 or 5 Arguments");
            var x = (int) args[0].Convert(ValueType.Real).Real;
            var y = (int) args[1].Convert(ValueType.Real).Real;
            var x2 = (int) args[2].Convert(ValueType.Real).Real;
            var y2 = (int) args[3].Convert(ValueType.Real).Real;
            var c = (int) args[4].Convert(ValueType.Real).Real;
            var clear = args.Count != 6 || args[5].Convert(ValueType.Real).Real > 0;
            line(x, y, x2, y2, c, clear);
            return Value.Zero;
        }

        public void line(int x, int y, int x2, int y2, int color, bool clear)
        {
            var w = x2 - x;
            var h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1;
            else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1;
            else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1;
            else if (w > 0) dx2 = 1;
            var longest = Math.Abs(w);
            var shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            var numerator = longest >> 1;
            for (var i = 0; i <= longest; i++)
            {
                SetBackGroundR(x, y, color, clear);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        public Value Rect(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 5 && args.Count != 6)
                interpreter.Error("Expected 4 or 5 Arguments");
            var x = (int) args[0].Convert(ValueType.Real).Real;
            var y = (int) args[1].Convert(ValueType.Real).Real;
            var w = (int) args[2].Convert(ValueType.Real).Real;
            var h = (int) args[3].Convert(ValueType.Real).Real;
            var c = (int) args[4].Convert(ValueType.Real).Real;
            var clear = args.Count != 6 || args[5].Convert(ValueType.Real).Real > 0;
            if (w <= 0 || h <= 0) return Value.Zero;
            DrawHLineR(x, y, w, c);
            DrawHLineR(x, y + (h - 1), w, c, clear);
            DrawVLineR(x, y, h, c);
            DrawVLineR(x + (w - 1), y, h, c, clear);
            return Value.Zero;
        }

        public Value Fill(Interpreter interpreter, List<Value> args)
        {
            if (args.Count == 5 || args.Count == 6)
            {
                FillRectR((int) args[0].Convert(ValueType.Real).Real, (int) args[1].Convert(ValueType.Real).Real,
                    (int) args[2].Convert(ValueType.Real).Real, (int) args[3].Convert(ValueType.Real).Real,
                    (int) args[4].Convert(ValueType.Real).Real,
                    args.Count != 6 || args[5].Convert(ValueType.Real).Real > 0);
                return Value.Zero;
            }
            if (args.Count != 4)
                interpreter.Error("Expected 4,5 or 6 Arguments");
            var x = (int) Math.Max(args[0].Convert(ValueType.Real).Real, 0);
            if (x > Terminal.Columns) return Value.Zero;
            var y = (int) Math.Max(args[1].Convert(ValueType.Real).Real, 0);
            if (y > Terminal.Rows) return Value.Zero;

            var s = (int) Math.Max(args[2].Convert(ValueType.Real).Real, 0);
            if (x + s < 0) return Value.Zero;
            if (s <= 0) return Value.Zero;
            var d = args[3].Convert(ValueType.Array).Array;
            if (d.Length < s * 3 - 1) interpreter.Error("Data array to small");

            var xoff = 0;
            var yoff = 0;
            var i = 0;
            while (i < d.Length)
            {
                var g = (int) d[i];
                i++;
                var f = (int) d[i];
                i++;
                var b = (int) d[i];
                i++;

                if (x + xoff < Terminal.Columns && x + xoff >= 0 && y + yoff >= 0)
                    Terminal.VideoBuffer[(y + yoff) * Terminal.Columns + x + xoff] = new Character((Glyph) g, f, b,
                        SpriteEffects.None);

                xoff++;
                if (xoff == s)
                {
                    xoff = 0;
                    yoff++;
                }
                if (y + yoff > Terminal.Rows)
                    break;
            }
            return Value.Zero;
        }


        public Value Yeild(Interpreter interpreter, List<Value> args)
        {
            RequiredYeildCounter = 0;
            if (args.Count == 0)
            {
                Interpreter.InterpreterState = LineState.Yeild;
                YeildTime = TimeSpan.FromMilliseconds(100);
                return new Value(100);
            }
            Interpreter.InterpreterState = LineState.Yeild;
            var t = (int) Math.Max(args[0].Convert(ValueType.Real).Real, 0);
            YeildTime = TimeSpan.FromMilliseconds(t);
            return new Value(t);
        }

        public Value SetMode(Interpreter interpreter, List<Value> args)
        {
            if(args.Count != 2)
                interpreter.Error("Expected 2 arguments");
            var c = (int)Math.Max(Math.Min(Math.Abs(args[0].Convert(ValueType.Real).Real),80), 1);
            var r = (int)Math.Max(Math.Min(Math.Abs(args[0].Convert(ValueType.Real).Real),24), 1);
            Terminal.SetResolution(c, r);
            return new Value(c*r);
        }

        public Value GetRows(Interpreter interpreter, List<Value> args)
        {
            return new Value(Terminal.Rows);
        }

        public Value GetColumns(Interpreter interpreter, List<Value> args)
        {
            return new Value(Terminal.Columns);
        }

        public Value GlyphGet(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 1)
                interpreter.Error("Expected 1 arguments");
            var g = MathHelper.Clamp((int) args[0].Convert(ValueType.Real).Real, 0, 191);
            return new Value(GetGlyphData(g, this).Select(p => p ? 1d : 0d).ToArray());
        }

        public Value GlyphSet(Interpreter interpreter, List<Value> args)
        {
            if (args.Count != 2)
                interpreter.Error("Expected 2 arguments");
            var g = MathHelper.Clamp((int) args[0].Convert(ValueType.Real).Real, 0, 191);
            if (args[1].Type != ValueType.Array)
                interpreter.Error("Expected array");
            if (args[1].Array.Length != 20 * 24)
                interpreter.Error($"Array not correct size expected {20 * 24}");
            SetGlyphData(g, args[1].Array.Select(p => (int) p).ToArray(), this);
            return Value.Zero;
        }

        public Value CursorXY(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                interpreter.Error("Expected 2 arguments");
            Terminal.X = (int) MathHelper.Clamp((float) args[0].Convert(ValueType.Real).Real, 0, Terminal.Columns);
            Terminal.Y = (int) MathHelper.Clamp((float) args[1].Convert(ValueType.Real).Real, 0, Terminal.Rows);
            return Value.Zero;
        }

        public Value CursorX(Interpreter interpreter, List<Value> args)
        {
            if (args.Count > 0)
                Terminal.X = (int) MathHelper.Clamp((float) args[0].Convert(ValueType.Real).Real, 0, Terminal.Columns);
            return new Value(Terminal.X);
        }

        public Value CursorY(Interpreter interpreter, List<Value> args)
        {
            if (args.Count > 0)
                Terminal.Y = (int) MathHelper.Clamp((float) args[0].Convert(ValueType.Real).Real, 0, Terminal.Rows);
            return new Value(Terminal.Y);
        }

        public Value Clear(Interpreter interpreter, List<Value> args)
        {
            if (args.Count == 0)
                Terminal.Clear();
            if (args.Count == 1)
                Terminal.Clear((int) args[0].Real);
            return Value.Zero;
        }

        public Value PrintG(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                interpreter.Error("Expected 1 Argument");
            if (args.Count == 1)
                Terminal.Print((char) (args[0].Convert(ValueType.Real).Real + 32), SpriteEffects.None);
            else if (args.Count == 2)
                Terminal.Print((char) (args[0].Convert(ValueType.Real).Real + 32),
                    (int) args[1].Convert(ValueType.Real).Real, Terminal.DefaultBackColorIndex, SpriteEffects.None);
            else if (args.Count == 3)
                Terminal.Print((char) (args[0].Convert(ValueType.Real).Real + 32),
                    (int) args[1].Convert(ValueType.Real).Real, (int) args[2].Convert(ValueType.Real).Real,
                    SpriteEffects.None);
            else if (args.Count == 4)
                Terminal.Print((char) (args[0].Convert(ValueType.Real).Real + 32),
                    (int) args[1].Convert(ValueType.Real).Real, (int) args[2].Convert(ValueType.Real).Real,
                    (SpriteEffects) MathHelper.Clamp((int) args[3].Convert(ValueType.Real).Real, 0, 2));

            return Value.Zero;
        }

        public Value Print(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
            {
                Terminal.X = 0;
                Terminal.Y++;
            }
            if (args.Count == 1)
                Terminal.Print(args[0].Convert(ValueType.String).String, SpriteEffects.None);
            else if (args.Count == 2)
                Terminal.Print(args[0].Convert(ValueType.String).String, (int) args[1].Convert(ValueType.Real).Real,
                    Terminal.DefaultBackColorIndex, SpriteEffects.None);
            else if (args.Count == 3)
                Terminal.Print(args[0].Convert(ValueType.String).String, (int) args[1].Convert(ValueType.Real).Real,
                    (int) args[2].Convert(ValueType.Real).Real, SpriteEffects.None);
            else if (args.Count == 4)
                Terminal.Print(args[0].Convert(ValueType.String).String, (int) args[1].Convert(ValueType.Real).Real,
                    (int) args[2].Convert(ValueType.Real).Real,
                    (SpriteEffects) MathHelper.Clamp((int) args[3].Convert(ValueType.Real).Real, 0, 2));

            return Value.Zero;
        }
    }
}