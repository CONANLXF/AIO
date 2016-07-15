using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;


using TargetSelector = PortAIO.TSManager; namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Varus
    {
        private Menu Config = Program.Config;
        public static Menu drawMenu, qMenu, eMenu, rMenu, farmMenu, harassMenu;

        private Spell Q, W, E, R;
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public AIHeroClient Player { get { return ObjectManager.Player; } }
        public float AArange = ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius * 2;
        float CastTime = Game.Time;
        bool CanCast = true;

        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 925);
            W = new Spell(SpellSlot.Q, 0);
            E = new Spell(SpellSlot.E, 975);
            R = new Spell(SpellSlot.R, 1050);

            Q.SetSkillshot(0.25f, 70, 1650, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.35f, 120, 1500, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 120, 1950, false, SkillshotType.SkillshotLine);
            Q.SetCharged("VarusQ", "VarusQ", 925, 1600, 1.5f);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("maxQ", new CheckBox("Cast Q only max range", true));
            qMenu.Add("fastQ", new CheckBox("Fast cast Q", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));


            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("rCount", new Slider("Auto R if enemies in range (combo mode)", 3, 0, 5));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                rMenu.Add("GapCloser" + enemy.ChampionName, new CheckBox("Gapclose : " + enemy.ChampionName, true));


            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox("Harass : " + enemy.ChampionName, true));


            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q", true));
            farmMenu.Add("farmE", new CheckBox("Lane clear E", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 100, 0));

            Game.OnUpdate += Game_OnGameUpdate;

            Drawing.OnDraw += Drawing_OnDraw;
            //SebbyLib.Orbwalking.BeforeAttack += BeforeAttack;
            //SebbyLib.Orbwalking.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            //Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

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

        private void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
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

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && getCheckBoxItem(rMenu, "GapCloser" + gapcloser.Sender.ChampionName))
            {
                var Target = gapcloser.Sender;
                if (Target.IsValidTarget(R.Range))
                {
                    R.Cast(Target.ServerPosition, true);
                    //Program.debug("AGC " );
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "VarusQ" || args.SData.Name == "VarusE" || args.SData.Name == "VarusR")
                {
                    CastTime = Game.Time;
                    CanCast = false;
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (R.IsReady())
            {
                if (getKeyBindItem(rMenu, "useR"))
                {
                    var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (t.IsValidTarget())
                        R.Cast(t);
                }
            }
            if (Program.LagFree(0))
            {
                SetMana();
                if (!CanCast)
                {
                    if (Game.Time - CastTime > 1)
                    {
                        CanCast = true;
                        return;
                    }
                    var t = PortAIO.OrbwalkerManager.LastTarget() as Obj_AI_Base;
                    if (t.IsValidTarget())
                    {
                        if (OktwCommon.GetBuffCount(t, "varuswdebuff") < 3)
                            CanCast = true;
                    }
                    else
                    {
                        CanCast = true;
                    }
                }
            }


            if (Program.LagFree(1) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ") && !Player.Spellbook.IsAutoAttacking)
            {
                LogicQ();
            }

            if (Program.LagFree(2) && E.IsReady() && getCheckBoxItem(eMenu, "autoE") && !Player.Spellbook.IsAutoAttacking)
                LogicE();
            if (Program.LagFree(3) && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
            if (Program.LagFree(4))
                Farm();
        }

        private void Farm()
        {
            if (Program.LaneClear && E.IsReady() && getCheckBoxItem(farmMenu, "farmE"))
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
                if (mobs.Count > 0 && Player.Mana > RMANA + EMANA + QMANA && OktwCommon.GetBuffCount(mobs[0], "varuswdebuff") == 3)
                {
                    E.Cast(mobs[0]);
                    return;
                }

                if (Player.ManaPercent > getSliderItem(farmMenu, "Mana"))
                {
                    var allMinionsE = Cache.GetMinions(Player.ServerPosition, E.Range);
                    var Efarm = Q.GetCircularFarmLocation(allMinionsE, E.Width);
                    if (Efarm.MinionsHit > 3)
                    {
                        E.Cast(Efarm.Position);
                        return;
                    }
                }
            }
        }

        private void LogicR()
        {
            foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(R.Range)))
            {

                if (enemy.CountEnemiesInRange(400) >= getSliderItem(rMenu, "rCount") && getSliderItem(rMenu, "rCount") > 0)
                {
                    R.Cast(enemy, true, true);
                    Program.debug("R AOE");
                }
                if ((enemy.CountAlliesInRange(600) == 0 || Player.Health < Player.MaxHealth * 0.5) && R.GetDamage(enemy) + GetWDmg(enemy) + Q.GetDamage(enemy) > enemy.Health && OktwCommon.ValidUlt(enemy))
                {
                    Program.CastSpell(R, enemy);
                    Program.debug("R KS");
                }
            }
            if (Player.Health < Player.MaxHealth * 0.5)
            {
                foreach (var target in Program.Enemies.Where(target => target.IsValidTarget(270) && target.IsMelee && getCheckBoxItem(rMenu, "GapCloser" + target.ChampionName)))
                {
                    Program.CastSpell(R, target);
                }
            }
        }

        private void LogicQ()
        {

            foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(1600) && Q.GetDamage(enemy) + GetWDmg(enemy) > enemy.Health))
            {
                if (enemy.IsValidTarget(R.Range))
                    CastQ(enemy);
                return;
            }

            if (getCheckBoxItem(qMenu, "maxQ") && (Q.Range < 1500) && Player.CountEnemiesInRange(AArange) == 0)
                return;

            var t = Orbwalker.ForcedTarget as AIHeroClient;
            if (!t.IsValidTarget())
                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                if (Q.IsCharging)
                {
                    if (getCheckBoxItem(qMenu, "fastQ"))
                        Q.Cast(Q.GetPrediction(t).CastPosition);

                    if (GetQEndTime() > 2)
                        Program.CastSpell(Q, t);
                    else
                        Q.Cast(Q.GetPrediction(t).CastPosition);
                    return;
                }

                if ((OktwCommon.GetBuffCount(t, "varuswdebuff") == 3 && CanCast && !E.IsReady()) || !SebbyLib.Orbwalking.InAutoAttackRange(t))
                {
                    if ((Program.Combo || (OktwCommon.GetBuffCount(t, "varuswdebuff") == 3 && Program.Farm)) && Player.Mana > RMANA + QMANA)
                    {
                        CastQ(t);
                    }
                    else if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA + QMANA && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && !Player.UnderTurret(true) && OktwCommon.CanHarras())
                    {
                        CastQ(t);
                    }
                    else if (!Program.None && Player.Mana > RMANA + WMANA)
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                            CastQ(enemy);
                    }
                }
            }
            else if (Program.LaneClear && getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA + WMANA && Q.Range > 1500 && Player.CountEnemiesInRange(1450) == 0 && (Q.IsCharging || (Player.ManaPercent > getSliderItem(farmMenu, "Mana"))))
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var Qfarm = Q.GetLineFarmLocation(allMinionsQ, Q.Width);
                if (Qfarm.MinionsHit > 3 || (Q.IsCharging && Qfarm.MinionsHit > 0))
                    Q.Cast(Qfarm.Position);
            }
        }

        private void LogicE()
        {
            foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && E.GetDamage(enemy) + GetWDmg(enemy) > enemy.Health))
            {
                Program.CastSpell(E, enemy);
            }
            var t = Orbwalker.ForcedTarget as AIHeroClient;
            if (!t.IsValidTarget())
                t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if ((OktwCommon.GetBuffCount(t, "varuswdebuff") == 3 && CanCast) || !SebbyLib.Orbwalking.InAutoAttackRange(t))
                {
                    if (Program.Combo && Player.Mana > RMANA + QMANA)
                    {
                        Program.CastSpell(E, t);
                    }
                    else if ((Program.Combo || Program.Farm) && Player.Mana > RMANA + WMANA)
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                            E.Cast(enemy);
                    }
                }
            }
        }

        private float GetQEndTime()
        {
            return
                Player.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Name == "VarusQ")
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault() - Game.Time;
        }

        private float GetWDmg(Obj_AI_Base target)
        {
            return (OktwCommon.GetBuffCount(target, "varuswdebuff") * W.GetDamage(target, 1));
        }

        private void CastQ(Obj_AI_Base target)
        {
            if (!Q.IsCharging)
            {
                if (target.IsValidTarget(Q.Range - 300))
                    Q.StartCharging();
            }
            else
            {
                if (GetQEndTime() > 1)
                    Program.CastSpell(Q, target);
                else
                    Q.Cast(Q.GetPrediction(target).CastPosition);
                return;
            }
        }

        private void SetMana()
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
    }
}