#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Common;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

#endregion

 namespace Marksman.Champions
{
    internal class Kogmaw : Champion
    {
        public static LeagueSharp.Common.Spell Q;
        public static LeagueSharp.Common.Spell W;
        public static LeagueSharp.Common.Spell E;
        public static LeagueSharp.Common.Spell R;
        public static bool bioArcaneActive = false;
        public int UltiBuffStacks;

        public Kogmaw()
        {
            Utils.Utils.PrintMessage("KogMaw loaded.");

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1175f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, float.MaxValue);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1280f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, float.MaxValue);

            Q.SetSkillshot(0.25f, 70f, 1650f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.50f, 120f, 1350, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.2f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            LeagueSharp.Common.Spell[] spellList = { Q, W, E, R };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position,
                        spell.Slot == SpellSlot.W
                            ? Orbwalking.GetRealAutoAttackRange(null) + 65 + W.Range
                            : spell.Range,
                        Color.FromArgb(100, 255, 0, 255));
            }
        }

        public override void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name.ToLower() == "kogmawbioarcanebarrage")
            {
                bioArcaneActive = true;
            }
        }

        public override void Obj_AI_Base_OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name.ToLower() == "kogmawbioarcanebarrage")
            {
                bioArcaneActive = false;
            }

        }

        private static float GetRealAARange
        {
            get
            {
                return Orbwalking.GetRealAutoAttackRange(null) + 65 + (bioArcaneActive ? W.Range : 0);
            }
        }

        public override void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!W.IsReady())
            {
                return;
            }

            if (Program.misc["Misc.UseW.Inhibitor"].Cast<CheckBox>().CurrentValue && args.Target is Obj_BarracksDampener)
            {
                W.Cast();
            }

            if (Program.misc["Misc.UseW.Nexus"].Cast<CheckBox>().CurrentValue && args.Target is Obj_HQ)
            {
                W.Cast();
            }

            if (Program.misc["Misc.UseW.Turret"].Cast<CheckBox>().CurrentValue && args.Target is Obj_AI_Turret)
            {
                W.Cast();
            }
        }


        private static void CastQ(AIHeroClient t)
        {
            var nPrediction = Q.GetPrediction(t);
            var nHitPosition = nPrediction.CastPosition.LSExtend(ObjectManager.Player.Position, -140);
            if (nPrediction.Hitchance >= Q.GetHitchance())
                Q.Cast(nHitPosition);
        }

        private static void CastE(Obj_AI_Base t)
        {
            var nPrediction = E.GetPrediction(t);
            var nHitPosition = nPrediction.CastPosition.LSExtend(ObjectManager.Player.Position, -140);
            if (nPrediction.Hitchance >= E.GetHitchance())
                E.Cast(nHitPosition);
        }

        private static void CastR(Obj_AI_Base t)
        {
            var nPrediction = E.GetPrediction(t);
            var nHitPosition = nPrediction.CastPosition.LSExtend(ObjectManager.Player.Position, -140);
            if (nPrediction.Hitchance >= R.GetHitchance())
                R.Cast(nHitPosition);
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            UltiBuffStacks = GetUltimateBuffStacks();

            W.Range = 110 + 20 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
            R.Range = 900 + 300 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level;

            if (R.IsReady() && Program.misc["UseRM"].Cast<CheckBox>().CurrentValue)
                foreach (
                    var hero in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                hero => hero.LSIsValidTarget(R.Range) && R.GetDamage(hero) > hero.Health))
                    CastR(hero);
            //R.Cast(hero, false, true);

            if ((!ComboActive && !HarassActive) || (!Orbwalker.CanMove &&
                 !(ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod > 100)))
            {
                return;
            }

            var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
            var useE = ComboActive ? Program.combo["UseEC"].Cast<CheckBox>().CurrentValue : Program.harass["UseEH"].Cast<CheckBox>().CurrentValue;
            var useR = ComboActive ? Program.combo["UseRC"].Cast<CheckBox>().CurrentValue : Program.harass["UseRH"].Cast<CheckBox>().CurrentValue;
            var rLim = ComboActive ? Program.combo["RlimC"].Cast<Slider>().CurrentValue : Program.harass["RlimH"].Cast<Slider>().CurrentValue;

            var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (useQ && Q.IsReady() && t.LSIsValidTarget(Q.Range))
            {
                CastQ(t);
            }

            if (useE && E.IsReady() && t.LSIsValidTarget(E.Range))
            {
                CastE(t);
            }
            if (R.IsReady() && t.LSIsValidTarget(R.Range))
            //if (GetValue<bool>("UseRSC") && R.IsReady() && t.LSIsValidTarget(R.Range))
            {
                if (t.LSIsValidTarget() &&
                    (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Slow) ||
                     t.HasBuffOfType(BuffType.Fear) ||
                     t.HasBuffOfType(BuffType.Taunt)))
                {
                    CastR(t);
                    //R.Cast(t, false, true);
                }
            }

            if (useR && R.IsReady() && UltiBuffStacks < rLim && t.LSIsValidTarget(R.Range))
            {
                CastR(t);
                //R.Cast(t, false, true);
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit targetA, EventArgs args)
        {
            var target = targetA;
            if (target != null && (!ComboActive && !HarassActive) || !(target is AIHeroClient))
            {
                return;
            }

            var t = (AIHeroClient)target;
            var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;
            var useE = ComboActive ? Program.combo["UseEC"].Cast<CheckBox>().CurrentValue : Program.harass["UseEH"].Cast<CheckBox>().CurrentValue;

            if (useW && W.IsReady())
                W.Cast();


            if (useE && E.IsReady())
            {
                CastE(t);
            }
            //if (E.Cast(t, false, true) == Spell.CastStates.SuccessfullyCasted)
            //  return;

        }

        public override void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var nClosesTarget = HeroManager.Enemies.Find(e => e.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65));
            {
                if (nClosesTarget != null)
                {
                    if (Program.combo["UseWMC"].Cast<CheckBox>().CurrentValue &&
                        ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level > 0 &&
                        Math.Abs(W.Cooldown) < 0.00001 &&
                        args.Slot != SpellSlot.W)
                    {
                        var lastMana = ObjectManager.Player.Mana - ObjectManager.Player.Spellbook.GetSpell(args.Slot).SData.Mana;
                        if (lastMana < ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana)
                        {
                            args.Process = false;
                        }
                    }
                }
            }
        }

        private static int GetUltimateBuffStacks()
        {
            return (from buff in ObjectManager.Player.Buffs
                    where buff.DisplayName.ToLower() == "kogmawlivingartillery"
                    select buff.Count).FirstOrDefault();
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Q:"));
            config.Add("UseWC", new CheckBox("W:"));
            config.Add("UseWMC", new CheckBox("W: Protect mana for W"));
            config.Add("UseEC", new CheckBox("E:"));
            config.Add("UseRC", new CheckBox("R:"));
            config.Add("RlimC", new Slider("R Limiter", 3, 1, 5));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q", false));
            config.Add("UseWH", new CheckBox("Use W", false));
            config.Add("UseEH", new CheckBox("Use E", false));
            config.Add("UseRH", new CheckBox("Use R"));
            config.Add("RlimH", new Slider("R Limiter", 1, 1, 5));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawW", new CheckBox("W range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawE", new CheckBox("E range", false));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawR", new CheckBox("R range", false));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("UseRM", new CheckBox("Use R To Killsteal"));
            config.Add("Misc.UseW.Turret", new CheckBox("Use W for Turret", false));
            config.Add("Misc.UseW.Inhibitor", new CheckBox("Use W for Inhibitor"));
            config.Add("Misc.UseW.Nexus", new CheckBox("Use W for Nexus"));

            return true;
        }

        public override void ExecuteLaneClear()
        {
            List<Obj_AI_Base> laneMinions;

            var laneWValue = Program.laneclear["Lane.UseW"].Cast<ComboBox>().CurrentValue;

            if (laneWValue != 0 && W.IsReady())
            {
                var totalAa =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            m =>
                                m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65 + W.Range))
                        .Sum(mob => (int)mob.Health);

                totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                if (totalAa > laneWValue * 5)
                {
                    W.Cast();
                }
            }

            var laneQValue = Program.laneclear["Lane.UseQ"].Cast<ComboBox>().CurrentValue;

            if (laneQValue != 0 && W.IsReady())
            {
                if (laneQValue == 1 || laneQValue == 3)
                {
                    var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                    foreach (var minions in vMinions
                        .Where(minions => minions.Health < ObjectManager.Player.LSGetSpellDamage(minions, SpellSlot.Q))
                        .Where(
                            m =>
                                m.LSIsValidTarget(Q.Range) &&
                                m.LSDistance(ObjectManager.Player.Position) > GetRealAARange)
                        )
                    {
                        var qP = Q.GetPrediction(minions);
                        var hit = qP.CastPosition.LSExtend(ObjectManager.Player.Position, -140);
                        if (qP.Hitchance >= HitChance.High)
                        {
                            Q.Cast(hit);
                        }
                    }
                }
                if (laneQValue == 2 || laneQValue == 3)
                {
                    var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range * 1);

                    foreach (var n in from n in minions
                                      let xH =
                                          HealthPrediction.GetHealthPrediction(n,
                                              (int)(ObjectManager.Player.AttackCastDelay * 1000), Game.Ping / 2 + 100)
                                      where xH < 0
                                      where n.Health < Q.GetDamage(n)
                                      select n)
                    {
                        Q.Cast(n);
                    }
                }
            }

            var laneEValue = Program.laneclear["Lane.UseE"].Cast<ComboBox>().CurrentValue;
            if (laneEValue != 0 && E.IsReady())
            {
                laneMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range,
                    MinionTypes.All);

                if (laneMinions != null)
                {
                    var locE = E.GetLineFarmLocation(laneMinions);
                    if (laneMinions.Count == laneMinions.Count(m => ObjectManager.Player.LSDistance(m) < E.Range) &&
                        locE.MinionsHit > laneEValue && locE.Position.IsValid())
                    {
                        E.Cast(locE.Position);
                    }
                }
            }

            var laneRValue = Program.laneclear["Lane.UseR"].Cast<ComboBox>().CurrentValue;
            if (laneRValue != 0 && R.IsReady() && UltiBuffStacks < Program.laneclear["Lane.UseRLim"].Cast<Slider>().CurrentValue)
            {
                switch (laneRValue)
                {
                    case 1:
                        {
                            var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                            foreach (var minions in vMinions
                                .Where(minions => minions.Health < R.GetDamage(minions))
                                .Where(
                                    m =>
                                        m.LSIsValidTarget(R.Range) &&
                                        m.LSDistance(ObjectManager.Player.Position) > GetRealAARange)
                                )
                            {
                                R.Cast(minions);
                            }

                            break;
                        }

                    case 2:
                        {
                            laneMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, R.Range + R.Width + 30,
                                MinionTypes.Ranged);

                            if (laneMinions != null)
                            {
                                var locR = R.GetCircularFarmLocation(laneMinions, R.Width * 0.75f);
                                if (locR.MinionsHit >= laneEValue && R.IsInRange(locR.Position.To3D()))
                                {
                                    R.Cast(locR.Position);
                                }
                            }

                            break;
                        }
                }

            }
        }

        public override void ExecuteJungleClear()
        {
            Obj_AI_Base jungleMobs;

            var jungleWValue = Program.jungleClear["Jungle.UseW"].Cast<ComboBox>().CurrentValue;
            if (jungleWValue != 0 && W.IsReady())
            {
                var jungleW = jungleWValue;
                if (jungleW != 0)
                {
                    if (jungleW == 1)
                    {
                        jungleMobs =
                            Utils.Utils.GetMobs(Orbwalking.GetRealAutoAttackRange(null) + 65 + W.Range,
                                Utils.Utils.MobTypes.BigBoys);
                        if (jungleMobs != null)
                        {
                            W.Cast();
                        }
                    }
                    else
                    {
                        var totalAa =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    m =>
                                        m.Team == GameObjectTeam.Neutral &&
                                        m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65 + W.Range))
                                .Sum(mob => (int)mob.Health);

                        totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                        if (totalAa > jungleW * 5)
                        {
                            W.Cast();
                        }

                    }
                }
            }


            var jungleQValue = Program.jungleClear["Jungle.UseQ"].Cast<ComboBox>().CurrentValue;
            if (jungleQValue != 0 && Q.IsReady())
            {
                jungleMobs = Marksman.Utils.Utils.GetMobs(Q.Range, Utils.Utils.MobTypes.All);

                if (jungleMobs != null)
                {
                    switch (jungleQValue)
                    {
                        case 1:
                            {
                                Q.Cast(jungleMobs);
                                break;
                            }
                        case 2:
                            {
                                jungleMobs = Utils.Utils.GetMobs(Q.Range, Utils.Utils.MobTypes.BigBoys);
                                if (jungleMobs != null)
                                {
                                    Q.Cast(jungleMobs);
                                }
                                break;
                            }

                    }
                }
            }

            var jungleEValue = Program.jungleClear["Jungle.UseE"].Cast<ComboBox>().CurrentValue;
            if (jungleEValue != 0 && E.IsReady())
            {
                jungleMobs = Marksman.Utils.Utils.GetMobs(E.Range, Utils.Utils.MobTypes.All);

                if (jungleMobs != null)
                {
                    switch (jungleEValue)
                    {
                        case 1:
                            {
                                E.Cast(jungleMobs, false, true);
                                break;
                            }
                        case 2:
                            {
                                jungleMobs = Utils.Utils.GetMobs(E.Range, Utils.Utils.MobTypes.BigBoys);
                                if (jungleMobs != null)
                                {
                                    E.Cast(jungleMobs, false, true);
                                }
                                break;
                            }

                    }
                }
            }

            var jungleRValue = Program.jungleClear["Jungle.UseR"].Cast<ComboBox>().CurrentValue;
            if (jungleRValue != 0 && R.IsReady() && UltiBuffStacks < Program.jungleClear["Jungle.UseRLim"].Cast<Slider>().CurrentValue)
            {
                jungleMobs = Marksman.Utils.Utils.GetMobs(R.Range, Utils.Utils.MobTypes.All);

                if (jungleMobs != null)
                {
                    switch (jungleRValue)
                    {
                        case 1:
                            {
                                R.Cast(jungleMobs, false, true);
                                break;
                            }
                        case 2:
                            {
                                jungleMobs = Utils.Utils.GetMobs(R.Range, Utils.Utils.MobTypes.BigBoys, jungleRValue);
                                if (jungleMobs != null)
                                {
                                    R.Cast(jungleMobs, false, true);
                                }
                                break;
                            }

                    }
                }
            }
        }

        public override bool LaneClearMenu(Menu config)
        {
            config.Add("Lane.UseQ", new ComboBox("Q:", 3, "Off", "Just For Out of AA Range", "Just Non Killable Minions", "Both"));

            string[] strW = new string[5];
            {
                strW[0] = "Off";
                for (var i = 1; i < 5; i++)
                {
                    var x = (i) * 5;
                    strW[i] = "If need to AA more than >= " + x;
                }
                config.Add("Lane.UseW", new ComboBox("W:", 1, strW));
            }

            string[] strE = new string[7];
            strE[0] = "Off";

            for (var i = 1; i < 7; i++)
            {
                strE[i] = "Minion Count >= " + i;
            }

            config.Add("Lane.UseE", new ComboBox("E:", 3, strE));

            string[] strR = new string[5];
            {
                strR[0] = "Off";
                strR[1] = "Just Out of AA Range";
                for (var i = 2; i < 5; i++)
                {
                    strR[i] = "Minion Count >= " + i;
                }

                config.Add("Lane.UseR", new ComboBox("R:", 3, strR));
                config.Add("Lane.UseRLim", new Slider(Marksman.Utils.Utils.Tab + "R Limit:", 3, 1, 5));
            }
            return true;
        }


        public override bool JungleClearMenu(Menu config)
        {
            config.Add("Jungle.UseQ", new ComboBox("Q:", 2, "Off", "On", "Just big Monsters"));

            string[] strW = new string[5];
            {
                strW[0] = "Off";
                strW[1] = "Just big Monsters";

                for (var i = 2; i < 5; i++)
                {
                    var x = (i - 1) * 5;
                    strW[i] = "If need to AA more than >= " + x;
                }
                config.Add("Jungle.UseW", new ComboBox("W:", 1, strW));
            }
            config.Add("Jungle.UseE", new ComboBox("E:", 2, "Off", "On", "Just big Monsters"));
            string[] strR = new string[4];
            strR[0] = "Off";
            strR[1] = "Just big Monsters";
            for (var i = 2; i < 4; i++)
            {
                strR[i] = "Mob Count >= " + i;
            }
            config.Add("Jungle.UseR", new ComboBox("R:", 3, strR));
            config.Add("Jungle.UseRLim", new Slider(Marksman.Utils.Utils.Tab + "R Limit:", 3, 1, 5));
            return true;
        }
    }
}