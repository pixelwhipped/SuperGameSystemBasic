using System.Linq;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class Alert : Window
    {
        private Button _okBtn;

        public Alert(string message, FullWindow parentWindow)
            : base(parentWindow.Width / 2 - 14, 6, 28, 5, parentWindow)
        {
            Create(message);
        }

        public Alert(string title, string message, FullWindow parentWindow)
            : base(parentWindow.Width / 2 - 14, 6, 28, 5, parentWindow)
        {
            Title = title;
            Create(message);
        }

        public Alert(string message, FullWindow parentWindow, int backgroundColour)
            : base(parentWindow.Width / 2 - 14, 6, 28, 5, parentWindow)
        {
            BackColor = backgroundColour;
            Create(message);
        }

        public Alert(string title, string message, FullWindow parentWindow, int backgroundColour)
            : base(parentWindow.Width / 2 - 14, 6, 28, 5, parentWindow)
        {
            Title = title;
            BackColor = backgroundColour;
            Create(message);
        }

        public string Message { get; set; }
        public string Title { get; set; } = string.Empty;

        private void Create(string message)
        {
            IsDialog = true;
            BackColor = Terminal.YELLOW;
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

            _okBtn = new Button(Width - 4, Height, "OK", this, "OkBtn") {Action = ExitWindow};

            Inputs.Add(_okBtn);

            CurrentlySelected = _okBtn;
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