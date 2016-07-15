using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Tristana
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
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !(Orbwalker.LastTarget as Obj_AI_Minion).LSIsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The Clear Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.Spellbook.IsAutoAttacking &&
                (Targets.Minions.Any() || Targets.JungleMinions.Any()) &&
                Vars.getCheckBoxItem(Vars.QMenu, "clear"))
            {
                Vars.Q.Cast();
            }

            /// <summary>
            ///     The Clear E Logics.
            /// </summary>
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     The JungleClear E Logic.
                /// </summary>
                if (Targets.JungleMinions.Any() &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "jungleclear")) &&
                    Vars.getSliderItem(Vars.EMenu, "jungleclear") != 101)
                {
                    Vars.E.CastOnUnit(Orbwalker.LastTarget as Obj_AI_Minion);
                }

                /// <summary>
                ///     The LaneClear E Logics.
                /// </summary>
                else
                {
                    /// <summary>
                    ///     The Aggressive LaneClear E Logic.
                    /// </summary>
                    if (GameObjects.EnemyHeroes.Any(
                        t =>
                            !Invulnerable.Check(t) &&
                            t.LSIsValidTarget(Vars.W.Range)) &&
                            GameObjects.Player.ManaPercent >
                                ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "harass")) &&
                            Vars.getSliderItem(Vars.EMenu, "harass") != 101)
                    {
                        foreach (var minion in Targets.Minions.Where(
                            m =>
                                m.CountEnemyHeroesInRange(150f) > 0 &&
                                Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m)))
                        {
                            Vars.E.CastOnUnit(minion);
                        }
                    }
                    else
                    {
                        /// <summary>
                        ///     The Normal LaneClear E Logic.
                        /// </summary>
                        if (Targets.Minions.Any() &&
                            GameObjects.Player.ManaPercent >
                                ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "laneclear")) &&
                            Vars.getSliderItem(Vars.EMenu, "laneclear") != 101)
                        {
                            if (Targets.Minions.Count(m => m.Distance(Orbwalker.LastTarget as Obj_AI_Minion) < 150f) >= 3)
                            {
                                Vars.E.CastOnUnit(Orbwalker.LastTarget as Obj_AI_Minion);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void BuildingClear(EventArgs args)
        {
            if (!(Orbwalker.LastTarget is Obj_AI_Turret))
            {
                return;
            }

            /// <summary>
            ///     The E BuildingClear Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "buildings")) &&
                Vars.getSliderItem(Vars.EMenu, "buildings") != 101)
            {
                Vars.E.CastOnUnit(Orbwalker.LastTarget as Obj_AI_Turret);
            }
        }
    }
}