using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Search;
using Microsoft.Xna.Framework;

namespace SuperGameSystemBasic.UI.Input
{
    public class FileBrowser : Input
    {
        private readonly int _selectedBackColor = Terminal.CYAN;
        private readonly int _selectedTextColor = Terminal.BLACK;

        private readonly int _textColor = Terminal.BLACK;
        private readonly bool _traverseDirectories;
        private bool _atRoot;
        private int _cursorY;

        private int _offset;
        private bool _selected;
        private bool _showingDrive;

        public Action ChangeItem;
        public string FilterByExtension;

        public bool IncludeFiles;
        public Action<StorageFile> SelectFile;

        public FileBrowser(int x, int y, int width, int height, StorageFolder path, Window.Window parentWindow,
            bool includeFiles = false, string filterByExtension = null, string id = null,
            bool traverseDirectories = true)
            : base(x, y, width, height, parentWindow, id)
        {
            _traverseDirectories = traverseDirectories;
            CurrentPath = path;
            CurrentlySelectedFile = string.Empty;
            BackColor = Terminal.LIGHT_CYAN;
            IncludeFiles = includeFiles;
            FilterByExtension = filterByExtension ?? ".basic";
            Drives = BasicOne.KnownStorageFolders;

            GetFileNames();
            if (FileNames.Any()) CurrentlySelectedFile = FileNames[0];
            Selectable = true;
        }

        public StorageFolder CurrentPath { get; private set; }
        public string CurrentlySelectedFile { get; private set; }
        private List<string> FileNames { get; set; } = new List<string>();
        private List<string> Folders { get; set; }
        private List<StorageFolder> Drives { get; }

        private int CursorY
        {
            get => _cursorY;
            set
            {
                _cursorY = value;
                GetCurrentlySelectedFileName();
                SetOffset();
            }
        }

        public override void Click(int x, int y)
        {
            var c = FileNames.Count + Folders.Count;
            if (y - 1 == CursorY - _offset)
            {
                Enter();
            }
            else if (y - 1 >= 0 && y - 1 <= c)
            {
                CursorY = y + _offset - 1;
                SetOffset();
            }
            //
        }

        public override void Draw()
        {
            FillRect(PositionX, PositionY, Width, Height, BackColor, true);


            if (!_showingDrive)
            {
                var trimedPath = CurrentPath.DisplayName.PadLeft(Width - 2, ' ');
                trimedPath = trimedPath.Substring(trimedPath.Length - Width + 2, Width - 2);
                WriteText(trimedPath, PositionX, PositionY, Terminal.BLACK, BackColor);
            }
            else
            {
                var trimedPath = "Drives".PadLeft(Width - 2, ' ');
                trimedPath = trimedPath.Substring(trimedPath.Length - Width + 2, Width - 2);
                WriteText(trimedPath, PositionX, PositionY, Terminal.BLACK, BackColor);
            }

            if (!_showingDrive)
            {
                var i = _offset;
                while (i < Math.Min(Folders.Count, Height + _offset - 1))
                {
                    var folderName = Folders[i].PadRight(Width - 2, ' ').Substring(0, Width - 2);

                    if (i == CursorY)
                        WriteText("[" + folderName + "]", PositionX, PositionY + i - _offset + 1, _selectedTextColor,
                            _selected ? _selectedBackColor : BackColor);
                    else
                        WriteText("[" + folderName + "]", PositionX, PositionY + i - _offset + 1, _textColor,
                            BackColor);

                    i++;
                }

                while (i < Math.Min(Folders.Count + FileNames.Count, Height + _offset - 1))
                {
                    var fileName = FileNames[i - Folders.Count].PadRight(Width - 2, ' ').Substring(0, Width - 2);

                    if (i == CursorY)
                        WriteText(fileName, PositionX, PositionY + i - _offset + 1, _selectedTextColor,
                            _selected ? _selectedBackColor : BackColor);
                    else
                        WriteText(fileName, PositionX, PositionY + i - _offset + 1, _textColor, BackColor);
                    i++;
                }
            }
            else
            {
                for (var i = 0; i < Drives.Count; i++)
                {
                    var driveName = Drives[i].DisplayName.PadRight(Width - 2, ' ').Substring(0, Width - 2);
                    if (i == CursorY)
                        WriteText(driveName, PositionX, PositionY + i - _offset + 1, _selectedTextColor,
                            _selected ? _selectedBackColor : BackColor);
                    else
                        WriteText(driveName, PositionX, PositionY + i - _offset + 1, _textColor, BackColor);
                }
            }
        }

