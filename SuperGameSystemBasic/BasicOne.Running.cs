using Microsoft.Xna.Framework.Graphics;
using SuperGameSystemBasic.Basic;

namespace SuperGameSystemBasic
{
    public partial class BasicOne
    {
        public void SetRunningState(string code)
        {
            Terminal.SetResolution(IdeTerminal.Columns, IdeTerminal.Rows);
            IdeState = IdeState.Running;
            Terminal.GlyphSheet =
                Content.Load<Texture2D>(Terminal._sheet); //need to repair any changes to the glyphsheet
            Terminal.Pallet = null;
            Terminal.Clear(0);
            Terminal.X = 0;
            Terminal.Y = 0;

            Interpreter = new Interpreter(this, code ?? ReadTutorial("Test.basic"), GetOsFunctions());
        }
    }
}