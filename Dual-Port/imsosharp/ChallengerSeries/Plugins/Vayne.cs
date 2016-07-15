#region License
/* Copyright (c) LeagueSharp 2016
 * No reproduction is allowed in any way unless given written consent
 * from the LeagueSharp staff.
 * 
 * Author: imsosharp
 * Date: 2/21/2016
 * File: Vayne.cs
 */
#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using Challenger_Series.Utils;
using LeagueSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using SpellDatabase = LeagueSharp.SDK.SpellDatabase;
using LeagueSharp.Data.Enumerations;
using Geometry = Challenger_Series.Utils.Geometry;
using TargetSelector = PortAIO.TSManager;

namespace Challenger_Series.Plugins
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp.Common;

    public class Vayne : CSPlugin
    {
        

        #region ctor
        public Vayne()
        {
            base.Q = new LeagueSharp.SDK.Spell(EloBuddy.SpellSlot.Q, 300);
            base.W = new LeagueSharp.SDK.Spell(EloBuddy.SpellSlot.W);
            base.E = new LeagueSharp.SDK.Spell(EloBuddy.SpellSlot.E, 550);
            base.R = new LeagueSharp.SDK.Spell(EloBuddy.SpellSlot.R);

            base.E.SetSkillshot(0.42f, 50f, 1300f, false, LeagueSharp.SDK.Enumerations.SkillshotType.SkillshotLine);
            CachedGapclosers = new List<Tuple<string, LeagueSharp.Data.DataTypes.SpellDatabaseEntry>>();
            CachedCrowdControl = new List<Tuple<string, LeagueSharp.Data.DataTypes.SpellDatabaseEntry>>();
            foreach (var enemy in EloBuddy.ObjectManager.Get<EloBuddy.AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                foreach (var spell in enemy.Spellbook.Spells)
                {
                    var sdata = SpellDatabase.GetByName(spell.Name);
                    if (sdata != null)
                    {
                        if (sdata.SpellTags == null)
                        {
                            break;
                        }
                        if (
                            sdata.SpellTags.Any(
                                st => st == SpellTags.Dash || st == LeagueSharp.Data.Enumerations.SpellTags.Blink))
                        {
                            CachedGapclosers.Add(new Tuple<string, LeagueSharp.Data.DataTypes.SpellDatabaseEntry>(enemy.CharData.BaseSkinName,
                                sdata));
                        }
                        if (sdata.SpellTags.Any(st => st == SpellTags.CrowdControl))
                        {
                            CachedCrowdControl.Add(new Tuple<string, LeagueSharp.Data.DataTypes.SpellDatabaseEntry>(enemy.CharData.BaseSkinName,
                                sdata));
                        }
                    }
                }
            }
            InitMenu();
            DelayedOnUpdate += OnUpdate;
            LSEvents.BeforeAttack += Orbwalker_OnPreAttack;
            LSEvents.AfterAttack += Orbwalker_OnPostAttack;
            EloBuddy.Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            EloBuddy.Drawing.OnDraw += OnDraw;
            Events.OnGapCloser += OnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
        }

        #endregion

        #region Cache bik
        public List<Tuple<string, LeagueSharp.Data.DataTypes.SpellDatabaseEntry>> CachedGapclosers;
        public List<Tuple<string, LeagueSharp.Data.DataTypes.SpellDatabaseEntry>> CachedCrowdControl;
        //private Items.Item ZZrot = new Items.Item(3512, 400);
        #endregion

        #region Events
        public bool IsWindingUp = false;
        public override void OnUpdate(EventArgs args)
        {
            base.OnUpdate(args);
            if (UseEBool && E.IsReady())
            {
                foreach (var enemy in ValidTargets.Where(e => e.LSIsValidTarget(550)))
                {
                    /*if (ZZrot.IsReady && enemy.IsValidTarget(ZZrot.Range))
                    {
                        if (E.CastOnUnit(enemy))
                        {
                            DelayAction.Add(100,
                                () =>
                                    {
                                        this.ZZrot.Cast(
                                            enemy.Position.ToVector2()
                                                .Extend(ObjectManager.Player.ServerPosition.ToVector2(), -100));
                                    });
                            return;
                        }
                    }*/
                    if (enemy.IsCastingInterruptableSpell())
                    {
                        E.CastOnUnit(enemy);
                    }
                    if (IsCondemnable(enemy))
                    {
                        if (EDelaySlider > 0)
                        {
                            var thisEnemy = enemy;
                            LeagueSharp.SDK.Core.Utils.DelayAction.Add(EDelaySlider, () => E.CastOnUnit(thisEnemy));
                            return;
                        }
                        E.CastOnUnit(enemy);
                    }
                }
            }
            if (SemiAutomaticCondemnKey)
            {
                foreach (
                    var hero in
                        ValidTargets.Where(
                            h => h.ServerPosition.Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 550))
                {
                    var prediction = E.GetPrediction(hero);
                    for (var i = 40; i < 425; i += 125)
                    {
                        var flags = EloBuddy.NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.ToVector2()
                                .Extend(EloBuddy.ObjectManager.Player.ServerPosition.ToVector2(),
                                    -i)
                                .ToVector3());
                        if (flags.HasFlag(EloBuddy.CollisionFlags.Wall) || flags.HasFlag(EloBuddy.CollisionFlags.Building))
                        {
                            if (EDelaySlider > 0)
                            {
                                var thisEnemy = hero;
                                LeagueSharp.SDK.Core.Utils.DelayAction.Add(EDelaySlider, () => E.CastOnUnit(thisEnemy));
                                return;
                            }
                            E.CastOnUnit(hero);
                            return;
                        }
                    }
                }
            }
            if (UseEInterruptBool)
            {
                var possibleChannelingTarget =
                    ValidTargets.FirstOrDefault(
                        e =>
                            e.ServerPosition.Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 550 &&
                            e.IsCastingInterruptableSpell());
                if (possibleChannelingTarget.LSIsValidTarget())
                {
                    if (EDelaySlider > 0)
                    {
                        var thisEnemy = possibleChannelingTarget;
                        LeagueSharp.SDK.Core.Utils.DelayAction.Add(EDelaySlider, () => E.CastOnUnit(thisEnemy));
                        return;
                    }
                    E.CastOnUnit(possibleChannelingTarget);
                }
            }
        }

        private void OnGapCloser(object oSender, Events.GapCloserEventArgs args)
        {
            var sender = args.Sender;
            var castedE = false;
            if (UseEAntiGapcloserBool)
            {
                if (args.IsDirectedToPlayer)
                {
                    if (E.IsReady())
                    {
                        if (EDelaySlider > 0)
                        {
                            var thisEnemy = sender;
                            LeagueSharp.SDK.Core.Utils.DelayAction.Add(EDelaySlider, () => E.CastOnUnit(thisEnemy));
                            return;
                        }
                        E.CastOnUnit(sender);
                        castedE = true;
                    }
                    if (Q.IsReady())
                    {
                        switch (UseQAntiGapcloserStringList)
                        {
                            case 3:
                                {
                                    if (args.End.Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 350)
                                    {
                                        var pos = EloBuddy.ObjectManager.Player.ServerPosition.LSExtend(args.End, -300);
                                        if (!IsDangerousPosition(pos))
                                        {
                                            Q.Cast(pos);
                                        }
                                    }
                                    if (sender.Distance(EloBuddy.ObjectManager.Player) < 350)
                                    {
                                        var pos = EloBuddy.ObjectManager.Player.ServerPosition.LSExtend(sender.Position, -300);
                                        if (!IsDangerousPosition(pos))
                                        {
                                            Q.Cast(pos);
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (!E.IsReady() && !castedE)
                                    {
                                        if (args.End.Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 350)
                                        {
                                            var pos = EloBuddy.ObjectManager.Player.ServerPosition.LSExtend(args.End, -300);
                                            if (!IsDangerousPosition(pos))
                                            {
                                                Q.Cast(pos);
                                            }
                                        }
                                        if (sender.Distance(EloBuddy.ObjectManager.Player) < 350)
                                        {
                                            var pos = EloBuddy.ObjectManager.Player.ServerPosition.LSExtend(sender.Position, -300);
                                            if (!IsDangerousPosition(pos))
                                            {
                                                Q.Cast(pos);
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
        }

        private void OnInterruptableTarget(object oSender, Events.InterruptableTargetEventArgs args)
        {
            var sender = args.Sender;
            if (args.DangerLevel >= DangerLevel.Medium && EloBuddy.ObjectManager.Player.Distance(sender) < 550 && !IsInvulnerable(sender))
            {
                if (EDelaySlider > 0)
                {
                    var thisEnemy = sender;
                    LeagueSharp.SDK.Core.Utils.DelayAction.Add(EDelaySlider, () => E.CastOnUnit(thisEnemy));
                    return;
                }
                E.CastOnUnit(sender);
            }
        }

        public override void OnProcessSpellCast(EloBuddy.GameObject sender, EloBuddy.GameObjectProcessSpellCastEventArgs args)
        {
            base.OnProcessSpellCast(sender, args);
            if (sender is EloBuddy.AIHeroClient && sender.IsEnemy)
            {
                var objaiherosender = (EloBuddy.AIHeroClient)sender;
                if (!IsInvulnerable(objaiherosender) && args.SData.Name == "summonerflash" && args.End.Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 350)
                {
                    if (EDelaySlider > 0)
                    {
                        var thisEnemy = objaiherosender;
                        LeagueSharp.SDK.Core.Utils.DelayAction.Add(EDelaySlider, () => E.CastOnUnit(thisEnemy));
                        return;
                    }
                    E.CastOnUnit(objaiherosender);
                }
            }
        }

        public override void OnDraw(EventArgs args)
        {
            base.OnDraw(args);
            if (DrawWStacksBool)
            {
                var target =
                    ValidTargets.FirstOrDefault(
                        enemy => enemy.HasBuff("vaynesilvereddebuff") && enemy.LSIsValidTarget(2000));
                if (target.LSIsValidTarget())
                {
                    var x = target.HPBarPosition.X + 50;
                    var y = target.HPBarPosition.Y - 20;

                    if (W.Level > 0)
                    {
                        int stacks = target.GetBuffCount("vaynesilvereddebuff");
                        if (stacks > -1)
                        {
                            for (var i = 0; i < 3; i++)
                            {
                                EloBuddy.Drawing.DrawLine(x + i * 20, y, x + i * 20 + 10, y, 10,
                                    stacks <= i ? Color.DarkGray : Color.DeepSkyBlue);
                            }
                        }
                    }
                }
            }
        }
        private void Orbwalker_OnPostAttack(AfterAttackArgs args)
        {
            PortAIO.OrbwalkerManager.ForcedTarget(null);
            var possible2WTarget = ValidTargets.FirstOrDefault(
                h =>
                    h.ServerPosition.Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 500 &&
                    h.GetBuffCount("vaynesilvereddebuff") == 2);
            if (!PortAIO.OrbwalkerManager.isComboActive)
            {
                if (possible2WTarget.LSIsValidTarget() && UseEAs3rdWProcBool && LeagueSharp.SDK.Core.Utils.MathUtils.GetWaypoints(possible2WTarget).LastOrDefault().Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 1000)
                {
                    if (EDelaySlider > 0)
                    {
                        var thisEnemy = possible2WTarget;
                        LeagueSharp.SDK.Core.Utils.DelayAction.Add(EDelaySlider, () => E.CastOnUnit(thisEnemy));
                        return;
                    }
                    E.CastOnUnit(possible2WTarget);
                }
            }
            if (args.Target is EloBuddy.AIHeroClient && UseQBool)
            {
                if (Q.IsReady())
                {
                    var tg = args.Target as EloBuddy.AIHeroClient;
                    if (tg != null)
                    {
                        var mode = QModeStringList;
                        var tumblePosition = EloBuddy.Game.CursorPos;
                        switch (mode)
                        {
                            case 1:
                                tumblePosition = GetTumblePos(tg);
                                break;
                            default:
                                tumblePosition = EloBuddy.Game.CursorPos;
                                break;
                        }
                        if (tumblePosition.Distance(EloBuddy.ObjectManager.Player.Position) > 2000 || IsDangerousPosition(tumblePosition)) return;
                        Q.Cast(tumblePosition);
                    }
                }
            }
            if (args.Target is EloBuddy.Obj_AI_Minion && PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                var tg = args.Target as EloBuddy.Obj_AI_Minion;
                if (E.IsReady())
                {
                    if (this.IsMinionCondemnable(tg) && GameObjects.Jungle.Any(m => m.NetworkId == tg.NetworkId) && tg.LSIsValidTarget() && this.UseEJungleFarm)
                    {
                        if (this.EDelaySlider > 0)
                        {
                            var thisEnemy = tg;
                            LeagueSharp.SDK.Core.Utils.DelayAction.Add(EDelaySlider, () => E.CastOnUnit(thisEnemy));
                            return;
                        }
                        E.CastOnUnit(tg);
                    }
                }
                if (this.UseQFarm && this.Q.IsReady())
                {
                    if (tg.CharData.BaseSkinName.Contains("SRU_") && !tg.CharData.BaseSkinName.Contains("Mini") && tg.LSIsValidTarget() && !this.IsDangerousPosition(EloBuddy.Game.CursorPos))
                    {
                        Q.Cast(EloBuddy.Game.CursorPos);
                    }
                    if (EloBuddy.ObjectManager.Player.UnderAllyTurret() && GameObjects.EnemyMinions.Count(
                            m =>
                                m.Position.Distance(EloBuddy.ObjectManager.Player.Position) < 550 && m.Health < EloBuddy.ObjectManager.Player.LSGetAutoAttackDamage(m) && Health.GetPrediction(m, (int)(100 + (EloBuddy.Game.Ping / 2) + EloBuddy.ObjectManager.Player.AttackCastDelay * 1000)) > 3) > 1 &&
                        !this.IsDangerousPosition(EloBuddy.Game.CursorPos))
                    {
                        Q.Cast(EloBuddy.Game.CursorPos);
                    }
                    if (EloBuddy.ObjectManager.Player.UnderAllyTurret())
                    {
                        if (GameObjects.EnemyMinions.Count(
                            m =>
                                m.Position.Distance(EloBuddy.ObjectManager.Player.Position) < 550 &&
                                m.Health < EloBuddy.ObjectManager.Player.LSGetAutoAttackDamage(m) + Q.GetDamage(m)) > 0 && !this.IsDangerousPosition(EloBuddy.Game.CursorPos))
                        {
                            Q.Cast(EloBuddy.Game.CursorPos);
                        }
                    }
                }
            }
            if (UseQOnlyAt2WStacksBool && !PortAIO.OrbwalkerManager.isComboActive && possible2WTarget.LSIsValidTarget())
            {
                Q.Cast(GetTumblePos(possible2WTarget));
            }
        }
        private void Orbwalker_OnPreAttack(BeforeAttackArgs args)
        {
            if (args.Process)
            {
                IsWindingUp = true;
            }
            else
            {
                IsWindingUp = false;
            }
            if (R.IsReady() && PortAIO.OrbwalkerManager.isComboActive && UseRBool && args.Target is EloBuddy.AIHeroClient && (!(args.Target as EloBuddy.AIHeroClient).IsUnderEnemyTurret() || EloBuddy.ObjectManager.Player.IsUnderEnemyTurret()) && EloBuddy.ObjectManager.Player.CountAllyHeroesInRange(800) >= EloBuddy.ObjectManager.Player.CountEnemyHeroesInRange(800))
            {
                R.Cast();
            }
            var possible2WTarget = ValidTargets.FirstOrDefault(
                h =>
                    h.ServerPosition.Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 500 &&
                    h.GetBuffCount("vaynesilvereddebuff") == 2);
            if (TryToFocus2WBool && possible2WTarget.LSIsValidTarget())
            {
                PortAIO.OrbwalkerManager.ForcedTarget(possible2WTarget);
            }
            if (EloBuddy.ObjectManager.Player.HasBuff("vaynetumblefade") && DontAttackWhileInvisibleAndMeelesNearBool)
            {
                if (
                    ValidTargets.Any(
                        e => e.ServerPosition.Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 350 && e.IsMelee))
                {
                    args.Process = false;
                }
            }
            var possibleTarget = TargetSelector.GetTarget(615, EloBuddy.DamageType.Physical);
            if (possibleTarget != null && args.Target is EloBuddy.Obj_AI_Minion &&
                UseQBonusOnEnemiesNotCS && EloBuddy.ObjectManager.Player.HasBuff("vaynetumblebonus"))
            {
                PortAIO.OrbwalkerManager.ForcedTarget(possibleTarget);
                args.Process = false;
            }
            var possibleNearbyMeleeChampion =
                ValidTargets.FirstOrDefault(
                    e => e.ServerPosition.Distance(EloBuddy.ObjectManager.Player.ServerPosition) < 350);

            if (possibleNearbyMeleeChampion.LSIsValidTarget())
            {
                if (Q.IsReady() && UseQBool)
                {
                    var pos = EloBuddy.ObjectManager.Player.ServerPosition.LSExtend(possibleNearbyMeleeChampion.ServerPosition,
                        -350);
                    if (!IsDangerousPosition(pos))
                    {
                        Q.Cast(pos);
                        args.Process = false;
                    }
                }
                if (UseEWhenMeleesNearBool && !Q.IsReady() && E.IsReady())
                {
                    var possibleMeleeChampionsGapclosers = from tuplet in CachedGapclosers
                                                           where tuplet.Item1 == possibleNearbyMeleeChampion.CharData.BaseSkinName
                                                           select tuplet.Item2;
                    if (possibleMeleeChampionsGapclosers.FirstOrDefault() != null)
                    {
                        if (
                            possibleMeleeChampionsGapclosers.Any(
                                gapcloserEntry =>
                                    possibleNearbyMeleeChampion.Spellbook.GetSpell(gapcloserEntry.Slot).IsReady()))
                        {
                            return;
                        }
                    }
                    if (
                        LeagueSharp.SDK.Core.Utils.MathUtils.GetWaypoints(possibleNearbyMeleeChampion)
                            .LastOrDefault()
                            .Distance(EloBuddy.ObjectManager.Player.ServerPosition) < possibleNearbyMeleeChampion.AttackRange)
                    {
                        if (EDelaySlider > 0)
                        {
                            var thisEnemy = possibleNearbyMeleeChampion;
                            LeagueSharp.SDK.Core.Utils.DelayAction.Add(EDelaySlider, () => E.CastOnUnit(thisEnemy));
                            return;
                        }
                        E.CastOnUnit(possibleNearbyMeleeChampion);
                    }
                }
            }
        }

        #endregion

        #region Menu
        private Menu ComboMenu, HarassMenu, FarmMenu, DrawMenu, CondemnMenu;
        bool UseQBool { get { return getCheckBoxItem(ComboMenu, "useq"); } }
        bool TryToFocus2WBool { get { return getCheckBoxItem(ComboMenu, "focus2w"); } }
        bool UseEBool { get { return getCheckBoxItem(CondemnMenu, "usee"); } }
        bool UseEInterruptBool { get { return getCheckBoxItem(CondemnMenu, "useeinterrupt"); } }
        bool UseEAntiGapcloserBool { get { return getCheckBoxItem(CondemnMenu, "useeantigapcloser"); } }
        bool UseEWhenMeleesNearBool { get { return getCheckBoxItem(CondemnMenu, "ewhenmeleesnear"); } }
        bool UseEAs3rdWProcBool { get { return getCheckBoxItem(HarassMenu, "usee3rdwproc"); } }
        bool UseQOnlyAt2WStacksBool { get { return getCheckBoxItem(HarassMenu, "useqonlyon2stackedenemies"); } }
        bool DontAttackWhileInvisibleAndMeelesNearBool { get { return getCheckBoxItem(ComboMenu, "dontattackwhileinvisible"); } }
        bool UseRBool { get { return getCheckBoxItem(ComboMenu, "user"); } }
        bool UseQBonusOnEnemiesNotCS { get { return getCheckBoxItem(HarassMenu, "useqonenemiesnotcs"); } }
        bool UseQFarm { get { return getCheckBoxItem(FarmMenu, "useqfarm"); } }
        bool UseEJungleFarm { get { return getCheckBoxItem(FarmMenu, "useejgfarm"); } }
        bool SemiAutomaticCondemnKey { get { return getKeyBindItem(CondemnMenu, "semiautoekey"); } }
        bool DrawWStacksBool { get { return getCheckBoxItem(DrawMenu, "drawwstacks"); } }
        int EDelaySlider { get { return getSliderItem(CondemnMenu, "edelay"); } }
        int EPushDistanceSlider { get { return getSliderItem(CondemnMenu, "epushdist"); } }
        int EHitchanceSlider { get { return getSliderItem(CondemnMenu, "ehitchance"); } }
        int EModeStringList { get { return getSliderItem(CondemnMenu, "emode"); } }
        int QModeStringList { get { return getSliderItem(ComboMenu, "qmode"); } }
        int UseQAntiGapcloserStringList { get { return getSliderItem(ComboMenu, "qantigc"); } }
        new bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        new int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        new bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        new int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private void InitMenu()
        {
            ComboMenu = MainMenu.AddSubMenu("Combo Settings:", "Combo Settings: ");
            ComboMenu.Add("useq", new CheckBox("Auto Q", true));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("1 : PRADA | 2 : MARKSMAN | 3 : VHR | 4: SharpShooter");
            ComboMenu.Add("qmode", new Slider("Q Mode: ", 1, 1, 4));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("1 : NEVER | 2 : E-NOT-READY | 3 : ALWAYS ");
            ComboMenu.Add("qantigc", new Slider("Use Q Antigapcloser ", 1, 1, 3));
            ComboMenu.AddSeparator();
            ComboMenu.Add("focus2w", new CheckBox("Try To Focus 2W", false));
            ComboMenu.Add("dontattackwhileinvisible", new CheckBox("Smart Invisible Attacking", true));
            ComboMenu.Add("user", new CheckBox("Use R In Combo", false));

            CondemnMenu = MainMenu.AddSubMenu("Condemn Settings:", "Condemn Settings:");
            CondemnMenu.Add("usee", new CheckBox("Auto E", true));
            CondemnMenu.Add("edelay", new Slider("E Delay (in ms) ", 0, 0, 100));
            CondemnMenu.AddSeparator();
            CondemnMenu.AddLabel("1 : PRADASMART | 2 : PRADAPERFECT | 3 : MARKSMAN");
            CondemnMenu.AddLabel("4 : SHARPSHOOTER | 5 : GOSU | 6 : VHR");
            CondemnMenu.AddLabel("7 : PRADALEGACY | 8 : FASTEST | 9 : OLDPRADA");
            CondemnMenu.Add("emode", new Slider("E Mode : ", 8, 1, 9));
            CondemnMenu.AddSeparator();
            CondemnMenu.Add("useeinterrupt", new CheckBox("Use E To Interrupt", true));
            CondemnMenu.Add("useeantigapcloser", new CheckBox("Use E AntiGapcloser", true));
            CondemnMenu.Add("ewhenmeleesnear", new CheckBox("Use E when Melee near", false));
            CondemnMenu.Add("epushdist", new Slider("E Push Distance : ", 425, 300, 470));
            CondemnMenu.Add("ehitchance", new Slider("Condemn Hitchance : ", 50, 0, 100));
            CondemnMenu.Add("semiautoekey", new KeyBind("Semi Automatic Condemn", false, KeyBind.BindTypes.HoldActive, "E".ToCharArray()[0]));

            HarassMenu = MainMenu.AddSubMenu("Harass Settings:", "Harass Settings: ");
            HarassMenu.Add("usee3rdwproc", new CheckBox("Use E as 3rd W Proc", false));
            HarassMenu.Add("useqonlyon2stackedenemies", new CheckBox("Use Q If Enemy Have 2W Stacks", false));
            HarassMenu.Add("useqonenemiesnotcs", new CheckBox("Use Q Bonus On ENEMY not CS", false));

            FarmMenu = MainMenu.AddSubMenu("Farm Settings:", "Farm Settings: ");
            FarmMenu.Add("useqfarm", new CheckBox("Use Q",true));
            FarmMenu.Add("useejgfarm", new CheckBox("Use E Jungle",true));

            DrawMenu = MainMenu.AddSubMenu("Drawing Settings:", "Drawing Settings: ");
            DrawMenu.Add("drawwstacks", new CheckBox("Draw W Stacks", true));
        }

        #endregion Menu

        #region ChampionLogic

        private bool IsCondemnable(EloBuddy.AIHeroClient hero)
        {
            if (!hero.LSIsValidTarget(550f) || hero.HasBuffOfType(EloBuddy.BuffType.SpellShield) ||
                hero.HasBuffOfType(EloBuddy.BuffType.SpellImmunity) || hero.IsDashing()) return false;

            //values for pred calc pP = player position; p = enemy position; pD = push distance
            var pP = EloBuddy.ObjectManager.Player.ServerPosition;
            var p = hero.ServerPosition;
            var pD = EPushDistanceSlider;
            var mode = EModeStringList;


            if (mode == 1 && (IsCollisionable(p.LSExtend(pP, -pD)) || IsCollisionable(p.LSExtend(pP, -pD / 2f)) ||
                                         IsCollisionable(p.LSExtend(pP, -pD / 3f))))
            {
                if (!hero.CanMove)
                    return true;

                var enemiesCount = EloBuddy.ObjectManager.Player.CountEnemyHeroesInRange(1200);
                if (enemiesCount > 1 && enemiesCount <= 3)
                {
                    var prediction = E.GetPrediction(hero);
                    for (var i = 15; i < pD; i += 75)
                    {
                        if (i > pD)
                        {
                            var lastPosFlags = EloBuddy.NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.ToVector2()
                                .Extend(
                                    pP.ToVector2(),
                                    -pD)
                                .ToVector3());
                            if (lastPosFlags.HasFlag(EloBuddy.CollisionFlags.Wall) || lastPosFlags.HasFlag(EloBuddy.CollisionFlags.Building))
                            {
                                return true;
                            }
                            return false;
                        }
                        var posFlags = EloBuddy.NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.ToVector2()
                                .Extend(
                                    pP.ToVector2(),
                                    -i)
                                .ToVector3());
                        if (posFlags.HasFlag(EloBuddy.CollisionFlags.Wall) || posFlags.HasFlag(EloBuddy.CollisionFlags.Building))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    var hitchance = EHitchanceSlider;
                    var angle = 0.20 * hitchance;
                    const float travelDistance = 0.5f;
                    var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                        (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                    var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                        (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                    for (var i = 15; i < pD; i += 100)
                    {
                        if (i > pD) return false;
                        if (IsCollisionable(pP.ToVector2().Extend(alpha,
                            i)
                            .ToVector3()) && IsCollisionable(pP.ToVector2().Extend(beta, i).ToVector3())) return true;
                    }
                    return false;
                }
            }

            if (mode == 2 &&
                (IsCollisionable(p.LSExtend(pP, -pD)) || IsCollisionable(p.LSExtend(pP, -pD / 2f)) ||
                 IsCollisionable(p.LSExtend(pP, -pD / 3f))))
            {
                if (!hero.CanMove ||
                    (hero.Spellbook.IsAutoAttacking))
                    return true;

                var hitchance = EHitchanceSlider;
                var angle = 0.20 * hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                for (var i = 15; i < pD; i += 100)
                {
                    if (i > pD)
                    {
                        return IsCollisionable(alpha.Extend(pP.ToVector2(),
                            -pD)
                            .ToVector3()) && IsCollisionable(beta.Extend(pP.ToVector2(), -pD).ToVector3());
                    }
                    if (IsCollisionable(alpha.Extend(pP.ToVector2(),
                        -i)
                        .ToVector3()) && IsCollisionable(beta.Extend(pP.ToVector2(), -i).ToVector3())) return true;
                }
                return false;
            }

            if (mode == 9)
            {
                if (!hero.CanMove ||
                    (hero.Spellbook.IsAutoAttacking))
                    return true;

                var hitchance = EHitchanceSlider;
                var angle = 0.20 * hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                for (var i = 15; i < pD; i += 100)
                {
                    if (IsCollisionable(pP.ToVector2().Extend(alpha,
                        i)
                        .ToVector3()) || IsCollisionable(pP.ToVector2().Extend(beta, i).ToVector3())) return true;
                }
                return false;
            }

            if (mode == 3)
            {
                var prediction = E.GetPrediction(hero);
                return EloBuddy.NavMesh.GetCollisionFlags(
                    prediction.UnitPosition.ToVector2()
                        .Extend(
                            pP.ToVector2(),
                            -pD)
                        .ToVector3()).HasFlag(EloBuddy.CollisionFlags.Wall) ||
                       EloBuddy.NavMesh.GetCollisionFlags(
                           prediction.UnitPosition.ToVector2()
                               .Extend(
                                   pP.ToVector2(),
                                   -pD / 2f)
                               .ToVector3()).HasFlag(EloBuddy.CollisionFlags.Wall);
            }

            if (mode == 4)
            {
                var prediction = E.GetPrediction(hero);
                for (var i = 15; i < pD; i += 100)
                {
                    if (i > pD) return false;
                    var posCF = EloBuddy.NavMesh.GetCollisionFlags(
                        prediction.UnitPosition.ToVector2()
                            .Extend(
                                pP.ToVector2(),
                                -i)
                            .ToVector3());
                    if (posCF.HasFlag(EloBuddy.CollisionFlags.Wall) || posCF.HasFlag(EloBuddy.CollisionFlags.Building))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (mode == 5)
            {
                var prediction = E.GetPrediction(hero);
                for (var i = 15; i < pD; i += 75)
                {
                    var posCF = EloBuddy.NavMesh.GetCollisionFlags(
                        prediction.UnitPosition.ToVector2()
                            .Extend(
                                pP.ToVector2(),
                                -i)
                            .ToVector3());
                    if (posCF.HasFlag(EloBuddy.CollisionFlags.Wall) || posCF.HasFlag(EloBuddy.CollisionFlags.Building))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (mode == 6)
            {
                var prediction = E.GetPrediction(hero);
                for (var i = 15; i < pD; i += (int)hero.BoundingRadius) //:frosty:
                {
                    var posCF = EloBuddy.NavMesh.GetCollisionFlags(
                        prediction.UnitPosition.ToVector2()
                            .Extend(
                                pP.ToVector2(),
                                -i)
                            .ToVector3());
                    if (posCF.HasFlag(EloBuddy.CollisionFlags.Wall) || posCF.HasFlag(EloBuddy.CollisionFlags.Building))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (mode == 7)
            {
                var prediction = E.GetPrediction(hero);
                for (var i = 15; i < pD; i += 75)
                {
                    var posCF = EloBuddy.NavMesh.GetCollisionFlags(
                        prediction.UnitPosition.ToVector2()
                            .Extend(
                                pP.ToVector2(),
                                -i)
                            .ToVector3());
                    if (posCF.HasFlag(EloBuddy.CollisionFlags.Wall) || posCF.HasFlag(EloBuddy.CollisionFlags.Building))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (mode == 8 &&
                (IsCollisionable(p.LSExtend(pP, -pD)) || IsCollisionable(p.LSExtend(pP, -pD / 2f)) ||
                 IsCollisionable(p.LSExtend(pP, -pD / 3f))))
            {
                return true;
            }

            return false;
        }

        private bool IsMinionCondemnable(EloBuddy.Obj_AI_Minion minion)
        {
            return GameObjects.JungleLarge.Any(m => minion.NetworkId == m.NetworkId) &&
                EloBuddy.NavMesh.GetCollisionFlags(
        minion.Position.ToVector2()
                    .Extend(
                        EloBuddy.ObjectManager.Player.Position.ToVector2(),
                        -400)
                    .ToVector3()).HasFlag(EloBuddy.CollisionFlags.Wall) ||
                   EloBuddy.NavMesh.GetCollisionFlags(
        minion.Position.ToVector2()
                           .Extend(
                               EloBuddy.ObjectManager.Player.Position.ToVector2(),
                               -200)
                           .ToVector3()).HasFlag(EloBuddy.CollisionFlags.Wall);
        }

        private Vector3 GetAggressiveTumblePos(EloBuddy.Obj_AI_Base target)
        {
            var cursorPos = EloBuddy.Game.CursorPos;

            if (!IsDangerousPosition(cursorPos)) return cursorPos;
            //if the target is not a melee and he's alone he's not really a danger to us, proceed to 1v1 him :^ )
            if (!target.IsMelee && EloBuddy.ObjectManager.Player.CountEnemyHeroesInRange(800) == 1) return cursorPos;

            var aRC =
                new Challenger_Series.Utils.Geometry.Circle(EloBuddy.ObjectManager.Player.ServerPosition.ToVector2(), 300).ToPolygon().ToClipperPath();
            var targetPosition = target.ServerPosition;


            foreach (var p in aRC)
            {
                var v3 = new Vector2(p.X, p.Y).ToVector3();
                var dist = v3.Distance(targetPosition);
                if (dist > 325 && dist < 450)
                {
                    return v3;
                }
            }
            return Vector3.Zero;
        }

        private Vector3 GetTumblePos(EloBuddy.Obj_AI_Base target)
        {
            if (!PortAIO.OrbwalkerManager.isComboActive)
                return GetAggressiveTumblePos(target);

            var cursorPos = EloBuddy.Game.CursorPos;
            var targetCrowdControl = from tuplet in CachedCrowdControl
                                     where tuplet.Item1 == target.CharData.BaseSkinName
                                     select tuplet.Item2;

            if (!IsDangerousPosition(cursorPos) && !(targetCrowdControl.FirstOrDefault() != null && targetCrowdControl.Any(
                        crowdControlEntry =>
                            target.Spellbook.GetSpell(crowdControlEntry.Slot).IsReady()))) return cursorPos;

            //if the target is not a melee and he's alone he's not really a danger to us, proceed to 1v1 him :^ )
            if (!target.IsMelee && EloBuddy.ObjectManager.Player.CountEnemyHeroesInRange(800) == 1) return cursorPos;
            var targetWaypoints = LeagueSharp.SDK.Core.Utils.MathUtils.GetWaypoints(target);
            if (targetWaypoints[targetWaypoints.Count - 1].Distance(EloBuddy.ObjectManager.Player.ServerPosition) > 550)
                return Vector3.Zero;

            var aRC =
                new Challenger_Series.Utils.Geometry.Circle(EloBuddy.ObjectManager.Player.ServerPosition.ToVector2(), 300).ToPolygon().ToClipperPath();
            var targetPosition = target.ServerPosition;
            var pList = (from p in aRC
                         select new Vector2(p.X, p.Y).ToVector3()
                             into v3
                         let dist = v3.Distance(targetPosition)
                         where !IsDangerousPosition(v3) && dist < 500
                         select v3).ToList();

            if (EloBuddy.ObjectManager.Player.UnderTurret() || EloBuddy.ObjectManager.Player.CountEnemyHeroesInRange(800) == 1 ||
                cursorPos.CountEnemyHeroesInRange(450) <= 1)
            {
                return pList.Count > 1 ? pList.OrderBy(el => el.Distance(cursorPos)).FirstOrDefault() : Vector3.Zero;
            }
            return pList.Count > 1
                ? pList.OrderByDescending(el => el.Distance(cursorPos)).FirstOrDefault()
                : Vector3.Zero;
        }

        private int VayneWStacks(EloBuddy.Obj_AI_Base o)
        {
            if (o == null) return 0;
            if (o.Buffs.FirstOrDefault(b => b.Name.Contains("vaynesilver")) == null ||
                !o.Buffs.Any(b => b.Name.Contains("vaynesilver"))) return 0;
            return o.Buffs.FirstOrDefault(b => b.Name.Contains("vaynesilver")).Count;
        }

        private Vector3 Randomize(Vector3 pos)
        {
            var r = new Random(Environment.TickCount);
            return new Vector2(pos.X + r.Next(-150, 150), pos.Y + r.Next(-150, 150)).ToVector3();
        }

        private bool IsDangerousPosition(Vector3 pos)
        {
            return ValidTargets.Any(e => e.IsMelee && e.Distance(pos) < 375) ||
                   (pos.UnderTurret(true) && !EloBuddy.ObjectManager.Player.ServerPosition.UnderTurret(true));
        }

        private bool IsKillable(EloBuddy.AIHeroClient hero)
        {
            return EloBuddy.ObjectManager.Player.LSGetAutoAttackDamage(hero) * 2 < hero.Health;
        }

        private bool IsCollisionable(Vector3 pos)
        {
            return EloBuddy.NavMesh.GetCollisionFlags(pos).HasFlag(EloBuddy.CollisionFlags.Wall) ||
                   (EloBuddy.NavMesh.GetCollisionFlags(pos).HasFlag(EloBuddy.CollisionFlags.Building));
        }

        private bool IsInvulnerable(EloBuddy.AIHeroClient target)
        {
            return target.HasBuffOfType(EloBuddy.BuffType.SpellShield) || target.HasBuffOfType(EloBuddy.BuffType.SpellImmunity);
        }

        private int CountHeroesInRange(EloBuddy.AIHeroClient target, bool checkteam, float range = 1200f)
        {
            var objListTeam =
                EloBuddy.ObjectManager.Get<EloBuddy.AIHeroClient>()
                    .Where(
                        x => x.LSIsValidTarget(range, false));

            return objListTeam.Count(hero => checkteam ? hero.Team != target.Team : hero.Team == target.Team);
        }

        #endregion
    }
}