using System;
using System.Linq;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class Confirm : Window
    {
        private Button _noBtn;
        private Button _yesBtn;

        public Confirm(string message, Action yes, Action no, FullWindow parentWindow)
            : base(parentWindow.Width / 2 - 14, 6, 28, 5, parentWindow)
        {
            OnYes = yes ?? (() => { });
            OnNo = no ?? (() => { });
            BackColor = Terminal.YELLOW;
            Create(message);
        }

        public Confirm(string title, string message, Action yes, Action no, FullWindow parentWindow)
            : base(parentWindow.Width / 2 - 14, 6, 28, 5, parentWindow)
        {
            OnYes = yes ?? (() => { });
            OnNo = no ?? (() => { });
            BackColor = Terminal.YELLOW;
            Title = title;
            Create(message);
        }

        public Confirm(string message, Action yes, Action no, int backColor, FullWindow parentWindow)
            : base(parentWindow.Width / 2 - 14, 6, 28, 5, parentWindow)
        {
            OnYes = yes ?? (() => { });
            OnNo = no ?? (() => { });
            BackColor = backColor;
            Create(message);
        }

        public Confirm(string title, string message, Action yes, Action no, int backColor, FullWindow parentWindow)
            : base(parentWindow.Width / 2 - 14, 6, 28, 5, parentWindow)
        {
            OnYes = yes ?? (() => { });
            OnNo = no ?? (() => { });
            Title = title;
            BackColor = backColor;
            Create(message);
        }

        public Action OnYes { get; set; }
        public Action OnNo { get; set; }

        public string Message { get; set; }
        public string Title { get; set; }

        private void Create(string message)
        {
            IsDialog = true;
            Message = message;
            var count = 0;
            var lines = BasicOne.SplitToLines(message, Width - 2);
            var enumerable = lines as string[] ?? lines.ToArray();
            Height = enumerable.Length + 3;
            PostionY = ParentWindow.Height / 2 - Height / 2;
            foreach (var l in enumerable)
            {
                var messageLabel = new Label(l, 1, 2 + count, this);
                Inputs.Add(messageLabel);
                count++;
            }


            /*
            var messageLabel = new Label(Message, PostionX + 2, PostionY + 2, "messageLabel", this);
            messageLabel.BackgroundColour = BackgroundColour;*/

            _yesBtn = new Button(Width - 9, Height, "YES", this)
            {
                Action = () =>
                {
                    OnYes?.Invoke();
                    ExitWindow();
                }
            };
            _noBtn = new Button(Width - 4, Height, "NO", this)
            {
                Action = () =>
                {
                    OnNo?.Invoke();
                    ExitWindow();
                }
            };

            Inputs.Add(_yesBtn);
            Inputs.Add(_noBtn);

            CurrentlySelected = _yesBtn;
        }

        public override void Draw()
        {
            var pl = (Width - Title.Length) / 2;
            WriteText((" ".PadLeft(pl) + Title).PadRight(Width, ' '), 0, 0, TitleColour, TitleBarColour);
            FillRect(0, 1, Width, Height, BackColor, true); //Main Box        
            base.Draw();
        }
    }
}