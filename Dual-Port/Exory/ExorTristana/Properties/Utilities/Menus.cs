using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Tristana
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
                Vars.QMenu.Add("logical", new CheckBox("Logical", true));
                Vars.QMenu.Add("clear", new CheckBox("Clear", true));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("antigrab", new CheckBox("Anti-Grab", true));
                Vars.WMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser"));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.Add("combo", new CheckBox("Combo", true));
                Vars.EMenu.Add("harass", new Slider("Harass / if Mana >= x%", 50, 0, 101));
                Vars.EMenu.Add("buildings", new Slider("Buildings / if Mana >= x%", 50, 0, 101));
                Vars.EMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 101));
                Vars.EMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 50, 0, 101));
                {
                    Vars.WhiteListMenu = Vars.Menu.AddSubMenu("E: Whitelist Menu");
                    {
                        foreach (var target in GameObjects.EnemyHeroes)
                        {
                            Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Use E on: {target.ChampionName}", true));
                        }
                    }
                }
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:");
            {
                Vars.RMenu.Add("killsteal", new CheckBox("KillSteal", true));
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings");
            {
                Vars.DrawingsMenu.Add("w", new CheckBox("W Range"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E Range"));
                Vars.DrawingsMenu.Add("r", new CheckBox("R Range"));
            }
        }
    }
}