using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;

namespace ExorAIO.Champions.Ryze
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
                Vars.QMenu.AddLabel("The Shield Logic allows you to manage when to use the shield.");
                Vars.QMenu.AddLabel("0 = Never use shield, 100 = Always use shield.");
                Vars.QMenu.Add("shield", new Slider("Shield / If Health <= x%", 25, 0, 100));
                Vars.QMenu.AddSeparator();
                Vars.QMenu.Add("combo", new CheckBox("Combo", true));
                Vars.QMenu.Add("killsteal", new CheckBox("KillSteal"));
                Vars.QMenu.Add("harass", new Slider("Harass / if Mana >= x%", 50, 0, 101));
                Vars.QMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 25, 0, 101));
                Vars.QMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 25, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("combo", new CheckBox("Combo", true));
                Vars.WMenu.Add("killsteal", new CheckBox("KillSteal"));
                Vars.WMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.Add("combo", new CheckBox("Combo", true));
                Vars.EMenu.Add("killsteal", new CheckBox("KillSteal"));
                Vars.EMenu.Add("harass", new Slider("Harass / if Mana >= x%", 50, 0, 101));
                Vars.EMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 25, 0, 101));
                Vars.EMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 25, 0, 101));
            }

            /// <summary>
            ///     Sets the miscellaneous menu.
            /// </summary>
            Vars.MiscMenu = Vars.Menu.AddSubMenu("Miscellaneous");
            {
                Vars.MiscMenu.Add("noaacombo", new CheckBox("Don't AA in Combo", true));
                Vars.MiscMenu.Add("tear", new Slider("Stack Tear / if Mana >= x%", 75, 1, 101));
                Vars.MiscMenu.AddLabel("The Support mode doesn't attack or throw spells to minions if there are allies nearby.");
                Vars.MiscMenu.Add("support", new CheckBox("Support Mode"));
            }

            /// <summary>
            ///     Sets the drawings menu.
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