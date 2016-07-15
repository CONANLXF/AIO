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
    internal class Urgot : Champion
    {
        private const string vSpace = "     ";
        public static LeagueSharp.Common.Spell Q, QEx, W, E, R;

        public Urgot()
        {
            Utils.Utils.PrintMessage("Urgot loaded.");

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1000);
            QEx = new LeagueSharp.Common.Spell(SpellSlot.Q, 1200);
            W = new LeagueSharp.Common.Spell(SpellSlot.W);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 900);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 700);

            Q.SetSkillshot(0.10f, 100f, 1600f, true, SkillshotType.SkillshotLine);
            QEx.SetSkillshot(0.10f, 60f, 1600f, false, SkillshotType.SkillshotLine);

            E.SetSkillshot(0.25f, 120f, 1500f, false, SkillshotType.SkillshotCircle);

            R.SetTargetted(1f, 100f);
        }

        public static int UnderTurretEnemyMinion
        {
            get
            {
                return ObjectManager.Get<Obj_AI_Minion>().Count(xMinion => xMinion.IsEnemy && UnderAllyTurret(xMinion));
            }
        }

        private static AIHeroClient getInfectedEnemy
        {
            get
            {
                return
                    (from enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                enemy =>
                                    enemy.IsEnemy && ObjectManager.Player.LSDistance(enemy) <= QEx.Range &&
                                    enemy.HasBuff("urgotcorrosivedebuff"))
                        select enemy).FirstOrDefault();
            }
        }

        public void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (W.IsReady() && gapcloser.Sender.LSIsValidTarget(250f))
                W.Cast();
        }

        public static bool UnderAllyTurret(Obj_AI_Base unit)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Where<Obj_AI_Turret>(turret =>
            {
                if (turret == null || !turret.IsValid || turret.Health <= 0f)
                {
                    return false;
                }
                if (!turret.IsEnemy)
                {
                    return true;
                }
                return false;
            })
                .Any<Obj_AI_Turret>(
                    turret =>
                        Vector2.Distance(unit.Position.LSTo2D(), turret.Position.LSTo2D()) < 900f && turret.IsAlly);
        }

        public static bool TeleportTurret(AIHeroClient vTarget)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Any(player => !player.IsDead && player.IsMe && UnderAllyTurret(ObjectManager.Player));
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            LeagueSharp.Common.Spell[] spellList = {Q, E, R};
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.LightGray);
            }

            var drawQEx = Program.marksmanDrawings["DrawQEx"].Cast<CheckBox>().CurrentValue;
            if (drawQEx)
            {
                if (getInfectedEnemy != null)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, QEx.Range, Color.GreenYellow);
                    Render.Circle.DrawCircle(getInfectedEnemy.Position, 125f, Color.GreenYellow);
                }
            }
        }

        private static void UseSpells(bool useQ, bool useW, bool useE)
        {
            AIHeroClient t;

            if (W.IsReady() && useW)
            {
                t = TargetSelector.GetTarget(ObjectManager.Player.AttackRange - 30, DamageType.Physical);
                if (t != null)
                    W.Cast();
            }
        }

        private static void UltUnderTurret()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            Drawing.DrawText(Drawing.Width*0.41f, Drawing.Height*0.80f, Color.GreenYellow,
                "Teleport enemy to under ally turret active!");

            if (R.IsReady() && Program.combo["UseRC"].Cast<CheckBox>().CurrentValue)
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t != null && UnderAllyTurret(ObjectManager.Player) && !UnderAllyTurret(t) &&
                    ObjectManager.Player.LSDistance(t) > 200)
                {
                    R.CastOnUnit(t);
                }
            }

            UseSpells(Program.combo["UseQC"].Cast<CheckBox>().CurrentValue, Program.combo["UseWC"].Cast<CheckBox>().CurrentValue,
                Program.combo["UseEC"].Cast<CheckBox>().CurrentValue);
        }

        private static void UltInMyTeam()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            Drawing.DrawText(Drawing.Width*0.42f, Drawing.Height*0.80f, Color.GreenYellow,
                "Teleport enemy to my team active!");

            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (R.IsReady() && t != null)
            {
                var Ally =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            ally =>
                                ally.IsAlly && !ally.IsDead && ObjectManager.Player.LSDistance(ally) <= R.Range &&
                                t.LSDistance(ally) > t.LSDistance(ObjectManager.Player));

                if (Ally.Count() >= Program.combo["UltOp2Count"].Cast<Slider>().CurrentValue)
                    R.CastOnUnit(t);
            }

            UseSpells(Program.combo["UseQC"].Cast<CheckBox>().CurrentValue, Program.combo["UseWC"].Cast<CheckBox>().CurrentValue,
                Program.combo["UseEC"].Cast<CheckBox>().CurrentValue);
        }

        private static void CastQ(AIHeroClient t)
        {
            
            var Qpredict = Q.GetPrediction(t);
            var hithere = Qpredict.CastPosition.LSExtend(ObjectManager.Player.Position, -20);

            if (Qpredict.Hitchance >= HitChance.High)
            {
                if (W.IsReady())
                    W.Cast();
                Q.Cast(hithere);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (R.Level > 0)
                R.Range = 150*R.Level + 400;

            if (Program.combo["UltOp1"].Cast<KeyBind>().CurrentValue)
            {
                UltUnderTurret();
            }

            if (Program.combo["UltOp2"].Cast<KeyBind>().CurrentValue)
            {
                UltInMyTeam();
            }

            if (!ComboActive)
            {
                var t = TargetSelector.GetTarget(QEx.Range, DamageType.Physical);
                if (!t.LSIsValidTarget())
                    return;

                if (HarassActive && Program.harass["UseQH"].Cast<CheckBox>().CurrentValue)
                    CastQ(t);

                if (Program.harass["UseQTH"].Cast<KeyBind>().CurrentValue)
                    CastQ(t);
            }

            if (ComboActive)
            {
                var t = TargetSelector.GetTarget(QEx.Range, DamageType.Physical);

                if (E.IsReady() && Program.combo["UseEC"].Cast<CheckBox>().CurrentValue)
                {
                    if (t.LSIsValidTarget(E.Range))
                    {
                        E.CastIfHitchanceEquals(t, HitChance.Medium);
                    }
                }

                if (Q.IsReady() && Program.combo["UseQC"].Cast<CheckBox>().CurrentValue)
                {
                    if (getInfectedEnemy != null)
                    {
                        if (W.IsReady())
                            W.Cast();
                        QEx.Cast(getInfectedEnemy);
                    }
                    else
                    {
                        if (t.LSIsValidTarget(Q.Range))
                            CastQ(t);
                    }
                }
            }

            if (LaneClearActive)
            {
                var useQ = Program.laneclear["UseQL"].Cast<CheckBox>().CurrentValue;

                if (Q.IsReady() && useQ)
                {
                    var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                    foreach (
                        var minions in
                            vMinions.Where(
                                minions => minions.Health < ObjectManager.Player.LSGetSpellDamage(minions, SpellSlot.Q)))
                        Q.Cast(minions);
                }
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseWC", new CheckBox("Use W"));
            config.Add("UseEC", new CheckBox("Use E"));
            config.Add("UseRC", new CheckBox("Use R"));


            config.AddGroupLabel("Ult Option 1");
            config.Add("UltOp1", new KeyBind(vSpace + "Teleport Ally Turrent", false, KeyBind.BindTypes.HoldActive, 'T'));

            config.AddGroupLabel("Ult Option 2");
            config.Add("UltOp2", new KeyBind(vSpace + "Teleport My Team", false, KeyBind.BindTypes.HoldActive, 'G'));
            config.Add("UltOp2Count", new Slider(vSpace + "Min. Ally Count", 1, 1, 5));


            config.AddGroupLabel("Don't Use Ult on");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                config.Add(string.Format("DontUlt{0}", enemy.CharData.BaseSkinName), new CheckBox(enemy.CharData.BaseSkinName, false));
            }

            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            config.Add("UseQTH", new KeyBind("Use Q (Toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));

            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, Color.LightGray)));
            config.Add("DrawE", new CheckBox("E range"));//.SetValue(new Circle(false, Color.LightGray)));
            config.Add("DrawR", new CheckBox("R range"));//.SetValue(new Circle(false, Color.LightGray)));
            config.Add("DrawQEx", new CheckBox("Corrosive Charge"));//.SetValue(new Circle(true, Color.LightGray)));

            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            config.Add("UseQL", new CheckBox("Use Q"));
            return true;
        }
        public override bool JungleClearMenu(Menu config)
        {
            return false;
        }
    }
}