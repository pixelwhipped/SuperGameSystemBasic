using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.Basic;

namespace SuperGameSystemBasic
{
    public static class GlyphHelpers
    {
        public static Dictionary<string, int> _glyphKeys;

        public static Dictionary<string, int> GlyphContants()
        {
            if (_glyphKeys != null) return _glyphKeys;
            _glyphKeys = new Dictionary<string, int>
            {
                ["@SPACE"] = 0,
                ["@EXCLAMATIONMARK"] = 1,
                ["@QUOTE"] = 2,
                ["@POUND"] = 3,
                ["@DOLLARSIGN"] = 4,
                ["@PERCENT"] = 5,
                ["@AMPERSAND"] = 6,
                ["@APOSTROPHE"] = 7,
                ["@OPENPARENTHESIS"] = 8,
                ["@CLOSEPARENTHESIS"] = 9,
                ["@ASTERISK"] = 10,
                ["@PLUS"] = 11,
                ["@COMMA"] = 12,
                ["@DASH"] = 13,
                ["@PERIOD"] = 14,
                ["@SLASH"] = 15,
                ["@DIGIT0"] = 16,
                ["@DIGIT1"] = 17,
                ["@DIGIT2"] = 18,
                ["@DIGIT3"] = 19,
                ["@DIGIT4"] = 20,
                ["@DIGIT5"] = 21,
                ["@DIGIT6"] = 22,
                ["@DIGIT7"] = 23,
                ["@DIGIT8"] = 24,
                ["@DIGIT9"] = 25,
                ["@COLON"] = 26,
                ["@SEMICOLON"] = 27,
                ["@LESSTHAN"] = 28,
                ["@EQUALS"] = 29,
                ["@GREATERTHAN"] = 30,
                ["@QUESTIONMARK"] = 31,
                ["@AT"] = 32,
                ["@AUPPER"] = 33,
                ["@BUPPER"] = 34,
                ["@CUPPER"] = 35,
                ["@DUPPER"] = 36,
                ["@EUPPER"] = 37,
                ["@FUPPER"] = 38,
                ["@GUPPER"] = 39,
                ["@HUPPER"] = 40,
                ["@IUPPER"] = 41,
                ["@JUPPER"] = 42,
                ["@KUPPER"] = 43,
                ["@LUPPER"] = 44,
                ["@MUPPER"] = 45,
                ["@NUPPER"] = 46,
                ["@OUPPER"] = 47,
                ["@PUPPER"] = 48,
                ["@QUPPER"] = 49,
                ["@RUPPER"] = 50,
                ["@SUPPER"] = 51,
                ["@TUPPER"] = 52,
                ["@UUPPER"] = 53,
                ["@VUPPER"] = 54,
                ["@WUPPER"] = 55,
                ["@XUPPER"] = 56,
                ["@YUPPER"] = 57,
                ["@ZUPPER"] = 58,
                ["@OPENBRACKET"] = 59,
                ["@BACKSLASH"] = 60,
                ["@CLOSEBRACKET"] = 61,
                ["@CARET"] = 62,
                ["@UNDERSCORE"] = 63,
                ["@ACCENT"] = 64,
                ["@ALOWER"] = 65,
                ["@BLOWER"] = 66,
                ["@CLOWER"] = 67,
                ["@DLOWER"] = 68,
                ["@ELOWER"] = 69,
                ["@FLOWER"] = 70,
                ["@GLOWER"] = 71,
                ["@HLOWER"] = 72,
                ["@ILOWER"] = 73,
                ["@JLOWER"] = 74,
                ["@KLOWER"] = 75,
                ["@LLOWER"] = 76,
                ["@MLOWER"] = 77,
                ["@NLOWER"] = 78,
                ["@OLOWER"] = 79,
                ["@PLOWER"] = 80,
                ["@QLOWER"] = 81,
                ["@RLOWER"] = 82,
                ["@SLOWER"] = 83,
                ["@TLOWER"] = 84,
                ["@ULOWER"] = 85,
                ["@VLOWER"] = 86,
                ["@WLOWER"] = 87,
                ["@XLOWER"] = 88,
                ["@YLOWER"] = 89,
                ["@ZLOWER"] = 90,
                ["@OPENBRACE"] = 91,
                ["@PIPE"] = 92,
                ["@CLOSEBRACE"] = 93,
                ["@TILDE"] = 94,
                ["@BULLET"] = 95,
                ["@BARUPDOWN // │"] = 96,
                ["@BARUPDOWNLEFT // ┤"] = 97,
                ["@BARUPDOWNDOUBLELEFT // ╡"] = 98,
                ["@BARDOUBLEUPDOWNSINGLELEFT // ╢"] = 99,
                ["@BARDOUBLEDOWNSINGLELEFT // ╜"] = 100,
                ["@BARDOWNDOUBLELEFT // ╛"] = 101,
                ["@BARDOUBLEUPDOWNLEFT"] = 102,
                ["@BARDOUBLEUPDOWN"] = 103,
                ["@BARDOUBLEDOWNLEFT"] = 104,
                ["@BARDOUBLEUPLEFT"] = 105,
                ["@BARDOUBLEUPSINGLELEFT"] = 106,
                ["@BARUPDOUBLELEFT"] = 107,
                ["@BARDOWNLEFT"] = 108,
                ["@BARUPRIGHT"] = 109,
                ["@BARUPLEFTRIGHT"] = 110,
                ["@BARDOWNLEFTRIGHT"] = 111,
                ["@BARUPDOWNRIGHT"] = 112,
                ["@BARLEFTRIGHT"] = 113,
                ["@BARUPDOWNLEFTRIGHT"] = 114,
                ["@BARUPDOWNDOUBLERIGHT // ╞"] = 115,
                ["@BARDOUBLEUPDOWNSINGLERIGHT"] = 116,
                ["@BARDOUBLEUPRIGHT"] = 117,
                ["@BARDOUBLEDOWNRIGHT"] = 118,
                ["@BARDOUBLEUPLEFTRIGHT"] = 119,
                ["@BARDOUBLEDOWNLEFTRIGHT"] = 120,
                ["@BARDOUBLEUPDOWNRIGHT"] = 121,
                ["@BARDOUBLELEFTRIGHT"] = 122,
                ["@BARDOUBLEUPDOWNLEFTRIGHT"] = 123,
                ["@BARUPDOUBLELEFTRIGHT"] = 124,
                ["@BARDOUBLEUPSINGLELEFTRIGHT"] = 125,
                ["@BARDOWNDOUBLELEFTRIGHT"] = 126,
                ["@BARDOUBLEDOWNSINGLELEFTRIGHT"] = 127,
                ["@BARDOUBLEUPSINGLERIGHT"] = 128,
                ["@BARUPDOUBLERIGHT"] = 129,
                ["@BARDOWNDOUBLERIGHT"] = 130,
                ["@BARDOUBLEDOWNSINGLERIGHT"] = 131,
                ["@BARDOUBLEUPDOWNSINGLELEFTRIGHT"] = 132,
                ["@BARUPDOWNDOUBLELEFTRIGHT"] = 133,
                ["@BARUPLEFT"] = 134,
                ["@BARDOWNRIGHT"] = 135,
                ["@BARDOWN"] = 136,
                ["@BARLEFT"] = 137,
                ["@BARRIGHT"] = 138,
                ["@BARUP"] = 139,
                ["@BARDOUBLEDOWN"] = 140,
                ["@BARDOUBLELEFT"] = 141,
                ["@BARDOUBLERIGHT"] = 142,
                ["@BARDOUBLEUP"] = 143,
                ["@TRIANGLEUP"] = 144,
                ["@TRIANGLEDOWN"] = 145,
                ["@TRIANGLERIGHT"] = 146,
                ["@TRIANGLELEFT"] = 147,
                ["@ARROWUP"] = 148,
                ["@ARROWDOWN"] = 149,
                ["@ARROWRIGHT"] = 150,
                ["@ARROWLEFT"] = 151,
                ["@UNUSED1"] = 152,
                ["@UNUSED2"] = 153,
                ["@UNUSED3"] = 154,
                ["@UNUSED4"] = 155,
                ["@UNUSED5"] = 156,
                ["@UNUSED6"] = 157,
                ["@UNUSED7"] = 158,
                ["@UNUSED8"] = 159,
                ["@SOLID"] = 160,
                ["@SOLIDFILL"] = 161,
                ["@DARK"] = 162,
                ["@DARKFILL"] = 163,
                ["@GRAY"] = 164,
                ["@GRAYFILL"] = 165,
                ["@LIGHT"] = 166,
                ["@LIGHTFILL"] = 167,
                ["@HORIZONTALBARS"] = 168,
                ["@HORIZONTALBARSFILL"] = 169,
                ["@VERTICALBARS"] = 170,
                ["@VERTICALBARSFILL"] = 171,
                ["@FACE"] = 172,
                ["@MOUNTAINS"] = 173,
                ["@TREECONICAL"] = 174,
                ["@TREEROUND"] = 175,
                ["@HEART"] = 176,
                ["@SHIPUP"] = 177,
                ["@SHIPUPRIGHT"] = 178,
                ["@SHIPRIGHT"] = 179,
                ["@ALIEN1"] = 180,
                ["@ALIEN2"] = 181,
                ["@DOOR"] = 182,
                ["@BOX"] = 183,
                ["@STAR"] = 184,
                ["@BOAT"] = 185,
                ["@NOTUSED2"] = 186,
                ["@NOTUSED3"] = 187,
                ["@NOTUSED4"] = 188,
                ["@NOTUSED5"] = 189,
                ["@NOTUSED6"] = 190,
                ["@CLOSE"] = 191,
                ["@KEYUP"] = 192,
                ["@KEYDOWN"] = 193,
                ["@KEYLEFT"] = 194,
                ["@KEYRIGHT"] = 195,
                ["@KEYENTER"] = 196,
                ["@KEYLEFTSHIFT"] = 197,
                ["@KEYRIGHTSHIFT"] = 198,
                ["@KEYLEFTCONTROL"] = 199,
                ["@KEYRIGHTCONTROL"] = 200,
                ["@KEYESC"] = 201,
                ["@KEYF1"] = 202,
                ["@KEYF2"] = 203,
                ["@KEYF3"] = 204,
                ["@KEYF4"] = 205,
                ["@KEYF5"] = 206,
                ["@KEYF6"] = 207,
                ["@KEYF7"] = 208,
                ["@KEYF8"] = 209,
                ["@KEYF9"] = 210,
                ["@KEYF10"] = 211,
                ["@KEYF11"] = 212,
                ["@KEYF12"] = 213,
                ["@TAB"] = 214,
                ["@ENTER"] = 215,
                ["@BACKSPACE"] = 216,
                ["@NOTUSED08"] = 217,
                ["@NOTUSED09"] = 218,
                ["@NOTUSED10"] = 219,
                ["@NOTUSED11"] = 220,
                ["@NOTUSED12"] = 221,
                ["@NOTUSED13"] = 222,
                ["@NOTUSED14"] = 223
            };
            return _glyphKeys;
        }

