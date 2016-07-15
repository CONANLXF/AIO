using TargetSelector = PortAIO.TSManager; namespace ElLux
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;
    public class ElLuxMenu
    {
        #region Static Fields

        public static Menu Menu, cMenu, hMenu, wMenu, lMenu, jMenu, miscMenu, ksMenu, drawMenu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("ElLux", "menuElLux");
            Menu.Add("ElLux.Combo.Semi.R", new KeyBind("Semi-manual R", false, KeyBind.BindTypes.HoldActive, 'T'));
            Menu.Add("ElLux.Combo.Semi.Q", new KeyBind("Semi-manual Q", false, KeyBind.BindTypes.HoldActive, 'Y'));

            cMenu = Menu.AddSubMenu("Combo", "Combo");
            cMenu.Add("ElLux.Combo.Q", new CheckBox("Use Q"));
            cMenu.Add("ElLux.Combo.E", new CheckBox("Use E"));
            cMenu.AddSeparator();
            cMenu.AddGroupLabel("Utlimate Settings : ");
            cMenu.Add("ElLux.Combo.R", new CheckBox("Use R"));
            cMenu.Add("ElLux.Combo.R.Rooted", new CheckBox("Use R when rooted", false));
            cMenu.Add("ElLux.Combo.R.Kill", new CheckBox("Use R when killable"));
            cMenu.Add("ElLux.Combo.R.AOE", new CheckBox("Use R AOE"));
            cMenu.Add("ElLux.Combo.R.Count", new Slider("Hit count", 3, 1, 5));

            hMenu = Menu.AddSubMenu("Harass", "Harass");
            hMenu.Add("ElLux.Harass.Q", new CheckBox("Use Q"));
            hMenu.Add("ElLux.Harass.E", new CheckBox("Use E"));

            wMenu = Menu.AddSubMenu("W settings", "WSettings");
            wMenu.Add("W.Activated", new CheckBox("Use W"));
            wMenu.Add("W.Damage", new Slider("W on damage dealt %", 80, 1));
            wMenu.AddSeparator();
            foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
            {
                wMenu.Add("wOn" + x.ChampionName, new CheckBox("Use for " + x.ChampionName));
            }

            lMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
            lMenu.Add("ElLux.Laneclear.Q", new CheckBox("Use Q"));
            lMenu.Add("ElLux.Laneclear.W", new CheckBox("Use W", false));
            lMenu.Add("ElLux.Laneclear.E", new CheckBox("Use E"));
            lMenu.Add("ElLux.Laneclear.E.Count", new Slider("Minion count", 1, 1, 5));

            jMenu = Menu.AddSubMenu("Jungleclear", "Jungleclear");
            jMenu.Add("ElLux.JungleClear.Q", new CheckBox("Use Q"));
            jMenu.Add("ElLux.JungleClear.W", new CheckBox("Use W"));
            jMenu.Add("ElLux.JungleClear.E", new CheckBox("Use E"));
            jMenu.Add("ElLux.JungleClear.E.Count", new Slider("Minion count", 1, 1, 5));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElLux.Auto.Q", new CheckBox("Auto Q on stuns"));
            miscMenu.Add("ElLux.Auto.E", new CheckBox("Auto E on stuns"));

            ksMenu = Menu.AddSubMenu("Killsteal", "Killsteal");
            ksMenu.Add("ElLux.KS.R", new CheckBox("KS with R"));

            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("ElLux.Draw.off", new CheckBox("Turn drawings off", false));
            drawMenu.Add("ElLux.Draw.Q", new CheckBox("Draw Q"));
            drawMenu.Add("ElLux.Draw.W", new CheckBox("Draw W"));
            drawMenu.Add("ElLux.Draw.E", new CheckBox("Draw E"));
            drawMenu.Add("ElLux.Draw.R", new CheckBox("Draw R"));

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}