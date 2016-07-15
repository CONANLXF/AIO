#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

#endregion

 namespace Marksman.Champions
{
    internal class Varus : Champion
    {
        public static LeagueSharp.Common.Spell Q, W, E, R;
        private float LastSpellTick;

        public Varus()
        {
            Utils.Utils.PrintMessage("Varus loaded!");

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1600f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 925f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1200f);

            Q.SetSkillshot(.25f, 70f, 1650f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(.50f, 250f, 1400f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(.25f, 120f, 1950f, false, SkillshotType.SkillshotLine);

            Q.SetCharged("VarusQ", "VarusQ", 250, 1600, 1.2f);

            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        private static float CalcWDamage
        {
            get
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var xEnemyWStackCount = EnemyWStackCount(t);
                var wExplodePerStack = ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.W, 1)*xEnemyWStackCount > 0
                    ? xEnemyWStackCount
                    : 1;
                return wExplodePerStack;
            }
        }

        private static float CalcQDamage
        {
            get
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (!Q.IsReady())
                    return 0;

                /*
                var qDamageMaxPerLevel = new[] {15f, 70f, 125f, 180f, 235f};
                var fxQDamage2 = qDamageMaxPerLevel[Q.Level - 1] +
                                 1.6*
                                 (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod);

                var xDis = ObjectManager.Player.LSDistance(t)/Q.ChargedMaxRange;
                return (float) fxQDamage2*xDis;
                */
                var fxQDamage2 = ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q, 1);
                return (float) fxQDamage2;
            }
        }

        private float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            if (Q.IsReady())
                fComboDamage += (float) ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q);
            //fComboDamage += CalcQDamage;

            if (W.Level > 0)
                fComboDamage += (float) ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.W);

            if (E.IsReady())
                fComboDamage += (float) ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.E);

            if (R.IsReady())
                fComboDamage += (float) ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R);

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot")) ==
                SpellState.Ready && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3144) && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(3153) && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Botrk);

            return fComboDamage;
        }

        private void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.SData.Name.ToLower().Contains("attack"))
                return;

            LastSpellTick = Environment.TickCount;
        }

        public static int EnemyWStackCount(AIHeroClient t)
        {
            return
                t.Buffs.Where(xBuff => xBuff.Name == "varuswdebuff" && t.LSIsValidTarget(Q.Range))
                    .Select(xBuff => xBuff.Count)
                    .FirstOrDefault();
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = Program.marksmanDrawings["DrawQ"].Cast<CheckBox>().CurrentValue;
            var drawE = Program.marksmanDrawings["DrawE"].Cast<CheckBox>().CurrentValue;
            var drawR = Program.marksmanDrawings["DrawR"].Cast<CheckBox>().CurrentValue;
            var drawQc = Program.marksmanDrawings["DrawQC"].Cast<CheckBox>().CurrentValue;
            var drawRs = Program.marksmanDrawings["DrawRS"].Cast<CheckBox>().CurrentValue;

            if (drawQ)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.DarkGray);

            if (drawE)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.DarkGray);

            if (drawQc)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Program.combo["QMinChargeC"].Cast<Slider>().CurrentValue, Color.White);

            if (drawR)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.DarkGray);

            if (Program.misc["CastR"].Cast<KeyBind>().CurrentValue && drawRs)
            {
                Vector3 drawPosition;

                if (ObjectManager.Player.LSDistance(Game.CursorPos) < R.Range - 300f)
                    drawPosition = Game.CursorPos;
                else
                    drawPosition = ObjectManager.Player.Position +
                                   Vector3.Normalize(Game.CursorPos - ObjectManager.Player.Position)*(R.Range - 300f);

                Render.Circle.DrawCircle(drawPosition, 300f, Color.White);
            }
        }

        private static void CastSpellQ()
        {
            if (!Q.IsReady())
                return;

            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!t.LSIsValidTarget(Q.Range))
                return;

            var qMinCharge = Program.combo["QMinChargeC"].Cast<Slider>().CurrentValue;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Q.IsCharging)
            {
                if (Q.Range >= qMinCharge)
                    Q.Cast(t, false, true);
            }
            else
            {
                Q.StartCharging();
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.misc["CastR"].Cast<KeyBind>().CurrentValue)
            {
                Vector3 searchPos;

                if (ObjectManager.Player.LSDistance(Game.CursorPos) < R.Range - 300f)
                    searchPos = Game.CursorPos;
                else
                    searchPos = ObjectManager.Player.Position +
                                Vector3.Normalize(Game.CursorPos - ObjectManager.Player.Position)*(R.Range - 300f);

                var rTarget =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(hero => hero.LSIsValidTarget(R.Range) && hero.LSDistance(searchPos) < 300f)
                        .OrderByDescending(TargetSelector.GetPriority)
                        .First();

                if (rTarget != null && R.IsReady())
                    R.Cast(rTarget);
            }

            if (Program.combo["UseQ2C"].Cast<CheckBox>().CurrentValue)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                CastSpellQ();
            }

            AIHeroClient t;

            if (E.IsReady() && Program.harass["UseETH"].Cast<KeyBind>().CurrentValue)
            {
                if (ObjectManager.Player.HasBuff("Recall"))
                    return;
                t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t != null)
                    E.Cast(t, false, true);
            }

            if (!ComboActive && !HarassActive) return;

            var useQ = ComboActive ? Program.combo["UseQC"].Cast<ComboBox>().CurrentValue : Program.harass["UseQH"].Cast<ComboBox>().CurrentValue;
            var useE = ComboActive ? Program.combo["UseEC"].Cast<CheckBox>().CurrentValue : Program.harass["UseEH"].Cast<CheckBox>().CurrentValue;
            var useR = Program.combo["UseRC"].Cast<CheckBox>().CurrentValue;

            t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (t.LSIsValidTarget(Q.Range) && t.Health <= CalcQDamage + CalcWDamage)
                CastSpellQ();

            switch (useQ)
            {
                case 1:
                {
                    CastSpellQ();
                    break;
                }
                case 2:
                {
                    if (EnemyWStackCount(t) > 2 || W.Level == 0)
                        CastSpellQ();
                    break;
                }
            }

            if (useE && E.IsReady())
            {
                t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t.LSIsValidTarget(E.Range))
                    E.Cast(t, false, true);
            }

            if (useR && R.IsReady())
            {
                t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.LSIsValidTarget(R.Range) && t.Health <= ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R) - 30f)
                    R.Cast(t);
            }
        }

        public override void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            args.Process = !Q.IsCharging;
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new ComboBox("Q Mode", 0, "Off", "Use Allways", "Max W Stack = 3"));
            config.Add("UseEC", new CheckBox("Use E"));
            config.Add("UseRC", new CheckBox("Use R"));
            config.Add("QMinChargeC", new Slider("Min. Q Charge", 1000, Q.ChargedMinRange, Q.ChargedMaxRange));
            config.Add("UseQ2C", new KeyBind("Use Insta Q", false, KeyBind.BindTypes.HoldActive, 'J'));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new ComboBox("Q", 0, "Off", "Use Allways", "Max W Stack = 3"));
            config.Add("UseEH", new CheckBox("E"));
            config.Add("UseETH", new KeyBind("Use E (Toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));

            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("spellDelay", new Slider("Spell delay", 500, 0, 3000));
            config.Add("CastR", new KeyBind("Cast R", false, KeyBind.BindTypes.HoldActive, 'T'));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q"));//.SetValue(new Circle(true, Color.DarkGray)));
            config.Add("DrawE", new CheckBox("E"));//.SetValue(new Circle(true, Color.DarkGray)));
            config.Add("DrawR", new CheckBox("R"));//.SetValue(new Circle(true, Color.DarkGray)));
            config.Add("DrawQC", new CheckBox("Min. Q Charge"));//.SetValue(new Circle(true, Color.White)));
            config.Add("DrawRS", new CheckBox("R: Search Area"));//.SetValue(new Circle(true, Color.White)));

            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            return true;
        }
        public override bool JungleClearMenu(Menu config)
        {
            return false;
        }
    }
}