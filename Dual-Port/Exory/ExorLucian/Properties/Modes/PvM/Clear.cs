using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using Geometry = ExorAIO.Utilities.Geometry;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Lucian
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
            ///     The Q Harass & Clear Logics.
            /// </summary>
            if (Vars.Q.IsReady())
            {
				if (!GameObjects.EnemyHeroes.Any(
					t =>
						!Invulnerable.Check(t) &&
						!t.LSIsValidTarget(Vars.Q.Range) &&
						t.LSIsValidTarget(Vars.Q2.Range-50f) &&
						Vars.getCheckBoxItem(Vars.WhiteListMenu, t.ChampionName.ToLower())))
				{
					/// <summary>
					///     The LaneClear Q Logic.
					/// </summary>
					if (Targets.Minions.Any() &&
						GameObjects.Player.ManaPercent >
							ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "laneclear")) &&
                        Vars.getSliderItem(Vars.QMenu, "laneclear") != 101)
					{
						if (Vars.Q2.GetLineFarmLocation(Targets.Minions, Vars.Q2.Width).MinionsHit >= 3)
						{
							Vars.Q.CastOnUnit(Targets.Minions[0]);
						}
					}
				}
				else
				{
					/// <summary>
					///     The Q Minion Harass Logic.
					/// </summary>
					if (GameObjects.Player.ManaPercent > 
							ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.Q2Menu, "exlaneclear")) &&
                        Vars.getSliderItem(Vars.Q2Menu, "exlaneclear") != 101)
					{
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
									Vars.getCheckBoxItem(Vars.WhiteListMenu, t.ChampionName.ToLower()))).UnitPosition)

							select minion)
						{
							Vars.Q.CastOnUnit(minion);
						}
					}
				}
            }

            /// <summary>
            ///     The LaneClear W Logic.
            /// </summary>
            if (Vars.W.IsReady()&&
				Targets.Minions.Any() &&
				GameObjects.Player.ManaPercent >
					ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "laneclear")) &&
                Vars.getSliderItem(Vars.WMenu, "laneclear") != 101)
			{
				if (Vars.W.GetCircularFarmLocation(Targets.Minions, Vars.W.Width).MinionsHit >= 2)
				{
					Vars.W.Cast(Vars.W.GetCircularFarmLocation(Targets.Minions, Vars.W.Width).Position);
				}
			}

            /// <summary>
            ///     The E LaneClear Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "laneclear")) &&
                Vars.getSliderItem(Vars.EMenu, "laneclear") != 101)
            {
                if (!Targets.Minions.Any(m => m.LSIsValidTarget(Vars.AARange)) &&
					Targets.Minions.Any(m => m.Distance(GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, Vars.E.Range)) < Vars.AARange))
                {
                    Vars.E.Cast(Game.CursorPos);
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
            if (Orbwalker.LastTarget as Obj_AI_Minion == null)
            {
                return;
            }

            /// <summary>
            ///     The JungleClear E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.JungleMinions.Any() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "jungleclear")) &&
                Vars.getSliderItem(Vars.EMenu, "jungleclear") != 101)
            {
                Vars.E.Cast(GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, 50));
				return;
            }

			/// <summary>
			///     The JungleClear Q Logic.
			/// </summary>
			if (Vars.Q.IsReady() &&
				Targets.JungleMinions.Any() &&
				GameObjects.Player.ManaPercent >
					ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "jungleclear")) &&
                Vars.getSliderItem(Vars.QMenu, "jungleclear") != 101)
			{
				Vars.Q.CastOnUnit(Orbwalker.LastTarget as Obj_AI_Minion);
				return;
			}

			/// <summary>
			///     The JungleClear W Logic.
			/// </summary>
			if (Vars.W.IsReady() &&
				Targets.JungleMinions.Any() &&
				GameObjects.Player.ManaPercent >
					ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "jungleclear")) &&
                Vars.getSliderItem(Vars.WMenu, "jungleclear") != 101)
			{
				Vars.W.Cast((Orbwalker.LastTarget as Obj_AI_Minion).ServerPosition);
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
            ///     The E BuildingClear Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "buildings")) &&
                Vars.getSliderItem(Vars.EMenu, "buildings") != 101)
            {
                Vars.E.Cast(GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, 25));
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
                Vars.W.Cast(Game.CursorPos);
            }
        }
    }
}