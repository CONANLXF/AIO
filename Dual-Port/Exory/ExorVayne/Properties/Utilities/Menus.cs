using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Vayne
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
                Vars.QMenu.Add("harass", new Slider("Harass / if Mana >= x%", 50, 0, 101));
                Vars.QMenu.Add("buildings", new Slider("Buildings / if Mana >= x%", 50, 0, 101));
                Vars.QMenu.Add("farmhelper", new Slider("FarmHelper / if Mana >= x%", 50, 0, 101));
                Vars.QMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.AddLabel("ExorCondemn: 95% Accuracy, 100% Walls and Buildings taken into account.");
                Vars.EMenu.Add("logical", new CheckBox("Logical", true));
                Vars.EMenu.Add("dashpred", new CheckBox("Dash-Prediction"));
                Vars.EMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser"));
                Vars.EMenu.Add("interrupter", new CheckBox("Interrupt Enemy Channels", true));
                Vars.EMenu.Add("killsteal", new CheckBox("KillSteal"));
                {
                    /// <summary>
                    ///     Sets the menu for the E Whitelist.
                    /// </summary>
                    Vars.WhiteListMenu = Vars.Menu.AddSubMenu("Condemn: Whitelist Menu");
                    {
                        foreach (var target in GameObjects.EnemyHeroes)
                        {
                            Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Condemn Only: {target.ChampionName}", true));
                        }
                    }
                }
            }

            /// <summary>
            ///     Sets the miscellaneous menu.
            /// </summary>
            Vars.MiscMenu = Vars.Menu.AddSubMenu("Miscellaneous");
            {
                Vars.MiscMenu.Add("alwaysq", new CheckBox("Always Q after AA", true));
                Vars.MiscMenu.Add("wstacks", new CheckBox("Use Q only to proc 3rd W Ring"));
                Vars.MiscMenu.Add("stealthtime", new Slider("Stay in stealth mode for at least x (ms) [1000 ms = 1 second]", 0, 0, 1000));
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q Range"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E Range"));
                Vars.DrawingsMenu.Add("epred", new CheckBox("E Prediction"));
            }
        }
    }
}