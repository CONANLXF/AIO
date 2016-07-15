#region

using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

#endregion

using TargetSelector = PortAIO.TSManager; namespace NechritoRiven.Menus
{
    internal class MenuConfig : Core.Core
    {
        public static Menu Config, combo, lane, jngl, animation, misc, trinket, draw, flee, qmove;
        public static string menuName = "Nechrito Riven";

        public static void LoadMenu()
        {
            Config = MainMenu.AddMenu(menuName, menuName);

            animation = Config.AddSubMenu("Animation", "Animation");
            animation.Add("qReset", new CheckBox("Fast & Legit Q"));
            animation.AddGroupLabel("Emote");
            animation.Add("Qstrange", new CheckBox("Enables Emotes Below", false));
            animation.AddSeparator();
            animation.Add("animLaugh", new CheckBox("Laugh", false));
            animation.Add("animTaunt", new CheckBox("Taunt", false));
            animation.Add("animTalk", new CheckBox("Joke", false));
            animation.Add("animDance", new CheckBox("Dance", false));

            combo = Config.AddSubMenu("Combo", "Combo");
            combo.Add("ignite", new CheckBox("Auto Ignite"));
            combo.Add("OverKillCheck", new CheckBox("R Max Damage"));
            combo.Add("AlwaysR", new KeyBind("Force R", false, KeyBind.BindTypes.PressToggle, 'G'));
            combo.Add("AlwaysF", new KeyBind("Force Flash", false, KeyBind.BindTypes.PressToggle, 'L'));
            combo.AddSeparator();
            combo.AddGroupLabel("Keys : ");
            combo.Add("burst", new KeyBind("Burst (Combo Key immediately after jump)", false, KeyBind.BindTypes.HoldActive, 'T'));
            combo.Add("fastharass", new KeyBind("Fast Harass", false, KeyBind.BindTypes.HoldActive, 'U'));

            lane = Config.AddSubMenu("Lane", "Lane");
            lane.Add("LaneQ", new CheckBox("Use Q"));
            lane.Add("LaneW", new CheckBox("Use W"));
            lane.Add("LaneE", new CheckBox("Use E"));

            jngl = Config.AddSubMenu("Jungle", "Jungle");
            jngl.Add("JungleQ", new CheckBox("Use Q"));
            jngl.Add("JungleW", new CheckBox("Use W"));
            jngl.Add("JungleE", new CheckBox("Use E"));

            misc = Config.AddSubMenu("Misc", "Misc");
            misc.Add("GapcloserMenu", new CheckBox("Anti-Gapcloser"));
            misc.Add("InterruptMenu", new CheckBox("Interrupter"));
            misc.Add("KeepQ", new CheckBox("Keep Q Alive"));
            misc.Add("QD", new Slider("Ping Delay", 56, 20, 300));
            misc.Add("QLD", new Slider("Spell Delay", 56, 20, 300));

            trinket = Config.AddSubMenu("Trinket", "Trinket");
            trinket.Add("Buytrinket", new CheckBox("Buy Trinket"));
            trinket.Add("Trinketlist", new ComboBox("Choose Trinket", 0, "Oracle Alternation", "Farsight Alternation"));

            draw = Config.AddSubMenu("Draw", "Draw");
            draw.Add("FleeSpot", new CheckBox("Draw Flee Spots"));
            draw.Add("Dind", new CheckBox("Damage Indicator"));
            draw.Add("DrawForceFlash", new CheckBox("Flash Status"));
            draw.Add("DrawAlwaysR", new CheckBox("R Status"));
            draw.Add("DrawCB", new CheckBox("Combo Engage", false));
            draw.Add("DrawBT", new CheckBox("Burst Engage", false));
            draw.Add("DrawFH", new CheckBox("FastHarass Engage", false));
            draw.Add("DrawHS", new CheckBox("Harass Engage", false));

            flee = Config.AddSubMenu("Flee", "Flee");
            flee.Add("WallFlee", new CheckBox("WallJump in Flee"));
            flee.Add("FleeYoumuu", new CheckBox("Youmuu's Ghostblade"));

            qmove = Config.AddSubMenu("Q Move", "Q Move");
            qmove.Add("QMove", new KeyBind("Q Move", false, KeyBind.BindTypes.HoldActive, 'K'));
        }

        public static bool Burst => combo["burst"].Cast<KeyBind>().CurrentValue;
        public static bool FastHarass => combo["fastharass"].Cast<KeyBind>().CurrentValue;

        public static bool GapcloserMenu => misc["GapcloserMenu"].Cast<CheckBox>().CurrentValue;
        public static bool InterruptMenu => misc["InterruptMenu"].Cast<CheckBox>().CurrentValue;
        public static bool QMove => qmove["QMove"].Cast<KeyBind>().CurrentValue;
        public static int Trinketlist => trinket["Trinketlist"].Cast<ComboBox>().CurrentValue;
        public static bool FleeYomuu => flee["FleeYoumuu"].Cast<CheckBox>().CurrentValue;
        public static bool OverKillCheck => combo["OverKillCheck"].Cast<CheckBox>().CurrentValue;
        public static bool Buytrinket => trinket["Buytrinket"].Cast<CheckBox>().CurrentValue;
        public static bool FleeSpot => draw["FleeSpot"].Cast<CheckBox>().CurrentValue;
        public static bool WallFlee => flee["WallFlee"].Cast<CheckBox>().CurrentValue;
        public static bool jnglQ => jngl["JungleQ"].Cast<CheckBox>().CurrentValue;
        public static bool jnglW => jngl["JungleW"].Cast<CheckBox>().CurrentValue;
        public static bool jnglE => jngl["JungleE"].Cast<CheckBox>().CurrentValue;
        public static bool AlwaysF => combo["AlwaysF"].Cast<KeyBind>().CurrentValue;
        public static bool ignite => combo["ignite"].Cast<CheckBox>().CurrentValue;
        public static bool ForceFlash => draw["DrawForceFlash"].Cast<CheckBox>().CurrentValue;
        public static bool QReset => animation["qReset"].Cast<CheckBox>().CurrentValue;
        public static bool Dind => draw["Dind"].Cast<CheckBox>().CurrentValue;
        public static bool DrawCb => draw["DrawCB"].Cast<CheckBox>().CurrentValue;
        public static bool AnimLaugh => animation["animLaugh"].Cast<CheckBox>().CurrentValue;
        public static bool AnimTaunt => animation["animTaunt"].Cast<CheckBox>().CurrentValue;
        public static bool AnimDance => animation["animDance"].Cast<CheckBox>().CurrentValue;
        public static bool AnimTalk => animation["animTalk"].Cast<CheckBox>().CurrentValue;
        public static bool DrawAlwaysR => draw["DrawAlwaysR"].Cast<CheckBox>().CurrentValue;
        public static bool KeepQ => misc["KeepQ"].Cast<CheckBox>().CurrentValue;
        public static bool DrawFh => draw["DrawFH"].Cast<CheckBox>().CurrentValue;
        public static bool DrawHs => draw["DrawHS"].Cast<CheckBox>().CurrentValue;
        public static bool DrawBt => draw["DrawBT"].Cast<CheckBox>().CurrentValue;
        public static bool AlwaysR => combo["AlwaysR"].Cast<KeyBind>().CurrentValue;
        public static int Qd => misc["QD"].Cast<Slider>().CurrentValue;
        public static int Qld => misc["QLD"].Cast<Slider>().CurrentValue;
        public static bool LaneW => lane["LaneW"].Cast<CheckBox>().CurrentValue;
        public static bool LaneE => lane["LaneE"].Cast<CheckBox>().CurrentValue;
        public static bool Qstrange => animation["Qstrange"].Cast<CheckBox>().CurrentValue;
        public static bool LaneQ => lane["LaneQ"].Cast<CheckBox>().CurrentValue;
    }
}
