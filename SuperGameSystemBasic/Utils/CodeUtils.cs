using System;
using System.Collections.Generic;
using System.Text;
using SuperGameSystemBasic.Basic;
using SuperGameSystemBasic.UI.Window;

namespace SuperGameSystemBasic.Utils
{
    public static class CodeUtils
    {
        public static void FixIndents(this IEnumerable<CodeLine> lines)
        {
            var ic = 0;
            foreach (var line in lines)
            {
                var l = line.Line.TrimStart();
                if (l.StartsWith("IF") || l.StartsWith("WHILE") || l.StartsWith("FOR") || l.StartsWith("FUNCTION"))
                {
                    line.Line = IndentString(l, ic);
                    ic += 2;
                }
                else if (l.StartsWith("ENDIF") || l.StartsWith("WEND") || l.StartsWith("NEXT") || l.StartsWith("ENDFUNCTION"))
                {
                    ic = Math.Max(ic - 2, 0);
                    line.Line = IndentString(l, ic);
                }
                else if (l.StartsWith("ELSE"))
                {
                    ic = Math.Max(ic - 2, 0);
                    line.Line = IndentString(l, ic);
                    ic += 2;
                }
                else
                {
                    line.Line = IndentString(l, ic);
                }
            }
        }

        private static string IndentString(string l, int indent)
        {
            var s = string.Empty;
            for (var i = 0; i < indent; i++)
                s += " ";
            s += l;
            return s;
        }

        public static string FixCase(this string line)
        {
            if (line.TrimStart().StartsWith("REM", StringComparison.CurrentCultureIgnoreCase))
            {
                var pos = line.IndexOf("REM", StringComparison.CurrentCultureIgnoreCase);
                return line.Substring(0, pos) + "REM" + line.Substring(pos + 3);
            }
            var tokens = Interpreter.SplitAndKeep(line, Lexer.Tokens);
            var sb = new StringBuilder();
            var inQuotes = false;
            var inRem = false;
            foreach (var t in tokens)
            {
                var x = t.ToUpper();                
                if (x == "\"") inQuotes = !inQuotes;
                if (Interpreter.IsSyntax(x) || Interpreter.IsFunction(x)) sb.Append((inQuotes||inRem)?t:x);                
                else sb.Append(t);
                if (x == "REM") inRem = true;
            }
            return sb.ToString();
        }
    }
}