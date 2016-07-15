using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Syndra
    {
        private Menu Config = Program.Config;
        public AIHeroClient Player { get { return ObjectManager.Player; } }
        private LeagueSharp.Common.Spell E, Q, R, W, EQ, Eany;
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        private static List<Obj_AI_Minion> BallsList = new List<Obj_AI_Minion>();
        private bool EQcastNow = false;
        public void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 790);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 950);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 700);
            EQ = new LeagueSharp.Common.Spell(SpellSlot.Q, Q.Range + 400);
            Eany = new LeagueSharp.Common.Spell(SpellSlot.Q, Q.Range + 400);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.6f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 100, 2500f, false, SkillshotType.SkillshotLine);
            EQ.SetSkillshot(0.6f, 100f, 2500f, false, SkillshotType.SkillshotLine);
            Eany.SetSkillshot(0.30f, 50f, 2500f, false, SkillshotType.SkillshotLine);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw when skill rdy", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("harrasQ", new CheckBox("Harass Q", true));
            qMenu.Add("QHarassMana", new Slider("Harass Mana", 30, 0, 100));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("harrasW", new CheckBox("Harass W", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto Q + E combo, ks", true));
            eMenu.Add("harrasE", new CheckBox("Harass Q + E", false));
            eMenu.Add("EInterrupter", new CheckBox("Auto Q + E Interrupter", true));
            eMenu.Add("useQE", new KeyBind("Semi-manual Q + E near mouse key", false, KeyBind.BindTypes.HoldActive, 'T'));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                eMenu.Add("Egapcloser" + enemy.ChampionName, new CheckBox("Q>E Gap : " + enemy.ChampionName, true));
            eMenu.AddSeparator();
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                eMenu.Add("Eon" + enemy.ChampionName, new CheckBox("Q + E : " + enemy.ChampionName, true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R KS", true));
            rMenu.Add("Rcombo", new CheckBox("Extra combo dmg calculation", true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                rMenu.Add("Rmode" + enemy.ChampionName, new ComboBox("R : " + enemy.ChampionName, 0, "Only To KS", "Always ", "Never"));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQout", new CheckBox("Last hit Q minion out range AA", true));
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q", true));
            farmMenu.Add("farmW", new CheckBox("Lane clear W", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 0, 100));
            farmMenu.Add("LCminions", new Slider(" LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q", true));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W", true));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        public static Menu farmMenu, harassMenu, rMenu, eMenu, wMenu, qMenu, drawMenu;

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.Q && EQcastNow && E.IsReady())
            {
                var customeDelay = Q.Delay - (E.Delay + ((Player.LSDistance(args.End)) / E.Speed));
                LeagueSharp.Common.Utility.DelayAction.Add((int)(customeDelay * 1000), () => E.Cast(args.End));
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && eMenu["EInterrupter"].Cast<CheckBox>().CurrentValue)
            {
                if (sender.LSIsValidTarget(E.Range))
                {
                    E.Cast(sender.Position);
                }
                else if (Q.IsReady() && sender.LSIsValidTarget(EQ.Range))
                {
                    TryBallE(sender);
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && eMenu["Egapcloser" + gapcloser.Sender.ChampionName].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady())
                {
                    EQcastNow = true;
                    Q.Cast(gapcloser.End);
                }
                else if (gapcloser.Sender.LSIsValidTarget(E.Range))
                {
                    E.Cast(gapcloser.Sender);
                }
            }
        }

        private void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Type == GameObjectType.obj_AI_Minion && sender.Name == "Seed")
            {
                var ball = sender as Obj_AI_Minion;
                BallsList.Add(ball);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!E.IsReady())
                EQcastNow = false;

            if (Program.LagFree(1))
            {
                SetMana();
                BallCleaner();
                Jungle();
            }

            if (Program.LagFree(1) && E.IsReady() && eMenu["autoE"].Cast<CheckBox>().CurrentValue)
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && qMenu["autoQ"].Cast<CheckBox>().CurrentValue)
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && wMenu["autoW"].Cast<CheckBox>().CurrentValue)
                LogicW();

            if (Program.LagFree(4) && R.IsReady() && rMenu["autoR"].Cast<CheckBox>().CurrentValue)
                LogicR();
        }

        private void TryBallE(AIHeroClient t)
        {
            if (Q.IsReady())
            {
                CastQE(t);
            }
            if (!EQcastNow)
            {
                var ePred = Eany.GetPrediction(t);
                if (ePred.Hitchance >= HitChance.VeryHigh)
                {
                    var playerToCP = Player.LSDistance(ePred.CastPosition);
                    foreach (var ball in BallsList.Where(ball => Player.LSDistance(ball.Position) < E.Range))
                    {
                        var ballFinalPos = Player.ServerPosition.LSExtend(ball.Position, playerToCP);
                        if (ballFinalPos.LSDistance(ePred.CastPosition) < 50)
                            E.Cast(ball.Position);
                    }
                }
            }
        }

        private void LogicE()
        {
            if (eMenu["useQE"].Cast<KeyBind>().CurrentValue)
            {
                var mouseTarget = Program.Enemies.Where(enemy =>
                    enemy.LSIsValidTarget(Eany.Range)).OrderBy(enemy => enemy.LSDistance(Game.CursorPos)).FirstOrDefault();

                if (mouseTarget != null)
                {
                    TryBallE(mouseTarget);
                    return;
                }
            }

            var t = TargetSelector.GetTarget(Eany.Range, DamageType.Magical);
            if (t.LSIsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, E) + Q.GetDamage(t) > t.Health)
                    TryBallE(t);
                if (Program.Combo && Player.Mana > RMANA + EMANA + QMANA && eMenu["Eon" + t.ChampionName].Cast<CheckBox>().CurrentValue)
                    TryBallE(t);
                if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA + WMANA && eMenu["harrasE"].Cast<CheckBox>().CurrentValue && harassMenu["harras" + t.ChampionName].Cast<CheckBox>().CurrentValue)
                    TryBallE(t);
            }
        }

        private void LogicR()
        {
            R.Range = R.Level == 3 ? 750 : 675;

            bool Rcombo = rMenu["Rcombo"].Cast<CheckBox>().CurrentValue;

            foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(R.Range) && OktwCommon.ValidUlt(enemy)))
            {
                int Rmode = rMenu["Rmode" + enemy.ChampionName].Cast<ComboBox>().CurrentValue;

                if (Rmode == 2)
                    continue;
                else if (Rmode == 1)
                    R.Cast(enemy);

                var comboDMG = OktwCommon.GetKsDamage(enemy, R);
                comboDMG += (R.GetDamage(enemy, 1) * (R.Instance.Ammo - 3));
                comboDMG += OktwCommon.GetEchoLudenDamage(enemy);

                if (Rcombo)
                {
                    if (Q.IsReady() && enemy.LSIsValidTarget(600))
                        comboDMG += Q.GetDamage(enemy);

                    if (E.IsReady())
                        comboDMG += E.GetDamage(enemy);

                    if (W.IsReady())
                        comboDMG += W.GetDamage(enemy);
                }

                if (enemy.Health < comboDMG)
                {
                    R.Cast(enemy);
                }
            }
        }

        private void LogicW()
        {
            if (W.Instance.ToggleState == 1)
            {
                var t = TargetSelector.GetTarget(W.Range - 150, DamageType.Magical);
                if (t.LSIsValidTarget())
                {
                    if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                        CatchW(t);
                    else if (Program.Farm && wMenu["harrasW"].Cast<CheckBox>().CurrentValue && harassMenu["harras" + t.ChampionName].Cast<CheckBox>().CurrentValue
                        && Player.ManaPercent > qMenu["QHarassMana"].Cast<Slider>().CurrentValue && OktwCommon.CanHarras())
                    {
                        CatchW(t);
                    }
                    else if (OktwCommon.GetKsDamage(t, W) > t.Health)
                        CatchW(t);
                    else if (Player.Mana > RMANA + WMANA)
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                            CatchW(t);
                    }
                }
                else if (Program.LaneClear && !Q.IsReady() && Player.ManaPercent > farmMenu["Mana"].Cast<Slider>().CurrentValue && farmMenu["farmW"].Cast<CheckBox>().CurrentValue)
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                    var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);

                    if (farmPos.MinionsHit >= farmMenu["LCminions"].Cast<Slider>().CurrentValue)
                        CatchW(allMinions.FirstOrDefault());
                }
            }
            else
            {
                var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (t.LSIsValidTarget())
                {
                    Program.CastSpell(W, t);
                }
                else if (Program.LaneClear && farmMenu["farmW"].Cast<CheckBox>().CurrentValue)
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                    var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);

                    if (farmPos.MinionsHit > 1)
                        W.Cast(farmPos.Position);
                }
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.LSIsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + QMANA + EMANA && !E.IsReady())
                    Program.CastSpell(Q, t);
                else if (Program.Farm && qMenu["harrasQ"].Cast<CheckBox>().CurrentValue && harassMenu["harras" + t.ChampionName].Cast<CheckBox>().CurrentValue && Player.ManaPercent > qMenu["QHarassMana"].Cast<Slider>().CurrentValue && OktwCommon.CanHarras())
                    Program.CastSpell(Q, t);
                else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Program.CastSpell(Q, t);
                else if (Player.Mana > RMANA + QMANA)
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Program.CastSpell(Q, t);
                }
            }

            if (Player.Spellbook.IsAutoAttacking)
                return;

            if (!Program.None && !Program.Combo && Player.ManaPercent > farmMenu["Mana"].Cast<Slider>().CurrentValue)
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, Q.Range);

                if (farmMenu["farmQout"].Cast<CheckBox>().CurrentValue)
                {
                    foreach (var minion in allMinions.Where(minion => minion.LSIsValidTarget(Q.Range) && (!Player.IsInAutoAttackRange(minion) || (!minion.UnderTurret(true) && minion.UnderTurret()))))
                    {
                        var hpPred = SebbyLib.HealthPrediction.GetHealthPrediction(minion, 600);
                        if (hpPred < Q.GetDamage(minion) && hpPred > minion.Health - hpPred * 2)
                        {
                            Q.Cast(minion);
                            return;
                        }
                    }
                }
                if (Program.LaneClear && farmMenu["farmQ"].Cast<CheckBox>().CurrentValue)
                {
                    var farmPos = Q.GetCircularFarmLocation(allMinions, Q.Width);
                    if (farmPos.MinionsHit >= farmMenu["LCminions"].Cast<Slider>().CurrentValue)
                        Q.Cast(farmPos.Position);
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && farmMenu["jungleQ"].Cast<CheckBox>().CurrentValue)
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }
                    else if (W.IsReady() && farmMenu["jungleW"].Cast<CheckBox>().CurrentValue && Utils.TickCount - Q.LastCastAttemptT > 900)
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                }
            }
        }

        private void CastQE(Obj_AI_Base target)
        {
            SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;

            var predInput2 = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = false,
                Collision = EQ.Collision,
                Speed = EQ.Speed,
                Delay = EQ.Delay,
                Range = EQ.Range,
                From = Player.ServerPosition,
                Radius = EQ.Width,
                Unit = target,
                Type = CoreType2
            };

            var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

            if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            Vector3 castQpos = poutput2.CastPosition;

            if (Player.LSDistance(castQpos) > Q.Range)
                castQpos = Player.Position.LSExtend(castQpos, Q.Range);

            if (Program.getHitChance == 0)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                {
                    EQcastNow = true;
                    Q.Cast(castQpos);
                }

            }
            else if (Program.getHitChance == 1)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                {
                    EQcastNow = true;
                    Q.Cast(castQpos);
                }

            }
            else if (Program.getHitChance == 2)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                {
                    EQcastNow = true;
                    Q.Cast(castQpos);
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (drawMenu["qRange"].Cast<CheckBox>().CurrentValue)
            {
                if (drawMenu["onlyRdy"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (drawMenu["wRange"].Cast<CheckBox>().CurrentValue)
            {
                if (drawMenu["onlyRdy"].Cast<CheckBox>().CurrentValue)
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (drawMenu["eRange"].Cast<CheckBox>().CurrentValue)
            {
                if (drawMenu["onlyRdy"].Cast<CheckBox>().CurrentValue)
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, EQ.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, EQ.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (drawMenu["rRange"].Cast<CheckBox>().CurrentValue)
            {
                if (drawMenu["onlyRdy"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }

        private void SetMana()
        {
            if ((Program.Config["manaDisable"].Cast<CheckBox>().CurrentValue && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        private void BallCleaner()
        {
            if (BallsList.Count > 0)
            {
                BallsList.RemoveAll(ball => !ball.IsValid || ball.Mana == 19);
            }
        }

        private void CatchW(Obj_AI_Base t, bool onlyMinin = false)
        {

            if (Utils.TickCount - W.LastCastAttemptT < 150)
                return;

            var catchRange = 925;
            Obj_AI_Base obj = null;
            if (BallsList.Count > 0 && !onlyMinin)
            {
                obj = BallsList.Find(ball => ball.LSDistance(Player) < catchRange);
            }
            if (obj == null)
            {
                obj = MinionManager.GetMinions(Player.ServerPosition, catchRange, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).FirstOrDefault();
            }

            if (obj != null)
            {
                foreach (var minion in MinionManager.GetMinions(Player.ServerPosition, catchRange, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth))
                {
                    if (t.LSDistance(minion) < t.LSDistance(obj))
                        obj = minion;
                }

                W.Cast(obj.Position);
            }
        }
    }
}