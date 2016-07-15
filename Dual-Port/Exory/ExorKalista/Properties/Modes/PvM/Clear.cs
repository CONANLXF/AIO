using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.Data.Enumerations;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Kalista
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Q Clear Logics.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "clear")) &&
                Vars.getSliderItem(Vars.QMenu, "clear") != 101)
            {
                /// <summary>
                ///     The Q LaneClear Logic.
                /// </summary>
                if (Vars.Q.GetLineFarmLocation(Targets.Minions.Where(
                    m =>
                        m.Health <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)).ToList(), Vars.Q.Width).MinionsHit >= 3)
                {
                    Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions.Where(
                        m =>
                            m.Health <
                                (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)).ToList(), Vars.Q.Width).Position);
                }

                /// <summary>
                ///     The Q JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }

            /// <summary>
            ///     The E LaneClear Logics.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "laneclear")) &&
                Vars.getSliderItem(Vars.EMenu, "laneclear") != 101)
            {
                if (Targets.Minions.Count(
                    m =>
                        Bools.IsPerfectRendTarget(m) &&
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E) +
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E, DamageStage.Buff)) >= 2)
                {
                    Vars.E.Cast();
                }
            }
        }
    }
}