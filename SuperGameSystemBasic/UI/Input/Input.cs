using System;
using Microsoft.Xna.Framework;

namespace SuperGameSystemBasic.UI.Input
{
    public class Input : IInput
    {
        private int _height;
        private int _width;

        public Input(int xPosition, int yPosition, int width, int height, Window.Window parentWindow,
            string id = null)
        {
            ParentWindow = parentWindow;
            Id = id ?? Guid.NewGuid().ToString();

            PositionX = xPosition;
            PositionY = yPosition;

            _height = height;
            _width = width;
        }

        public Window.Window ParentWindow { get; set; }

        public string Id { get; }
        public bool Selectable { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public int PositionXOnScreen => ParentWindow.PositionXOnScreen + PositionX;

        public int PositionYOnScreen => ParentWindow.PositionYOnScreen + PositionY;

        public virtual int Width
        {
            get => _width;
            set => _width = value;
        }

        public virtual int Height
        {
            get => _height;
            set => _height = value;
        }

        public int ForeColor { get; set; }

        public int BackColor { get; set; }


        public virtual void AddLetter(char letter)
        {
        }

        public virtual void BackSpace()
        {
        }

        public virtual void Delete()
        {
        }

        public virtual void Escape()
        {
        }

        public virtual void PageUp()
        {            
        }

        public virtual void PageDown()
        {
        }

        public virtual void CursorMoveLeft()
        {
        }

        public virtual void CursorMoveRight()
        {
        }

        public virtual void CursorMoveUp()
        {
        }

        public virtual void CursorMoveDown()
        {
        }

        public virtual void CursorToStart()
        {
        }

        public virtual void CursorToEnd()
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Click(int x, int y)
        {
        }

        public virtual void Tab()
        {
        }

        public virtual void Unselect()
        {
        }

        public virtual void Select()
        {
        }


        public virtual void Draw()
        {
        }

        public void SetCursor(int x, int y)
        {
            ClampXY(ref x, ref y);
            ParentWindow.SetCursor(MathHelper.Clamp(x + PositionX, PositionX, PositionX + Width),
                MathHelper.Clamp(y + PositionY, PositionY, PositionY + Height));
        }

        public void Clear(int color)
        {
            FillRect(PositionX, PositionY, Width, Height, color, true);
        }

        public void DrawHLine(int x, int y, int l, int c, bool clear = false)
        {
            ParentWindow.DrawHLine(x, y, l, c, clear);
        }

        public void DrawVLine(int x, int y, int l, int c, bool clear = false)
        {
            ClampXY(ref x, ref y);
            ParentWindow.DrawVLine(x, y, l, c, clear);
        }

        public void FillRect(int x, int y, int w, int h, int c, bool clear = false)
        {
            ParentWindow.FillRect(x, y, w, h, c, clear);
        }

        public void DrawRect(int x, int y, int w, int h, int c, bool clear = false)
        {
            if (x > Width || y > Height) return;
            ClampXY(ref x, ref y);
            w = MathHelper.Clamp(w, 0, Width - x);
            h = MathHelper.Clamp(h, 0, Height - y);
            ParentWindow.DrawRect(x, y, w, h, c, clear);
        }

        public void SetBackGround(int x, int y, int c, bool clear = false)
        {
            if (x > Width || y > Height || x < 0 || y < 0) return;
            ParentWindow.SetBackGround(x + PositionX, y + PositionY, c, clear);
        }

        public void SetForgroundGround(int x, int y, int c, bool clear = false)
        {
            if (x > Width || y > Height || x < 0 || y < 0) return;
            ParentWindow.SetForgroundGround(x + PositionX, y + PositionY, c, clear);
        }

        public int GetXY(int x, int y)
        {
            return ParentWindow.GetXY(x, y);
        }

        public void WriteText(string s, int x, int y, int fColor, int bColor)
        {
            foreach (var l in BasicOne.SplitToLines(s, Width))
            {
                ParentWindow.WriteText(l, x, y, fColor, bColor);
                y++;
            }
        }

        public void WriteGlyph(Glyph g, int x, int y, int fColor, int bColor)
        {
            ParentWindow.WriteGlyph(g, x, y, fColor, bColor);
        }

        private void ClampXY(ref int x, ref int y)
        {
            x = MathHelper.Clamp(x, 0, Width);
            y = MathHelper.Clamp(y, 0, Height);
        }
    }
}