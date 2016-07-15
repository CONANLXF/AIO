#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;
using SharpDX;
using Color = System.Drawing.Color;
using SharpDX.Direct3D9;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

#endregion

 namespace Marksman.Champions
{

    internal class Twitch : Champion
    {
        internal class EnemyMarker
        {
            public string ChampionName { get; set; }
            public double ExpireTime { get; set; }
            public int BuffCount { get; set; }
        }
        public static LeagueSharp.Common.Spell W;
        public static LeagueSharp.Common.Spell E;
        public Twitch()
        {
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 950);
            W.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1200);

            //Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
            //Utility.HpBarDamageIndicator.Enabled = true;
            Utils.Utils.PrintMessage("Twitch loaded.");
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t == null || (!ComboActive && !HarassActive))
                return;

            var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;

            if (useW && W.IsReady())
                W.Cast(t, false, true);
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            //Spell[] spellList = {W};
            //foreach (var spell in spellList)
            //{
            //    var menuItem = GetValue<Circle>("Draw" + spell.Slot);
            //    if (menuItem.Active)
            //        Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            //}
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.CanMove && (ComboActive || HarassActive))
            {
                var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;
                var useE = ComboActive ? Program.combo["UseEC"].Cast<CheckBox>().CurrentValue : Program.harass["UseEH"].Cast<CheckBox>().CurrentValue;

                var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (!t.LSIsValidTarget() || t.HasKindredUltiBuff())
                {
                    return;
                }

                if (useW)
                {
                    if (W.IsReady() && t.LSIsValidTarget(W.Range))
                        W.Cast(t, false, true);
                }

                if (useE && E.IsReady() && t.GetBuffCount("TwitchDeadlyVenom") == 6)
                {
                    E.Cast();
                }

                if (ObjectManager.Get<AIHeroClient>().Find(e1 => e1.LSIsValidTarget(E.Range) && E.IsKillable(e1)) != null)
                {
                    E.Cast();
                }
            }

            if (Program.misc["UseEM"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                foreach (
                    var hero in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                hero =>
                                    hero.LSIsValidTarget(E.Range) &&
                                    (ObjectManager.Player.LSGetSpellDamage(hero, SpellSlot.E) - 10 > hero.Health)))
                {
                    E.Cast();
                }
            }
        }

        public override void ExecuteLaneClear()
        {
            //var prepareMinions = Program.Config.Item("PrepareMinionsE.Lane").GetValue<StringList>().SelectedIndex;
            //if (prepareMinions != 0)
            //{
            //    List<Obj_AI_Minion> list = new List<Obj_AI_Minion>();

            //    IEnumerable<Obj_AI_Minion> minions =
            //        from m in
            //            ObjectManager.Get<Obj_AI_Minion>()
            //                .Where(
            //                    m =>
            //                        m.Health > ObjectManager.Player.TotalAttackDamage &&
            //                        m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            //        select m;

            //    var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            //    foreach (var m in objAiMinions)
            //    {
            //        if (m.GetBuffCount(twitchEBuffName) > 0)
            //        {
            //            list.Add(m);
            //        }
            //        else
            //        {
            //            list.Remove(m);
            //        }
            //    }

            //    foreach (var l in objAiMinions.Except(list).ToList())
            //    {
            //        Program.ChampionClass.Orbwalker.ForcedTarget =(l);
            //    }
            //}
        }

        public override void ExecuteJungleClear()
        {
            var jungleWValue = Program.jungleClear["UseW.Jungle"].Cast<ComboBox>().CurrentValue;
            if (W.IsReady() && jungleWValue != 0)
            {
                var jungleMobs = Utils.Utils.GetMobs(W.Range, 
                    jungleWValue != 3 ? Utils.Utils.MobTypes.All : Utils.Utils.MobTypes.BigBoys,
                    jungleWValue != 3 ? jungleWValue : 1);

                if (jungleMobs != null)
                {
                    W.Cast(jungleMobs);
                }
            }

            if (E.IsReady() && Program.jungleClear["UseE.Jungle"].Cast<ComboBox>().CurrentValue != 0)
            {
                var jungleMobs = Utils.Utils.GetMobs(E.Range, Program.jungleClear["UseE.Jungle"].Cast<ComboBox>().CurrentValue == 1
                        ? Utils.Utils.MobTypes.All
                        : Utils.Utils.MobTypes.BigBoys);

                if (jungleMobs != null && E.CanCast(jungleMobs) && jungleMobs.Health <= E.GetDamage(jungleMobs) + 20)
                {
                    E.Cast();
                }
            }
        }


        private static float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            if (E.IsReady())
                fComboDamage += (float) ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.E);

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot")) ==
                SpellState.Ready && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3144) && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(3153) && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Botrk);

            return fComboDamage;
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseWC", new CheckBox("Use W"));
            config.Add("UseEC", new CheckBox("Use E max Stacks"));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseWH", new CheckBox("Use W", false));
            config.Add("UseEH", new CheckBox("Use E at max Stacks", false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawW", new CheckBox("W range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));

            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("UseEM", new CheckBox("Use E KS"));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            config.Add("PrepareMinionsE.Lane", new ComboBox("Prepare Minions for E", 2, "Off", "Everytime", "Just Under Ally Turret"));

            string[] strW = new string[6];
            strW[0] = "Off";

            for (var i = 1; i < 6; i++)
            {
                strW[i] = "If Could Infect Minion Count>= " + i;
            }

            config.Add("UseW.Lane", new ComboBox("Use W:", 0, strW));


            string[] strE = new string[6];
            strE[0] = "Off";

            for (var i = 1; i < 6; i++)
            {
                strE[i] = "Minion Count >= " + i;
            }

            config.Add("UseE.Lane", new ComboBox("Use E:", strE, 0));
            return true;
        }
        public override bool JungleClearMenu(Menu config)
        {

            string[] strW = new string[4];
            strW[0] = "Off";
            strW[3] = "Just big Monsters";

            for (var i = 1; i < 3; i++)
            {
                strW[i] = "If Could Infect Mobs Count>= " + i;
            }
            
            config.Add("UseW.Jungle", new ComboBox("Use W:", 3, strW));

            //config.Add("UseW.Jungle", "Use W").SetValue(new StringList(new[] { "Off", "On", "Just big Monsters" }, 2)));
            config.Add("UseE.Jungle", new ComboBox("Use E", 2, "Off", "On", "Just big Monsters"));

            return true;
        }
    }
}
