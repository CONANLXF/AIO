using System;
using System.Linq;
using ExorAIO.Utilities;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Diana
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
            if (Bools.HasSheenBuff() &&
                Targets.Target.LSIsValidTarget(Vars.AARange))
            {
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.W.Range)) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                Vars.W.Cast();
            }

            if (!Targets.Target.LSIsValidTarget() ||
                Targets.Target.LSIsValidTarget(Vars.AARange) ||
                Invulnerable.Check(Targets.Target, DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range))
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).CastPosition);
            }

            /// <summary>
            ///     The R Logics.
            /// </summary>
            if (Vars.R.IsReady())
            {
                /// <summary>
                ///     The R Combo Logics.
                /// </summary>
                if (Targets.Target.LSIsValidTarget(Vars.R.Range) &&
                    Vars.getCheckBoxItem(Vars.RMenu, "combo"))
                {
                    /// <summary>
                    ///     The R Normal Combo Logic.
                    /// </summary>
                    if (Targets.Target.HasBuff("dianamoonlight") &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
                    {
                        if (!Targets.Target.IsUnderEnemyTurret() ||
                            Vars.GetRealHealth(Targets.Target) <
                                (float)GameObjects.Player.LSGetSpellDamage(Targets.Target, SpellSlot.Q) * 2 +
                                (float)GameObjects.Player.LSGetSpellDamage(Targets.Target, SpellSlot.R) * 2 ||
                            !Vars.getCheckBoxItem(Vars.MiscMenu, "safe"))
                        {
                            Vars.R.CastOnUnit(Targets.Target);
                            return;
                        }
                    }

                    /// <summary>
                    ///     The Second R Combo Logic.
                    /// </summary>
                    if (!Vars.Q.IsReady() &&
                        !Targets.Target.HasBuff("dianamoonlight") &&
                        Vars.getCheckBoxItem(Vars.MiscMenu, "rcombo") &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
                    {
                        DelayAction.Add(1000, () =>
                        {
                            if (!Vars.Q.IsReady() &&
                                !Targets.Target.HasBuff("dianamoonlight"))
                            {
                                Vars.R.CastOnUnit(Targets.Target);
                            }
                        });
                    }
                }
                /// <summary>
                ///     The R Gapclose Logic.
                /// </summary>
                else if (Targets.Target.LSIsValidTarget(Vars.Q.Range * 2) &&
                         !Targets.Target.LSIsValidTarget(Vars.Q.Range + 200) &&
                    Vars.getCheckBoxItem(Vars.MiscMenu, "gapclose"))
                {
                    if (Targets.Minions.Any(
                        m =>
                            m.LSIsValidTarget(Vars.Q.Range) &&
                            m.Distance(Targets.Target) < Vars.Q.Range &&
                            Vars.GetRealHealth(m) >
                                (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)))
                    {
                        Vars.Q.Cast(Targets.Minions.Where(
                            m =>
                                m.LSIsValidTarget(Vars.Q.Range) &&
                                m.Distance(Targets.Target) < Vars.Q.Range &&
                                Vars.GetRealHealth(m) >
                                    (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)).OrderBy(
                            o =>
                                o.DistanceToPlayer()).Last());
                    }

                    if (Targets.Minions.Any(
                        m =>
                            m.HasBuff("dianamoonlight") &&
                            m.LSIsValidTarget(Vars.R.Range) &&
                            m.Distance(Targets.Target) < Vars.Q.Range))
                    {
                        Vars.R.CastOnUnit(Targets.Minions.Where(
                            m =>
                                m.HasBuff("dianamoonlight") &&
                                m.LSIsValidTarget(Vars.R.Range) &&
                                m.Distance(Targets.Target) < Vars.Q.Range).OrderBy(
                            o =>
                                o.DistanceToPlayer()).Last());
                    }
                }
            }

            if (Vars.Q.IsReady() &&
                Vars.R.IsReady())
            {
                return;
            }
            
            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                !Targets.Target.LSIsValidTarget(Vars.AARange) &&
                Targets.Target.LSIsValidTarget(Vars.E.Range - 25) &&
                Vars.getCheckBoxItem(Vars.EMenu, "logical"))
            {
                Vars.E.Cast();
            }
        }
    }
}