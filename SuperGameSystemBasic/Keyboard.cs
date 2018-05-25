using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.Xna.Framework.Input;

namespace SuperGameSystemBasic
{
    public class KeyboardInput
    {
        #region Members

        //  private KeyboardState _currKey;
        //  private KeyboardState _prevKey;
        private readonly List<Action<KeyboardInput>> _keyboardListeners;

        #endregion

        public KeyboardInput()
        {
            Typed = new List<string>();


            _keyboardListeners = new List<Action<KeyboardInput>>();
            _oskCurrentState = new List<Keys>();
            _oskPreviousState = new List<Keys>();
        }

        public bool IsCapsLock
            => Window.Current.CoreWindow.GetKeyState(VirtualKey.CapitalLock) == CoreVirtualKeyStates.Locked;

        public void Update()
        {
            #region reset states

            _oskPreviousState.Clear();
            _oskPreviousState.AddRange(_oskCurrentState);
            _oskCurrentState.Clear();
            foreach (var k in Enum.GetValues(typeof(Keys)))
            {
                if ((Keys) k == Keys.None) continue;
                if (Keyboard.GetState().IsKeyDown((Keys) k) && !_oskCurrentState.Contains((Keys) k))
                    _oskCurrentState.Add((Keys) k);
            }

            #endregion

            Typed.Clear();
            foreach (var t in _keys)
            {
                if (!TypedKey(t)) continue;
                var l = t.ToString();
                if (l == "Tab")
                {
                    l = "\t";
                }
                else if (l.Length == 1)
                {
                    //if (Released(Keys.LeftShift) && Released(Keys.RightShift))
                    //    l = l.ToLower();
                    if (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift) && IsCapsLock)
                        l = l.ToUpper();
                    else if (IsCapsLock)
                        l = l.ToUpper();
                    else
                        l = l.ToLower();
                }
                else
                {
                    #region Name Numbers

                    if (l.StartsWith(D) & !l.Equals(DecimalStr) &
                        !l.Equals(Divide))
                    {
                        l = l.Substring(1);
                        if (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                            switch (l)
                            {
                                case Zero:
                                {
                                    l = ZeroShift;
                                    break;
                                }
                                case One:
                                {
                                    l = OneShift;
                                    break;
                                }
                                case Two:
                                {
                                    l = TwoShift;
                                    break;
                                }
                                case Three:
                                {
                                    l = ThreeShift;
                                    break;
                                }
                                case Four:
                                {
                                    l = FourShift;
                                    break;
                                }
                                case Five:
                                {
                                    l = FiveShift;
                                    break;
                                }
                                case Six:
                                {
                                    l = SixShift;
                                    break;
                                }
                                case Seven:
                                {
                                    l = SevenShift;
                                    break;
                                }
                                case Eight:
                                {
                                    l = EightShift;
                                    break;
                                }
                                case Nine:
                                {
                                    l = NineShift;
                                    break;
                                }
                            }
                    }

                    #endregion

                    else if (l.StartsWith(NumPad))
                    {
                        l = l.Substring(6);
                    }
                    else
                    {
                        switch (l)
                        {
                            case OemCommaStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemCommaShift
                                    : OemComma;
                                break;
                            }
                            case OemPeriodStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemPeriodShift
                                    : OemPeriod;
                                break;
                            }
                            case OemQuestionStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemQuestionShift
                                    : OemQuestion;
                                break;
                            }
                            case OemSemicolonStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemSemicolonShift
                                    : OemSemicolon;
                                break;
                            }
                            case OemQuotesStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemQuotesShift
                                    : OemQuotes;
                                break;
                            }
                            case OemOpenBracketsStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemOpenBracketsShift
                                    : OemOpenBrackets;
                                break;
                            }
                            case OemCloseBracketsStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemCloseBracketsShift
                                    : OemCloseBrackets;
                                break;
                            }
                            case OemPipeStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemPipeShift
                                    : OemPipe;
                                break;
                            }
                            case OemPlusStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemPlusShift
                                    : OemPlus;
                                break;
                            }
                            case OemMinusStr:
                            {
                                l = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)
                                    ? OemMinusShift
                                    : OemMinus;
                                break;
                            }
                            case TabStr:
                            {
                                l = Tab;
                                break;
                            }
                            case MultiplyStr:
                            {
                                l = Multiply;
                                break;
                            }
                            case DivideStr:
                            {
                                l = Divide;
                                break;
                            }
                            case SubtractStr:
                            {
                                l = Subtract;
                                break;
                            }
                            case AddStr:
                            {
                                l = Add;
                                break;
                            }
                            case DecimalStr:
                            {
                                l = Dot;
                                break;
                            }
                        }
                    }
                }
                Typed.Add(l);
                CurrentLine += l;
            }

            // Check input for spacebar
            if (TypedKey(Keys.Space) && CurrentLine != string.Empty &&
                CurrentLine[CurrentLine.Length - 1].ToString() != Space)
                CurrentLine += Space;


            // Check input for backspace
            if (TypedKey(Keys.Back) && CurrentLine != string.Empty)
                CurrentLine = CurrentLine.Remove(CurrentLine.Length - 1, 1);

            // Check input for enter
            if (TypedKey(Keys.Enter))
            {
                PreviousLine = CurrentLine;
                CurrentLine = string.Empty;
                Typed.Add("\n");
            }
            if (_oskCurrentState == _oskPreviousState) return;

            var remove = new List<Action<KeyboardInput>>();
            foreach (var keyboardListener in _keyboardListeners)
                try
                {
                    keyboardListener(this);
                }
                catch
                {
                    remove.Add(keyboardListener);
                }
            _keyboardListeners.RemoveAll(remove.Contains);
        }

        #region Listeners

        public void AddKeyboardListener(Action<KeyboardInput> listener)
        {
            _keyboardListeners.Add(listener);
        }

        #endregion

        #region Key Definitions

        #region Key Strings

        private const string D = "D";
        private const string DecimalStr = "Decimal";
        private const string DivideStr = "Divide";
        private const string Zero = "0";
        private const string One = "1";
        private const string Two = "2";
        private const string Three = "3";
        private const string Four = "4";
        private const string Five = "5";
        private const string Six = "6";
        private const string Seven = "7";
        private const string Eight = "8";
        private const string Nine = "9";
        private const string ZeroShift = "}";
        private const string OneShift = "!";
        private const string TwoShift = "@";
        private const string ThreeShift = "#";
        private const string FourShift = "$";
        private const string FiveShift = "%";
        private const string SixShift = "^";
        private const string SevenShift = "&";
        private const string EightShift = "*";
        private const string NineShift = "(";
        private const string NumPad = "NumPad";
        private const string Dot = ".";
        private const string OemCommaStr = "OemComma";
        private const string OemComma = ",";
        private const string OemCommaShift = "<";
        private const string OemPeriodStr = "OemPeriod";
        private const string OemPeriod = ".";
        private const string OemPeriodShift = ">";
        private const string OemQuestionStr = "OemQuestion";
        private const string OemQuestion = "/";
        private const string OemQuestionShift = "?";
        private const string OemSemicolonStr = "OemSemicolon";
        private const string OemSemicolon = ";";
        private const string OemSemicolonShift = ":";
        private const string OemQuotesStr = "OemQuotes";
        private const string OemQuotes = "'";
        private const string OemQuotesShift = "\"";
        private const string OemOpenBracketsStr = "OemOpenBrackets";
        private const string OemOpenBrackets = "[";
        private const string OemOpenBracketsShift = "{";
        private const string OemCloseBracketsStr = "OemCloseBrackets";
        private const string OemCloseBrackets = "]";
        private const string OemCloseBracketsShift = "}";
        private const string OemPipeStr = "OemPipe";
        private const string OemPipe = "\\";
        private const string OemPipeShift = "|";
        private const string OemPlusStr = "OemPlus";
        private const string OemPlus = "=";
        private const string OemPlusShift = "+";
        private const string OemMinusStr = "OemMinus";
        private const string OemMinus = "-";
        private const string OemMinusShift = "_";
        private const string TabStr = "Tab";
        private const string Tab = "     ";
        private const string MultiplyStr = "Multiply";
        private const string Multiply = "*";
        private const string Divide = "/";
        private const string SubtractStr = "Subtract";
        private const string Subtract = "-";
        private const string AddStr = "Add";
        private const string Add = "+";
        private const string Space = " ";

        #endregion

        #region Alpha Keys

        private readonly Keys[] _alphaKeys =
        {
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I,
            Keys.J, Keys.K, Keys.L, Keys.M,
            Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V,
            Keys.W, Keys.X, Keys.Y, Keys.Z
        };

        #endregion

        #region All Used Key

        private readonly Keys[] _keys =
        {
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I,
            Keys.J, Keys.K, Keys.L, Keys.M,
            Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V,
            Keys.W, Keys.X, Keys.Y, Keys.Z,
            Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7,
            Keys.D8, Keys.D9,
            Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4,
            Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
            Keys.OemComma, Keys.OemPeriod, Keys.OemQuestion, Keys.OemSemicolon,
            Keys.OemQuotes, Keys.OemOpenBrackets, Keys.OemCloseBrackets, Keys.OemPipe,
            Keys.OemPlus, Keys.OemMinus,
            Keys.Tab, Keys.Divide, Keys.Multiply, Keys.Subtract, Keys.Add, Keys.Decimal
        };

        #endregion

        #region Numerical Key

        private readonly Keys[] _numericKeys =
        {
            Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6,
            Keys.D7,
            Keys.D8, Keys.D9
        };

        #endregion

        #endregion

        #region String Input

        public string CurrentLine { get; set; } = string.Empty;

        public string CurrentChar => CurrentLine == string.Empty
            ? string.Empty
            : CurrentLine.ToCharArray()[CurrentLine.Length - 1].ToString();

        public List<string> Typed { get; set; }

        public string PreviousLine { get; private set; } = string.Empty;

        #endregion

        #region OSK

        private readonly List<Keys> _oskCurrentState;
        private readonly List<Keys> _oskPreviousState;

        #endregion

        #region Functions

        public bool TypedKey(Keys k)
        {
            return !_oskCurrentState.Contains(k) && _oskPreviousState.Contains(k);
        }

        public bool Pressed(Keys k)
        {
            return _oskCurrentState.Contains(k);
        }

        public bool Released(Keys k)
        {
            return !Pressed(k);
        }

        public bool Any()
        {
            return Enum.GetValues(typeof(Keys)).Cast<Keys>().Any(TypedKey);
        }

        public IEnumerable<Keys> TypedKeys => Enum.GetValues(typeof(Keys)).Cast<Keys>().Where(TypedKey);

        public bool AnyAlpha()
        {
            return _oskCurrentState.Any(k => _alphaKeys.Contains(k) && !_oskPreviousState.Contains(k));
        }

        public bool AnyNumeric()
        {
            return _oskCurrentState.Any(k => _numericKeys.Contains(k) && !_oskPreviousState.Contains(k));
        }

        public bool AnyAlphaNumeric()
        {
            return AnyNumeric() || AnyAlpha();
        }

        #endregion
    }
}