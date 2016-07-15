using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Draven
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
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("combo", new Slider("Combo / If Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.Add("combo", new CheckBox("Combo", true));
                Vars.EMenu.Add("clear", new Slider("Clear / If Mana >= x%", 50, 0, 101));
                Vars.EMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.EMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
                Vars.EMenu.Add("interrupter", new CheckBox("Interrupt Enemy Channels", true));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:");
            {
                Vars.RMenu.Add("combo", new CheckBox("Combo", true));
                Vars.RMenu.Add("killsteal", new CheckBox("KillSteal", true));
                {
                    /// <summary>
                    ///     Sets the menu for the R Whitelist.
                    /// </summary>
                    Vars.WhiteListMenu = Vars.Menu.AddSubMenu("Ultimate: Whitelist Menu");
                    {
                        foreach (var target in GameObjects.EnemyHeroes)
                        {
                            Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Use against: {target.ChampionName}", true));
                        }
                    }
                }
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings");
            {
                Vars.DrawingsMenu.Add("w", new CheckBox("W Range"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E Range"));
            }
        }
    }
}