        public void GetFileNames()
        {
            if (_showingDrive) //Currently Showing drives. This function should not be called!
                return;

            if (IncludeFiles)
                FileNames = AsyncIO.GetFilesAsync(CurrentPath, new QueryOptions(CommonFileQuery.OrderByName,
                    FilterByExtension.Split(',')) {FolderDepth = FolderDepth.Shallow}).Select(p => p.Name).ToList();

            if (_traverseDirectories)
            {
                Folders = AsyncIO.GetFoldersAsync(CurrentPath).Select(p => p.Name).ToList();
                // Directory.GetDirectories(CurrentPath).Select(path => System.IO.Path.GetFileName(path)).ToList();
                Folders.Insert(0, "..");
                _atRoot = AsyncIO.GetParentAsync(CurrentPath) == null;
            }
            else
            {
                Folders = new List<string>();
                _atRoot = true;
            }
            if (CursorY > FileNames.Count + Folders.Count)
                CursorY = 0;
        }

        private void DisplayDrives()
        {
            _showingDrive = true;
            CursorY = 0;
        }

        public override void Select()
        {
            if (!_selected)
                _selected = true;
        }

        public override void Unselect()
        {
            if (_selected)
                _selected = false;
        }

        public override void PageDown()
        {
            if (CursorY != Folders.Count + FileNames.Count - 1 && !_showingDrive)
                CursorY = MathHelper.Clamp(CursorY + (Height-1), 0, Folders.Count + FileNames.Count -1);
            else if (CursorY != Drives.Count - 1 && _showingDrive)
                CursorY++;
        }

        public override void CursorMoveDown()
        {
            if (CursorY != Folders.Count + FileNames.Count - 1 && !_showingDrive)
                CursorY++;
            else if (CursorY != Drives.Count - 1 && _showingDrive)
                CursorY++;
            else
                ParentWindow.MovetoNextItemDown(PositionX, PositionY, Width);
        }

        public override void CursorMoveUp()
        {
            if (CursorY != 0)
                CursorY--;
            else
                ParentWindow.MovetoNextItemUp(PositionX, PositionY, Width);
        }

        public override void PageUp()
        {
            if (CursorY != 0)
                CursorY = MathHelper.Clamp(CursorY - (Height - 1), 0, Folders.Count + FileNames.Count - 1);
        }

        public override void CursorMoveRight()
        {
            if (CursorY >= 1 && CursorY < Folders.Count && !_showingDrive) //Folder is selected
                GoIntoFolder();
            else if (_showingDrive)
                GoIntoDrive();
        }

        public override void Enter()
        {
            try
            {
                if (CursorY >= 1 && CursorY < Folders.Count && !_showingDrive) //Folder is selected
                    GoIntoFolder();
                else if (_cursorY == 0 && !_atRoot) //Back is selected
                    GoToParentFolder();
                else if (SelectFile != null && !_showingDrive && !string.IsNullOrEmpty(CurrentlySelectedFile))
                    //File is selcted
                    SelectFile(AsyncIO.GetFileAsync(CurrentPath, CurrentlySelectedFile));
                else if (_cursorY == 0 && _atRoot && !_showingDrive && _traverseDirectories
                ) //Back is selected and at root, thus show drives
                    DisplayDrives();
                else if (_showingDrive)
                    GoIntoDrive();
            }
            catch (Exception e)
            {
            }
        }

        private void GoIntoDrive()
        {
            CurrentPath = Drives[_cursorY];

            try
            {
                _showingDrive = false;
                GetFileNames();
                CursorY = 0;
            }
            catch (IOException)
            {
                _showingDrive = true;
            }
        }

        private void GoIntoFolder()
        {
            CurrentPath = AsyncIO.GetFolderAsync(CurrentPath, Folders[_cursorY]);

            try
            {
                GetFileNames();
                CursorY = 0;
            }
            catch (UnauthorizedAccessException)
            {
                // CurrentPath = Directory.GetParent(CurrentPath).FullName; //Change Path back to parent
                // new Alert("Access Denied", ParentWindow, ConsoleColor.White, "Error");
            }
        }

        public override void CursorMoveLeft()
        {
            if (!_atRoot)
                GoToParentFolder();
            else
                DisplayDrives();
        }

        public override void BackSpace()
        {
            if (!_atRoot)
                GoToParentFolder();
        }

        private void GoToParentFolder()
        {
            CurrentPath = AsyncIO.GetParentAsync(CurrentPath) ?? CurrentPath;
            //.GetParentAsync().GetResults();// Directory.GetParent(CurrentPath).FullName;
            GetFileNames();
            CursorY = 0;
        }

        private void SetOffset()
        {
            while (CursorY - _offset > Height - 2)
                _offset++;

            while (CursorY - _offset < 0)
                _offset--;
        }

        private void GetCurrentlySelectedFileName()
        {
            if (_cursorY >= Folders.Count && FileNames.Any()) //File is selected
            {
                try
                {
                    CurrentlySelectedFile = FileNames[_cursorY - Folders.Count];
                    ChangeItem?.Invoke();
                }
                catch { }
            }
            else
            {
                if (CurrentlySelectedFile != string.Empty)
                {
                    CurrentlySelectedFile = string.Empty;
                    ChangeItem?.Invoke();
                }
            }
        }
    }
}