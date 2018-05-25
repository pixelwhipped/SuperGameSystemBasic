using SuperGameSystemBasic.Utils;

namespace SuperGameSystemBasic.UI.Window
{
    public class CodeLine
    {
        private string _line;

        public CodeLine(string line)
        {
            Line = line ?? string.Empty;
        }

        public string Line
        {
            get => _line ?? string.Empty;
            set => _line = value == null ? string.Empty : value.Replace("\t", "     ").FixCase();
        }
    }
}