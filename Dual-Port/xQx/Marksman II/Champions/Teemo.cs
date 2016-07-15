#region

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

#endregion

 namespace Marksman.Champions
{
    internal class Teemo : Champion
    {
        public LeagueSharp.Common.Spell Q;
        public LeagueSharp.Common.Spell R;
        private int LastRCast;
        public Teemo()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 680);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 230);
            Q.SetTargetted(0f, 2000f);
            R.SetSkillshot(0.1f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                if (R.IsReady())
                {
                    BuffInstance aBuff =
                        (from fBuffs in
                             sender.Buffs.Where(
                                 s =>
                                 sender.Team != ObjectManager.Player.Team
                                 && sender.LSDistance(ObjectManager.Player.Position) < R.Range)
                         from b in new[]
                                           {
                                               "teleport", /* Teleport */ "pantheon_grandskyfall_jump", /* Pantheon */ 
                                               "crowstorm", /* FiddleScitck */
                                               "zhonya", "katarinar", /* Katarita */
                                               "MissFortuneBulletTime", /* MissFortune */
                                               "gate", /* Twisted Fate */
                                               "chronorevive" /* Zilean */
                                           }
                         where args.Buff.Name.ToLower().Contains(b)
                         select fBuffs).FirstOrDefault();

                    if (aBuff != null)
                    {
                        R.Cast(sender.Position);
                    }
                }
            };

            Utils.Utils.PrintMessage("Teemo loaded.");
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t != null && (ComboActive || HarassActive))
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;

                if (useQ && Q.IsReady())
                    Q.CastOnUnit(t);
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            LeagueSharp.Common.Spell[] spellList = { Q };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 0, 255));
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {

            //var lee = HeroManager.Allies.Find(l => l.ChampionName.ToLower() == "leesin");
            //if (lee != null && !lee.IsDead)
            //{
            //    if (lee.Distance(ObjectManager.Player.Position) > 250 && !ObjectManager.Player.IsRecalling() &&
            //        Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            //    {
            //        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, lee.Position);
            //    }
            //}

            R.Range = 150 + (R.Level * 250);

            if (Q.IsReady() && Program.harass["UseQTH"].Cast<KeyBind>().CurrentValue && ToggleActive)
            {
                if (ObjectManager.Player.HasBuff("Recall"))
                    return;
                var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (Q.IsReady() && qTarget.LSIsValidTarget())
                    Q.CastOnUnit(qTarget);
            }

            if (ComboActive || HarassActive)
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
                if (useQ)
                {
                    var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                    if (Q.IsReady() && qTarget.LSIsValidTarget())
                        Q.CastOnUnit(qTarget);
                }
            }

            if (R.IsReady() && ComboActive)
            {
                foreach (var t in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(R.Range) && !hero.IsDead))
                {
                    if (Program.combo["UseRC"].Cast<CheckBox>().CurrentValue && LeagueSharp.Common.Utils.TickCount > LastRCast + 1200)
                    {
                        if (t.HealthPercent > ObjectManager.Player.HealthPercent)
                        //if (t.HealthPercent > ObjectManager.Player.HealthPercent && t.IsFacing(ObjectManager.Player))
                        {
                            R.Cast(ObjectManager.Player, false, true);
                        }
                        else
                        {
                            R.Cast(t, false, true);
                        }
                        LastRCast = LeagueSharp.Common.Utils.TickCount;
                    }

                    if (Program.misc["AutoRI"].Cast<CheckBox>().CurrentValue)
                    {
                        if (t.LSIsValidTarget(R.Range) &&
                            (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                             t.HasBuffOfType(BuffType.Taunt) || t.HasBuff("zhonyasringshield") ||
                             t.HasBuff("Recall")))
                        {
                            R.Cast(t.Position);
                        }
                    }
                }
            }

            if (LaneClearActive && Q.IsReady() && Program.laneclear["UseQL"].Cast<CheckBox>().CurrentValue)
            {
                var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                foreach (var minions in vMinions.Where(minions => minions.Health < Q.GetDamage(minions) - 10))
                {
                    Q.Cast(minions);
                }
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseRC", new CheckBox("Use R", false));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q", false));
            config.Add("UseQTH", new KeyBind("Use Q (Toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("UseQM", new CheckBox("Use Q KS"));
            config.Add("AutoRI", new CheckBox("R: Stun/Snare/Taunt/Zhonya"));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            config.Add("UseQL", new CheckBox("Use Q"));
            return true;
        }
        public override bool JungleClearMenu(Menu config)
        {
            return true;
        }
    }
}