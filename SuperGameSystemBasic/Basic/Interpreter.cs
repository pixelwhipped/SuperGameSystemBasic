using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace SuperGameSystemBasic.Basic
{
    public class Interpreter
    {
        public delegate Value BasicFunction(Interpreter interpreter, List<Value> args);

        private readonly Dictionary<string, BasicFunction> _funcs = new Dictionary<string, BasicFunction>();

        private readonly int _ifCounter = 0;

        private readonly Lexer _lexer;

        private readonly Dictionary<string, Marker> _loops = new Dictionary<string, Marker>();
        private readonly Dictionary<string, Value> _vars = new Dictionary<string, Value>();
        public readonly Dictionary<int, StopWatchWithOffset> Timers = new Dictionary<int, StopWatchWithOffset>();
        private readonly Dictionary<string, FunctionStack> _ufunc = new Dictionary<string, FunctionStack>();

        private Token _lastToken;

        private Marker _lineMarker;
        private Token _prevToken;

        private bool _started;
        public Queue<Note> AudioQueue = new Queue<Note>();

        public bool EchoOn = true;

        private Point _startReadLineTo;
        public SoundEffectInstance WaveOut;

        public Interpreter(BasicOne basicOne, string input, Dictionary<string, BasicFunction> functions)
        {
            BasicOne = basicOne;
            basicOne.ErrorLine = -1;
            if (string.IsNullOrEmpty(input))
                NoData = true;
            else if (string.IsNullOrEmpty(input.Trim()))
                NoData = true;
            input = FixSigns(input);
            _lexer = new Lexer(AddSymbolsAndConstants(input));

            BuiltInFunctions.DefaultSignalType = Audio.SignalType.Sine;
            BuiltInFunctions.InstallAll(this);
            foreach (var f in functions ?? new Dictionary<string, BasicFunction>())
                AddFunction(f.Key, f.Value);
            AddFunction("KEYPRESSED", KeyPressed);
            while (_lastToken != Token.EOF)
            {
                GetNextToken();
                if (_lastToken == Token.Function)
                {
                    GetNextToken();
                    Match(Token.Identifer);
                    var var = _lexer.Identifer;
                    if (_ufunc.ContainsKey(var))
                        Error($"Function {var} already defined");
                    else if (_vars.ContainsKey(var))
                        Error($"Function Value Result {var} already defined");
                    else
                    {
                        while (GetNextToken() != Token.NewLine) { }
                        _ufunc.Add(var, new FunctionStack(var, _lexer.TokenMarker));//, this));
                    }
                }
            }
            _lexer.Reset();
        }

        public bool KeyBlocking => ReadToBlocking || ReadLineToBlocking;
        public bool ReadToBlocking;
        public bool ReadLineToBlocking;
        public bool Exit;

        public TimeSpan RunTime = TimeSpan.Zero;
        public BasicOne BasicOne;
        public bool NoData;
        public LineState InterpreterState = LineState.Continue;
        public List<char> BlockingKeys { get; set; }

        private static bool IsOperator(char c) => c == '=' || c == '-' || c == '*' || c == '/' || c == '+' || c == '^';

        private static string FixSigns(string input)
        {
            var sb = new StringBuilder();
            for (var index = 0; index < input.Length; index++)
            {
                var c = input[index];
                if (IsOperator(c))
                {
                    while (input[index + 1] == ' ') index++;
                    if (input[index + 1] == '-' && char.IsDigit(input[index + 2]))
                    {
                        sb.Append(c);
                        index += 2;
                        sb.Append("NEG(");
                        c = input[index];
                        while (char.IsDigit(c) || c == '.')
                        {
                            sb.Append(c);
                            index++;
                            c = input[index];
                        }
                        sb.Append(')');
                        index--;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static IEnumerable<string> SplitAndKeep(string s, char[] delims)
        {
            int start = 0, index;

            while ((index = s.IndexOfAny(delims, start)) != -1)
            {
                if (index - start > 0)
                    yield return s.Substring(start, index - start);
                yield return s.Substring(index, 1);
                start = index + 1;
            }

            if (start < s.Length)
                yield return s.Substring(start);
        }

        private string AddSymbolsAndConstants(string input)
        {
            var sb = new StringBuilder();
            var tokens = SplitAndKeep(input,
                new[] {' ', '\t', '\r', '\n', '!', '=', '+', '-', '*', '/', '(', ')', ',', '<', '>', '[', ']'}).ToArray();
            var whileCount = 0;
            var funcs = new Stack<string>();
            var line = 1;
            for (var index = 0; index < tokens.Length; index++)
            {               
                var t = tokens[index];
                if (t == "\n" || t == "\r") line++;
                var tu = t.ToUpper();
                if (tu == "WHILE")
                {
                    if(funcs.Any())
                        sb.Append($"{t} @WHILE{funcs.Peek()}{whileCount}");
                    else
                        sb.Append($"{t} @WHILE{whileCount}");
                    whileCount++;
                }
                else if (tu == "WEND")
                {
                    whileCount--;
                    if (funcs.Any())
                        sb.Append($"{t} @WHILE{funcs.Peek()}{whileCount}");
                    else
                        sb.Append($"{t} @WHILE{whileCount}");
                    //sb.Append($"{t} @WHILE{whileCount}");
                }
                else if (tu == "FUNCTION")
                {
                    sb.Append(t);//$"{t} @FUNCTION{funcCount}");
                    index++;
                    t = tokens[index];
                    while (t.Equals(' ') || t.Equals('\t') || t.Equals('\n') || t.Equals('\r'))
                    {
                        sb.Append(t);//$"{t} @FUNCTION{funcCount}");
                        index++;
                        t = tokens[index];
                    }
                    sb.Append(t);
                    funcs.Push(t);
                    index++;
                    t = tokens[index];
                    sb.Append(t);
                    funcs.Push(t);
                    index++;
                    var nt = tokens[index];
                    if (nt != "(")
                    {
                        _lineMarker = new Marker(0, line, 0);
                        Error($"Excpected {t}()");
                    }
                    sb.Append(nt);
                    index++;
                    nt = tokens[index];
                    if (nt != ")")
                    {
                        _lineMarker = new Marker(0, line, 0);
                        Error($"Excpected {t}()");
                    }
                    //sb.Append($"{nt}\nREM\n");
                    //funcCount++;
                }
                else if (tu == "ENDFUNCTION")
                {
                    //funcCount--;
                    //sb.Append($"{t} @FUNCTION{funcCount}");
                    sb.Append($"{t} {funcs.Pop()}");
                }
                else if (tu == "TRUE")
                {
                    sb.Append("1");
                }
                else if (tu == "FALSE")
                {
                    sb.Append("0");
                }
                else
                {
                    if (IsContant(t, out var constant))
                    {
                        sb.Append(constant);
                    }
                    else
                    {
                        if (GlyphHelpers.GlyphContants().ContainsKey(t))
                            sb.Append(GlyphHelpers.GlyphContants()[t]);
                        else
                            sb.Append(t);
                    }
                }
            }
            sb.Append("\n");
            sb.Append("\n");
            return sb.ToString();
        }

        private static bool IsContant(string s, out string constant)
        {
            constant = null;
            if (string.IsNullOrEmpty(s) || !s[0].Equals('@')) return false;
            if (s == "@HORIZONTAL")
                constant = "1";
            else if (s == "@VERTICAL")
                constant = "2";
            else if (s == "@NONE")
                constant = "0";
            else if (s == "@PI")
                constant = "3.1415926535897931";
            else if (s == "@BLACK")
                constant = "0";
            else if (s == "@BLUE")
                constant = "1";
            else if (s == "@GREEN")
                constant = "2";
            else if (s == "@CYAN")
                constant = "3";
            else if (s == "@RED")
                constant = "4";
            else if (s == "@MAGENTA")
                constant = "5";
            else if (s == "@BROWN")
                constant = "6";
            else if (s == "@GRAY")
                constant = "7";
            else if (s == "@DARK_GRAY")
                constant = "8";
            else if (s == "@LIGHT_BLUE")
                constant = "9";
            else if (s == "@LIGHT_GREEN")
                constant = "10";
            else if (s == "@LIGHT_CYAN")
                constant = "11";
            else if (s == "@LIGHT_RED")
                constant = "12";
            else if (s == "@LIGHT_MAGENTA")
                constant = "13";
            else if (s == "@YELLOW")
                constant = "14";
            else if (s == "@WHITE")
                constant = "15";
            else if (s == "@SINE")
                constant = "0";
            else if (s == "@SAWTOOTH")
                constant = "1";
            else if (s == "@TRIANGLE")
                constant = "2";
            else if (s == "@SQUARE")
                constant = "3";
            else if (s == "@NOISE")
                constant = "4";
            return constant != null;
        }

        public Value GetVar(string name)
        {
            if (!_vars.ContainsKey(name))
                Error("Variable with name " + name + " does not exist.");
            return _vars[name];
        }

        public void SetVar(string name, Value val)
        {
            if (!_vars.ContainsKey(name)) _vars.Add(name, val);
            else _vars[name] = val;
        }

        public void AddFunction(string name, BasicFunction function)
        {
            if (!_funcs.ContainsKey(name)) _funcs.Add(name, function);
            else _funcs[name] = function;
        }

        public void Error(string text)
        {
            Exit = true;
            InterpreterState = LineState.Blit;
            BasicOne.ErrorLine = _lineMarker.Line;
            throw new Exception(text + " at line: " + _lineMarker.Line + "\n" + _lexer.CurrentLine);
        }

        private void Match(Token tok)
        {
            if (_lastToken != tok)
                Error("Expect " + tok + " got " + _lastToken);
        }

        public Value KeyPressed(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                return new Value(Keyboard.GetState().GetPressedKeys().Any() ? 1 : 0);
            return args[0].Type == ValueType.Real ? new Value(GlyphHelpers.IsKeyPressed((int) args[0].Real,this) ? 1 : 0) : Value.Zero;
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Pause))
            {
                Exit = true;
                WaveOut?.Stop();
                WaveOut = null;
                InterpreterState = LineState.Blit;
            }
            else
            {
                if (NoData)
                {
                    BasicOne.ErrorLine = 1;
                    throw new Exception("Expected end at line: 1");
                }
                if (!_started)
                {
                    InterpreterState = LineState.Blit;
                    _started = true;
                    Exit = false;
                    RunTime = TimeSpan.Zero;
                    GetNextToken();
                    return;
                }
                if (!Exit)
                {

                    InterpreterState = LineState.Continue;
                    while (InterpreterState == LineState.Continue)
                    {
                        UpdateJoyPad();
                        RunTime += gameTime.ElapsedGameTime;
                        if (KeyBlocking)
                        {
                            if (ReadLineToBlocking)
                            {
                                InterpreterState = LineState.Blit;
                                ReadLineTo();
                            }
                            else if (ReadToBlocking)
                            {
                                InterpreterState = LineState.Blit;
                                ReadTo();
                            }
                        }
                        else
                        {
                            Line();
                        }
                        if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Pause))
                        {
                            Exit = true;
                            InterpreterState = LineState.Yeild;
                        }

                        if (WaveOut == null || WaveOut.State != SoundState.Stopped) return;
                        WaveOut = null;
                        if (!AudioQueue.Any()) return;
                        var que = AudioQueue.Dequeue();
                        BuiltInFunctions.Sound(this,
                            new List<Value> { new Value(que.Frequency), new Value(que.Amplitude), new Value(que.Duration), new Value(BuiltInFunctions.GetSignalType(que.SignalType)) });
                    }
                }
                else
                {
                    InterpreterState = LineState.Blit;
                    WaveOut?.Stop();
                    WaveOut = null;
                }
            }
        }

        public bool PadAPressed;
        public bool PadBPressed;
        public bool PadUpPressed;
        public bool PadDownPressed;
        public bool PadLeftPressed;
        public bool PadRightPressed;
        public bool PadStartPressed;
        public bool PadBackPressed;
        private void UpdateJoyPad()
        {
            PadLeftPressed = false;
            PadRightPressed = false;
            PadUpPressed = false;
            PadDownPressed = false;
            PadAPressed = false;
            PadBPressed = false; 
            PadStartPressed = false;
            PadBackPressed = false;
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);            
            if (!capabilities.IsConnected) capabilities = GamePad.GetCapabilities(PlayerIndex.Two);
            if (!capabilities.IsConnected) capabilities = GamePad.GetCapabilities(PlayerIndex.Three);
            if (!capabilities.IsConnected) capabilities = GamePad.GetCapabilities(PlayerIndex.Four);
            if (capabilities.IsConnected)
            {
                // Get the current state of Controller1
                GamePadState state = GamePad.GetState(PlayerIndex.One);
                if (capabilities.HasLeftXThumbStick && capabilities.HasLeftYThumbStick)
                {
                    if (state.ThumbSticks.Left.X < -0.5f)
                        PadLeftPressed = true;
                    if (state.ThumbSticks.Left.X > 0.5f)
                        PadRightPressed = true;
                    if (state.ThumbSticks.Left.Y < -0.5f)
                        PadUpPressed = true;
                    if (state.ThumbSticks.Left.Y > 0.5f)
                        PadDownPressed = true;
                }
                PadLeftPressed = PadLeftPressed ||
                                 (capabilities.HasDPadLeftButton && state.IsButtonDown(Buttons.DPadLeft));
                PadRightPressed = PadRightPressed ||
                                  (capabilities.HasDPadRightButton && state.IsButtonDown(Buttons.DPadRight));
                PadUpPressed = PadUpPressed || (capabilities.HasDPadUpButton && state.IsButtonDown(Buttons.DPadUp));
                PadDownPressed = PadDownPressed ||
                                 (capabilities.HasDPadDownButton && state.IsButtonDown(Buttons.DPadDown));
                PadAPressed = capabilities.HasAButton && state.IsButtonDown(Buttons.A);
                PadBPressed = capabilities.HasBButton && state.IsButtonDown(Buttons.B);
                PadStartPressed = capabilities.HasStartButton && state.IsButtonDown(Buttons.Start);
                PadBackPressed = capabilities.HasBackButton && state.IsButtonDown(Buttons.Back);
            }
        }

        private Token GetNextToken()
        {
            _prevToken = _lastToken;
            _lastToken = _lexer.GetToken();
            if (_lastToken == Token.EOF && _prevToken == Token.EOF)
                Error("Unexpected end of file");
            return _lastToken;
        }

        private void Line()
        {
            while (_lastToken == Token.NewLine) GetNextToken();
            if (_lastToken == Token.EOF)
            {
                Exit = true;
                InterpreterState = LineState.Blit;
            }
            _lineMarker = _lexer.TokenMarker;
            Statment();
            if (_lastToken != Token.NewLine && _lastToken != Token.EOF && !KeyBlocking)
                Error("Expect new line got " + _lastToken);
        }

        public static bool IsSyntax(string syn)
        {            
            switch (syn)
            {
                case "":
                    return false;
                case "REM":
                    return true;
                case "OR":
                    return true;
                case "AND":
                    return true;
                case "NOT":
                    return true;
                case "EVAL":
                    return true;
                case "IF":
                    return true;
                case "ELSEIF":
                    return true;
                case "ENDIF":
                    return true;
                case "THEN":
                    return true;
                case "ELSE":
                    return true;
                case "FOR":
                    return true;
                case "TO":
                    return true;
                case "BLIT":
                    return true;
                case "WHILE":
                    return true;
                case "WEND":
                    return true;
                case "OBJ":
                    return true;
                case "STEP":
                    return true;
                case "NEXT":
                    return true;
                case "LET":
                    return true;
                case "VAR":
                    return true;
                case "ARRAY":
                    return true;
                case "DIM":
                    return true;
                case "GOSUB":
                    return true;
                case "RETURN":
                    return true;
                case "FUNCTION":
                    return true;
                case "ENDFUNCTION":
                    return true;
                case "END":
                    return true;
                case "ECHO":
                    return true;
                case "READTO":
                    return true;
                case "READLINETO":
                    return true;
                case "TRUE":
                    return true;
                case "FALSE":
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsFunction(string func)
        {         
            switch (func)
            {                
                case "": return false;
                case "PRINT": return true;
                case "PRINTG": return true;
                case "CLEAR": return true;
                case "CURSORX": return true;
                case "CURSORXY": return true;
                case "CURSORY": return true;
                case "ABS": return true;
                case "COS": return true;
                case "KEYPRESSED": return true;
                case "MAX": return true;
                case "MIN": return true;
                case "NEG": return true;
                case "RAND": return true;
                case "ROUND": return true;
                case "ROUNDDOWN": return true;
                case "ROUNDUP": return true;
                case "SIGN": return true;
                case "SIN": return true;
                case "SQRT": return true;
                case "TAN": return true;
                case "FILL": return true;
                case "GETCOLUMNS": return true;
                case "GETROWS": return true;
                case "GLYPHGET": return true;
                case "GLYPHSET": return true;
                case "RGBDACGET": return true;
                case "RGBDACSET": return true;
                case "RGBDACRESET": return true;
                case "SOUND": return true;
                case "SOUNDQUE": return true;
                case "SOUNDQUECLEAR": return true;
                case "SOUNDSIGNAL": return true;
                case "TIMER": return true;
                case "TODIM": return true;
                case "TONUM": return true;
                case "TOSTR": return true;
                case "YEILD": return true;
                case "LEN": return true;
                case "TOLOWER": return true;
                case "TOUPPER": return true;
                case "TRIM": return true;
                case "ISNAN": return true;
                case "ISNUM": return true;
                case "ISDIM": return true;
                case "ISSTR": return true;
                case "TRIMSTART": return true;
                case "TRIMEND": return true;
                case "LEFT": return true;
                case "RIGHT": return true;
                case "REPEAT": return true;
                case "SUBSTRING": return true;
                case "INSTR": return true;
                case "REPLACE": return true;
                case "COUNT": return true;
                case "UNIQUE": return true;
                case "UNION": return true;
                case "CONCAT": return true;
                case "ASCENDING": return true;
                case "DESCENDING": return true;
                case "REVERSE": return true;
                case "SHUFFLE": return true;
                case "SETMODE": return true;
                case "SUM": return true;
                case "AVG": return true;
                case "LERP": return true;
                case "LINE": return true;
                case "RECT": return true;
                case "ELIPSE": return true;
                default:
                    return false;
            }
        }

        private void Statment()
        {           
            var keyword = _lastToken;
            GetNextToken();
            switch (keyword)
            {
                case Token.Blit:
                    Blit();
                    break;
                case Token.Echo:
                    Echo();
                    break;
                case Token.If:
                    If();
                    break;
                case Token.Else:
                    Else();
                    break;
                case Token.ElseIf:
                    Else();
                    break;
                case Token.EndIf:
                    break;
                case Token.For:
                    For();
                    BasicOne.RequiredYeildCounter++;
                    break;
                case Token.Next:
                    Next();
                    if (BasicOne.RequiredYeildCounter > 2034)
                        BasicOne.Yeild(this, new List<Value> {Value.Zero});
                    break;
                case Token.While:
                    While();
                    BasicOne.RequiredYeildCounter++;
                    break;
                case Token.Wend:
                    Wend();
                    if (BasicOne.RequiredYeildCounter > 2034)
                        BasicOne.Yeild(this, new List<Value> {Value.Zero});
                    break;
                case Token.Let:
                    Let();
                    break;
                case Token.Var:
                    Let();
                    break;
                case Token.Dim:
                    Dim();
                    break;
                case Token.Array:
                    Dim();
                    break;
                case Token.End:
                    End();
                    break;
                case Token.ReadTo:
                    ReadTo();
                    return;
                case Token.Function:
                    Function();
                    return;
                case Token.EndFunction:
                    EndFunction();
                    return;
                case Token.ReadLineTo:
                    ReadLineTo();
                    return;
                case Token.Identifer:
                    if (_lastToken == Token.Equal || _lastToken == Token.LBracket) Let();
                    else goto default;
                    break;
               /* case Token.Rem:
                    while (GetNextToken() != Token.NewLine) { }
                    return;*/
                case Token.EOF:
                    Exit = true;
                    InterpreterState = LineState.Blit;
                    break;
                default:
                    if (IsSyntax(Enum.GetName(typeof(Token), keyword).ToUpper()))
                        Error("Found reserved word " + keyword);
                    if (_funcs.Any(p => p.Key == _lexer.Identifer))
                        Expr();
                    else if (_ufunc.Any(p => p.Key == _lexer.Identifer))
                    {
                        var func = _ufunc[_lexer.Identifer];
                        EnterFunc(func);
                    }
                    else
                        Error("Expect keyword got " + keyword);
                    break;
            }
            if (_lastToken != Token.Colon) return;
            GetNextToken();
            Statment();
        }

        private void Echo()
        {
            var result = Math.Abs(Expr().BinOp(Value.Zero, Token.Equal).Real - 1) > double.Epsilon;
            EchoOn = result;
        }

        private void ReadLineTo()
        {
            if (!ReadLineToBlocking)
            {
                ReadLineToBlocking = true;
                _startReadLineTo = new Point(BasicOne.Terminal.X, BasicOne.Terminal.Y);
                var id = _lexer.Identifer;
                if (_vars.ContainsKey(id))
                    _vars[id] = new Value(string.Empty);
                return;
            }
            if (BlockingKeys.Any())
                foreach (var k in BlockingKeys)
                    if (k == '\n')
                    {
                        GetNextToken();
                        ReadLineToBlocking = false;
                        var id = _lexer.Identifer;
                        if (!_vars.ContainsKey(id)) continue;
                        BasicOne.Terminal.X = _startReadLineTo.X + _vars[id].String.Length;
                        BasicOne.Terminal.Y = _startReadLineTo.Y;
                    }
                    else if (k == '\r')
                    {
                        var id = _lexer.Identifer;
                        if (!_vars.ContainsKey(id)) continue;
                        if (_vars[id].String == string.Empty) continue;
                        SetVar(id, new Value(_vars[id].String.Substring(0, _vars[id].String.Length - 1)));
                        if (!EchoOn) return;
                        BasicOne.Terminal.X = _startReadLineTo.X;
                        BasicOne.Terminal.Y = _startReadLineTo.Y;
                        BasicOne.Terminal.PrintSpecial(_vars[id].String + " ");
                        return;
                    }
                    else
                    {
                        var id = _lexer.Identifer;
                        SetVar(id,_vars.ContainsKey(id) ? new Value(_vars[id] + k.ToString()) : new Value(k.ToString()));
                        if (!EchoOn) continue;
                        BasicOne.Terminal.X = _startReadLineTo.X;
                        BasicOne.Terminal.Y = _startReadLineTo.Y;
                        BasicOne.Terminal.PrintSpecial(_vars[id].String);
                    }
        }

        private void ReadTo()
        {
            if (!ReadToBlocking)
            {
                ReadToBlocking = true;
                return;
            }
            if (!BlockingKeys.Any()) return;
            var id = _lexer.Identifer;
            SetVar(id, new Value(BlockingKeys[0].ToString()));
            if (EchoOn)
                BasicOne.Terminal.PrintSpecial(_vars[id].String);
            GetNextToken();
            ReadToBlocking = false;
        }

        private void Blit() => InterpreterState = LineState.Blit;

        private void If()
        {                 
            var result = Math.Abs(Expr().BinOp(Value.Zero, Token.Equal).Real - 1) < double.Epsilon;            
            Match(Token.Then);
            GetNextToken();
            if (!result) return;
            var i = _ifCounter;
            while (true) //Add check that else is last
            {
                if (_lastToken == Token.If)
                {
                    i++;
                }
                else if (_lastToken == Token.ElseIf && i == _ifCounter)
                {
                    GetNextToken();
                    If();
                    return;
                }
                else if (_lastToken == Token.Else)
                {
                    if (i == _ifCounter)
                    {
                        GetNextToken();
                        return;
                    }
                }
                else if (_lastToken == Token.EndIf)
                {
                    if (i == _ifCounter)
                    {
                        GetNextToken();
                        return;
                    }
                    i--;
                }
                GetNextToken();
            }
        }

        private void Else()
        {
            var i = _ifCounter;
            while (true)
            {
                if (_lastToken == Token.If)
                {
                    i++;
                }
                else if (_lastToken == Token.EndIf)
                {
                    if (i == _ifCounter)
                    {
                        GetNextToken();
                        return;
                    }
                    i--;
                }
                GetNextToken();
            }
        }


        private void End() => Exit = true;

        private void Let()
        {
            if (_lastToken == Token.LBracket)
            {
                var id = _lexer.Identifer;
                GetNextToken();
                var index = Expr();
                GetNextToken();
                GetNextToken();
                var r = Expr();
                var a = _vars[id].Array;
                var i = (int) index.Real;
                if (i >= 0 && i < a.Length)
                {
                    a[i] = r.Real;
                    SetVar(id, new Value(a));
                }
                else
                {
                    Error("Array out of bounds");
                }
            }
            else
            {
                if (_lastToken != Token.Equal)
                {
                    Match(Token.Identifer);
                    GetNextToken();
                    Match(Token.Equal);
                }
                var id = _lexer.Identifer;
                GetNextToken();
                SetVar(id, Expr());
            }
        }

        private void Dim()
        {
            if (_lastToken != Token.Equal)
            {
                Match(Token.Identifer);
                GetNextToken();
                Match(Token.Equal);
            }
            var id = _lexer.Identifer;
            GetNextToken();
            if (_lastToken == Token.LBrace)
            {
                SetVar(id, ReadDim());
            }
            else if (_lastToken == Token.Identifer && _vars.ContainsKey(_lexer.Identifer))
            {
                var var = _vars[_lexer.Identifer];
                switch (var.Type)
                {
                    case ValueType.Array:
                        var nArray = new double[var.Array.Length];
                        Array.Copy(var.Array, nArray, var.Array.Length);
                        SetVar(id, new Value(nArray));
                        GetNextToken();
                        break;
                    case ValueType.Real:
                        SetVar(id, new Value(new double[(int) var.Real]));
                        GetNextToken();
                        break;
                    case ValueType.String:
                        SetVar(id, new Value(new double[(int) var.Convert(ValueType.Real).Real]));
                        GetNextToken();
                        break;
                    default:
                        Error("Expected array got " + var.Type);
                        break;
                }
            }
            else
            {
                var len = Expr();
                switch (len.Type)
                {
                    case ValueType.String:
                        var sArray = len.String.ToCharArray();
                        var nArray = new double[sArray.Length];
                        for (var i = 0; i < sArray.Length; i++) nArray[i] = sArray[i] - 32;
                        SetVar(id, new Value(nArray));
                        break;
                    case ValueType.Array:
                        var a = new double[len.Array.Length];
                        Array.Copy(len.Array, a, len.Array.Length);
                        SetVar(id, new Value(a));
                        break;
                    default:
                        if (len.Real < 0)
                        {
                            Error("Array must be greater than 0");
                        }
                        else
                        {
                            SetVar(id, new Value(new double[(int) len.Real]));
                        }
                        break;
                }
            }
        }

        private Value ReadDim()
        {
            var array = new List<double>();
            GetNextToken();
            if (_lastToken == Token.RBrace) Error("Empty array initializer");
            while (_lastToken != Token.RBrace)
            {
                if (_lastToken == Token.Comma)
                {
                    GetNextToken();
                    continue;
                }
                var e = Expr();
                if (e.Type != ValueType.Real) Error("Expected real value");
                array.Add(e.Real);
            }
            GetNextToken();
            return new Value(array.ToArray());
        }

        private void EndFunction()
        {
            Match(Token.Identifer);
            var var = _lexer.Identifer;            
            _lexer.GoTo(_ufunc[var].Stack.Pop());
            while (GetNextToken() != Token.NewLine) {
                if (_lastToken == Token.EOF)
                {
                    Exit = true;
                    InterpreterState = LineState.Blit;
                }
            }
            _lastToken = Token.NewLine;
        }

        private void EnterFunc(FunctionStack func)
        {
            // if (BasicOne.RequiredYeildCounter > 2034)
            if (func.Stack.Count > 2)
            {                
                BasicOne.Yeild(this, new List<Value> { Value.Zero });
            }
            func.Stack.Push(_lexer.TokenMarker);
            _lexer.GoTo(func.Marker);
            while (GetNextToken() != Token.RParen)
            {
                if (_lastToken == Token.EOF)
                {
                    Exit = true;
                    InterpreterState = LineState.Blit;
                }
            }
            _lastToken = Token.NewLine;
            _lineMarker = _lexer.TokenMarker;            
        }
        private void Function()
        {
            Match(Token.Identifer);
            var var = _lexer.Identifer;
            GetNextToken();
            while (true)
            {
                while (!(GetNextToken() == Token.Identifer && _prevToken == Token.EndFunction))
                {
                }
                if (_lexer.Identifer != var) continue;                
                GetNextToken();
                Match(Token.NewLine);
                break;
            }
        }

        private void While()
        {
            Match(Token.Identifer);
            var var = _lexer.Identifer;
            GetNextToken();
            var v = Expr();
            if (Math.Abs(v.Real - 1) < double.Epsilon)
                if (_loops.ContainsKey(var))
                    _loops[var] = _lineMarker;
                else
                    _loops.Add(var, _lineMarker);
            else
                while (true)
                {
                    while (!(GetNextToken() == Token.Identifer && _prevToken == Token.Wend))
                    {
                    }
                    if (_lexer.Identifer != var) continue;
                    _loops.Remove(var);
                    GetNextToken();
                    Match(Token.NewLine);
                    break;
                }
        }

        private void Wend()
        {
            Match(Token.Identifer);
            var var = _lexer.Identifer;
            _lexer.GoTo(new Marker(_loops[var].Pointer - 1, _loops[var].Line, _loops[var].Column - 1));
            _lastToken = Token.NewLine;
        }

        private void For()
        {
            Match(Token.Identifer);
            var var = _lexer.Identifer;
            GetNextToken();
            Match(Token.Equal);
            GetNextToken();
            var v = Expr();
            if (!_vars.ContainsKey(var))
                SetVar(var, v);

            Match(Token.To);
            GetNextToken();
            var v2 = Expr();
            var t = v.Real > v2.Real ? Token.Less : Token.More;
            var step = _lexer.Identifer.Equals("STEP",StringComparison.OrdinalIgnoreCase);
            if (step)
                GetNextToken();
            if (_loops.ContainsKey(var))
            {
                _loops[var] = _lineMarker;
            }
            else
            {
                if (t == Token.Less)
                {
                    v.Modifier = step ? new[] {Modifiers.Decrement, Modifiers.Step} : new[] {Modifiers.Decrement};
                }
                else
                {
                    v.Modifier = step ? new[] {Modifiers.Increment, Modifiers.Step} : new Modifiers[] { };
                }
                SetVar(var, v);
                _loops.Add(var, _lineMarker);
            }
            if (step)
            {
                var s = Expr();
                s.Real = Math.Abs(s.Real);
                SetVar($"@{var}STEP", s);
            }
            if (!(Math.Abs(_vars[var].BinOp(v2, t).Real - 1) < double.Epsilon)) return;
            while (true)
            {
                while (!(GetNextToken() == Token.Identifer && _prevToken == Token.Next))
                {
                }
                if (_lexer.Identifer != var) continue;
                _loops.Remove(var);
                GetNextToken();
                Match(Token.NewLine);
                break;
            }
        }

        private void Next()
        {
            Match(Token.Identifer);
            var var = _lexer.Identifer;
            _vars[var] = _vars[var].BinOp(_vars[var].Modifier == null
                    ? Value.One
                    : _vars[var].Modifier.Any(p => p == Modifiers.Step)
                        ? _vars[$"@{var}STEP"]
                        : Value.One
                ,
                _vars[var].Modifier == null
                    ? Token.Plus
                    : _vars[var].Modifier.Any(p => p == Modifiers.Decrement)
                        ? Token.Minus
                        : Token.Plus);
            _lexer.GoTo(new Marker(_loops[var].Pointer - 1, _loops[var].Line, _loops[var].Column - 1));
            _lastToken = Token.NewLine;
        }

        private static readonly Dictionary<Token, int> _precedents = new Dictionary<Token, int>
        {
            {Token.Or, 0},
            {Token.And, 0},
            {Token.Not, 0},
            {Token.Equal, 1},
            {Token.NotEqual, 1},
            {Token.Less, 1},
            {Token.More, 1},
            {Token.LessEqual, 1},
            {Token.MoreEqual, 1},
            {Token.Plus, 2},
            {Token.Minus, 2},
            {Token.Asterisk, 3},
            {Token.Slash, 3},
            {Token.Mod, 3},
            {Token.Caret, 4}
        };

        private Value Expr()
        {            
            if (_lastToken == Token.Not)
            {
                GetNextToken();
                var expr = Expr();
                return Math.Abs(expr.Real) < double.Epsilon ? Value.One : Value.Zero;
            }
            var stack = new Stack<Value>();
            var operators = new Stack<Token>();

            var i = 0;
            while (true)
            {
                if (_lastToken == Token.Value)
                {
                    stack.Push(_lexer.Value);
                }
                else if (_lastToken == Token.Identifer)
                {
                    if (_vars.ContainsKey(_lexer.Identifer))
                    {
                        if (_vars[_lexer.Identifer].Type != ValueType.Array && _lastToken != Token.LBracket)
                        {
                            stack.Push(_vars[_lexer.Identifer]);
                        }
                        else
                        {
                            GetNextToken();
                            if (_lastToken == Token.LBracket)
                            {
                                var id = _lexer.Identifer;
                                GetNextToken();
                                var index = Expr(); // 

                                if (_vars[id].Type == ValueType.Array)
                                {
                                    if ((index.Real >= 0) && (index.Real < _vars[id].Array.Length))
                                        stack.Push(new Value(_vars[id].Array[(int) index.Real]));
                                    else
                                        Error("Array out of bounds");
                                }
                                else
                                {
                                    stack.Push(new Value((int) _vars[id].Real));
                                }
                            }
                            else
                            {
                                stack.Push(_vars[_lexer.Identifer]);
                                break;
                            }
                        }
                    }
                    else if (_funcs.ContainsKey(_lexer.Identifer) || _lexer.Identifer.Equals("EVAL"))
                    {
                        var name = _lexer.Identifer;
                        _lexer.Identifer = "EVAL";
                        var args = new List<Value>();
                        GetNextToken();
                        Match(Token.LParen);
                        start:
                        if (GetNextToken() != Token.RParen)
                        {
                            args.Add(Expr());
                            if (_lastToken == Token.Comma)
                                goto start;
                        }
                        stack.Push(name.Equals("EVAL") ? args[0] : _funcs[name](this, args));
                    }
                    else
                    {
                        Error("Unknown variable " + _lexer.Identifer);
                    }
                }
                else if (_lastToken == Token.LParen)
                {
                    if (_funcs.ContainsKey(_lexer.Identifer) || _lexer.Identifer.Equals("EVAL"))
                    {
                        var name = _lexer.Identifer;
                        _lexer.Identifer = "EVAL";
                        var args = new List<Value>();
                        startSub:
                        if (GetNextToken() != Token.RParen)
                        {
                            args.Add(Expr());
                            if (_lastToken == Token.Comma)
                                goto startSub;
                        }
                        stack.Push(name.Equals("EVAL") ? args[0] : _funcs[name](this, args));                     
                    }
                    else
                    {                        
                        GetNextToken();
                        stack.Push(Expr());
                        Match(Token.RParen);
                    }
                }
                else if (_lastToken == Token.LBrace)
                {
                    return ReadDim();
                }
                else if (_lastToken >= Token.Plus && _lastToken <= Token.And)
                {
                    if ((_lastToken == Token.Minus || _lastToken == Token.Minus) &&
                        (i == 0 || _prevToken == Token.LParen))
                    {
                        stack.Push(Value.Zero);
                        operators.Push(_lastToken);
                    }
                    else
                    {
                        while (operators.Count > 0 && _precedents[_lastToken] <= _precedents[operators.Peek()])
                            Operation(ref stack, operators.Pop());
                        operators.Push(_lastToken);
                    }
                }
                else
                {
                    if (i == 0){
                        /*if (_ufunc.ContainsKey(_lexer.Identifer))
                        {
                            var func = _ufunc[_lexer.Identifer];
                            EnterFunc(func);
                            stack.Push(Value.Zero);// func.Value);
                        }
                        else*/
                        {
                            Error("Empty expression");
                        }
                    }                    
                    break;
                }
                i++;
                if (_lastToken != Token.Comma) GetNextToken();
            }
            while (operators.Count > 0)
                Operation(ref stack, operators.Pop());
            return stack.Pop();
        }

        

        private static void Operation(ref Stack<Value> stack, Token token)
        {
            var b = stack.Pop();
            var a = stack.Pop();
            var result = a.BinOp(b, token);
            stack.Push(result);
        }
    }
}