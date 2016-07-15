using EloBuddy.SDK.Menu;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace Azir_Creator_of_Elo
{
    class Menu
    {
        public static EloBuddy.SDK.Menu.Menu _drawSettingsMenu, _jumpMenu, _comboMenu, _harashMenu, _laneClearMenu, _JungleClearMenu, menu;

        public EloBuddy.SDK.Menu.Menu GetMenu
        {
            get { return menu; }
        }
        private string _menuName;

        public Menu(string menuName)
        {
            this._menuName = menuName;
        }

        public virtual void loadMenu()
        {
            menu = MainMenu.AddMenu(_menuName, _menuName);
        }
    }
}
