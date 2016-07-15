using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firestorm_AIO.Helpers;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace Firestorm_AIO.Champions.Rumble
{
    public class Rumble :Bases.ChampionBase
    {
        public override void Init()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 950);
            R = new Spell(SpellSlot.R, 1200);

            E.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.425f, 200f, 1600f, true, SkillshotType.SkillshotLine);
        }

        public override void Menu()
        {
            Q.CreateBool(ComboMenu);
            E.CreateBool(ComboMenu);
            R.CreateBool(ComboMenu);

            Q.CreateBool(MixedMenu);
            E.CreateBool(MixedMenu);

            Q.CreateBool(LaneClearMenu);
            E.CreateBool(LaneClearMenu);

            Q.CreateBool(LastHitMenu);
            E.CreateBool(LastHitMenu);

            Q.CreateBool(JungleClearMenu);
            E.CreateBool(JungleClearMenu);
        }

        public override void Active()
        {
            
        }

        public override void Combo()
        {
            
        }

        public override void Mixed()
        {
            
        }

        public override void LaneClear()
        {
            
        }

        public override void LastHit()
        {
            
        }

        public override void KillSteal()
        {
            
        }

        public override void Draw()
        {
            
        }
    }
}
