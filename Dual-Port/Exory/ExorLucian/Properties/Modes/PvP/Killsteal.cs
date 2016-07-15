using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using Geometry = ExorAIO.Utilities.Geometry;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Lucian
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
            ///     The Q Killsteal Logic.
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

                if (!GameObjects.EnemyHeroes.Any(
                    t =>
                        !Invulnerable.Check(t) &&
                        !t.LSIsValidTarget(Vars.Q.Range) &&
                        t.LSIsValidTarget(Vars.Q2.Range-50f) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q)))
                {
                    return;
                }

                /// <summary>
                ///     Extended Q KilLSteal Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.Q2Menu, "exkillsteal"))
                {
                    /// <summary>
                    ///     Through enemy minions.
                    /// </summary>
                    foreach (var minion 
                        in from minion
                        in Targets.Minions.Where(m => m.LSIsValidTarget(Vars.Q.Range))

                        let polygon = new Geometry.Rectangle(
                            GameObjects.Player.ServerPosition,
                            GameObjects.Player.ServerPosition.LSExtend(minion.ServerPosition, Vars.Q2.Range-50f),
                            Vars.Q2.Width)

                        where !polygon.IsOutside(
                            (Vector2)Vars.Q2.GetPrediction(GameObjects.EnemyHeroes.FirstOrDefault(
                            t =>
                                !Invulnerable.Check(t) &&
                                !t.LSIsValidTarget(Vars.Q.Range) &&
                                t.LSIsValidTarget(Vars.Q2.Range-50f) &&
                                Vars.GetRealHealth(t) <
                                    (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q))).UnitPosition)

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

                        let polygon = new Geometry.Rectangle(
                            GameObjects.Player.ServerPosition,
                            GameObjects.Player.ServerPosition.LSExtend(target.ServerPosition, Vars.Q2.Range-50f),
                            Vars.Q2.Width)

                        where !polygon.IsOutside(
                            (Vector2)Vars.Q2.GetPrediction(GameObjects.EnemyHeroes.FirstOrDefault(
                            t =>
                                !Invulnerable.Check(t) &&
                                !t.LSIsValidTarget(Vars.Q.Range) &&
                                t.LSIsValidTarget(Vars.Q2.Range-50f) &&
                                Vars.GetRealHealth(t) <
                                    (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q))).UnitPosition)

                        select target)
                    {
                        Vars.Q.CastOnUnit(target);
                    }
                }
            }

            /// <summary>
            ///     The KillSteal W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.W.Range) &&
                        !t.LSIsValidTarget(Vars.Q.Range) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.W)))
                {
                    if (!Vars.W.GetPrediction(target).CollisionObjects.Any())
                    {
                        Vars.W.Cast(Vars.W.GetPrediction(target).UnitPosition);
                    }
                }
            }
        }
    }
}