using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Tryndamere
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
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                !Targets.Target.LSIsFacing(GameObjects.Player) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                Vars.W.Cast();
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                !Targets.Target.LSIsValidTarget(Vars.AARange) &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo"))
            {
                Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).UnitPosition);
            }
        }
    }
}