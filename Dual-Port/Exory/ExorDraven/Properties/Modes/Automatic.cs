using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Draven
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                GameObjects.Player.GetBuffCount("dravenspinningattack") < 1 &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                Vars.Q.Cast();
            }
        }
    }
}