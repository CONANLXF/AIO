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
                Vars.QMenu.Add("combo", new CheckBox("Combo", true));
                Vars.QMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.QMenu.Add("harass", new Slider("Harass if Mana >= x%", 50, 0, 101));
                Vars.QMenu.Add("clear", new Slider("Clear if Mana >= x%", 25, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("combo", new CheckBox("Combo", true));
                Vars.WMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.WMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
                Vars.WMenu.Add("clear", new Slider("Clear if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.Add("combo", new CheckBox("Combo", true));
                Vars.EMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.EMenu.Add("harass", new Slider("Harass if Mana >= x%", 50, 0, 101));
                Vars.EMenu.Add("clear", new Slider("Clear if Mana >= x%", 25, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:");
            {
                Vars.RMenu.Add("combo", new CheckBox("Combo", true));
                Vars.RMenu.Add("clear", new CheckBox("Clear", true));
            }

            /// <summary>
            ///     Sets the miscellaneous menu.
            /// </summary>
            Vars.MiscMenu = Vars.Menu.AddSubMenu("Miscellaneous");
            {
                Vars.MiscMenu.Add("noaacombo", new CheckBox("Don't AA in Combo", true));
                Vars.MiscMenu.Add("tear", new Slider("Stack Tear if Mana >= x%", 75, 1, 101));
                Vars.MiscMenu.Add("stacks", new Slider("Keep Passive Stacks:", 0, 0, 4));
                Vars.MiscMenu.Add("stacksmana", new Slider("Keep Passive Stacks If Mana >= x%", 50, 1, 101));
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
            }
        }
    }
}