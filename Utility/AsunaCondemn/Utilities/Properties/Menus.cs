using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using TargetSelector = PortAIO.TSManager; namespace AsunaCondemn
{
    /// <summary>
    ///     The settings class.
    /// </summary>
    internal class Menus
    {
        /// <summary>
        ///     Sets the menu.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            ///     Sets the main menu.
            /// </summary>
            Vars.Menu = MainMenu.AddMenu("AsunaCondemn", "AsunaCondemn");
            Vars.Menu.Add("enable", new CheckBox("Enable", true));
            Vars.Menu.Add("keybind", new KeyBind("Execute:", false, KeyBind.BindTypes.HoldActive, 32));

            /// <summary>
            ///     Sets the spells menu.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Features Menu:");
            {
                Vars.EMenu.Add("dashpred", new CheckBox("Enable Dash-Prediction", true));
            }

            /// <summary>
            ///     Sets the menu for the Whitelist.
            /// </summary>
            Vars.WhiteListMenu = Vars.Menu.AddSubMenu("Whitelist Menu");
            {
                foreach (var target in GameObjects.EnemyHeroes)
                {
                    Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Use against: {target.ChampionName}", true));
                }
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings");
            {
                Vars.DrawingsMenu.Add("e", new CheckBox("E Prediction"));
            }
        }
    }
}