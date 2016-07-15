using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyHarass
    {
        public static void BadaoActiavate()
        {
            Game.OnUpdate += Game_OnUpdate;
            LSEvents.AfterAttack += Orbwalking_AfterAttack;
        }
        
        private static void Game_OnUpdate(EventArgs args)
        {
            if (!PortAIO.OrbwalkerManager.isHarassActive)
                return;
            if (BadaoPoppyHelper.UseQHarass())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, DamageType.Physical);
                if (target.BadaoIsValidTarget())
                {
                    if (BadaoMainVariables.Q.Cast(target) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        BadaoPoppyVariables.QCastTick = Environment.TickCount;
                }
            }
        }

        private static void Orbwalking_AfterAttack(AfterAttackArgs args)
        {
            if (!PortAIO.OrbwalkerManager.isHarassActive || !(args.Target is AIHeroClient))
                return;
            if (args.Target.Position.LSDistance(ObjectManager.Player.Position) <= 200 + 125 + 140)
                BadaoChecker.BadaoUseTiamat();
        }
    }
}
