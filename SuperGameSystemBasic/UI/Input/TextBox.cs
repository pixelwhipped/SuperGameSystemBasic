namespace SuperGameSystemBasic.UI.Input
{
    public class TextBox : Input
    {
        private int _cursorPostion;
        private int _offset;

        public TextBox(int x, int y, string text, string id, Window.Window parentWindow, int length = 20)
            : base(x, y, length, 1, parentWindow, id)
        {
            Text = text;
            CursorPostion = text.Length;
            Selectable = true;
            BackColor = Terminal.LIGHT_CYAN;
        }

        public int TextColor { get; set; } = Terminal.WHITE;
        public int SelectedBackColor { get; set; } = Terminal.DARK_GRAY;
        public int SelectedTextColor { get; set; } = Terminal.WHITE;
        public bool Selected { get; set; }

        public int CursorPostion
        {
            get => _cursorPostion;
            set
            {
                _cursorPostion = value;
                SetOffset();
            }
        }

        private string Text { get; set; }

        public override void Select()
        {
            if (!Selected)
                Selected = Selectable;
        }

        public override void Unselect()
        {
            if (Selected)
                Selected = false;
        }

        public override void Enter()
        {
            ParentWindow.MoveToNextItem();
        }

        public override void AddLetter(char letter)
        {
            var textBefore = Text.Substring(0, CursorPostion);
            var textAfter = Text.Substring(CursorPostion, Text.Length - CursorPostion);

            Text = textBefore + letter + textAfter;
            CursorPostion++;
        }

        public override void BackSpace()
        {
            if (CursorPostion == 0) return;
            var textBefore = Text.Substring(0, CursorPostion);
            var textAfter = Text.Substring(CursorPostion, Text.Length - CursorPostion);

            textBefore = textBefore.Substring(0, textBefore.Length - 1);

            Text = textBefore + textAfter;
            CursorPostion--;
        }

        public override void Delete()
        {
            if (CursorPostion >= Text.Length) return;
            var textBefore = Text.Substring(0, CursorPostion);
            var textAfter = Text.Substring(CursorPostion, Text.Length - CursorPostion);

            textBefore = textBefore.Substring(0, textBefore.Length - 1);

            Text = textBefore + textAfter;
        }


        public override void CursorMoveLeft()
        {
            if (CursorPostion != 0)
            {
                CursorPostion--;
                Draw();
            }
            else
            {
                ParentWindow.MovetoNextItemLeft(PositionX - 1, PositionY, 3);
            }
        }

        public override void CursorMoveRight()
        {
            if (CursorPostion != Text.Length)
            {
                CursorPostion++;
                Draw();
            }
            else
            {
                ParentWindow.MovetoNextItemRight(PositionX - 1, PositionY + Width, 3);
            }
        }

        public override void CursorToStart()
        {
            CursorPostion = 0;
        }

        public override void CursorToEnd()
        {
            CursorPostion = Text.Length;
        }

        public string GetText()
        {
            return Text;
        }

        public void SetText(string text)
        {
            Text = text;
        }

        public override void Draw()
        {
            string clippedPath;
            FillRect(PositionX, PositionY, Width, Height, BackColor, true);
            if (Selected)
                clippedPath = ' ' + Text.PadRight(Width + _offset, ' ').Substring(_offset, Width - 2);
            else
                clippedPath = ' ' + Text.PadRight(Width, ' ').Substring(0, Width - 2);

            WriteText(clippedPath + " ", PositionX, PositionY, TextColor, BackColor);
        }

        private void SetOffset()
        {
            while (CursorPostion - _offset > Width - 2)
                _offset++;

            while (CursorPostion - _offset < 0)
                _offset--;
        }


        public override void CursorMoveDown()
        {
            ParentWindow.MovetoNextItemDown(PositionX, PositionY, Width);
        }

        public override void CursorMoveUp()
        {
            ParentWindow.MovetoNextItemUp(PositionX, PositionY, Width);
        }
    }
}