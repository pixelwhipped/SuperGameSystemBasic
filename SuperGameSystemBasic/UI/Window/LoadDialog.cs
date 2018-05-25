using System;
using Windows.Storage;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class LoadDialog : Window
    {
        private Button _closeButton;
        private readonly StorageFolder _storage;
        private readonly bool _traverseDirectories;

        public LoadDialog(string title, FullWindow parentWindow, bool traverseDirectories, StorageFolder storage = null)
            : base(parentWindow.Width / 2 - 15, 6, 30, 5, parentWindow)
        {
            _storage = storage ?? BasicOne.LocalStorageFolder;
            _traverseDirectories = traverseDirectories;
            Height = 10;
            Create(title);
        }

        public string Title { get; set; } = string.Empty;

        public Action<StorageFile> SelectFile
        {
            get => Files.SelectFile;
            set => Files.SelectFile = value;
        }

        public FileBrowser Files { get; set; }

        private void Create(string title)
        {
            IsDialog = true;
            BackColor = Terminal.CYAN;
            Title = title;
            PostionY = ParentWindow.Height / 2 - Height / 2;
            Files = new FileBrowser(1, 1, Width - 2, Height - 2, _storage, this, true, null, null,
                _traverseDirectories);
            Inputs.Add(Files);


            _closeButton = new Button(Width - 7, Height, "CLOSE", this, "OkBtn") {Action = ExitWindow};

            Inputs.Add(_closeButton);

            CurrentlySelected = Files;
        }

        public override void Click(int x, int y, bool handled)
        {
            if (x == Width - 1 && y == 0) ExitWindow();
        }

        public override void Draw()
        {
            var pl = (Width - Title.Length) / 2;
            WriteText((" ".PadLeft(pl) + Title).PadRight(Width, ' '), 0, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.Close, Width - 1, 0, TitleColour, TitleBarColour);
            FillRect(0, 1, Width, Height, BackColor, true); //Main Box        
            base.Draw();
        }
    }
}