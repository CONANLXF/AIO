using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK;
using Swiftly_Teemo.Main;
using SharpDX;
using System;
using System.Linq;

using TargetSelector = PortAIO.TSManager; namespace Swiftly_Teemo.Handler
{
    internal class AfterAa : Core
    {
        
        public static void Orbwalker_OnPostAttack(LeagueSharp.Common.AfterAttackArgs args)
        {
            var dgfg = args.Target;
            if (dgfg is AIHeroClient)
            {
                var Target = dgfg as AIHeroClient;

                if (PortAIO.OrbwalkerManager.isComboActive || PortAIO.OrbwalkerManager.isHarassActive)
                {
                    if (Target == null || Target.IsDead || Target.IsInvulnerable || !Target.LSIsValidTarget(Spells.Q.Range)) return;
                    {
                        if (MenuConfig.TowerCheck && Target.IsUnderEnemyTurret()) return;
                        if (Spells.Q.IsReady())
                        {
                            Spells.Q.Cast(Target);
                        }
                    }

                    if (PortAIO.OrbwalkerManager.isLaneClearActive) return;

                    var mob = GameObjects.Jungle.Where(m => m != null && m.LSIsValidTarget(Player.AttackRange) && !GameObjects.JungleSmall.Contains(m));

                    foreach (var m in mob)
                    {
                        if (Spells.Q.IsReady())
                        {
                            Spells.Q.Cast(m);
                        }
                    }
                }
            }
        }
    }
}