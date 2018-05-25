using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.ApplicationModel.Activation;

namespace SuperGameSystemBasic.Basic
{
    public class Lexer
    {
        public static char[] Tokens = {' ', ',', '(', ')', '=','"', '+', '-', '>', '<', '{', '}', '*', '/', '%'};
        private readonly string _source;
        private char _lastChar;
        private Marker _sourceMarker;

        public Lexer(string input)
        {
            Lines = new List<string>(input.Split('\n'));
            for (var index = 0; index < Lines.Count; index++)
            {
                var l = Lines[index];
                if (l.TrimStart().StartsWith("REM", StringComparison.CurrentCultureIgnoreCase))
                    Lines[index] = string.Empty;
                else
                    Lines[index] = l.Trim();
            }
            _source = input;
            Reset();
        }

        public void Reset()
        {
            _sourceMarker = new Marker(0, 1, 1);
            _lastChar = _source[0];
        }
        public List<string> Lines;
        public string CurrentLine => Lines[_sourceMarker.Line - 1];
        public Marker TokenMarker;
        public string Identifer;
        public Value Value;
        public void GoTo(Marker marker) => _sourceMarker = marker;


        private char GetChar()

        {
            _sourceMarker.Column++;
            _sourceMarker.Pointer++;
            if (_sourceMarker.Pointer >= _source.Length)
                return _lastChar = (char) 0;
            if ((_lastChar = _source[_sourceMarker.Pointer]) == '\n')
            {
                _sourceMarker.Column = 1;
                _sourceMarker.Line++;
            }
            return _lastChar;
        }

        public Token GetToken()
        {
            while (_lastChar == ' ' || _lastChar == '\r')// || _lastChar == '\t')
                GetChar();
            TokenMarker = _sourceMarker;
            if (char.IsLetter(_lastChar) || _lastChar == '@')
            {
                Identifer = _lastChar.ToString();
                char c;
                while (char.IsLetterOrDigit(c = GetChar()) || c == '@')
                    Identifer += _lastChar;
                switch (Identifer.ToUpper())
                {
                    case "BLIT":
                        return Token.Blit;
                    case "IF":
                        return Token.If;
                    case "ELSEIF":
                        return Token.ElseIf;
                    case "ENDIF":
                        return Token.EndIf;
                    case "THEN":
                        return Token.Then;
                    case "ELSE":
                        return Token.Else;
                    case "FOR":
                        return Token.For;
                    case "TO":
                        return Token.To;
                    case "STEP":
                        return Token.Step;
                    case "NEXT":
                        return Token.Next;
                    case "LET":
                        return Token.Let;
                    case "VAR":
                        return Token.Var;
                    case "DIM":
                        return Token.Dim;
                    case "ARRAY":
                        return Token.Array;
                    case "WHILE":
                        return Token.While;
                    case "WEND":
                        return Token.Wend;
                    case "RETURN":
                        return Token.Return;
                    case "EVAL":
                        return Token.Eval;
                    case "END":
                        return Token.End;
                    case "OR":
                        return Token.Or;
                    case "AND":
                        return Token.And;
                    case "NOT":
                        return Token.Not;
                    case "FUNCTION":
                        return Token.Function;
                    case "ENDFUNCTION":
                        return Token.EndFunction;
                    case "OBJ":
                        return Token.Obj;
                    case "GOSUB":
                        return Token.Gosub;
                    case "ECHO":
                        return Token.Echo;
                    case "READTO":
                        return Token.ReadTo;
                    case "READLINETO":
                        return Token.ReadLineTo;
                    case "REM":
                        while (_lastChar != '\n') GetChar();
                        GetChar();
                        return GetToken();
                    default:
                        return Token.Identifer;
                }
            }

            if (char.IsDigit(_lastChar))
            {
                var num = string.Empty;
                do
                {
                    num += _lastChar;
                } while (char.IsDigit(GetChar()) || _lastChar == '.');
                if (
                    !double.TryParse(num, NumberStyles.Float,
                        CultureInfo.InvariantCulture, out var real))
                    throw new Exception("ERROR while parsing number");
                Value = new Value(real);
                return Token.Value;
            }
            var tok = Token.Unknown;
            switch (_lastChar)
            {
                case '\n':
                    tok = Token.NewLine;
                    break;
                case ':':
                    tok = Token.Colon;
                    break;
                case ';':
                    tok = Token.Semicolon;
                    break;
                case ',':
                    tok = Token.Comma;
                    break;
                case '=':
                    tok = Token.Equal;
                    break;
                case '+':
                    tok = Token.Plus;
                    break;
                case '-':
                    tok = Token.Minus;
                    break;
                case '/':
                    tok = Token.Slash;
                    break;
                case '*':
                    tok = Token.Asterisk;
                    break;
                case '%':
                    tok = Token.Mod;
                    break;
                case '^':
                    tok = Token.Caret;
                    break;
                case '(':
                    tok = Token.LParen;
                    break;
                case ')':
                    tok = Token.RParen;
                    break;
                case '[':
                    tok = Token.LBracket;
                    break;
                case ']':
                    tok = Token.RBracket;
                    break;
                case '{':
                    tok = Token.LBrace;
                    break;
                case '}':
                    tok = Token.RBrace;
                    break;
                case '\'':
                    while (_lastChar != '\n') GetChar();
                    GetChar();
                    return GetToken();
                case '<':
                    GetChar();
                    if (_lastChar == '>') tok = Token.NotEqual;
                    else if (_lastChar == '=') tok = Token.LessEqual;
                    else return Token.Less;
                    break;
                case '>':
                    GetChar();
                    if (_lastChar == '=') tok = Token.MoreEqual;
                    else return Token.More;
                    break;
                case '"':
                    var str = string.Empty;
                    while (GetChar() != '"')
                        if (_lastChar == '\\')
                            switch (char.ToLower(GetChar()))
                            {
                                case 'n':
                                    str += '\n';
                                    break;

                                case 't':
                                    str += '\t';
                                    break;

                                case '\\':
                                    str += '\\';
                                    break;

                                case '"':
                                    str += '"';
                                    break;
                            }

                        else

                            str += _lastChar;
                    Value = new Value(str);
                    tok = Token.Value;
                    break;
                case (char) 0:
                    return Token.EOF;
            }
            GetChar();
            return tok;
        }
    }
}