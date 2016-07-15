using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

 namespace OneKeyToWin_AIO_Sebby
{
    class Morgana
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell E, Q, R, W;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Menu drawMenu, qMenu, wMenu, farmMenu, rMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1150);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1000);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 750);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 600);

            Q.SetSkillshot(0.25f, 70f, 1200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.50f, 200f, 2200f, false, SkillshotType.SkillshotCircle);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw when skill rdy", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("ts", new CheckBox("Use common TargetSelector", true));
            qMenu.AddLabel("ON - only one target");
            qMenu.AddLabel("OFF - all targets");
            qMenu.Add("qCC", new CheckBox("Auto Q cc & dash enemy", true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                qMenu.Add("grab" + enemy.ChampionName, new CheckBox("Q : " + enemy.ChampionName));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("autoWcc", new CheckBox("Auto W only CC enemy", false));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmW", new CheckBox("Lane clear W", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 0, 100));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q", true));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W", true));

            MorganaE.Shield.Initialize(Config);

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("rCount", new Slider("Auto R if enemies in range", 3, 0, 5));
            rMenu.Add("rKs", new CheckBox("R ks", false));
            rMenu.Add("inter", new CheckBox("OnPossibleToInterrupt", true));
            rMenu.Add("Gap", new CheckBox("OnEnemyGapcloser", true));

            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
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

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && getCheckBoxItem(rMenu, "inter") && sender.LSIsValidTarget(R.Range))
                R.Cast();
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && getCheckBoxItem(rMenu, "Gap") && gapcloser.Sender.LSIsValidTarget(R.Range))
                R.Cast();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }
            if (Program.LagFree(1) && Q.IsReady())
                LogicQ();
            if (Program.LagFree(2) && R.IsReady())
                LogicR();
            if (Program.LagFree(3) && W.IsReady() && getCheckBoxItem(wMenu, "autoW"))
                LogicW();
        }

        private static void LogicQ()
        {
            if (Program.Combo && getCheckBoxItem(qMenu, "ts"))
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (t.LSIsValidTarget(Q.Range) && !t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield) && getCheckBoxItem(qMenu, "grab" + t.ChampionName))
                    Program.CastSpell(Q, t);
            }
            foreach (var t in Program.Enemies.Where(t => t.LSIsValidTarget(Q.Range) && getCheckBoxItem(qMenu, "grab" + t.ChampionName)))
            {
                if (!t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield))
                {
                    if (Program.Combo && !getCheckBoxItem(qMenu, "ts"))
                        Program.CastSpell(Q, t);

                    if (getCheckBoxItem(qMenu, "qCC"))
                    {
                        if (!OktwCommon.CanMove(t))
                            Q.Cast(t, true);
                        Q.CastIfHitchanceEquals(t, HitChance.Dashing);
                        Q.CastIfHitchanceEquals(t, HitChance.Immobile);
                    }
                }
            }
        }

        private static void LogicR()
        {
            bool rKs = getCheckBoxItem(rMenu, "rKs");
            foreach (var target in Program.Enemies.Where(target => target.LSIsValidTarget(R.Range) && target.HasBuff("rocketgrab2")))
            {
                if (rKs && R.GetDamage(target) > target.Health)
                    R.Cast();
            }
            if (Player.CountEnemiesInRange(R.Range) >= getSliderItem(rMenu, "rCount") && getSliderItem(rMenu, "rCount") > 0)
                R.Cast();
        }
        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                if (!getCheckBoxItem(wMenu, "autoWcc") && !Q.IsReady())
                {
                    if (W.GetDamage(t) > t.Health)
                        Program.CastSpell(W, t);
                    else if (Program.Combo && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(W, t);
                }

                foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                    W.Cast(enemy, true);
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmW") && Player.Mana > RMANA + WMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetCircularFarmLocation(minionList, W.Width);

                if (farmPosition.MinionsHit > getSliderItem(farmMenu, "LCminions"))
                    W.Cast(farmPosition.Position);
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }
                }
            }
        }

        private static bool HardCC(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned)
            {
                return true;

            }
            else
                return false;
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

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }
}