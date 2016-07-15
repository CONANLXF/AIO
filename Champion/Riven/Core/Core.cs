using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.Common;

 namespace NechritoRiven.Core
{
    internal partial class Core
    {
        public static AttackableUnit qTarget;

        public const string IsFirstR = "RivenFengShuiEngine";
        public const string IsSecondR = "RivenIzunaBlade";

        public static int Qstack = 1;

        public static AIHeroClient Player => ObjectManager.Player;
        public static AIHeroClient Target => TargetSelector.GetTarget(250 + Player.AttackRange + 70, DamageType.Physical);
    }
}
