using System;
using System.Linq;

namespace SuperGameSystemBasic.UI.Input
{
    public class RadioButton : Input
    {
        private readonly bool _selected = false;

        public RadioButton(int x, int y, string id, string radioGroup, Window.Window parentWindow)
            : base(x, y, 3, 1, parentWindow, id)
        {
            RadioGroup = radioGroup;
            BackColor = parentWindow.BackColor;
            Selectable = true;
        }

        public int TextColor { get; set; } = Terminal.BLACK;
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

        public string RadioGroup { get; }
        public bool Checked { get; set; }
        public Action Action { get; set; }

        public override void Select()
        {
            if (Selected) return;
            Selected = true;
            Draw();
        }

        public override void Unselect()
        {
            if (!Selected) return;
            Selected = false;
            Draw();
        }

        public override void Enter()
        {
            if (Checked) //Already checked, no need to change
                return;

            //Uncheck all other Radio Buttons in the group
            ParentWindow.Inputs.OfType<RadioButton>()
                .Where(x => x.RadioGroup == RadioGroup)
                .ToList()
                .ForEach(x => x.Uncheck());

            Checked = true;

            Draw();

            Action?.Invoke();
        }

        public void Uncheck()
        {
            if (!Checked) //Already unchecked, no need to change
                return;
            Checked = false;
            Draw();
        }

        public override void Draw()
        {
            WriteText($"[{(Checked ? (char) 95 : ' ')}]", PositionX, PositionY,
                Selected ? SelectedTextColor : TextColor, Selected ? SelectedBackColor : BackColor);
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