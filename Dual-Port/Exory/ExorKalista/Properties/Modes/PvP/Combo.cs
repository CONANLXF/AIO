using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Combo(EventArgs args)
        {
            /// <summary>
            ///     Orbwalk on minions.
            /// </summary>
            if (Items.HasItem(3085) &&
                Targets.Minions.Any(m => m.LSIsValidTarget(Vars.AARange)) &&
                !GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.AARange)) &&
                Vars.getCheckBoxItem(Vars.MiscMenu, "minionsorbwalk"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, Targets.Minions.FirstOrDefault(m => m.LSIsValidTarget(Vars.AARange)));
            }

            if (!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            if (Bools.HasSheenBuff())
            {
                if (Targets.Target.LSIsValidTarget(Vars.AARange))
                {
                    return;
                }
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !Invulnerable.Check(Targets.Target) &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any())
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                }
                else if (Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Count(
                    c =>
                        Targets.Minions.Contains(c) &&
                        c.Health <
                            (float)GameObjects.Player.LSGetSpellDamage(c, SpellSlot.Q)) == Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Count(c => Targets.Minions.Contains(c)))
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                }
            }
        }
    }
}