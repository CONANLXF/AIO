using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;

using TargetSelector = PortAIO.TSManager; namespace BLeblanc
{
    public static class TwoChains
    {
        public static Spell R2 = new Spell(SpellSlot.R, 950);
        public static int LastChain = 0;
        public static int LastChainM = 0;
        public static void TwoChainsActive(AIHeroClient target)
        {
            //string t = "";
            //if (target.IsValidTarget())
            //{
            //    foreach (var x in target.Buffs)
            //    {
            //        t += ", " + x.Name;
            //    }
            //    Game.PrintChat(t);
            //}

            R2.SetSkillshot(0.25f, 70, 1600, true, SkillshotType.SkillshotLine);
            PortAIO.OrbwalkerManager.MoveA(Game.CursorPos);
            if (target != null && target.LSIsValidTarget(Program.Q.Range) && (Program.Rstate != 3 || !R2.IsReady()))
            {
                Program.Q.Cast(target);
            }
            if (target != null && target.LSIsValidTarget(Program.E.Range) && Program.E.IsReady() && !ObjectManager.Player.LSIsDashing()
                 && !target.HasBuff("LeblancSoulShackleM") && Environment.TickCount - LastChainM >= 1500 + Game.Ping)
            {
                Program.E.Cast(target);
                LastChain = Environment.TickCount;
            }
            if (target != null && target.LSIsValidTarget(Program.E.Range) && Program.R.IsReady() && !ObjectManager.Player.LSIsDashing() && Program.Rstate == 3
                && !target.HasBuff("LeblancSoulShackle") && Environment.TickCount - LastChain >= 1500 + Game.Ping)
            {
                R2.Cast(target);
                LastChainM = Environment.TickCount;
            }
            if (target.LSIsValidTarget(Program.W.Range) && Program.getCheckBoxItem(Program.spellMenu, "Wcombo") && (Program.Rstate != 3 || !R2.IsReady()))
            {
                Program.CastW(target);
            }
        }
    }
}