        public static bool IsKeyPressed(int i, Interpreter interpreter)
        {
            var state = Keyboard.GetState();
            if (!state.GetPressedKeys().Any()) return false;
            if (i == 0) return state.IsKeyDown(Keys.Space);
            if (state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift))
                switch (i)
                {
                    case 1:
                        return state.IsKeyDown(Keys.D1);
                    case 2:
                        return state.IsKeyDown(Keys.OemQuotes);
                    case 3:
                        return state.IsKeyDown(Keys.D3);
                    case 4:
                        return state.IsKeyDown(Keys.D4);
                    case 5:
                        return state.IsKeyDown(Keys.D5);
                    case 6:
                        return state.IsKeyDown(Keys.D7);
                    case 8:
                        return state.IsKeyDown(Keys.D9);
                    case 9:
                        return state.IsKeyDown(Keys.D0);
                    case 10:
                        return state.IsKeyDown(Keys.D8);
                    case 11:
                        return state.IsKeyDown(Keys.OemPlus);
                    case 27:
                        return state.IsKeyDown(Keys.OemSemicolon);
                    case 28:
                        return state.IsKeyDown(Keys.OemComma);
                    case 30:
                        return state.IsKeyDown(Keys.OemPeriod);
                    case 31:
                        return state.IsKeyDown(Keys.OemQuestion);
                    case 32:
                        return state.IsKeyDown(Keys.D2);
                    case 33:
                        return state.IsKeyDown(Keys.A) || interpreter.PadAPressed;
                    case 34:
                        return state.IsKeyDown(Keys.B) || interpreter.PadBPressed;
                    case 35:
                        return state.IsKeyDown(Keys.C);
                    case 36:
                        return state.IsKeyDown(Keys.D);
                    case 37:
                        return state.IsKeyDown(Keys.E);
                    case 38:
                        return state.IsKeyDown(Keys.F);
                    case 39:
                        return state.IsKeyDown(Keys.G);
                    case 40:
                        return state.IsKeyDown(Keys.H);
                    case 41:
                        return state.IsKeyDown(Keys.I);
                    case 42:
                        return state.IsKeyDown(Keys.J);
                    case 43:
                        return state.IsKeyDown(Keys.K);
                    case 44:
                        return state.IsKeyDown(Keys.L);
                    case 45:
                        return state.IsKeyDown(Keys.M);
                    case 46:
                        return state.IsKeyDown(Keys.N);
                    case 47:
                        return state.IsKeyDown(Keys.O);
                    case 48:
                        return state.IsKeyDown(Keys.P);
                    case 49:
                        return state.IsKeyDown(Keys.Q);
                    case 50:
                        return state.IsKeyDown(Keys.R);
                    case 51:
                        return state.IsKeyDown(Keys.S);
                    case 52:
                        return state.IsKeyDown(Keys.T);
                    case 53:
                        return state.IsKeyDown(Keys.U);
                    case 54:
                        return state.IsKeyDown(Keys.V);
                    case 55:
                        return state.IsKeyDown(Keys.W);
                    case 56:
                        return state.IsKeyDown(Keys.X);
                    case 57:
                        return state.IsKeyDown(Keys.Y);
                    case 58:
                        return state.IsKeyDown(Keys.Z);
                    case 62:
                        return state.IsKeyDown(Keys.D6);
                    case 63:
                        return state.IsKeyDown(Keys.OemMinus);
                    case 91:
                        return state.IsKeyDown(Keys.OemOpenBrackets);
                    case 92:
                        return state.IsKeyDown(Keys.OemPipe);
                    case 93:
                        return state.IsKeyDown(Keys.OemCloseBrackets);
                    case 94:
                        return state.IsKeyDown(Keys.OemTilde);
                    case 197:
                        return state.IsKeyDown(Keys.LeftShift);
                    case 198:
                        return state.IsKeyDown(Keys.RightShift);
                }

