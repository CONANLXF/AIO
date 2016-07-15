using LeagueSharp.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

 namespace Preserved_Kassadin.Cores
{
    class MenuConfig
    {
        private const string MenuName = "Preserved Kassadin";
        private static Menu Menu { get; set; }
        public static Menu comboMenu, harassMenu, laneMenu, jungleMenu, drawMenu, miscMenu, ksMenu;

        public static void Load()
        {
            Menu = MainMenu.AddMenu(MenuName, MenuName);

            #region Combo
            comboMenu = Menu.AddSubMenu("Combo", "ComboMenu");
            comboMenu.Add("SafeR", new Slider("Don't R Into x Enemies", 3, 0, 5));
            #endregion

            #region Harass
            harassMenu = Menu.AddSubMenu("Harass", "HarassMenu");
            harassMenu.Add("AutoHarass", new KeyBind("Auto Harass", false, KeyBind.BindTypes.PressToggle, 'T'));
            harassMenu.Add("HarassQ", new CheckBox("Harass Q", true));
            #endregion

            #region Lane
            laneMenu = Menu.AddSubMenu("Lane", "LaneMenu");
            laneMenu.Add("StackQ", new CheckBox("Stack Tear With Q", true));
            laneMenu.Add("StackMana", new Slider("Stack Minimum Mana %", 50, 0, 100));
            laneMenu.Add("LaneW", new CheckBox("Laneclear W", true));
            laneMenu.Add("LaneE", new CheckBox("Laneclear E", true));
            laneMenu.Add("LaneR", new CheckBox("Laneclear R", true));
            laneMenu.Add("LaneMana", new Slider("Lane Minimum Mana %", 50, 0, 100));
            #endregion

            #region Jungle
            jungleMenu = Menu.AddSubMenu("Jungle", "JungleMenu");
            jungleMenu.Add("JungleQ", new CheckBox("Jungle Q", true));
            jungleMenu.Add("JungleW", new CheckBox("Jungle W", true));
            jungleMenu.Add("JungleE", new CheckBox("Jungle E", true));
            jungleMenu.Add("JungleR", new CheckBox("Jungle R", true));
            #endregion

            #region Draw
            drawMenu = Menu.AddSubMenu("Draw", "DrawMenu");
            drawMenu.Add("DrawDmg", new CheckBox("Draw Damage", true));
            drawMenu.Add("DisableDraw", new CheckBox("Don't Draw", true));
            drawMenu.Add("DrawQ", new CheckBox("Q Range", true));
            drawMenu.Add("DrawW", new CheckBox("W Range", true));
            drawMenu.Add("DrawE", new CheckBox("E Range", true));
            drawMenu.Add("DrawR", new CheckBox("R Range", true));
            #endregion

            #region Killsteal
            ksMenu = Menu.AddSubMenu("Killsteal", "KillstealMenu");
            ksMenu.Add("KsQ", new CheckBox("Q Killsteal", true));
            ksMenu.Add("KsW", new CheckBox("W Killsteal", true));
            ksMenu.Add("KsE", new CheckBox("E Killsteal", true));
            ksMenu.Add("KsR", new CheckBox("R Killsteal", true));
            #endregion

            #region Trinket
            miscMenu = Menu.AddSubMenu("Trinket", "TrinketMenu");
            miscMenu.Add("BuyTrinket", new CheckBox("Buy Trinket", true));
            miscMenu.Add("TrinketList", new ComboBox("Choose Trinket", 1, "Oracle Alternation", "Farsight Alternation"));
            #endregion

        }


        // Bools
        public static bool BuyTrinket => miscMenu["BuyTrinket"].Cast<CheckBox>().CurrentValue;
        public static bool DrawDmg => drawMenu["DrawDmg"].Cast<CheckBox>().CurrentValue;
        public static bool DisableDraw => drawMenu["DisableDraw"].Cast<CheckBox>().CurrentValue;
        public static bool DrawQ => drawMenu["DrawQ"].Cast<CheckBox>().CurrentValue;
        public static bool DrawW => drawMenu["DrawW"].Cast<CheckBox>().CurrentValue;
        public static bool DrawE => drawMenu["DrawE"].Cast<CheckBox>().CurrentValue;
        public static bool DrawR => drawMenu["DrawR"].Cast<CheckBox>().CurrentValue;
        public static bool KsQ => ksMenu["KsQ"].Cast<CheckBox>().CurrentValue;
        public static bool KsW => ksMenu["KsW"].Cast<CheckBox>().CurrentValue;
        public static bool KsE => ksMenu["KsE"].Cast<CheckBox>().CurrentValue;
        public static bool KsR => ksMenu["KsR"].Cast<CheckBox>().CurrentValue;
        public static bool KsIgnite => ksMenu["KsIgnite"].Cast<CheckBox>().CurrentValue;
        public static bool StackQ => laneMenu["StackQ"].Cast<CheckBox>().CurrentValue;
        public static bool LaneW => laneMenu["LaneW"].Cast<CheckBox>().CurrentValue;
        public static bool LaneE => laneMenu["LaneE"].Cast<CheckBox>().CurrentValue;
        public static bool LaneR => laneMenu["LaneR"].Cast<CheckBox>().CurrentValue;
        public static bool JungleQ => jungleMenu["JungleQ"].Cast<CheckBox>().CurrentValue;
        public static bool JungleW => jungleMenu["JungleW"].Cast<CheckBox>().CurrentValue;
        public static bool JungleE => jungleMenu["JungleE"].Cast<CheckBox>().CurrentValue;
        public static bool JungleR => jungleMenu["JungleR"].Cast<CheckBox>().CurrentValue;
        public static bool HarassQ => harassMenu["HarassQ"].Cast<CheckBox>().CurrentValue;


        // List
        public static int TrinketList => miscMenu["TrinketList"].Cast<ComboBox>().CurrentValue;

        // Slider
        public static int SafeR => comboMenu["SafeR"].Cast<Slider>().CurrentValue;
        public static int StackMana => laneMenu["StackMana"].Cast<Slider>().CurrentValue;
        public static int LaneMana => laneMenu["LaneMana"].Cast<Slider>().CurrentValue;

        // Keybind
        public static bool AutoHarass => harassMenu["AutoHarass"].Cast<KeyBind>().CurrentValue;
    }
}
