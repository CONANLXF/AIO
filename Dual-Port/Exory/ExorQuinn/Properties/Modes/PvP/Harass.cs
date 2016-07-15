using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Quinn
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
        public static void Harass(EventArgs args)
        {
            if (!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target) ||
                Vars.R.Instance.Name.Equals("QuinnRFinale"))
            {
                return;
            }

            /// <summary>
            ///     The Q Harass Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "harass")) &&
                Vars.getSliderItem(Vars.QMenu, "harass") != 101)
            {
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                }
            }
        }
    }
}