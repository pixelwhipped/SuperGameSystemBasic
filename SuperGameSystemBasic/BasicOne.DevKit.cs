using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.UI.Input;
using SuperGameSystemBasic.UI.Window;

namespace SuperGameSystemBasic
{
    public partial class BasicOne : IDrawingContext
    {
        private bool _editorHintsShown;

        private bool _errorHintsShown;
        private bool _mouseDown;

        public PopupWindow AboutWindow;

        public CalculatorWindow Calculator;
        public ColorChooserWindow ColorChooser;

        public EditorWindow Editor;
        public AsciiArt GlyphArt;
        public GlyphEditor GlyphEditor;
        public GlyphWindow GlyphWindow;

        public LanguageWindow LanguageWindow;

        public MenuWindow Menu;
        public PopupWindow NextPopupHint;

        public PopupWindow PopupHint;
        public MouseState PreviousMouseState { get; set; }
        public MouseState CurrentMouseState { get; set; }
        public KeyboardState CurrenKeyboardState { get; set; }
        public KeyboardState PreviousKeyboardState { get; set; }

        public Point MouseTerminalLocation
        {
            get
            {
                var w = (float) Graphics.PreferredBackBufferWidth / (Terminal.GlyphWidth * (Terminal.Columns + 1)) *
                        Terminal.GlyphWidth;
                var h = (float) Graphics.PreferredBackBufferHeight / (Terminal.GlyphHeight * (Terminal.Rows + 1)) *
                        Terminal.GlyphHeight;
                var x = (int) MathHelper.Clamp(Mouse.GetState().X / w, 0,
                    Terminal.Columns);
                var y = (int) MathHelper.Clamp(Mouse.GetState().Y / h, 0,
                    Terminal.Rows);
                return new Point(x, y);
            }
        }

        public Rectangle MouseTerminalScreenRect
        {
            get
            {
                var xinc = (float) Graphics.PreferredBackBufferWidth / Terminal.Columns;
                var yinc = (float) Graphics.PreferredBackBufferHeight / Terminal.Rows;
                var x = (int) MathHelper.Clamp(MouseTerminalLocation.X * xinc, 0,
                    Graphics.PreferredBackBufferWidth);
                var y = (int) MathHelper.Clamp(MouseTerminalLocation.Y * yinc, 0,
                    Graphics.PreferredBackBufferHeight);
                return new Rectangle(x, y, (int) xinc, (int) yinc);
            }
        }

        public Rectangle CursorTerminalScreenRect
        {
            get
            {
                var xinc = (float) Graphics.PreferredBackBufferWidth / Terminal.Columns;
                var yinc = (float) Graphics.PreferredBackBufferHeight / Terminal.Rows;
                var x = (int) MathHelper.Clamp(IdeTerminal.X * xinc, 0,
                    Graphics.PreferredBackBufferWidth);
                var y = (int) MathHelper.Clamp(IdeTerminal.Y * yinc, 0,
                    Graphics.PreferredBackBufferHeight);
                return new Rectangle(x, y + ((int) yinc - 2), (int) xinc, 2);
            }
        }

        public bool CursorBlink { get; set; }

        public Timer BlinkTimer { get; set; }

        public Color CursorColor
        {
            get
            {
                if (!CursorVisible || !CursorBlink) return Color.Transparent;
                return Color.White;
            }
        }

        public static bool CursorVisible { get; set; }


        public string IdeVersion { get; set; } = "v1.4";
        public string LanguageVersion { get; set; } = "v1.4";

        private bool _glyphEditorHintsShown { get; set; }
        private bool _glyphHintsShown { get; set; }
        private bool _glyphArtShown { get; set; }

        public int ForeColor
        {
            get => IdeTerminal.VideoBuffer[GetXY(IdeTerminal.X, IdeTerminal.Y)].ForeColor;
            set
            {
                var c = IdeTerminal.VideoBuffer[GetXY(IdeTerminal.X, IdeTerminal.Y)];
                IdeTerminal.VideoBuffer[GetXY(IdeTerminal.X, IdeTerminal.Y)] = new Character(c.Glyph, value,
                    c.BackColor,
                    c.Effect);
            }
        }

        public int BackColor
        {
            get => IdeTerminal.VideoBuffer[GetXY(IdeTerminal.X, IdeTerminal.Y)].BackColor;
            set
            {
                var c = IdeTerminal.VideoBuffer[GetXY(IdeTerminal.X, IdeTerminal.Y)];
                IdeTerminal.VideoBuffer[GetXY(IdeTerminal.X, IdeTerminal.Y)] = new Character(c.Glyph, c.ForeColor,
                    value,
                    c.Effect);
            }
        }


        public void SetCursor(int x, int y)
        {
            IdeTerminal.X = MathHelper.Clamp(x, 0, IdeTerminal.Columns);
            IdeTerminal.Y = MathHelper.Clamp(y, 0, IdeTerminal.Rows);
        }

        public void Clear(int color)
        {
            IdeTerminal.Clear(color);
        }

