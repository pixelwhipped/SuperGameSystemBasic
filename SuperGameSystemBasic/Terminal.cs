using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable InconsistentNaming

namespace SuperGameSystemBasic
{
    public class Terminal
    {
        private const int GlyphsPerRow = 32;


        private const int GlyphsRows = 6;
        public static int BLACK = 0;
        public static int BLUE = 1;
        public static int GREEN = 2;
        public static int CYAN = 3;
        public static int RED = 4;
        public static int MAGENTA = 5;
        public static int BROWN = 6;
        public static int GRAY = 7;
        public static int DARK_GRAY = 8;
        public static int LIGHT_BLUE = 9;
        public static int LIGHT_GREEN = 10;
        public static int LIGHT_CYAN = 11;
        public static int LIGHT_RED = 12;
        public static int LIGHT_MAGENTA = 13;
        public static int YELLOW = 14;
        public static int WHITE = 15;

        public static Color[] DefaultPallet =
        {
            Color.Black, Color.Navy, Color.Green, Color.DarkCyan, Color.DarkRed,
            Color.Magenta, Color.Brown, Color.LightGray, Color.DarkGray, Color.RoyalBlue, Color.GreenYellow,
            Color.MediumTurquoise, Color.Red, Color.DeepPink, Color.Yellow, Color.White
        };

        internal readonly string _sheet;

        public Character[] VideoBuffer;

        private Dictionary<Color, Character> _colorShades;

        private Color[] _pallet;

        private int _x;
        private int _y;
        public Dictionary<Glyph, Rectangle> GlyphRects;
        public int Columns { get; set; }
        public int Rows { get; set; }


        public Terminal(BasicOne game, string sheet)
        {
            _sheet = sheet;
            Game = game;            
            GlyphRects = new Dictionary<Glyph, Rectangle>();            
            SetResolution(80, 24);
        }

        public void SetResolution(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
            VideoBuffer = new Character[Rows * Columns];
            var g = new Character(Glyph.Space, DefaultForeColorIndex, DefaultBackColorIndex, SpriteEffects.None);
            for (var y = 0; y < Rows; y++)
            for (var x = 0; x < Columns; x++)
                VideoBuffer[y * Columns + x] = g;
        }

        public Color[] Pallet
        {
            get
            {
                if (_pallet != null) return _pallet;
                _pallet = DefaultPallet;
                _colorShades = null;
                return _pallet;
            }
            set
            {
                if (value == null)
                {
                    _pallet = new Color[DefaultPallet.Length];
                    Array.Copy(DefaultPallet, _pallet, DefaultPallet.Length);
                }
                else
                {
                    _pallet = value;
                }
                _colorShades = null;
            }
        }

        public Dictionary<Color, Character> ColorShades
        {
            get
            {
                if (_colorShades != null) return _colorShades;
                _colorShades = GenerateShades();
                return _colorShades;
            }
        }


        /// <summary>
        ///     Gets the default foreground <see cref="Color" /> for a Character.
        /// </summary>
        public int DefaultForeColorIndex => Array.IndexOf(Pallet, ClosestColor(Color.White));

        /// <summary>
        ///     Gets the default background <see cref="Color" /> for a Character.
        /// </summary>
        public int DefaultBackColorIndex => Array.IndexOf(Pallet, ClosestColor(Color.Black));

        public Color DefaultBackColor => Pallet[DefaultBackColorIndex];
        public Color DefaultForeColor => Pallet[DefaultForeColorIndex];

        public BasicOne Game;

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
        public ContentManager Content => Game.Content;

        public Texture2D GlyphSheet;
        public int GlyphWidth;
        public int GlyphHeight;

        public int X
        {
            get => _x;
            set => _x = MathHelper.Clamp(value, 0, Columns - 1);
        }

        public int Y
        {
            get => _y;
            set => _y = MathHelper.Clamp(value, 0, Rows - 1);
        }

        public Effect ColorMapFx;        

        public SpriteBatch SpriteBatch;

        private Dictionary<Color, Character> GenerateShades()
        {
            var s = new Dictionary<Color, Character>();
            var mixes = new[]
            {
                new KeyValuePair<float, Glyph>(0.75f, Glyph.DarkFill),
                new KeyValuePair<float, Glyph>(0.5f, Glyph.GrayFill),
                new KeyValuePair<float, Glyph>(0.25f, Glyph.LightFill)
            };
            for (var i = 0; i < Pallet.Length; i++)
            {
                var c = Pallet[i];
                s.Add(c, new Character(Glyph.SolidFill, i, i, SpriteEffects.None));
                foreach (var m in mixes)
                    for (var j = 0; j < Pallet.Length; j++)
                    {
                        var df = Pallet[j];
                        var dfc = new Color((byte) (c.R * m.Key + df.R * (1f - m.Key)),
                            (byte) (c.G * m.Key + df.G * (1f - m.Key)),
                            (byte) (c.B * m.Key + df.B * (1f - m.Key)));
                        if (!s.ContainsKey(dfc)) s.Add(dfc, new Character(m.Value, i, j, SpriteEffects.None));
                    }
            }
            return s;
        }

        public Character ClosesetShade(Color color)
        {
            var pallet = ColorShades.Keys.ToArray();
            if (pallet.Any(c => c == color)) return ColorShades[color];
            var colorDiffs = pallet.Select(n => ColorDiff(n, color)).Min(n => n);
            var i = Array.FindIndex(pallet, n => ColorDiff(n, color) == colorDiffs);
            return ColorShades[pallet[i]];
        }

