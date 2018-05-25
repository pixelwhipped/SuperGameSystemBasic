using System;

namespace SuperGameSystemBasic.UI.Input
{
    public class Label : Input
    {
        public Label(string text, int x, int y, Window.Window parentWindow, string id = null)
            : base(x, y, text.Length, 1, parentWindow, id)
        {
            TextGetter = () => text;
            BackColor = parentWindow.BackColor;
            Selectable = false;
        }

        public Label(Func<string> getter, int x, int y, Window.Window parentWindow, string id = null)
            : base(x, y, (getter ?? (() => string.Empty)).Invoke().Length, 1, parentWindow, id)
        {
            TextGetter = getter ?? (() => string.Empty);
            BackColor = parentWindow.BackColor;
            Selectable = false;
        }

        public Func<string> TextGetter { get; set; }
        public string Text => TextGetter?.Invoke();

        public override int Width => Text.Length;
        public int TextColor { get; set; } = Terminal.WHITE;

        public override void Draw()
        {
            WriteText(Text, PositionX, PositionY, TextColor, BackColor);
        }
    }
}