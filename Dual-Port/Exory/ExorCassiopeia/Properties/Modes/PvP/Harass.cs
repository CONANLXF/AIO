using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Cassiopeia
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
                Invulnerable.Check(Targets.Target, EloBuddy.DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "harass")) &&
                Vars.getSliderItem(Vars.QMenu, "harass") != 101)
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).CastPosition);
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            DelayAction.Add(1000,
                () =>
                {
                    if (Vars.W.IsReady() &&
                        Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                        GameObjects.Player.ManaPercent >
                            ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "harass")) &&
                        Vars.getSliderItem(Vars.WMenu, "harass") != 101)
                    {
                        Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).CastPosition);
                    }
                });
        }
    }
}
