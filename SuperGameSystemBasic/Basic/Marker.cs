namespace SuperGameSystemBasic.Basic
{
    public struct Marker
    {
        public int Pointer;
        public int Line;
        public int Column;

        public Marker(int pointer, int line, int column)
            : this()
        {
            Pointer = pointer;
            Line = line;
            Column = column;
        }
    }
}