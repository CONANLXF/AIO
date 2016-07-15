using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

 namespace Slutty_Thresh
{
    internal class MenuConfig
    {
        public static Menu Config;
        public static Menu drawMenu, qMenu, comboMenu, lanternMenu, miscMenu, laneMenu, flashMenu;
        public const string Menuname = "Slutty Thresh";

        public static void CreateMenuu()
        {
            Config = MainMenu.AddMenu(Menuname, Menuname);


            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("Draw", new CheckBox("Display Drawings", true));
            drawMenu.Add("qDraw", new CheckBox("Draw [Q]", true));
            drawMenu.Add("wDraw", new CheckBox("Draw [W]", true));
            drawMenu.Add("eDraw", new CheckBox("Draw [E]", true));
            drawMenu.Add("qfDraw", new CheckBox("Draw Flash-[Q] Range", true));


            qMenu = Config.AddSubMenu("Q Settings", "qsettings");
            qMenu.Add("useQ", new CheckBox("Use [Q] (Death Sentence)", true));
            qMenu.Add("smartq", new CheckBox("Smart [Q]", true));
            qMenu.Add("useQ1", new CheckBox("Use 2nd [Q] (Death Leap)", true));
            qMenu.Add("useQ2", new Slider("Set 2nd-[Q] Delay (Death Leap)", 1000, 0, 1500));
            qMenu.Add("qrange", new Slider("Use [Q] Only if Target Range >=", 500, 0, 1040));


            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("useE", new CheckBox("Use [E] (Flay)", true));
            comboMenu.Add("combooptions", new ComboBox("Set [E] Mode", 1, "Push", "Pull"));
            comboMenu.Add("FlayPush", new KeyBind("Flay Push Key", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("FlayPull", new KeyBind("Flay Pull Key", false, KeyBind.BindTypes.HoldActive, 'H'));
            comboMenu.Add("useR", new CheckBox("Use [R] (The Box)", true));
            comboMenu.Add("rslider", new Slider("Use [R] Only if X Target(s) in Range", 3, 1, 5));


            lanternMenu = Config.AddSubMenu("Lantern Settings", "lantern");
            lanternMenu.Add("ThrowLantern", new KeyBind("Manual Lantern to Ally", false, KeyBind.BindTypes.HoldActive, 'T'));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe))
            {
                lanternMenu.Add("healop" + hero.ChampionName, new ComboBox(hero.ChampionName, 0, "Lantern", "No Lantern"));
                lanternMenu.Add("hpsettings" + hero.ChampionName, new Slider("Lantern When % HP <", 20));

            }
                            
            lanternMenu.Add("manalant", new Slider("Set % Mana for Lantern", 20));
            lanternMenu.Add("autolantern", new CheckBox("Auto-Lantern Ally if [Q] hits", false));

           
            laneMenu = Config.AddSubMenu("Lane Clear", "laneclear");
            laneMenu.Add("useelch", new CheckBox("Use [E]", true));


            flashMenu = Config.AddSubMenu("Flash-Hook Settings", "flashf");
            flashMenu.Add("flashmodes", new ComboBox("Flash Modes", 1, "Flash->E->Q", "Flash->Q"));
            flashMenu.Add("qflash", new KeyBind("Flash-Hook!", false, KeyBind.BindTypes.HoldActive, 'T'));


            miscMenu = Config.AddSubMenu("Miscellaneous", "miscsettings");
            miscMenu.Add("useW2I", new CheckBox("Interrupt with [W]", true));
            miscMenu.Add("useE2I", new CheckBox("Interrupt with [E]", true));
            miscMenu.Add("useQW2D", new CheckBox("Use W/Q on Dashing", true));

        }
    }
}