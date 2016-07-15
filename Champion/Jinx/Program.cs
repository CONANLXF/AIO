using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using TargetSelector = PortAIO.TSManager;

namespace OneKeyToWin_AIO_Sebby
{

    class Jinx
    {
        private Menu Config = Program.Config;
        
        public LeagueSharp.Common.Spell Q, W, E, R;
        public float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        public double lag = 0, WCastTime = 0, QCastTime = 0, DragonTime = 0, grabTime = 0;
        public float DragonDmg = 0;

        public AIHeroClient Player { get { return ObjectManager.Player; } }

        public void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1500f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 920f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 3000f);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 100f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);

            LoadMenuOKTW();
            Game.OnUpdate += Game_OnUpdate;
            LSEvents.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static Menu qMenu, wMenu, eMenu, rMenu, farmMenu, drawMenu;

        private void LoadMenuOKTW()
        {
            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("noti", new CheckBox("Show notification", false));
            drawMenu.Add("semi", new CheckBox("Semi-manual R target", false));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.AddGroupLabel("Harass : ");
            foreach (var enemy in HeroManager.Enemies)
                wMenu.Add("haras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("Qharras", new CheckBox("Harass Q", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E on CC", true));
            eMenu.Add("comboE", new CheckBox("Auto E in Combo BETA", true));
            eMenu.Add("AGC", new CheckBox("AntiGapcloserE", true));
            eMenu.Add("opsE", new CheckBox("OnProcessSpellCastE", true));
            eMenu.Add("telE", new CheckBox("Auto E teleport", true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("Rjungle", new CheckBox("R Jungle stealer", true));
            rMenu.Add("Rdragon", new CheckBox("Dragon", true));
            rMenu.Add("Rbaron", new CheckBox("Baron", true));
            rMenu.Add("hitchanceR", new Slider("Hit Chance R", 2, 0, 3));
            rMenu.Add("useR", new KeyBind("OneKeyToCast R", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("Rturrent", new CheckBox("Don't R under turret", true));

            farmMenu = Config.AddSubMenu("Farm Menu");
            farmMenu.Add("farmQout", new CheckBox("Q farm out range AA", true));
            farmMenu.Add("farmQ", new CheckBox("Q LaneClear Q", true));
            farmMenu.Add("Mana", new Slider("LaneClear Q Mana", 80, 30, 100));

        }

        private void BeforeAttack(BeforeAttackArgs args)
        {
            if (!Q.IsReady() || !qMenu["autoQ"].Cast<CheckBox>().CurrentValue || !FishBoneActive)
                return;

            var t = args.Target as AIHeroClient;

            if (t != null)
            {
                var realDistance = GetRealDistance(t) - 50;
                if (Program.Combo && (realDistance < GetRealPowPowRange(t) || (Player.Mana < RMANA + 20 && Player.GetAutoAttackDamage(t) * 3 < t.Health)))
                    Q.Cast();
                else if (Program.Farm && qMenu["Qharras"].Cast<CheckBox>().CurrentValue && (realDistance > bonusRange() || realDistance < GetRealPowPowRange(t) || Player.Mana < RMANA + EMANA + WMANA + WMANA))
                    Q.Cast();
            }

            var minion = args.Target as Obj_AI_Minion;
            if (Program.Farm && minion != null)
            {
                var realDistance = GetRealDistance(minion);

                if (realDistance < GetRealPowPowRange(minion) || Player.ManaPercent < farmMenu["Mana"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast();
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (eMenu["AGC"].Cast<CheckBox>().CurrentValue && E.IsReady() && Player.Mana > RMANA + EMANA)
            {
                var Target = gapcloser.Sender;
                if (Target.LSIsValidTarget(E.Range))
                {
                    E.Cast(gapcloser.End);
                }
                return;
            }
            return;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (unit.IsMinion)
                return;

            if (unit.IsMe)
            {
                if (args.SData.Name == "JinxWMissile")
                    WCastTime = Game.Time;
            }
            if (E.IsReady())
            {
                if (unit.IsEnemy && eMenu["opsE"].Cast<CheckBox>().CurrentValue && unit.LSIsValidTarget(E.Range) && ShouldUseE(args.SData.Name))
                {
                    E.Cast(unit.ServerPosition, true);
                }
                if (unit.IsAlly && args.SData.Name == "RocketGrab" && Player.LSDistance(unit.Position) < E.Range)
                {
                    grabTime = Game.Time;
                }
            }

        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (R.IsReady())
            {
                if (rMenu["useR"].Cast<KeyBind>().CurrentValue)
                {
                    var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (t.LSIsValidTarget())
                        R.Cast(t, true, true);
                }
                if (rMenu["Rjungle"].Cast<CheckBox>().CurrentValue)
                {
                    KsJungle();
                }
            }

            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (E.IsReady())
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && qMenu["autoQ"].Cast<CheckBox>().CurrentValue)
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && !ObjectManager.Player.Spellbook.IsAutoAttacking && wMenu["autoW"].Cast<CheckBox>().CurrentValue)
                LogicW();

            if (Program.LagFree(4) && R.IsReady())
                LogicR();
        }

        private void LogicQ()
        {
            if (Program.Farm && !FishBoneActive && !ObjectManager.Player.Spellbook.IsAutoAttacking && PortAIO.OrbwalkerManager.LastTarget() == null && SebbyLib.Orbwalking.CanAttack() && farmMenu["farmQout"].Cast<CheckBox>().CurrentValue && Player.Mana > RMANA + WMANA + EMANA + 10)
            {
                foreach (var minion in Cache.GetMinions(Player.Position, bonusRange() + 30).Where(
                minion => !SebbyLib.Orbwalking.InAutoAttackRange(minion) && GetRealPowPowRange(minion) < GetRealDistance(minion) && bonusRange() < GetRealDistance(minion)))
                {
                    var hpPred = SebbyLib.HealthPrediction.GetHealthPrediction(minion, 400, 70);
                    if (hpPred < Player.GetAutoAttackDamage(minion) * 1.1 && hpPred > 5)
                    {
                        PortAIO.OrbwalkerManager.ForcedTarget(minion);
                        Q.Cast();
                        return;
                    }
                }
            }

            var t = TargetSelector.GetTarget(bonusRange() + 60, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                if (!FishBoneActive && (!SebbyLib.Orbwalking.InAutoAttackRange(t) || t.CountEnemiesInRange(250) > 2) && PortAIO.OrbwalkerManager.LastTarget() == null)
                {
                    var distance = GetRealDistance(t);
                    if (Program.Combo && (Player.Mana > RMANA + WMANA + 10 || Player.GetAutoAttackDamage(t) * 3 > t.Health))
                        Q.Cast();
                    else if (Program.Farm && !ObjectManager.Player.Spellbook.IsAutoAttacking && SebbyLib.Orbwalking.CanAttack() && qMenu["Qharras"].Cast<CheckBox>().CurrentValue && !ObjectManager.Player.UnderTurret(true) && Player.Mana > RMANA + WMANA + EMANA + 20 && distance < bonusRange() + t.BoundingRadius + Player.BoundingRadius)
                        Q.Cast();
                }
            }
            else if (!FishBoneActive && Program.Combo && Player.Mana > RMANA + WMANA + 20 && Player.LSCountEnemiesInRange(2000) > 0)
                Q.Cast();
            else if (FishBoneActive && Program.Combo && Player.Mana < RMANA + WMANA + 20)
                Q.Cast();
            else if (FishBoneActive && Program.Combo && Player.LSCountEnemiesInRange(2000) == 0)
                Q.Cast();
            else if (FishBoneActive && (Program.Farm || PortAIO.OrbwalkerManager.isLastHitActive))
            {
                Q.Cast();
            }
        }

        /// <summary>
        /// Calculates the Damage done with R - KARMAPANDA
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>Returns the Damage done with useR</returns>
        private static float RDamage(Obj_AI_Base target)
        {
            var distance = ObjectManager.Player.Distance(target);
            var increment = Math.Floor(distance / 100f);

            if (increment > 15)
            {
                increment = 15;
            }

            var extraPercent = Math.Floor((10f + (increment * 6f))) / 10f;

            if (extraPercent > 10)
            {
                extraPercent = 10;
            }

            var damage = (new[] { 0f, 25f, 35f, 45f }[Program.R.Level] * (extraPercent)) +
                         ((extraPercent / 100f) * ObjectManager.Player.FlatPhysicalDamageMod) +
                         ((new[] { 0f, 0.25f, 0.3f, 0.35f }[Program.R.Level] * (target.MaxHealth - target.Health)));

            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical, (float)damage);
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.LSIsValidTarget() && t.IsHPBarRendered && t.IsVisible)
            {

                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && enemy.LSDistance(Player) > bonusRange() && enemy.IsHPBarRendered && enemy.IsVisible))
                {
                    var comboDmg = OktwCommon.GetKsDamage(enemy, W);
                    if (R.IsReady() && Player.Mana > RMANA + WMANA + 20)
                    {
                        comboDmg += RDamage(enemy);
                    }
                    if (comboDmg > enemy.Health && OktwCommon.ValidUlt(enemy))
                    {
                        Program.CastSpell(W, enemy);
                        return;
                    }
                }


                if (Player.LSCountEnemiesInRange(bonusRange()) == 0)
                {
                    if (Program.Combo && Player.Mana > RMANA + WMANA + 10)
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && GetRealDistance(enemy) > bonusRange()).OrderBy(enemy => enemy.Health))
                            Program.CastSpell(W, enemy);
                    }
                    else if (Program.Farm && Player.Mana > RMANA + EMANA + WMANA + WMANA + 40 && OktwCommon.CanHarras())
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && wMenu["haras" + enemy.ChampionName].Cast<CheckBox>().CurrentValue))
                            Program.CastSpell(W, enemy);
                    }
                }
                if (!Program.None && Player.Mana > RMANA + WMANA && Player.LSCountEnemiesInRange(GetRealPowPowRange(t)) == 0)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && !OktwCommon.CanMove(enemy) && enemy.IsHPBarRendered && enemy.IsVisible))
                        W.Cast(enemy, true);
                }
            }
        }

        private void LogicE()
        {
            if (Player.Mana > RMANA + EMANA && eMenu["autoE"].Cast<CheckBox>().CurrentValue && Game.Time - grabTime > 1)
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                {
                    E.Cast(enemy.Position);
                    return;
                }
                if (!Program.LagFree(1))
                    return;

                if (eMenu["telE"].Cast<CheckBox>().CurrentValue)
                {
                    var trapPos = OktwCommon.GetTrapPos(E.Range);
                    if (!trapPos.IsZero)
                        E.Cast(trapPos);
                }

                if (Program.Combo && Player.IsMoving && eMenu["comboE"].Cast<CheckBox>().CurrentValue && Player.Mana > RMANA + EMANA + WMANA)
                {
                    var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                    if (t.LSIsValidTarget(E.Range) && E.GetPrediction(t).CastPosition.LSDistance(t.Position) > 200 && (int)E.GetPrediction(t).Hitchance == 5)
                    {
                        E.CastIfWillHit(t, 2);
                        if (t.HasBuffOfType(BuffType.Slow))
                        {
                            Program.CastSpell(E, t);
                        }
                        else
                        {
                            if (E.GetPrediction(t).CastPosition.LSDistance(t.Position) > 200)
                            {
                                if (Player.Position.LSDistance(t.ServerPosition) > Player.Position.LSDistance(t.Position))
                                {
                                    if (t.Position.LSDistance(Player.ServerPosition) < t.Position.LSDistance(Player.Position))
                                        Program.CastSpell(E, t);
                                }
                                else
                                {
                                    if (t.Position.LSDistance(Player.ServerPosition) > t.Position.LSDistance(Player.Position))
                                        Program.CastSpell(E, t);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LogicR()
        {
            if (Player.UnderTurret(true) && rMenu["Rturrent"].Cast<CheckBox>().CurrentValue)
                return;
            if (Game.Time - WCastTime > 0.9 && rMenu["autoR"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.LSIsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
                {
                    if (!target.IsVisible || !target.IsHPBarRendered)
                    {
                        return;
                    }
                    var predictedHealth = target.Health - OktwCommon.GetIncomingDamage(target);
                    var Rdmg = RDamage(target);

                    if (Rdmg > predictedHealth && !OktwCommon.IsSpellHeroCollision(target, R) && GetRealDistance(target) > bonusRange() + 200)
                    {
                        if (GetRealDistance(target) > bonusRange() + 300 + target.BoundingRadius && target.CountAlliesInRange(600) == 0 && Player.CountEnemiesInRange(400) == 0)
                        {
                            castR(target);
                        }
                        else if (target.LSCountEnemiesInRange(200) > 2)
                        {
                            R.Cast(target, true, true);
                        }
                    }
                }
            }
        }

        private void castR(AIHeroClient target)
        {
            if (!target.IsVisible || !target.IsHPBarRendered)
            {
                return;
            }
            var inx = rMenu["hitchanceR"].Cast<Slider>().CurrentValue;
            if (inx == 0)
            {
                R.Cast(R.GetPrediction(target).CastPosition);
            }
            else if (inx == 1)
            {
                R.Cast(target);
            }
            else if (inx == 2)
            {
                Program.CastSpell(R, target);
            }
            else if (inx == 3)
            {
                List<Vector2> waypoints = target.GetWaypoints();
                if ((Player.LSDistance(waypoints.Last<Vector2>().To3D()) - Player.LSDistance(target.Position)) > 400)
                {
                    Program.CastSpell(R, target);
                }
            }
        }

        private float bonusRange() { return 670f + Player.BoundingRadius + 25 * Player.Spellbook.GetSpell(SpellSlot.Q).Level; }

        private bool FishBoneActive { get { return Player.HasBuff("JinxQ"); } }

        private float GetRealPowPowRange(GameObject target)
        {
            return 650f + Player.BoundingRadius + target.BoundingRadius;

        }

        private float GetRealDistance(Obj_AI_Base target)
        {
            return Player.ServerPosition.LSDistance(EloBuddy.SDK.Prediction.Position.PredictUnitPosition(target, (int)0.05f).To3D()) + Player.BoundingRadius + target.BoundingRadius;
        }

        public bool ShouldUseE(string SpellName)
        {
            switch (SpellName)
            {
                case "ThreshQ":
                    return true;
                case "KatarinaR":
                    return true;
                case "AlZaharNetherGrasp":
                    return true;
                case "GalioIdolOfDurand":
                    return true;
                case "LuxMaliceCannon":
                    return true;
                case "MissFortuneBulletTime":
                    return true;
                case "RocketGrabMissile":
                    return true;
                case "CaitlynPiltoverPeacemaker":
                    return true;
                case "EzrealTrueshotBarrage":
                    return true;
                case "InfiniteDuress":
                    return true;
                case "VelkozR":
                    return true;
            }
            return false;
        }

        private void KsJungle()
        {
            var mobs = Cache.GetMinions(Player.ServerPosition, float.MaxValue, MinionTeam.Neutral);
            foreach (var mob in mobs)
            {
                //debug(mob.SkinName);
                if (mob.Health < mob.MaxHealth && ((mob.BaseSkinName == "SRU_Dragon" && rMenu["Rdragon"].Cast<CheckBox>().CurrentValue)
                    || (mob.BaseSkinName == "SRU_Baron" && rMenu["Rbaron"].Cast<CheckBox>().CurrentValue))
                    && mob.CountAlliesInRange(1000) == 0
                    && mob.LSDistance(Player.Position) > 1000 && mob.IsVisible && mob.IsHPBarRendered)
                {
                    if (DragonDmg == 0)
                        DragonDmg = mob.Health;

                    if (Game.Time - DragonTime > 4)
                    {
                        if (DragonDmg - mob.Health > 0)
                        {
                            DragonDmg = mob.Health;
                        }
                        DragonTime = Game.Time;
                    }

                    else
                    {
                        var DmgSec = (DragonDmg - mob.Health) * (Math.Abs(DragonTime - Game.Time) / 4);
                        //debug("DS  " + DmgSec);
                        if (DragonDmg - mob.Health > 0)
                        {
                            var timeTravel = GetUltTravelTime(Player, R.Speed, R.Delay, mob.Position);
                            var timeR = (mob.Health - Player.CalcDamage(mob, DamageType.Physical, (250 + (100 * R.Level)) + Player.FlatPhysicalDamageMod + 300)) / (DmgSec / 4);
                            //debug("timeTravel " + timeTravel + "timeR " + timeR + "d " + ((150 + (100 * R.Level + 200) + Player.FlatPhysicalDamageMod)));
                            if (timeTravel > timeR)
                                R.Cast(mob.Position);
                        }
                        else
                        {
                            DragonDmg = mob.Health;
                        }
                        //debug("" + GetUltTravelTime(Player, R.Speed, R.Delay, mob.Position));
                    }
                }
            }
        }

        private float GetUltTravelTime(AIHeroClient source, float speed, float delay, Vector3 targetpos)
        {
            float distance = Vector3.Distance(source.ServerPosition, targetpos);
            float missilespeed = speed;
            if (source.ChampionName == "Jinx" && distance > 1350)
            {
                const float accelerationrate = 0.3f; //= (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second
                var acceldifference = distance - 1350f;
                if (acceldifference > 150f) //it only accelerates 150 units
                    acceldifference = 150f;
                var difference = distance - 1500f;
                missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference) + difference * 2200f) / distance;
            }
            return (distance / missilespeed + delay);
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

            QMANA = 10;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = WMANA - Player.PARRegenRate * W.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private void Drawing_OnDraw(EventArgs args)
        {

            if (drawMenu["qRange"].Cast<CheckBox>().CurrentValue)
            {
                if (FishBoneActive)
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, 590f + Player.BoundingRadius, System.Drawing.Color.DeepPink, 1, 1);
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, bonusRange() - 29, System.Drawing.Color.DeepPink, 1, 1);
            }
            if (drawMenu["wRange"].Cast<CheckBox>().CurrentValue)
            {
                if (drawMenu["onlyRdy"].Cast<CheckBox>().CurrentValue)
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (drawMenu["eRange"].Cast<CheckBox>().CurrentValue)
            {
                if (drawMenu["onlyRdy"].Cast<CheckBox>().CurrentValue)
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
            }
            if (drawMenu["noti"].Cast<CheckBox>().CurrentValue)
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (R.IsReady() && t.LSIsValidTarget() && RDamage(t) > t.Health)
                {
                    Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                    drawLine(t.Position, Player.Position, 5, System.Drawing.Color.Red);
                }
                else if (t.LSIsValidTarget(2000) && W.GetDamage(t) > t.Health)
                {
                    Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "W can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                    drawLine(t.Position, Player.Position, 3, System.Drawing.Color.Yellow);
                }
            }
        }
    }
}