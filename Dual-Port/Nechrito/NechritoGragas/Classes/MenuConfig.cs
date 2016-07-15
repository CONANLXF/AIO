using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

using TargetSelector = PortAIO.TSManager; namespace Nechrito_Gragas
{
    class MenuConfig
    {
        public static Menu Config, combo, Lane, misc, draw, harass;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static string menuName = "Nechrito Gragas";
        public static void LoadMenu()
        {
            Config = MainMenu.AddMenu(menuName, menuName);

            combo = Config.AddSubMenu("Combo", "Combo");
            combo.Add("xd", new CheckBox("Select Target For Insec"));
            combo.Add("ComboR", new CheckBox("Use R"));

            harass = Config.AddSubMenu("Harass", "Harass");
            harass.Add("harassQ", new CheckBox("Harass Q"));
            harass.Add("harassW", new CheckBox("Harass W"));
            harass.Add("harassE", new CheckBox("Harass E"));

            Lane = Config.AddSubMenu("Lane", "Lane");
            Lane.Add("LaneQ", new CheckBox("Use Q"));
            Lane.Add("LaneW", new CheckBox("Use W"));
            Lane.Add("LaneE", new CheckBox("Use E"));

            misc = Config.AddSubMenu("Misc", "Misc");
            misc.Add("SmiteJngl", new CheckBox("Auto Smite"));

            draw = Config.AddSubMenu("Draw", "Draw");
            draw.Add("dind", new CheckBox("Damage Indicator"));
            draw.Add("prediction", new CheckBox("R Prediction"));

        }
        public static bool ComboR => getCheckBoxItem(combo, "ComboR");
        public static bool harassQ => getCheckBoxItem(harass, "harassQ");
        public static bool harassW => getCheckBoxItem(harass, "harassW");
        public static bool harassE => getCheckBoxItem(harass, "harassE");
        public static bool LaneQ => getCheckBoxItem(Lane, "LaneQ");
        public static bool LaneW => getCheckBoxItem(Lane, "LaneW");
        public static bool LaneE => getCheckBoxItem(Lane, "LaneE");
        public static bool dind => getCheckBoxItem(draw, "dind");
        public static bool prediction => getCheckBoxItem(draw, "prediction");

    }
}
