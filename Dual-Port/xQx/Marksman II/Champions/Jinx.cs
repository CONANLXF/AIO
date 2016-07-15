#region

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Champions;
using Marksman.Common;
using Marksman.Utils;
using SharpDX;
using Color = SharpDX.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

#endregion

 namespace Marksman.Champions
{
    internal class Jinx : Champion
    {
        public static LeagueSharp.Common.Spell Q, W, E, R;

        public Jinx()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1450f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 900f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 25000f);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);

            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                if (E.IsReady())
                {
                    BuffInstance aBuff =
                        (from fBuffs in
                             sender.Buffs.Where(
                                 s =>
                                 sender.Team != ObjectManager.Player.Team
                                 && sender.LSDistance(ObjectManager.Player.Position) < E.Range)
                         from b in new[]
                                           {
                                               "teleport_", /* Teleport */ "pantheon_grandskyfall_jump", /* Pantheon */ 
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
                        E.Cast(sender.Position);
                    }
                }
            };

            Utils.Utils.PrintMessage("Jinx loaded.");
        }

        private static float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            fComboDamage += R.IsReady() ? R.GetDamage(t) : 0;

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot"))
                == SpellState.Ready && ObjectManager.Player.LSDistance(t) < 550) fComboDamage += (float)ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3144) && ObjectManager.Player.LSDistance(t) < 550) fComboDamage += (float)ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(3153) && ObjectManager.Player.LSDistance(t) < 550) fComboDamage += (float)ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Botrk);

            return fComboDamage;
        }


        public float QAddRange => 50 + 25 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level;

        private static bool FishBoneActive => ObjectManager.Player.AttackRange > 565f;

        private static int PowPowStacks
        {
            get
            {
                return
                    ObjectManager.Player.Buffs.Where(buff => buff.DisplayName.ToLower() == "jinxqramp")
                        .Select(buff => buff.Count)
                        .FirstOrDefault();
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {

            /*----------------------------------------------------*/
            var drawQbound = Program.marksmanDrawings["DrawQBound"].Cast<CheckBox>().CurrentValue;
            foreach (var spell in new[] { W, E })
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, System.Drawing.Color.CornflowerBlue);
                }
            }

            if (drawQbound)
            {
                if (FishBoneActive)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 525f + ObjectManager.Player.BoundingRadius + 65f, System.Drawing.Color.FromArgb(100, 255, 0, 0));
                }
                else
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 525f + ObjectManager.Player.BoundingRadius + 65f + QAddRange + 20f, System.Drawing.Color.FromArgb(100, 255, 0, 0));
                }
            }
        }
        
        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.combo["PingCH"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => R.IsReady() && enemy.IsValidTarget() && R.GetDamage(enemy) > enemy.Health))
                {
                    //Marksman.Utils.Utils.MPing.Ping(enemy.Position.To2D());
                }
            }
            if (Q.IsReady() && Program.misc["SwapDistance"].Cast<CheckBox>().CurrentValue && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var activeQ = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level * 25 + 650;
                var t = TargetSelector.GetTarget(activeQ, DamageType.Physical);

                if (t.LSIsValidTarget() && ObjectManager.Player.LSDistance(t) > Orbwalking.GetRealAutoAttackRange(null) + 65)
                {
                    if (!FishBoneActive)
                    {
                        Q.Cast();
                        return;
                    }
                }
                if (!t.LSIsValidTarget() && FishBoneActive)
                {
                    Q.Cast();
                    return;
                }

            }

            if (Program.combo["PingCH"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        t =>
                        R.IsReady() && t.LSIsValidTarget() && R.GetDamage(t) > t.Health
                        && t.LSDistance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(null) + 65 + QAddRange))
                {
                    //Utils.Utils.MPing.Ping(enemy.Position.LSTo2D(), 2, PingCategory.Normal);
                }
            }

            var autoEi = Program.misc["AutoEI"].Cast<CheckBox>().CurrentValue;
            var autoEs = Program.misc["AutoES"].Cast<CheckBox>().CurrentValue;
            var autoEd = Program.misc["AutoED"].Cast<CheckBox>().CurrentValue;

            //foreach (var e in HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range)))
            //{
            //    if (E.IsReady()
            //        && (e.HasBuffOfType(BuffType.Stun) || e.HasBuffOfType(BuffType.Snare)
            //            || e.HasBuffOfType(BuffType.Charm) || e.HasBuffOfType(BuffType.Fear) ||
            //            e.HasBuffOfType(BuffType.Slow)
            //            || e.HasBuffOfType(BuffType.Taunt) || e.HasBuff("zhonyasringshield")
            //            || e.HasBuff("Recall")))
            //    {
            //        E.Cast(e);
            //    }
            //}

            if (autoEs || autoEi || autoEd)
            {
                foreach (
                    var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.LSIsValidTarget(E.Range - 50)))
                {
                    if (autoEs && E.IsReady() && enemy.HasBuffOfType(BuffType.Slow))
                    {
                        var castPosition =
                            LeagueSharp.Common.Prediction.GetPrediction(
                                new PredictionInput
                                {
                                    Unit = enemy,
                                    Delay = 0.7f,
                                    Radius = 120f,
                                    Speed = 1750f,
                                    Range = 900f,
                                    Type = SkillshotType.SkillshotCircle
                                }).CastPosition;


                        if (GetSlowEndTime(enemy) >= (Game.Time + E.Delay + 0.5f))
                        {
                            E.Cast(castPosition);
                        }
                    }

                    if (E.IsReady()
                            && (enemy.HasBuffOfType(BuffType.Stun)
                            || enemy.HasBuffOfType(BuffType.Snare)
                            || enemy.HasBuffOfType(BuffType.Charm)
                            || enemy.HasBuffOfType(BuffType.Fear)
                            || enemy.HasBuffOfType(BuffType.Slow)
                            || enemy.HasBuffOfType(BuffType.Taunt)
                            || enemy.HasBuff("zhonyasringshield")
                            || enemy.HasBuff("Recall")))
                    {
                        E.CastIfHitchanceEquals(enemy, HitChance.High);
                    }
                    else
                    if (W.IsReady()
                            && (enemy.HasBuffOfType(BuffType.Stun)
                            || enemy.HasBuffOfType(BuffType.Snare)
                            || enemy.HasBuffOfType(BuffType.Charm)
                            || enemy.HasBuffOfType(BuffType.Fear)
                            || enemy.HasBuffOfType(BuffType.Slow)
                            || enemy.HasBuffOfType(BuffType.Taunt)
                            || enemy.HasBuff("Recall")))
                    {
                        W.CastIfHitchanceEquals(enemy, HitChance.High);
                    }

                    if (autoEd && E.IsReady() && enemy.LSIsDashing())
                    {
                        E.CastIfHitchanceEquals(enemy, HitChance.Dashing);
                    }
                }
            }

            if (Program.misc["CastR"].Cast<KeyBind>().CurrentValue && R.IsReady())
            {
                var t = TargetSelector.GetTarget(1500, DamageType.Physical);
                if (t.LSIsValidTarget())
                {
                    if (ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R) > t.Health && !t.IsZombie)
                    {
                        R.CastIfHitchanceGreaterOrEqual(t);
                        //R.CastIfHitchanceEquals(t, HitChance.High, false);
                    }
                }
            }

            if (Program.misc["SwapQ"].Cast<CheckBox>().CurrentValue && FishBoneActive && !ComboActive)
            {
                Q.Cast();
            }

            if ((!ComboActive && !HarassActive) || !Orbwalker.CanMove)
            {
                return;
            }

            var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;
            var useR = Program.combo["UseRC"].Cast<CheckBox>().CurrentValue;

            if (useW && W.IsReady())
            {
                var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                var minW = Program.misc["MinWRange"].Cast<Slider>().CurrentValue;

                if (t.LSIsValidTarget() && !t.HasKindredUltiBuff() && GetRealDistance(t) >= minW)
                {
                    W.CastIfHitchanceGreaterOrEqual(t);
                }
            }

            if (useR && R.IsReady())
            {
                var checkRok = Program.misc["ROverKill"].Cast<CheckBox>().CurrentValue;
                var minR = Program.misc["MinRRange"].Cast<Slider>().CurrentValue;
                var maxR = Program.misc["MaxRRange"].Cast<Slider>().CurrentValue;
                var t = TargetSelector.GetTarget(maxR, DamageType.Physical);

                if (t.LSIsValidTarget() && !t.HasKindredUltiBuff())
                {
                    var distance = GetRealDistance(t);

                    if (!checkRok)
                    {
                        if (ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R, 1) > t.Health && !t.IsZombie)
                        {
                            R.CastIfHitchanceGreaterOrEqual(t);
                            //R.CastIfHitchanceEquals(t, HitChance.High, false);
                            //if (R.Cast(t) == Spell.CastStates.SuccessfullyCasted) { }
                        }
                    }
                    else if (distance > minR)
                    {
                        var aDamage = ObjectManager.Player.LSGetAutoAttackDamage(t);
                        var wDamage = ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.W);
                        var rDamage = ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R);
                        var powPowRange = GetRealPowPowRange(t);

                        if (distance < (powPowRange + QAddRange) && !(aDamage * 3.5 > t.Health))
                        {
                            if (!W.IsReady() || !(wDamage > t.Health) || W.GetPrediction(t).CollisionObjects.Count > 0)
                            {
                                if (CountAlliesNearTarget(t, 500) <= 3)
                                {
                                    if (rDamage > t.Health && !t.IsZombie /*&& !ObjectManager.Player.IsAutoAttacking &&
                                        !ObjectManager.Player.IsChanneling*/)
                                    {
                                        R.CastIfHitchanceGreaterOrEqual(t);
                                        //R.CastIfHitchanceEquals(t, HitChance.High, false);
                                        //if (R.Cast(t) == Spell.CastStates.SuccessfullyCasted) { }
                                    }
                                }
                            }
                        }
                        else if (distance > (powPowRange + QAddRange))
                        {
                            if (!W.IsReady() || !(wDamage > t.Health) || distance > W.Range
                                || W.GetPrediction(t).CollisionObjects.Count > 0)
                            {
                                if (CountAlliesNearTarget(t, 500) <= 3)
                                {
                                    if (rDamage > t.Health && !t.IsZombie /*&& !ObjectManager.Player.IsAutoAttacking &&
                                        !ObjectManager.Player.IsChanneling*/)
                                    {
                                        R.CastIfHitchanceGreaterOrEqual(t);
                                        //R.CastIfHitchanceEquals(t, HitChance.High, false);
                                        //if (R.Cast(t) == Spell.CastStates.SuccessfullyCasted) { }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit targetA, EventArgs args)
        {
            var target = targetA;
            if ((ComboActive || HarassActive) && (target is AIHeroClient))
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
                var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;

                if (useW && W.IsReady())
                {
                    var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                    var minW = Program.misc["MinWRange"].Cast<Slider>().CurrentValue;

                    if (t.LSIsValidTarget() && !t.HasKindredUltiBuff() && GetRealDistance(t) >= minW)
                    {
                        W.CastIfHitchanceGreaterOrEqual(t);
                    }
                }

                if (useQ)
                {
                    foreach (var t in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(t => t.LSIsValidTarget(GetRealPowPowRange(t) + QAddRange + 20f) && !t.HasKindredUltiBuff()))
                    {
                        var swapDistance = Program.misc["SwapDistance"].Cast<CheckBox>().CurrentValue;
                        var swapAoe = Program.misc["SwapAOE"].Cast<CheckBox>().CurrentValue;
                        var distance = GetRealDistance(t);
                        var powPowRange = GetRealPowPowRange(t);

                        if (swapDistance && Q.IsReady())
                        {
                            if (distance > powPowRange && !FishBoneActive)
                            {
                                if (Q.Cast())
                                {
                                    return;
                                }
                            }
                            else if (distance < powPowRange && FishBoneActive)
                            {
                                if (Q.Cast())
                                {
                                    return;
                                }
                            }
                        }

                        if (swapAoe && Q.IsReady())
                        {
                            if (distance > powPowRange && PowPowStacks > 2 && !FishBoneActive
                                && CountEnemies(t, 150) > 1)
                            {
                                if (Q.Cast())
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static int CountEnemies(Obj_AI_Base target, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Count(
                        hero =>
                        hero.LSIsValidTarget() && hero.Team != ObjectManager.Player.Team
                        && hero.ServerPosition.LSDistance(target.ServerPosition) <= range);
        }

        private int CountAlliesNearTarget(Obj_AI_Base target, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Count(
                        hero =>
                        hero.Team == ObjectManager.Player.Team
                        && hero.ServerPosition.LSDistance(target.ServerPosition) <= range);
        }

        private static float GetRealPowPowRange(GameObject target)
        {
            return 525f + ObjectManager.Player.BoundingRadius + target.BoundingRadius;
        }

        private static float GetRealDistance(GameObject target)
        {
            return ObjectManager.Player.Position.LSDistance(target.Position) + ObjectManager.Player.BoundingRadius
                   + target.BoundingRadius;
        }

        private static float GetSlowEndTime(Obj_AI_Base target)
        {
            return
                target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Type == BuffType.Slow)
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseWC", new CheckBox("Use W"));
            config.Add("UseRC", new CheckBox("Use R"));
            config.Add("PingCH", new CheckBox("Ping Killable Enemy with R"));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            config.Add("UseQMH", new CheckBox("Use Q Nearby Minions"));
            config.Add("UseWH", new CheckBox("Use W", false));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            // Q
            string[] strQ = new string[5];
            {
                strQ[0] = "Off";

                for (var i = 1; i < 5; i++)
                {
                    strQ[i] = "Minion Count >= " + i;
                }
                config.Add("Lane.UseQ", new ComboBox("Q:", 0, strQ));
            }
            config.Add("Lane.UseQ.Mode", new ComboBox("Q Mode:", 2, "Under Ally Turret", "Out of AA Range", "Botch"));

            // W
            config.Add("Lane.UseW", new ComboBox("W:", 1, "Off", "Out of AA Range"));
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            // Q
            string[] strQ = new string[4];
            {
                strQ[0] = "Off";
                strQ[1] = "Just for big Monsters";

                for (var i = 2; i < 4; i++)
                {
                    strQ[i] = "Mobs Count >= " + i;
                }
                config.Add("Lane.UseQ", new ComboBox("Q:", 3, strQ));
            }

            // W
            config.Add("Lane.UseW", new ComboBox("W [Just Big Mobs]:", 0, "Off", "On", "Just Slows the Mob"));

            // R
            config.Add("Lane.UseR", new ComboBox("R:", 1, "Off", "Baron/Dragon Steal"));

            return true;
        }

        public override bool MainMenu(Menu config)
        {
            return base.MainMenu(config);
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("SwapQ", new CheckBox("Q: Always swap to Minigun", false));
            config.Add("SwapDistance", new CheckBox("Q: Swap for distance"));
            config.Add("SwapAOE", new CheckBox("Q: Swap for AOE", false));
            config.AddSeparator();
            config.Add("MinWRange", new Slider("W: Min. range", 525 + 65 * 2, 0, 1200));
            config.AddSeparator();
            config.Add("AutoEI", new CheckBox("E: on immobile"));
            config.Add("AutoES", new CheckBox("E: on slowed"));
            config.Add("AutoED", new CheckBox("E: on dashing", false));
            config.AddSeparator();
            config.Add("CastR", new KeyBind("R: (2000 Range)", false, KeyBind.BindTypes.HoldActive, 'T'));
            config.Add("ROverKill", new CheckBox("R: Kill Steal"));
            config.Add("MinRRange", new Slider("R: Min. range", 300, 0, 1500));
            config.Add("MaxRRange", new Slider("R: Max. range", 1700, 0, 4000));
            config.Add("PingCH", new CheckBox("R: Ping Killable Enemy with R"));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQBound", new CheckBox("Draw Q bound"));//.SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 0))));
            config.Add("DrawE", new CheckBox("E range"));//.SetValue(new Circle(false, System.Drawing.Color.CornflowerBlue)));
            config.Add("DrawW", new CheckBox("W range"));//.SetValue(new Circle(false, System.Drawing.Color.CornflowerBlue)));
            config.Add("DrawCH", new CheckBox("Draw Killable Enemy with R"));
            return true;
        }

        public override void ExecuteFlee()
        {
            foreach (
                AIHeroClient unit in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(e => e.LSIsValidTarget(E.Range) && !e.IsDead && e.IsEnemy)
                        .OrderBy(e => ObjectManager.Player.LSDistance(e)))
            {
                PredictionOutput ePred = E.GetPrediction(unit);
                Vector3 eBehind = ePred.CastPosition - Vector3.Normalize(unit.ServerPosition - ObjectManager.Player.ServerPosition) * 150;

                if (E.IsReady())
                    E.Cast(eBehind);
            }

            base.ExecuteFlee();
        }

        public override void PermaActive()
        {
        }

    }
}
