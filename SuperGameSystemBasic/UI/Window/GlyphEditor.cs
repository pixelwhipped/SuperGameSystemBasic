using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class GlyphEditor : Window
    {
        public bool[] GlyphArray = new bool[20 * 24];

        public GlyphEditor(int postionX, int postionY, IWindow parentWindow, BasicOne basicOne)
            : base(postionX, postionY, 31, 24, parentWindow)
        {
            BasicOne = basicOne;
            BackColor = Terminal.YELLOW;
            ForeColor = Terminal.BLACK;
            Inputs.Add(new Button(22, 1, "COPY ", this)
            {
                Action = () =>
                {
                    CurrentlySelected?.Unselect();
                    CurrentlySelected = null;
                    var data = new DataPackage();
                    data.SetText($"{GetGlyph()}");
                    Clipboard.SetContent(data);
                }
            });
            Inputs.Add(new Button(22, 3, "PASTE", this)
            {
                Action = () =>
                {
                    CurrentlySelected?.Unselect();
                    CurrentlySelected = null;

                    Paste();
                }
            });

            Inputs.Add(new Button(22, 5, "NEG  ", this)
            {
                Action = () =>
                {
                    CurrentlySelected?.Unselect();
                    CurrentlySelected = null;

                    Negative();
                }
            });
        }

        public string Title { get; set; } = "Glyph Editor";

        public int GlyphX { get; set; }
        public int GlyphY { get; set; }
        public int Offset => GlyphY * 20 + GlyphX;
        public BasicOne BasicOne { get; set; }

        private void Negative()
        {
            var cd = new List<bool>();
            foreach (var g in GlyphArray) cd.Add(!g);
            GlyphArray = cd.ToArray();
        }

        private void PasteGlyphValue(byte[] bitmap)
        {
            try
            {
                var cd = new List<bool>();
                for (var y = 0; y < 24; y++)
                for (var x = 0; x < 20; x++)
                {
                    var p = GetPixel(bitmap, y, x, 20, 24);
                    if (p.R < 128 && p.G < 128 && p.B < 128 && p.A == 255)
                        cd.Add(true);
                    else
                        cd.Add(false);
                }
                if (cd.Count == GlyphArray.Length) GlyphArray = cd.ToArray();
            }
            catch
            {
            }
        }

        public Color GetPixel(byte[] pixels, int x, int y, uint width, uint height)
        {
            var i = x;
            var j = y;
            var k = (i * (int) width + j) * 4;
            var r = pixels[k + 0];
            var g = pixels[k + 1];
            var b = pixels[k + 2];
            return new Color(r, g, b, (byte) 255);
        }

        private void PasteGlyphValue(string name)
        {
            name = name.Trim();
            foreach (var g in Enum.GetValues(typeof(Glyph)))
                if ($"@{Enum.GetName(typeof(Glyph), g)}".Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    var gd = BasicOne.GetGlyphData(Array.IndexOf(Enum.GetValues(typeof(Glyph)), g), BasicOne);
                    if (gd.Length == GlyphArray.Length) GlyphArray = gd;
                    return;
                }
        }

        private string GetGlyph()
        {
            var sb = new StringBuilder();
            sb.Append("DIM myGlyph = {");
            foreach (var v in GlyphArray)
                sb.Append($"{(v ? 1 : 0)},");
            sb.Append("}");
            return sb.ToString().Replace(",}", "}");
        }

        public override void Click(int x, int y, bool handled)
        {
            if (x == Width - 1 && y == 0) ExitWindow();
            if (x >= 1 && y >= 0 && x <= 20 && y <= 24)
            {
                CurrentlySelected = null;
                GlyphX = x - 1;
                GlyphY = y;
                var g = GlyphY * 20 + GlyphX;
                GlyphArray[g] = !GlyphArray[g];
            }
        }

        public override bool Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
        {
            if (key == Keys.Right)
                GlyphX = MathHelper.Clamp(GlyphX + 1, 0, 19);
            if (key == Keys.Left)
                GlyphX = MathHelper.Clamp(GlyphX - 1, 0, 19);
            if (key == Keys.Up)
                GlyphY = MathHelper.Clamp(GlyphY - 1, 0, 23);
            if (key == Keys.Down)
                GlyphY = MathHelper.Clamp(GlyphY + 1, 0, 23);
            if (key == Keys.F4)
                Paste();
            if (key == Keys.Enter || key == Keys.Space && CurrentlySelected == null)
            {
                var g = GlyphY * 20 + GlyphX;
                GlyphArray[g] = !GlyphArray[g];
            }
            return base.Update(key, getKey, click, mouseX, mouseY);
        }

        private void Paste()
        {
            var data = Clipboard.GetContent();
            if (data.Contains(StandardDataFormats.Bitmap))
            {
                var bitmap = Task.Run(async () =>
                {
                    try
                    {
                        var t = await data.GetBitmapAsync();
                        var t2 = await t.OpenReadAsync();
                        var dec = await BitmapDecoder.CreateAsync(t2.AsStream().AsRandomAccessStream());
                        if (dec.PixelWidth != 20 && dec.PixelHeight != 24) return null;
                        var pd = await dec.GetPixelDataAsync();
                        var bytes = pd.DetachPixelData();
                        return bytes;
                    }
                    catch
                    {
                        return null;
                    }
                }).Result;
                if (bitmap == null) return;
                PasteGlyphValue(bitmap);
                return;
            }
            if (!data.Contains(StandardDataFormats.Text)) return;
            var stringData = Task.Run(async () =>
            {
                try
                {
                    var clipboardAsync = data.GetTextAsync();
                    return await clipboardAsync;
                }
                catch
                {
                    return null;
                }
            }).Result;
            if (stringData.StartsWith("@"))
                PasteGlyphValue(stringData);
            else
                try
                {
                    stringData = stringData.Substring(stringData.IndexOf('{'),
                        stringData.IndexOf('}') - stringData.IndexOf('{'));
                    var cd = new List<bool>();
                    stringData = stringData.Replace('{', ' ');
                    stringData = stringData.Replace('}', ' ');
                    stringData = stringData.Trim();
                    foreach (var d in stringData.Split(','))
                        if (int.TryParse(d, out var b))
                            cd.Add(b > 0);
                        else
                            return;
                    if (cd.Count == GlyphArray.Length) GlyphArray = cd.ToArray();
                }
                catch
                {
                }
        }

        public override void Draw()
        {
            if (!Visible) return;
            var pl = (Height - 1 - Title.Length) / 2;
            DrawVLine(Width - 1, 0, Height, TitleBarColour, true);
            for (var index = 0; index < Title.Length; index++)
            {
                var c = Title[index];
                WriteText(c.ToString(), Width - 1, pl + index, TitleColour, TitleBarColour);
            }
            WriteGlyph(Glyph.Close, Width - 1, 0, TitleColour, TitleBarColour);
            FillRect(0, 0, Width - 1, Height, BackColor, true); //Main Box     


            for (var i = 0; i < 20; i++)
            for (var j = 0; j < 24; j++)
            {
                var g = j * 20 + i;
                WriteGlyph(Glyph.Space, i + 1, j, ForeColor,
                    g == Offset ? Terminal.RED : GlyphArray[g] ? Terminal.BLACK : Terminal.GRAY);
            }
            // WriteText($"{CurrentGlyph} @{Enum.GetName(typeof(Glyph), CurrentGlyph).ToUpper()}", 1, 7, ForeColor,
            //     BackColor);
            base.Draw();
        }
    }
}