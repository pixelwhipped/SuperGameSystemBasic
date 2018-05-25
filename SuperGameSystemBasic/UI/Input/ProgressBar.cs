using System;
using Microsoft.Xna.Framework;

namespace SuperGameSystemBasic.UI.Input
{
    public class ProgressBar : Input
    {
        private int _percentageComplete;

        public ProgressBar(int percentageComplete, int x, int y, int height, int width, string id,
            Window.Window parentWindow) : base(x, y, width, height, parentWindow, id)
        {
            Selectable = false;
            PercentageComplete = percentageComplete;
        }

        public int BarColor { get; set; } = Terminal.CYAN;

        public int PercentageComplete
        {
            get => _percentageComplete;
            set
            {
                _percentageComplete = MathHelper.Clamp(value, 0, 100);
                Draw();
            }
        }

        public override void Draw()
        {
            var widthCompleted = (int) Math.Round(Width * ((double) PercentageComplete / 100));
            var widthUncompleted = Width - widthCompleted;

            WriteText("".PadRight(widthCompleted, (char) 161), PositionX, PositionY, BarColor, BackColor);
            WriteText("".PadRight(widthUncompleted, (char) 167), PositionX, PositionY + widthCompleted, BarColor,
                BackColor);
        }
    }
}