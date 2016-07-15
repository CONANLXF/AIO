using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

 namespace ExorAIO.Champions.Lux
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
                Targets.Target.HasBuff("luxilluminatingfraulein") ||
                Invulnerable.Check(Targets.Target, DamageType.Magical))
            {
                return;
            }

            /// <summary>
            ///     The E Harass Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                GameObjects.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "harass")) &&
                Vars.getSliderItem(Vars.EMenu, "harass") != 101)
            {
                Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).UnitPosition);
            }
        }
    }
}