namespace SuperGameSystemBasic.UI.Window
{
    public interface IDrawingContext
    {
        int ForeColor { get; set; }
        int BackColor { get; set; }
        void SetCursor(int x, int y);
        void Clear(int color);
        void DrawHLine(int x, int y, int l, int c, bool clear = false);
        void DrawVLine(int x, int y, int l, int c, bool clear = false);
        void FillRect(int x, int y, int w, int h, int c, bool clear = false);
        void DrawRect(int x, int y, int w, int h, int c, bool clear = false);
        void SetBackGround(int x, int y, int c, bool clear = false);
        void SetForgroundGround(int x, int y, int c, bool clear = false);
        int GetXY(int x, int y);

        void WriteText(string s, int x, int y, int fColor, int bColor);
        void WriteGlyph(Glyph g, int x, int y, int fColor, int bColor);
    }
}