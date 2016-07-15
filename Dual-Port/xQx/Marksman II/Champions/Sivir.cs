#region
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Marksman.Champions
{
    using System.Collections.Generic;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Marksman.Utils;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK;
    internal class DangerousSpells
    {
        public string ChampionName { get; private set; }
        public SpellSlot SpellSlot { get; private set; }

        public DangerousSpells(string championName, SpellSlot spellSlot)
        {
            ChampionName = championName;
            SpellSlot = spellSlot;
        }
    }

    internal class Sivir : Champion
    {
        public static LeagueSharp.Common.Spell Q;
        public LeagueSharp.Common.Spell E;
        public LeagueSharp.Common.Spell W;
        public static List<DangerousSpells> DangerousList = new List<DangerousSpells>();

        public Sivir()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1220);
            Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 593);

            E = new LeagueSharp.Common.Spell(SpellSlot.E);

            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;

            DangerousList.Add(new DangerousSpells("darius", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("fiddlesticks", SpellSlot.Q));
            DangerousList.Add(new DangerousSpells("garen", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("leesin", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("nautilius", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("skarner", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("syndra", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("warwick", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("zed", SpellSlot.R));
            DangerousList.Add(new DangerousSpells("tristana", SpellSlot.R));

            Utils.PrintMessage("Sivir loaded.");
        }

        public override void Orbwalking_BeforeAttack(BeforeAttackArgs args)
        {
            if (!W.IsReady())
            {
                return;
            }

            if (Program.misc["Misc.UseW.Inhibitor"].Cast<CheckBox>().CurrentValue && args.Target is Obj_BarracksDampener)
            {
                W.Cast();
            }

            if (Program.misc["Misc.UseW.Nexus"].Cast<CheckBox>().CurrentValue && args.Target is Obj_HQ)
            {
                W.Cast();
            }

            if (Program.misc["Misc.UseW.Turret"].Cast<CheckBox>().CurrentValue && args.Target is Obj_AI_Turret)
            {
                W.Cast();
            }
        }

        public void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!E.IsReady() || !(sender is AIHeroClient) || !sender.IsMe)
            {
                return;
            }

            if (sender.IsEnemy && args.Target.IsMe && E.IsReady())
            {
                foreach (
                    var c in
                        DangerousList.Where(c => ((AIHeroClient) sender).ChampionName.ToLower() == c.ChampionName)
                            .Where(c => args.SData.Name == ((AIHeroClient) sender).GetSpell(c.SpellSlot).Name))
                {
                    E.Cast();
                }
            }
            return;
            /*
            if (((AIHeroClient)sender).ChampionName.ToLower() == "vayne" && args.SData.Name == ((AIHeroClient)sender).GetSpell(SpellSlot.E).Name)
            {
                for (var i = 1; i < 8; i++)
                {
                    var championBehind = ObjectManager.Player.Position + Vector3.Normalize(((AIHeroClient) sender).ServerPosition - ObjectManager.Player.Position)*(-i*50);
                    if (championBehind.LSIsWall())
                    {
                        E.Cast();
                    }
                }
            }
            */
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.misc["AutoQ"].Cast<CheckBox>().CurrentValue)
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (Q.IsReady() && t.LSIsValidTarget())
                {
                    if ((t.HasBuffOfType(BuffType.Slow) || t.HasBuffOfType(BuffType.Stun) ||
                         t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Fear) ||
                         t.HasBuffOfType(BuffType.Taunt)))
                    {
                        Q.CastIfHitchanceGreaterOrEqual(t);
                        //CastQ();
                    }
                }
            }

            if (ComboActive || HarassActive)
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;

                if (Q.IsReady() && useQ)
                {
                    var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                    if (t != null)
                    {
                        Q.CastIfHitchanceGreaterOrEqual(t);
                        //CastQ();
                    }
                }
            }
        }

        public override void ExecuteJungleClear()
        {
            var jungleMobs = Utils.GetMobs(Q.Range, Marksman.Utils.Utils.MobTypes.All);

            if (jungleMobs != null)
            {
                if (Q.IsReady())
                {
                    switch (Program.jungleClear["UseQ.Jungle"].Cast<ComboBox>().CurrentValue)
                    {
                        case 1:
                        {
                            Q.Cast(jungleMobs);
                            break;
                        }
                        case 2:
                        {
                            jungleMobs = Utils.GetMobs(Q.Range, Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                Q.Cast(jungleMobs);
                            }
                            break;
                        }
                    }
                }

                if (W.IsReady())
                {
                    var jW = Program.jungleClear["UseW.Jungle"].Cast<ComboBox>().CurrentValue;
                    if (jW != 0)
                    {
                        if (jW == 1)
                        {
                            jungleMobs = Utils.GetMobs(Orbwalking.GetRealAutoAttackRange(null) + 65,
                                Utils.MobTypes.BigBoys);
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
                                    .Sum(mob => (int) mob.Health);
                            totalAa = (int) (totalAa/ObjectManager.Player.TotalAttackDamage);
                            if (totalAa > jW)
                            {
                                W.Cast();
                            }
                        }
                    }
                }
            }
        }

        public override void ExecuteLaneClear()
        {
            var qJ = Program.laneclear["UseQ.Lane"].Cast<ComboBox>().CurrentValue;
            if (qJ != 0)
            {
                var minionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All);
                if (minionsQ != null)
                {
                    if (Q.IsReady())
                    {
                        var locQ = Q.GetLineFarmLocation(minionsQ);
                        if (minionsQ.Count == minionsQ.Count(m => ObjectManager.Player.LSDistance(m) < Q.Range) &&
                            locQ.MinionsHit > qJ && locQ.Position.IsValid())
                        {
                            Q.Cast(locQ.Position);
                        }
                    }
                }
            }
            var wJ = Program.laneclear["UseW.Lane"].Cast<ComboBox>().CurrentValue;
            if (wJ != 0)
            {
                var minionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                    Orbwalking.GetRealAutoAttackRange(null) + 165, MinionTypes.All);
                if (minionsW != null && minionsW.Count >= wJ)
                {
                    if (W.IsReady())
                    {
                        W.Cast();
                    }
                }
            }
        }

        public override void Orbwalking_AfterAttack(AfterAttackArgs args)
        {
            var target = args.Target;
            var t = target as AIHeroClient;
            if (t != null && (ComboActive || HarassActive))
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;

                var useW = Program.combo["UseWC"].Cast<CheckBox>().CurrentValue;

                if (W.IsReady() && useW)
                {
                    W.Cast();
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            LeagueSharp.Common.Spell[] spellList = {Q};
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 0, 255));
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseWC", new CheckBox("Use W"));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q", false));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("AutoQ", new CheckBox("Auto Q on Stun/Slow/Fear/Taunt/Snare"));
            config.Add("Misc.UseW.Turret", new CheckBox("Use W for Turret", false));
            config.Add("Misc.UseW.Inhibitor", new CheckBox("Use W for Inhibitor", false));
            config.Add("Misc.UseW.Nexus", new CheckBox("Use W for Nexus", false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            string[] strQ = new string[5];
            strQ[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strQ[i] = "Minion Count >= " + i;
            }

            config.Add("UseQ.Lane", new ComboBox(Utils.Tab + "Use Q:", 0, strQ));
            config.Add("UseQR.Lane", new CheckBox(Utils.Tab + "Use Q for out of AA Range"));


            string[] strW = new string[5];
            strW[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strW[i] = "Minion Count >= " + i;
            }

            config.Add("UseW.Lane", new ComboBox(Utils.Tab + "Use W:", 0, strW));

            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            config.Add("UseQ.Jungle", new ComboBox("Use Q", 1, "Off", "On", "Just for big Monsters"));

            string[] strW = new string[8];
            strW[0] = "Off";
            strW[1] = "Just for big Monsters";

            for (var i = 2; i < 8; i++)
            {
                strW[i] = "If need to AA more than >= " + i;
            }

            config.Add("UseW.Jungle", new ComboBox("Use W", 4, strW));

            return true;
        }
    }
}
