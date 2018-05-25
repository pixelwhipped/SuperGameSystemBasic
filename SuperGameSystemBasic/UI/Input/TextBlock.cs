using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SuperGameSystemBasic.UI.Input
{
    public class TextBlock : Input
    {
        private List<string> _lines = new List<string>();
        private int _scrollOffset;
        private string _text;

        public TextBlock(string text, int x, int y, int width, int height, Window.Window parentWindow,
            string iD = null) : base(x, y, width, height, parentWindow, iD)
        {
            Selectable = true;
            BackColor = parentWindow.BackColor;
            Text = text;
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _lines = BasicOne.SplitToLines(Text, Width).ToList();
                VirtualHeight = _lines.Count;
            }
        }

        public int TextColor { get; set; } = Terminal.WHITE;

        public int VirtualHeight { get; private set; }

        public bool ShowVerticalScroll => VirtualHeight > Height;

        public override void Click(int x, int y)
        {
            if (x == Width - 1 && y == 0)
                _scrollOffset = MathHelper.Clamp(_scrollOffset - 1, 0, VirtualHeight);
            else if (x == Width - 1 && y == Height - 1)
                _scrollOffset = MathHelper.Clamp(_scrollOffset + 1, 0, VirtualHeight);
            else if (x == Width - 1 && y < Height - 1 && y > 0)
                _scrollOffset = MathHelper.Clamp((int) ((float) Math.Max(y - 2, 0) / (Height - 4) * VirtualHeight), 0,
                    VirtualHeight);
        }


        public override void Tab()
        {
            ParentWindow.MoveToNextItem();
        }

        public override void CursorMoveUp()
        {
            _scrollOffset = MathHelper.Clamp(_scrollOffset - 1, 0, VirtualHeight);
        }

        public override void CursorMoveDown()
        {
            _scrollOffset = MathHelper.Clamp(_scrollOffset + 1, 0, VirtualHeight);
        }

        public override void Draw()
        {
            if (ShowVerticalScroll)
            {
                var y = 0;
                for (var i = _scrollOffset; i < VirtualHeight; i++)
                {
                    WriteText(_lines[i], PositionX, PositionY + y, TextColor, BackColor);
                    y++;
                    if (y >= Height) break;
                }
                DrawVLine(Width - 1, 1, Height, Terminal.GRAY, true);
                WriteGlyph(Glyph.TriangleUp, Width - 1, 1, TextColor, Terminal.DARK_GRAY);
                WriteGlyph(Glyph.TriangleDown, Width - 1, Height, TextColor, Terminal.DARK_GRAY);
                WriteText("-", Width - 1, (int) ((float) _scrollOffset / VirtualHeight * (Height - 3)) + 2,
                    Terminal.DARK_GRAY, Terminal.GRAY);
                return;
            }
            WriteText(Text, PositionX, PositionY, TextColor, BackColor);
        }
    }
}