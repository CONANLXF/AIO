using System;
using System.Linq;
using ExorAIO.Utilities;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using Geometry = ExorAIO.Utilities.Geometry;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.MissFortune
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
            /// <summary>
            ///     The Q Extended Harass Logic.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     The Q Minion Harass Logic.
                /// </summary>
                if (GameObjects.Player.ManaPercent > 
                        ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.Q2Menu, "exlaneclear")))
                {
                    foreach (var minion 
                        in from minion
                        in Targets.Minions.Where(
                            m =>
                                m.LSIsValidTarget(Vars.Q.Range) &&
                                Vars.getCheckBoxItem(Vars.Q2Menu, "exlaneclearkill")
                                    ? m.Health <
                                        (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)
                                    : true)

                        let polygon = new Geometry.Sector(
                            (Vector2)minion.ServerPosition,
                            (Vector2)minion.ServerPosition.LSExtend(GameObjects.Player.ServerPosition, -(Vars.Q2.Range - Vars.Q.Range)),
                            40f * (float)Math.PI / 180f,
                            (Vars.Q2.Range - Vars.Q.Range)-50f)

                        let target = GameObjects.EnemyHeroes.FirstOrDefault(
                            t =>
                                !Invulnerable.Check(t) &&
                                t.LSIsValidTarget(Vars.Q2.Range-50f) &&
                                ((Vars.PassiveTarget.LSIsValidTarget() &&
                                    t.NetworkId == Vars.PassiveTarget.NetworkId) ||
                                    !Targets.Minions.Any(m => !polygon.IsOutside((Vector2)m.ServerPosition))) &&
                                    Vars.getCheckBoxItem(Vars.WhiteListMenu, t.ChampionName.ToLower()))

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
				}
            }

            /// <summary>
            ///     The W Clear Logics.
            /// </summary>
            if (Vars.W.IsReady())
            {
                /// <summary>
                ///     The W JungleClear Logics.
                /// </summary>
                if (Targets.JungleMinions.Contains(PortAIO.OrbwalkerManager.LastTarget() as Obj_AI_Minion) &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "jungleclear")))
                {
                    Vars.W.Cast();
                }

                /// <summary>
                ///     The W LaneClear Logics.
                /// </summary>
                else if (Targets.Minions.Contains(PortAIO.OrbwalkerManager.LastTarget() as Obj_AI_Minion) &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "laneclear")))
                {
                    Vars.W.Cast();
                }
            }

            /// <summary>
            ///     The E Clear Logics.
            /// </summary>
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     The E JungleClear Logics.
                /// </summary>
                if (Targets.JungleMinions.Any() &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "jungleclear")))
                {
                    Vars.E.Cast(Targets.JungleMinions[0]);
                }

                /// <summary>
                ///     The E LaneClear Logics.
                /// </summary>
                else if (Targets.Minions.Any() &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "laneclear")) &&
                        Vars.E.GetCircularFarmLocation(Targets.Minions, Vars.E.Width).MinionsHit >= 4)
                {
                    Vars.E.Cast(Vars.E.GetCircularFarmLocation(Targets.Minions, Vars.E.Width).Position);
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
			/// <summary>
			///     The JungleClear Q Logic.
			/// </summary>
			if (Vars.Q.IsReady() &&
				Targets.JungleMinions.Any(m => m.LSIsValidTarget(Vars.Q.Range)) &&
				GameObjects.Player.ManaPercent >
					ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "jungleclear")))
            {
                Vars.Q.CastOnUnit(Targets.JungleMinions[0]);
				return;
			}
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void BuildingClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(PortAIO.OrbwalkerManager.LastTarget() is Obj_HQ) &&
                !(PortAIO.OrbwalkerManager.LastTarget() is Obj_AI_Turret) &&
                !(PortAIO.OrbwalkerManager.LastTarget() is Obj_BarracksDampener))
            {
                return;
            }

            /// <summary>
            ///     The W BuildingClear Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "buildings")))
            {
                Vars.W.Cast();
            }
        }
    }
}