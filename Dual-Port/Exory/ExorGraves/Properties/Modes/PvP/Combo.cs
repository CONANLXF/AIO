using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

 namespace ExorAIO.Champions.Graves
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
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                !Targets.Target.LSIsValidTarget(Vars.AARange) &&
                Vars.getCheckBoxItem(Vars.EMenu, "engager"))
            {
                if (GameObjects.Player.Distance(Game.CursorPos) > Vars.AARange &&
                    GameObjects.Player.ServerPosition
                        .LSExtend(Game.CursorPos, Vars.E.Range - Vars.AARange).CountEnemyHeroesInRange(1000f) < 3 &&
                    Targets.Target
                        .Distance(GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, Vars.E.Range - Vars.AARange)) < Vars.AARange)
                {
                    Vars.E.Cast(Game.CursorPos);
                }
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                !Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "combo"))
            {
                Vars.R.CastIfWillHit(Targets.Target, 2);
            }
        }
    }
}