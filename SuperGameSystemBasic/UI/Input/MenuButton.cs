namespace SuperGameSystemBasic.UI.Input
{
    public class MenuButton : Button
    {
        public MenuButton(int x, int y, string text, Window.Window parentWindow, string id = null)
            : base(x, y, text, parentWindow, id)
        {
            BackColor = Terminal.DARK_GRAY;
            TextColor = Terminal.WHITE;
            SelectedBackColor = Terminal.GRAY;
            SelectedTextColor = Terminal.WHITE;
        }

        public override void Draw()
        {
            var fc = Selected ? SelectedTextColor : TextColor;
            var bc = Selected ? SelectedBackColor : BackColor;
            DrawHLine(0, PositionY, ParentWindow.Width, bc);
            WriteGlyph(Glyph.BarUpDown, PositionX, PositionY, fc, bc);
            WriteText(Text, PositionX + 1, PositionY, fc, bc);
            WriteGlyph(Glyph.BarUpDown, ParentWindow.Width - 1, PositionY, fc, bc);
        }

        public override void Tab()
        {
            ParentWindow.MoveToNextItem();
        }

        public override void Escape()
        {
            ParentWindow.Visible = false;
        }
    }
}