using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using TargetSelector = PortAIO.TSManager; namespace OneKeyToWin_AIO_Sebby
{
    class Lucian
    {
        private static Menu Config = Program.Config;

        private static Spell E, Q, Q1, R, R1, W;

        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        private static bool passRdy = false;
        private static float castR = Game.Time;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Core.OKTWdash Dash;
        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu, harassMenu;

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 675f);
            Q1 = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 475f);
            R = new Spell(SpellSlot.R, 1200f);
            R1 = new Spell(SpellSlot.R, 1200f);

            Q1.SetSkillshot(0.40f, 10f, float.MaxValue, true, SkillshotType.SkillshotLine);
            Q.SetTargetted(0.25f, 1400f);
            W.SetSkillshot(0.30f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.1f, 110, 2800, true, SkillshotType.SkillshotLine);
            R1.SetSkillshot(0.1f, 110, 2800, false, SkillshotType.SkillshotLine);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("harasQ", new CheckBox("Use Q on minion", true));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("ignoreCol", new CheckBox("Ignore collision", true));
            wMenu.Add("wInAaRange", new CheckBox("Cast only in AA range", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));
            eMenu.Add("slowE", new CheckBox("Auto SlowBuff E", true));
            Dash = new Core.OKTWdash(E);

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("LaneClear Q", true));
            farmMenu.Add("farmW", new CheckBox("LaneClear W", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 100, 30));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            LSEvents.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
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

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E)
            {
                passRdy = true;
            }
        }

        private static void afterAttack(AfterAttackArgs args)
        {
            //if (!target.IsMe)
                //return;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "LucianW" || args.SData.Name == "LucianE" || args.SData.Name == "LucianQ")
                {
                    passRdy = true;
                }
                else
                    passRdy = false;

                if (args.SData.Name == "LucianR")
                    castR = Game.Time;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            passRdy = false;
            if (Player.IsChannelingImportantSpell() && (int)(Game.Time * 10) % 2 == 0)
            {
                Console.WriteLine("chaneling");
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (R1.IsReady() && Game.Time - castR > 5 && getKeyBindItem(rMenu, "useR"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.IsValidTarget(R1.Range))
                {
                    R1.Cast(t);
                    return;
                }
            }
            if (Program.LagFree(0))
            {
                SetMana();

            }
            if (Program.LagFree(1) && Q.IsReady() && !passRdy && !SpellLock)
                LogicQ();
            if (Program.LagFree(2) && W.IsReady() && !passRdy && !SpellLock && getCheckBoxItem(wMenu, "autoW"))
                LogicW();
            if (Program.LagFree(3) && E.IsReady())
                LogicE();
            if (Program.LagFree(4))
            {
                if (R.IsReady() && Game.Time - castR > 5 && getCheckBoxItem(rMenu, "autoR"))
                    LogicR();

                if (!passRdy && !SpellLock)
                    farm();
            }
        }

        private static double AaDamage(AIHeroClient target)
        {
            if (Player.Level > 12)
                return Player.LSGetAutoAttackDamage(target) * 1.3;
            else if (Player.Level > 6)
                return Player.LSGetAutoAttackDamage(target) * 1.4;
            else if (Player.Level > 0)
                return Player.LSGetAutoAttackDamage(target) * 1.5;
            return 0;
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var t1 = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
            if (t.IsValidTarget(Q.Range))
            {
                if (OktwCommon.GetKsDamage(t, Q) + AaDamage(t) > t.Health)
                    Q.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Q.Cast(t);
                else if (Program.Farm && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && Player.Mana > RMANA + QMANA + EMANA + WMANA)
                    Q.Cast(t);
            }
            else if ((Program.Farm || Program.Combo) && getCheckBoxItem(qMenu, "harasQ") && t1.IsValidTarget(Q1.Range) && getCheckBoxItem(harassMenu, "harras" + t1.ChampionName) && Player.LSDistance(t1.ServerPosition) > Q.Range + 100)
            {
                if (Program.Combo && Player.Mana < RMANA + QMANA)
                    return;
                if (Program.Farm && Player.Mana < RMANA + QMANA + EMANA + WMANA)
                    return;
                if (!OktwCommon.CanHarras())
                    return;
                var prepos = Prediction.GetPrediction(t1, Q1.Delay);
                if ((int)prepos.Hitchance < 5)
                    return;
                var distance = Player.LSDistance(prepos.CastPosition);
                var minions = Cache.GetMinions(Player.ServerPosition, Q.Range);

                foreach (var minion in minions.Where(minion => minion.LSIsValidTarget(Q.Range)))
                {
                    if (prepos.CastPosition.LSDistance(Player.Position.LSExtend(minion.Position, distance)) < 25)
                    {
                        Q.Cast(minion);
                        return;
                    }
                }
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (getCheckBoxItem(wMenu, "ignoreCol") && SebbyLib.Orbwalking.InAutoAttackRange(t))
                    W.Collision = false;
                else
                    W.Collision = true;

                var qDmg = Q.GetDamage(t);
                var wDmg = OktwCommon.GetKsDamage(t, W);

                if (SebbyLib.Orbwalking.InAutoAttackRange(t))
                {
                    qDmg += (float)AaDamage(t);
                    wDmg += (float)AaDamage(t);
                }

                if (wDmg > t.Health)
                    Program.CastSpell(W, t);
                else if (wDmg + qDmg > t.Health && Q.IsReady() && Player.Mana > RMANA + WMANA + QMANA)
                    Program.CastSpell(W, t);

                var orbT = PortAIO.OrbwalkerManager.LastTarget() as AIHeroClient;
                if (orbT == null)
                {
                    if (getCheckBoxItem(wMenu, "wInAaRange"))
                    {
                        return;
                    }
                }
                else if (orbT.LSIsValidTarget())
                {
                    t = orbT;
                }


                if (Program.Combo && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                    Program.CastSpell(W, t);
                else if (Program.Farm && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && !Player.UnderTurret(true) && Player.Mana > Player.MaxMana * 0.8 && Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA)
                    Program.CastSpell(W, t);
                else if ((Program.Combo || Program.Farm) && Player.Mana > RMANA + WMANA + EMANA)
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
        }

        private static void LogicR()
        {
            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (t.IsValidTarget(R.Range) && t.CountAlliesInRange(500) == 0 && OktwCommon.ValidUlt(t) && !SebbyLib.Orbwalking.InAutoAttackRange(t))
            {
                var rDmg = R.GetDamage(t, 1) * (10 + 5 * R.Level);

                var tDis = Player.LSDistance(t.ServerPosition);
                if (rDmg * 0.8 > t.Health && tDis < 700 && !Q.IsReady())
                    R.Cast(t, true, true);
                else if (rDmg * 0.7 > t.Health && tDis < 800)
                    R.Cast(t, true, true);
                else if (rDmg * 0.6 > t.Health && tDis < 900)
                    R.Cast(t, true, true);
                else if (rDmg * 0.5 > t.Health && tDis < 1000)
                    R.Cast(t, true, true);
                else if (rDmg * 0.4 > t.Health && tDis < 1100)
                    R.Cast(t, true, true);
                else if (rDmg * 0.3 > t.Health && tDis < 1200)
                    R.Cast(t, true, true);
                return;
            }
        }

        private static void LogicE()
        {
            if (Player.Mana < RMANA + EMANA || !getCheckBoxItem(eMenu, "autoE"))
                return;

            if (Program.Enemies.Any(target => target.LSIsValidTarget(270) && target.IsMelee))
            {
                var dashPos = Dash.CastDash(true);
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }
            else
            {
                if (!Program.Combo || passRdy || SpellLock)
                    return;

                var dashPos = Dash.CastDash();
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }
        }

        public static void farm()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady())
                    {
                        Q.Cast(mob);
                        return;
                    }

                    if (W.IsReady())
                    {
                        W.Cast(mob);
                        return;
                    }
                }

                if (Player.ManaPercent > getSliderItem(farmMenu, "Mana"))
                {

                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "farmQ"))
                    {
                        var minions = Cache.GetMinions(Player.ServerPosition, Q1.Range);
                        foreach (var minion in minions)
                        {
                            var poutput = Q1.GetPrediction(minion);
                            var col = poutput.CollisionObjects;

                            if (col.Count() > 2)
                            {
                                var minionQ = col.First();
                                if (minionQ.LSIsValidTarget(Q.Range))
                                {
                                    Q.Cast(minion);
                                    return;
                                }
                            }
                        }
                    }
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "farmW"))
                    {
                        var minions = Cache.GetMinions(Player.ServerPosition, Q1.Range);
                        var Wfarm = W.GetCircularFarmLocation(minions, 150);
                        if (Wfarm.MinionsHit > 3)
                            W.Cast(Wfarm.Position);
                    }
                }
            }
        }


        private static bool SpellLock
        {
            get
            {
                if (Player.HasBuff("lucianpassivebuff"))
                    return true;
                else
                    return false;
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
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] - 200, color, msg);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q1.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q1.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }
}
