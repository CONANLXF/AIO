using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

 namespace ThreshWarden
{
	public static class Extensions
    {

		public static float GetPassiveTime(this Obj_AI_Base target, String buffName)
        {
			return
				target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
					.Where(buff => buff.Name == buffName)
					.Select(buff => buff.EndTime)
					.FirstOrDefault() - Game.Time;
		}

		public static Obj_AI_Turret GetMostCloseTower(this Obj_AI_Base target)
        {
			Obj_AI_Turret tur = null;

			if (target.IsDead) return null;

			foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(t =>
				t.IsValid && !t.IsDead && t.Health > 1f && t.IsVisible && t.LSDistance(target) < 1000))
			{
				if (turret != null)
				{
					if (tur == null)
					{
						tur = turret;
					}
					else if (tur != null && tur.LSDistance(target) > turret.LSDistance(target))
					{

						tur = turret;
					}
				}
			}
			return tur;
		}



		public static bool IsInTurret(this Obj_AI_Base targetHero, Obj_AI_Turret targetTurret = null)
        {
			

			if (targetTurret == null)
			{
				targetTurret = targetHero.GetMostCloseTower();
			}
			if (targetTurret != null && targetHero.LSDistance(targetTurret) < 850)
			{
				return true;
			}
			return false;
		}

		public static bool CastToReverse(this LeagueSharp.Common.Spell spell, Obj_AI_Base target) {
			var eCastPosition = spell.GetPrediction(target).CastPosition;
			var position = ThreshWarden.Player.ServerPosition + ThreshWarden.Player.ServerPosition - eCastPosition;
			return spell.Cast(position);
		}

		public static bool IsFleeing(this AIHeroClient hero, Obj_AI_Base target) {
			if (hero == null || target == null)
			{
				return false;
			}

			if (hero.Path.Count()>0 && target.LSDistance(hero.Position) < target.LSDistance(hero.Path.Last()))
			{
				return true;
			}
			return false;
		}

		public static bool IsHunting(this AIHeroClient hero, Obj_AI_Base target) {
			if (target == null)
			{
				return false;
			}
			if (target.Path.Count() > 0 && hero.LSDistance(target.Position) > hero.LSDistance(target.Path.Last()))
			{
				return true;
			}
			return false;
		}

		public static int CountEnemiesInRangeDeley(this AIHeroClient hero, float range, float delay) {
			int count = 0;
			foreach (var t in HeroManager.Enemies.Where(t => t.LSIsValidTarget()))
			{
				Vector3 prepos = LeagueSharp.Common.Prediction.GetPrediction(t, delay).CastPosition;
				if (hero.LSDistance(prepos) < range)
					count++;
			}
			return count;
		}

	}
}
