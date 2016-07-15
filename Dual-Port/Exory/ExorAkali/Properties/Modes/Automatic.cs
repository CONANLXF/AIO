using System;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;

 namespace ExorAIO.Champions.Akali
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
                Vars.getCheckBoxItem(Vars.WMenu, "logical"))
            {
                if (Bools.HasDeadlyMark() ||
                    Health.GetPrediction(GameObjects.Player, (int) (750 + Game.Ping / 2f), 70) <= GameObjects.Player.MaxHealth / 4)
                {
                    Vars.W.Cast(GameObjects.Player.ServerPosition);
                }
            }
        }
    }
}