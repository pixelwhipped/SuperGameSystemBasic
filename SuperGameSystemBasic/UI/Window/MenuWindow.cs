using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class MenuWindow : Window
    {
        public List<MenuItemWindow> MenuWindows = new List<MenuItemWindow>();

        public MenuWindow(IWindow parentWindow) : base(parentWindow.PostionX, 0, parentWindow.Width, 1, parentWindow)
        {
        }

        public int MenuWidth => Inputs.Any() ? Math.Max(Inputs.Max(p => p.PositionX + p.Width + 1), 4) : 0;

        public override void Draw()
        {
            DrawHLine(PostionX, PostionY, Width, Terminal.DARK_GRAY, true);
            base.Draw();
        }

        public void AddItem(string item, List<MenuButton> menuItems)
        {
            if (!(ParentWindow is FullWindow fw)) return;
            var w = MenuWidth;
            var menuWindow = new MenuItemWindow(MenuWidth, 1, 20, menuItems.Count, this) {Visible = false};                            
            var y = 0;

            foreach (var i in menuItems)
            {
                var a = i.Action;
                i.ParentWindow = menuWindow;
                i.PositionX = 0;
                i.PositionY = y;
                i.Action = () =>
                {
                    menuWindow.Visible = false;
                    a?.Invoke();
                };
                menuWindow.Inputs.Add(i);
                y++;
            }
            MenuWindows.Add(menuWindow);
            fw.Windows.Add(menuWindow);
            Inputs.Add(new Button(w + 1, 0, item, this)
            {
                BackColor = Terminal.DARK_GRAY,
                SelectedBackColor = Terminal.GRAY,
                SelectedTextColor = Terminal.WHITE,
                Action = () => ActivateMenu(menuWindow)
            });
        }


        private void ActivateMenu(MenuItemWindow menuWindow)
        {
            foreach (var w in MenuWindows.Where(p => p != menuWindow))
                w.Visible = false;

            menuWindow.Visible = true;
            if (!(ParentWindow is FullWindow f)) return;

            if (f.CurrentWindow == menuWindow) return;
            f.SelectWindow(menuWindow);
            menuWindow.SelectFirstItem();
        }

        public override bool Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
        {
            base.Update(key, getKey, click, mouseX, mouseY);
            return false;
        }
    }
}