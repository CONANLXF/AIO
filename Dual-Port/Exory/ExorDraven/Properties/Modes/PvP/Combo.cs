using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

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
                !GameObjects.Player.HasBuff("dravenfurybuff") &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "combo")) &&
                Vars.getSliderItem(Vars.WMenu, "combo") != 101)
            {
                if (GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.AARange)) &&
                    !GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.AARange)) &&
                    Vars.getCheckBoxItem(Vars.WMenu, "engager"))
                {
                    Vars.W.Cast();
                }
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo"))
            {
                Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).UnitPosition);
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.R.Range) &&
                Vars.getCheckBoxItem(Vars.RMenu, "combo"))
            {
                Vars.R.CastIfWillHit(Targets.Target, 2);
            }
        }
    }
}