using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Corki
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
            ///     The R Harass Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.R.Range) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.R.Slot, Vars.getSliderItem(Vars.RMenu, "autoharass")) &&
                Vars.getSliderItem(Vars.RMenu, "autoharass") != 101 &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
            {
                if (!Vars.R.GetPrediction(Targets.Target).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                {
                    Vars.R.Cast(Vars.R.GetPrediction(Targets.Target).UnitPosition);
                }
            }
        }
    }
}