        public void DrawHLine(int x, int y, int l, int c, bool clear = false)
        {
            for (var i = 0; i < l; i++) SetBackGround(i + x, y, c, clear);
        }

        public void DrawVLine(int x, int y, int l, int c, bool clear = false)
        {
            for (var i = 0; i < l; i++) SetBackGround(x, y + i, c, clear);
        }

        public void FillRect(int x, int y, int w, int h, int c, bool clear = false)
        {
            if (w <= 0 || h <= 0) return;
            for (var i = 0; i < h; i++)
                DrawHLine(x, y + i, w, c, clear);
        }

        public void DrawRect(int x, int y, int w, int h, int c, bool clear = false)
        {
            if (w <= 0 || h <= 0) return;
            DrawHLine(x, y, w, c);
            DrawHLine(x, y + (h - 1), w, c, clear);
            DrawVLine(x, y, h, c);
            DrawVLine(x + (w - 1), y, h, c, clear);
        }

        public void SetBackGround(int x, int y, int c, bool clear = false)
        {
            if (x > IdeTerminal.Columns - 1 || x < 0) return;
            if (y > IdeTerminal.Rows - 1 || y < 0) return;
            var g = IdeTerminal.VideoBuffer[GetXY(x, y)];
            IdeTerminal.VideoBuffer[GetXY(x, y)] =
                new Character(clear ? Glyph.Space : g.Glyph, g.ForeColor, c, g.Effect);
        }

        public void SetForgroundGround(int x, int y, int c, bool clear = false)
        {
            if (x > IdeTerminal.Columns - 1 || x < 0) return;
            if (y > IdeTerminal.Rows - 1 || y < 0) return;
            var g = IdeTerminal.VideoBuffer[GetXY(x, y)];
            IdeTerminal.VideoBuffer[GetXY(x, y)] =
                new Character(clear ? Glyph.Space : g.Glyph, c, g.BackColor, g.Effect);
        }

        public int GetXY(int x, int y)
        {
            return y * IdeTerminal.Columns + x;
        }

        public void WriteText(string s, int x, int y, int fColor, int bColor)
        {
            foreach (var l in SplitToLines(s, IdeTerminal.Columns))
            {
                SetCursor(x, y);
                IdeTerminal.Print(l, fColor, bColor, SpriteEffects.None);
                y++;
            }
        }

        public void WriteGlyph(Glyph g, int x, int y, int fColor, int bColor)
        {
            SetCursor(x, y);
            IdeTerminal.Print(g, fColor, bColor, SpriteEffects.None);
        }

        public void FillRectR(int x, int y, int w, int h, int c, bool clear = false)
        {
            if (w <= 0 || h <= 0) return;
            for (var i = 0; i < h; i++)
                DrawHLineR(x, y + i, w, c, clear);
        }

        public void DrawHLineR(int x, int y, int l, int c, bool clear = false)
        {
            for (var i = 0; i < l; i++) SetBackGroundR(i + x, y, c, clear);
        }

        public void DrawVLineR(int x, int y, int l, int c, bool clear = false)
        {
            for (var i = 0; i < l; i++) SetBackGroundR(x, y + i, c, clear);
        }

        public void SetBackGroundR(int x, int y, int c, bool clear = false)
        {
            if (x > Terminal.Columns - 1 || x < 0) return;
            if (y > Terminal.Rows - 1 || y < 0) return;
            var g = Terminal.VideoBuffer[GetXY(x, y)];
            Terminal.VideoBuffer[GetXY(x, y)] = new Character(clear ? Glyph.Space : g.Glyph, g.ForeColor, c, g.Effect);
        }

        public void SetDevKitState()
        {
            Terminal.SetResolution(IdeTerminal.Columns, IdeTerminal.Rows);
            CurrentMouseState = PreviousMouseState = Mouse.GetState();
            CurrenKeyboardState = PreviousKeyboardState = Keyboard.GetState();
            IdeTerminal.GlyphSheet = Content.Load<Texture2D>(IdeTerminal._sheet);
            IdeTerminal.Pallet = null;
            IdeState = IdeState.DevKit;
            Interpreter = null;
            IdeTerminal.Clear(Terminal.BLUE);
            if (Editor != null)
                if (IdeWindow.Windows.Any(p => p == Editor && p.Visible && !p.Exit))
                    IdeWindow.SelectWindow(Editor);
        }


