using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Karma
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
                Vars.QMenu.Add("clear", new Slider("Clear if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("combo", new CheckBox("Combo", true));
                Vars.WMenu.Add("jungleclear", new Slider("JungleClear if Mana >= x%", 50, 0, 101));
                Vars.WMenu.Add("lifesaver", new Slider("Logical Lifesaver If Health < x%", 20, 10, 100));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.Add("engager", new CheckBox("Engager"));
                Vars.EMenu.Add("logical", new CheckBox("Logical", true));
                Vars.EMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
                Vars.EMenu.Add("aoe", new Slider("Logical AoE If Allies >= x", 3, 2, 6));
            }

            /// <summary>
            ///     Sets the whitelist menu for the E.
            /// </summary>
            Vars.WhiteListMenu = Vars.Menu.AddSubMenu("Shield: Whitelist Menu");
            {
                foreach (var ally in GameObjects.AllyHeroes)
                {
                    Vars.WhiteListMenu.Add(ally.ChampionName.ToLower(), new CheckBox($"Use for: {ally.ChampionName}", true));
                }
            }

            /// <summary> summary set W
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:");
            {
                Vars.RMenu.Add("empq", new CheckBox("Logical Q Empower", true));
                Vars.RMenu.Add("empe", new CheckBox("Logical E Empower", true));
            }

            /// <summary>
            ///     Sets the miscellaneous menu.
            /// </summary>
            Vars.MiscMenu = Vars.Menu.AddSubMenu("Miscellaneous");
            {
                Vars.MiscMenu.AddLabel("The Support mode doesn't attack or throw spells to minions if there are allies nearby.");
                Vars.MiscMenu.Add("support", new CheckBox("Support Mode", true));
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