using Dark_Star_Thresh.Core;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using System;

using TargetSelector = PortAIO.TSManager; namespace Dark_Star_Thresh.Update
{
    class Misc : Core.Core
    {
        public static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!MenuConfig.Interrupt || sender.IsInvulnerable) return;

            if (sender.LSIsValidTarget(Spells.E.Range))
            {
                if (Spells.E.IsReady())
                {
                    Spells.E.Cast(sender);
                }
            }
        }

        public static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!MenuConfig.Gapcloser) return;

            var sender = gapcloser.Sender;
            if (sender.IsEnemy && Spells.E.IsReady() && sender.LSIsValidTarget())
            {
                if (sender.LSIsValidTarget(Spells.E.Range))
                {
                    Spells.E.Cast(sender);
                }
            }
        }
    }
}
