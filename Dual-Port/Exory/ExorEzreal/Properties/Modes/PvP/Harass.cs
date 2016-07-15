using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Ezreal
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
                Invulnerable.Check(Targets.Target))
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
                Vars.getSliderItem(Vars.QMenu, "harass") != 101 &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
            {
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any())
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                    return;
                }
            }

            /// <summary>
            ///     The W Harass Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "harass")) &&
                Vars.getSliderItem(Vars.WMenu, "harass") != 101)
            {
                Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).UnitPosition);
            }
        }
    }
}