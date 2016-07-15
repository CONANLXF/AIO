#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;
using SharpDX;
using Font = SharpDX.Direct3D9.Font;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

#endregion

 namespace Marksman.Champions
{
    internal class Tristana : Champion
    {
        public static AIHeroClient Player = ObjectManager.Player;
        public static LeagueSharp.Common.Spell Q, W, E, R;

        public Tristana()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 703);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 900);
            W.SetSkillshot(.50f, 250f, 1400f, false, SkillshotType.SkillshotCircle);

            E = new LeagueSharp.Common.Spell(SpellSlot.E, 703);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 703);
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            //font = new Font(
            //    Drawing.Direct3DDevice,
            //    new FontDescription
            //    {
            //        FaceName = "Segoe UI",
            //        Height = 25,
            //        OutputPrecision = FontPrecision.Default,
            //        Quality = FontQuality.Default
            //    });
            //fontsmall = new Font(
            //    Drawing.Direct3DDevice,
            //    new FontDescription
            //    {
            //        FaceName = "Segoe UI",
            //        Height = 15,
            //        OutputPrecision = FontPrecision.Default,
            //        Quality = FontQuality.Default
            //    });

            Utils.Utils.PrintMessage("Tristana loaded.");
        }

        public void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && gapcloser.Sender.LSIsValidTarget(R.Range) && Program.misc["UseRMG"].Cast<CheckBox>().CurrentValue)
                R.CastOnUnit(gapcloser.Sender);
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && unit.LSIsValidTarget(R.Range) && Program.misc["UseRMI"].Cast<CheckBox>().CurrentValue)
                R.CastOnUnit(unit);
        }

        public override void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Program.misc["Misc.UseQ.Inhibitor"].Cast<CheckBox>().CurrentValue && args.Target is Obj_BarracksDampener && Q.IsReady())
            {
                if (((Obj_BarracksDampener)args.Target).Health >= Player.TotalAttackDamage * 3)
                {
                    Q.Cast();
                }
            }

            if (Program.misc["Misc.UseQ.Nexus"].Cast<CheckBox>().CurrentValue && args.Target is Obj_HQ && Q.IsReady())
            {
                Q.Cast();
            }

            var unit = args.Target as Obj_AI_Turret;
            if (unit != null)
            {
                if (Program.misc["UseEM"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    if (((Obj_AI_Turret)args.Target).Health >= Player.TotalAttackDamage * 3)
                    {
                        E.CastOnUnit(unit);
                    }
                }

                if (Program.misc["Misc.UseQ.Turret"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    if (((Obj_AI_Turret)args.Target).Health >= Player.TotalAttackDamage * 3)
                    {
                        Q.Cast();
                    }
                }
            }
            if (args.Target is AIHeroClient)
            {
                var t = args.Target as AIHeroClient;
                if (t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null)) && ComboActive)
                {
                    var useQ = Q.IsReady() && Program.combo["UseQC"].Cast<CheckBox>().CurrentValue;
                    if (useQ)
                        Q.CastOnUnit(Player);
                }
            }
        }
        
        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (!Orbwalker.CanMove)
            {
                return;
            }

            var getEMarkedEnemy = TristanaData.GetEMarkedEnemy;
            if (getEMarkedEnemy != null)
            {
                Orbwalker.ForcedTarget =((getEMarkedEnemy));
            }
            else
            {
                var attackRange = Orbwalking.GetRealAutoAttackRange(Player);
                Orbwalker.ForcedTarget =(TargetSelector.GetTarget(attackRange, DamageType.Physical));
            }

            Q.Range = 600 + 5 * (Player.Level - 1);
            E.Range = 630 + 7 * (Player.Level - 1);
            R.Range = 630 + 7 * (Player.Level - 1);

            if (!Player.HasBuff("Recall") && Program.harass["UseETH"].Cast<KeyBind>().CurrentValue && ToggleActive && E.IsReady())
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);

                if (t.LSIsValidTarget(E.Range))
                {
                    if (Program.harass["DontEToggleHarass" + t.ChampionName] != null &&
                        Program.harass["DontEToggleHarass" + t.ChampionName].Cast<CheckBox>().CurrentValue == false)
                    {
                        E.CastOnUnit(t);
                    }
                }
            }

            var useW = W.IsReady() && Program.combo["UseWC"].Cast<CheckBox>().CurrentValue;
            var useWc = W.IsReady() && Program.combo["UseWCS"].Cast<CheckBox>().CurrentValue;
            var useWks = W.IsReady() && Program.combo["UseWKs"].Cast<CheckBox>().CurrentValue;
            var useE = E.IsReady() && Program.combo["UseEC"].Cast<CheckBox>().CurrentValue;
            var useR = R.IsReady() && Program.misc["UseRM"].Cast<CheckBox>().CurrentValue;

            if (ComboActive)
            {
                AIHeroClient t;
                if (TristanaData.GetEMarkedEnemy != null)
                {
                    t = TristanaData.GetEMarkedEnemy;
                    Orbwalker.ForcedTarget =(TristanaData.GetEMarkedEnemy);
                }
                else
                {
                    t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                }

                if (useE && E.IsReady())
                {
                    if (E.IsReady() && t.LSIsValidTarget(E.Range))
                        E.CastOnUnit(t);
                }

                if (useW)
                {
                    t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                    if (t.LSIsValidTarget())
                        W.Cast(t);
                }
                /*
                else if (useWks)
                {
                    t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                    if (t.LSIsValidTarget() && t.Health < TristanaData.GetWDamage)
                        W.Cast(t);
                }
                else if (useWc)
                {
                    t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                    if (t.LSIsValidTarget() && TristanaData.GetEMarkedCount == 4)
                        W.Cast(t);
                }
                */
            }

            if (ComboActive)
            {
                if (useR)
                {
                    var t = TargetSelector.GetTarget(R.Range - 10, DamageType.Physical);

                    if (!t.LSIsValidTarget())
                        return;

                    if (Player.LSGetSpellDamage(t, SpellSlot.R) - 30 < t.Health ||
                        t.Health < Player.LSGetAutoAttackDamage(t, true))
                        return;

                    R.CastOnUnit(t);
                }
            }
        }

        public override void ExecuteJungleClear()
        {
            var jungleMobs = Marksman.Utils.Utils.GetMobs(E.Range, Marksman.Utils.Utils.MobTypes.All);

            if (jungleMobs != null)
            {
                if (E.IsReady())
                {
                    switch (Program.jungleClear["Jungle.UseE"].Cast<ComboBox>().CurrentValue)
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

                if (Q.IsReady())
                {
                    var jE = Program.jungleClear["Jungle.UseQ"].Cast<ComboBox>().CurrentValue;
                    if (jE != 0)
                    {
                        if (jE == 1)
                        {
                            jungleMobs = Utils.Utils.GetMobs(
                                Orbwalking.GetRealAutoAttackRange(null) + 65,
                                Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                Q.Cast();
                            }
                        }
                        else
                        {
                            var totalAa =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .Where(
                                        m =>
                                            m.Team == GameObjectTeam.Neutral &&
                                            m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 165))
                                    .Sum(mob => (int)mob.Health);

                            totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                            if (totalAa > jE)
                            {
                                Q.Cast();
                            }

                        }
                    }
                }
            }
        }

        public override void ExecuteLaneClear()
        {

            if (E.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, E.Range, MinionTypes.All,
                    MinionTeam.Enemy);

                if (minions != null)
                {
                    var eJ = Program.laneclear["UseE.Lane"].Cast<ComboBox>().CurrentValue;
                    if (eJ != 0)
                    {
                        var mE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + 175,
                            MinionTypes.All);
                        var locW = E.GetCircularFarmLocation(mE, 175);
                        if (locW.MinionsHit >= eJ && E.IsInRange(locW.Position.To3D()))
                        {
                            foreach (
                                var x in
                                    ObjectManager.Get<Obj_AI_Minion>()
                                        .Where(m => m.IsEnemy && !m.IsDead && m.LSDistance(locW.Position) < 100))
                            {
                                E.CastOnUnit(x);
                            }
                        }
                    }
                }
                if (Q.IsReady())
                {
                    var jE = Program.laneclear["UseQ.Lane"].Cast<ComboBox>().CurrentValue;
                    if (jE != 0)
                    {
                        var totalAa =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    m =>
                                        m.IsEnemy && !m.IsDead &&
                                        m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null)))
                                .Sum(mob => (int)mob.Health);

                        totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                        if (totalAa > jE)
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            return;
            /*
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            LeagueSharp.Common.Spell[] spellList = { W };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, System.Drawing.Color.Beige, 1);
            }

            var drawE = Program.marksmanDrawings["DrawE"].Cast<CheckBox>().CurrentValue;
            if (drawE)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Beige, 1);
            }
            */
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseWC", new CheckBox("Use W", false));
            config.Add("UseEC", new CheckBox("Use E"));
            config.Add("UseWKs", new CheckBox("Use W Kill Steal", false));
            config.Add("UseWCS", new CheckBox("Complete E stacks with W", false));

            config.AddGroupLabel("Don't Use E On");
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                {
                    config.Add("DontUseE" + enemy.ChampionName, new CheckBox(enemy.ChampionName, false));
                }
            }

            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddGroupLabel("Don't E Toggle to");
            {
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
                {
                    config.Add("DontEToggleHarass" + enemy.ChampionName, new CheckBox(enemy.ChampionName, false));
                }
            }

            config.Add("UseETH", new KeyBind("Use E (Toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));

            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawW", new CheckBox("W range"));//.SetValue(new Circle(true, System.Drawing.Color.Beige)));
            config.Add("DrawE", new CheckBox("E range"));//.SetValue(new Circle(true, System.Drawing.Color.Beige)));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.AddGroupLabel("Q Spell");
            config.Add("Misc.UseQ.Turret", new CheckBox("Use Q for Turret"));
            config.Add("Misc.UseQ.Inhibitor", new CheckBox("Use Q for Inhibitor"));
            config.Add("Misc.UseQ.Nexus", new CheckBox("Use Q for Nexus"));

            config.AddGroupLabel("W Spell");
            config.Add("ProtectWMana", new Slider("[Soon/WIP] Protect my mana for [W] if my Level < ", 8, 2, 18));
            config.Add("UseWM", new CheckBox("Use W KillSteal", false));

            config.AddGroupLabel("E Spell");
            config.Add("UseEM", new CheckBox("Use E for Enemy Turret"));

            config.AddGroupLabel("R Spell");
            config.Add("ProtectRMana", new Slider("[Soon/WIP] Protect my mana for [R] if my Level < ", 11, 6, 18));
            config.Add("UseRM", new CheckBox("Use R KillSteal"));
            config.Add("UseRMG", new CheckBox("Use R Gapclosers"));
            config.Add("UseRMI", new CheckBox("Use R Interrupt"));

            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            string[] strQ = new string[7];
            strQ[0] = "Off";

            for (var i = 1; i < 7; i++)
            {
                strQ[i] = "If need to AA more than >= " + i;
            }

            config.Add("UseQ.Lane", new ComboBox("Q:", 0, strQ));


            string[] strE = new string[5];
            strE[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strE[i] = "Minion Count >= " + i;
            }

            config.Add("UseE.Lane", new ComboBox("E:", 0, strE));
            ;
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            string[] strLaneMinCount = new string[8];
            strLaneMinCount[0] = "Off";
            strLaneMinCount[1] = "Just for big Monsters";

            for (var i = 2; i < 8; i++)
            {
                strLaneMinCount[i] = "If need to AA more than >= " + i;
            }

            config.Add("Jungle.UseQ", new ComboBox("Q:", 4, strLaneMinCount));
            config.Add("Jungle.UseE", new ComboBox("E:", 1, "Off", "On", "Just for big Monsters"));

            return true;
        }

        public class TristanaData
        {
            public static double GetWDamage
            {
                get
                {
                    if (W.IsReady())
                    {
                        var wDamage = new double[] { 80, 105, 130, 155, 180 }[W.Level - 1] + 0.5 * Player.FlatMagicDamageMod;
                        if (GetEMarkedCount > 0 && GetEMarkedCount < 4)
                        {
                            return wDamage + (wDamage * GetEMarkedCount * .20);
                        }
                        switch (GetEMarkedCount)
                        {
                            case 0:
                                return wDamage;
                            case 4:
                                return wDamage * 2;
                        }
                    }
                    return 0;
                }
            }

            public static float GetComboDamage(AIHeroClient t)
            {
                if (!t.LSIsValidTarget(W.Range))
                {
                    return 0;
                }

                var fComboDamage = 0d;
                /*
                    if (Q.IsReady())
                    {
                        var baseAttackSpeed = 0.656 + (0.656 / 100 * (Player.Level - 1) * 1.5);
                        var qExtraAttackSpeed = new double[] { 30, 50, 70, 90, 110 }[Q.Level - 1];
                        var attackDelay = (float) (baseAttackSpeed + (baseAttackSpeed / 100 * qExtraAttackSpeed));
                        attackDelay = (float) Math.Round(attackDelay, 2);

                        attackDelay *= 5;
                        attackDelay *= (float) Math.Floor(Player.TotalAttackDamage);
                        fComboDamage += attackDelay;
                    }
                    */
                if (W.IsReady())
                {
                    //fComboDamage += GetWDamage;
                    fComboDamage += W.GetDamage(t);
                }

                if (E.IsReady())
                {
                    fComboDamage += E.GetDamage(t);
                }

                if (R.IsReady())
                {
                    fComboDamage += R.GetDamage(t);
                    //new double[] {300, 400, 500}[R.Level - 1] + Player.FlatMagicDamageMod);
                }
                return (float)fComboDamage;
            }

            public static AIHeroClient GetEMarkedEnemy
                =>
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            enemy =>
                                !enemy.IsDead &&
                                enemy.LSIsValidTarget(W.Range + Orbwalking.GetRealAutoAttackRange(Player)))
                        .FirstOrDefault(enemy => enemy.Buffs.Any(buff => buff.DisplayName == "TristanaEChargeSound"));

            public static int GetEMarkedCount
                =>
                    GetEMarkedEnemy?.Buffs.Where(buff => buff.DisplayName == "TristanaECharge")
                        .Select(xBuff => xBuff.Count)
                        .FirstOrDefault() ?? 0;
        }
    }
}