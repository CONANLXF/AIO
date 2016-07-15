
namespace Valvrave_Sharp.Plugin
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Modes;
    using EloBuddy;
    using SharpDX;

    using Valvrave_Sharp.Core;
    using Valvrave_Sharp.Evade;

    using Color = System.Drawing.Color;
    using Skillshot = Valvrave_Sharp.Evade.Skillshot;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Polygons;
    #endregion

    internal class Yasuo : Program
    {
        #region Constants

        private const float QDelay = 0.38f, Q2Delay = 0.35f, QDelays = 0.2f, Q2Delays = 0.3f;

        private const int RWidth = 400;

        #endregion

        #region Static Fields

        private static int cDash;

        public static Menu config = _MainMenu;

        private static bool haveQ3;

        private static bool isDash;

        private static int lastE;

        private static Vector3 posDash;

        private static MissileClient wallLeft, wallRight;

        public static EloBuddy.SDK.Spell.Skillshot QEB;

        private static EloBuddy.SDK.Spell.Skillshot Q3EB;

        private static RectanglePoly wallPoly;

        public static Menu comboMenu, hybridMenu, lcMenu, lhMenu, ksMenu, fleeMenu, drawMenu, miscMenu;

        #endregion

        #region Constructors and Destructors

        private static EloBuddy.SDK.Spell.Skillshot GetQ()
        {
            return new EloBuddy.SDK.Spell.Skillshot(SpellSlot.Q, 475, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 325, QSpeed(), 55)
            {
                AllowedCollisionCount = int.MaxValue
            };
        }

        private static int QSpeed()
        {
            return (int)(1 / (1 / 0.5 * Player.AttackSpeedMod));
        }


        public Yasuo()
        {
            QEB = GetQ();
            Q3EB = new EloBuddy.SDK.Spell.Skillshot(SpellSlot.Q, 1100, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 300, 1200, 90)
            {
                AllowedCollisionCount = int.MaxValue
            };

            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 505).SetSkillshot(QDelay, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q2 = new LeagueSharp.SDK.Spell(Q.Slot, 1100).SetSkillshot(Q2Delay, 90, 1200, true, Q.Type);
            Q3 = new LeagueSharp.SDK.Spell(Q.Slot, 225).SetSkillshot(0.005f, 225, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 400).SetTargetted(0.25f, float.MaxValue);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 475).SetTargetted(0, 1200);
            E2 = new LeagueSharp.SDK.Spell(E.Slot, E.Range).SetTargetted(Q3.Delay, E.Speed);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 1200);
            Q.DamageType = Q2.DamageType = R.DamageType = DamageType.Physical;
            E.DamageType = DamageType.Magical;
            Q.MinHitChance = Q2.MinHitChance = HitChance.VeryHigh;
            E.CastCondition += () => !posDash.IsValid();

            if (YasuoPro.YasuoMenu.ComboM != null || YasuoSharpV2.YasuoSharp.comboMenu != null)
            {
                if (EntityManager.Heroes.Enemies.Any())
                {
                    Evade.Init();
                }
                Evade.Evading += Evading;
                Evade.TryEvading += TryEvading;
                return;
            }

            comboMenu = config.AddSubMenu("Combo", "Combo");
            comboMenu.AddGroupLabel("Q: Always On");
            comboMenu.AddGroupLabel("Smart Settings");
            comboMenu.Add("W", new CheckBox("Use W", false));
            comboMenu.Add("E", new CheckBox("Use E", false));
            comboMenu.AddGroupLabel("E Gap Settings");
            comboMenu.Add("EGap", new CheckBox("Use E"));
            comboMenu.Add("EMode", new ComboBox("Follow Mode", 0, "Enemy", "Mouse"));
            comboMenu.Add("ETower", new CheckBox("Under Tower", false));
            comboMenu.Add("EStackQ", new CheckBox("Stack Q While Gap", false));
            comboMenu.AddGroupLabel("R Settings");
            comboMenu.Add("R", new KeyBind("Use R", false, KeyBind.BindTypes.PressToggle, 'X'));
            comboMenu.Add("RDelay", new CheckBox("Delay Cast"));
            comboMenu.Add("RHpU", new Slider("If Enemies Hp < (%)", 60));
            comboMenu.Add("RCountA", new Slider("Or Count >=", 2, 1, 5));

            hybridMenu = config.AddSubMenu("Hybrid", "Hybrid");
            hybridMenu.AddGroupLabel("Q: Always On");
            hybridMenu.Add("Q3", new CheckBox("Also Q3"));
            hybridMenu.Add("QLastHit", new CheckBox("Last Hit (Q1/2)"));
            hybridMenu.AddGroupLabel("Auto Q Settings");
            hybridMenu.Add("AutoQ", new KeyBind("KeyBind", false, KeyBind.BindTypes.PressToggle, 'T'));
            hybridMenu.Add("AutoQ3", new CheckBox("Also Q3", false));

            lcMenu = config.AddSubMenu("LaneClear", "Lane Clear");
            lcMenu.AddGroupLabel("Q Settings");
            lcMenu.Add("Q", new CheckBox("Use Q"));
            lcMenu.Add("Q3", new CheckBox("Also Q3", false));
            lcMenu.AddGroupLabel("E Settings");
            lcMenu.Add("E", new CheckBox("Use E"));
            lcMenu.Add("ELastHit", new CheckBox("Last Hit Only", false));

            lhMenu = config.AddSubMenu("LastHit", "Last Hit");
            lhMenu.AddGroupLabel("Q Settings");
            lhMenu.Add("Q", new CheckBox("Use Q"));
            lhMenu.Add("Q3", new CheckBox("Also Q3", false));
            lhMenu.AddGroupLabel("E Settings");
            lhMenu.Add("E", new CheckBox("Use E"));

            ksMenu = config.AddSubMenu("KillSteal", "Kill Steal");
            ksMenu.Add("Q", new CheckBox("Use Q"));
            ksMenu.Add("E", new CheckBox("Use E"));
            ksMenu.Add("R", new CheckBox("Use R"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                ksMenu.Add("RCast" + enemy.NetworkId, new CheckBox("Cast On " + enemy.ChampionName, false));
            }

            fleeMenu = config.AddSubMenu("Flee", "Flee");
            fleeMenu.Add("E", new KeyBind("Use E", false, KeyBind.BindTypes.HoldActive, 'C'));
            fleeMenu.Add("Q", new CheckBox("Stack Q While Dash"));

            if (EntityManager.Heroes.Enemies.Any())
            {
                Evade.Init();
            }

            drawMenu = config.AddSubMenu("Draw", "Draw");
            drawMenu.Add("Q", new CheckBox("Q Range", false));
            drawMenu.Add("E", new CheckBox("E Range", false));
            drawMenu.Add("R", new CheckBox("R Range", false));
            drawMenu.Add("UseR", new CheckBox("R In Combo Status"));
            drawMenu.Add("StackQ", new CheckBox("Auto Stack Q Status"));

            miscMenu = config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("StackQ", new KeyBind("Auto Stack Q", false, KeyBind.BindTypes.PressToggle, 'Z'));
            miscMenu.Add("EQ3Flash", new KeyBind("Use E-Q3-Flash", false, KeyBind.BindTypes.HoldActive, 'X'));

            Evade.Evading += Evading;
            Evade.TryEvading += TryEvading;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += args =>
                {
                    if (Player.IsDead)
                    {
                        if (isDash)
                        {
                            isDash = false;
                            posDash = new Vector3();
                        }
                        return;
                    }
                    if (isDash && !Player.IsDashing())
                    {
                        isDash = false;
                        DelayAction.Add(
                            70,
                            () =>
                            {
                                if (!isDash)
                                {
                                    posDash = new Vector3();
                                }
                            });
                    }
                    Q.Delay = GetQDelay(false);
                    Q2.Delay = GetQDelay(true);
                    E.Speed = E2.Speed = 1200 + (Player.MoveSpeed - 345);
                };
            Orbwalker.OnPostAttack += (target, args) =>
                {
                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)
                        || !(target is Obj_AI_Turret) || !Q.IsReady() || haveQ3)
                    {
                        return;
                    }
                    if (Q.GetTarget(50) != null
                        || Common.ListMinions().Count(i => i.IsValidTarget(Q.Range + 50)) > 0)
                    {
                        return;
                    }
                    if ((Items.HasItem((int)ItemId.Sheen) && Items.CanUseItem((int)ItemId.Sheen))
                        || (Items.HasItem((int)ItemId.Trinity_Force)
                            && Items.CanUseItem((int)ItemId.Trinity_Force)))
                    {
                        Q.Cast(Game.CursorPos);
                    }
                };
            Orbwalker.OnPreAttack += (target, args) =>
            {
                args.Process = !IsDashing;
            };
            Events.OnDash += (sender, args) =>
                {
                    if (!args.Unit.IsMe)
                    {
                        return;
                    }
                    isDash = true;
                    posDash = args.EndPos.ToVector3();
                };
            Obj_AI_Base.OnBuffGain += (sender, args) =>
                {
                    if (!sender.IsMe)
                    {
                        return;
                    }
                    switch (args.Buff.DisplayName)
                    {
                        case "YasuoQ3W":
                            haveQ3 = true;
                            break;
                        case "YasuoDashScalar":
                            cDash = 1;
                            break;
                        case "yasuoeqcombosoundmiss":
                        case "YasuoEQComboSoundHit":
                            Orbwalker.DisableAttacking = (true);
                            DelayAction.Add(220, () => Orbwalker.DisableAttacking = (false));
                            break;
                    }
                };
            Obj_AI_Base.OnBuffUpdate += (sender, args) =>
            {
                if (!sender.IsMe || !args.Buff.Caster.IsMe || args.Buff.DisplayName != "YasuoDashScalar")
                {
                    return;
                }
                cDash = 2;
            };
            Obj_AI_Base.OnBuffLose += (sender, args) =>
                {
                    if (!sender.IsMe)
                    {
                        return;
                    }
                    switch (args.Buff.DisplayName)
                    {
                        case "YasuoQ3W":
                            haveQ3 = false;
                            break;
                        case "YasuoDashScalar":
                            cDash = 0;
                            break;
                    }
                };

            GameObjectNotifier<MissileClient>.OnCreate += (sender, args) =>
            {
                var spellCaster = args.SpellCaster as AIHeroClient;
                if (spellCaster == null || !spellCaster.IsMe)
                {
                    return;
                }
                switch (args.SData.Name)
                {
                    case "YasuoWMovingWallMisL":
                        wallLeft = args;
                        break;
                    case "YasuoWMovingWallMisR":
                        wallRight = args;
                        break;
                }
            };
            GameObjectNotifier<MissileClient>.OnDelete += (sender, args) =>
            {
                if (args.Compare(wallLeft))
                {
                    wallLeft = null;
                }
                else if (args.Compare(wallRight))
                {
                    wallRight = null;
                }
            };
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

        #endregion

        #region Properties

        private static bool CanCastQCir => posDash.IsValid() && posDash.DistanceToPlayer() < 150;

        private static List<Obj_AI_Base> GetQCirObj => Common.ListEnemies(true).Where(i => i.IsValidTarget() && Q3.WillHit(Q3.GetPredPosition(i), posDash)).ToList();

        private static List<Obj_AI_Base> GetQCirTarget => EntityManager.Heroes.Enemies.Where(i => Q3.WillHit(Q3.GetPredPosition(i), posDash) && Q3.IsInRange(i) && i.LSIsValidTarget()).Cast<Obj_AI_Base>().ToList();

        private static AIHeroClient GetRTarget
        {
            get
            {
                var result = new Tuple<AIHeroClient, List<AIHeroClient>>(null, new List<AIHeroClient>());
                foreach (var target in GetRTargets)
                {
                    var nears =
                        GameObjects.EnemyHeroes.Where(
                            i => !i.Compare(target) && i.IsValidTarget(RWidth, true, target.ServerPosition) && HaveR(i))
                            .ToList();
                    nears.Add(target);
                    if (nears.Count > result.Item2.Count
                        && ((nears.Count > 1
                             && nears.Any(i => i.Health + i.AttackShield <= R.GetDamage(i) + GetQDmg(i)))
                            || nears.Sum(i => i.HealthPercent) / nears.Count < getSliderItem(comboMenu, "RHpU")
                            || nears.Count >= getSliderItem(comboMenu, "RCountA")))
                    {
                        result = new Tuple<AIHeroClient, List<AIHeroClient>>(target, nears);
                    }
                }
                return getCheckBoxItem(comboMenu, "RDelay")
                       && (Player.HealthPercent >= 20
                           || GameObjects.EnemyHeroes.Count(i => i.IsValidTarget(600) && !HaveR(i)) == 0)
                           ? (result.Item2.Any(CanCastDelayR) ? result.Item1 : null)
                           : result.Item1;
            }
        }

        private static List<AIHeroClient> GetRTargets
            => EntityManager.Heroes.Enemies.Where(x => R.IsInRange(x) && HaveR(x) && x.LSIsValidTarget() && x.IsVisible).ToList();

        private static bool IsDashing => Variables.TickCount - lastE <= 70 || Player.IsDashing() || posDash.IsValid();

        private static LeagueSharp.SDK.Spell SpellQ => !haveQ3 ? Q : Q2;

        #endregion

        #region Methods

        private static void AutoQ()
        {
            if (!getKeyBindItem(hybridMenu, "AutoQ") || !Q.IsReady() || IsDashing || (haveQ3 && !getCheckBoxItem(hybridMenu, "AutoQ3")))
            {
                return;
            }
            if (!haveQ3)
            {
                Q.CastingBestTarget(true);
            }
            else
            {
                CastQ3();
            }
        }

        private static void BeyBlade()
        {
            if (!Common.CanFlash)
            {
                return;
            }
            if (Q.IsReady() && haveQ3 && IsDashing && CanCastQCir)
            {
                var hits =
                    GameObjects.EnemyHeroes.Count(
                        i => i.LSIsValidTarget() && Q3.GetPredPosition(i).Distance(posDash) < Q3.Range + FlashRange);
                if (hits > 0 && Q3.Cast(posDash))
                {
                    //DelayAction.Add();
                }
            }
            if (!E.IsReady() || IsDashing)
            {
                return;
            }
            var obj =
                Common.ListEnemies(true)
                    .Where(i => i.LSIsValidTarget(E.Range) && !HaveE(i))
                    .MaxOrDefault(
                        i =>
                        GameObjects.EnemyHeroes.Count(
                            a =>
                            !a.Compare(i) && a.LSIsValidTarget()
                            && Q3.GetPredPosition(a).Distance(GetPosAfterDash(i)) < Q3.Range + FlashRange - 50));
            if (obj != null && E.CastOnUnit(obj))
            {
                lastE = Variables.TickCount;
            }
        }

        private static bool CanCastDelayR(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Knockback))
            {
                return true;
            }
            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockup);
            return buff != null && Game.Time - buff.StartTime >= 0.9 * (buff.EndTime - buff.StartTime);
        }

        private static bool CanDash(
            Obj_AI_Base target,
            bool inQCir = false,
            bool underTower = true,
            Vector3 pos = new Vector3())
        {
            if (HaveE(target))
            {
                return false;
            }
            if (!pos.IsValid())
            {
                pos = E.GetPredPosition(target, true);
            }
            var posAfterE = GetPosAfterDash(target);
            return (underTower || !posAfterE.IsUnderEnemyTurret())
                   && (inQCir ? Q3.WillHit(pos, posAfterE) : posAfterE.Distance(pos) < pos.DistanceToPlayer())
                   && Evade.IsSafePoint(posAfterE.ToVector2()).IsSafe;
        }

        private static bool CastQ3()
        {
            var targets = EntityManager.Heroes.Enemies.Where(i => Q2.IsInRange(i) && i.LSIsValidTarget()).ToList();
            if (targets.Count == 0)
            {
                return false;
            }
            var posCast = new Vector3();
            foreach (var pred in
                targets.Select(i => Q2.GetPrediction(i, true, -1, CollisionableObjects.YasuoWall))
                    .Where(
                        i =>
                        i.Hitchance >= Q2.MinHitChance || (i.Hitchance >= HitChance.High && i.AoeTargetsHitCount > 1))
                    .OrderByDescending(i => i.AoeTargetsHitCount))
            {
                posCast = pred.CastPosition;
                break;
            }
            return posCast.IsValid() && Q2.Cast(posCast);
        }

        private static bool CastQCir(List<Obj_AI_Base> obj)
        {
            if (obj.Count == 0)
            {
                return false;
            }
            var target = obj.FirstOrDefault();
            return target != null && Q3.Cast(SpellQ.GetPredPosition(target, true));
        }

        private static void Combo()
        {
            if (R.IsReady() && getKeyBindItem(comboMenu, "R"))
            {
                var target = GetRTarget;
                if (target != null && R.CastOnUnit(target))
                {
                    return;
                }
            }

            if (getCheckBoxItem(comboMenu, "W") && W.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target != null && Math.Abs(target.GetProjectileSpeed() - float.MaxValue) > float.Epsilon
                    && (target.HealthPercent > Player.HealthPercent
                            ? Player.CountAllyHeroesInRange(500) < target.CountEnemyHeroesInRange(700)
                            : Player.HealthPercent <= 30))
                {
                    var posPred = W.GetPredPosition(target, true);
                    if (posPred.DistanceToPlayer() > 100 && posPred.DistanceToPlayer() < 330 && W.Cast(posPred))
                    {
                        return;
                    }
                }
            }
            if (getCheckBoxItem(comboMenu, "E") && E.IsReady() && wallLeft != null && wallRight != null)
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target != null && Math.Abs(target.GetProjectileSpeed() - float.MaxValue) > float.Epsilon
                    && !HaveE(target) && Evade.IsSafePoint(GetPosAfterDash(target).ToVector2()).IsSafe)
                {
                    var listPos =
                        Common.ListEnemies()
                            .Where(i => i.LSIsValidTarget(E.Range * 2) && !HaveE(i))
                            .Select(GetPosAfterDash)
                            .Where(
                                i =>
                                target.Distance(i) < target.DistanceToPlayer()
                                || target.Distance(i) < target.GetRealAutoAttackRange() + 100)
                            .ToList();
                    if (listPos.Any(i => IsThroughWall(target.ServerPosition, i)) && E.CastOnUnit(target))
                    {
                        lastE = Variables.TickCount;
                        return;
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "EGap") && E.IsReady())
            {
                var underTower = getCheckBoxItem(comboMenu, "ETower");

                if (getBoxItem(comboMenu, "EMode") == 0)
                {
                    var listDashObj = GetDashObj(underTower);

                    var target = E.GetTarget(Q3.Width);
                    if (target != null && haveQ3 && Q.IsReady(50))
                    {
                        var nearObj = GetBestObj(listDashObj, target, true);
                        if (nearObj != null && (GetPosAfterDash(nearObj).CountEnemyHeroesInRange(Q3.Width) > 1 || Player.CountEnemyHeroesInRange(Q.Range + E.Range / 2) == 1) && E.CastOnUnit(nearObj))
                        {
                            lastE = Variables.TickCount;
                            return;
                        }
                    }

                    target = E.GetTarget();
                    if (target != null && ((cDash > 0 && CanDash(target, false, underTower)) || (haveQ3 && Q.IsReady(50) && CanDash(target, true, underTower))) && E.CastOnUnit(target))
                    {
                        lastE = Variables.TickCount;
                        return;
                    }

                    target = Q.GetTarget(100) ?? Q2.GetTarget();

                    if (target != null && (!Player.Spellbook.IsAutoAttacking || Player.HealthPercent < 40))
                    {
                        var nearObj = GetBestObj(listDashObj, target);
                        var canDash = cDash == 0 && nearObj != null && !HaveE(target);
                        if (Q.IsReady(50))
                        {
                            var nearObjQ3 = GetBestObj(listDashObj, target, true);
                            if (nearObjQ3 != null)
                            {
                                nearObj = nearObjQ3;
                                canDash = true;
                            }
                        }
                        if (!canDash && target.DistanceToPlayer() > target.GetRealAutoAttackRange() * 0.7)
                        {
                            canDash = true;
                        }
                        if (canDash)
                        {
                            if (nearObj == null && E.IsInRange(target) && CanDash(target, false, underTower))
                            {
                                nearObj = target;
                            }
                            if (nearObj != null && E.CastOnUnit(nearObj))
                            {
                                lastE = Variables.TickCount;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    var target = Orbwalker.LastTarget;
                    if (target == null || Player.Distance(target) > target.GetRealAutoAttackRange() * 0.7
                        || Player.Distance(Game.CursorPos) > E.Range * 1.5)
                    {
                        var obj = GetBestObjToMouse(underTower);
                        if (obj != null && E.CastOnUnit(obj))
                        {
                            lastE = Variables.TickCount;
                            return;
                        }
                    }
                }
            }

            if (Q.IsReady())
            {
                if (IsDashing)
                {
                    if (CanCastQCir)
                    {
                        if (CastQCir(GetQCirTarget))
                        {
                            return;
                        }
                        if (!haveQ3 && getCheckBoxItem(comboMenu, "EGap") && getCheckBoxItem(comboMenu, "EStackQ") && Player.CountEnemyHeroesInRange(700) == 0 && CastQCir(GetQCirObj))
                        {
                            return;
                        }
                    }
                }
                else if (!haveQ3 ? Q.CastingBestTarget(true).IsCasted() : CastQ3())
                {
                    return;
                }
            }
        }

        private static void Evading(Obj_AI_Base sender)
        {
            var yasuoW = EvadeSpellDatabase.Spells.FirstOrDefault(i => i.Enable && i.IsReady && i.Slot == SpellSlot.W);
            if (yasuoW == null)
            {
                return;
            }
            var skillshot = Evade.SkillshotAboutToHit(sender, yasuoW.Delay - Evade.getSliderItem("Yasuo WDelay"), true).OrderByDescending(i => i.DangerLevel).FirstOrDefault(i => i.DangerLevel >= yasuoW.DangerLevel);
            if (skillshot != null)
            {
                W.Cast(sender.ServerPosition.LSExtend((Vector3)skillshot.Start, 100));

            }
        }

        private static void Flee()
        {
            if (getCheckBoxItem(fleeMenu, "Q") && Q.IsReady() && !haveQ3 && IsDashing && CanCastQCir && CastQCir(GetQCirObj))
            {
                return;
            }
            if (!E.IsReady())
            {
                return;
            }
            var obj = GetBestObjToMouse();
            if (obj != null && E.CastOnUnit(obj))
            {
                lastE = Variables.TickCount;
            }
        }

        private static Obj_AI_Base GetBestObj(List<Obj_AI_Base> obj, AIHeroClient target, bool inQCir = false)
        {
            obj.RemoveAll(i => i.Compare(target));
            if (obj.Count == 0)
            {
                return null;
            }
            var pos = E.GetPredPosition(target, true);
            return obj.Where(i => CanDash(i, inQCir, true, pos)).MinOrDefault(i => GetPosAfterDash(i).Distance(pos));
        }

        private static Obj_AI_Base GetBestObjToMouse(bool underTower = true)
        {
            var pos = Game.CursorPos;
            return
                GetDashObj(underTower)
                    .Where(i => CanDash(i, false, true, pos))
                    .MinOrDefault(i => GetPosAfterDash(i).Distance(pos));
        }

        private static List<Obj_AI_Base> GetDashObj(bool underTower = false)
        {
            return
                Common.ListEnemies()
                    .Where(i => i.LSIsValidTarget(E.Range) && (underTower || !GetPosAfterDash(i).IsUnderEnemyTurret()))
                    .ToList();
        }

        public static double GetEDmg(Obj_AI_Base target)
        {
            var stacksPassive = ObjectManager.Player.Buffs.Find(b => b.DisplayName.Equals("YasuoDashScalar"));
            var Estacks = (stacksPassive != null) ? stacksPassive.Count : 0;
            var damage = ((E.Level * 20) + 50) * (1 + 0.25 * Estacks) + (ObjectManager.Player.FlatMagicDamageMod * 0.6);
            return LeagueSharp.Common.Damage.CalcDamage(ObjectManager.Player, target, DamageType.Magical, damage);
        }

        private static Vector3 GetPosAfterDash(Obj_AI_Base target)
        {
            return Player.ServerPosition.LSExtend(target.ServerPosition, E.Range);
        }

        private static float GetQDelay(bool isQ3)
        {
            var delayOri = !isQ3 ? QDelay : Q2Delay;
            var delayMax = !isQ3 ? QDelays : Q2Delays;
            var perReduce = 1 - delayMax / delayOri;
            var delayReal =
                Math.Max(
                    delayOri * (1 - Math.Min((Player.AttackSpeedMod - 1) * (perReduce / 1.1f), perReduce)),
                    delayMax);
            return (float)Math.Round((decimal)delayReal, 3, MidpointRounding.AwayFromZero);
        }

        private static double GetQDmg(Obj_AI_Base target)
        {
            var dmgItem = 0d;
            if (Items.HasItem((int)ItemId.Sheen) && (Items.CanUseItem((int)ItemId.Sheen) || Player.HasBuff("Sheen")))
            {
                dmgItem = Player.BaseAttackDamage;
            }
            if (Items.HasItem((int)ItemId.Trinity_Force)
                && (Items.CanUseItem((int)ItemId.Trinity_Force) || Player.HasBuff("Sheen")))
            {
                dmgItem = Player.BaseAttackDamage * 2;
            }
            if (dmgItem > 0)
            {
                dmgItem = Player.CalculateDamage(target, DamageType.Physical, dmgItem);
            }
            double dmgQ = Q.GetDamage(target);
            if (Math.Abs(Player.Crit - 1) < float.Epsilon)
            {
                dmgQ += Player.CalculateDamage(
                    target,
                    Q.DamageType,
                    (Items.HasItem((int)ItemId.Infinity_Edge) ? 0.875 : 0.5) * Player.TotalAttackDamage);
            }
            return dmgQ + dmgItem;
        }

        private static bool HaveE(Obj_AI_Base target)
        {
            return target.HasBuff("YasuoDashWrapper");
        }

        private static bool HaveR(AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup);
        }

        private static void Hybrid()
        {
            if (!Q.IsReady() || IsDashing)
            {
                return;
            }
            if (!haveQ3)
            {
                foreach (Obj_AI_Base minion in LeagueSharp.Common.MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q3.Range, LeagueSharp.Common.MinionTypes.All, LeagueSharp.Common.MinionTeam.Enemy).OrderByDescending(m => m.Health))
                {
                    if (minion != null)
                    {
                        if (!minion.IsDead && minion != null && getCheckBoxItem(hybridMenu, "QLastHit") && Q.IsReady() && minion.LSIsValidTarget(500) && !haveQ3 && Q.IsInRange(minion) && !IsDashing)
                        {
                            var predHealth = LeagueSharp.Common.HealthPrediction.GetHealthPrediction(minion, (int)(ObjectManager.Player.Distance(minion.Position) * 1000 / 2000));
                            if (predHealth <= GetQDmg(minion))
                            {
                                Q.Cast(minion);
                            }
                        }
                    }
                }
            }
            else if (getCheckBoxItem(hybridMenu, "Q3"))
            {
                CastQ3();
            }
        }

        private static bool IsInRangeQ(Obj_AI_Minion minion)
        {
            return minion.LSIsValidTarget(Math.Max(465 + minion.BoundingRadius / 3, 475));
        }

        private static bool IsThroughWall(Vector3 from, Vector3 to)
        {
            if (wallLeft == null || wallRight == null)
            {
                return false;
            }
            wallPoly = new RectanglePoly(wallLeft.Position, wallRight.Position, 75);
            for (var i = 0; i < wallPoly.Points.Count; i++)
            {
                var inter = wallPoly.Points[i].LSIntersection(
                    wallPoly.Points[i != wallPoly.Points.Count - 1 ? i + 1 : 0],
                    from.ToVector2(),
                    to.ToVector2());
                if (inter.Intersects)
                {
                    return true;
                }
            }
            return false;
        }

        private static void KillSteal()
        {
            if (getCheckBoxItem(ksMenu, "Q") && Q.IsReady())
            {
                if (IsDashing)
                {
                    if (CanCastQCir)
                    {
                        var targets = GetQCirTarget.Where(i => i.Health + i.AttackShield <= GetQDmg(i)).ToList();
                        if (CastQCir(targets))
                        {
                            return;
                        }
                    }
                }
                else
                {
                    var target = SpellQ.GetTarget(SpellQ.Width / 2);
                    if (target != null && target.Health + target.AttackShield <= GetQDmg(target))
                    {
                        if (!haveQ3)
                        {
                            if (Q.Casting(target).IsCasted())
                            {
                                return;
                            }
                        }
                        else if (Q2.Casting(target, false, CollisionableObjects.YasuoWall).IsCasted())
                        {
                            return;
                        }
                    }
                }
            }
            if (getCheckBoxItem(ksMenu, "E") && E.IsReady())
            {
                var targets = EntityManager.Heroes.Enemies.Where(i => !HaveE(i) && E.IsInRange(i)).ToList();
                if (targets.Count > 0)
                {
                    var target = targets.FirstOrDefault(i => i.Health + i.MagicShield <= GetEDmg(i));
                    if (target != null)
                    {
                        if (E.CastOnUnit(target))
                        {
                            lastE = Variables.TickCount;
                            return;
                        }
                    }
                    else if (getCheckBoxItem(ksMenu, "Q") && Q.IsReady(50))
                    {
                        target =
                            targets.Where(i => i.Distance(GetPosAfterDash(i)) < Q3.Width)
                            .FirstOrDefault(
                                 i =>
                                     i.Health - Math.Max(GetEDmg(i) - i.MagicShield, 0) + i.AttackShield
                                    <= GetQDmg(i));
                        if (target != null && E.CastOnUnit(target))
                        {
                            lastE = Variables.TickCount;
                            return;
                        }
                    }
                }
            }
            if (getCheckBoxItem(ksMenu, "R") && R.IsReady())
            {
                var target =
                    GetRTargets.Where(
                        i =>
                        getCheckBoxItem(ksMenu, "RCast" + i.NetworkId)
                        && i.Health + i.AttackShield <= R.GetDamage(i) + (Q.IsReady(1000) ? GetQDmg(i) : 0))
                        .MaxOrDefault(i => new Priority().GetDefaultPriority(i));
                if (target != null)
                {
                    R.CastOnUnit(target);
                }
            }
        }

        public static bool CanCastE(Obj_AI_Base target)
        {
            return !target.HasBuff("YasuoDashWrapper");
        }

        public static bool isDangerous(Obj_AI_Base target, float range)
        {
            return LeagueSharp.Common.HeroManager.Enemies.Where(tar => tar.Distance(GetPosAfterDash(target)) < range).Any(tar => tar != null);
        }

        public static Vector2 PosAfterE(Obj_AI_Base target)
        {
            return ObjectManager.Player.ServerPosition.LSExtend(target.ServerPosition, ObjectManager.Player.Distance(target) < 410 ? E.Range : ObjectManager.Player.Distance(target) + 65).To2D();
        }

        private static void LaneClear()
        {
            if (haveQ3 && getCheckBoxItem(lcMenu, "Q3"))
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(i => Q2.IsInRange(i)).Cast<Obj_AI_Base>().ToList();
                if (minions != null)
                {
                    Q2.Cast(minions.FirstOrDefault());
                }
            }

            var allMinionsE = LeagueSharp.Common.MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, LeagueSharp.Common.MinionTypes.All, LeagueSharp.Common.MinionTeam.Enemy);
            foreach (var minion in allMinionsE.Where(x => x.LSIsValidTarget(E.Range) && CanCastE(x)))
            {
                if (minion != null)
                {
                    if (getCheckBoxItem(lcMenu, "E") && E.IsReady() && minion.LSIsValidTarget(E.Range) && CanCastE(minion))
                    {
                        if (!UnderTower(PosAfterE(minion)))
                        {
                            if (minion.Health <= GetEDmg(minion) && !isDangerous(minion, 600))
                            {
                                E.CastOnUnit(minion);
                            }
                        }
                    }
                }
            }

            if (getCheckBoxItem(lcMenu, "Q") && Q.IsReady() && (!haveQ3 || getCheckBoxItem(lcMenu, "Q3")))
            {
                if (IsDashing)
                {
                    if (CanCastQCir)
                    {
                        var minions = GetQCirObj.Where(i => i is Obj_AI_Minion).ToList();
                        if (minions.Any(i => i.Health <= GetQDmg(i) || i.Team == GameObjectTeam.Neutral) || minions.Count > 1)
                        {
                            CastQCir(minions);
                        }
                    }
                }
                else
                {
                    var minions = Common.ListMinions().Where(i => !haveQ3 ? IsInRangeQ(i) : i.LSIsValidTarget(Q2.Range - i.BoundingRadius / 2)).OrderByDescending(i => i.MaxHealth).ToList();
                    if (minions.Count == 0)
                    {
                        return;
                    }
                    if (!haveQ3)
                    {
                        var minion = minions.FirstOrDefault(i => Q.CanLastHit(i, GetQDmg(i)));
                        if (minion != null)
                        {
                            Q.Casting(minion);
                        }
                        else
                        {
                            var pos = Q.GetLineFarmLocation(minions);
                            if (pos.MinionsHit > 0)
                            {
                                Q.Cast(pos.Position);
                            }
                        }
                    }
                    else
                    {
                        var pos = Q2.GetLineFarmLocation(minions);
                        if (pos.MinionsHit > 0)
                        {
                            Q2.Cast(pos.Position);
                        }
                    }
                }
            }
        }

        public static bool UnderTower(Vector2 pos)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(i => i.Health > 0 && i.Distance(pos) <= 950 && i.IsEnemy);
        }

        public static int GetQRange()
        {
            return HasQ3() ? 1100 : 475;
        }

        public static bool HasQ3()
        {
            return Player.HasBuff("yasuoq3w");
        }

        public static int ECount()
        {
            // ReSharper disable once StringLiteralTypo
            var count = Player.GetBuffCount("yasuodashscalar");
            return count > 0 ? count : 0;
        }

        public static float GetDamage(Obj_AI_Base target, SpellSlot slot)
        {
            var dmg = 0f;

            var level = Player.Spellbook.GetSpell(slot).Level - 1;

            if (!Player.Spellbook.GetSpell(slot).IsReady || target == null) return 0f;

            switch (slot)
            {
                case SpellSlot.Q:
                    dmg += new[] { 20f, 40f, 60f, 80f, 100f }[level] + Player.TotalAttackDamage;
                    break;
                case SpellSlot.E:
                    var bonusDamage = new[] { 17.5f, 22.5f, 27.5f, 32.5f, 37.5f }[level] * ECount();

                    dmg += new[] { 70f, 90f, 110f, 130f, 150f }[level] + Player.FlatMagicDamageMod * 0.6f + bonusDamage;
                    break;
                case SpellSlot.R:
                    dmg += new[] { 200f, 300f, 400f }[level] + Player.FlatPhysicalDamageMod * 1.5f;
                    break;
            }

            return dmg - 10f;
        }

        public static void CastQ(Obj_AI_Base target)
        {
            var QEB = GetQ();
            if (!Q.IsReady() || target == null || target.IsInvulnerable) return;

            if (HasQ3())
            {
                var pred = Q3EB.GetPrediction(target);
                if (pred.HitChancePercent >= 80)
                {
                    Q3.Cast(target);
                }
            }
            else
            {
                var pred = QEB.GetPrediction(target);
                if (pred.HitChancePercent >= 80)
                {
                    Q.Cast(target);
                }
            }
        }

        private static void LastHit()
        {

            foreach (Obj_AI_Base minion in LeagueSharp.Common.MinionManager.GetMinions(ObjectManager.Player.ServerPosition, QEB.Range, LeagueSharp.Common.MinionTypes.All, LeagueSharp.Common.MinionTeam.Enemy).OrderByDescending(m => m.Health))
            {
                if (minion != null)
                {
                    if (QEB.IsReady() && GetDamage(minion, SpellSlot.Q) > minion.Health && getCheckBoxItem(lhMenu, "Q"))
                    {
                        QEB.Cast(minion);
                    }

                    if (getCheckBoxItem(lhMenu, "E") && E.IsReady() && minion.LSIsValidTarget(475))
                    {
                        if (!UnderTower(PosAfterE(minion)) && CanCastE(minion))
                        {
                            if (minion.Health <= GetEDmg(minion) && !isDangerous(minion, 600))
                            {
                                E.CastOnUnit(minion);
                            }
                        }
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (getCheckBoxItem(drawMenu, "Q") && Q.Level > 0)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    (IsDashing ? Q3 : (!haveQ3 ? Q : Q2)).Range,
                    Q.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (getCheckBoxItem(drawMenu, "E") && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (R.Level > 0)
            {
                if (getCheckBoxItem(drawMenu, "R") && R.IsReady())
                {
                    Render.Circle.DrawCircle(
                        Player.Position,
                        R.Range,
                        GetRTargets.Count > 0 ? Color.LimeGreen : Color.IndianRed);
                }
                if (getCheckBoxItem(drawMenu, "UseR"))
                {
                    var menuR = getKeyBindItem(comboMenu, "R");
                    var pos = Drawing.WorldToScreen(Player.Position);
                    var text = $"Use R In Combo: {(menuR ? "On" : "Off")}";
                    Drawing.DrawText(pos.X - (float)50 / 2, pos.Y + 40, menuR ? Color.White : Color.Gray, text);
                }
            }
            if (getCheckBoxItem(drawMenu, "StackQ") && Q.Level > 0)
            {
                var menu = getKeyBindItem(miscMenu, "StackQ");
                var text =
                    $"Auto Stack Q: {(menu ? (haveQ3 ? "Full" : (Q.IsReady() ? "Ready" : "Not Ready")) : "Off")}";
                var pos = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(
                    pos.X - (float)50 / 2,
                    pos.Y + 20,
                    menu ? Color.White : Color.Gray,
                    text);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsRecalling())
            {
                return;
            }

            KillSteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Hybrid();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee) || getKeyBindItem(fleeMenu, "E"))
            {
                Orbwalker.MoveTo(Game.CursorPos);
                Flee();
            }
            else if (miscMenu["EQ3Flash"].Cast<KeyBind>().CurrentValue)
            {
                Orbwalker.MoveTo(Game.CursorPos);
                //BeyBlade();
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                AutoQ();
            }
            if (getKeyBindItem(miscMenu, "StackQ") && !getKeyBindItem(fleeMenu, "E"))
            {
                StackQ();
            }
        }

        private static void StackQ()
        {
            if (!Q.IsReady() || haveQ3 || IsDashing)
            {
                return;
            }
            var state = Q.CastingBestTarget(true);
            if (state.IsCasted() || state != CastStates.InvalidTarget)
            {
                return;
            }
            var minions =
                Common.ListMinions().Where(IsInRangeQ).OrderByDescending(i => i.MaxHealth).ToList();
            if (minions.Count == 0)
            {
                return;
            }
            var minion = minions.FirstOrDefault(i => Q.CanLastHit(i, GetQDmg(i))) ?? minions.FirstOrDefault();
            if (minion == null)
            {
                return;
            }
            Q.Casting(minion);
        }

        private static void TryEvading(List<Skillshot> hitBy, Vector2 to)
        {
            var dangerLevel = hitBy.Select(i => i.DangerLevel).Concat(new[] { 0 }).Max();
            var yasuoE =
                EvadeSpellDatabase.Spells.FirstOrDefault(
                    i => i.Enable && dangerLevel >= i.DangerLevel && i.IsReady && i.Slot == SpellSlot.E);
            if (yasuoE == null)
            {
                return;
            }
            yasuoE.Speed = (int)E.Speed;
            var target =
                yasuoE.GetEvadeTargets(false, true)
                    .OrderBy(i => GetPosAfterDash(i).CountEnemyHeroesInRange(400))
                    .ThenBy(i => GetPosAfterDash(i).Distance(to))
                    .FirstOrDefault();
            if (target != null && E.CastOnUnit(target))
            {
                lastE = Variables.TickCount;
            }
        }
        #endregion
    }
}