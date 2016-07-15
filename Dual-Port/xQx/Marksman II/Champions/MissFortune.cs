#region
using System;
using System.Drawing;
using System.Linq;
using System.Resources;
using EloBuddy;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Marksman.Champions
{
    internal class MissFortune : Champion
    {
        public static Spell Q, W, E;
        private static float UltiCastedTime = 0;
        public static AIHeroClient Player = ObjectManager.Player;

        public MissFortune()
        {
            Q = new Spell(SpellSlot.Q, 650);
            Q.SetTargetted(0.29f, 1400f);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E, 800);
            E.SetSkillshot(0.5f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            Utils.Utils.PrintMessage("MissFortune loaded.");
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "MissFortuneBulletTime")
                UltiCastedTime = Game.Time;
        }

        public override void Orbwalking_AfterAttack(AfterAttackArgs args)
        {
            var t = args.Target as AIHeroClient;
            if (t != null && (ComboActive || HarassActive))
            {
                //var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue; 
                var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;
            
                //if (useQ)
                    //Q.CastOnUnit(t);

                if (useW && W.IsReady())
                {
                    W.CastOnUnit(ObjectManager.Player);
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            Spell[] spellList = { Q, E };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem && spell.Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 0, 255));
                }
            }
        }

        private static void CastQ()
        {
            if (!Q.IsReady())
                return;

            var t = TargetSelector.GetTarget(Q.Range + 450, DamageType.Physical);
            if (t.LSIsValidTarget(Q.Range))
            {
                Q.CastOnUnit(t);
            }
        }
        

        public override void Game_OnGameUpdate(EventArgs args)
        {
            var ultCasting = Game.Time - UltiCastedTime < 0.2 || ObjectManager.Player.IsChannelingImportantSpell();

            PortAIO.OrbwalkerManager.SetAttack(!ultCasting);
            PortAIO.OrbwalkerManager.SetMovement(!ultCasting);
            if (ultCasting)
            {
                PortAIO.OrbwalkerManager.SetAttack(false);
                PortAIO.OrbwalkerManager.SetMovement(false);
            }

            if (Q.IsReady() && Program.harass["UseQTH"].Cast<KeyBind>().CurrentValue)
            {
                if (ObjectManager.Player.HasBuff("Recall"))
                    return;
                CastQ();
            }

            if (E.IsReady() && Program.harass["UseETH"].Cast<KeyBind>().CurrentValue)
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t.LSIsValidTarget() && (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                                          t.HasBuffOfType(BuffType.Charm) || t.HasBuffOfType(BuffType.Fear) ||
                                          t.HasBuffOfType(BuffType.Taunt) || t.HasBuff("zhonyasringshield") ||
                                          t.HasBuff("Recall")))
                {
                    E.CastIfHitchanceEquals(t, HitChance.Low);
                }
            }

            if (ComboActive || HarassActive)
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
                var useE = ComboActive ? Program.combo["UseEC"].Cast<CheckBox>().CurrentValue : Program.harass["UseEH"].Cast<CheckBox>().CurrentValue;

                if (Q.IsReady() && useQ)
                {
                    CastQ();
                }

                if (E.IsReady() && useE)
                {
                    var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                    if (t.LSIsValidTarget())
                    {
                        if (ObjectManager.Player.LSDistance(t) > 600)
                            E.CastIfHitchanceEquals(t, t.Path.Count() > 1 ? HitChance.High : HitChance.Medium);
                        else
                            E.CastIfHitchanceEquals(t, HitChance.Low);
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
                                minions =>
                                    minions.Health < ObjectManager.Player.LSGetSpellDamage(minions, SpellSlot.Q) - 20))
                        Q.Cast(minions);
                }
            }
        }

        public override void ExecuteJungleClear()
        {
            var jungleMobs = Marksman.Utils.Utils.GetMobs(Q.Range, Marksman.Utils.Utils.MobTypes.All);

            if (jungleMobs != null)
            {
                if (Q.IsReady())
                {
                    switch (Program.jungleClear["Jungle.UseQ"].Cast<ComboBox>().CurrentValue)
                    {
                        case 1:
                            {
                                Q.CastOnUnit(jungleMobs);
                                break;
                            }
                        case 2:
                            {
                                jungleMobs = Utils.Utils.GetMobs(Q.Range, Utils.Utils.MobTypes.BigBoys);
                                if (jungleMobs != null)
                                {
                                    Q.CastOnUnit(jungleMobs);
                                }
                                break;
                            }
                    }
                }

                if (W.IsReady())
                {
                    var jW = Program.jungleClear["Jungle.UseW"].Cast<ComboBox>().CurrentValue;
                    if (jW != 0)
                    {
                        if (jW == 1)
                        {
                            jungleMobs = Utils.Utils.GetMobs(Orbwalking.GetRealAutoAttackRange(null) + 65,
                                Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                W.Cast();
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
                            if (totalAa >= jW)
                            {
                                W.Cast();
                            }

                        }
                    }
                }

                if (E.IsReady())
                {
                    var jE = Program.jungleClear["Jungle.UseE"].Cast<ComboBox>().CurrentValue;
                    if (jE != 0)
                    {
                        var aMobs = MinionManager.GetMinions(ObjectManager.Player.Position, E.Range, MinionTypes.All,
                            MinionTeam.Neutral);
                        if (aMobs.Count > jE)
                        {
                            E.Cast(aMobs[0]);
                        }
                    }
                }
            }
        }

        public override void Orbwalking_BeforeAttack(BeforeAttackArgs args)
        {
            if (!W.IsReady())
            {
                return;
            }

            if (Program.misc["Misc.UseW.Turret"].Cast<CheckBox>().CurrentValue && args.Target is Obj_AI_Turret)
            {
                if (((Obj_AI_Turret)args.Target).Health >= Player.TotalAttackDamage * 3)
                {
                    W.Cast();
                }
            }

            if (Program.misc["Misc.UseW.Inhibitor"].Cast<CheckBox>().CurrentValue && args.Target is Obj_BarracksDampener)
            {
                if (((Obj_BarracksDampener)args.Target).Health >= Player.TotalAttackDamage * 3)
                {
                    W.Cast();
                }
            }

            if (Program.misc["Misc.UseW.Nexus"].Cast<CheckBox>().CurrentValue && args.Target is Obj_HQ)
            {
                W.Cast();
            }
        }

        public override void ExecuteLaneClear()
        {
            if (Q.IsReady())
            {
                var lQ = Program.laneclear["Lane.UseQ"].Cast<ComboBox>().CurrentValue;
                if (lQ != 0)
                {
                    {
                        var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                        foreach (var minions in
                            vMinions.Where(
                                minions => minions.Health < ObjectManager.Player.LSGetSpellDamage(minions, SpellSlot.Q)))
                        {
                            Q.CastOnUnit(minions);
                        }
                    }
                }
            }

            if (W.IsReady())
            {
                var lW = Program.laneclear["Lane.UseW"].Cast<ComboBox>().CurrentValue;
                if (lW != 0)
                {
                    var totalAa =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                m =>
                                    m.IsEnemy && !m.IsDead &&
                                    m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null)))
                            .Sum(mob => (int)mob.Health);

                    totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                    if (totalAa > lW)
                    {
                        W.Cast();
                    }
                }
            }

            if (E.IsReady())
            {
                var lE = Program.laneclear["Lane.UseE"].Cast<ComboBox>().CurrentValue;
                if (lE != 0)
                {
                    var mE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All);
                    if (mE != null)
                    {
                        var locE = E.GetCircularFarmLocation(mE, 175);
                        if (locE.MinionsHit >= lE && E.IsInRange(locE.Position.To3D()))
                        {
                            foreach (
                                var x in
                                    ObjectManager.Get<Obj_AI_Minion>()
                                        .Where(m => m.IsEnemy && !m.IsDead && m.LSDistance(locE.Position) < 500))
                            {
                                E.Cast(x);
                            }
                        }
                    }
                }
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseWC", new CheckBox("Use W"));
            config.Add("UseEC", new CheckBox("Use E"));

            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Q"));
            config.Add("UseWH", new CheckBox("W"));
            config.Add("UseEH", new CheckBox("E"));

            config.Add("UseQTH", new KeyBind("Use Q (Toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));
            config.Add("UseETH", new KeyBind("Use E (Toggle)", false, KeyBind.BindTypes.PressToggle, 'T'));

            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, System.Drawing.Color.DarkRed))); 
            config.Add("DrawE", new CheckBox("E range"));//.SetValue(new Circle(true, System.Drawing.Color.DarkRed))); 
            config.Add("DrawR", new CheckBox("R range"));//.SetValue(new Circle(true, System.Drawing.Color.DarkRed))); 

            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            config.Add("Lane.UseQ", new ComboBox(Utils.Utils.Tab + "Use Q:", 0, "Off", "On"));

            string[] strW = new string[7];
            {
                strW[0] = "Off";

                for (var i = 1; i < 7; i++)
                {
                    strW[i] = "If need to AA more than >= " + i;
                }
                config.Add("Lane.UseW", new ComboBox(Utils.Utils.Tab + "Use W:", 0, strW));
            }

            string[] strE = new string[5];
            {
                strE[0] = "Off";

                for (var i = 1; i < 5; i++)
                {
                    strE[i] = "Minion Count >= " + i;
                }
                config.Add("Lane.UseE", new ComboBox(Utils.Utils.Tab + "Use E:", 0, strE));
            }
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            config.Add("Jungle.UseQ", new ComboBox("Use Q", 2, "Off", "On", "Just big Monsters"));

            string[] strW = new string[8];
            strW[0] = "Off";
            strW[1] = "Just for big Monsters";

            for (var i = 2; i < 8; i++)
            {
                strW[i] = "If need to AA more than >= " + i;
            }

            config.Add("Jungle.UseW", new ComboBox("Use W", 4, strW));

            string[] strE = new string[4];
            strE[0] = "Off";

            for (var i = 1; i < 4; i++)
            {
                strE[i] = "Mob Count >= " + i;
            }

            config.Add("Jungle.UseE", new ComboBox("Use E:", 3, strE));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("Misc.UseW.Turret", new CheckBox("Use W for Turret"));
            config.Add("Misc.UseW.Inhibitor", new CheckBox("Use W for Inhibitor"));
            config.Add("Misc.UseW.Nexus", new CheckBox("Use W for Nexus"));
            return true;
        }
    }
}
