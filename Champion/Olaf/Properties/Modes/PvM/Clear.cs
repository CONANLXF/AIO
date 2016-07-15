using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using Geometry = ExorAIO.Utilities.Geometry;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;
using LeagueSharp.SDK.Enumerations;

 namespace ExorAIO.Champions.Olaf
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
                ///     The JungleClear Q Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The LaneClear Q Logics.
                /// </summary>
                else
                {
                    /// <summary>
                    ///     The Aggressive LaneClear Q Logic.
                    /// </summary>
                    if (GameObjects.EnemyHeroes.Any(
                        t =>
                            !Invulnerable.Check(t) &&
                            t.LSIsValidTarget(Vars.Q.Range)))
                    {
                        if (Vars.Q.GetLineFarmLocation(Targets.Minions).MinionsHit >= 3 &&
                            !new Geometry.Rectangle(
                                GameObjects.Player.ServerPosition,
                                GameObjects.Player.ServerPosition.LSExtend(Targets.Minions[0].ServerPosition, Vars.Q.Range),
                                Vars.Q.Width).IsOutside(
                                    (Vector2)Vars.Q.GetPrediction(GameObjects.EnemyHeroes.FirstOrDefault(
                                        t =>
                                            !Invulnerable.Check(t) &&
                                            t.LSIsValidTarget(Vars.Q.Range))).UnitPosition))
                        {
                            Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions).Position);
                        }
                    }

                    /// <summary>
                    ///     The LaneClear Q Logic.
                    /// </summary>
                    else if (!GameObjects.EnemyHeroes.Any(
                        t =>
                            !Invulnerable.Check(t) &&
                            t.LSIsValidTarget(Vars.Q.Range + 100f)))
                    {
                        if (Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3)
                        {
                            Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).Position);
                        }
                    }
                }
            }

            if (Orbwalker.LastTarget as Obj_AI_Minion == null)
            {
                return;
            }

            /// <summary>
            ///     The W Clear Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "clear")) &&
                Vars.getSliderItem(Vars.WMenu, "clear") != 101)
            {
                Vars.W.Cast();
            }

            /// <summary>
            ///     The E LaneClear Logics.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getSliderItem(Vars.EMenu, "clear") != 101)
            {
                foreach (var minion in Targets.Minions.Where(
                    m =>
                        m.LSIsValidTarget(Vars.E.Range) &&
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E)))
                {
                    if (minion.GetMinionType() == MinionTypes.Siege ||
                        minion.GetMinionType() == MinionTypes.Super)
                    {
                        Vars.E.CastOnUnit(minion);
                    }
                }
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void JungleClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Orbwalker.LastTarget as Obj_AI_Minion == null ||
                !Targets.JungleMinions.Contains(Orbwalker.LastTarget as Obj_AI_Minion))
            {
                return;
            }

            /// <summary>
            ///     The E JungleClear Logics.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.HealthPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                Vars.getSliderItem(Vars.EMenu, "clear") != 101)
            {
                Vars.E.CastOnUnit(Targets.JungleMinions[0]);
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void BuildingClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(Orbwalker.LastTarget is Obj_HQ) &&
                !(Orbwalker.LastTarget is Obj_AI_Turret) &&
                !(Orbwalker.LastTarget is Obj_BarracksDampener))
            {
                return;
            }

            /// <summary>
            ///     The W BuildingClear Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "buildings")) &&
                Vars.getSliderItem(Vars.WMenu, "buildings") != 101)
            {
                Vars.W.Cast();
            }
        }
    }
}