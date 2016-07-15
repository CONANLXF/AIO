using System;
using System.Linq;
using ExorAIO.Utilities;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.Data.Enumerations;
using SharpDX;
using Geometry = ExorAIO.Utilities.Geometry;

 namespace ExorAIO.Champions.MissFortune
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
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The Q KillSteal Logic.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     Normal Q KilLSteal Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.QMenu, "killsteal"))
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(
                        t =>
                            !Invulnerable.Check(t) &&
                            t.LSIsValidTarget(Vars.Q.Range) &&
                            Vars.GetRealHealth(t) <
                                (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q)))
                    {
                        Vars.Q.CastOnUnit(target);
                    }
                }

                /// <summary>
                ///     Extended Q KillSteal Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.Q2Menu, "exkillsteal"))
                {
                    /// <summary>
                    ///     Through enemy minions.
                    /// </summary>
                    foreach (var minion
                        in from minion
                        in Targets.Minions.Where(m => m.LSIsValidTarget(Vars.Q.Range))

                           let polygon = new Geometry.Sector(
                               (Vector2)minion.ServerPosition,
                               (Vector2)minion.ServerPosition.LSExtend(GameObjects.Player.ServerPosition, -(Vars.Q2.Range - Vars.Q.Range)),
                               40f * (float)Math.PI / 180f,
                               (Vars.Q2.Range - Vars.Q.Range) - 50f)

                           let target = GameObjects.EnemyHeroes.FirstOrDefault(
                               t =>
                                   !Invulnerable.Check(t) &&
                                   t.LSIsValidTarget(Vars.Q2.Range - 50f) &&
                                   Vars.GetRealHealth(t) <
                                       (Vars.GetRealHealth(minion) <
                                           (float)GameObjects.Player.LSGetSpellDamage(minion, SpellSlot.Q)
                                               ? (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q)
                                               : 0) +
                                       (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q, DamageStage.SecondForm) &&
                                   ((Vars.PassiveTarget.LSIsValidTarget() &&
                                       t.NetworkId == Vars.PassiveTarget.NetworkId) ||
                                       !Targets.Minions.Any(m => !polygon.IsOutside((Vector2)m.ServerPosition))))

                           where
                               target != null
                           where
                               !polygon.IsOutside((Vector2)target.ServerPosition) &&
                               !polygon.IsOutside(
                                   (Vector2)Movement.GetPrediction(
                                       target,
                                       GameObjects.Player.Distance(target) / Vars.Q.Speed + Vars.Q.Delay).UnitPosition)

                           select minion)
                    {
                        Vars.Q.CastOnUnit(minion);
                    }

                    /// <summary>
                    ///     Through enemy heroes.
                    /// </summary>
                    foreach (var target
                        in from target
                        in GameObjects.EnemyHeroes.Where(t => t.LSIsValidTarget(Vars.Q.Range))

                           let polygon = new Geometry.Sector(
                               (Vector2)target.ServerPosition,
                               (Vector2)target.ServerPosition.LSExtend(GameObjects.Player.ServerPosition, -(Vars.Q2.Range - Vars.Q.Range)),
                               40f * (float)Math.PI / 180f,
                               (Vars.Q2.Range - Vars.Q.Range) - 50f)

                           let target2 = GameObjects.EnemyHeroes.FirstOrDefault(
                               t =>
                                   !Invulnerable.Check(t) &&
                                   t.LSIsValidTarget(Vars.Q2.Range - 50f) &&
                                   Vars.GetRealHealth(t) <
                                       (Vars.GetRealHealth(target) <
                                           (float)GameObjects.Player.LSGetSpellDamage(target, SpellSlot.Q)
                                               ? (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q)
                                               : 0) +
                                       (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q, DamageStage.SecondForm) &&
                                   ((Vars.PassiveTarget.LSIsValidTarget() &&
                                       t.NetworkId == Vars.PassiveTarget.NetworkId) ||
                                       !Targets.Minions.Any(m => !polygon.IsOutside((Vector2)m.ServerPosition))))

                           where
                               target2 != null
                           where
                               !polygon.IsOutside((Vector2)target2.ServerPosition) &&
                               !polygon.IsOutside(
                                   (Vector2)Movement.GetPrediction(
                                       target2,
                                       GameObjects.Player.Distance(target) / Vars.Q.Speed + Vars.Q.Delay).UnitPosition)

                           select target)
                    {
                        Vars.Q.CastOnUnit(target);
                    }
                }
            }
        }
    }
}