            switch (i)
            {
                case 192:
                    return state.IsKeyDown(Keys.Up) || interpreter.PadUpPressed;
                case 193:
                    return state.IsKeyDown(Keys.Down) || interpreter.PadDownPressed;
                case 194:
                    return state.IsKeyDown(Keys.Left) || interpreter.PadLeftPressed;
                case 195:
                    return state.IsKeyDown(Keys.Right) || interpreter.PadRightPressed;
                case 196:
                    return state.IsKeyDown(Keys.Enter) || interpreter.PadStartPressed;
                case 199:
                    return state.IsKeyDown(Keys.LeftControl);
                case 200:
                    return state.IsKeyDown(Keys.RightControl);
                case 201:
                    return state.IsKeyDown(Keys.Escape) || interpreter.PadBackPressed;
                case 202:
                    return state.IsKeyDown(Keys.F1);
                case 203:
                    return state.IsKeyDown(Keys.F2);
                case 204:
                    return state.IsKeyDown(Keys.F3);
                case 205:
                    return state.IsKeyDown(Keys.F4);
                case 206:
                    return state.IsKeyDown(Keys.F5);
                case 207:
                    return state.IsKeyDown(Keys.F6);
                case 208:
                    return state.IsKeyDown(Keys.F6);
                case 209:
                    return state.IsKeyDown(Keys.F8);
                case 210:
                    return state.IsKeyDown(Keys.F9);
                case 211:
                    return state.IsKeyDown(Keys.F10);
                case 212:
                    return state.IsKeyDown(Keys.F11);
                case 213:
                    return state.IsKeyDown(Keys.F12);
                case 214:
                    return state.IsKeyDown(Keys.Tab);
                case 215:
                    return state.IsKeyDown(Keys.Enter);
                case 216:
                    return state.IsKeyDown(Keys.Back);
                case 7:
                    return state.IsKeyDown(Keys.OemQuotes);
                case 10:
                    return state.IsKeyDown(Keys.Multiply);
                case 11:
                    return state.IsKeyDown(Keys.Add);
                case 12:
                    return state.IsKeyDown(Keys.OemComma);
                case 13:
                    return state.IsKeyDown(Keys.Subtract);
                case 14:
                    return state.IsKeyDown(Keys.Decimal) || state.IsKeyDown(Keys.OemPeriod);
                case 15:
                    return state.IsKeyDown(Keys.Divide) || state.IsKeyDown(Keys.OemQuestion);
                case 16:
                    return state.IsKeyDown(Keys.D0) || state.IsKeyDown(Keys.NumPad0);
                case 17:
                    return state.IsKeyDown(Keys.D1) || state.IsKeyDown(Keys.NumPad1);
                case 18:
                    return state.IsKeyDown(Keys.D2) || state.IsKeyDown(Keys.NumPad2);
                case 19:
                    return state.IsKeyDown(Keys.D3) || state.IsKeyDown(Keys.NumPad3);
                case 20:
                    return state.IsKeyDown(Keys.D4) || state.IsKeyDown(Keys.NumPad4);
                case 21:
                    return state.IsKeyDown(Keys.D5) || state.IsKeyDown(Keys.NumPad5);
                case 22:
                    return state.IsKeyDown(Keys.D6) || state.IsKeyDown(Keys.NumPad6);
                case 23:
                    return state.IsKeyDown(Keys.D7) || state.IsKeyDown(Keys.NumPad7);
                case 24:
                    return state.IsKeyDown(Keys.D8) || state.IsKeyDown(Keys.NumPad8);
                case 25:
                    return state.IsKeyDown(Keys.D9) || state.IsKeyDown(Keys.NumPad9);
                case 26:
                    return state.IsKeyDown(Keys.OemSemicolon);
                case 29:
                    return state.IsKeyDown(Keys.OemPlus);
                case 59:
                    return state.IsKeyDown(Keys.OemOpenBrackets);
                case 60:
                    return state.IsKeyDown(Keys.OemBackslash);
                case 61:
                    return state.IsKeyDown(Keys.OemCloseBrackets);
                case 64:
                    return state.IsKeyDown(Keys.OemTilde);
                case 65:
                    return state.IsKeyDown(Keys.A);
                case 66:
                    return state.IsKeyDown(Keys.B);
                case 67:
                    return state.IsKeyDown(Keys.C);
                case 68:
                    return state.IsKeyDown(Keys.D);
                case 69:
                    return state.IsKeyDown(Keys.E);
                case 70:
                    return state.IsKeyDown(Keys.F);
                case 71:
                    return state.IsKeyDown(Keys.G);
                case 72:
                    return state.IsKeyDown(Keys.H);
                case 73:
                    return state.IsKeyDown(Keys.I);
                case 74:
                    return state.IsKeyDown(Keys.J);
                case 75:
                    return state.IsKeyDown(Keys.K);
                case 76:
                    return state.IsKeyDown(Keys.L);
                case 77:
                    return state.IsKeyDown(Keys.M);
                case 78:
                    return state.IsKeyDown(Keys.N);
                case 79:
                    return state.IsKeyDown(Keys.O);
                case 80:
                    return state.IsKeyDown(Keys.P);
                case 81:
                    return state.IsKeyDown(Keys.Q);
                case 82:
                    return state.IsKeyDown(Keys.R);
                case 83:
                    return state.IsKeyDown(Keys.S);
                case 84:
                    return state.IsKeyDown(Keys.T);
                case 85:
                    return state.IsKeyDown(Keys.U);
                case 86:
                    return state.IsKeyDown(Keys.V);
                case 87:
                    return state.IsKeyDown(Keys.W);
                case 88:
                    return state.IsKeyDown(Keys.X);
                case 89:
                    return state.IsKeyDown(Keys.Y);
                case 90:
                    return state.IsKeyDown(Keys.Z);
            }
            return false;
        }
    }

    public enum Glyph
    {
        Space,
        ExclamationMark,
        Quote,
        Pound,
        DollarSign,
        Percent,
        Ampersand,
        Apostrophe,
        OpenParenthesis,
        CloseParenthesis,
        Asterisk,
        Plus,
        Comma,
        Dash,
        Period,
        Slash,
        Digit0,
        Digit1,
        Digit2,
        Digit3,
        Digit4,
        Digit5,
        Digit6,
        Digit7,
        Digit8,
        Digit9,
        Colon,
        Semicolon,
        LessThan,
        Equals,
        GreaterThan,
        QuestionMark,
        At,
        AUpper,
        BUpper,
        CUpper,
        DUpper,
        EUpper,
        FUpper,
        GUpper,
        HUpper,
        IUpper,
        JUpper,
        KUpper,
        LUpper,
        MUpper,
        NUpper,
        OUpper,
        PUpper,
        QUpper,
        RUpper,
        SUpper,
        TUpper,
        UUpper,
        VUpper,
        WUpper,
        XUpper,
        YUpper,
        ZUpper,
        OpenBracket,
        Backslash,
        CloseBracket,
        Caret,
        Underscore,
        Accent,
        ALower,
        BLower,
        CLower,
        DLower,
        ELower,
        FLower,
        GLower,
        HLower,
        ILower,
        JLower,
        KLower,
        LLower,
        MLower,
        NLower,
        OLower,
        PLower,
        QLower,
        RLower,
        SLower,
        TLower,
        ULower,
        VLower,
        WLower,
        XLower,
        YLower,
        ZLower,
        OpenBrace,
        Pipe,
        CloseBrace,
        Tilde,
        Bullet,
        BarUpDown, // │
        BarUpDownLeft, // ┤
        BarUpDownDoubleLeft, // ╡
        BarDoubleUpDownSingleLeft, // ╢
        BarDoubleDownSingleLeft, // ╜
        BarDownDoubleLeft, // ╛    
        BarDoubleUpDownLeft,
        BarDoubleUpDown,
        BarDoubleDownLeft,
        BarDoubleUpLeft,
        BarDoubleUpSingleLeft,
        BarUpDoubleLeft,
        BarDownLeft,
        BarUpRight,
        BarUpLeftRight,
        BarDownLeftRight,
        BarUpDownRight,
        BarLeftRight,
        BarUpDownLeftRight,
        BarUpDownDoubleRight, // ╞
        BarDoubleUpDownSingleRight,
        BarDoubleUpRight,
        BarDoubleDownRight,
        BarDoubleUpLeftRight,
        BarDoubleDownLeftRight,
        BarDoubleUpDownRight,
        BarDoubleLeftRight,
        BarDoubleUpDownLeftRight,
        BarUpDoubleLeftRight,
        BarDoubleUpSingleLeftRight,
        BarDownDoubleLeftRight,
        BarDoubleDownSingleLeftRight,
        BarDoubleUpSingleRight,
        BarUpDoubleRight,
        BarDownDoubleRight,
        BarDoubleDownSingleRight,
        BarDoubleUpDownSingleLeftRight,
        BarUpDownDoubleLeftRight,
        BarUpLeft,
        BarDownRight,
        BarDown,
        BarLeft,
        BarRight,
        BarUp,
        BarDoubleDown,
        BarDoubleLeft,
        BarDoubleRight,
        BarDoubleUp,
        TriangleUp,
        TriangleDown,
        TriangleRight,
        TriangleLeft,
        ArrowUp,
        ArrowDown,
        ArrowRight,
        ArrowLeft,
        Unused1,
        Unused2,
        Unused3,
        Unused4,
        Unused5,
        Unused6,
        Unused7,
        Unused8,
        Solid,
        SolidFill,
        Dark,
        DarkFill,
        Gray,
        GrayFill,
        Light,
        LightFill,
        HorizontalBars,
        HorizontalBarsFill,
        VerticalBars,
        VerticalBarsFill,
        Face,
        Mountains,
        TreeConical,
        TreeRound,
        Heart,
        ShipUp,
        ShipUpRight,
        ShipRight,
        Alien1,
        Alien2,
        Door,
        Box,
        Star,
        Boat,
        NotUsed2,
        NotUsed3,
        NotUsed4,
        NotUsed5,
        NotUsed6,
        Close
    }
}