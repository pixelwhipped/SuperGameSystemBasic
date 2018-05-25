using System;

namespace SuperGameSystemBasic.UI.Input
{
    public class CheckBox : Input
    {
        private bool _selected;
        public int TextColour = Terminal.WHITE;

        public CheckBox(int x, int y, string title, Window.Window parentWindow, string id = null) : base(x, y,
            3 + (title ?? string.Empty).Length, 1, parentWindow, id)
        {
            Title = title ?? string.Empty;
            BackColor = parentWindow.BackColor;
            Selectable = true;
        }

        public string Title { get; set; }

        public int SelectedBackColor { get; set; } = Terminal.DARK_GRAY;
        public int SelectedTextColor { get; set; } = Terminal.WHITE;

        public bool Selected
        {
            get => _selected;

            set
            {
                if (value)
                    Select();
                else
                    Unselect();
            }
        }

        public bool Checked { get; set; }

        public Action Action { get; set; }

        public override void Select()
        {
            if (_selected) return;
            _selected = true;
        }

        public override void Unselect()
        {
            if (!_selected) return;
            _selected = false;
        }

        public override void Enter()
        {
            Checked = !Checked; //Toggle Checked            
            Action?.Invoke();
        }

        public override void Draw()
        {
            WriteText($"{Title}[{(Checked ? "X" : " ")}]", PositionX, PositionY,
                Selected ? SelectedTextColor : TextColour,
                Selected ? SelectedBackColor : BackColor);
        }

        public override void Click(int x, int y)
        {
            if (Selected) Enter();
            base.Click(x, y);
        }

        public override void CursorMoveDown()
        {
            ParentWindow.MovetoNextItemDown(PositionX + 1, PositionY, Width);
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
            ParentWindow.MovetoNextItemUp(PositionX - 1, PositionY, Width);
        }
    }
}