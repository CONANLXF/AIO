using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TargetSelector = PortAIO.TSManager; namespace Azir_Creator_of_Elo
{
    class AzirMenu : Menu
    {
        public AzirMenu(String name) : base(name)
        {
            loadMenu();
        }
        public override void loadMenu()
        {
            base.loadMenu();
            loadLaneClearMenu();
            loadHarashMenu();
            loadComboMenu();
            loadJungleClearMenu();
            loadDrawings();
            loadJumps();
        }

        public void loadDrawings()
        {
            _drawSettingsMenu = menu.AddSubMenu("Draw Settings", "Draw Settings");
            {
                _drawSettingsMenu.Add("dsl", new CheckBox("Draw Soldier Line"));
                _drawSettingsMenu.Add("dcr", new CheckBox("Draw Control range"));
                _drawSettingsMenu.Add("dfr", new CheckBox("Draw Flee range"));
            }
        }
        public void loadComboMenu()
        {
            _comboMenu = menu.AddSubMenu("Combo Menu", "Combo Menu");
            {
                _comboMenu.Add("SoldiersToQ", new Slider("Soldiers to Q", 1, 1, 3));
                _comboMenu.Add("CQ", new CheckBox("Use Q"));
                _comboMenu.Add("CW", new CheckBox("Use W"));
                _comboMenu.Add("CR", new CheckBox("Use R killeable"));
            }
        }
        public void loadLaneClearMenu()
        {
            _laneClearMenu = menu.AddSubMenu("Laneclear Menu", "Laneclear Menu");
            {
                _laneClearMenu.Add("LQ", new CheckBox("Use Q"));
                _laneClearMenu.Add("LW", new CheckBox("Use W"));

            }
        }
        public void loadJungleClearMenu()
        {
            _JungleClearMenu = menu.AddSubMenu("JungleClear Menu", "JungleClear Menu");
            {
                _JungleClearMenu.Add("JW", new CheckBox("Use W"));

            }
        }
        public void loadHarashMenu()
        {
            _harashMenu = menu.AddSubMenu("Harash Menu", "Harash Menu");
            {
                _harashMenu.Add("hSoldiersToQ", new Slider("Soldiers to Q", 1, 1, 3));
                _harashMenu.Add("HQ", new CheckBox("Use Q"));
                _harashMenu.Add("HW", new CheckBox("Use W"));
                _harashMenu.Add("HW2", new CheckBox("Save on 1 w for flee"));
            }
        }
        public void loadJumps()
        {
            _jumpMenu = menu.AddSubMenu("Key Menu", "Key Menu");
            {
                _jumpMenu.Add("fleekey", new KeyBind("Jump key", false, KeyBind.BindTypes.HoldActive, 'Z'));
                _jumpMenu.Add("inseckey", new KeyBind("Insec key", false, KeyBind.BindTypes.HoldActive, 'T'));

            }
        }
    }
}
