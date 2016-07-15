using LeagueSharp.SDK;
using EloBuddy.SDK;
using EloBuddy;
using System;

using TargetSelector = PortAIO.TSManager; namespace Arcane_Ryze.Modes
{
    class BeforeAA : Core
    {
        
        public static void OnAction(LeagueSharp.Common.AfterAttackArgs args)
        {
            var fdf = args.Target;
            if (fdf is AIHeroClient)
            {
                var target = fdf as AIHeroClient;

                if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                if(Spells.Q.IsReady() && target.LSIsValidTarget() && !target.IsZombie && PassiveStack < 4)
                {
                    Spells.Q.Cast(target);  
                }
            }
        }
    }
  }
}
