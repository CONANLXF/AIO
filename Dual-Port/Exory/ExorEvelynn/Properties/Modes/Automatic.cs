using System;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;

 namespace ExorAIO.Champions.Evelynn
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
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.Player.HasBuffOfType(BuffType.Slow) &&
                Vars.getCheckBoxItem(Vars.WMenu, "auto"))
            {
                Vars.W.Cast();
            }
        }
    }
}