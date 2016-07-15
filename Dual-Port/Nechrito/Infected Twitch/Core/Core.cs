#region

using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using EloBuddy.SDK;

#endregion

 namespace Infected_Twitch.Core
{
    internal class Core
    {
        public static bool HasPassive => Player.HasBuff("TwitchHideInShadows");
        public static AIHeroClient Player => ObjectManager.Player;
        public static AIHeroClient Target => TargetSelector.GetTarget(1200, DamageType.Physical);

        public static bool SafeTarget(Obj_AI_Base target)
        {
            return target != null && target.LSIsValidTarget() && !target.IsDead && !target.IsInvulnerable && !target.HasBuff("KindredRNoDeathBuff") && !target.HasBuffOfType(BuffType.SpellShield);
        }
    }
}
