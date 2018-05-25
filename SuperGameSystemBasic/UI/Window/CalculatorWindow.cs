using System;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class CalculatorWindow : Window
    {
        private readonly TextBox _display;
        private bool _displayingTotal = true;
        private char _lastOp = '=';
        private bool _pointUsed;

        private double _total;

        public CalculatorWindow(int x, int y, IWindow parentWindow) : base(x, y, 17, 14, parentWindow)
        {
            _display = new TextBox(1, 2, "0", "displayTxtBox", this, 15) {Selectable = false};

            var pointBtn = new Button(5, 12, ".", this, "pointBtn") {Action = AddPoint};
            var clearBtn = new Button(1, 4, "C", this, "clearBtn") {Action = Clear};

            var zeroBtn = new Button(1, 12, "0", this, "zeroBtn") {Action = delegate { Number('0'); }};
            var oneBtn = new Button(1, 10, "1", this, "oneBtn") {Action = delegate { Number('1'); }};
            var twoBtn = new Button(5, 10, "2", this, "twoBtn") {Action = delegate { Number('2'); }};
            var threeBtn = new Button(9, 10, "3", this, "threeBtn") {Action = delegate { Number('3'); }};
            var fourBtn = new Button(1, 8, "4", this, "fourBtn") {Action = delegate { Number('4'); }};
            var fiveBtn = new Button(5, 8, "5", this, "fiveBtn") {Action = delegate { Number('5'); }};
            var sixBtn = new Button(9, 8, "6", this, "sixBtn") {Action = delegate { Number('6'); }};
            var sevenBtn = new Button(1, 6, "7", this, "sevenBtn") {Action = delegate { Number('7'); }};
            var eightBtn = new Button(5, 6, "8", this, "eightBtn") {Action = delegate { Number('8'); }};
            var nineBtn = new Button(9, 6, "9", this, "nineBtn") {Action = delegate { Number('9'); }};


            var minusBtn = new Button(13, 4, "-", this, "minusBtn") {Action = delegate { Operator('-'); }};
            var addBtn = new Button(13, 6, "+", this, "addBtn") {Action = delegate { Operator('+'); }};
            var timesBtn = new Button(13, 8, "x", this, "timesBtn") {Action = delegate { Operator('*'); }};
            var divideBtn = new Button(13, 10, "/", this, "divideBtn") {Action = delegate { Operator('/'); }};
            EqualsBtn = new Button(9, 12, "  =  ", this, "equalsBtn") {Action = delegate { Operator('='); }};

            Inputs.Add(_display);

            Inputs.Add(clearBtn);
            Inputs.Add(minusBtn);

            Inputs.Add(sevenBtn);
            Inputs.Add(eightBtn);
            Inputs.Add(nineBtn);
            Inputs.Add(addBtn);

            Inputs.Add(fourBtn);
            Inputs.Add(fiveBtn);
            Inputs.Add(sixBtn);

            Inputs.Add(timesBtn);

            Inputs.Add(oneBtn);
            Inputs.Add(twoBtn);
            Inputs.Add(threeBtn);

            Inputs.Add(divideBtn);

            Inputs.Add(zeroBtn);
            Inputs.Add(pointBtn);
            Inputs.Add(EqualsBtn);
            Inputs.Reverse();

            CurrentlySelected = oneBtn;
        }

        public string Title { get; set; } = "Calculator";

        public Button EqualsBtn { get; set; }

        public override bool Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
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
                Clear();
            }
            else if (key == Keys.Enter)
            {
                CurrentlySelected?.Enter();
            }
            else if (key == Keys.Multiply)
            {
                Operator('*');
                CurrentlySelected = EqualsBtn;
                SetSelected();
            }
            else if (key == Keys.Divide)
            {
                Operator('/');
                CurrentlySelected = EqualsBtn;
                SetSelected();
            }
            else if (key == Keys.Add)
            {
                Operator('+');
                CurrentlySelected = EqualsBtn;
                SetSelected();
            }
            else if (key == Keys.Subtract)
            {
                Operator('-');
                CurrentlySelected = EqualsBtn;
                SetSelected();
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
                AddLetter((char) c); // Letter(input.KeyChar);  
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

        private void AddLetter(char c)
        {
            switch (c)
            {
                case '0':
                    Number('0');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '1':
                    Number('1');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '2':
                    Number('2');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '3':
                    Number('3');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '4':
                    Number('4');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '5':
                    Number('5');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '6':
                    Number('6');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '7':
                    Number('7');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '8':
                    Number('8');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '9':
                    Number('9');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '.':
                    AddPoint();
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '+':
                    Operator('+');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '-':
                    Operator('-');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '/':
                    Operator('/');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '*':
                    Operator('*');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
                case '=':
                    Operator('=');
                    CurrentlySelected = EqualsBtn;
                    SetSelected();
                    break;
            }
        }


        public override void Click(int x, int y, bool handled)
        {
            if (x == Width - 1 && y == 0) ExitWindow();
        }


        public override void Draw()
        {
            if (!Visible) return;
            var pl = (Width - 1 - Title.Length) / 2;
            WriteText((" ".PadLeft(pl) + Title).PadRight(Width - 1, ' '), 0, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.Close, Width - 1, 0, TitleColour, TitleBarColour);
            FillRect(0, +1, Width, Height - 1, BackColor, true); //Main Box        
            base.Draw();
        }

        private void Number(char number)
        {
            if (_displayingTotal)
                _display.SetText(number.ToString());
            else
                _display.SetText(_display.GetText() + number);

            _displayingTotal = false;
        }

        private void AddPoint()
        {
            if (_pointUsed) //Number already has a point
                return;

            if (_displayingTotal)
                _display.SetText("0.");
            else
                _display.SetText(_display.GetText() + '.');

            _displayingTotal = false;
            _pointUsed = true;
        }

        private void Clear()
        {
            _lastOp = '=';
            _total = 0;
            _display.SetText("0");
            _displayingTotal = true;
        }

        private void Operator(char op)
        {
            double number = 0;

            if (_display.GetText() != string.Empty)
                number = double.Parse(_display.GetText());

            if (_lastOp == '-')
                _total = _total - number;
            else if (_lastOp == '+')
                _total = _total + number;
            else if (_lastOp == '/')
                _total = _total / number;
            else if (_lastOp == '*')
                _total = _total * number;
            else if (_lastOp == '=')
                _total = number;

            _display.SetText(_total.ToString(CultureInfo.InvariantCulture));
            _displayingTotal = true;
            _pointUsed = false;

            _lastOp = op;
        }
    }
}