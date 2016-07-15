#region

using System;
using System.Linq;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;
using EloBuddy.SDK;
using EloBuddy;

#endregion

 namespace NechritoRiven.Event
{
    internal class KillSteal : Core.Core
    {
        public static void Update(EventArgs args)
        {
            if (Spells.Q.IsReady())
            {
                var T = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells.R.Range) && !x.IsZombie);
                foreach (var target in T)
                {
                    if (target.Health < Spells.Q.GetDamage(target) && InQRange(target))
                        Spells.Q.Cast(target);
                }
            }
            if (Spells.W.IsReady())
            {
                var T = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells.R.Range) && !x.IsZombie);
                foreach (var target in T)
                {
                    if (target.Health < Spells.W.GetDamage(target) && InWRange(target))
                        Spells.W.Cast();
                }
            }
            if (Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR)
            {
                var T = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells.R.Range) && !x.IsZombie);
                foreach (var target in T)
                {
                    if (target.Health < Dmg.Rdame(target, target.Health) && !target.HasBuff("kindrednodeathbuff") &&
                        !target.HasBuff("Undying Rage") && !target.HasBuff("JudicatorIntervention"))
                        Spells.R.Cast(target.Position);
                }
            }
            if (Spells.Ignite.IsReady() && MenuConfig.ignite)
            {
                var target = TargetSelector.GetTarget(600f, DamageType.True);
                if (target.LSIsValidTarget(600f) && Dmg.IgniteDamage(target) >= target.Health)
                {
                    Player.Spellbook.CastSpell(Spells.Ignite, target);
                }
            }
        }
    }
}
