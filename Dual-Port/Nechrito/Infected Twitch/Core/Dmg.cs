#region

using System;
using Infected_Twitch.Menus;
using EloBuddy;
using LeagueSharp.SDK;

#endregion

 namespace Infected_Twitch.Core
{
    internal class Dmg : Core
    {
        public static int IgniteDmg = 50 + 20 * GameObjects.Player.Level;


        public static float EDamage(Obj_AI_Base target)
        {
            if (!Spells.E.IsReady()) return 0;
            if (!SafeTarget(target)) return 0;

            float eDmg = 0;

            if (MenuConfig.Eaaq)
            {
                eDmg = eDmg + ERaw(target) * Stacks(target);
            }
            else
            {
                eDmg = eDmg + ERaw(target) + Passive(target);
            }

            if (GameObjects.Player.HasBuff("SummonerExhaust")) eDmg = eDmg * 0.6f;

            return eDmg;
        }

        public static float ERaw(Obj_AI_Base target)
        {
            return (float)GameObjects.Player.CalculateDamage(target, DamageType.True,
                EStackDamage[Spells.E.Level - 1] * Stacks(target) +
                0.2 * GameObjects.Player.FlatMagicDamageMod +
                0.25 * GameObjects.Player.FlatPhysicalDamageMod +
                EBaseDamage[Spells.E.Level - 1]);
        }

        private static float Passive(Obj_AI_Base target)
        {
            float dmg = 6;

            if (GameObjects.Player.Level > 16) dmg = 6;
            if (GameObjects.Player.Level > 12) dmg = 5;
            if (GameObjects.Player.Level > 8) dmg = 4;
            if (GameObjects.Player.Level > 4) dmg = 3;
            if (GameObjects.Player.Level > 0) dmg = 2;

            return dmg * Stacks(target) * PassiveTime(target) - target.HPRegenRate * PassiveTime(target);
        }

        private static float PassiveTime(Obj_AI_Base target)
        {
            if (!target.HasBuff("twitchdeadlyvenom")) return 0;

            return Math.Max(0, target.GetBuff("twitchdeadlyvenom").EndTime) - Game.Time;
        }

        public static float Stacks(Obj_AI_Base target)
        {
            return target.GetBuffCount("TwitchDeadlyVenom");
        }

        private static readonly float[] EStackDamage = { 15, 20, 25, 30, 35 };

        private static readonly float[] EBaseDamage = { 20, 35, 50, 65, 80 };
    }
}