using System;
using Microsoft.Xna.Framework.Graphics;

namespace SuperGameSystemBasic
{
    public partial class BasicOne
    {
        public Exception LastError { get; set; }
        public int ErrorLine { get; set; } = -1;

        public TimeSpan ErrorWait { get; set; } = TimeSpan.Zero;

        public void SetErrorState(Exception e)
        {
            Terminal.SetResolution(IdeTerminal.Columns, IdeTerminal.Rows);
            LastError = e;
            ErrorWait = TimeSpan.Zero;
            IdeState = IdeState.Error;

            Interpreter = null;
            IdeTerminal.Clear(Terminal.YELLOW);
            IdeTerminal.GlyphSheet = Content.Load<Texture2D>(IdeTerminal._sheet);
            IdeTerminal.Pallet = null;
            IdeTerminal.Y = 4;
            var err = e.Message.Split('\n');
            foreach (var l in err)
            {
                IdeTerminal.X = 4;
                IdeTerminal.Print(CursorBlink ? Glyph.Alien1 : Glyph.Alien2, Terminal.BLACK, Terminal.YELLOW,
                    SpriteEffects.None);
                IdeTerminal.Print(l, Terminal.BLACK, Terminal.YELLOW, SpriteEffects.None);
                IdeTerminal.Y++;
            }

            IdeTerminal.Y++;
            IdeTerminal.X = 4;
            IdeTerminal.Print("Press any key to return to the Development Kit", Terminal.BLACK, Terminal.YELLOW,
                SpriteEffects.None);
            if (Editor != null && ErrorLine <= Editor.VirtualHeight)
                Editor.ScrollTo(Math.Max(ErrorLine, 0));
        }
    }
}