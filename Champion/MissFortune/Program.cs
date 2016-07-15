using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Orbwalking = SebbyLib.Orbwalking;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

using TargetSelector = PortAIO.TSManager; namespace OneKeyToWin_AIO_Sebby
{
    internal class MissFortune
    {
        private static readonly Menu Config = Program.Config;
        private static Spell E, Q, Q1, R, W;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static int LastAttackId;
        private static float RCastTime;

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 655f);
            Q1 = new Spell(SpellSlot.Q, 1300f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1350f);

            Q1.SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);
            Q.SetTargetted(0.25f, 1400f);
            E.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 50f, 3000f, false, SkillshotType.SkillshotCircle);

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));
            drawMenu.Add("QRange", new CheckBox("Q range", false));
            drawMenu.Add("ERange", new CheckBox("E range", false));
            drawMenu.Add("RRange", new CheckBox("R range", false));
            drawMenu.Add("noti", new CheckBox("Show notification & line"));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q"));
            qMenu.AddGroupLabel("Minion Config");
            qMenu.Add("harasQ", new CheckBox("Use Q on minion"));
            qMenu.Add("killQ", new CheckBox("Use Q only if can kill minion", false));
            qMenu.Add("qMinionMove", new CheckBox("Don't use if minions moving"));
            qMenu.Add("qMinionWidth", new Slider("secound Q angle", 80, 0, 100));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            wMenu.Add("harasW", new CheckBox("Harass W"));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E"));
            eMenu.Add("AGC", new CheckBox("AntiGapcloserE"));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R"));
            rMenu.Add("forceBlockMove", new CheckBox("Force block player"));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
                //32 == space
            rMenu.Add("disableBlock", new KeyBind("Disable R key", false, KeyBind.BindTypes.HoldActive, 'R'));
                //32 == space
            rMenu.Add("Rturrent", new CheckBox("Don't R under turret"));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("LaneClear Q"));
            farmMenu.Add("farmW", new CheckBox("LaneClear W"));
            farmMenu.Add("farmE", new CheckBox("LaneClear E"));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("Mana", new Slider("LaneClear  Mana", 80, 0, 100));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W"));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E"));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            LSEvents.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            new OktwCommon();
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && getCheckBoxItem(eMenu, "AGC") && Player.Mana > RMANA + EMANA)
            {
                var Target = gapcloser.Sender;
                if (Target.LSIsValidTarget(E.Range))
                {
                    E.Cast(gapcloser.End);
                }
            }
        }
        

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name != "MissFortuneBulletTime")
                {
                    return;
                }
                RCastTime = Game.Time;
                Program.debug(args.SData.Name);
                PortAIO.OrbwalkerManager.SetAttack(false);
                PortAIO.OrbwalkerManager.SetMovement(false);
                if (getCheckBoxItem(rMenu, "forceBlockMove"))
                {
                    PortAIO.OrbwalkerManager.SetAttack(false);
                    PortAIO.OrbwalkerManager.SetMovement(false);
                }
            }
        }

        private static void afterAttack(AfterAttackArgs args)
        {
            LastAttackId = args.Target.NetworkId;

            var t = args.Target as AIHeroClient;
            if (t != null)
            {
                if (Q.IsReady())
                {
                    if (Q.GetDamage(t) + Player.LSGetAutoAttackDamage(t) * 3 > t.Health)
                        Q.Cast(t);
                    else if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                        Q.Cast(t);
                    else if (Program.Farm && Player.Mana > RMANA + QMANA + EMANA + WMANA)
                        Q.Cast(t);
                }
                if (W.IsReady())
                {
                    if (PortAIO.OrbwalkerManager.isComboActive && Player.Mana > RMANA + WMANA && getCheckBoxItem(wMenu, "autoW"))
                        W.Cast();
                    else if (Player.Mana > RMANA + WMANA + QMANA && getCheckBoxItem(wMenu, "harasW"))
                        W.Cast();
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana"))
            {
                var minions = Cache.GetMinions(Player.ServerPosition, 600);

                if (minions.Count >= getSliderItem(farmMenu, "LCminions"))
                {
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "farmQ") && minions.Count > 1)
                        Q.Cast(minions.FirstOrDefault());
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "farmW") && minions.Count > 1)
                        W.Cast();
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ") && !SebbyLib.Orbwalking.CanAttack() && !Player.Spellbook.IsAutoAttacking)
                    {
                        Q.Cast(mob);
                        return;
                    }
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        W.Cast();
                        return;
                    }
                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (getKeyBindItem(rMenu, "disableBlock"))
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
                PortAIO.OrbwalkerManager.SetMovement(true);
                return;
            }

            if (Player.IsChannelingImportantSpell() || Game.Time - RCastTime < 0.3)
            {
                if (getCheckBoxItem(rMenu, "forceBlockMove"))
                {
                    PortAIO.OrbwalkerManager.SetAttack(false);
                    PortAIO.OrbwalkerManager.SetMovement(false);
                }

                Program.debug("cast R");
                return;
            }

            PortAIO.OrbwalkerManager.SetAttack(true);
            PortAIO.OrbwalkerManager.SetMovement(true);

            if (getCheckBoxItem(rMenu, "forceBlockMove"))
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
                PortAIO.OrbwalkerManager.SetMovement(true);
            }

            if (R.IsReady() && getKeyBindItem(rMenu, "useR"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.LSIsValidTarget(R.Range))
                {
                    Console.WriteLine("1");
                    R.Cast(t, true, true);
                    RCastTime = Game.Time;
                    return;
                }
            }

            if (Program.LagFree(1))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(2) && !ObjectManager.Player.Spellbook.IsAutoAttacking && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (Program.LagFree(3) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();

            if (Program.LagFree(4) && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();

            PortAIO.OrbwalkerManager.SetAttack(true);
            PortAIO.OrbwalkerManager.SetMovement(true);
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var t1 = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
            if (t.LSIsValidTarget(Q.Range) && Player.LSDistance(t.ServerPosition) > 500)
            {
                var qDmg = OktwCommon.GetKsDamage(t, Q);
                if (qDmg + Player.LSGetAutoAttackDamage(t) > t.Health)
                    Q.Cast(t);
                else if (qDmg + Player.LSGetAutoAttackDamage(t) * 3 > t.Health)
                    Q.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                    Q.Cast(t);
                else if (Program.Farm && Player.Mana > RMANA + QMANA + EMANA + WMANA)
                    Q.Cast(t);
            }
            else if (t1.LSIsValidTarget(Q1.Range) && getCheckBoxItem(qMenu, "harasQ") && Player.LSDistance(t1.ServerPosition) > Q.Range + 50)
            {
                var minions = Cache.GetMinions(Player.ServerPosition, Q1.Range);



                if (getCheckBoxItem(qMenu, "qMinionMove"))
                {
                    if (minions.Exists(x => x.IsMoving))
                        return;
                }

                var enemyPredictionPos = SebbyLib.Prediction.Prediction.GetPrediction(t1, 0.2f).CastPosition;
                foreach (var minion in minions)
                {
                    if (getCheckBoxItem(qMenu, "killQ") && Q.GetDamage(minion) < minion.Health)
                        continue;

                    var posExt = Player.ServerPosition.LSExtend(minion.ServerPosition, 420 + Player.LSDistance(minion));

                    if (InCone(enemyPredictionPos, posExt, minion.ServerPosition, getSliderItem(qMenu, "qMinionWidth")))
                    {
                        Program.debug("dupa");
                        if (minions.Exists(x =>
                        InCone(x.Position, posExt, minion.ServerPosition, getSliderItem(qMenu, "qMinionWidth"))
                        ))
                            continue;
                        Q.Cast(minion);
                        return;
                    }
                }
            }
        }

        private static bool InCone(Vector3 Position, Vector3 finishPos, Vector3 firstPos, int angleSet)
        {
            var range = 420;
            var angle = angleSet * (float)Math.PI / 180;
            var end2 = finishPos.LSTo2D() - firstPos.LSTo2D();
            var edge1 = end2.LSRotated(-angle / 2);
            var edge2 = edge1.LSRotated(angle);

            var point = Position.LSTo2D() - firstPos.LSTo2D();
            if (point.LSDistance(new Vector2(), true) < range * range && edge1.LSCrossProduct(point) > 0 && point.LSCrossProduct(edge2) > 0)
                return true;

            return false;
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.LSIsValidTarget())
            {
                var eDmg = OktwCommon.GetKsDamage(t, E);
                if (eDmg > t.Health)
                    Program.CastSpell(E, t);
                else if (eDmg + Q.GetDamage(t) > t.Health && Player.Mana > QMANA + EMANA + RMANA)
                    Program.CastSpell(E, t);
                else if (Program.Combo && Player.Mana > RMANA + WMANA + QMANA + EMANA)
                {
                    if (!SebbyLib.Orbwalking.InAutoAttackRange(t) || Player.CountEnemiesInRange(300) > 0 || t.CountEnemiesInRange(250) > 1)
                        Program.CastSpell(E, t);
                    else
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                            E.Cast(enemy, true, true);
                    }
                }
            }
            if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmE"))
            {
                var minions = Cache.GetMinions(Player.ServerPosition, E.Range);
                var farmPos = E.GetCircularFarmLocation(minions, E.Width);
                if (farmPos.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                {
                    E.Cast(farmPos.Position);
                }
            }
        }

        private static void LogicR()
        {
            if (Player.UnderTurret(true) && getCheckBoxItem(rMenu, "Rturrent"))
                return;

            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (t.LSIsValidTarget(R.Range) && OktwCommon.ValidUlt(t))
            {
                var rDmg = R.GetDamage(t) * new double[] { 0.5, 0.75, 1 }[R.Level - 1];

                if (Player.LSCountEnemiesInRange(700) == 0 && t.CountAlliesInRange(400) == 0)
                {
                    var tDis = Player.LSDistance(t.ServerPosition);
                    if (rDmg*7 > t.Health && tDis < 800)
                    {
                        R.Cast(t, false, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg*6 > t.Health && tDis < 900)
                    {
                        R.Cast(t, false, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg*5 > t.Health && tDis < 1000)
                    {
                        R.Cast(t, false, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg*4 > t.Health && tDis < 1100)
                    {
                        R.Cast(t, false, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg*3 > t.Health && tDis < 1200)
                    {
                        R.Cast(t, false, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg > t.Health && tDis < 1300)
                    {
                        R.Cast(t, false, true);
                        RCastTime = Game.Time;
                    }
                    return;
                }
                if (rDmg*8 > t.Health - OktwCommon.GetIncomingDamage(t) && rDmg*2 < t.Health && Player.CountEnemiesInRange(300) == 0 && !OktwCommon.CanMove(t))
                {
                    R.Cast(t, false, true);
                    RCastTime = Game.Time;
                }
            }
        }

        private static void SetMana()
        {
            if ((Program.getCheckBoxItem("manaDisable") && Program.Combo) || Player.HealthPercent < 20)
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
                RMANA = QMANA - Player.PARRegenRate*Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "noti") && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t.LSIsValidTarget())
                {
                    var rDamage = R.GetDamage(t) + W.GetDamage(t)*10;
                    if (rDamage*8 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.GreenYellow,
                            "8 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.GreenYellow);
                    }
                    else if (rDamage*5 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Orange,
                            "5 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Orange);
                    }
                    else if (rDamage*3 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Yellow,
                            "3 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Yellow);
                    }
                    else if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Red,
                            "1 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Red);
                    }
                }
            }

            if (getCheckBoxItem(drawMenu, "QRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "ERange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "RRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
            }
        }

        public static void drawText(string msg, Obj_AI_Base Hero, Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1], color, msg);
        }
    }
}