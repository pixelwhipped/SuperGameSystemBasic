namespace SuperGameSystemBasic.UI.Window
{
    public class PopupWindow : Window
    {
        public PopupWindow(string title, int postionX, int postionY, int width, int height, IWindow parentWindow)
            : base(postionX, postionY, width, height, parentWindow)
        {
            Title = title;
            BackColor = Terminal.RED;
        }

        public string Title { get; set; }

        public override void Click(int x, int y, bool handled)
        {
            if (x == Width - 1 && y == 0) ExitWindow();
        }

        public override void Draw()
        {
            var pl = (Width - 1 - Title.Length) / 2;
            WriteText((" ".PadLeft(pl) + Title).PadRight(Width - 1, ' '), 0, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.Close, Width - 1, 0, TitleColour, TitleBarColour);
            FillRect(0, +1, Width, Height - 1, BackColor, true); //Main Box        
            base.Draw();
        }
    }
}