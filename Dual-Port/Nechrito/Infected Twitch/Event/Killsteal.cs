#region

using System;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using LeagueSharp.SDK;
using EloBuddy.SDK;
using EloBuddy;

#endregion

 namespace Infected_Twitch.Event
{
    internal class Killsteal : Core.Core
    {
        public static void Update(EventArgs args)
        {
            if (!SafeTarget(Target)) return;

            if (Target.HealthPercent <= 10 && !Spells.Q.IsReady())
            {
                Usables.Botrk();
            }

            if (!MenuConfig.KillstealIgnite) return;
            if (!Spells.Ignite.IsReady()) return;

            var target = TargetSelector.GetTarget(600f, DamageType.Physical);
            if (target == null || !target.LSIsValidTarget(600f) || Spells.E.IsReady()) return;

            if (target.IsValidTarget(600f) && Dmg.IgniteDmg >= target.Health)
            {
                GameObjects.Player.Spellbook.CastSpell(Spells.Ignite, target);
            }
        }
    }
}
