using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace OAnnie
{
    internal class MenuConfig
    {
        public static Menu Config;
        public static Menu drawMenu, comboMenu, flashMenu, tibbersMenu, ksMenu, laneMenu, miscMenu, harassMenu;
        public const string Menuname = "Annie";

        public static void CreateMenu()
        {
            #region General

            Config = MainMenu.AddMenu("OAnnie", "OAnnie");

            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            {
                drawMenu.Add("Draw", new CheckBox("Display Drawing", true));
                drawMenu.Add("qDraw", new CheckBox("Q Drawing", true));
                drawMenu.Add("wDraw", new CheckBox("W Drawing", true));
                drawMenu.Add("rDraw", new CheckBox("R Drawing", true));
                drawMenu.Add("rfDraw", new CheckBox("Flash->R Drawing", true));
                drawMenu.Add("FillDamage", new CheckBox("Show Combo Damage", true));
                drawMenu.Add("RushDrawWDamageFill", new CheckBox("Combo Damage Fill", true));
            }

            #endregion

            #region combo

            comboMenu = Config.AddSubMenu("Combo", "Combo");
            {
                comboMenu.Add("comboMenu.useignite", new CheckBox("Use Ignite", true));
                comboMenu.Add("comboMenu.useq", new CheckBox("Use [Q]", true));
                comboMenu.Add("comboMenu.usew", new CheckBox("Use [W]", true));
                comboMenu.Add("comboMenu.usee", new CheckBox("Use [E]", true));
                comboMenu.AddLabel("[E] Settings");
                comboMenu.Add("comboMenu.emenu.eaa", new CheckBox("[E] Against AA", true));
                comboMenu.Add("comboMenu.emenu.emode", new ComboBox("[E] Mode", 1, "E When Passive 3", "Always E"));
                comboMenu.AddLabel("[R] Settings");
                comboMenu.Add("comboMenu.user", new CheckBox("Use [R]", true));
                comboMenu.Add("comboMenu.user.smart", new CheckBox("Smart [R] 1v1 Logic", true));
                comboMenu.Add("comboMenu.user.Slider", new Slider("R If Hit X Enemies", 3, 0, 5));
                comboMenu.AddLabel("Passive Utillization");
                comboMenu.Add("comboMenu.passivemanagement.e.before", new CheckBox("Use E Before Q To Gain Stun", true));
                comboMenu.Add("comboMenu.passivemanagement.e.stack", new CheckBox("Use E To Stack Stun", false));
                comboMenu.Add("comboMenu.passivemanagement.w.stack", new CheckBox("Use W To Stack Stun", false));

            }

            #endregion

            #region flash

            flashMenu = Config.AddSubMenu("Flash Modes", "Flash");
            {
                flashMenu.Add("comboMenu.flashmenu.flashr", new KeyBind("Regular Flash R", false, KeyBind.BindTypes.HoldActive, 'T'));
                flashMenu.Add("comboMenu.flashmenu.flasher", new KeyBind("Ninja Flash R", false, KeyBind.BindTypes.HoldActive, 'H'));
            }

            #endregion

            #region tibbers

            tibbersMenu = Config.AddSubMenu("Tibbers Settings", "Tibbers");
            {
                tibbersMenu.Add("tibbersMenu.move", new CheckBox("Dynamic Tibbers Movement", true));
                tibbersMenu.Add("tibmove", new KeyBind("Move Tibbers", false, KeyBind.BindTypes.HoldActive, 'L'));
            }

            #endregion

            #region Kill Steal

            ksMenu = Config.AddSubMenu("Kill Steal", "KillSteal");
            {
                ksMenu.Add("killstealMenu.ks", new CheckBox("Kill Steal", true));
                ksMenu.Add("killstealMenu.q", new CheckBox("Use [Q]", true));
                ksMenu.Add("killstealMenu.w", new CheckBox("Use [W]", true));
                ksMenu.Add("killstealMenu.r", new CheckBox("Use [R]", true));
            }

            #endregion

            #region clear

            laneMenu = Config.AddSubMenu("Clear Settings", "ClearSettings");
            {
                laneMenu.AddGroupLabel("Wave Clear");
                laneMenu.Add("clearMenu.laneMenu.keepstun", new CheckBox("Keep Stun", true));
                laneMenu.Add("clearMenu.laneMenu.manaslider", new Slider("> % Mana to Lane clear", 30, 0, 100));
                laneMenu.Add("clearMenu.laneMenu.useq", new CheckBox("Use [Q]", true));
                laneMenu.Add("clearMenu.laneMenu.useqlast", new CheckBox("Use [Q] To Last Hit", true));
                laneMenu.Add("clearMenu.laneMenu.usew", new CheckBox("Use [W]", true));  
                laneMenu.Add("clearMenu.laneMenu.usewslider", new Slider("Use [W] If Hits X Enemies", 3, 1, 10));
                laneMenu.AddGroupLabel("Jungle Clear");
                laneMenu.Add("clearMenu.jungleMenu.useq", new CheckBox("Use [Q]", true));
                laneMenu.Add("clearMenu.jungleMenu.usee", new CheckBox("Use [E]", true));
                laneMenu.Add("clearMenu.jungleMenu.usew", new CheckBox("Use [W]", true));
                laneMenu.AddGroupLabel("Last Hit");
                laneMenu.Add("clearMenu.lastMenu.keepstun", new CheckBox("Keep Stun", true));
                laneMenu.Add("clearMenu.lastMenu.useqlast", new CheckBox("Use [Q]", true));
            }

            #endregion

            #region misc

            miscMenu = Config.AddSubMenu("Misc Settings", "MiscSettings");
            {
                miscMenu.Add("miscMenu.qwdash", new CheckBox("[Q]/[W] Dash", true));
                miscMenu.Add("miscMenu.qwgap", new CheckBox("[Q]/[W] Gap Closer", true));
            }

            #endregion

            #region harras

            harassMenu = Config.AddSubMenu("Harras Settings", "HarrasSettings");
            {
                harassMenu.Add("harrasMenu.useq", new CheckBox("Use [Q]", true));
                harassMenu.Add("harrasMenu.usew", new CheckBox("Use [W]", true));
            }

            #endregion
        }
    }
}
           