#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Marksman.Champions
{
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using System.Diagnostics.Eventing.Reader;

    internal class Quinn : Champion
    {
        public static float ValorMinDamage;
        public static float ValorMaxDamage;
        public LeagueSharp.Common.Spell E;
        public LeagueSharp.Common.Spell Q;
        public LeagueSharp.Common.Spell R;

        public Quinn()
        {
            Utils.Utils.PrintMessage("Quinn loaded.");

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1010);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 800);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 160f, 1150, true, SkillshotType.SkillshotLine);
            E.SetTargetted(0.25f, 2000f);

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        public void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Program.misc["Misc.AntiGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (E.IsReady() && gapcloser.Sender.LSIsValidTarget(E.Range))
                E.CastOnUnit(gapcloser.Sender);
        }

        public override void Orbwalking_AfterAttack(AfterAttackArgs args)
        {
            var target = args.Target;
            var t = target as AIHeroClient;
            if (t == null || (!ComboActive && !HarassActive)) return;

            if (Q.IsReady() && ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue)
                Q.Cast(t, false, true);
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            LeagueSharp.Common.Spell[] spellList = { Q, E};
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem && spell.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 0, 255));

                if (menuItem && spell.Level > 0 && IsValorMode)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.FromArgb(100, 255, 0, 255));
            }
        }

        public static bool IsPositionSafe(AIHeroClient target, LeagueSharp.Common.Spell spell)
            // use underTurret and .Extend for this please
        {
            var predPos = spell.GetPrediction(target).UnitPosition.LSTo2D();
            var myPos = ObjectManager.Player.Position.LSTo2D();
            var newPos = (target.Position.LSTo2D() - myPos);
            newPos.Normalize();

            var checkPos = predPos + newPos*(spell.Range - Vector2.Distance(predPos, myPos));
            Obj_Turret closestTower = null;

            foreach (var tower in ObjectManager.Get<Obj_Turret>()
                .Where(tower => tower.IsValid && !tower.IsDead && Math.Abs(tower.Health) > float.Epsilon)
                .Where(tower => Vector3.Distance(tower.Position, ObjectManager.Player.Position) < 1450))
            {
                closestTower = tower;
            }

            if (closestTower == null)
                return true;

            if (Vector2.Distance(closestTower.Position.LSTo2D(), checkPos) <= 910)
                return false;

            return true;
        }

        public static bool isHePantheon(AIHeroClient target)
        {
            /* Quinn's Spell E can do nothing when Pantheon's passive is active. */
            return target.Buffs.All(buff => buff.Name == "pantheonpassivebuff");
        }

        private static bool IsValorMode
        {
            get
            {
                return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "QuinnRFinale";
            }
        }

        public static void calculateValorDamage()
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level > 0)
            {
                ValorMinDamage = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level*50 + 50;
                ValorMinDamage += ObjectManager.Player.BaseAttackDamage*50;

                ValorMaxDamage = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level*100 + 100;
                ValorMaxDamage += ObjectManager.Player.BaseAttackDamage*100;
            }
        }
        
        public override void Game_OnGameUpdate(EventArgs args)
        {
            var enemy =
                HeroManager.Enemies.Find(
                    e => e.Buffs.Any(b => b.Name.ToLower() == "quinnw_cosmetic" && e.LSIsValidTarget(E.Range)));
            if (enemy != null)
            {
                if (enemy.LSDistance(ObjectManager.Player.Position) > Orbwalking.GetRealAutoAttackRange(null) + 65)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, enemy);
                }
                PortAIO.OrbwalkerManager.ForcedTarget(enemy);
            }

            if (Q.IsReady() && Program.harass["UseQTH"].Cast<KeyBind>().CurrentValue)
            {
                if (ObjectManager.Player.HasBuff("Recall"))
                    return;
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (t != null)
                    Q.Cast(t, false, true);
            }

            if (ComboActive || HarassActive)
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
                var useE = ComboActive ? Program.combo["UseEC"].Cast<CheckBox>().CurrentValue : Program.harass["UseEH"].Cast<CheckBox>().CurrentValue;

                if (PortAIO.OrbwalkerManager.CanMove(0))
                {
                    if (E.IsReady() && useE)
                    {
                        var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                        if (t.LSIsValidTarget() && !t.IsZombie && !isHePantheon(t) && !t.HasBuff("QuinnW_Cosmetic"))
                        {
                            E.CastOnUnit(t);
                        }
                    }

                    if (Q.IsReady() && useQ)
                    {
                        var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                        if (t.LSIsValidTarget() && !t.IsZombie)
                            Q.Cast(t);
                    }

                    if (IsValorMode && !E.IsReady())
                    {
                        var vTarget = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                        if (vTarget != null)
                        {
                            calculateValorDamage();
                            if (vTarget.Health >= ValorMinDamage && vTarget.Health <= ValorMaxDamage)
                                R.Cast();
                        }
                    }
                }
            }
        }

        public override void ExecuteJungleClear()
        {
            if (Q.IsReady())
            {
                var jQ = Marksman.Utils.Utils.GetMobs(Orbwalking.GetRealAutoAttackRange(null) + 65, Marksman.Utils.Utils.MobTypes.All);
                if (jQ != null)
                {
                    switch (Program.jungleClear["UseQJ"].Cast<ComboBox>().CurrentValue)
                    {
                        case 1:
                            {
                                Q.Cast(jQ);
                                break;
                            }
                        case 2:
                            {
                                jQ = Utils.Utils.GetMobs(Orbwalking.GetRealAutoAttackRange(null) + 65, Utils.Utils.MobTypes.BigBoys);
                                if (jQ != null)
                                {
                                    Q.Cast(jQ);
                                }
                                break;
                            }
                    }
                }
            }


            if (E.IsReady())
            {
                var jungleMobs = Marksman.Utils.Utils.GetMobs(E.Range, Marksman.Utils.Utils.MobTypes.All);

                if (jungleMobs != null)
                {
                    switch (Program.jungleClear["UseEJ"].Cast<ComboBox>().CurrentValue)
                    {
                        case 1:
                            {
                                E.CastOnUnit(jungleMobs);
                                break;
                            }
                        case 2:
                            {
                                jungleMobs = Utils.Utils.GetMobs(E.Range, Utils.Utils.MobTypes.BigBoys);
                                if (jungleMobs != null)
                                {
                                    E.CastOnUnit(jungleMobs);
                                }
                                break;
                            }
                    }
                }
            }
        }
        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseEC", new CheckBox("Use E"));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            config.Add("UseEH", new CheckBox("Use E"));
            config.Add("UseQTH", new KeyBind("Use Q (Toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range", false));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawE", new CheckBox("E range", false));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            config.AddLabel(ObjectManager.Player.ChampionName + " Doesn't Support Lane Clear");
            return true;
        }
        public override bool JungleClearMenu(Menu config)
        {
            config.Add("UseQJ", new ComboBox("Use Q", 1, "Off", "On", "Just for big Monsters"));//.SetValue(new StringList(new[] { "Off", "On", "Just for big Monsters" }, 1)));
            config.Add("UseEJ", new ComboBox("Use E", 1, "Off", "On", "Just for big Monsters"));//.SetValue(new StringList(new[] { "Off", "On", "Just for big Monsters" }, 1)));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("Misc.AntiGapCloser", new CheckBox("E Anti Gap Closer"));
            return true;
        }
    }
}
