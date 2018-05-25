using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.Basic;
using SuperGameSystemBasic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace SuperGameSystemBasic.UI.Window
{
    public class EditorWindow : Window
    {
        private IStorageFile _codeFile;
        private int _scrollOffsetX;
        private int _scrollOffsetY;

        public List<CodeLine> Lines = new List<CodeLine>();

        public EditorWindow(BasicOne basicOne, string name, IStorageFile code, int postionX, int postionY, int width,
            int height, FullWindow parentWindow)
            : base(postionX, postionY, width, height, parentWindow)
        {
            Saved = true;
            CodeFile = code;
            BasicOne = basicOne;
            Name = string.IsNullOrEmpty(name) ? "new.basic" : name;
            BackColor = Terminal.BLUE;

            CursorX = Indent;
            CursorY = 1;
        }

        public string Name { get; set; }
        public int VirtualWidth => Lines.Max(p => p.Line.Length); //.Count;
        public int VirtualHeight => Lines.Count;
        public int Indent => VirtualHeight.ToString().Length + 3;
        public int CursorX { get; set; }
        public int CursorY { get; set; }

        public BasicOne BasicOne { get; set; }

        public IStorageFile CodeFile
        {
            get => _codeFile;
            set
            {
                _codeFile = value;
                Lines.Clear();
                if (_codeFile != null)
                {
                    var c = AsyncIO.ReadTextFileAsync(_codeFile).TrimEnd();
                    var s = c.Replace("\r", "").Split('\n');
                    for (var index = 0; index < s.Length; index++)
                        Lines.Add(new CodeLine(s[index]));

                    if (Lines.Count == 0) Lines.Add(new CodeLine(string.Empty));
                }
                else
                {
                    Lines.Add(new CodeLine($"REM {Name} Game Logic......."));
                    Lines.Add(new CodeLine("REM Press F5 to start"));
                    Lines.Add(new CodeLine("REM Press Break or Pause to return to the DevKit"));

                    Lines.Add(new CodeLine(""));
                    Lines.Add(new CodeLine("WHILE NOT KEYPRESSED(@SPACE)"));
                    Lines.Add(new CodeLine("  YEILD(100)"));
                    Lines.Add(new CodeLine("  BLIT"));
                    Lines.Add(new CodeLine("WEND"));
                    Lines.Add(new CodeLine("END"));
                }
                //  FixLines();
            }
        }

        public bool Saved { get; set; }

        public string Code
        {
            get
            {
                var str = new StringBuilder();
                for (var index = 0; index < Lines.Count; index++)
                    str.AppendLine(Lines[index].Line.Replace("\t", "     ").Replace("\r",""));
                return str.ToString();
            }
            set
            {
                Lines.Clear();
                var l = value.Replace("\r","").Split('\n');
                for (var index = 0; index < l.Length; index++)
                    Lines.Add(new CodeLine(l[index]));
                if (Lines.Count == 0) Lines.Add(new CodeLine(string.Empty));
            }
        }

        public string RunCode
        {
            get
            {
                var str = new StringBuilder();
                for (var index = 0; index < Lines.Count; index++)
                {
                    var line = Lines[index].Line.Replace("\t", "     ").Trim();
                    if (line.StartsWith("REM")) line = "REM";//e can get rid of the extra text
                    var f = line.ToUpper().IndexOf("REM", 0);
                    if (f > 0) line = line.Substring(0, f).Trim();
                    str.AppendLine(line);

                }
                return str.ToString();
            }
        }

        public bool Selected { get; set; }

        public int ScrollOffsetY
        {
            get => _scrollOffsetY;
            set => _scrollOffsetY = Math.Max(value, 0);
        }

        internal void ScrollTo(int errorLine)
        {
            ScrollToTop();
            ScrollOffsetY = errorLine - 1;
        }

        public override bool Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
        {
            if (!Visible) return false;
            char? c;
            if (key == Keys.Tab)
            {
                AddLetter('\t');
            }
            else if (key == Keys.PageDown)
            {
                for (var i = 0; i < 20; i++)
                {
                    if (ScrollOffsetY + CursorY >= VirtualHeight) break;
                    CursorMoveDown();
                }
            }
            else if (key == Keys.PageUp)
            {
                for (var i = 0; i < 20; i++)
                {
                    if (ScrollOffsetY + CursorY <= 1) break;
                    CursorMoveUp();
                }
            }
            else if (key == Keys.Escape)
            {
            }
            else if (key == Keys.Enter)
            {
                AddLine();
            }
            else if (key == Keys.F6)
            {
                var data = new DataPackage();
                data.SetText(GetLine(ScrollOffsetY + (CursorY - 1)).Line);
                Clipboard.SetContent(data);
            }
            else if (key == Keys.F7)
            {
                var data = new DataPackage();
                data.SetText(Code);
                Clipboard.SetContent(data);
            }
            else if (key == Keys.Left)
            {
                CursorMoveLeft();
            }
            else if (key == Keys.Right)
            {
                CursorMoveRight();
            }
            else if (key == Keys.Up)
            {
                CursorMoveUp();
            }
            else if (key == Keys.Down)
            {
                CursorMoveDown();
            }
            else if (key == Keys.Back)
            {
                BackSpace();
            }
            else if (key == Keys.Home)
            {
                CursorToStart();
            }
            else if (key == Keys.End)
            {
                CursorToEnd();
            }
            else if (key == Keys.Delete)
            {
                CursorDelete();
            }
            else if (key == Keys.F5)
            {
                CursorToEnd();
                BasicOne.SetRunningState(RunCode);
            }
            else if (key == Keys.F8)
            {
                Lines.FixIndents();
            }
            else if (key == Keys.F4)
            {
                Paste();
            }
            else if ((c = getKey?.Invoke()) != null)
            {
                AddLetter((char) c);
            }
            else if (click)
            {
                return Click(mouseX, mouseY);
            }
            return true;
        }


        private void CursorMoveRight()
        {
            CursorX++;
            if (CursorX >= Width - 1)
            {
                CursorX--;
                _scrollOffsetX++;
            }
        }

        private void CursorMoveLeft()
        {
            if (_scrollOffsetX > 0 && CursorX == Indent)
                ScrollLeft();
            else
                CursorX = Math.Max(CursorX - 1, 1);
        }

        private void AddLine(string str = null)
        {
            Saved = false;
            str = str ?? string.Empty;
            var currentLine = ScrollOffsetY + (CursorY - 1);
            var line = GetLine(currentLine);
            if (CursorX - Indent >= line.Line.Length)
            {
                if (Lines.Count == currentLine)
                    Lines.Add(new CodeLine(str));
                else
                    Lines.Insert(currentLine + 1, new CodeLine(str));
            }
            else
            {
                if (CursorX - Indent == 0 || Lines[currentLine].Line.Length == 0)
                {
                    Lines.Insert(currentLine, new CodeLine(str));
                }
                else
                {
                    var current = Lines[currentLine].Line;
                    Lines[currentLine].Line = current.Substring(0, Math.Min(_scrollOffsetX + CursorX - Indent, current.Length));
                    if (Lines.Count == currentLine)
                        Lines.Add(new CodeLine(current.Substring(_scrollOffsetX + CursorX - Indent)));
                    else
                        Lines.Insert(currentLine + 1,
                            new CodeLine(current.Substring(Math.Min(_scrollOffsetX + CursorX - Indent, current.Length))));
                }
            }
            CursorY++;
            if (CursorY >= 21)
            {
                CursorY--;
                ScrollOffsetY++;
            }
            CursorToStart();
        }

        private void CursorMoveUp()
        {
            if (ScrollOffsetY > 0 && CursorY == 1)
                ScrollUp();
            else
                CursorY = Math.Max(CursorY - 1, 1);
        }

        private void CursorMoveDown()
        {
            //  if (fixLines) FixLines();
            CursorY++;
            if (CursorY >= Height - 1)
            {
                CursorY--;
                ScrollOffsetY++;
            }
        }

        private void CursorDelete()
        {
            Saved = false;
            var currentLine = Math.Max(ScrollOffsetY + (CursorY - 1), 0);
            if (currentLine > Lines.Count - 1) return;
            if (CursorX < Indent) return;
            var line = GetLine(currentLine);
            if (CursorX == Indent && line.Line.Length == 0)
            {
                Lines.RemoveAt(currentLine);
            }
            else
            {
                if (CursorX - Indent >= line.Line.Length)
                {
                    if (currentLine < VirtualHeight - 1)
                    {
                        var l = line.Line.PadRight(CursorX - Indent, ' ');
                        Lines[currentLine].Line = l + Lines[currentLine + 1].Line;
                        Lines.RemoveAt(currentLine + 1);
                    }
                }
                else
                {
                    var strleft = line.Line.Substring(0, CursorX - Indent);
                    var ctrl = Keyboard.GetState().IsKeyDown(Keys.LeftControl);
                    var strRight = ctrl ? string.Empty : line.Line.Substring(CursorX - Indent + 1);
                    if (ctrl)
                    {
                        var data = new DataPackage();
                        data.SetText(line.Line);
                        Clipboard.SetContent(data);
                    }
                    line.Line = strleft + strRight;
                }
            }
            if(!Lines.Any())Lines.Add(new CodeLine(string.Empty));
            //FixLines();
        }

        public CodeLine GetLine(int currentLine)
        {
            if (currentLine < 0) return new CodeLine(string.Empty);
            if (currentLine + 1 > Lines.Count)
                while (currentLine + 1 > Lines.Count)
                    Lines.Add(new CodeLine(string.Empty));
            var line = Lines[currentLine];
            if (CursorX - Indent >= line.Line.Length)
                Lines[currentLine].Line = line.Line.PadRight(CursorX - Indent, ' ');
            return Lines[currentLine];
        }

        private void BackSpace()
        {
            Saved = false;
            var currentLine = ScrollOffsetY + (CursorY - 1);
            if (currentLine > Lines.Count - 1)
            {
                if (CursorX > Indent)
                {
                    CursorMoveLeft();
                }
                else
                {
                    CursorMoveUp();
                    CursorToEnd();
                }
            }
            else
            {
                var line = GetLine(currentLine);
                if (!line.Line.Any())
                {
                    Lines.RemoveAt(currentLine);
                    CursorMoveUp();
                    CursorToEnd();
                }
                else
                {
                    if (CursorX == Indent && currentLine > 0)
                    {
                        CursorMoveUp();
                        CursorToEnd();
                        Lines[currentLine - 1].Line = Lines[currentLine - 1].Line + Lines[currentLine].Line;
                        Lines.RemoveAt(currentLine);
                    }
                    else if (CursorX != Indent)
                    {
                        line = GetLine(currentLine);
                        if (_scrollOffsetX + CursorX - Indent - 1 < line.Line.Length)
                        {
                            var strleft = line.Line.Substring(0, _scrollOffsetX + CursorX - Indent - 1);
                            var strRight = line.Line.Substring(_scrollOffsetX + CursorX - Indent);
                            line.Line = strleft + strRight;
                        }
                        CursorMoveLeft();
                    }
                }
            }
            if (!Lines.Any()) Lines.Add(new CodeLine(string.Empty));
            //FixLines();
        }

        public void CursorToStart()
        {
            var currentLine = ScrollOffsetY + (CursorY - 1);
            if (currentLine > Lines.Count - 1) return;
            CursorX = Indent;
            _scrollOffsetX = 0;
        }

        private void CursorToEnd()
        {
            var currentLine = ScrollOffsetY + (CursorY - 1);
            if (currentLine >= Lines.Count) return;
            _scrollOffsetX = MathHelper.Clamp(Lines[ScrollOffsetY + CursorY - 1].Line.Length - (Width - (2 + Indent)),
                0, Lines[ScrollOffsetY + CursorY - 1].Line.Length);
            CursorX = MathHelper.Clamp(Indent + Lines[ScrollOffsetY + CursorY - 1].Line.Length, Indent, Width - 2);
        }

        private void AddString(string c)
        {
            c = c.Replace("\t", "     ").Replace("\r", "").Replace("\n","");
            var lines = 0;
            Saved = false;
            var currentLine = ScrollOffsetY + (CursorY - 1);
            var line = GetLine(currentLine);
            if (_scrollOffsetX + CursorX - Indent >= line.Line.Length)
                line.Line = line.Line.PadRight(_scrollOffsetX + CursorX - Indent, ' ');
            var strleft = _scrollOffsetX + CursorX - Indent == 0
                ? string.Empty
                : line.Line.Substring(0, _scrollOffsetX + CursorX - Indent);
            var strRight = line.Line.Substring(_scrollOffsetX + CursorX - Indent);
            line.Line = strleft + c + strRight;
            for(int i = 0 ; i < c.Length;i++)
                CursorMoveRight();
           // CursorMoveRight();
        }

        private int AddLetter(char c)
        {
            var lines = 0;

            Saved = false;
            var currentLine = ScrollOffsetY + (CursorY - 1);
            var line = GetLine(currentLine);
            if (_scrollOffsetX + CursorX - Indent >= line.Line.Length)
                line.Line = line.Line.PadRight(_scrollOffsetX + CursorX - Indent, ' ');
            var strleft = _scrollOffsetX + CursorX - Indent == 0
                ? string.Empty
                : line.Line.Substring(0, _scrollOffsetX + CursorX - Indent);
            var strRight = line.Line.Substring(_scrollOffsetX + CursorX - Indent);
            line.Line = strleft + (c == '\t' ? "     " : c.ToString()) + strRight;
            if (c == '\t')
            {
                CursorMoveRight();
                CursorMoveRight();
                CursorMoveRight();
                CursorMoveRight();
            }
            CursorMoveRight();
            if (c != '\n') return lines;
            lines++;
            AddLine();
            return lines;
        }

        private bool Click(int x, int y)
        {
            if (x == Width - 1 && y == 0)
            {
                if (!Saved)
                    BasicOne.IdeWindow.SelectWindow(new Confirm("Save Current Document",
                        $"Do you wish to save the current Document {Name}",
                        () =>
                        {
                            BasicOne.SaveFile();
                            ExitWindow();
                        },
                        ExitWindow, BasicOne.IdeWindow));
                else
                    ExitWindow();
                return false;
            }
            if (x == 53 && y == 0)
            {
                Paste();
                return false;
            }
            if (x == Width - 1 && y == 1)
            {
                ScrollUp();
            }
            else if (x == Width - 1 && y == Height - 1)
            {
                ScrollDown();
            }
            else if (x == Width - 1 && y < Height - 1 && y > 1)
            {
                ScrollOffsetY = MathHelper.Clamp((int) ((float) Math.Max(y - 2, 0) / (Height - 2) * VirtualHeight), 0,
                    VirtualHeight);
            }
            else if (y == Height - 1 && x < Width - 2 && x > 0)
            {
                _scrollOffsetX = MathHelper.Clamp((int) ((float) Math.Max(x - 1, 0) / (Width - 3) * VirtualWidth), 0,
                    VirtualWidth);
            }
            else if (x == 0 && y == Height - 1)
            {
                ScrollLeft();
            }
            else if (x == Width - 2 && y == Height - 1)
            {
                _scrollOffsetX++;
            }
            else if (x <= Width - 2 && x >= Width - 6 && y == 0)
            {
                BasicOne.SetRunningState(RunCode);
            }
            else if (x >= Indent && y >= 1 && x < Width - 1 && y <= Height - 2)
            {
                CursorX = x;
                CursorY = y;
            }
            else if (x == 0 && BasicOne.ErrorLine == y + ScrollOffsetY)
            {
                BasicOne.ShowLastError();
            }
            return true;
        }

        private void Paste()
        {
            try
            {
                var pasteData = Clipboard.GetContent();

                if (!pasteData.Contains(StandardDataFormats.Text)) return;
                var stringData = Task.Run(async () =>
                {
                    try
                    {
                        var clipboardAsync = pasteData.GetTextAsync();
                        return await clipboardAsync;
                    }
                    catch
                    {
                        return null;
                    }
                }).Result;
                if (string.IsNullOrEmpty(stringData)) return;
                var lines = stringData.Split('\n');
                if (lines.Length == 1)
                {
                    AddString(lines[0]);
                    if(stringData.EndsWith("\n"))AddLine();
                    return;
                }
                foreach (var s in lines)
                {
                    AddString(s);
                    AddLine();
                }
                if (stringData.EndsWith("\n")) AddLine();            
            }
            catch (Exception e)
            {
            }
        }

        public void ScrollLeft()
        {
            _scrollOffsetX = MathHelper.Clamp(_scrollOffsetX - 1, 0, VirtualWidth);
        }

        public void ScrollUp()
        {
            ScrollOffsetY = MathHelper.Clamp(ScrollOffsetY - 1, 0, VirtualHeight);
        }

        public void ScrollDown()
        {
            ScrollOffsetY = MathHelper.Clamp(ScrollOffsetY + 1, 0, VirtualHeight);
        }

        public override void Draw()
        {
            if (CursorX < Indent) CursorX = Indent;
            var pl = (Width - 1 - Name.Length) / 2;
            bool lineRem = false;
            WriteText((" ".PadLeft(pl) + Name).PadRight(Width - 1, ' '), 0, 0, TitleColour, TitleBarColour);
            WriteText("[F5]", Width - 6, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.TriangleRight, Width - 2, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.Close, Width - 1, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.HorizontalBars, Width - 7, 0, TitleColour, TitleBarColour);
            WriteText("[F4]", Width - 11, 0, TitleColour, TitleBarColour);
            FillRect(0, +1, Width, Height - 1, BackColor, true); //Main Box
            DrawVLine(Width - 1, 1, Height, Terminal.GRAY, true);
            DrawHLine(0, CursorY, Width - 1, Terminal.LIGHT_BLUE, true);
            int bc;
            WriteGlyph(Glyph.TriangleUp, Width - 1, 1, Terminal.GRAY, Terminal.DARK_GRAY);
            WriteGlyph(Glyph.TriangleDown, Width - 1, Height, Terminal.GRAY, Terminal.DARK_GRAY);
            WriteText("-", Width - 1,
                (int) ((float) ScrollOffsetY / Math.Max(VirtualHeight, 1) * Math.Max(Height - 3, 0)) +
                PositionYOnScreen +
                1, Terminal.DARK_GRAY, Terminal.GRAY);

            DrawHLine(0, Height - 1, Width - 2, Terminal.GRAY, true);
            WriteGlyph(Glyph.TriangleLeft, 0, Height, Terminal.GRAY, Terminal.DARK_GRAY);
            WriteGlyph(Glyph.TriangleRight, Width - 2, Height, Terminal.GRAY, Terminal.DARK_GRAY);
            //TODO THIS NEEDS FIXING
            WriteText("-", MathHelper.Clamp(
                Math.Min(
                    (int) ((float) _scrollOffsetX / Math.Max(VirtualWidth, Width) * Math.Max(Width - 2, 0)) +
                    PositionXOnScreen, Width - 1), 1, Width - 3), Height, Terminal.DARK_GRAY, Terminal.GRAY);

            var y = 1;
            for (var i = ScrollOffsetY; i < Lines.Count; i++)
            {
                lineRem = false;
                bc = CursorY == y ? Terminal.LIGHT_BLUE : BackColor;
                if (BasicOne.ErrorLine == y + ScrollOffsetY)
                    WriteGlyph(BasicOne.CursorBlink ? Glyph.Alien1 : Glyph.Alien2, 0, y, Terminal.WHITE, bc);
                WriteText($"{i + 1}", 1, y, Terminal.WHITE, bc);
                WriteGlyph(Glyph.BarUpDown, Indent - 1, y, Terminal.WHITE, bc);
                if (i >= 0 && i < Lines.Count && Lines[i].Line.Length - 1 >= _scrollOffsetX)
                {
                    var l = Lines[i].Line.Substring(_scrollOffsetX, Lines[i].Line.Length - _scrollOffsetX);
                    if (Lines[i].Line.TrimStart().StartsWith("REM", StringComparison.OrdinalIgnoreCase))
                    {
                        WriteText(
                            l.Substring(0, Math.Min(l.Length, Width - (Indent + 1))),
                            Indent, y, Terminal.DARK_GRAY, bc);
                    }
                    else
                    {
                        var tokens = Interpreter.SplitAndKeep(l, Lexer.Tokens);
                        var xo = 0;
                        foreach (var t in tokens)
                        {                                   
                            var tu = t.ToUpper();
                            if (tu == "REM") lineRem = true;
                             var tx = t.Substring(0, Math.Min(t.Length, Width - (Indent + 1)));
                            if (xo + Indent > Width - 1) break;
                            if (xo + tx.Length + Indent > Width - 1)
                                tx = tx.Substring(0, Math.Max(Width - 1 - (xo + Indent), 0));
                            if (lineRem)
                            {
                                WriteText(tx, xo + Indent,
                                    y, Terminal.DARK_GRAY, bc);
                                xo += t.Length;
                            }
                            else if (Interpreter.IsFunction(tu))
                            {
                                WriteText(tx, xo + Indent,
                                    y, Terminal.GREEN, bc);
                                xo += t.Length;
                            }
                            else if (Interpreter.IsSyntax(tu))
                            {
                                WriteText(tx, xo + Indent,
                                    y, Terminal.LIGHT_GREEN, bc);
                                xo += t.Length;
                            }
                            else if (Lexer.Tokens.Any(p => p.ToString() == tx))
                            {
                                WriteText(tx, xo + Indent,
                                    y, Terminal.YELLOW, bc);
                                xo += t.Length;
                            }
                            else
                            {
                                WriteText(tx, xo + Indent,
                                    y, tx.StartsWith("@") ? Terminal.RED : Terminal.WHITE, bc);
                                xo += t.Length;
                            }
                        }
                    }
                }
                y++;

                if (y >= Height - 1) break;
            }
            while (y < Height - 1)
            {
                bc = CursorY == y ? Terminal.LIGHT_BLUE : BackColor;
                WriteGlyph(Glyph.BarUpDown, Indent - 1, y, Terminal.WHITE, bc);
                y++;
            }


            base.Draw();
            if (!Selected) return;
            SetCursor(CursorX, CursorY);
            BasicOne.CursorVisible = true;
        }

        public override void Select()
        {
            Selected = true;
        }

        public override void Unselect()
        {
            Selected = false;
        }

        public void ScrollToTop()
        {
            ScrollOffsetY = 0;
            CursorY = 1;
            CursorToStart();
        }
    }
}