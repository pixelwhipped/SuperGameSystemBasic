using System;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class GlyphWindow : Window
    {
        public GlyphWindow(int postionX, int postionY, IWindow parentWindow)
            : base(postionX, postionY, 36, 9, parentWindow)
        {
            BackColor = Terminal.YELLOW;
            ForeColor = Terminal.BLACK;
            Inputs.Add(new Button(30, Height - 1, "COPY", this)
            {
                Action = () =>
                {
                    CurrentlySelected = null;
                    var data = new DataPackage();
                    data.SetText($"@{Enum.GetName(typeof(Glyph), CurrentGlyph)}".ToUpper());
                    Clipboard.SetContent(data);
                }
            });
        }

        public string Title { get; set; } = "Glyphs";

        public int GlyphX { get; set; }
        public int GlyphY { get; set; }
        public int CurrentGlyph => GlyphY * 32 + GlyphX;

        public override void Click(int x, int y, bool handled)
        {
            if (x == Width - 1 && y == 0) ExitWindow();
            if (x >= 1 && y >= 1 && x <= 32 && y <= 6)
            {
                GlyphX = x - 1;
                GlyphY = y - 1;
            }
        }

        public override bool Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
        {
            if (key == Keys.Right)
                GlyphX = MathHelper.Clamp(GlyphX + 1, 0, 31);
            if (key == Keys.Left)
                GlyphX = MathHelper.Clamp(GlyphX - 1, 0, 31);
            if (key == Keys.Up)
                GlyphY = MathHelper.Clamp(GlyphY - 1, 0, 5);
            if (key == Keys.Down)
                GlyphY = MathHelper.Clamp(GlyphY + 1, 0, 5);
            return base.Update(key, getKey, click, mouseX, mouseY);
        }

        public override void Draw()
        {
            if (!Visible) return;
            var pl = (Width - 1 - Title.Length) / 2;
            WriteText((" ".PadLeft(pl) + Title).PadRight(Width - 1, ' '), 0, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.Close, Width - 1, 0, TitleColour, TitleBarColour);
            FillRect(0, +1, Width, Height - 1, BackColor, true); //Main Box     

            for (var i = 0; i < 32; i++)
            for (var j = 0; j < 6; j++)
            {
                var g = j * 32 + i;
                WriteGlyph((Glyph) g, i + 1, j + 1, ForeColor, g == CurrentGlyph ? Terminal.WHITE : BackColor);
            }
            WriteText($"{CurrentGlyph} @{Enum.GetName(typeof(Glyph), CurrentGlyph).ToUpper()}", 1, 7, ForeColor,
                BackColor);
            base.Draw();
        }
    }
}