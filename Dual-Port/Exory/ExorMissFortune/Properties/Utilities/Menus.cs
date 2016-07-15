using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.MissFortune
{
    /// <summary>
    ///     The menu class.
    /// </summary>
    internal class Menus
    {
        /// <summary>
        ///     Sets the menu.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            ///     Sets the menu for the Q.
            /// </summary>
            Vars.QMenu = Vars.Menu.AddSubMenu("Use Q to:");
            {
                Vars.QMenu.Add("combo", new CheckBox("Combo", true));
                Vars.QMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.QMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 50, 0, 99));
            }

            /// <summary>
            ///     Sets the Extended Q menu.
            /// </summary>
            Vars.Q2Menu = Vars.Menu.AddSubMenu("Use Extended Q in:");
            {
                Vars.Q2Menu.Add("combo", new CheckBox("Combo", true));
                Vars.Q2Menu.Add("exkillsteal", new CheckBox("KillSteal", true));
                Vars.Q2Menu.Add("mixed", new Slider("Mixed / if Mana >= x%", 50, 0, 99));
                Vars.Q2Menu.Add("exlaneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 99));
                Vars.Q2Menu.AddGroupLabel("Miscellaneous Exceptions List:");
                Vars.Q2Menu.Add("excombokill", new CheckBox("Combo: Only if Minion Killable", true));
                Vars.Q2Menu.Add("mixedkill", new CheckBox("Mixed: Only if Minion Killable", true));
                Vars.Q2Menu.Add("exlaneclearkill", new CheckBox("LaneClear: Only if Minion Killable", true));
            }

            /// <summary>
            ///     Sets the Whitelist menu for the Q.
            /// </summary>
            Vars.WhiteListMenu = Vars.Menu.AddSubMenu("Extended Harass: Whitelist");
            {
                Vars.WhiteListMenu.AddLabel("Note: The Whitelist only works for Mixed and LaneClear");

                foreach (var target in GameObjects.EnemyHeroes)
                {
                    Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Harass: {target.ChampionName}", true));

                }
            }


            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("combo", new CheckBox("combo", true));
                Vars.WMenu.Add("engager", new CheckBox("Engager", true));
                Vars.WMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 99));
                Vars.WMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 50, 0, 99));
                Vars.WMenu.Add("buildings", new Slider("Buildings / if Mana >= x%", 50, 0, 99));

            }


            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.Add("combo", new CheckBox("Combo", true));
                Vars.EMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
                Vars.EMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 99));
                Vars.EMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 50, 0, 99));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:");
            {
                Vars.RMenu.AddLabel("How does it work:");
                Vars.RMenu.AddLabel("Get in range for a target and keep the button pressed until you want to stop the ultimate.");
                Vars.RMenu.AddLabel("ExorMissFortune will not move or stop the ult automatically unless you release the button.");
                Vars.RMenu.Add("bool", new CheckBox("Semi-Automatic R", true));
                Vars.RMenu.Add("key", new KeyBind("Key (Semi-Auto) : ", false, KeyBind.BindTypes.HoldActive, 'T'));
            }

            /// <summary>
            ///     Sets the miscellaneous menu.
            /// </summary>
            Vars.MiscMenu = Vars.Menu.AddSubMenu("Miscellaneous");
            {
                Vars.MiscMenu.Add("passive", new CheckBox("Try to change target for Passive Proc", true));
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q Range"));
                Vars.DrawingsMenu.Add("qe", new CheckBox("Q Extended Range"));
                Vars.DrawingsMenu.Add("qc", new CheckBox("Q Extended Cones"));
                Vars.DrawingsMenu.Add("p", new CheckBox("Passive Target"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E Range"));
                Vars.DrawingsMenu.Add("r", new CheckBox("R Range"));

            }
        }
    }
}