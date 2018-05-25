using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class FullWindow : IWindow
    {
        public List<Window> Windows = new List<Window>();

        public FullWindow(BasicOne basicOne)
        {
            BasicOne = basicOne;
            ParentWindow = this;
        }

        public BasicOne BasicOne { get; set; }
        public IWindow ParentWindow { get; set; }

        public Window CurrentWindow { get; set; }
        public Window PreviousWindow { get; set; }

        public bool SkipNextWindowSelect { get; set; }
        public bool IsDialog { get; } = false;
        public int PostionX { get; } = 0;
        public int PostionY { get; } = 0;
        public int PositionXOnScreen => PostionY;
        public int PositionYOnScreen => PostionY;
        public int Width => BasicOne.IdeTerminal.Columns;
        public int Height => BasicOne.IdeTerminal.Rows;
        public int ForeColor { get; set; } = Terminal.WHITE;
        public int BackColor { get; set; } = Terminal.GREEN;

        public void Draw()
        {
            FillRect(PostionX, PostionY, Width, Height, BackColor, true);
            foreach (var w in Windows.Where(p => p.Visible))
                w.Draw();
        }

        public void Select()
        {
        }

        public void Unselect()
        {
        }

        public void SetCursor(int x, int y)
        {
            BasicOne.SetCursor(x, y);
        }

        public void Clear(int color)
        {
            BasicOne.Clear(color);
        }

        public void DrawHLine(int x, int y, int l, int c, bool clear = false)
        {
            BasicOne.DrawHLine(x, y, l, c, clear);
        }

        public void DrawVLine(int x, int y, int l, int c, bool clear = false)
        {
            BasicOne.DrawVLine(x, y, l, c, clear);
        }

        public void FillRect(int x, int y, int w, int h, int c, bool clear = false)
        {
            BasicOne.FillRect(x, y, w, h, c, clear);
        }

        public void DrawRect(int x, int y, int w, int h, int c, bool clear = false)
        {
            BasicOne.DrawRect(x, y, w, h, c, clear);
        }

        public void SetBackGround(int x, int y, int c, bool clear = false) => BasicOne.SetBackGround(x, y, c, clear);

        public void SetForgroundGround(int x, int y, int c, bool clear = false) => BasicOne.SetForgroundGround(x, y, c, clear);

        public int GetXY(int x, int y) => BasicOne.GetXY(x, y);

        public void WriteText(string s, int x, int f, int fColor, int bColor) => BasicOne.WriteText(s, x, f, fColor, bColor);

        public void WriteGlyph(Glyph g, int x, int y, int fColor, int bColor) => BasicOne.WriteGlyph(g, x, y, fColor, bColor);

        public void Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
        {
            if (key == Keys.None && !click) return;
            if (key == Keys.F2)
            {
                SelectNextWindow();
                return;
            }
            if (CurrentWindow is MenuItemWindow && CurrentWindow.Visible)
            {
                var mx = BasicOne.MouseTerminalLocation.X - CurrentWindow.PositionXOnScreen;
                var my = BasicOne.MouseTerminalLocation.Y - CurrentWindow.PositionYOnScreen;
                if (!(mx >= 0 && mx < CurrentWindow.Width && my >= 0 && my < CurrentWindow.Height))
                    CurrentWindow.Visible = false;
            }
            var remove = new List<Window>();
            if (CurrentWindow == null && Windows.Any(p => p.Visible && !p.Exit))
            {
                var select = Windows.First(p => p.Visible && !p.Exit);
                SelectWindow(select);
            }
            var clicked = false;

            if (CurrentWindow != null && !CurrentWindow.IsDialog)
            {
                Window nextWindow = null;
                foreach (var w in Windows.Where(p => p.Visible).Reverse())
                {
                    if (!clicked)
                        if (click)
                        {
                            var mx = BasicOne.MouseTerminalLocation.X - w.PositionXOnScreen;
                            var my = BasicOne.MouseTerminalLocation.Y - w.PositionYOnScreen;
                            if (mx >= 0 && mx < w.Width && my >= 0 && my <= w.Height)
                            {
                                clicked = true;
                                if (w.Update(key, getKey, true, mx, my))
                                    nextWindow = w;
                            }
                        }
                        else
                        {
                            if (w == CurrentWindow) CurrentWindow?.Update(key, getKey, false, 0, 0);
                        }
                    if (w.Exit) remove.Add(w);
                }
                if (nextWindow != null && nextWindow.Visible && !nextWindow.Exit) SelectWindow(nextWindow);
               // else if (PreviousWindow != null && nextWindow.Visible && !nextWindow.Exit) SelectWindow(nextWindow);
            }
            else if (CurrentWindow != null && CurrentWindow.IsDialog)
            {
                var mx = BasicOne.MouseTerminalLocation.X - CurrentWindow.PositionXOnScreen;
                var my = BasicOne.MouseTerminalLocation.Y - CurrentWindow.PositionYOnScreen;
                CurrentWindow.Update(key, getKey, click, mx, my);
            }
            Windows.RemoveAll(p => remove.Contains(p) || p.Exit);
            if (!Windows.Contains(CurrentWindow)) CurrentWindow = null;
        }

        public void SelectWindow(Window window)
        {
            if (SkipNextWindowSelect)
            {
                SkipNextWindowSelect = false;
                return;
            }
            if (window == null)
                return;
            if (!Windows.Contains(window) && !window.Exit)
            {
                window.Visible = true;
                Windows.Add(window);
            }
            if (CurrentWindow != window)
            {
                foreach (var w in Windows.Where(p => p != window))
                {
                    w.TitleBarColour = Terminal.BLACK;
                    w.TitleColour = Terminal.WHITE;
                    w.Unselect();
                    if (w is MenuItemWindow)
                        w.Visible = false;
                }
                window.Select();
            }
            PreviousWindow = CurrentWindow is MenuWindow || CurrentWindow is MenuItemWindow
                ? PreviousWindow
                : CurrentWindow;
            CurrentWindow = window;
            CurrentWindow.TitleBarColour = Terminal.WHITE;
            CurrentWindow.TitleColour = Terminal.BLACK;
            BasicOne.CursorVisible = false;
            Windows.Remove(CurrentWindow);
            Windows.Add(CurrentWindow);
        }

        public void SelectNextWindow()
        {
            if (CurrentWindow != null && CurrentWindow.Visible && !CurrentWindow.IsDialog)
            {
                if (!Windows.Any(p => p.Visible && !p.Exit && !(p is MenuWindow) && p != PreviousWindow)) return;
                var doNext = false;
                foreach (var e in Windows.Where(p => p.Visible && !p.Exit && !(p is MenuWindow) && p != PreviousWindow))
                {
                    if (doNext)
                    {
                        SelectWindow(e);
                        return;
                    }
                    if (e == CurrentWindow) doNext = true;
                }
                SelectWindow(Windows.First(p => p.Visible && !p.Exit));
                return;
            }
            if (!Windows.Any(p => p.Visible && !p.Exit && !(p is MenuWindow))) return;
            SelectWindow(Windows.First(p => p.Visible && !p.Exit && !(p is MenuWindow)));
        }

        public void SelectPreviousWindow()
        {
            if (PreviousWindow != null && PreviousWindow.Visible && !PreviousWindow.Exit && !(PreviousWindow is MenuWindow))
            {
                foreach (var w in Windows.Where(p => p != PreviousWindow))
                {
                    w.TitleBarColour = Terminal.BLACK;
                    w.TitleColour = Terminal.WHITE;
                    w.Unselect();
                    if (w is MenuItemWindow)
                        w.Visible = false;
                }
                PreviousWindow.Select();
                CurrentWindow = PreviousWindow;
                CurrentWindow.TitleBarColour = Terminal.WHITE;
                CurrentWindow.TitleColour = Terminal.BLACK;
                BasicOne.CursorVisible = false;
                Windows.Remove(CurrentWindow);
                Windows.Add(CurrentWindow);
            }
        }
    }
}