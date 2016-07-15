using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace Preserved_Kassadin.Cores
{
    class Coree
    {
        public static AIHeroClient Player => ObjectManager.Player;
        public static AIHeroClient Target => TargetSelector.GetTarget(1400, DamageType.Magical);

        public static bool SafeTarget(Obj_AI_Base target)
        {
            return target != null && !target.IsDead && !target.IsInvulnerable && target.IsVisible && target.IsHPBarRendered;
        }
    }
}
