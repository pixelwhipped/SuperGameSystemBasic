using System.Collections.Generic;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class ColorItem
    {
        public int Color;
        public string Desc;

        public string GetDescription(int foreColor, int backColor)
        {
            return $"{(foreColor == Color ? "F" : " ")}{(backColor == Color ? "B" : " ")} {Color} = {Desc}";
        }
    }

    public class ColorChooserWindow : Window
    {
        public List<ColorItem> Items = new List<ColorItem>
        {
            new ColorItem {Color = Terminal.BLACK, Desc = "@BLACK"},
            new ColorItem {Color = Terminal.BLUE, Desc = "@BLUE"},
            new ColorItem {Color = Terminal.GREEN, Desc = "@GREEN"},
            new ColorItem {Color = Terminal.CYAN, Desc = "@CYAN"},
            new ColorItem {Color = Terminal.RED, Desc = "@RED"},
            new ColorItem {Color = Terminal.MAGENTA, Desc = "@MAGENTA"},
            new ColorItem {Color = Terminal.BROWN, Desc = "@BROWN"},
            new ColorItem {Color = Terminal.GRAY, Desc = "@GRAY"},
            new ColorItem {Color = Terminal.DARK_GRAY, Desc = "@DARK_GRAY"},
            new ColorItem {Color = Terminal.LIGHT_BLUE, Desc = "@LIGHT_BLUE"},
            new ColorItem {Color = Terminal.LIGHT_GREEN, Desc = "@LIGHT_GREEN"},
            new ColorItem {Color = Terminal.LIGHT_CYAN, Desc = "@LIGHT_CYAN"},
            new ColorItem {Color = Terminal.LIGHT_RED, Desc = "@LIGHT_RED"},
            new ColorItem {Color = Terminal.LIGHT_MAGENTA, Desc = "@LIGHT_MAGENTA"},
            new ColorItem {Color = Terminal.YELLOW, Desc = "@YELLOW"},
            new ColorItem {Color = Terminal.WHITE, Desc = "@WHITE"}
        };

        public int SelectedBackground = 15;

        public int SelectedForground;

        public ColorChooserWindow(int postionX, int postionY, IWindow parentWindow)
            : base(postionX, postionY, 30, 23, parentWindow)
        {
            BackColor = Terminal.BROWN;
            Items.Reverse();
            var y = 16;
            foreach (var item in Items)
            {
                Inputs.Add(new Button(1, y, "F", this)
                {
                    BackColor = BackColor,
                    SelectedBackColor = Terminal.LIGHT_BLUE,
                    Action = () => { SelectedForground = item.Color; }
                });
                Inputs.Add(new Button(4, y, "B", this)
                {
                    BackColor = BackColor,
                    SelectedBackColor = Terminal.LIGHT_BLUE,
                    Action = () => { SelectedBackground = item.Color; }
                });
                Inputs.Add(
                    new Label(
                        () => item.GetDescription(SelectedForground, SelectedBackground).PadRight(Width - 8, ' '), 7, y,
                        this)
                    {
                        TextColor = item.Color == Terminal.WHITE ? Terminal.BLACK : Terminal.WHITE,
                        BackColor = item.Color
                    });
                y--;
            }
            CurrentlySelected = Inputs[15];
            Inputs[15].Select();
        }

        public string Title { get; set; } = "Colors";

        public override void Click(int x, int y, bool handled)
        {
            if (x == Width - 1 && y == 0) ExitWindow();
        }

        public override void Draw()
        {
            if (!Visible) return;
            var pl = (Width - 1 - Title.Length) / 2;
            WriteText((" ".PadLeft(pl) + Title).PadRight(Width - 1, ' '), 0, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.Close, Width - 1, 0, TitleColour, TitleBarColour);
            FillRect(0, +1, Width, Height - 1, BackColor, true); //Main Box     
            WriteGlyph(Glyph.SolidFill, 1, 17, SelectedForground, SelectedBackground);
            WriteText($"DIM glyph = {{{(int) Glyph.SolidFill}, {SelectedForground}, {SelectedBackground}}}", 3, 17,
                Terminal.WHITE, BackColor);
            WriteGlyph(Glyph.DarkFill, 1, 18, SelectedForground, SelectedBackground);
            WriteText($"DIM glyph = {{{(int) Glyph.DarkFill}, {SelectedForground}, {SelectedBackground}}}", 3, 18,
                Terminal.WHITE, BackColor);
            WriteGlyph(Glyph.GrayFill, 1, 19, SelectedForground, SelectedBackground);
            WriteText($"DIM glyph = {{{(int) Glyph.GrayFill}, {SelectedForground}, {SelectedBackground}}}", 3, 19,
                Terminal.WHITE, BackColor);
            WriteGlyph(Glyph.LightFill, 1, 20, SelectedForground, SelectedBackground);
            WriteText($"DIM glyph = {{{(int) Glyph.LightFill}, {SelectedForground}, {SelectedBackground}}}", 3, 20,
                Terminal.WHITE, BackColor);
            WriteGlyph(Glyph.Space, 1, 21, SelectedForground, SelectedBackground);
            WriteText($"DIM glyph = {{{(int) Glyph.Space}, {SelectedForground}, {SelectedBackground}}}", 3, 21,
                Terminal.WHITE, BackColor);
            base.Draw();
        }
    }
}