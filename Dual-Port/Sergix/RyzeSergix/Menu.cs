using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace RyzeAssembly
{
    class Menu
    {
        public static EloBuddy.SDK.Menu.Menu _drawSettingsMenu, _laneclearMenu, _jungleclearMenu, _harrashMenu, _miscMenu;
        public EloBuddy.SDK.Menu.Menu menu;
        public Menu()
        {
            loadMenu();
        }
        public void loadMenu()
        {
            Chat.Print("Hello");
            menu = EloBuddy.SDK.Menu.MainMenu.AddMenu("Ryze", "Ryze");

            loadLaneClear();
            loadDrawings();
            loadJungleClear();
            loadHarassh();
            loadMisc();
        }
        public void loadHarassh()
        {

            _harrashMenu = menu.AddSubMenu("Harrash", "Harrash");
            {
                _harrashMenu.Add("QH", new CheckBox("Use Q in Harrash"));
                _harrashMenu.Add("ManaH", new Slider("% mana Harrash", 40, 0, 100));
            }
        }

        public void loadLaneClear()
        {
            _laneclearMenu = menu.AddSubMenu("Laneclear", "Laneclear");
            {
                _laneclearMenu.Add("QL", new CheckBox("Use Q in Laneclear"));
                _laneclearMenu.Add("WL", new CheckBox("Use W in Laneclear"));
                _laneclearMenu.Add("EL", new CheckBox("Use E in Laneclear"));
                _laneclearMenu.Add("RL", new CheckBox("Use R in Laneclear"));
                _laneclearMenu.Add("ManaL", new Slider("% mana LaneClear", 40, 0, 100));
            }

        }
        public void loadJungleClear()
        {
            _jungleclearMenu = menu.AddSubMenu("Jungleclear", "Jungleclear");
            {
                _jungleclearMenu.Add("QJ", new CheckBox("Use Q in JungleClear"));
                _jungleclearMenu.Add("WJ", new CheckBox("Use W in JungleClear"));
                _jungleclearMenu.Add("EJ", new CheckBox("Use E in JungleClear"));
                _jungleclearMenu.Add("RJ", new CheckBox("Use R in JungleClear"));
                _jungleclearMenu.Add("ManaJ", new Slider("% mana JungleClear", 40, 0, 100));

            }
        }
        public void loadDrawings()
        {
            _drawSettingsMenu = menu.AddSubMenu("Draw Settings", "Draw Settings");
            {
                _drawSettingsMenu.Add("Draw Q Range", new CheckBox("Draw Q Range"));
                _drawSettingsMenu.Add("Draw W Range", new CheckBox("Draw W Range"));
                _drawSettingsMenu.Add("Draw E Range", new CheckBox("Draw E Range"));
            }
        }

        public void loadMisc()
        {
            _miscMenu = menu.AddSubMenu("Misc", "Misc");
            {
                _miscMenu.Add("%R", new Slider("% R heal ", 30, 0, 100));
            }
        }
    }
}
