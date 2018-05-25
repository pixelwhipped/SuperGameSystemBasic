using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class Window : IWindow
    {
        protected IInput CurrentlySelected;
        public bool Exit;

        public List<IInput> Inputs = new List<IInput>();

        public Window(int postionX, int postionY, int width, int height, IWindow parentWindow)
        {
            PostionY = postionY;
            PostionX = postionX;
            Width = width;
            Height = height;
            ParentWindow = parentWindow;
        }

        public IWindow ParentWindow { get; set; }

        public bool Visible { get; set; } = true;

        public int TitleBarColour { get; set; } = Terminal.BLACK;
        public int TitleColour { get; set; } = Terminal.WHITE;
        public bool IsDialog { get; set; } = false;
        public int PostionX { get; set; }
        public int PostionY { get; set; }
        public int PositionXOnScreen => ParentWindow.PositionXOnScreen + PostionX;
        public int PositionYOnScreen => ParentWindow.PositionYOnScreen + PostionY;
        public int Width { get; set; }
        public int Height { get; set; }
        public int ForeColor { get; set; } = Terminal.WHITE;
        public int BackColor { get; set; } = Terminal.BLUE;

        public virtual void Draw()
        {
            if (!Visible) return;
            foreach (var input in Inputs)
                input.Draw();
            CurrentlySelected?.Select();
        }

        public virtual void Select()
        {
        }

        public virtual void Unselect()
        {
        }

        public void SetCursor(int x, int y)
        {
            ClampXY(ref x, ref y);
            ParentWindow.SetCursor(MathHelper.Clamp(x + PostionX, PostionX, PostionX + Width),
                MathHelper.Clamp(y + PostionY, PostionY, PostionY + Height));
        }

        public void Clear(int color)
        {
            FillRect(PostionX, PostionY, Width, Height, color, true);
        }

        public void DrawHLine(int x, int y, int l, int c, bool clear = false)
        {
            ParentWindow.DrawHLine(x + PostionX, y + PostionY, l, c, clear);
        }

        public void DrawVLine(int x, int y, int l, int c, bool clear = false)
        {
            ClampXY(ref x, ref y);
            l = MathHelper.Clamp(l, 0, Height - y);
            ParentWindow.DrawVLine(x + PostionX, y + PostionY, l, c, clear);
        }

        public void FillRect(int x, int y, int w, int h, int c, bool clear = false)
        {
            ParentWindow.FillRect(x + PostionX, y + PostionY, w, h, c, clear);
        }

        public void DrawRect(int x, int y, int w, int h, int c, bool clear = false)
        {
            if (x > Width || y > Height) return;
            ClampXY(ref x, ref y);
            w = MathHelper.Clamp(w, 0, Width - x);
            h = MathHelper.Clamp(h, 0, Height - y);
            ParentWindow.DrawRect(x + PostionX, y + PostionY, w, h, c, clear);
        }

        public void SetBackGround(int x, int y, int c, bool clear = false)
        {
            if (x > Width || y > Height || x < 0 || y < 0) return;
            ParentWindow.SetBackGround(x + PostionX, y + PostionY, c, clear);
        }

        public void SetForgroundGround(int x, int y, int c, bool clear = false)
        {
            if (x > Width || y > Height || x < 0 || y < 0) return;
            ParentWindow.SetForgroundGround(x + PostionX, y + PostionY, c, clear);
        }

        public int GetXY(int x, int y) => ParentWindow.GetXY(x, y);

        public void WriteText(string s, int x, int y, int fColor, int bColor)
        {
            foreach (var l in BasicOne.SplitToLines(s, Width))
            {
                ParentWindow.WriteText(l, x + PostionX, y + PostionY, fColor, bColor);
                y++;
            }
        }

        public void WriteGlyph(Glyph g, int x, int y, int fColor, int bColor) => ParentWindow.WriteGlyph(g, x + PostionX, y + PostionY, fColor, bColor);


        public virtual bool Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
        {
            if (!Visible) return false;
            char? c;
            if (key == Keys.Tab)
            {
                if (CurrentlySelected == null && Inputs.Any())
                    SelectFirstItem();
                else
                    CurrentlySelected?.Tab();
            }
            else if (key == Keys.Escape)
            {
                CurrentlySelected?.Escape();
            }
            else if (key == Keys.PageUp)
            {
                CurrentlySelected?.PageUp();
            }
            else if (key == Keys.PageDown)
            {
                CurrentlySelected?.PageDown();
            }
            else if (key == Keys.Enter)
            {
                CurrentlySelected?.Enter();
            }
            else if (key == Keys.Delete)
            {
                CurrentlySelected?.Delete();
            }
            else if (key == Keys.Left)
            {
                CurrentlySelected?.CursorMoveLeft();
            }
            else if (key == Keys.Right)
            {
                CurrentlySelected?.CursorMoveRight();
            }
            else if (key == Keys.Up)
            {
                CurrentlySelected?.CursorMoveUp();
            }
            else if (key == Keys.Down)
            {
                CurrentlySelected?.CursorMoveDown();
            }
            else if (key == Keys.Back)
            {
                CurrentlySelected?.BackSpace();
            }
            else if (key == Keys.Home)
            {
                CurrentlySelected?.CursorToStart();
            }
            else if (key == Keys.End)
            {
                CurrentlySelected?.CursorToEnd();
            }
            else if ((c = getKey?.Invoke()) != null)
            {
                CurrentlySelected?.AddLetter((char) c); // Letter(input.KeyChar);  
            }
            else if (click)
            {
                var handled = false;
                foreach (var i in Inputs)
                {
                    var rect = new Rectangle(i.PositionX, i.PositionY, i.Width, i.Height);
                    if (rect.Contains(new Point(mouseX, mouseY)))
                    {
                        var mx = mouseX - i.PositionX;
                        var my = mouseY - i.PositionY;
                        if (i.Selectable)
                        {
                            CurrentlySelected = i;
                            SetSelected();
                        }
                        i.Click(mx, my);
                        handled = true;
                    }
                }
                Click(mouseX, mouseY, handled);
            }
            return true;
        }

        public virtual void Click(int x, int y, bool handled)
        {
        }

        public void SelectFirstItem()
        {
            if (Inputs.All(x => !x.Selectable)) //No Selectable inputs on page
                return;

            CurrentlySelected = Inputs.First(x => x.Selectable);

            SetSelected();
        }

        public void SelectItemByID(string id)
        {
            if (Inputs.All(x => !x.Selectable)) //No Selectable inputs on page
                return;

            var newSelectedInput = Inputs.FirstOrDefault(x => x.Id == id);
            if (newSelectedInput == null) //No Input with this ID
                return;

            CurrentlySelected = newSelectedInput;

            SetSelected();
        }

        public void MoveToNextItem()
        {
            if (Inputs.All(x => !x.Selectable))
                return;

            if (Inputs.Count(x => x.Selectable) == 1)
                return;

            var indexOfCurrent = Inputs.IndexOf(CurrentlySelected);

            while (true)
            {
                indexOfCurrent = MoveIndexBackOne(indexOfCurrent);

                if (Inputs[indexOfCurrent].Selectable)
                    break;
            }
            CurrentlySelected = Inputs[indexOfCurrent];

            SetSelected();
        }

        public void MoveToLastItem()
        {
            if (Inputs.All(x => !x.Selectable)) //No Selectable inputs on page
                return;

            if (Inputs.Count(x => x.Selectable) == 1) //Only one selectable input on page, thus no point chnaging it
                return;

            var indexOfCurrent = Inputs.IndexOf(CurrentlySelected);

            while (true)
            {
                indexOfCurrent = MoveIndexBackOne(indexOfCurrent);

                if (Inputs[indexOfCurrent].Selectable)
                    break;
            }
            CurrentlySelected = Inputs[indexOfCurrent];

            SetSelected();
        }

        public void MovetoNextItemRight(int startX, int startY, int searchHeight)
        {
            if (Inputs.All(x => !x.Selectable)) //No Selectable inputs on page
                return;

            if (Inputs.Count(x => x.Selectable) == 1) //Only one selectable input on page, thus no point chnaging it
                return;
            if (CurrentlySelected == null) return;
            var inputs =
                Inputs.Where(x => x.Selectable && x != CurrentlySelected && x.PositionX > CurrentlySelected.PositionX);
            var enumerable = inputs as IInput[] ?? inputs.ToArray();
            if (!enumerable.Any()) return;
            var loc = new Vector2(CurrentlySelected.PositionX, CurrentlySelected.PositionY);

            var nextItem =
                enumerable.Select(
                        p => new {distance = Vector2.DistanceSquared(loc, new Vector2(p.PositionX, p.PositionY)), p})
                    .OrderBy(x => x.distance)
                    .First().p;          
            CurrentlySelected = nextItem;
            SetSelected();
        }

        public void MovetoNextItemLeft(int startX, int startY, int searchHeight)
        {
            if (Inputs.All(x => !x.Selectable)) //No Selectable inputs on page
                return;

            if (Inputs.Count(x => x.Selectable) == 1) //Only one selectable input on page, thus no point chnaging it
                return;
            if (CurrentlySelected == null) return;
            var inputs =
                Inputs.Where(x => x.Selectable && x != CurrentlySelected && x.PositionX < CurrentlySelected.PositionX);
            var enumerable = inputs as IInput[] ?? inputs.ToArray();
            if (!enumerable.Any()) return;
            var loc = new Vector2(CurrentlySelected.PositionX, CurrentlySelected.PositionY);

            var nextItem =
                enumerable.Select(
                        p => new {distance = Vector2.DistanceSquared(loc, new Vector2(p.PositionX, p.PositionY)), p})
                    .OrderBy(x => x.distance)
                    .First().p;

            CurrentlySelected = nextItem;
            SetSelected();
        }

        public void MovetoNextItemDown(int startX, int startY, int searchWidth)
        {
            if (Inputs.All(x => !x.Selectable)) //No Selectable inputs on page
                return;

            if (Inputs.Count(x => x.Selectable) == 1) //Only one selectable input on page, thus no point chnaging it
                return;

            if (CurrentlySelected == null) return;
            var inputs =
                Inputs.Where(x => x.Selectable && x != CurrentlySelected && x.PositionY > CurrentlySelected.PositionY);
            var enumerable = inputs as IInput[] ?? inputs.ToArray();
            if (!enumerable.Any()) return;
            var loc = new Vector2(CurrentlySelected.PositionX, CurrentlySelected.PositionY);

            var nextItem =
                enumerable.Select(
                        p => new {distance = Vector2.DistanceSquared(loc, new Vector2(p.PositionX, p.PositionY)), p})
                    .OrderBy(x => x.distance)
                    .First().p;


            CurrentlySelected = nextItem;
            SetSelected();
        }

        public void MovetoNextItemUp(int startX, int startY, int searchWidth)
        {
            if (Inputs.All(x => !x.Selectable)) //No Selectable inputs on page
                return;

            if (Inputs.Count(x => x.Selectable) == 1) //Only one selectable input on page, thus no point chnaging it
                return;

            if (CurrentlySelected == null) return;
            var inputs =
                Inputs.Where(x => x.Selectable && x != CurrentlySelected && x.PositionY < CurrentlySelected.PositionY);
            var enumerable = inputs as IInput[] ?? inputs.ToArray();
            if (!enumerable.Any()) return;
            var loc = new Vector2(CurrentlySelected.PositionX, CurrentlySelected.PositionY);

            var nextItem =
                enumerable.Select(
                        p => new {distance = Vector2.DistanceSquared(loc, new Vector2(p.PositionX, p.PositionY)), p})
                    .OrderBy(x => x.distance)
                    .First().p;
          
            CurrentlySelected = nextItem;
            SetSelected();
        }


        private int MoveIndexBackOne(int index)
        {
            if (index == 0)
                return Inputs.Count - 1;

            return index - 1;
        }

        public void SetSelected()
        {
            Inputs.ForEach(x => x.Unselect());
            CurrentlySelected?.Select();
        }

        public IInput GetInputById(string id)
        {
            return Inputs.FirstOrDefault(x => x.Id == id);
        }

        public void ExitWindow()
        {
            Visible = false;
            Exit = true;            
            (ParentWindow as FullWindow)?.SelectPreviousWindow();
        }

        private void ClampXY(ref int x, ref int y)
        {
            x = MathHelper.Clamp(x, 0, Width);
            y = MathHelper.Clamp(y, 0, Height);
        }

        public void Center()
        {
            PostionX = ParentWindow.Width / 2 - Width / 2;
            PostionY = ParentWindow.Height / 2 - Height / 2;
        }
    }
}