        private void UpdateIde()
        {
            //Keyboard.Update();

            if (FileArguments != null)
            {
                try
                {

                    var f = FileArguments[0];
                    var sf = f as StorageFile; //Task.Run(async () => await StorageFile.GetFileFromPathAsync(f.Path)).Result;                    
                    var t = AsyncIO.ReadTextFileAsync(sf); // Task.Run(async () => await FileIO.ReadTextAsync(sf)).Result;
                    FileArguments = null;
                    if (Editor != null && Editor.Visible && IdeWindow.Windows.Contains(Editor))
                    {
                        IdeWindow.SelectWindow(new Confirm("Save Current Document",
                            $"Do you wish to save the current Document {Editor.Name}",
                            () =>
                            {
                                SaveFile();
                                OpenEditor(f.Name, null, t, true);
                            },
                            () =>
                            {
                                OpenEditor(f.Name, null, t, true);
                            }, IdeWindow));
                    }
                    else
                    {
                        OpenEditor(f.Name, null, t, true);
                    }                  
                }
                catch (Exception e)
                {
                    var p = e;
                }
            }

            CurrenKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();

            var click = false;

            if (_mouseDown && CurrentMouseState.LeftButton == ButtonState.Released)
            {
                click = true;
                _mouseDown = false;
            }
            if (!_mouseDown && CurrentMouseState.LeftButton == ButtonState.Pressed)
                _mouseDown = true;

            var current = CurrenKeyboardState.GetPressedKeys();
            var pressed = PreviousKeyboardState.GetPressedKeys().Where(k => !current.Contains(k));
            var keys = pressed as Keys[] ?? pressed.ToArray();

            if (keys.Any())
                foreach (var k in keys)
                {
                    if (k == Keys.LeftShift || k == Keys.RightShift) continue;
                    IdeWindow.Update(k, () =>
                    {
                        if (TryConvertKeyboardInput(k, CurrenKeyboardState, PreviousKeyboardState, out var c)) return c;
                        return null;
                    }, click, CurrentMouseState.X, CurrentMouseState.Y);
                }
            else if (click)
                IdeWindow.Update(Keys.None, () => null, true, CurrentMouseState.X, CurrentMouseState.Y);
            PreviousKeyboardState = CurrenKeyboardState;
            IdeWindow.Draw();
            IdeIpsTimer.Update();
        }

