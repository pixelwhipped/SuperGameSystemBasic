using System;

namespace SuperGameSystemBasic.UI.Input
{
    public class Button : Input
    {
        public Button(int x, int y, string text, Window.Window parentWindow, string id = null)
            : base(x, y, text.Length + 2, 1, parentWindow, id)
        {
            Text = text;
            BackColor = parentWindow.BackColor == Terminal.RED
                ? Terminal.LIGHT_RED
                : parentWindow.BackColor == Terminal.GREEN
                    ? Terminal.LIGHT_GREEN
                    : parentWindow.BackColor == Terminal.BLUE
                        ? Terminal.LIGHT_BLUE
                        : Terminal.DARK_GRAY;
            Selectable = true;
        }

        public string Text { get; set; }
        public int TextColor { get; set; } = Terminal.WHITE;

        public int SelectedBackColor { get; set; } = Terminal.WHITE;
        public int SelectedTextColor { get; set; } = Terminal.BLACK;

        public bool Selected { get; set; }

        public Action Action { get; set; }

        public override void Select()
        {
            if (!Selected)
                Selected = true;
        }

        public override void Unselect()
        {
            if (Selected)
                Selected = false;
        }

        public override void Tab()
        {
            ParentWindow.MoveToNextItem();
        }

        public override void Enter()
        {
            Action?.Invoke();
        }

        public override void Click(int x, int y)
        {
            Action?.Invoke();
        }

        public override void Draw()
        {
            var fc = Selected ? SelectedTextColor : TextColor;
            var bc = Selected ? SelectedBackColor : BackColor;
            WriteGlyph(Glyph.BarUpDown, PositionX, PositionY, fc, bc);
            WriteText(Text, PositionX + 1, PositionY, fc, bc);
            WriteGlyph(Glyph.BarUpDown, PositionX + Text.Length + 1, PositionY, fc, bc);
        }

        public override void CursorMoveDown()
        {
            ParentWindow.MovetoNextItemDown(PositionX, PositionY, Width);
        }

        public override void CursorMoveRight()
        {
            ParentWindow.MovetoNextItemRight(PositionX - 1, PositionY + Width, 3);
        }

        public override void CursorMoveLeft()
        {
            ParentWindow.MovetoNextItemLeft(PositionX - 1, PositionY, 3);
        }

        public override void CursorMoveUp()
        {
            ParentWindow.MovetoNextItemUp(PositionX, PositionY, Width);
        }
    }
}