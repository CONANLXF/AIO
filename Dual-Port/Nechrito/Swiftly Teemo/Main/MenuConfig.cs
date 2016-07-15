#region

using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Swiftly_Teemo.Main
{
    internal class MenuConfig
    {
        public static Menu Menu, comboMenu, laneMenu, drawMenu;

        public static bool KillStealSummoner;
        public static bool TowerCheck;
        public static bool LaneQ;
        public static bool Dind;
        public static bool EngageDraw;
        public static bool Flee;
        public static bool DrawR;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }


        public static void Load()
        {

            Menu = MainMenu.AddMenu("Swiftly Teemo", "Teemo");

            comboMenu = Menu.AddSubMenu("Combo", "ComboMenu");
            comboMenu.Add("KillStealSummoner", new CheckBox("KillSecure Ignite", true));
            comboMenu.Add("TowerCheck", new CheckBox("Don't Combo Under Turret", true));

            laneMenu = Menu.AddSubMenu("Lane", "LaneMenu");
            laneMenu.Add("LaneQ", new CheckBox("Last Hit Q AA", true));

            drawMenu = Menu.AddSubMenu("Draw", "Draw");
            drawMenu.Add("dind", new CheckBox("Damage Indicator", true));
            drawMenu.Add("EngageDraw", new CheckBox("Draw Engage", true));
            drawMenu.Add("DrawR", new CheckBox("Draw R Prediction"));

            Menu.Add("Flee", new KeyBind("Flee", false, KeyBind.BindTypes.HoldActive, 'Z'));

            KillStealSummoner = getCheckBoxItem(comboMenu, "KillStealSummoner");
            TowerCheck = getCheckBoxItem(comboMenu, "TowerCheck");
            LaneQ = getCheckBoxItem(laneMenu, "LaneQ");
            Dind = getCheckBoxItem(drawMenu, "dind");
            EngageDraw = getCheckBoxItem(drawMenu, "EngageDraw");
            Flee = getKeyBindItem(Menu, "Flee");
            DrawR = getCheckBoxItem(drawMenu, "DrawR");
        }
    }
}
