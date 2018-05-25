using System;
using System.Threading.Tasks;
using Windows.Storage;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class SaveDialog : Window
    {
        private Button _cancelButton;
        private Button _saveButton;

        public SaveDialog(BasicOne basicOne, string title, string name, FullWindow parentWindow)
            : base(parentWindow.Width / 2 - 15, 6, 30, 5, parentWindow)
        {
            BasicOne = basicOne;
            CurrentName = name ?? "new.basic";
            Height = 10;
            Create(title);
        }

        public string Title { get; set; } = string.Empty;

        public Action<StorageFile> SelectFile
        {
            get => Files.SelectFile;
            set => Files.SelectFile = value;
        }

        public string CurrentName { get; set; }

        public BasicOne BasicOne { get; set; }

        public FileBrowser Files { get; set; }

        public TextBox FileName { get; set; }

        private void Create(string title)
        {
            IsDialog = true;
            BackColor = Terminal.CYAN;
            Title = title;
            PostionY = ParentWindow.Height / 2 - Height / 2;
            Files = new FileBrowser(1, 1, Width - 2, Height - 2, BasicOne.LocalStorageFolder, this, true);

            Inputs.Add(Files);
            FileName = new TextBox(1, Height, CurrentName, null, this);
            Inputs.Add(FileName);
            Files.ChangeItem = () =>
            {
                if (Files.CurrentlySelectedFile.EndsWith(".basic", StringComparison.CurrentCultureIgnoreCase))
                    FileName.SetText(Files.CurrentlySelectedFile);
            };

            _cancelButton = new Button(Width - 8, Height, "CANCEL", this) {Action = ExitWindow};

            _saveButton = new Button(Width - 14, Height, "SAVE", this) {Action = DoSave};

            Inputs.Add(_saveButton);
            Inputs.Add(_cancelButton);

            CurrentlySelected = Files;
        }

        private void DoSave()
        {
            var f = FileName.GetText();
            if (!f.EndsWith(".basic")) f = f + ".basic";
            f = f.ToLower();
            if (f == ".basic") return;
            if (AsyncIO.DoesFileExistAsync(Files.CurrentPath, f))
                BasicOne.IdeWindow.SelectWindow(new Confirm("File Exists", "Do you wish to overwrite this file",
                    () =>
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await
                                    FileIO.WriteTextAsync(AsyncIO.CreateFileAsync(Files.CurrentPath, f),
                                        BasicOne.Editor.Code);
                                ExitWindow();
                                var s = AsyncIO.GetFileAsync(Files.CurrentPath, f);
                                BasicOne.OpenEditor(f, s, null, false);
                            }
                            catch (Exception e)
                            {
                            }
                        }).Wait();
                    },
                    () => { BasicOne.IdeWindow.SelectWindow(this); },
                    BasicOne.IdeWindow));
            else
                Task.Run(async () =>
                {
                    try
                    {
                        await FileIO.WriteTextAsync(AsyncIO.CreateFileAsync(Files.CurrentPath, f),
                            BasicOne.Editor.Code);
                        ExitWindow();
                        var s = AsyncIO.GetFileAsync(Files.CurrentPath, f);
                        BasicOne.OpenEditor(f, s, null, false);
                        BasicOne.OpenEditor(f, BasicOne.Editor.CodeFile, null, false);
                    }
                    catch (Exception e)
                    {
                    }
                }).Wait();
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
            if (FileName.Selected)
            {
                SetCursor(FileName.CursorPostion + 2, FileName.PositionY);
                BasicOne.CursorVisible = true;
            }
        }
    }
}