        public void Clear() => Clear(DefaultBackColorIndex);

        public void Clear(int color)
        {
            var g = new Character(Glyph.Space, DefaultForeColorIndex, color, SpriteEffects.None);

            for (var x = 0; x < VideoBuffer.Length; x++)
                VideoBuffer[x] = g;
        }

        public void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            GlyphSheet = Content.Load<Texture2D>(_sheet);
            GlyphWidth = GlyphSheet.Width / GlyphsPerRow;
            GlyphHeight = GlyphSheet.Height / GlyphsRows;
            foreach (var glyph in Enum.GetValues(typeof(Glyph)))
            {
                var g = (int) glyph;
                var column = g % GlyphsPerRow;
                var row = g / GlyphsPerRow;
                var rect = new Rectangle(column * GlyphWidth, row * GlyphHeight, GlyphWidth, GlyphHeight);
                GlyphRects.Add((Glyph) glyph, rect);
            }
            ColorMapFx = Game.Content.Load<Effect>("ColorMap");
            ColorMapFx.Parameters["OldForeground"].SetValue(Color.Black.ToVector4());
            ColorMapFx.Parameters["OldBackground"].SetValue(Color.Transparent.ToVector4());
        }

        public void Update(Rectangle bounds)
        {
        }

        public void Draw(Rectangle bounds)
        {
            var xinc = (double) bounds.Width / Columns;
            var yinc = (double) bounds.Height / Rows;
            for (var y = 0; y < Rows; y++)
            for (var x = 0; x < Columns; x++)
            {
                var r = new Rectangle((int) (xinc * x) + bounds.X, (int) (yinc * y) + bounds.Y, (int) xinc + 1,
                    (int) yinc + 1);
                var c = VideoBuffer[y * Columns + x];
                ColorMapFx.Parameters["NewForeground"].SetValue(
                    Pallet[MathHelper.Clamp(c.ForeColor, 0, Pallet.Length)].ToVector4());
                ColorMapFx.Parameters["NewBackground"].SetValue(
                    Pallet[MathHelper.Clamp(c.BackColor, 0, Pallet.Length)].ToVector4());
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                    DepthStencilState.Default, RasterizerState.CullNone, ColorMapFx);
                SpriteBatch.Draw(GlyphSheet, r, GlyphRects[c.Glyph], Color.White, 0, Vector2.Zero, c.Effect, 0);
                SpriteBatch.End();
            }
        }

        public Color ClosestColor(Color color)
        {
            if (Pallet.Any(c => c == color)) return color;
            var colorDiffs = Pallet.Select(n => ColorDiff(n, color)).Min(n => n);
            return Pallet[Array.FindIndex(Pallet, n => ColorDiff(n, color) == colorDiffs)];
        }

        private static int ColorDiff(Color c1, Color c2)
        {
            return (int) Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                   + (c1.G - c2.G) * (c1.G - c2.G)
                                   + (c1.B - c2.B) * (c1.B - c2.B));
        }

        public void Print(string str, SpriteEffects effect)
        {
            Print(str, DefaultForeColorIndex, DefaultBackColorIndex, effect);
        }

        public void Print(string str, int fColor, int bColor, SpriteEffects effect)
        {
            foreach (var s in str)
                Print(s, fColor, bColor, effect);
        }

        public void Print(char ch, SpriteEffects effect)
        {
            Print(ch, DefaultForeColorIndex, DefaultBackColorIndex, effect);
        }

        public void PrintSpecial(string str)
        {
            foreach (var s in str)
                PrintSpecial(s);
        }

        public void PrintSpecial(char ch)
        {
            if (ch == '\n')
                Y++;
            else if (ch == '\r')
                X = 0;
            else if (ch == '\t')
                for (var i = 0; i < 5; i++) PrintSpecial(' ');
            else
                PrintSpecial(Character.ToGlyph(ch));
        }

        public void Print(char ch, int fColor, int bColor, SpriteEffects effect)
        {
            if (ch == '\n')
            {
                Y++;
                X = 0;
            }
            else if (ch == '\r')
            {
                X = 0;
            }
            else if (ch == '\t')
            {
                for (var i = 0; i < 5; i++) Print(' ', fColor, bColor, effect);
            }
            else
            {
                Print(Character.ToGlyph(ch), fColor, bColor, effect);
            }
        }

        public void Print(Glyph g, SpriteEffects effect)
        {
            Print(g, DefaultForeColorIndex, DefaultBackColorIndex, effect);
        }

        public void PrintSpecial(Glyph g)
        {
            var c = VideoBuffer[Y * Columns + X];
            VideoBuffer[Y * Columns + X] = new Character(g, c.ForeColor, c.BackColor, c.Effect);
            if (X + 1 >= Columns)
            {
                X = 0;
                if (Y + 1 >= Rows)
                    Y = 0;
                else
                    Y++;
            }
            else
            {
                X++;
            }
        }

        public void Print(Glyph g, int fColor, int bColor, SpriteEffects effect)
        {
            VideoBuffer[Y * Columns + X] = new Character(g, fColor, bColor, effect);
            if (X + 1 >= Columns)
            {
                X = 0;
                if (Y + 1 >= Rows)
                    Y = 0;
                else
                    Y++;
            }
            else
            {
                X++;
            }
        }
    }
}