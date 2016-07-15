using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

using TargetSelector = PortAIO.TSManager; namespace Dark_Star_Thresh
{
    class MenuConfig : Core.Core
    {
        public static Menu Config, combo, Harass, Misc, Draw;
        public static string menuName = "Dark Star Thresh";

        public static void LoadMenu()
        {
            Config = MainMenu.AddMenu(menuName, menuName);

            combo = Config.AddSubMenu("Combo", "Combo");
            combo.Add("ComboFlash", new KeyBind("Flash Combo", false, KeyBind.BindTypes.HoldActive, 'T'));
            combo.Add("ComboR", new Slider("Min Enemies For R", 3, 0, 5));
            combo.Add("ComboQ", new Slider("Max Q Range", 110, 0, 110));
            combo.Add("ComboTaxi", new CheckBox("Taxi Mode (Beta!)"));

            Harass = Config.AddSubMenu("Harass", "Harass");
            Harass.Add("HarassAA", new CheckBox("Disable AA In Harass", false));
            Harass.Add("HarassQ", new CheckBox("Use Q (Won't use Q2)"));
            Harass.Add("HarassE", new CheckBox("Use E"));

            Misc = Config.AddSubMenu("Misc", "Misc");
            Misc.Add("Interrupt", new CheckBox("Interrupter"));
            Misc.Add("Gapcloser", new CheckBox("Gapcloser"));
            Misc.Add("Flee", new KeyBind("Flee", false, KeyBind.BindTypes.HoldActive, 'A'));

            Draw = Config.AddSubMenu("Draw", "Draw");
            Draw.Add("DrawDmg", new CheckBox("Draw Damage"));
            Draw.Add("DrawPred", new CheckBox("Draw Q Prediction"));
            Draw.Add("DrawQ", new CheckBox("Draw Q Range"));
            Draw.Add("DrawW", new CheckBox("Draw W Range"));
            Draw.Add("DrawE", new CheckBox("Draw E Range"));
            Draw.Add("DrawR", new CheckBox("Draw R Range"));

            Config.Add("Debug", new CheckBox("Debug Mode", false));
        }

        // Keybind
        public static bool ComboFlash => combo["ComboFlash"].Cast<KeyBind>().CurrentValue;
        public static bool Flee => Misc["Flee"].Cast<KeyBind>().CurrentValue;

        // Slider
        public static int ComboR => combo["ComboR"].Cast<Slider>().CurrentValue;
        public static int ComboQ => combo["ComboQ"].Cast<Slider>().CurrentValue;


        // Bool
        public static bool ComboTaxi => combo["ComboTaxi"].Cast<CheckBox>().CurrentValue;

        public static bool HarassAA => Harass["HarassAA"].Cast<CheckBox>().CurrentValue;
        public static bool HarassQ => Harass["HarassQ"].Cast<CheckBox>().CurrentValue;
        public static bool HarassE => Harass["HarassE"].Cast<CheckBox>().CurrentValue;

        public static bool Interrupt => Misc["Interrupt"].Cast<CheckBox>().CurrentValue;
        public static bool Gapcloser => Misc["Gapcloser"].Cast<CheckBox>().CurrentValue;

        public static bool DrawDmg => Draw["DrawDmg"].Cast<CheckBox>().CurrentValue;
        public static bool DrawPred => Draw["DrawPred"].Cast<CheckBox>().CurrentValue;
        public static bool DrawQ => Draw["DrawQ"].Cast<CheckBox>().CurrentValue;
        public static bool DrawW => Draw["DrawW"].Cast<CheckBox>().CurrentValue;
        public static bool DrawE => Draw["DrawE"].Cast<CheckBox>().CurrentValue;
        public static bool DrawR => Draw["DrawR"].Cast<CheckBox>().CurrentValue;

        public static bool Debug => Config["Debug"].Cast<CheckBox>().CurrentValue;
    }
}