        public void InitializeIde()
        {
            PopupHint = new PopupWindow("Welcome", 0, 0, 41, 15, IdeWindow);
            PopupHint.Inputs.Add(
                new TextBlock(
                    "Welcome to Super G Basic\n \nWe are please to announce you have been chosen to be part of our early partner program.\nAs such you have been provided with this prototype(early beta) Development Kit for the upcoming release of our new entertainment system the Super Game System.\nThis is early days so while we improve and add features to the system we will roll them out to you.\nIn the mean time go though the tutorials and familiarize yourself with the language and system.\nOur system architects as super proud with their prototype and think it's unreal graphics and specialized language are going to make this system the most competitive out there.\nOur system specialists will pop by with hints every now and then to help you out and make sure your primed to produce some launch titles for the system.\nHave Fun!!!\n \nSincerely\n \nMr P Whipped.",
                    0, 1, PopupHint.Width, PopupHint.Height - 2, PopupHint));
            PopupHint.Inputs.Add(new Button(37, 14, "Ok", PopupHint)
            {
                Action = () =>
                {
                    PopupHint.ExitWindow();
                    if (NextPopupHint != null && NextPopupHint != PopupHint) PopupHint = NextPopupHint;
                }
            });
            PopupHint.Center();

            NextPopupHint = PopupHint;
            if (ShowHints) IdeWindow.SelectWindow(PopupHint);// .Windows.Add(PopupHint);

            AboutWindow = new PopupWindow("About", 0, 0, 50, 9, IdeWindow);
            AboutWindow.Inputs.Add(new Label($"Super Game System {IdeVersion}\nSuper G Basic {LanguageVersion}", 0, 1,
                AboutWindow));
            AboutWindow.Inputs.Add(
                new Label(() => $"DevKit IPS(Instructions Per Second): {Math.Round(IdeIpsTimer.Ips, 2)}", 0, 3,
                    AboutWindow));
            AboutWindow.Inputs.Add(
                new Label(() => $"Prototype IPS(Instructions Per Second): {Math.Round(RunningIpsTimer.Ips, 2)}", 0, 4,
                    AboutWindow));
            AboutWindow.Inputs.Add(new Label("Pixel Whipped 2017", 0, 7, AboutWindow));
            AboutWindow.Inputs.Add(new Button(46, 8, "Ok", AboutWindow)
            {
                Action = () => { AboutWindow.ExitWindow(); }
            });
            AboutWindow.Inputs.Add(new Button(38, 8, " WWW ", AboutWindow)
            {
                Action = () =>
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Launcher.LaunchUriAsync(new Uri(
                                "http://www.indiedb.com/members/pixelwhipped"));
                        }
                        catch
                        {
                        }
                    });
                }
            });
            AboutWindow.Inputs.Add(new CheckBox(0, 8, "ScanLines", AboutWindow)
            {
                Checked = ScanLines,
                Action = () => ScanLines = !ScanLines
            });

            AboutWindow.Inputs.Add(new CheckBox(13, 8, "OverScan", AboutWindow)
            {
                Checked = OverScan,
                Action = () => OverScan = !OverScan
            });
            AboutWindow.Inputs.Add(new CheckBox(25, 8, "Hints", AboutWindow)
            {
                Checked = ShowHints,
                Action = () => ShowHints = !ShowHints
            });
            AboutWindow.Center();
            AboutWindow.PostionX += 2;
            AboutWindow.PostionY += 2;

            Editor = new EditorWindow(this, null, null, 0, 1, 60, 23, IdeWindow);


            Menu = new MenuWindow(IdeWindow);
            Menu.AddItem("File", new List<MenuButton>
            {
                new MenuButton(0, 0, "Tutorial", Menu)
                {
                    Action = () =>
                    {
                        var ld = new LoadDialog("Load Tutorial", IdeWindow, false, TutorialsStorageFolder);
                        ld.SelectFile = file =>
                        {
                            LoadFile(file, ld, AsyncIO.ReadTextFileAsync(file));
                            Editor.ScrollToTop();
                        };
                        IdeWindow.SelectWindow(ld);
                    }
                },
                new MenuButton(0, 0, "New", Menu)
                {
                    Action = () => { OpenEditor(null, null, null, true); }
                },
                new MenuButton(0, 0, "Load", Menu)
                {
                    Action = () =>
                    {
                        var ld = new LoadDialog("Load File", IdeWindow, true);
                        ld.SelectFile = file =>
                        {
                            LoadFile(file, ld, null);
                            Editor.ScrollToTop();
                        };
                        IdeWindow.SelectWindow(ld);
                    }
                },
                new MenuButton(0, 0, "Save", Menu)
                {
                    Action = SaveFile
                },
                new MenuButton(0, 0, "Save As", Menu)
                {
                    Action = SaveFileAs
                },
                new MenuButton(0, 0, "Exit", Menu)
                {
                    Action = () =>
                    {
                        if (Editor != null && Editor.Visible && IdeWindow.Windows.Contains(Editor))
                            if (!Editor.Saved)
                                IdeWindow.SelectWindow(new Confirm("Save Current Document",
                                    $"Do you wish to save the current Document {Editor.Name}",
                                    () =>
                                    {
                                        SaveFile();
                                        Exit();
                                    },
                                    Exit, IdeWindow));
                            else
                                Exit();
                        else
                            Exit();
                    }
                }
            });


            Calculator = new CalculatorWindow(61, 1, IdeWindow);
            ColorChooser = new ColorChooserWindow(49, 2, IdeWindow);
            GlyphWindow = new GlyphWindow(40, 14, IdeWindow);
            GlyphEditor = new GlyphEditor(30, 0, IdeWindow, this);
            GlyphArt = new AsciiArt(1, 1, IdeWindow, IdeTerminal);
            Menu.AddItem("Windows",
                new List<MenuButton>
                {
                    new MenuButton(0, 0, "Next [F2]", Menu)
                    {
                        Action = NextWindow
                    },
                    new MenuButton(0, 0, "Calculator", Menu)
                    {
                        Action = () =>
                        {
                            if (Calculator.Exit || Calculator.Visible == false)
                            {
                                Calculator.Exit = false;
                                Calculator.Visible = true;
                            }
                            //if (!IdeWindow.Windows.Contains(Calculator))
                            //    IdeWindow.Windows.Add(Calculator);
                            IdeWindow.SelectWindow(Calculator);
                        }
                    },
                    new MenuButton(0, 0, "Last Error", Menu)
                    {
                        Action = ShowLastError
                    },
                    new MenuButton(0, 0, "Colors", Menu)
                    {
                        Action = () =>
                        {
                            if (ColorChooser.Exit || ColorChooser.Visible == false)
                            {
                                ColorChooser.Exit = false;
                                ColorChooser.Visible = true;
                            }
                            //if (!IdeWindow.Windows.Contains(ColorChooser))
                            //    IdeWindow.Windows.Add(ColorChooser);
                            IdeWindow.SelectWindow(ColorChooser);
                        }
                    },
                    new MenuButton(0, 0, "Glyphs", Menu)
                    {
                        Action = () =>
                        {
                            if (GlyphWindow.Exit || GlyphWindow.Visible == false)
                            {
                                GlyphWindow.Exit = false;
                                GlyphWindow.Visible = true;
                            }
                            //if (!IdeWindow.Windows.Contains(GlyphWindow))
                           //     IdeWindow.Windows.Add(GlyphWindow);
                            IdeWindow.SelectWindow(GlyphWindow);
                            if (ShowHints && !_glyphHintsShown) ShowGlyphHints();
                        }
                    },
                    new MenuButton(0, 0, "Glyph Editor", Menu)
                    {
                        Action = () =>
                        {
                            if (GlyphEditor.Exit || GlyphEditor.Visible == false)
                            {
                                GlyphEditor.Exit = false;
                                GlyphEditor.Visible = true;
                            }
                            //if (!IdeWindow.Windows.Contains(GlyphEditor))
                            //    IdeWindow.Windows.Add(GlyphEditor);
                            IdeWindow.SelectWindow(GlyphEditor);
                            if (ShowHints && !_glyphEditorHintsShown) ShowGlyphEditorHints();
                        }
                    },
                    new MenuButton(0, 0, "Glyph Art", Menu)
                    {
                        Action = () =>
                        {
                            if (GlyphArt.Exit || GlyphArt.Visible == false)
                            {
                                GlyphArt.Exit = false;
                                GlyphArt.Visible = true;
                            }
                            //if (!IdeWindow.Windows.Contains(GlyphArt))
                            //    IdeWindow.Windows.Add(GlyphArt);
                            IdeWindow.SelectWindow(GlyphArt);
                            if (ShowHints && !_glyphArtShown) ShowGlyphArtHints();
                        }
                    }
                });

            LanguageWindow = new LanguageWindow(this, 19, 1, 60, 23, IdeWindow);
            Menu.AddItem("Help", new List<MenuButton>
            {
                new MenuButton(0, 0, "Last Hint", Menu)
                {
                    Action = () =>
                    {
                        if (NextPopupHint.Exit || NextPopupHint.Visible == false)
                        {
                            NextPopupHint.Exit = false;
                            NextPopupHint.Visible = true;
                        }
                        //if (!IdeWindow.Windows.Contains(NextPopupHint))
                        //    IdeWindow.Windows.Add(NextPopupHint);
                        IdeWindow.SelectWindow(NextPopupHint);
                    }
                },
                new MenuButton(0, 0, "About", Menu)
                {
                    Action = () =>
                    {
                        if (AboutWindow.Exit || AboutWindow.Visible == false)
                        {
                            AboutWindow.Exit = false;
                            AboutWindow.Visible = true;
                        }
                        //if (!IdeWindow.Windows.Contains(AboutWindow))
                        //    IdeWindow.Windows.Add(AboutWindow);
                        IdeWindow.SelectWindow(AboutWindow);
                    }
                },
                new MenuButton(0, 0, "Language", Menu)
                {
                    Action = () =>
                    {
                        if (LanguageWindow.Exit || LanguageWindow.Visible == false)
                        {
                            LanguageWindow.Exit = false;
                            LanguageWindow.Visible = true;
                        }
                        //if (!IdeWindow.Windows.Contains(LanguageWindow))
                        //    IdeWindow.Windows.Add(LanguageWindow);
                        IdeWindow.SelectWindow(LanguageWindow);
                    }
                },
                new MenuButton(0, 0, "What's New", Menu)
                {
                    Action = () =>
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await Launcher.LaunchUriAsync(new Uri(
                                    "http://www.indiedb.com/games/super-game-system-basic/forum/thread/version-14/#1078057"));
                            }
                            catch
                            {
                            }
                        });
                    }
                }
            });
            //     menu.AddItem("Menu2");
            IdeWindow.Windows.Add(Menu);
            
        }

        private void NextWindow()
        {
            IdeWindow.SelectNextWindow();
        }

        public void OpenEditor(string name, IStorageFile file, string code, bool reset)
        {
            name = name ?? "new.basic";
            ErrorLine = -1;
            LastError = null;
            if (Editor.Exit || Editor.Visible == false)
            {
                Editor.Name = name;
                Editor.CodeFile = file; // = new EditorWindow(this, name, code, 0, 1, 60, 23, IdeWindow);
                if (code != null) Editor.Code = code;
                Editor.Visible = true;
                Editor.Exit = false;
                IdeWindow.SelectWindow(Editor);
            }
            if (!IdeWindow.Windows.Contains(Editor))
            {
                if (Editor != null)
                {
                    Editor.Name = name;
                    Editor.CodeFile = file;
                    if (code != null) Editor.Code = code;
                    Editor.Visible = true;
                    Editor.Exit = false;
                }
                else
                {
                    Editor = new EditorWindow(this, name, file, 0, 1, 60, 23, IdeWindow);
                }
               // IdeWindow.Windows.Add(Editor);
                IdeWindow.SelectWindow(Editor);
            }
            else
            {
                if (Editor != null)
                {
                    Editor.Name = name;
                    Editor.CodeFile = file;
                    if (code != null) Editor.Code = code;
                    Editor.Visible = true;
                    Editor.Exit = false;
                }
                else
                {
                    Editor = new EditorWindow(this, name, file, 0, 1, 60, 23, IdeWindow);
                    if (code != null) Editor.Code = code;
                }
                IdeWindow.SelectWindow(Editor);
                if (reset)
                {
                    Editor.ScrollToTop();
                    Editor.CursorToStart();
                }
            }
            if (ShowHints && !_editorHintsShown) ShowEditorHints();
        }

        public void ShowGlyphHints()
        {
            _glyphHintsShown = true;
            var hint = new PopupWindow("Glyph Hints", 0, 0, 41, 15, IdeWindow);
            hint.Inputs.Add(
                new TextBlock(
                    "The Glyph Window displays all system glyphs. you can copy these and paste the constant for them in the Editor or in the Glyph Editor.\n",
                    0, 1, hint.Width, hint.Height - 2, hint));
            hint.Inputs.Add(new Button(37, 14, "Ok", hint)
            {
                Action = () =>
                {
                    hint.ExitWindow();
                    if (NextPopupHint != null && NextPopupHint != hint) PopupHint = NextPopupHint;
                }
            });
            hint.Center();
            NextPopupHint = hint;
            if (ShowHints) IdeWindow.SelectWindow(NextPopupHint); //IdeWindow.Windows.Add(hint);
        }

        public void ShowGlyphArtHints()
        {
            _glyphArtShown = true;
            var hint = new PopupWindow("Glyph Art Hints", 0, 0, 41, 15, IdeWindow);
            hint.Inputs.Add(
                new TextBlock(
                    "The Glyph Art Window allows you to open an image which it will show as Glyph Art and is copied as an array that can be pasted into the Editor with F4.\n",
                    0, 1, hint.Width, hint.Height - 2, hint));
            hint.Inputs.Add(new Button(37, 14, "Ok", hint)
            {
                Action = () =>
                {
                    hint.ExitWindow();
                    if (NextPopupHint != null && NextPopupHint != hint) PopupHint = NextPopupHint;
                }
            });
            hint.Center();
            NextPopupHint = hint;
            if (ShowHints) IdeWindow.SelectWindow(NextPopupHint); //IdeWindow.Windows.Add(hint);if (ShowHints) IdeWindow.SelectWindow(hint);
        }

        public void ShowErrorHints()
        {
            _errorHintsShown = true;
            var hint = new PopupWindow("Exception Hints", 0, 0, 41, 15, IdeWindow);
            hint.Inputs.Add(
                new TextBlock(
                    "Congratulations!!!!\nYour first error.\nThe last window should have given you a clue but you can look for the little Alien glyph" +
                    (char) 212 + " for the offending line.",
                    0, 1, hint.Width, hint.Height - 2, hint));
            hint.Inputs.Add(new Button(37, 14, "Ok", hint)
            {
                Action = () =>
                {
                    hint.ExitWindow();
                    if (NextPopupHint != null && NextPopupHint != hint) PopupHint = NextPopupHint;
                }
            });
            hint.Center();
            NextPopupHint = hint;
            if (ShowHints) IdeWindow.SelectWindow(NextPopupHint); //IdeWindow.Windows.Add(hint);if (ShowHints) IdeWindow.Windows.Add(hint);
        }

        public void ShowGlyphEditorHints()
        {
            _glyphEditorHintsShown = true;
            var hint = new PopupWindow("Glyph Editor Hints", 0, 0, 41, 15, IdeWindow);
            hint.Inputs.Add(
                new TextBlock(
                    "The Glyph editor allows you to edit a glyph for use in your game.\nIt's also handy for maps and such.\n You can copy from the Glyph Window and Paste directly here or from an external source providing it matches the Dim or Array Pattern.\nFor example you could paste DIM x = {1,0,1,0.....} but the lenght must equal 20x24 for the glyph Width and Height",
                    0, 1, hint.Width, hint.Height - 2, hint));
            hint.Inputs.Add(new Button(37, 14, "Ok", hint)
            {
                Action = () =>
                {
                    hint.ExitWindow();
                    if (NextPopupHint != null && NextPopupHint != hint) PopupHint = NextPopupHint;
                }
            });
            hint.Center();
            NextPopupHint = hint;
            if (ShowHints) IdeWindow.SelectWindow(NextPopupHint); //IdeWindow.Windows.Add(hint);if (ShowHints) IdeWindow.Windows.Add(hint);
        }

        public void ShowEditorHints()
        {
            _editorHintsShown = true;
            var hint = new PopupWindow("Editor Hints", 0, 0, 41, 15, IdeWindow);
            hint.Inputs.Add(
                new TextBlock(
                    "Welcome to the code editor where all the magic happens.\nYou can run your program by pressing F5.\nRemember! press PAUSE or BREAK to end it at any time.\nUse the Arrow keys, Home, End, Page Up and Page Down to move around\nPressing SHIFT and DEL will delete all text after the cursor.\n Move around using the Arrow Keys or Mouse and clicking or pressing ENTER or SPACE.",
                    0, 1, hint.Width, hint.Height - 2, hint));
            hint.Inputs.Add(new Button(37, 14, "Ok", hint)
            {
                Action = () =>
                {
                    hint.ExitWindow();
                    if (NextPopupHint != null && NextPopupHint != hint) PopupHint = NextPopupHint;
                }
            });
            hint.Center();
            NextPopupHint = hint;
            if (ShowHints) IdeWindow.SelectWindow(NextPopupHint); //IdeWindow.Windows.Add(hint);if (ShowHints) IdeWindow.Windows.Add(hint);
        }

        private void LoadFile(StorageFile file, LoadDialog ld, string code)
        {
            if (Editor != null && Editor.Visible && IdeWindow.Windows.Contains(Editor))
            {
                IdeWindow.SelectWindow(new Confirm("Save Current Document",
                    $"Do you wish to save the current Document {Editor.Name}",
                    () =>
                    {
                        SaveFile();
                        ld.ExitWindow();
                        OpenEditor(file.Name, code == null ? file : null, code, true);
                    },
                    () =>
                    {
                        ld.ExitWindow();
                        OpenEditor(file.Name, code == null ? file : null, code, true);
                    }, IdeWindow));
            }
            else
            {
                ld.ExitWindow();
                OpenEditor(file.Name, code == null ? file : null, code, true);
            }
        }

        public void SaveFile()
        {
            if (Editor == null || !Editor.Visible || !IdeWindow.Windows.Contains(Editor))
                IdeWindow.SelectWindow(new Alert("Save", "Nothing to Save", IdeWindow));
            else if (Editor.CodeFile == null)
                SaveFileAs();
            else
                Save();
        }

        private void Save()
        {
            Task.Run(async () =>
            {
                await FileIO.WriteTextAsync(Editor.CodeFile, Editor.Code);
                OpenEditor(Editor.Name, Editor.CodeFile, null, false);
            }).Wait();
        }

        private void SaveFileAs()
        {
            if (Editor == null || !Editor.Visible || !IdeWindow.Windows.Contains(Editor))
                IdeWindow.SelectWindow(new Alert("Save", "Nothing to Save", IdeWindow));
            else
                IdeWindow.SelectWindow(new SaveDialog(this, "Save File", Editor.Name, IdeWindow));
        }

        /// <summary>
        ///     Tries to convert keyboard input to characters and prevents repeatedly returning the
        ///     same character if a key was pressed last frame, but not yet unpressed this frame.
        /// </summary>
        /// <param name="keyboard">The current KeyboardState</param>
        /// <param name="oldKeyboard">The KeyboardState of the previous frame</param>
        /// <param name="key">
        ///     When this method returns, contains the correct character if conversion succeeded.
        ///     Else contains the null, (000), character.
        /// </param>
        /// <returns>True if conversion was successful</returns>
        public static bool TryConvertKeyboardInput(Keys k, KeyboardState keyboard, KeyboardState oldKeyboard,
            out char key)
        {
            //  var keys = oldKeyboard.GetPressedKeys();
            var pshift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            var shift = pshift;
            if (Windows.UI.Xaml.Window.Current.CoreWindow.GetKeyState(VirtualKey.CapitalLock) ==
                CoreVirtualKeyStates.Locked)
                shift = !shift;
            //if (keys.Length > 0 && keyboard.IsKeyUp(keys[0]))
            {
                switch (k)
                {
                    //Alphabet keys
                    case Keys.A:
                        key = shift ? 'A' : 'a';
                        return true;
                    case Keys.B:
                        key = shift ? 'B' : 'b';
                        return true;
                    case Keys.C:
                        key = shift ? 'C' : 'c';
                        return true;
                    case Keys.D:
                        key = shift ? 'D' : 'd';
                        return true;
                    case Keys.E:
                        key = shift ? 'E' : 'e';
                        return true;
                    case Keys.F:
                        key = shift ? 'F' : 'f';
                        return true;
                    case Keys.G:
                        key = shift ? 'G' : 'g';
                        return true;
                    case Keys.H:
                        key = shift ? 'H' : 'h';
                        return true;
                    case Keys.I:
                        key = shift ? 'I' : 'i';
                        return true;
                    case Keys.J:
                        key = shift ? 'J' : 'j';
                        return true;
                    case Keys.K:
                        key = shift ? 'K' : 'k';
                        return true;
                    case Keys.L:
                        key = shift ? 'L' : 'l';
                        return true;
                    case Keys.M:
                        key = shift ? 'M' : 'm';
                        return true;
                    case Keys.N:
                        key = shift ? 'N' : 'n';
                        return true;
                    case Keys.O:
                        key = shift ? 'O' : 'o';
                        return true;
                    case Keys.P:
                        key = shift ? 'P' : 'p';
                        return true;
                    case Keys.Q:
                        key = shift ? 'Q' : 'q';
                        return true;
                    case Keys.R:
                        key = shift ? 'R' : 'r';
                        return true;
                    case Keys.S:
                        key = shift ? 'S' : 's';
                        return true;
                    case Keys.T:
                        key = shift ? 'T' : 't';
                        return true;
                    case Keys.U:
                        key = shift ? 'U' : 'u';
                        return true;
                    case Keys.V:
                        key = shift ? 'V' : 'v';
                        return true;
                    case Keys.W:
                        key = shift ? 'W' : 'w';
                        return true;
                    case Keys.X:
                        key = shift ? 'X' : 'x';
                        return true;
                    case Keys.Y:
                        key = shift ? 'Y' : 'y';
                        return true;
                    case Keys.Z:
                        key = shift ? 'Z' : 'z';
                        return true;
                    //Decimal keys
                    case Keys.D0:
                        key = pshift ? ')' : '0';
                        return true;
                    case Keys.D1:
                        key = pshift ? '!' : '1';
                        return true;
                    case Keys.D2:
                        key = pshift ? '@' : '2';
                        return true;
                    case Keys.D3:
                        key = pshift ? '#' : '3';
                        return true;
                    case Keys.D4:
                        key = pshift ? '$' : '4';
                        return true;
                    case Keys.D5:
                        key = pshift ? '%' : '5';
                        return true;
                    case Keys.D6:
                        key = pshift ? '^' : '6';
                        return true;
                    case Keys.D7:
                        key = pshift ? '&' : '7';
                        return true;
                    case Keys.D8:
                        key = pshift ? '*' : '8';
                        return true;
                    case Keys.D9:
                        key = pshift ? '(' : '9';
                        return true;
                    case Keys.Decimal:
                        key = '.';
                        return true;
                    //Decimal numpad keys
                    case Keys.NumPad0:
                        key = '0';
                        return true;
                    case Keys.NumPad1:
                        key = '1';
                        return true;
                    case Keys.NumPad2:
                        key = '2';
                        return true;
                    case Keys.NumPad3:
                        key = '3';
                        return true;
                    case Keys.NumPad4:
                        key = '4';
                        return true;
                    case Keys.NumPad5:
                        key = '5';
                        return true;
                    case Keys.NumPad6:
                        key = '6';
                        return true;
                    case Keys.NumPad7:
                        key = '7';
                        return true;
                    case Keys.NumPad8:
                        key = '8';
                        return true;
                    case Keys.NumPad9:
                        key = '9';
                        return true;

                    //Special keys
                    case Keys.OemTilde:
                        key = pshift ? '~' : '`';
                        return true;
                    case Keys.OemSemicolon:
                        key = pshift ? ':' : ';';
                        return true;
                    case Keys.OemQuotes:
                        key = pshift ? '"' : '\'';
                        return true;
                    case Keys.OemQuestion:
                        key = pshift ? '?' : '/';
                        return true;
                    case Keys.Multiply:
                        key = '*';
                        return true;
                    case Keys.Divide:
                        key = '/';
                        return true;
                    case Keys.Subtract:
                        key = '-';
                        return true;
                    case Keys.Add:
                        key = '+';
                        return true;
                    case Keys.OemPlus:
                        key = pshift ? '+' : '=';
                        return true;
                    case Keys.OemPipe:
                        key = pshift ? '|' : '\\';
                        return true;
                    case Keys.OemPeriod:
                        key = pshift ? '>' : '.';
                        return true;
                    case Keys.OemOpenBrackets:
                        key = pshift ? '{' : '[';
                        return true;
                    case Keys.OemCloseBrackets:
                        key = pshift ? '}' : ']';
                        return true;
                    case Keys.OemMinus:
                        key = pshift ? '_' : '-';
                        return true;
                    case Keys.OemComma:
                        key = pshift ? '<' : ',';
                        return true;
                    case Keys.Space:
                        key = ' ';
                        return true;
                }
            }

            key = (char) 0;
            return false;
        }

        public static IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
        {
            //    stringToSplit = stringToSplit;
            var lines = new List<string>();
            var paragraphs = stringToSplit.Split('\n');
            foreach (var p in paragraphs)
            {
                stringToSplit = p;
                while (stringToSplit.Length > 0)
                {
                    if (stringToSplit.Length <= maximumLineLength)
                    {
                        lines.Add(stringToSplit);
                        break;
                    }

                    var indexOfLastSpaceInLine = stringToSplit.Substring(0, maximumLineLength).LastIndexOf(' ');
                    lines.Add(
                        stringToSplit.Substring(0,
                                indexOfLastSpaceInLine >= 0 ? indexOfLastSpaceInLine : maximumLineLength)
                            .Trim()
                            .PadRight(maximumLineLength, ' '));
                    stringToSplit =
                        stringToSplit.Substring(indexOfLastSpaceInLine >= 0
                            ? indexOfLastSpaceInLine + 1
                            : maximumLineLength);
                }
            }
            return lines.ToArray();
        }

        public void ShowLastError()
        {
            var hint = new PopupWindow("Last Error", 0, 0, 41, 15, IdeWindow);
            if (LastError?.Message != null)
                hint.Inputs.Add(
                    new TextBlock(
                        LastError.Message,
                        0, 1, hint.Width, hint.Height - 2, hint));
            else
                hint.Inputs.Add(
                    new TextBlock(
                        "Unknown?",
                        0, 1, hint.Width, hint.Height - 2, hint));
            hint.Inputs.Add(new Button(37, 14, "Ok", hint)
            {
                Action = () =>
                {
                    hint.ExitWindow();
                    if (NextPopupHint != null && NextPopupHint != hint) PopupHint = NextPopupHint;
                }
            });
            hint.Center();
            NextPopupHint = hint;
            IdeWindow.Windows.Add(hint);
            IdeWindow.SelectWindow(hint);
            IdeWindow.SkipNextWindowSelect = true;
        }
    }
}