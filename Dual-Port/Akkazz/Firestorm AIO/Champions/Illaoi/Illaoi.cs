using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firestorm_AIO.Helpers;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using static Firestorm_AIO.Helpers.Helpers;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace Firestorm_AIO.Champions.Illaoi
{
    public class Illaoi : Bases.ChampionBase
    {
        public override void Init()
        {
            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 850);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 350);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 950);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 500);

            Q.SetSkillshot(0.75f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 50f, 1900f, true, SkillshotType.SkillshotLine);

            LeagueSharp.Common.LSEvents.AfterAttack += Orbwalker_OnPostAttack;
        }

        private void Orbwalker_OnPostAttack(LeagueSharp.Common.AfterAttackArgs args)
        {
            var Target = args.Target;
            //AA Cancel
            if (Target != null && Target.LSIsValidTarget(Me.GetRealAutoAttackRange()))
            {
                W.Cast();
            }
        }

        public override void Menu()
        {
            Q.CreateBool(ComboMenu);
            W.CreateBool(ComboMenu);
            E.CreateBool(ComboMenu);
            R.CreateBool(ComboMenu);

            Q.CreateBool(MixedMenu);

            Q.CreateBool(LaneClearMenu);

            Q.CreateBool(LastHitMenu);

            Q.CreateBool(JungleClearMenu);
        }

        public override void Active()
        {
            Target = TargetSelector.GetTarget(950, DamageType.Physical);
        }

        public override void Combo()
        {
            if (GetBoolValue(Q, ComboMenu))
            {
                Q.SmartCast(Target);
            }

            if (GetBoolValue(E, ComboMenu))
            {
                E.SmartCast(Target, HitChance.High);
            }

            if (GetBoolValue(W, ComboMenu))
            {
                if (!Target.LSIsInAARange())
                {
                    W.Cast();
                }
            }

            var countR =
                GameObjects.EnemyHeroes.Count(e => Movement.GetPrediction(e, 500f).UnitPosition.LSIsInRange(Me, R.Range));
             
        }

        public override void Mixed()
        {
            if (GetBoolValue(Q, MixedMenu))
            {
                Q.SmartCast(Target);
            }
        }

        public override void LaneClear()
        {
            if (GetBoolValue(Q, LaneClearMenu))
            {
                Q.SmartCast(Target);
            }

            //JungleClear

            if (GetBoolValue(Q, JungleClearMenu))
            {
                Q.SmartCast(Q.GetBestJungleClearMinion());
            }
        }

        public override void LastHit()
        {
            //TODO GET Last hit target
            if (GetBoolValue(Q, LastHitMenu))
            {
                Q.SmartCast(Target);
            }
        }

        public override void KillSteal()
        {
        }

        public override void Draw()
        {
        }
    }
}
