using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Lucian
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
                Vars.QMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 101));
                Vars.QMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 50, 0, 101));
                {
                    if (GameObjects.EnemyHeroes.Any())
                    {
                        /// <summary>
                        ///     Sets the Extended Q menu.
                        /// </summary>
                        Vars.Q2Menu = Vars.Menu.AddSubMenu("Use Extended Q in:");
                        {
                            Vars.Q2Menu.Add("excombo", new CheckBox("Combo", true));
                            Vars.Q2Menu.Add("exkillsteal", new CheckBox("KillSteal", true));
                            Vars.Q2Menu.Add("mixed", new Slider("Mixed / if Mana >= %", 50, 0, 101));
                            Vars.Q2Menu.Add("exlaneclear", new Slider("LaneClear / if Mana >= %", 50, 0, 101));
                        }

                        /// <summary>
                        ///     Sets the Whitelist menu for the Extended Q.
                        /// </summary>
                        Vars.WhiteListMenu = Vars.Menu.AddSubMenu("Extended Harass: Whitelist");
                        {
                            Vars.WhiteListMenu.AddLabel("Note: The Whitelist only works for Mixed and LaneClear.");
                            foreach (var target in GameObjects.EnemyHeroes)
                            {
                                Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Harass: {target.ChampionName}", true));
                            }
                        }
                    }
                    else
                    {
                        Vars.QMenu.AddLabel("No enemy champions found, no need for an Extended Q Menu.");
                    }
                }
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("combo", new CheckBox("Combo", true));
                Vars.WMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.WMenu.Add("buildings", new Slider("Buildings / if Mana >= x%", 50, 0, 101));
                Vars.WMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 101));
                Vars.WMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.AddLabel("E Modes:");
                Vars.EMenu.AddLabel("[KEEP IN MIND THAT, NO MATTER THE MODE, THE DASH WILL BE DIRECTED TOWARDS YOUR MOUSE]");
                Vars.EMenu.AddLabel("Exory: The Logic you have always used, with smart Short & Long dash.");
                Vars.EMenu.AddLabel("Normal: This Logic will make you always dash at the maximum distance.");
                Vars.EMenu.AddLabel("None: This Logic will prevent the assembly from using E automatically in combo.");
                Vars.EMenu.Add("mode", new ComboBox("E Mode", 0, "Exory", "Normal", "None"));
                Vars.EMenu.Add("engager", new CheckBox("Engager", true));
                Vars.EMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
                Vars.EMenu.Add("buildings", new Slider("Buildings / if Mana >= x%", 50, 0, 101));
                Vars.EMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 101));
                Vars.EMenu.Add("jungleclear", new Slider("JungleClear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:");
            {
                Vars.RMenu.AddLabel("How does it work:");
                Vars.RMenu.AddLabel("Keep the button pressed until you want to stop the ultimate.");
                Vars.RMenu.AddLabel("You don't have to press both Spacebar and the Semi-Automatic button while doing this,");
                Vars.RMenu.AddLabel("since ExorLucian automatically orbwalks while channelling his R, so just press the button.");
                Vars.RMenu.Add("bool", new CheckBox("Semi-Automatic R", true));
                Vars.RMenu.Add("key", new KeyBind("Key:", false, KeyBind.BindTypes.HoldActive, 'T'));
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q Range"));
                Vars.DrawingsMenu.Add("qe", new CheckBox("Q Extended Range"));
                Vars.DrawingsMenu.Add("w", new CheckBox("W Range"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E Range"));
            }
        }
    }
}