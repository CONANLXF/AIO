using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;

 namespace ExorAIO.Champions.Veigar
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
                Vars.QMenu.Add("clear", new Slider("Clear / if Mana >= x%", 25, 0, 101));
                Vars.QMenu.Add("harass", new Slider("Harass / if Mana >= x%", 25, 0, 101));
                Vars.QMenu.Add("lasthit", new Slider("LastHit / if Mana >= x%", 0, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("logical", new CheckBox("Logical", true));
                Vars.WMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.WMenu.Add("harass", new Slider("Harass / if Mana >= x%", 50, 0, 101));
                Vars.WMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 50, 0, 101));
                Vars.WMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 101));
                Vars.WMenu.Add("minionshit", new Slider("LaneClear / if can hit >= x minions", 3, 1, 6));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.Add("combo", new CheckBox("Combo", true));
                Vars.EMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
                Vars.EMenu.Add("interrupter", new CheckBox("Interrupt Enemy Channels", true));
                Vars.EMenu.Add("enemies", new Slider("Automatic / if can hit >= than x Enemies", 2, 2, 6));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:");
            {
                Vars.RMenu.Add("killsteal", new CheckBox("KillSteal", true));
            }

            /// <summary>
            ///     Sets the miscellaneous menu.
            /// </summary>
            Vars.MiscMenu = Vars.Menu.AddSubMenu("Miscellaneous");
            {
                Vars.MiscMenu.Add("noaacombo", new CheckBox("Don't AA in Combo (Doesn't attack in Combo Mode if any Spell is ready)"));
                Vars.MiscMenu.Add("qfarmmode", new CheckBox("Only LastHit with Q while farming (Doesn't Attack In LastHit/LaneClear if Q is ready)"));
                Vars.MiscMenu.Add("tear", new Slider("Stack Tear / if Mana >= x%", 80, 0, 101));
                Vars.MiscMenu.AddLabel("The Support mode doesn't attack or throw spells to minions if there are allies nearby.");
                Vars.MiscMenu.Add("support", new CheckBox("Support Mode"));
            }

            /// <summary>
            /// Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q Range"));
                Vars.DrawingsMenu.Add("w", new CheckBox("W Range"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E Range"));
                Vars.DrawingsMenu.Add("r", new CheckBox("R Range"));
            }
        }
    }
}