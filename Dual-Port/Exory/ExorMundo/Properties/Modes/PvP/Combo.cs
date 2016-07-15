using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.DrMundo
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
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                }
            }
        }
    }
}