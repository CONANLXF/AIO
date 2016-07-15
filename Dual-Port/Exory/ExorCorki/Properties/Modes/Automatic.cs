using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Corki
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
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.Q.Range)))
                {
                    Vars.Q.Cast(target.ServerPosition);
                }
            }

            /// <summary>
            ///     The Automatic R LastHit Logics.
            /// </summary>
            if (Vars.R.IsReady() &&
                !PortAIO.OrbwalkerManager.isComboActive &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.R.Slot, Vars.getSliderItem(Vars.RMenu, "logical")) &&
                Vars.getSliderItem(Vars.RMenu, "logical") != 101)
            {
                foreach (var minion in GameObjects.EnemyMinions.Where(
                    m =>
                        m.LSIsValidTarget(Vars.R.Range) &&
                        !m.LSIsValidTarget(Vars.AARange)))
                {
                    if (Vars.GetRealHealth(minion) < (float)GameObjects.Player.LSGetSpellDamage(minion, SpellSlot.R, (ObjectManager.Player.HasBuff("corkimissilebarragecounterbig")
                        ? DamageStage.Empowered
                        : DamageStage.Default)))
                    {
                        if (!Vars.R.GetPrediction(minion).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                        {
                            Vars.R.Cast(minion.ServerPosition);
                        }
                    }
                }
            }
        }
    }
}