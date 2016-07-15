#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Marksman.Champions
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using System.Threading;

    using Utils = LeagueSharp.Common.Utils;

    internal interface IKindred
    {
        void Orbwalking_AfterAttack(AfterAttackArgs args);
        void Drawing_OnDraw(EventArgs args);
        void Game_OnGameUpdate(EventArgs args);
        bool ComboMenu(Menu config);
        bool HarassMenu(Menu config);
        bool MiscMenu(Menu config);
        bool DrawingMenu(Menu config);
        bool LaneClearMenu(Menu config);
        //bool JungleClearMenu(Menu config);
    }

    internal class Kindred : Champion, IKindred
    {
        public static LeagueSharp.Common.Spell Q;
        public static LeagueSharp.Common.Spell E;
        public static LeagueSharp.Common.Spell W;
        public static LeagueSharp.Common.Spell R;
        public static AIHeroClient KindredECharge;
        public static List<DangerousSpells> DangerousList = new List<DangerousSpells>();

        public Kindred()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 375);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 900);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 740);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1100);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotCircle);

            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;

            DangerousList.Add(new DangerousSpells("darius", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("garen", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("leesin", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("nautilius", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("syndra", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("warwick", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("zed", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("chogath", SpellSlot.R));

            Marksman.Utils.Utils.PrintMessage("Kindred loaded.");
        }

        public override void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (args.Buff.Name.ToLower() == "kindredecharge" && !sender.IsMe)
            {
                KindredECharge = sender as AIHeroClient;
            }
        }

        public override void Obj_AI_Base_OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (args.Buff.Name.ToLower() == "kindredecharge" && !sender.IsMe)
            {
                KindredECharge = null;
            }
        }

        public override void OnCreateObject(GameObject sender, EventArgs args)
        {
        }

        public override void OnDeleteObject(GameObject sender, EventArgs args)
        {
        }

        public override void Orbwalking_AfterAttack(AfterAttackArgs args)
        {
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = Program.marksmanDrawings["DrawQ"].Cast<ComboBox>().CurrentValue;
            switch (drawQ)
            {
                case 1:
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Aqua);
                    break;
                case 2:
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65, Color.Aqua);
                    break;
            }
            LeagueSharp.Common.Spell[] spellList = { W, E, R };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 255, 255));
                }
            }
        }

        public void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!R.IsReady())
            {
                return;
            }

            if (sender.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            if (!sender.IsValid || sender.Team == ObjectManager.Player.Team)
            {
                return;
            }

            if (R.IsReady())
            {
                if (sender.IsEnemy && sender is AIHeroClient && args.Target.IsMe)
                {
                    foreach (
                        var c in
                            DangerousList.Where(c => ((AIHeroClient) sender).ChampionName.ToLower() == c.ChampionName)
                                .Where(c => args.Slot == c.SpellSlot))
                        //.Where(c => args.SData.Name == ((AIHeroClient)sender).GetSpell(c.SpellSlot).Name))
                    {
                        R.Cast(ObjectManager.Player.Position);
                    }
                }

            }

            if (R.IsReady())
            {
                var x = 0d;
                if (ObjectManager.Player.HealthPercent < 20 && ObjectManager.Player.LSCountEnemiesInRange(500) > 0)
                {
                    x = HeroManager.Enemies.Where(e => e.LSIsValidTarget(1000))
                        .Aggregate(0, (current, enemy) => (int)(current + enemy.Health));
                }
                if (ObjectManager.Player.Health < x)
                {
                    R.Cast(ObjectManager.Player.Position);
                }
                
                if (Program.combo["UseRC"].Cast<CheckBox>().CurrentValue &&
                    ObjectManager.Player.Health < ObjectManager.Player.MaxHealth * .2)
                {
                    if (!sender.IsMe && sender.IsEnemy && R.IsReady() && args.Target.IsMe) // for minions attack
                    {
                        R.Cast(ObjectManager.Player.Position);
                    }
                    else if (!sender.IsMe && sender.IsEnemy && (sender is AIHeroClient || sender is Obj_AI_Turret) &&
                             args.Target.IsMe && R.IsReady())
                    {
                        R.Cast(ObjectManager.Player.Position);
                    }
                }
            }
        }
        

        public override void Orbwalking_BeforeAttack(BeforeAttackArgs args)
        {
            foreach (
                var targetA in
                    HeroManager.Enemies.Where(
                        e =>
                            e.IsValid && e.LSDistance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(null) + 65 &&
                            e.IsVisible).Where(targetA => targetA.HasBuff("kindredcharge")))
            {
                PortAIO.OrbwalkerManager.ForcedTarget(targetA);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (R.IsReady())
            {
                var x = 0d;
                if (ObjectManager.Player.HealthPercent < 20 && ObjectManager.Player.LSCountEnemiesInRange(500) > 0)
                {
                    x = HeroManager.Enemies.Where(e => e.LSIsValidTarget(1000))
                        .Aggregate(0, (current, enemy) => (int)(current + enemy.Health));
                }
                if (ObjectManager.Player.Health < x)
                {
                    R.Cast(ObjectManager.Player.Position);
                }
            }

            AIHeroClient t = null;
            if (KindredECharge != null)
            {
                t = KindredECharge;
            }
            else
            {
                t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            }


            if (!t.LSIsValidTarget())
            {
                return;
            }

            if (ComboActive && !t.HasKindredUltiBuff())
            {
                if (t.LSIsValidTarget(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65) && !t.HasKindredUltiBuff())
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(Game.CursorPos);
                    }

                    if (E.IsReady() && t.LSIsValidTarget(E.Range))
                    {
                        E.CastOnUnit(t);
                    }

                    if (W.IsReady() && t.LSIsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                }
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseWC", new CheckBox("Use W"));
            config.Add("UseEC", new CheckBox("Use E"));
            config.Add("UseRC", new CheckBox("Use R"));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Q"));
            config.Add("UseWH", new CheckBox("W"));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            config.Add("UseQL", new CheckBox("Use Q"));
            config.Add("UseQLM", new Slider("Min. Minion:", 2, 1, 3));
            config.Add("UseWL", new CheckBox("Use W", false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new ComboBox("Q range", 2, "Off", "Q Range", "Q + AA Range"));//.SetValue(new StringList(new[] { "Off", "Q Range", "Q + AA Range" }, 2)));
            config.Add("DrawW", new CheckBox("W range"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("DrawE", new CheckBox("E range"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("DrawR", new CheckBox("R range"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            return false;
        }

        public override void ExecuteLaneClear()
        {
            var useQ = Program.laneclear["UseQL"].Cast<ComboBox>().CurrentValue;

            var minion =
                MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range)
                    .FirstOrDefault(m => m.Health < ObjectManager.Player.LSGetSpellDamage(m, SpellSlot.Q));

            if (minion != null)
            {
                switch (useQ)
                {
                    case 1:
                        minion =
                            MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range)
                                .FirstOrDefault(
                                    m =>
                                        m.Health < ObjectManager.Player.LSGetSpellDamage(m, SpellSlot.Q)
                                        && m.Health > ObjectManager.Player.TotalAttackDamage);
                        Q.Cast(minion);
                        break;

                    case 2:
                        minion =
                            MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range)
                                .FirstOrDefault(
                                    m =>
                                        m.Health < ObjectManager.Player.LSGetSpellDamage(m, SpellSlot.Q)
                                        && ObjectManager.Player.LSDistance(m)
                                        > Orbwalking.GetRealAutoAttackRange(null) + 65);
                        Q.Cast(minion);
                        break;
                }
            }
        }

        public override bool JungleClearMenu(Menu config)
        {
            config.Add("UseQJ", new ComboBox("Use Q", 1, "Off", "On", "Just big Monsters"));
            config.Add("UseWJ", new ComboBox("Use W", 1, "Off", "On", "Just big Monsters"));
            config.Add("UseEJ", new ComboBox("Use E", 1, "Off", "On", "Just big Monsters"));
            return true;
        }

        public override void ExecuteJungleClear()
        {
            var jungleMobs = Marksman.Utils.Utils.GetMobs(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,
                Marksman.Utils.Utils.MobTypes.All);

            if (jungleMobs != null)
            {
                switch (Program.jungleClear["UseQJ"].Cast<ComboBox>().CurrentValue)
                {
                    case 1:
                        {
                            if (jungleMobs.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                                Q.Cast(jungleMobs.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)
                                    ? Game.CursorPos
                                    : jungleMobs.Position);
                            break;
                        }
                    case 2:
                        {
                            jungleMobs = Marksman.Utils.Utils.GetMobs(
                                Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,
                                Marksman.Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                Q.Cast(jungleMobs.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)
                                    ? Game.CursorPos
                                    : jungleMobs.Position);
                            }
                            break;
                        }
                }

                switch (Program.jungleClear["UseWJ"].Cast<ComboBox>().CurrentValue)
                {
                    case 1:
                        {
                            if (jungleMobs.LSIsValidTarget(W.Range))
                                W.Cast(jungleMobs.Position);
                            break;
                        }
                    case 2:
                        {
                            jungleMobs = Marksman.Utils.Utils.GetMobs(E.Range, Marksman.Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                W.Cast(jungleMobs.Position);
                            }
                            break;
                        }
                }

                switch (Program.jungleClear["UseEJ"].Cast<ComboBox>().CurrentValue)
                {
                    case 1:
                        {
                            if (jungleMobs.LSIsValidTarget(E.Range))
                                E.CastOnUnit(jungleMobs);
                            break;
                        }
                    case 2:
                        {
                            jungleMobs = Marksman.Utils.Utils.GetMobs(E.Range, Marksman.Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                E.CastOnUnit(jungleMobs);
                            }
                            break;
                        }
                }

            }
        }

        public void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            throw new NotImplementedException();
        }
    }
}
