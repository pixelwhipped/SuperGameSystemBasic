using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.ApplicationModel;
using Windows.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class LanguageWindow : Window
    {
        private bool _isTextselected;

        private List<string> _lines = new List<string>();
        private int _lscrollOffset;
        private int _scrollOffset;
        private int _selected;
        private string _text;

        public LanguageWindow(BasicOne basicOne, int postionX, int postionY, int width, int height,
            IWindow parentWindow) : base(postionX, postionY, width, height, parentWindow)
        {
            BackColor = Terminal.LIGHT_GREEN;
            BasicOne = basicOne;

            Inputs.Add(new Button(1, 1, "Home", this) {Action = ShowHome, BackColor = Terminal.GREEN});
            Inputs.Add(new Button(7, 1, "Syntax", this) {Action = ShowSyntax, BackColor = Terminal.GREEN});
            Inputs.Add(new Button(15, 1, "Types", this) {Action = ShowTypes, BackColor = Terminal.GREEN});
            Inputs.Add(new Button(22, 1, "Constants", this) {Action = ShowConstants, BackColor = Terminal.GREEN});
            Inputs.Add(new Button(33, 1, "Functions", this) {Action = ShowFunctions, BackColor = Terminal.GREEN});

            CurrentlySelected = Inputs[0];
            ShowHome();
        }

        public BasicOne BasicOne { get; set; }
        public StorageFolder LanguageFolder { get; set; }

        public List<StorageFile> List { get; set; } = new List<StorageFile>();
        public string Title => $"SGS Basic Language {BasicOne.LanguageVersion}";
        public int VirtualHeight => _lines.Count;

        public string Text
        {
            get => _text ?? string.Empty;
            set
            {
                var t = value ?? string.Empty;
                t = t.Replace('\r', '\n');
                var sb = new StringBuilder();
                for (var index = 0; index < t.Length; index++)
                {
                    var c = t[index];
                    if (c == '~')
                    {
                        index++;
                        var n = string.Empty;
                        while ((c = t[index]) != '~')
                        {
                            n = n + c;
                            index++;
                        }
                        if (int.TryParse(n, out var r)) sb.Append((char) (r + 32));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                _text = sb.ToString();
                _lines = BasicOne.SplitToLines(_text ?? string.Empty, Width - 22).ToList();
            }
        }

        public override bool Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
        {
            if (key == Keys.Down)
                if (CurrentlySelected != null && Inputs.Any())
                {
                    CurrentlySelected.Unselect();
                    CurrentlySelected = null;
                }
                else if (!_isTextselected)
                {
                    _selected = Math.Max(MathHelper.Clamp(_selected + 1, 0, List.Count - 1), 0);
                    if (_selected + _lscrollOffset == List.Count)
                    {
                        _selected--;
                        return true;
                    }
                    if (_selected - _lscrollOffset >= Height - 4) _lscrollOffset++;
                    ShowSelected();
                }
                else
                {
                    _scrollOffset = MathHelper.Clamp(_scrollOffset + 1, 0, VirtualHeight);
                }
            if (key == Keys.Up)
            {
                if (!_isTextselected)
                {
                    if (_selected - _lscrollOffset == 0)
                        if (CurrentlySelected == null && Inputs.Any())
                            SelectFirstItem();
                    {
                        _selected = Math.Max(MathHelper.Clamp(_selected - 1, 0, List.Count - 1), 0);
                        if (_selected - _lscrollOffset == 0) _lscrollOffset = Math.Max(_lscrollOffset - 1, 0);
                        ShowSelected();
                    }
                }
                else
                {
                    if (_scrollOffset == 0)
                    {
                        if (CurrentlySelected == null && Inputs.Any())
                            SelectFirstItem();
                    }
                    else
                    {
                        _scrollOffset = MathHelper.Clamp(_scrollOffset - 1, 0, VirtualHeight);
                    }
                }
            }
            else if (key == Keys.PageUp)
            {
                if (!_isTextselected)
                {
                    var diff = Math.Max(MathHelper.Clamp(_selected - (Height - 3), 0, List.Count - 1), 0);
                    _lscrollOffset = MathHelper.Clamp(_lscrollOffset - Math.Abs(diff - _selected),0, List.Count - 1);
                    _selected = MathHelper.Clamp(diff, 0, List.Count - 1);
                    ShowSelected();
                }
                else
                {
                    _scrollOffset = MathHelper.Clamp(_scrollOffset - (Height - 3), 0, VirtualHeight);
                }
            }
            else if (key == Keys.PageDown)
            {
                if (!_isTextselected)
                {
                    for (var i = 0; i < 9; i++)
                    {
                        _selected = Math.Max(MathHelper.Clamp(_selected + 1, 0, List.Count - 1), 0);
                        if (_selected + _lscrollOffset == List.Count)
                        {
                            _selected--;
                            return true;
                        }
                        if (_selected - _lscrollOffset >= Height - 4) _lscrollOffset++;
                        if (_selected + _lscrollOffset == List.Count)
                        {
                            _selected--;
                            return true;
                        }
                        ShowSelected();
                    }
                }
                else
                {
                    _scrollOffset = MathHelper.Clamp(_scrollOffset + (Height-3), 0, VirtualHeight);
                }
            }
            else if (key == Keys.Tab)
            {
                if (CurrentlySelected == null && Inputs.Any())
                    SelectFirstItem();
                else
                    CurrentlySelected?.Tab();
            }
            else if (key == Keys.Escape)
            {
                CurrentlySelected = null;
            }
            else if (key == Keys.Enter)
            {
                if (CurrentlySelected != null)
                {
                    CurrentlySelected.Enter();
                    CurrentlySelected.Unselect();
                    CurrentlySelected = null;
                }
            }
            else if (key == Keys.Left)
            {
                if (CurrentlySelected != null)
                    CurrentlySelected?.CursorMoveLeft();
                else
                    _isTextselected = false;
            }
            else if (key == Keys.Right)
            {
                if (CurrentlySelected != null)
                    CurrentlySelected?.CursorMoveRight();
                else
                    _isTextselected = true;
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

        private void ShowSelected()
        {
            _selected = Math.Max(_selected, 0);
            if (_selected + _lscrollOffset < List.Count)
            {
                var f = List[_selected + _lscrollOffset];
                if (f != null)
                {
                    Text = AsyncIO.ReadTextFileAsync(f);
                    _scrollOffset = 0;
                }
            }
        }


        private void ShowFunctions()
        {
            Text = Read("Language\\Functions\\Functions.txt");
            LanguageFolder = AsyncIO.GetFolderAsync(Package.Current.InstalledLocation, "Language\\Functions");
            ReadLanguageList("Functions");
        }

        private void ShowConstants()
        {
            Text = Read("Language\\Constants\\Constants.txt");
            LanguageFolder = AsyncIO.GetFolderAsync(Package.Current.InstalledLocation, "Language\\Constants");
            ReadLanguageList("Constants");
        }

        private void ShowTypes()
        {
            Text = Read("Language\\Types\\Types.txt");
            LanguageFolder = AsyncIO.GetFolderAsync(Package.Current.InstalledLocation, "Language\\Types");
            ReadLanguageList("Types");
        }

        private void ShowSyntax()
        {
            Text = Read("Language\\Syntax\\Syntax.txt");
            LanguageFolder = AsyncIO.GetFolderAsync(Package.Current.InstalledLocation, "Language\\Syntax");
            ReadLanguageList("Syntax");
        }

        private void ShowHome()
        {
            Text = Read("Language\\Home\\Home.txt");
            LanguageFolder = AsyncIO.GetFolderAsync(Package.Current.InstalledLocation, "Language\\Home");
            ReadLanguageList("Home");
        }

        private void ReadLanguageList(string title)
        {
            var list = new List<StorageFile>();
            var files = AsyncIO.GetFilesAsync(LanguageFolder);
            StorageFile heading = null;
            foreach (var f in files)
                if (title.Equals(f.DisplayName, StringComparison.CurrentCultureIgnoreCase))
                    heading = f;
                else
                    list.Add(f);
            List.Clear();
            if (heading != null) List.Add(heading);
            List.AddRange(list.OrderBy(p => p.Name));
            _selected = 0;
        }

        public string Read(string file)
        {
            return new StreamReader(AsyncIO.GetContentStream(file)).ReadToEnd();
        }

        public override void Click(int x, int y, bool handled)
        {
            if (x == Width - 1 && y == 0) ExitWindow();
            if (x >= 23 && y >= 3 && y >= 3 && x <= 58 && y <= 22)
                _isTextselected = true;
            if (x >= 0 && y >= 3 && x <= 19 && y <= 22)
                _isTextselected = false;
            if (x >= 1 && x <= 19)
            {
                _selected = _lscrollOffset + y - 3;
                ShowSelected();
                if (y == Height - 1)
                {
                    _selected--;
                    _lscrollOffset++;
                }
                //   var p = 0;
            }
            if (x == Width - 1 && y == 3)
            {
                _scrollOffset = MathHelper.Clamp(_scrollOffset - 1, 0, VirtualHeight);
            }
            else if (x == Width - 1 && y == Height - 1)
            {
                _scrollOffset = MathHelper.Clamp(_scrollOffset + 1, 0, VirtualHeight);
                _isTextselected = true;
            }
            else if (x == Width - 1 && y < Height - 1 && y > 3)
            {
                _scrollOffset = MathHelper.Clamp((int) ((float) Math.Max(y - 4, 0) / (Height - 6) * VirtualHeight), 0,
                    VirtualHeight);
                _isTextselected = true;
            }
            base.Click(x, y, handled);
        }

        public override void Draw()
        {
            var pl = (Width - 1 - Title.Length) / 2;
            WriteText((" ".PadLeft(pl) + Title).PadRight(Width - 1, ' '), 0, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.Close, Width - 1, 0, TitleColour, TitleBarColour);
            FillRect(0, +1, Width, Height - 1, BackColor, true); //Main Box        
            for (var i = 0; i < Width; i++)
                WriteGlyph(i == 20 ? Glyph.BarDoubleDownLeftRight : Glyph.BarDoubleLeftRight, i, 2, Terminal.WHITE,
                    BackColor);
            for (var i = 3; i < Height; i++)
                WriteGlyph(Glyph.BarDoubleUpDown, 20, i, Terminal.WHITE, BackColor);
            if (_isTextselected) FillRect(21, 3, Width, Height - 1, Terminal.GREEN, true); //Main Box  
            var y = 0;
            for (var i = _scrollOffset; i < VirtualHeight; i++)
            {
                WriteText(_lines[i], 21, 3 + y, Terminal.WHITE, _isTextselected ? Terminal.GREEN : BackColor);
                y++;
                if (y >= Height - 3) break;
            }
            DrawVLine(Width - 1, 3, Height - 3, Terminal.GRAY, true);
            WriteGlyph(Glyph.TriangleUp, Width - 1, 3, Terminal.WHITE, Terminal.DARK_GRAY);
            WriteGlyph(Glyph.TriangleDown, Width - 1, Height, Terminal.WHITE, Terminal.DARK_GRAY);
            WriteText("-", Width - 1,
                (int) ((float) _scrollOffset / Math.Max(VirtualHeight, 1) * Math.Max(Height - 6, 0)) + 4,
                Terminal.DARK_GRAY, Terminal.GRAY);
            y = 3;
            for (var index = _lscrollOffset; index < List.Count; index++)
            {
                if (index + _lscrollOffset >= List.Count) break;
                var i = List[index + _lscrollOffset];
                if (y >= Height) break;
                if (index == _selected && !_isTextselected)
                    WriteText($"[{i.DisplayName.PadRight(17, ' ')}]", 1, y, Terminal.WHITE, Terminal.GREEN);
                else
                    WriteText(i.DisplayName.PadRight(18, ' '), 1, y, Terminal.WHITE, BackColor);
                y++;
            }

            base.Draw();
        }
    }
}