#region
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;
using SharpDX;
using Color = System.Drawing.Color;
using SharpDX.Direct3D9;
using Collision = LeagueSharp.Common.Collision;

#endregion

 namespace Marksman.Champions
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using System.Runtime.Remoting.Messaging;

    internal class EnemyMarker
    {
        public string ChampionName { get; set; }

        public double ExpireTime { get; set; }

        public int BuffCount { get; set; }
    }

    internal class AttackMinions
    {
        public Obj_AI_Minion Minion;

        public AttackMinions(Obj_AI_Minion minion)
        {
            Minion = minion;
        }
    }

    internal class Kalista : Champion
    {
        public static LeagueSharp.Common.Spell Q, W, E, R;

        public static Font font;

        public static AIHeroClient SoulBound { get; private set; }

        private static string kalistaEBuffName = "kalistaexpungemarker";

        private static List<Obj_AI_Minion> attackMinions = new List<Obj_AI_Minion>();

        private static List<EnemyMarker> xEnemyMarker = new List<EnemyMarker>();

        private static Dictionary<String, int> MarkedChampions = new Dictionary<String, int>();

        private static Dictionary<Vector3, Vector3> JumpPos = new Dictionary<Vector3, Vector3>();

        private static Dictionary<float, float> incomingDamage = new Dictionary<float, float>();

        private static Dictionary<float, float> InstantDamage = new Dictionary<float, float>();

        public static float IncomingDamage
        {
            get { return incomingDamage.Sum(e => e.Value) + InstantDamage.Sum(e => e.Value); }
        }

        public Kalista()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 5000);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1000);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1100);

            Q.SetSkillshot(0.25f, 40f, 2100f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoe UI",
                    Height = 45,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });


            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;

            Utils.Utils.PrintMessage("Kalista loaded.");
        }

        private void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            font.Dispose();
        }

        private void DrawingOnOnPostReset(EventArgs args)
        {
            font.OnResetDevice();
        }

        private void DrawingOnOnPreReset(EventArgs args)
        {
            font.OnLostDevice();
        }


        public static int KalistaMarkerCount
        {
            get
            {
                return (from enemy in ObjectManager.Get<AIHeroClient>().Where(tx => tx.IsEnemy && !tx.IsDead)
                        where ObjectManager.Player.LSDistance(enemy) < E.Range
                        from buff in enemy.Buffs
                        where buff.Name.Contains("kalistaexpungemarker")
                        select buff).Select(buff => buff.Count).FirstOrDefault();
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            /*
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Program.combo["Combo.UseQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var enemy = target as AIHeroClient;
                if (enemy != null)
                {
                    if (ObjectManager.Player.TotalAttackDamage < enemy.Health + enemy.AllShield)
                    {
                        Q.Cast(enemy);
                    }
                }
            }
            */
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            var killableMinionCount = 0;
            foreach (var m in
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range)
                    .Where(x => E.CanCast(x) && x.Health < EDamage(x)))
            {
                if (m.BaseSkinName.ToLower() == "sru_chaosminionsiege" || m.BaseSkinName.ToLower() == "sru_chaosminionsuper")
                    killableMinionCount += 2;
                else killableMinionCount++;

                Render.Circle.DrawCircle(m.Position, (float)(m.BoundingRadius * 1.5), Color.White, 5);
            }

            foreach (var m in
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    E.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral).Where(m => E.CanCast(m) && m.Health < EDamage(m)))
            {
                if (m.BaseSkinName.ToLower().Contains("baron") || m.BaseSkinName.ToLower().Contains("dragon") && E.CanCast(m))
                    E.Cast(m);
                else Render.Circle.DrawCircle(m.Position, (float)(m.BoundingRadius * 1.5), Color.White, 5);
            }

            LeagueSharp.Common.Spell[] spellList = { Q, E, R };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem && spell.Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 255, 255));
                }
            }

            var drawEStackCount = Program.marksmanDrawings["DrawEStackCount"].Cast<CheckBox>().CurrentValue;
            if (drawEStackCount)
            {
                xEnemyMarker.Clear();
                foreach (var xEnemy in
                    HeroManager.Enemies.Where(
                        tx => tx.IsEnemy && !tx.IsDead && ObjectManager.Player.LSDistance(tx) < E.Range))
                {
                    foreach (var buff in xEnemy.Buffs.Where(buff => buff.Name.Contains("kalistaexpungemarker")))
                    {
                        xEnemyMarker.Add(
                            new EnemyMarker
                            {
                                ChampionName = xEnemy.ChampionName,
                                ExpireTime = Game.Time + 4,
                                BuffCount = buff.Count
                            });
                    }
                }

                foreach (var markedEnemies in xEnemyMarker)
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                    {
                        if (enemy.IsEnemy && !enemy.IsDead && ObjectManager.Player.LSDistance(enemy) <= E.Range
                            && enemy.ChampionName == markedEnemies.ChampionName)
                        {
                            if (!(markedEnemies.ExpireTime > Game.Time))
                            {
                                continue;
                            }
                            var xCoolDown = TimeSpan.FromSeconds(markedEnemies.ExpireTime - Game.Time);
                            var display = string.Format("{0}", markedEnemies.BuffCount);
                            Utils.Utils.DrawText(
                                font,
                                display,
                                (int)enemy.HPBarPosition.X - 10,
                                (int)enemy.HPBarPosition.Y,
                                SharpDX.Color.Wheat);
                        }
                    }
                }
            }
            var drawJumpPos = Program.marksmanDrawings["DrawJumpPos"].Cast<CheckBox>().CurrentValue;
            if (drawJumpPos)
            {
                foreach (var pos in JumpPos)
                {
                    if (ObjectManager.Player.LSDistance(pos.Key) <= 500f
                        || ObjectManager.Player.LSDistance(pos.Value) <= 500f)
                    {
                        Drawing.DrawCircle(pos.Key, 75f, Color.HotPink);
                        Drawing.DrawCircle(pos.Value, 75f, Color.HotPink);
                    }
                    if (ObjectManager.Player.LSDistance(pos.Key) <= 35f || ObjectManager.Player.LSDistance(pos.Value) <= 35f)
                    {
                        Render.Circle.DrawCircle(pos.Key, 70f, Color.GreenYellow);
                        Render.Circle.DrawCircle(pos.Value, 70f, Color.GreenYellow);
                    }
                }
            }
        }

        public void JumpTo()
        {
            if (!Q.IsReady())
            {
                Drawing.DrawText(
                    Drawing.Width * 0.44f,
                    Drawing.Height * 0.80f,
                    Color.Red,
                    "Q is not ready! You can not Jump!");
                return;
            }

            Drawing.DrawText(
                Drawing.Width * 0.39f,
                Drawing.Height * 0.80f,
                Color.White,
                "Jumping Mode is Active! Go to the nearest jump point!");

            foreach (var xTo in from pos in JumpPos
                                where
                                    ObjectManager.Player.LSDistance(pos.Key) <= 35f
                                    || ObjectManager.Player.LSDistance(pos.Value) <= 35f
                                let xTo = pos.Value
                                select
                                    ObjectManager.Player.LSDistance(pos.Key) < ObjectManager.Player.LSDistance(pos.Value)
                                        ? pos.Value
                                        : pos.Key)
            {
                Q.Cast(new Vector2(xTo.X, xTo.Y), true);
                Player.IssueOrder(GameObjectOrder.MoveTo, xTo);
            }
        }

        private static float EDamage(Obj_AI_Base target)
        {
            if ((target.IsMinion || target.IsMonster) && !(target is AIHeroClient))
            {
                int stacksMin = GetMinionStacks(target);

                var EDamageMinion = new float[] { 20, 30, 40, 50, 60 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] + (0.6 * ObjectManager.Player.FlatPhysicalDamageMod);

                if (stacksMin > 1)
                {
                    EDamageMinion += ((new float[] { 10, 14, 19, 25, 32 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] + (new float[] { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] * ObjectManager.Player.FlatPhysicalDamageMod)) * (stacksMin - 1));
                }

                return (float)ObjectManager.Player.CalcDamage(target, DamageType.Physical, EDamageMinion) * 0.9f;
            }
            if (target is AIHeroClient)
            {
                if (GetStacks(target) == 0) return 0;

                int stacksChamps = GetStacks(target);

                var EDamageChamp = new[] { 20, 30, 40, 50, 60 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] + (0.6 * ObjectManager.Player.FlatPhysicalDamageMod);

                if (stacksChamps > 1)
                {
                    EDamageChamp += ((new[] { 10, 14, 19, 25, 32 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] + (new[] { 0.2, 0.225, 0.25, 0.275, 0.3 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] * ObjectManager.Player.FlatPhysicalDamageMod)) * (stacksChamps - 1));
                }

                return (float)ObjectManager.Player.CalcDamage(target, DamageType.Physical, EDamageChamp);
            }
            return 0;
        }

        private static int GetMinionStacks(Obj_AI_Base minion)
        {
            int stacks = 0;
            foreach (var rendbuff in minion.Buffs.Where(x => x.Name.ToLower().Contains("kalistaexpungemarker")))
            {
                stacks = rendbuff.Count;
            }

            if (stacks == 0 || !minion.HasBuff("kalistaexpungemarker")) return 0;
            return stacks;
        }

        private static int GetStacks(Obj_AI_Base target)
        {
            int stacks = 0;

            if (target.HasBuff("kalistaexpungemarker"))
            {
                foreach (var rendbuff in target.Buffs.Where(x => x.Name.ToLower().Contains("kalistaexpungemarker")))
                {
                    stacks = rendbuff.Count;
                }
            }
            else
            {
                return 0;
            }
            return stacks;
        }

        private static float GetEDamage(Obj_AI_Base t)
        {
            return E.IsReady() && E.CanCast(t) ? EDamage(t) : 0;
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            SoulBoundSaver();

            foreach (var e in HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range)))
            {
                foreach (var b in e.Buffs.Where(buff => buff.Name.Contains("kalistaexpungemarker")))
                {
                    if (E.IsReady() && e.Health < EDamage(e))
                    {
                        E.Cast();
                    }
                }

            }

            foreach (var myBoddy in
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(obj => obj.Name == "RobotBuddy" && obj.IsAlly && ObjectManager.Player.LSDistance(obj) < 1500))
            {
                Render.Circle.DrawCircle(myBoddy.Position, 75f, Color.Red);
            }


            AIHeroClient t;

            if (Q.IsReady() && Program.harass["UseQTH"].Cast<KeyBind>().CurrentValue)
            {
                if (ObjectManager.Player.HasBuff("Recall"))
                {
                    return;
                }

                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (t.LSIsValidTarget(Q.Range) && ObjectManager.Player.Mana > E.ManaCost + Q.ManaCost)
                {
                    Q.Cast(t);
                }
            }

            if (ComboActive || HarassActive)
            {
                if (Orbwalker.CanMove)
                {
                    if (Q.IsReady())
                    {
                        t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                        if (!t.HasKindredUltiBuff() && t.LSIsValidTarget(Q.Range)
                            && ObjectManager.Player.Mana > E.ManaCost + Q.ManaCost)
                        {
                            Q.Cast(t);
                        }
                    }
                }
            }
        }

        public override void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!Program.misc["Misc.BlockE"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (args.Slot == SpellSlot.E)
            {
                var minion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range).Find(m => m.Health < EDamage(m) + 10 && E.CanCast(m) && E.Cooldown < 0.0001);
                var enemy = HeroManager.Enemies.Find(e => e.Buffs.Any(b => b.Name.ToLower() == kalistaEBuffName && e.LSIsValidTarget(E.Range) && e.Health < EDamage(e)));
                if (enemy == null && minion == null)
                {
                    //args.Process = false;
                }
            }
        }

        public override void Obj_AI_Base_OnProcessSpellCast(
            Obj_AI_Base sender,
            GameObjectProcessSpellCastEventArgs args)
        {
            //attackMinions.Clear();
            if (sender == null) return;

            if (sender.IsEnemy)
            {
                if (SoulBound != null && Program.combo["SoulBoundSaver"].Cast<CheckBox>().CurrentValue)
                {

                    if ((!(sender is AIHeroClient) || args.SData.IsAutoAttack()) && args.Target != null
                        && args.Target.NetworkId == SoulBound.NetworkId)
                    {

                        incomingDamage.Add(
                            SoulBound.ServerPosition.LSDistance(sender.ServerPosition) / args.SData.MissileSpeed
                            + Game.Time,
                            (float)sender.LSGetAutoAttackDamage(SoulBound));
                    }


                    else if (sender is AIHeroClient)
                    {
                        var attacker = (AIHeroClient)sender;
                        var slot = attacker.GetSpellSlot(args.SData.Name);

                        if (slot != SpellSlot.Unknown)
                        {
                            if (slot == attacker.GetSpellSlot("SummonerDot") && args.Target != null
                                && args.Target.NetworkId == SoulBound.NetworkId)
                            {

                                InstantDamage.Add(
                                    Game.Time + 2,
                                    (float)attacker.GetSummonerSpellDamage(SoulBound, LeagueSharp.Common.Damage.SummonerSpell.Ignite));
                            }
                            else if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R)
                                     && ((args.Target != null && args.Target.NetworkId == SoulBound.NetworkId)
                                         || args.End.LSDistance(SoulBound.ServerPosition, true)
                                         < Math.Pow(args.SData.LineWidth, 2)))
                            {

                                InstantDamage.Add(Game.Time + 2, (float)attacker.LSGetSpellDamage(SoulBound, slot));
                            }
                        }
                    }
                }
            }

            if (sender.IsMe && args.SData.Name == E.Instance.Name)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(250, Orbwalker.ResetAutoAttack);
            }
        }

        private static void SoulBoundSaver()
        {
            if (SoulBound == null)
            {
                SoulBound =
                    HeroManager.Allies.Find(
                        h => !h.IsMe && h.Buffs.Any(b => b.Caster.IsMe && b.Name == "kalistacoopstrikeally"));
            }
            else if (Program.combo["SoulBoundSaver"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                if (SoulBound.HealthPercent < 5 && SoulBound.LSCountEnemiesInRange(500) > 0
                    || IncomingDamage > SoulBound.Health) R.Cast();
            }

            var itemsToRemove = incomingDamage.Where(entry => entry.Key < Game.Time).ToArray();
            foreach (var item in itemsToRemove) incomingDamage.Remove(item.Key);

            itemsToRemove = InstantDamage.Where(entry => entry.Key < Game.Time).ToArray();
            foreach (var item in itemsToRemove) InstantDamage.Remove(item.Key);
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("Combo.UseQ", new CheckBox("Use Q"));
            config.Add("SoulBoundSaver", new CheckBox("Auto SoulBound Saver"));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            config.Add("UseQTH", new KeyBind("Use Q (Toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("Misc.BlockE", new CheckBox("Block E if can not kill anything", false));
            config.Add("Misc.UseSlowE", new CheckBox("Use E for slow if it possible"));
            config.Add("Misc.EReduction", new Slider("E Damage Reduction : ", 100, 0, 300));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawE", new CheckBox("E range", false));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("DrawR", new CheckBox("R range", false));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("DrawEStackCount", new CheckBox("E Stack Count"));//.SetValue(new Circle(true, Color.White)));
            config.Add("DrawJumpPos", new CheckBox("Jump Positions", false));//.SetValue(new Circle(false, Color.HotPink)));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {

            string[] srtQ = new string[6];
            srtQ[0] = "Off";

            for (var i = 1; i < 6; i++)
            {
                srtQ[i] = "Minion Count >= " + i;
            }

            config.Add("UseQ.Lane", new ComboBox("Use Q:", 0, srtQ));
            config.Add("UseQ.Mode.Lane", new ComboBox("Use Q Mode:", 1, "Everytime", "Just Out of AA Range"));

            string[] strW = new string[6];
            strW[0] = "Off";

            for (var i = 1; i < 6; i++)
            {
                strW[i] = "Minion Count >= " + i;
            }

            config.Add("UseE.Lane", new ComboBox("Use E:", 0, strW));
            config.Add("UseE.LaneNon", new CheckBox("Use E for Non Killable Minions:"));
            config.Add("UseE.Prepare.Lane", new ComboBox("Prepare Minions for E Farm", 2, "Off", "On", "Just Under Ally Turret"));
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            config.Add("UseQJ", new ComboBox("Use Q", 1, "Off", "On", "Just big Monsters"));
            config.Add("UseEJ", new ComboBox("Use E", 1, "Off", "On", "Just big Monsters"));
            return true;
        }

        private static List<Obj_AI_Base> qGetCollisionMinions(AIHeroClient source, Vector3 targetposition)
        {
            var input = new PredictionInput { Unit = source, Radius = Q.Width, Delay = Q.Delay, Speed = Q.Speed, };

            input.CollisionObjects[0] = CollisionableObjects.Minions;

            return
                Collision.GetCollision(new List<Vector3> { targetposition }, input)
                    .OrderBy(obj => obj.LSDistance(source, false))
                    .ToList();
        }

        public override void ExecuteJungleClear()
        {
            if (Q.IsReady())
            {
                var jungleMobs = Utils.Utils.GetMobs(
                    Q.Range,
                    Program.jungleClear["UseQJ"].Cast<ComboBox>().CurrentValue == 1
                        ? Utils.Utils.MobTypes.All
                        : Utils.Utils.MobTypes.BigBoys);

                if (jungleMobs != null && ObjectManager.Player.Mana > E.ManaCost + Q.ManaCost) Q.Cast(jungleMobs);
            }

            if (E.IsReady())
            {
                var jungleMobs = Utils.Utils.GetMobs(
                    E.Range,
                    Program.jungleClear["UseEJ"].Cast<ComboBox>().CurrentValue == 1
                        ? Utils.Utils.MobTypes.All
                        : Utils.Utils.MobTypes.BigBoys);

                if (jungleMobs != null && E.CanCast(jungleMobs) && jungleMobs.Health < EDamage(jungleMobs))
                    E.CastOnUnit(jungleMobs);
            }
        }

        public override void ExecuteLaneClear()
        {
            if (Q.IsReady())
            {
                var qCount = Program.laneclear["UseQ.Lane"].Cast<ComboBox>().CurrentValue;
                if (qCount != 0)
                {
                    var minions = MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition,
                        Q.Range,
                        MinionTypes.All,
                        MinionTeam.Enemy);

                    foreach (var minion in minions.Where(x => x.Health <= Q.GetDamage(x)))
                    {
                        var killableMinionCount = 0;
                        foreach (
                            var colminion in
                                qGetCollisionMinions(
                                    ObjectManager.Player,
                                    ObjectManager.Player.ServerPosition.LSExtend(minion.ServerPosition, Q.Range)))
                        {
                            if (colminion.Health <= Q.GetDamage(colminion))
                            {
                                if (Program.laneclear["UseQ.Mode.Lane"].Cast<ComboBox>().CurrentValue == 1
                                    && colminion.LSDistance(ObjectManager.Player)
                                    > Orbwalking.GetRealAutoAttackRange(null) + 65)
                                {
                                    killableMinionCount++;
                                }
                                else
                                {
                                    killableMinionCount++;
                                }
                            }
                            else break;
                        }

                        if (killableMinionCount >= qCount)
                        {
                            if (!ObjectManager.Player.Spellbook.IsAutoAttacking && !ObjectManager.Player.LSIsDashing())
                            {
                                Q.Cast(minion.ServerPosition);
                                break;
                            }
                        }
                    }
                }
            }

            if (E.IsReady())
            {
                var minECount = Program.laneclear["UseE.Lane"].Cast<ComboBox>().CurrentValue;
                if (minECount != 0)
                {
                    var killableMinionCount = 0;
                    foreach (var m in
                        MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range)
                            .Where(x => E.CanCast(x) && x.Health < EDamage(x)))
                    {
                        if (m.BaseSkinName.ToLower().Contains("siege") || m.BaseSkinName.ToLower().Contains("super"))
                        {
                            killableMinionCount += 2;
                        }
                        else
                        {
                            killableMinionCount++;
                        }
                    }

                    if (killableMinionCount >= minECount && E.IsReady()
                        && ObjectManager.Player.ManaPercent > E.ManaCost * 2)
                    {
                        E.Cast();
                    }
                }
            }

            // Don't miss minion
            if (Program.laneclear["UseE.LaneNon"].Cast<CheckBox>().CurrentValue)
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range * 1);

                foreach (var n in minions)
                {
                    var xH = HealthPrediction.GetHealthPrediction(n, (int)(ObjectManager.Player.AttackCastDelay * 1000),
                        Game.Ping / 2 + 100);
                    if (xH < 0)
                    {
                        if (n.Health < EDamage(n) && E.CanCast(n))
                        {
                            E.Cast(n);
                        }
                        else if (Q.IsReady() && Q.CanCast(n) &&
                                 n.LSDistance(ObjectManager.Player.Position) < Orbwalking.GetRealAutoAttackRange(null) + 75)
                        {
                            xH = HealthPrediction.GetHealthPrediction(n,
                                (int)(ObjectManager.Player.AttackCastDelay * 1000), (int)Q.Speed);
                            if (xH < 0)
                            {
                                var input = new PredictionInput
                                {
                                    Unit = ObjectManager.Player,
                                    Radius = Q.Width,
                                    Delay = Q.Delay,
                                    Speed = Q.Speed,
                                };

                                input.CollisionObjects[0] = CollisionableObjects.Minions;

                                int count =
                                    Collision.GetCollision(new List<Vector3> { n.Position }, input)
                                        .OrderBy(obj => obj.LSDistance(ObjectManager.Player))
                                        .Count(obj => obj.NetworkId != n.NetworkId);
                                if (count == 0)
                                {
                                    Q.Cast(n);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void PermaActive()
        {
            if (Program.misc["Misc.UseSlowE"].Cast<CheckBox>().CurrentValue)
            {
                var minion =
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range)
                        .Find(m => m.Health < EDamage(m) + 10 && E.CanCast(m) && E.Cooldown < 0.0001);
                var enemy =
                    HeroManager.Enemies.Find(
                        e => e.Buffs.Any(b => b.Name.ToLower() == kalistaEBuffName && e.LSIsValidTarget(E.Range)));
                if ((E.CanCast(enemy) || E.CanCast(minion)) && minion != null && enemy != null
                    && ObjectManager.Player.ManaPercent > E.ManaCost * 2)
                {
                    E.Cast();
                }
            }
        }
    }

    public static class Damages
    {

        private static readonly float[] RawRendDamage = { 20, 30, 40, 50, 60 };
        private static readonly float[] RawRendDamagePerSpear = { 10, 14, 19, 25, 32 };
        private static readonly float[] RawRendDamagePerSpearMultiplier = { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        private static float EDamage(Obj_AI_Base target)
        {
            if (target.IsMinion || target.IsMonster)
            {
                int stacksMin = GetMinionStacks(target);
                var indexMin = Kalista.E.Level - 1;

                var EDamageMinion = new float[] { 20, 30, 40, 50, 60 }[indexMin] + (0.6 * ObjectManager.Player.TotalAttackDamage);

                if (stacksMin > 1)
                {
                    EDamageMinion += ((new float[] { 10, 14, 19, 25, 32 }[indexMin] + (new float[] { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f }[indexMin] * ObjectManager.Player.TotalAttackDamage)) * (stacksMin - 1));
                }

                return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical, (float)EDamageMinion) * 0.9f;
            }
            else
            {
                if (GetStacks(target) == 0) return 0;

                int stacksChamps = GetStacks(target);
                var indexChamp = Kalista.E.Level - 1;

                var EDamageChamp = new[] { 0, 20, 30, 40, 50, 60 }[indexChamp] + (0.6 * ObjectManager.Player.TotalAttackDamage);

                if (stacksChamps > 1)
                {
                    EDamageChamp += ((new[] { 0, 10, 14, 19, 25, 32 }[indexChamp] + (new[] { 0, 0.2, 0.225, 0.25, 0.275, 0.3 }[indexChamp] * ObjectManager.Player.TotalAttackDamage)) * (stacksChamps - 1));
                }

                return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical, (float)EDamageChamp);
            }
        }

        private static int GetMinionStacks(Obj_AI_Base minion)
        {
            int stacks = 0;
            foreach (var rendbuff in minion.Buffs.Where(x => x.Name.ToLower().Contains("kalistaexpungemarker")))
            {
                stacks = rendbuff.Count;
            }

            if (stacks == 0 || !minion.HasBuff("kalistaexpungemarker")) return 0;
            return stacks;
        }

        private static int GetStacks(Obj_AI_Base target)
        {
            int stacks = 0;

            if (target.HasBuff("kalistaexpungemarker"))
            {
                foreach (var rendbuff in target.Buffs.Where(x => x.Name.ToLower().Contains("kalistaexpungemarker")))
                {
                    stacks = rendbuff.Count;
                }
            }
            else
            {
                return 0;
            }
            return stacks;
        }

        static Damages()
        {

        }

        public static bool IsRendKillable(this AIHeroClient target)
        {
            // Validate unit
            if (target == null || !target.IsValidTarget(Kalista.E.Range + 200) || !target.HasRendBuff() || target.Health <= 0 || !Kalista.E.IsReady())
            {
                return false;
            }


            var hero = target as AIHeroClient;
            if (hero != null)
            {
                if (hero.HasUndyingBuff() || hero.HasSpellShield())
                {
                    return false;
                }

                if (hero.ChampionName == "Blitzcrank")
                {
                    if (!hero.HasBuff("BlitzcrankManaBarrierCD") && !hero.HasBuff("ManaBarrier"))
                    {
                        return GetActualDamage(hero) > (GetTotalHealthWithShieldsApplied(hero) + (hero.Mana / 2));
                    }

                    if (hero.HasBuff("ManaBarrier") && !(hero.AllShield > 0))
                    {
                        return false;
                    }
                }
            }

            return EDamage(hero) > GetTotalHealthWithShieldsApplied(hero);
        }

        private static double GetTotalHealthWithShieldsApplied(AIHeroClient target)
        {
            return target.Health;
        }


        public static float GetActualDamage(Obj_AI_Base target)
        {
            if (!Kalista.E.IsReady() || !target.HasRendBuff())
                return 0f;

            var damage = EDamage(target);

            return damage;
        }

        public static bool HasRendBuff(this Obj_AI_Base target)
        {
            return target.GetRendBuff() != null;
        }

        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return target.Buffs.Find(b => b.Caster.IsMe && b.IsValid() && b.DisplayName == "KalistaExpungeMarker");
        }
    }
}