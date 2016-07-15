using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using Geometry = ExorAIO.Utilities.Geometry;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Sivir
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void Clear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Clear W Logic.
            /// </summary>
            if (Vars.W.IsReady() && GameObjects.Player.ManaPercent > ManaManager.GetNeededMana(Vars.W.Slot, Menus.getSliderItem(Vars.WMenu, "clear")) && Menus.getSliderItem(Vars.WMenu, "clear") != 101)
            {
                /// <summary>
                ///     The LaneClear W Logic.
                /// </summary>
                if (Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3)
                {
                    Vars.W.Cast();
                }

                /// <summary>
                ///     The JungleClear W Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.W.Cast();
                }
                return;
            }

            /// <summary>
            ///     The Clear Q Logics.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Menus.getSliderItem(Vars.QMenu, "clear")) && Menus.getSliderItem(Vars.QMenu, "clear") != 101)
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
                        if (Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3 &&
                            !new Geometry.Rectangle(
                                GameObjects.Player.ServerPosition,
                                GameObjects.Player.ServerPosition.LSExtend(
                                    Targets.Minions[0].ServerPosition, Vars.Q.Range),
                                    Vars.Q.Width).IsOutside((Vector2)Vars.Q.GetPrediction(
                                        GameObjects.EnemyHeroes.FirstOrDefault(
                                            t =>
                                                !Invulnerable.Check(t) &&
                                                t.LSIsValidTarget(Vars.Q.Range))).UnitPosition))
                        {
                            Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).Position);
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
            if (Vars.W.IsReady() && GameObjects.Player.ManaPercent > ManaManager.GetNeededMana(Vars.W.Slot, Menus.getSliderItem(Vars.WMenu, "buildings")) && Menus.getSliderItem(Vars.WMenu, "buildings") != 101)
            {
                Vars.W.Cast();
            }
        }
    }
}