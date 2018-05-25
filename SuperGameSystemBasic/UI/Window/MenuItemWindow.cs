using System;
using Microsoft.Xna.Framework.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class MenuItemWindow : Window
    {
        public MenuItemWindow(int postionX, int postionY, int width, int height, IWindow parentWindow)
            : base(postionX, postionY, width, height, parentWindow)
        {
            BackColor = Terminal.DARK_GRAY;
        }

        public override void Unselect()
        {
            //Visible = false;
        }

        public override bool Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
        {
            base.Update(key, getKey, click, mouseX, mouseY);
            return false;
        }

        public override void Draw()
        {
            FillRect(0, 0, Width, Height, BackColor, true); //Main Box
            base.Draw();
        }
    }
}