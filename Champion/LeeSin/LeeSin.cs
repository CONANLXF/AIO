namespace Valvrave_Sharp.Plugin
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;
    using EloBuddy.SDK.Menu;
    using SharpDX;
    using EloBuddy.SDK.Menu.Values;
    using Valvrave_Sharp.Core;

    using Color = System.Drawing.Color;

    #endregion
    using EloBuddy;
    using LeagueSharp.SDK.Modes;
    using EloBuddy.SDK;
    using LeagueSharp.Data.Enumerations;
    using EloBuddy.SDK.Enumerations;
    using LeagueSharp.SDK.Enumerations;
    using TargetSelector = PortAIO.TSManager;

    internal class LeeSin : Program
    {
        #region Constants

        private const int RKickRange = 750;

        #endregion

        #region Static Fields

        private static readonly List<string> SpecialPet = new List<string>
                                                              { "jarvanivstandard", "teemomushroom", "illaoiminion" };

        private static int cPassive;

        private static bool isDashing;

        private static int lastBubbaKush;

        private static int lastW, lastW2, lastE2, lastR;

        private static Obj_AI_Base objQ;

        private static Vector3 posBubbaKushFlash, posBubbaKushJump;

        private static EloBuddy.SDK.Menu.Menu config = Program._MainMenu;

        #endregion

        public static Menu miscMenu, drawMenu, ksMenu, lhMenu, lcMenu, comboMenu, kuMenu, insecMenu, bkMenu;

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

        #region Constructors and Destructors

        public static LeagueSharp.Common.Spell QS;

        public LeeSin()
        {
            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 1100).SetSkillshot(0.25f, 60, 1800, true, SkillshotType.SkillshotLine);
            QS = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100);
            QS.SetSkillshot(0.275f, 60f, 1850f, true, LeagueSharp.Common.SkillshotType.SkillshotLine);
            Q2 = new LeagueSharp.SDK.Spell(Q.Slot, 1300);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 700);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 425).SetTargetted(0.25f, float.MaxValue);
            E2 = new LeagueSharp.SDK.Spell(E.Slot, 570);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 375).SetTargetted(0.25f, float.MaxValue);
            R2 = new LeagueSharp.SDK.Spell(R.Slot, RKickRange).SetSkillshot(R.Delay, 0, 900, false, SkillshotType.SkillshotLine);
            Q.DamageType = Q2.DamageType = W.DamageType = R.DamageType = DamageType.Physical;
            E.DamageType = DamageType.Magical;
            Q.MinHitChance = R2.MinHitChance = LeagueSharp.SDK.Enumerations.HitChance.VeryHigh;

            WardManager.Init();
            Insec.Init();

            kuMenu = config.AddSubMenu("Auto Knock Up");
            kuMenu.Add("R", new KeyBind("Keybind (R-Flash)", false, KeyBind.BindTypes.PressToggle, 'L'));
            kuMenu.Add("RKill", new CheckBox("Priority To Kill Enemy Behind"));
            kuMenu.Add("RCountA", new Slider("Or Hit Enemy Behind >=", 1, 1, 4));

            bkMenu = config.AddSubMenu("Bubba Kush", "BubbaKush");
            bkMenu.Add("R", new KeyBind("Keybind (R-Flash)", false, KeyBind.BindTypes.HoldActive, 'X'));
            bkMenu.Add("RMode", new ComboBox("Mode", 0, new[] { "Flash", "WardJump", "Both" }));
            bkMenu.Add("RKill", new CheckBox("Priority To Kill Enemy"));
            bkMenu.Add("RCountA", new Slider("Or Hit Enemy >=", 1, 1, 4));

            comboMenu = config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("Ignite", new CheckBox("Use Ignite"));
            comboMenu.Add("Item", new CheckBox("Use Item"));
            comboMenu.AddGroupLabel("Q Settings");
            comboMenu.Add("Q", new CheckBox("Use Q"));
            comboMenu.Add("Q2", new CheckBox("Also Q2"));
            comboMenu.Add("Q2Obj", new CheckBox("Q2 Even Miss", false));
            comboMenu.Add("QCol", new CheckBox("Smite Collision"));
            comboMenu.AddGroupLabel("W Settings");
            comboMenu.Add("W", new CheckBox("Use W", false));
            comboMenu.Add("W2", new CheckBox("Also W2", false));
            comboMenu.AddGroupLabel("E Settings");
            comboMenu.Add("E", new CheckBox("Use E"));
            comboMenu.Add("E2", new CheckBox("Also E2"));
            comboMenu.AddGroupLabel("Star Combo Settings");
            comboMenu.Add("Star", new KeyBind("Star Combo", false, KeyBind.BindTypes.HoldActive, 'X'));
            comboMenu.Add("StarKill", new CheckBox("Auto Star Combo If Killable", false));
            comboMenu.Add("StarKillWJ", new CheckBox("-> Ward Jump In Auto Star Combo", false));

            lcMenu = config.AddSubMenu("LaneClear", "Lane Clear");
            lcMenu.Add("W", new CheckBox("Use W", false));
            lcMenu.Add("E", new CheckBox("Use E"));
            lcMenu.AddGroupLabel("Q Settings");
            lcMenu.Add("Q", new CheckBox("Use Q"));
            lcMenu.Add("QBig", new CheckBox("Only Q Big Mob In Jungle"));

            lhMenu = config.AddSubMenu("LastHit", "Last Hit");
            lhMenu.Add("Q", new CheckBox("Use Q1"));

            ksMenu = config.AddSubMenu("KillSteal", "Kill Steal");
            ksMenu.Add("E", new CheckBox("Use E"));
            ksMenu.Add("R", new CheckBox("Use R"));
            ksMenu.AddGroupLabel("Q Settings");
            ksMenu.Add("Q", new CheckBox("Use Q"));
            ksMenu.Add("Q2", new CheckBox("Also Q2"));
            ksMenu.AddGroupLabel("Extra R Settings");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                ksMenu.Add("RCast" + enemy.NetworkId, new CheckBox("Cast On " + enemy.ChampionName, false));
            }

            drawMenu = config.AddSubMenu("Draw", "Draw");
            drawMenu.Add("Q", new CheckBox("Q Range", false));
            drawMenu.Add("W", new CheckBox("W Range", false));
            drawMenu.Add("E", new CheckBox("E Range", false));
            drawMenu.Add("R", new CheckBox("R Range", false));
            drawMenu.Add("KnockUp", new CheckBox("Auto Knock Up Status"));

            miscMenu = config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("FleeW", new KeyBind("Use W To Flee", false, KeyBind.BindTypes.HoldActive, 'C'));
            miscMenu.Add("RFlash", new KeyBind("R-Flash To Mouse", false, KeyBind.BindTypes.HoldActive, 'Z'));

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;

            Obj_AI_Base.OnBuffGain += (sender, args) =>
                {
                    if (sender.IsMe)
                    {
                        switch (args.Buff.DisplayName)
                        {
                            case "BlindMonkFlurry":
                                cPassive = 2;
                                break;
                            case "BlindMonkQTwoDash":
                                isDashing = true;
                                break;
                        }
                    }
                    else if (sender.IsEnemy)
                    {
                        if (args.Buff.DisplayName == "BlindMonkSonicWave")
                        {
                            objQ = sender;
                        }
                        else if (args.Buff.Name == "blindmonkrroot" && Common.CanFlash)
                        {
                            CastRFlash(sender);
                        }
                    }
                };
            Obj_AI_Base.OnBuffLose += (sender, args) =>
            {
                if (sender.IsMe)
                {
                    switch (args.Buff.DisplayName)
                    {
                        case "BlindMonkFlurry":
                            cPassive = 0;
                            break;
                        case "BlindMonkQTwoDash":
                            isDashing = false;
                            break;
                    }
                }
                else if (sender.IsEnemy && args.Buff.DisplayName == "BlindMonkSonicWave")
                {
                    objQ = null;
                }
            };
            Obj_AI_Base.OnBuffUpdate += (sender, args) =>
            {
                if (!sender.IsMe || args.Buff.DisplayName != "BlindMonkFlurry")
                {
                    return;
                }
                cPassive = args.Buff.Count;
            };
            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                {
                    if (!sender.IsMe)
                    {
                        return;
                    }
                    if (args.Slot == SpellSlot.R)
                    {
                        lastR = Variables.TickCount;
                    }
                    else if (args.SData.Name == "SummonerFlash" && posBubbaKushFlash.IsValid())
                    {
                        posBubbaKushFlash = new Vector3();
                    }
                };
        }

        #endregion

        #region Properties

        private static bool IsDashing => (lastW > 0 && Variables.TickCount - lastW <= 100) || Player.IsDashing();

        private static bool IsEOne => E.Instance.SData.Name.ToLower().Contains("one");

        private static bool IsQOne => Q.Instance.SData.Name.ToLower().Contains("one");

        private static bool IsRecentR => Variables.TickCount - lastR < 2500;

        private static bool IsWOne => W.Instance.SData.Name.ToLower().Contains("one");

        #endregion

        #region Methods

        private static void AutoKnockUp()
        {
            if (!R.IsReady())
            {
                return;
            }
            var multi = GetMultiHit(getCheckBoxItem(kuMenu, "RKill"), getSliderItem(kuMenu, "RCountA"), 0);
            if (multi.Item1 != null && multi.Item2 != 0 && !multi.Item3.IsValid())
            {
                R.CastOnUnit(multi.Item1);
            }
        }

        private static void BubbaKush()
        {
            if (!R.IsReady())
            {
                return;
            }
            var multi = GetMultiHit(getCheckBoxItem(bkMenu, "RKill"), getSliderItem(bkMenu, "RCountA"), 0);
            if (multi.Item1 != null && multi.Item2 != 0 && !multi.Item3.IsValid() && R.CastOnUnit(multi.Item1))
            {
                return;
            }
            if (Variables.TickCount - lastBubbaKush <= 1500)
            {
                if (posBubbaKushJump.IsValid() && posBubbaKushJump.DistanceToPlayer() < 100)
                {
                    var targetSelect = TargetSelector.GetSelectedTarget();
                    if (targetSelect.IsValidTarget())
                    {
                        R.CastOnUnit(targetSelect);
                    }
                }
                return;
            }
            posBubbaKushFlash = posBubbaKushJump = new Vector3();
            TargetSelector.SetTarget(null);
            var mode = getBoxItem(bkMenu, "RMode");
            var multiW = WardManager.CanWardJump && mode != 0
                             ? GetMultiHit(getCheckBoxItem(bkMenu, "RKill"), getSliderItem(bkMenu, "RCountA"), 2)
                             : new Tuple<AIHeroClient, int, Vector3>(null, 0, new Vector3());
            var multiF = Common.CanFlash && mode != 1
                             ? GetMultiHit(getCheckBoxItem(bkMenu, "RKill"), getSliderItem(bkMenu, "RCountA"), 1)
                             : new Tuple<AIHeroClient, int, Vector3>(null, 0, new Vector3());
            if (multiW.Item1 != null && multiW.Item2 != 0 && multiW.Item3.IsValid())
            {
                posBubbaKushJump = multiW.Item3;
                lastBubbaKush = Variables.TickCount;
                TargetSelector.SetTarget(multiW.Item1);
                WardManager.Place(posBubbaKushJump);
            }
            else if (multiF.Item1 != null && multiF.Item2 != 0 && multiF.Item3.IsValid() && R.CastOnUnit(multiF.Item1))
            {
                posBubbaKushFlash = multiF.Item3;
                lastBubbaKush = Variables.TickCount;
                TargetSelector.SetTarget(multiF.Item1);
            }
        }

        private static bool CanE2(Obj_AI_Base target)
        {
            var buff = target.GetBuff("BlindMonkTempest");
            return buff != null && buff.EndTime - Game.Time < 0.25 * (buff.EndTime - buff.StartTime);
        }

        private static bool CanQ2(Obj_AI_Base target)
        {
            var buff = target.GetBuff("BlindMonkSonicWave");
            return buff != null && buff.EndTime - Game.Time < 0.25 * (buff.EndTime - buff.StartTime);
        }

        private static bool CanR(AIHeroClient target)
        {
            var buff = target.GetBuff("BlindMonkDragonsRage");
            return buff != null && buff.EndTime - Game.Time <= 0.75 * (buff.EndTime - buff.StartTime);
        }

        private static void CastE(List<Obj_AI_Minion> minions = null)
        {
            if (!E.IsReady() || isDashing || IsDashing || Variables.TickCount - lastW <= 250
                || Variables.TickCount - lastW2 <= 150)
            {
                return;
            }
            if (minions == null)
            {
                CastECombo();
            }
            else
            {
                CastELaneClear(minions);
            }
        }

        private static void CastECombo()
        {
            if (IsEOne)
            {
                var target = EntityManager.Heroes.Enemies.Where(x => !x.IsDead && Player.IsInRange(x, E.Range + 20) && x.LSIsValidTarget()).Where(x => E.CanHitCircle(x)).ToList();
                if (target.Count == 0)
                {
                    return;
                }
                if ((cPassive == 0 && Player.Mana >= 70) || target.Count > 2
                    || (PortAIO.OrbwalkerManager.LastTarget() == null
                            ? target.Any(i => i.DistanceToPlayer() > Player.GetRealAutoAttackRange() + 100)
                            : cPassive < 2))
                {
                    E.Cast();
                }
            }
            else if (getCheckBoxItem(comboMenu, "E2"))
            {
                var target = GameObjects.EnemyHeroes.Where(i => i.LSIsValidTarget(E2.Range) && HaveE(i)).ToList();
                if (target.Count == 0)
                {
                    return;
                }
                if ((cPassive == 0 || target.Count > 2
                     || target.Any(i => CanE2(i) || i.DistanceToPlayer() > i.GetRealAutoAttackRange() + 50))
                    && E2.Cast())
                {
                    lastE2 = Variables.TickCount;
                }
            }
        }

        private static void CastELaneClear(List<Obj_AI_Minion> minions)
        {
            if (IsEOne)
            {
                if (cPassive > 0)
                {
                    return;
                }
                var count = minions.Count(i => i.LSIsValidTarget(E.Range));
                if (count > 0 && (Player.Mana >= 70 || count > 2))
                {
                    E.Cast();
                }
            }
            else
            {
                var minion = minions.Where(i => i.LSIsValidTarget(E2.Range) && HaveE(i)).ToList();
                if (minion.Count > 0 && (cPassive == 0 || minion.Any(CanE2)) && E2.Cast())
                {
                    lastE2 = Variables.TickCount;
                }
            }
        }

        private static void CastRFlash(Obj_AI_Base target)
        {
            var targetSelect = TargetSelector.SelectedTarget;
            if (!targetSelect.LSIsValidTarget() || !targetSelect.Compare(target)
                || target.Health + target.AttackShield <= R.GetDamage(target))
            {
                return;
            }
            var pos = new Vector3();
            if (getKeyBindItem(miscMenu, "RFlash"))
            {
                pos = Game.CursorPos;
            }
            else if (getKeyBindItem(bkMenu, "R") && posBubbaKushFlash.IsValid())
            {
                pos = posBubbaKushFlash;
            }
            else if (getKeyBindItem(insecMenu, "R") && Insec.IsRecentRFlash)
            {
                pos = Insec.GetPositionKickTo((AIHeroClient)target);
            }
            if (pos.IsValid())
            {
                Player.Spellbook.CastSpell(
                    Flash,
                    target.ServerPosition.LSExtend(pos, -(Player.BoundingRadius / 2 + target.BoundingRadius + 50)));
            }
        }

        private static void CastW(List<Obj_AI_Minion> minions = null)
        {
            if (!W.IsReady() || Variables.TickCount - lastW <= 300 || isDashing || IsDashing
                             || Variables.TickCount - lastE2 <= 250)
            {
                return;
            }
            var hero = PortAIO.OrbwalkerManager.LastTarget() as AIHeroClient;
            Obj_AI_Minion minion = null;
            if (minions != null && minions.Count > 0)
            {
                minion = minions.FirstOrDefault(i => i.InAutoAttackRange());
            }
            if (hero == null && minion == null)
            {
                return;
            }
            if (hero != null && !IsWOne && !getCheckBoxItem(comboMenu, "W2"))
            {
                return;
            }
            if (hero != null && Player.HealthPercent < hero.HealthPercent && Player.HealthPercent < 30)
            {
                if (IsWOne)
                {
                    if (W.Cast())
                    {
                        lastW = Variables.TickCount;
                        return;
                    }
                }
                else if (W.Cast())
                {
                    lastW2 = Variables.TickCount;
                    return;
                }
            }
            if (Player.HealthPercent < (minions == null ? 8 : 5) || (!IsWOne && Variables.TickCount - lastW > 2600)
                || cPassive == 0
                || (minion != null && minion.Team == GameObjectTeam.Neutral
                    && minion.GetJungleType() != JungleType.Small && Player.HealthPercent < 40 && IsWOne))
            {
                if (IsWOne)
                {
                    if (W.Cast())
                    {
                        lastW = Variables.TickCount;
                    }
                }
                else if (W.Cast())
                {
                    lastW2 = Variables.TickCount;
                }
            }
        }

        private static void Combo()
        {
            if (getCheckBoxItem(comboMenu, "StarKill"))
            {
                if (R.IsReady() && Q.IsReady() && !IsQOne && getCheckBoxItem(comboMenu, "Q") && getCheckBoxItem(comboMenu, "Q2"))
                {
                    var target = EntityManager.Heroes.Enemies.Where(x => Q2.IsInRange(x) && HaveQ(x)).FirstOrDefault();
                    if (target != null && target.Health + target.AttackShield > Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target) && target.Health + target.AttackShield <= GetQ2Dmg(target, R.GetDamage(target)) + Player.GetAutoAttackDamage(target))
                    {
                        if (R.CastOnUnit(target))
                        {
                            return;
                        }
                        if (getCheckBoxItem(comboMenu, "StarKillWJ") && !R.IsInRange(target) && target.DistanceToPlayer() < WardManager.WardRange + R.Range - 50 && Player.Mana >= 80 && !isDashing)
                        {
                            Flee(target.ServerPosition, true);
                        }
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "Q") && Q.IsReady())
            {
                if (IsQOne)
                {
                    var target = Q.GetTarget(Q.Width / 2);
                    if (!R.IsReady() && Variables.TickCount - lastR < 5000)
                    {
                        var targetR = EntityManager.Heroes.Enemies.Where(x => Q.IsInRange(x) && x.LSIsValidTarget() && x.HasBuff("BlindMonkDragonsRage")).FirstOrDefault();
                        if (targetR != null)
                        {
                            target = targetR;
                        }
                    }
                    if (target != null)
                    {
                        Q.CastSpellSmite(target, getCheckBoxItem(comboMenu, "QCol"));
                    }
                }
                else if (getCheckBoxItem(comboMenu, "Q2") && !IsDashing && objQ.LSIsValidTarget(Q2.Range))
                {
                    var target = objQ as AIHeroClient;
                    if (target != null)
                    {
                        if (CanQ2(target) || (!R.IsReady() && IsRecentR && CanR(target)) || (target.Health + target.AttackShield <= Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target)) || ((R.IsReady() || (!target.HasBuff("BlindMonkDragonsRage") && Variables.TickCount - lastR > 1000)) && target.DistanceToPlayer() > target.GetRealAutoAttackRange() + 100) || cPassive == 0)
                        {
                            Q2.Cast();
                            isDashing = true;
                            return;
                        }
                    }
                    else if (getCheckBoxItem(comboMenu, "Q2Obj"))
                    {
                        var targetQ2 = Q2.GetTarget(200);
                        if (targetQ2 != null && objQ.Distance(targetQ2) < targetQ2.DistanceToPlayer() && !targetQ2.InAutoAttackRange())
                        {
                            Q2.Cast();
                            isDashing = true;
                            return;
                        }
                    }
                }
            }
            if (getCheckBoxItem(comboMenu, "E"))
            {
                CastE();
            }
            if (getCheckBoxItem(comboMenu, "W"))
            {
                CastW();
            }
            var subTarget = W.GetTarget();
            if (getCheckBoxItem(comboMenu, "Item"))
            {
                UseItem(subTarget);
            }
            if (subTarget != null && getCheckBoxItem(comboMenu, "Ignite") && Common.CanIgnite && subTarget.HealthPercent < 30 && subTarget.DistanceToPlayer() <= IgniteRange)
            {
                Player.Spellbook.CastSpell(Ignite, subTarget);
            }
        }

        private static void Flee(Vector3 pos, bool isStar = false)
        {
            if (!W.IsReady() || !IsWOne || Variables.TickCount - lastW <= 500)
            {
                return;
            }
            var posPlayer = Player.ServerPosition;
            var posJump = pos.Distance(posPlayer) < W.Range ? pos : posPlayer.LSExtend(pos, W.Range);
            var objJumps = new List<Obj_AI_Base>();
            objJumps.AddRange(GameObjects.AllyHeroes.Where(i => !i.IsMe));
            objJumps.AddRange(GameObjects.AllyWards.Where(i => i.IsWard()));
            objJumps.AddRange(
                GameObjects.AllyMinions.Where(
                    i => i.IsMinion() || i.IsPet() || SpecialPet.Contains(i.CharData.BaseSkinName.ToLower())));
            var objJump =
                objJumps.Where(
                    i => i.IsValidTarget(W.Range, false) && i.Distance(posJump) < (isStar ? R.Range - 50 : 200))
                    .MinOrDefault(i => i.Distance(posJump));
            if (objJump != null)
            {
                if (W.CastOnUnit(objJump))
                {
                    lastW = Variables.TickCount;
                }
            }
            else
            {
                WardManager.Place(posJump);
            }
        }

        private static Tuple<AIHeroClient, int, Vector3> GetMultiHit(bool checkKill, int minHit, int mode)
        {
            var bestHit = 0;
            AIHeroClient bestTarget = null;
            var bestPos = new Vector3();
            var targetKicks =
                GameObjects.EnemyHeroes
                    //TargetSelector(R.Range + (mode == 2 ? 500 : 0), R.DamageType)
                    .Where(
                        i =>
                        i.LSIsValidTarget(R.Range + (mode == 2 ? 500 : 0)) &&
                        (mode != 2
                         || i.DistanceToPlayer() < R.Range + (WardManager.WardRange - Insec.GetDistance(i) - 80))
                        && i.Health + i.AttackShield > R.GetDamage(i) && !i.HasBuffOfType(BuffType.SpellShield)
                        && !i.HasBuffOfType(BuffType.SpellImmunity))
                    .OrderByDescending(i => i.AllShield)
                    .ToList();
            foreach (var targetKick in targetKicks)
            {
                var posTarget = mode == 1 ? targetKick.ServerPosition : R.GetPredPosition(targetKick, true);
                R2.UpdateSourcePosition(posTarget, posTarget);
                R2.Width = targetKick.BoundingRadius;
                var targetHits =
                    GameObjects.EnemyHeroes.Where(
                        i => !i.Compare(targetKick) && i.IsValidTarget(R2.Range + R2.Width / 2, true, R2.From))
                        .OrderByDescending(i => new Priority().GetDefaultPriority(i))
                        .ToList();
                var posEnd = new Vector3();
                if (mode == 0)
                {
                    var pos = R2.From.LSExtend(Player.ServerPosition, -R2.Range);
                    targetHits = targetHits.Where(i => R2.WillHit(i, pos, 0, R2.MinHitChance)).ToList();
                    if (checkKill && targetHits.Any(i => i.Health + i.AttackShield <= GetRColDmg(targetKick, i)))
                    {
                        return new Tuple<AIHeroClient, int, Vector3>(targetKick, -1, posEnd);
                    }
                }
                else
                {
                    R2.Delay = R.Delay + (mode == 2 ? 0.05f : 0);
                    var hits = new List<AIHeroClient>();
                    foreach (var targetHit in targetHits)
                    {
                        var list = new List<AIHeroClient>();
                        var pred = R2.GetPrediction(targetHit);
                        var pos = new Vector3();
                        if (pred.Hitchance >= R2.MinHitChance)
                        {
                            list.Add(targetHit);
                            list.AddRange(
                                targetHits.Where(
                                    i => !i.Compare(targetHit) && R2.WillHit(i, pred.CastPosition, 0, R2.MinHitChance)));
                            pos = mode == 1
                                      ? pred.CastPosition
                                      : pred.Input.From.LSExtend(pred.CastPosition, -Insec.GetDistance(targetKick));
                        }
                        if (!pos.IsValid())
                        {
                            continue;
                        }
                        if (checkKill && list.Any(i => i.Health + i.AttackShield <= GetRColDmg(targetKick, i)))
                        {
                            return new Tuple<AIHeroClient, int, Vector3>(targetKick, -1, pos);
                        }
                        if (list.Count > hits.Count)
                        {
                            hits = list;
                            posEnd = pos;
                        }
                    }
                    targetHits = hits;
                }
                if (targetHits.Count > bestHit)
                {
                    bestTarget = targetKick;
                    bestHit = targetHits.Count;
                    bestPos = posEnd;
                }
            }
            return new Tuple<AIHeroClient, int, Vector3>(bestTarget, bestHit >= minHit ? 1 : 0, bestPos);
        }

        private static double GetQ2Dmg(Obj_AI_Base target, double subHp)
        {
            var dmg = new[] { 50, 80, 110, 140, 170 }[Q.Level - 1] + 0.9 * Player.FlatPhysicalDamageMod
                      + 0.08 * (target.MaxHealth - (target.Health - subHp));
            return Player.CalculateDamage(
                target,
                DamageType.Physical,
                target is Obj_AI_Minion ? Math.Min(dmg, 400) : dmg) + subHp;
        }

        private static float GetRColDmg(AIHeroClient kickTarget, AIHeroClient hitTarget)
        {
            return R.GetDamage(hitTarget)
                   + (float)
                     Player.CalculateDamage(
                         hitTarget,
                         DamageType.Physical,
                         new[] { 0.12, 0.15, 0.18 }[R.Level - 1] * kickTarget.AllShield);
        }

        private static bool HaveE(Obj_AI_Base target)
        {
            return target.HasBuff("BlindMonkTempest");
        }

        private static bool HaveQ(Obj_AI_Base target)
        {
            return target.HasBuff("BlindMonkSonicWave");
        }

        private static void KillSteal()
        {
            if (getCheckBoxItem(ksMenu, "Q") && Q.IsReady())
            {
                if (IsQOne)
                {
                    var target = Q.GetTarget(Q.Width / 2);
                    var predS = QS.GetPrediction(target);
                    if (target != null
                        && (target.Health + target.AttackShield <= Q.GetDamage(target)
                            || (getCheckBoxItem(ksMenu, "Q2")
                            && target.Health + target.AttackShield
                                <= GetQ2Dmg(target, Q.GetDamage(target)) + Player.GetAutoAttackDamage(target)
                                && Player.Mana - Q.Instance.SData.Mana >= 30))
                        && Q.Cast(predS.CastPosition))
                    {
                        return;
                    }
                }
                else if (getCheckBoxItem(ksMenu, "Q2") && !IsDashing)
                {
                    var target = objQ as AIHeroClient;
                    if (target != null
                        && target.Health + target.AttackShield
                        <= Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target) && Q2.Cast())
                    {
                        isDashing = true;
                        return;
                    }
                }
            }
            if (getCheckBoxItem(ksMenu, "E") && E.IsReady() && IsEOne && EntityManager.Heroes.Enemies.Where(x => !x.IsDead && E.IsInRange(x) && E.CanHitCircle(x) && x.Health + x.MagicShield <= E.GetDamage(x) && x.LSIsValidTarget()).Any() && E.Cast())
            {
                return;
            }
            if (getCheckBoxItem(ksMenu, "R") && R.IsReady())
            {
                var targetList = EntityManager.Heroes.Enemies.Where(x => !x.IsDead && R.IsInRange(x) && getCheckBoxItem(ksMenu, "RCast" + x.NetworkId) && x.LSIsValidTarget()).ToList();
                if (targetList.Count > 0)
                {
                    var targetR = targetList.FirstOrDefault(i => i.Health + i.AttackShield <= R.GetDamage(i));
                    if (targetR != null)
                    {
                        R.CastOnUnit(targetR);
                    }
                    else if (getCheckBoxItem(ksMenu, "Q") && getCheckBoxItem(ksMenu, "Q2") && Q.IsReady() && !IsQOne)
                    {
                        var targetQ2R =
                            targetList.FirstOrDefault(
                                i =>
                                HaveQ(i)
                                && i.Health + i.AttackShield
                                <= GetQ2Dmg(i, R.GetDamage(i)) + Player.GetAutoAttackDamage(i));
                        if (targetQ2R != null)
                        {
                            R.CastOnUnit(targetQ2R);
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var minions =
                Common.ListMinions().Where(i => i.LSIsValidTarget(Q2.Range)).OrderByDescending(i => i.MaxHealth).ToList();
            if (minions.Count == 0)
            {
                return;
            }
            if (getCheckBoxItem(lcMenu, "E"))
            {
                CastE(minions);
            }
            if (getCheckBoxItem(lcMenu, "W"))
            {
                CastW(minions);
            }
            if (getCheckBoxItem(lcMenu, "Q") && Q.IsReady())
            {
                if (IsQOne)
                {
                    if (cPassive < 2)
                    {
                        var minionQ = minions.Where(i => i.DistanceToPlayer() < Q.Range - 10).ToList();
                        if (minionQ.Count > 0)
                        {
                            var minionJungle =
                                minionQ.Where(i => i.Team == GameObjectTeam.Neutral)
                                    .OrderByDescending(i => i.MaxHealth)
                                    .ThenBy(i => i.DistanceToPlayer())
                                    .ToList();
                            if (getCheckBoxItem(lcMenu, "QBig") && minionJungle.Count > 0 && Player.Health > 100)
                            {
                                minionJungle =
                                    minionJungle.Where(
                                        i =>
                                        i.GetJungleType() == JungleType.Legendary
                                        || i.GetJungleType() == JungleType.Large || i.Name.Contains("Crab")).ToList();
                            }
                            if (minionJungle.Count > 0)
                            {
                                minionJungle.ForEach(i => Q.Casting(i));
                            }
                            else
                            {
                                var minionLane =
                                    minionQ.Where(i => i.Team != GameObjectTeam.Neutral)
                                        .OrderByDescending(i => i.GetMinionType().HasFlag(MinionTypes.Siege))
                                        .ThenBy(i => i.GetMinionType().HasFlag(MinionTypes.Super))
                                        .ThenBy(i => i.Health)
                                        .ThenByDescending(i => i.MaxHealth)
                                        .ToList();
                                if (minionLane.Count == 0)
                                {
                                    return;
                                }
                                foreach (var minion in minionLane)
                                {
                                    if (minion.InAutoAttackRange())
                                    {
                                        if (Q.GetHealthPrediction(minion) > Q.GetDamage(minion)
                                            && Q.Casting(minion).IsCasted())
                                        {
                                            return;
                                        }
                                    }
                                    else if ((PortAIO.OrbwalkerManager.LastTarget() != null
                                                  ? Q.CanLastHit(minion, Q.GetDamage(minion))
                                                  : Q.GetHealthPrediction(minion) > Q.GetDamage(minion))
                                             && Q.Casting(minion).IsCasted())
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!IsDashing)
                {
                    var q2Minion = objQ;
                    if (q2Minion.LSIsValidTarget(Q2.Range)
                        && (CanQ2(q2Minion) || q2Minion.Health <= Q.GetDamage(q2Minion, DamageStage.SecondCast)
                            || q2Minion.DistanceToPlayer() > q2Minion.GetRealAutoAttackRange() + 100 || cPassive == 0)
                        && Q2.Cast())
                    {
                        isDashing = true;
                    }
                }
            }
        }

        private static void LastHit()
        {
            if (!getCheckBoxItem(lhMenu, "Q") || !Q.IsReady() || !IsQOne || Player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            var minions =
                GameObjects.EnemyMinions.Where(
                    i => (i.IsMinion() || i.IsPet(false)) && i.LSIsValidTarget(Q.Range) && Q.CanLastHit(i, Q.GetDamage(i)))
                    .OrderByDescending(i => i.MaxHealth)
                    .ToList();
            if (minions.Count == 0)
            {
                return;
            }
            minions.ForEach(
                i =>
                Q.Casting(
                    i,
                    false,
                    CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall));
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (Q == null || W == null || E == null || R == null)
            {
                return;
            }
            if (getCheckBoxItem(drawMenu, "Q") && Q != null)
            {
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, (IsQOne ? Q : Q2).Range, Q.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (getCheckBoxItem(drawMenu, "W") && W.Level > 0 && IsWOne)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (getCheckBoxItem(drawMenu, "E") && E.Level > 0)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    (IsEOne ? E : E2).Range, E.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (R.Level > 0)
            {
                if (getCheckBoxItem(drawMenu, "R"))
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.LimeGreen : Color.IndianRed);
                }
                if (getCheckBoxItem(drawMenu, "KnockUp"))
                {
                    var menu = getKeyBindItem(kuMenu, "R");
                    var text = $"Auto Knock Up: {(menu ? "On" : "Off")} <{getSliderItem(kuMenu, "RCountA")}> [{kuMenu["R"].Cast<KeyBind>().Keys.Item1}]";
                    var pos = Drawing.WorldToScreen(Player.Position);
                    Drawing.DrawText(pos.X - (float)90 / 2, pos.Y + 20, menu ? Color.White : Color.Gray, text);
                }
            }
        }

        

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.LSIsRecalling() || Shop.IsOpen)
            {
                return;
            }
            KillSteal();

            PortAIO.OrbwalkerManager.SetAttack(!getKeyBindItem(insecMenu, "R"));

            if (!PortAIO.OrbwalkerManager.isNoneActive)
            {
                PortAIO.OrbwalkerManager.ForcedTarget(null);
            }

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo();
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                LaneClear();
            }

            if (PortAIO.OrbwalkerManager.isLastHitActive)
            {
                LastHit();
            }

            if (PortAIO.OrbwalkerManager.isNoneActive)
            {
                if (getKeyBindItem(miscMenu, "FleeW"))
                {
                    PortAIO.OrbwalkerManager.MoveA(Game.CursorPos);
                    Flee(Game.CursorPos);
                }

                else if (getKeyBindItem(miscMenu, "RFlash"))
                {
                    PortAIO.OrbwalkerManager.MoveA(Game.CursorPos);
                    if (R.IsReady() && Common.CanFlash)
                    {
                        var target = EntityManager.Heroes.Enemies.Where(i => i.Health + i.AttackShield > R.GetDamage(i) && R.IsInRange(i)).FirstOrDefault();
                        if (target != null && R.CastOnUnit(target))
                        {
                            PortAIO.OrbwalkerManager.ForcedTarget(target);
                        }
                    }
                }
                else if (getKeyBindItem(bkMenu, "R"))
                {
                    PortAIO.OrbwalkerManager.MoveA(Game.CursorPos);
                    BubbaKush();
                }
                else if (getKeyBindItem(comboMenu, "Star"))
                {
                    StarCombo();
                }
                else if (getKeyBindItem(insecMenu, "R"))
                {
                    Insec.Start(Insec.GetTarget);
                }
            }


            if (getKeyBindItem(kuMenu, "R") && !getKeyBindItem(bkMenu, "R") && !getKeyBindItem(insecMenu, "R"))
            {
                AutoKnockUp();
            }
        }

        private static void StarCombo()
        {
            var target = Q.GetTarget(Q.Width / 2);
            if (!IsQOne)
            {
                target = objQ as AIHeroClient;
            }
            if (!Q.IsReady())
            {
                target = W.GetTarget();
            }
            PortAIO.OrbwalkerManager.MoveA(Game.CursorPos);
            if (target == null)
            {
                return;
            }
            if (Q.IsReady())
            {
                if (IsQOne)
                {
                    Q.CastSpellSmite(target, false);
                }
                else if (!IsDashing && HaveQ(target)
                         && (target.Health + target.AttackShield
                             <= Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target)
                             || (!R.IsReady() && IsRecentR && CanR(target))) && Q2.Cast())
                {
                    isDashing = true;
                    return;
                }
            }
            if (E.IsReady() && IsEOne && E.CanHitCircle(target) && (!HaveQ(target) || Player.Mana >= 70) && E.Cast())
            {
                return;
            }
            if (!R.IsReady() || !Q.IsReady() || IsQOne || !HaveQ(target))
            {
                return;
            }
            if (R.IsInRange(target))
            {
                R.CastOnUnit(target);
            }
            else if (target.DistanceToPlayer() < WardManager.WardRange + R.Range - 50 && Player.Mana >= 70 && !isDashing)
            {
                Flee(target.ServerPosition, true);
            }
        }

        private static void UseItem(AIHeroClient target)
        {
            if (target != null && (target.HealthPercent < 40 || Player.HealthPercent < 50))
            {
                if (Bilgewater.IsReady())
                {
                    Bilgewater.Cast(target);
                }
                if (BotRuinedKing.IsReady())
                {
                    BotRuinedKing.Cast(target);
                }
            }
            if (Youmuu.IsReady() && Player.CountEnemyHeroesInRange(W.Range + E.Range) > 0)
            {
                Youmuu.Cast();
            }
            if (Tiamat.IsReady() && Player.CountEnemyHeroesInRange(Tiamat.Range) > 0)
            {
                Tiamat.Cast();
            }
            if (Hydra.IsReady() && Player.CountEnemyHeroesInRange(Hydra.Range) > 0)
            {
                Hydra.Cast();
            }
            if (Titanic.IsReady() && !Player.Spellbook.IsAutoAttacking && PortAIO.OrbwalkerManager.LastTarget() != null)
            {
                Titanic.Cast();
            }
        }

        #endregion

        private static class Insec
        {

            #region Static Fields

            internal static bool IsWardFlash;

            private static Vector3 lastEndPos, lastFlashPos;

            private static int lastInsecTime, lastMoveTime, lastRFlashTime, lastFlashRTime;

            private static Obj_AI_Base lastObjQ;

            #endregion

            #region Properties

            internal static AIHeroClient GetTarget
            {
                get
                {
                    AIHeroClient target = null;
                    if (getCheckBoxItem(insecMenu, "TargetSelect"))
                    {
                        var sub = TargetSelector.SelectedTarget;
                        if (sub.LSIsValidTarget())
                        {
                            target = sub;
                        }
                    }
                    else
                    {
                        target = Q.GetTarget(-100);
                        if ((getCheckBoxItem(insecMenu, "Q") && Q.IsReady()) || objQ.LSIsValidTarget(Q2.Range))
                        {
                            target = Q2.GetTarget(CanWardFlash ? GetRange(null, true) : FlashRange);
                        }
                    }
                    return target;
                }
            }

            internal static float GetDistance(AIHeroClient target)
            {
                return Math.Min((Player.BoundingRadius + target.BoundingRadius + 50) * 1.4f, 300);
            }

            internal static bool IsRecentRFlash => Variables.TickCount - lastRFlashTime < 5000;

            private static bool CanInsec
                =>
                    (WardManager.CanWardJump || (getCheckBoxItem(insecMenu, "Flash") && Common.CanFlash) || IsRecent)
                    && R.IsReady();

            private static bool CanWardFlash
                =>
                    getCheckBoxItem(insecMenu, "Flash") && getCheckBoxItem(insecMenu, "FlashJump") && WardManager.CanWardJump
                    && Common.CanFlash;

            private static bool IsRecent
                => IsRecentWardJump || (getCheckBoxItem(insecMenu, "Flash") && Variables.TickCount - lastFlashRTime < 5000);


            private static bool IsRecentWardJump
                =>
                    Variables.TickCount - WardManager.LastInsecWardTime < 5000
                    || Variables.TickCount - WardManager.LastInsecJumpTme < 5000;

            #endregion

            #region Methods

            internal static Vector3 GetPositionKickTo(AIHeroClient target)
            {
                if (lastEndPos.IsValid())
                {
                    return lastEndPos;
                }
                var pos = Player.ServerPosition;
                switch (getBoxItem(insecMenu, "Mode"))
                {
                    case 0:
                        var turret =
                            GameObjects.AllyTurrets.Where(
                                i =>
                                target.Distance(i) <= 1400 && i.Distance(target) - RKickRange <= 950
                                && i.Distance(target) > 225).MinOrDefault(i => i.DistanceToPlayer());
                        if (turret != null)
                        {
                            pos = turret.ServerPosition;
                        }
                        else
                        {
                            var hero =
                                GameObjects.AllyHeroes.Where(
                                    i =>
                                    i.IsValidTarget(RKickRange + 700, false, target.ServerPosition) && !i.IsMe
                                    && i.HealthPercent > 10 && i.Distance(target) > 325)
                                    .MaxOrDefault(i => i.CountAllyHeroesInRange(600));
                            if (hero != null)
                            {
                                pos = hero.ServerPosition;
                            }
                        }
                        break;
                    case 1:
                        pos = Game.CursorPos;
                        break;
                }
                return pos;
            }

            internal static void Init()
            {
                insecMenu = config.AddSubMenu("Insec", "Insec");
                insecMenu.Add("TargetSelect", new CheckBox("Only Insec Target Selected", false));
                insecMenu.Add("Mode", new ComboBox("Mode", 0, "Tower/Hero/Current", "Mouse Position", "Current Position"));

                insecMenu.AddGroupLabel("Draw Settings");
                insecMenu.Add("DLine", new CheckBox("Line"));
                insecMenu.Add("DWardFlash", new CheckBox("WardJump Flash Range"));

                insecMenu.AddGroupLabel("Flash Settings");
                insecMenu.Add("Flash", new CheckBox("Use Flash"));
                insecMenu.Add("FlashMode", new ComboBox("Flash Mode", 0, "R-Flash", "Flash-R", "Both"));
                insecMenu.Add("FlashJump", new CheckBox("Use WardJump To Gap For Flash"));

                insecMenu.AddGroupLabel("Q Settings");
                insecMenu.Add("Q", new CheckBox("Use Q"));
                insecMenu.Add("QCol", new CheckBox("Smite Collision"));
                insecMenu.Add("QObj", new CheckBox("Use Q On Near Object"));

                insecMenu.AddGroupLabel("Keybinds");
                insecMenu.Add("R", new KeyBind("Insec", false, KeyBind.BindTypes.HoldActive, 'T'));

                Game.OnUpdate += args =>
                    {
                        if (lastInsecTime > 0 && Variables.TickCount - lastInsecTime > 5000)
                        {
                            CleanData();
                        }
                        if (lastMoveTime > 0 && Variables.TickCount - lastMoveTime > 1000 && !R.IsReady())
                        {
                            lastMoveTime = 0;
                        }
                    };
                Drawing.OnDraw += args =>
                    {
                        if (Player.IsDead || R.Level == 0 || !CanInsec)
                        {
                            return;
                        }
                        if (getCheckBoxItem(insecMenu, "DLine"))
                        {
                            var target = GetTarget;
                            if (target != null)
                            {
                                var posTarget = target.Position;
                                var posEnd = GetPositionKickTo(target);
                                var radius = target.BoundingRadius * 1.35f;
                                Render.Circle.DrawCircle(posTarget, radius, Color.BlueViolet);
                                Render.Circle.DrawCircle(
                                    GetPositionBehind(target, posEnd, posTarget),
                                    radius,
                                    Color.BlueViolet);
                                Drawing.DrawLine(
                                    Drawing.WorldToScreen(posTarget),
                                    Drawing.WorldToScreen(posEnd),
                                    1,
                                    Color.BlueViolet);
                            }
                        }
                        if (getCheckBoxItem(insecMenu, "DWardFlash") && CanWardFlash)
                        {
                            Render.Circle.DrawCircle(Player.Position, GetRange(null, true), Color.Orange);
                        }
                    };
                Obj_AI_Base.OnBuffGain += (sender, args) =>
                    {
                        if (!sender.IsEnemy || args.Buff.DisplayName != "BlindMonkSonicWave")
                        {
                            return;
                        }
                        lastObjQ = sender;
                    };
                Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                    {
                        if (!lastFlashPos.IsValid() || !sender.IsMe
                            || !getKeyBindItem(insecMenu, "R")
                            || args.SData.Name != "SummonerFlash" || !getCheckBoxItem(insecMenu, "Flash")
                            || Variables.TickCount - lastFlashRTime > 1250 || args.End.Distance(lastFlashPos) > 80)
                        {
                            return;
                        }

                        lastFlashRTime = Variables.TickCount;
                        var target = TargetSelector.SelectedTarget;
                        if (target.LSIsValidTarget())
                        {
                            DelayAction.Add(5, () => R.CastOnUnit(target));
                        }
                    };
                Obj_AI_Base.OnSpellCast += (sender, args) =>
                    {
                        if (!sender.IsMe || args.Slot != SpellSlot.R)
                        {
                            return;
                        }
                        CleanData();
                    };
            }

            internal static void Start(AIHeroClient target)
            {
                if (PortAIO.OrbwalkerManager.CanMove(0) && Variables.TickCount - lastMoveTime > 250)
                {
                    var posMove = Game.CursorPos;
                    if (target != null && lastMoveTime > 0 && CanInsec)
                    {
                        var posEnd = GetPositionKickTo(target);
                        if (posEnd.DistanceToPlayer() > target.Distance(posEnd))
                        {
                            posMove = GetPositionBehind(target, posEnd);
                        }
                    }
                    PortAIO.OrbwalkerManager.MoveA(posMove);
                }
                if (target == null || !CanInsec)
                {
                    return;
                }
                if (!IsRecent)
                {
                    if (!IsWardFlash)
                    {
                        var checkJump = GapCheck(target);
                        if (checkJump.Item2)
                        {
                            GapByWardJump(target, checkJump.Item1);
                        }
                        else
                        {
                            var checkFlash = GapCheck(target, true);
                            if (checkFlash.Item2)
                            {
                                GapByFlash(target, checkFlash.Item1);
                            }
                            else if (CanWardFlash)
                            {
                                var posTarget = target.ServerPosition;
                                if (posTarget.DistanceToPlayer() < GetRange(target, true)
                                    && (!isDashing
                                        || (!lastObjQ.Compare(target) && lastObjQ.Distance(posTarget) > GetRange(target))))
                                {
                                    IsWardFlash = true;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        PortAIO.OrbwalkerManager.ForcedTarget(target);
                        WardManager.Place(target.ServerPosition);
                        return;
                    }
                }
                if (R.IsInRange(target))
                {
                    var posEnd = GetPositionKickTo(target);
                    var posTarget = target.ServerPosition;
                    var posPlayer = Player.ServerPosition;
                    if (posPlayer.Distance(posEnd) > posTarget.Distance(posEnd))
                    {
                        var segment = posTarget.LSExtend(posPlayer, -RKickRange)
                            .ProjectOn(posTarget, posEnd.LSExtend(posTarget, -(RKickRange * 0.5f)));
                        if (segment.IsOnSegment && segment.SegmentPoint.Distance(posEnd) <= RKickRange * 0.5f
                            && R.CastOnUnit(target))
                        {
                            return;
                        }
                    }
                }
                GapByQ(target);
            }

            private static void CleanData()
            {
                lastEndPos = lastFlashPos = new Vector3();
                lastInsecTime = 0;
                IsWardFlash = false;
                PortAIO.OrbwalkerManager.ForcedTarget(null);
            }

            private static void GapByFlash(AIHeroClient target, Vector3 posBehind)
            {
                switch (getBoxItem(insecMenu, "FlashMode"))
                {
                    case 0:
                        GapByRFlash(target);
                        break;
                    case 1:
                        GapByFlashR(target, posBehind);
                        break;
                    case 2:
                        if (!posBehind.IsValid())
                        {
                            GapByRFlash(target);
                        }
                        else
                        {
                            GapByFlashR(target, posBehind);
                        }
                        break;
                }
            }

            private static void GapByFlashR(AIHeroClient target, Vector3 posBehind)
            {
                if (PortAIO.OrbwalkerManager.CanMove(0))
                {
                    lastMoveTime = Variables.TickCount;
                    PortAIO.OrbwalkerManager.MoveA(
                        posBehind.LSExtend(GetPositionKickTo(target), -(GetDistance(target) + Player.BoundingRadius / 2)));
                }
                lastFlashPos = posBehind;
                lastEndPos = GetPositionAfterKick(target);
                lastInsecTime = lastFlashRTime = Variables.TickCount;
                PortAIO.OrbwalkerManager.ForcedTarget(target);
                Player.Spellbook.CastSpell(Flash, posBehind);
            }

            private static void GapByQ(AIHeroClient target)
            {
                if (!getCheckBoxItem(insecMenu, "Q") || !Q.IsReady() || IsDashing)
                {
                    return;
                }
                if (CanWardFlash && (IsWardFlash || (IsQOne && Player.Mana < 50 + 80)))
                {
                    return;
                }
                var minDist = GetRange(target, CanWardFlash);
                if (IsQOne)
                {
                    Q.CastSpellSmite(target, getCheckBoxItem(insecMenu, "QCol"));
                    if (!getCheckBoxItem(insecMenu, "QObj"))
                    {
                        return;
                    }
                    var nearObj =
                        Common.ListEnemies(true)
                            .Where(
                                i =>
                                !i.Compare(target) && i.IsValidTarget(Q.Range)
                                && Q.GetHealthPrediction(i) > Q.GetDamage(i)
                                && i.Distance(target) < target.DistanceToPlayer() && i.Distance(target) < minDist - 80)
                            .OrderBy(i => i.Distance(target))
                            .ThenByDescending(i => i.Health)
                            .ToList();
                    if (nearObj.Count == 0)
                    {
                        return;
                    }
                    nearObj.ForEach(i => Q.Casting(i));
                }
                else if (target.DistanceToPlayer() > minDist
                         && (HaveQ(target) || (objQ.IsValidTarget(Q2.Range) && target.Distance(objQ) < minDist - 80))
                         && ((WardManager.CanWardJump && Player.Mana >= 80)
                             || (getCheckBoxItem(insecMenu, "Flash") && Common.CanFlash)) && Q2.Cast())
                {
                    isDashing = true;
                    PortAIO.OrbwalkerManager.ForcedTarget(target);
                }
            }

            private static void GapByRFlash(AIHeroClient target)
            {
                if (!R.CastOnUnit(target))
                {
                    return;
                }
                lastEndPos = GetPositionAfterKick(target);
                lastInsecTime = lastRFlashTime = Variables.TickCount;
                PortAIO.OrbwalkerManager.ForcedTarget(target);
            }

            private static void GapByWardJump(AIHeroClient target, Vector3 posBehind)
            {
                if (PortAIO.OrbwalkerManager.CanMove(0))
                {
                    lastMoveTime = Variables.TickCount;
                }
                lastEndPos = GetPositionAfterKick(target);
                lastInsecTime = WardManager.LastInsecWardTime = WardManager.LastInsecJumpTme = Variables.TickCount;
                PortAIO.OrbwalkerManager.ForcedTarget(target);
                WardManager.Place(posBehind, 1);
            }

            private static Tuple<Vector3, bool> GapCheck(AIHeroClient target, bool useFlash = false)
            {
                if (!useFlash ? !WardManager.CanWardJump : !getCheckBoxItem(insecMenu, "Flash") || !Common.CanFlash)
                {
                    return new Tuple<Vector3, bool>(new Vector3(), false);
                }
                var posEnd = GetPositionKickTo(target);
                var posPlayer = Player.ServerPosition;
                var posTarget = target.ServerPosition;
                if (!useFlash)
                {
                    var posBehind = posTarget.LSExtend(posEnd, -GetDistance(target));
                    if (posBehind.Distance(posPlayer) < WardManager.WardRange
                        && posTarget.Distance(posBehind) < posEnd.Distance(posBehind))
                    {
                        return new Tuple<Vector3, bool>(posBehind, true);
                    }
                }
                else
                {
                    var flashMode = getBoxItem(insecMenu, "FlashMode");
                    if (flashMode != 1 && posTarget.Distance(posPlayer) < R.Range)
                    {
                        return new Tuple<Vector3, bool>(new Vector3(), true);
                    }
                    if (flashMode != 0)
                    {
                        var posBehind = posTarget.LSExtend(posEnd, -GetDistance(target));
                        var posFlash = posPlayer.LSExtend(posBehind, FlashRange);
                        if (posBehind.Distance(posPlayer) < FlashRange
                            && posTarget.Distance(posBehind) < posEnd.Distance(posBehind)
                            && posFlash.Distance(posTarget) > 50
                            && posFlash.Distance(posTarget) < posFlash.Distance(posEnd))
                        {
                            return new Tuple<Vector3, bool>(posBehind, true);
                        }
                    }
                }
                return new Tuple<Vector3, bool>(new Vector3(), false);
            }

            private static Vector3 GetPositionAfterKick(AIHeroClient target)
            {
                return target.ServerPosition.LSExtend(GetPositionKickTo(target), RKickRange);
            }

            private static Vector3 GetPositionBehind(AIHeroClient target, Vector3 to, Vector3 from = default(Vector3))
            {
                return (from.IsValid() ? from : target.ServerPosition).LSExtend(to, -GetDistance(target));
            }

            private static float GetRange(AIHeroClient target, bool isWardFlash = false)
            {
                return !isWardFlash
                           ? (WardManager.CanWardJump ? WardManager.WardRange : FlashRange) - GetDistance(target)
                           : WardManager.WardRange + R.Range - ((target ?? Player).BoundingRadius + 20);
            }

            #endregion
        }

        private static class WardManager
        {
            #region Constants

            internal const int WardRange = 600;

            #endregion

            #region Static Fields

            internal static int LastInsecWardTime, LastInsecJumpTme;

            private static Vector3 lastPlacePos;

            private static int lastPlaceTime;

            #endregion

            #region Properties

            internal static bool CanWardJump => CanCastWard && W.IsReady() && IsWOne;

            private static bool CanCastWard => Variables.TickCount - lastPlaceTime > 1250 && Items.GetWardSlot() != null
                ;

            private static bool IsTryingToJump => lastPlacePos.IsValid() && Variables.TickCount - lastPlaceTime < 1250;

            #endregion

            #region Methods

            internal static void Init()
            {
                Game.OnUpdate += args =>
                    {
                        if (lastPlacePos.IsValid() && Variables.TickCount - lastPlaceTime > 1500)
                        {
                            lastPlacePos = new Vector3();
                        }
                        if (Player.IsDead)
                        {
                            return;
                        }
                        if (IsTryingToJump)
                        {
                            Jump(lastPlacePos);
                        }
                    };
                Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                    {
                        if (!lastPlacePos.IsValid() || !sender.IsMe || args.Slot != SpellSlot.W
                            || !args.SData.Name.ToLower().Contains("one"))
                        {
                            return;
                        }
                        var ward = args.Target as Obj_AI_Minion;
                        if (ward == null || !ward.IsValid() || !ward.IsWard() || ward.Distance(lastPlacePos) > 80)
                        {
                            return;
                        }
                        var tick = Variables.TickCount;
                        if (tick - LastInsecJumpTme < 1250)
                        {
                            LastInsecJumpTme = tick;
                        }
                        Insec.IsWardFlash = false;
                        lastPlacePos = new Vector3();
                    };
                GameObjectNotifier<Obj_AI_Minion>.OnCreate += (sender, minion) =>
                {
                    if (!lastPlacePos.IsValid() || minion.Distance(lastPlacePos) > 80 || !minion.IsAlly
                        || !minion.IsWard() || !W.IsInRange(minion))
                    {
                        return;
                    }
                    var tick = Variables.TickCount;
                    if (tick - LastInsecWardTime < 1250)
                    {
                        LastInsecWardTime = tick;
                    }
                    if (tick - lastPlaceTime < 1250 && W.IsReady() && IsWOne && W.CastOnUnit(minion))
                    {
                        lastW = tick;
                    }
                };
            }

            internal static void Place(Vector3 pos, int mode = 0)
            {
                if (!CanWardJump)
                {
                    return;
                }
                var ward = Items.GetWardSlot();
                var posPlayer = Player.ServerPosition;
                var posPlace = pos.Distance(posPlayer) < WardRange ? pos : posPlayer.LSExtend(pos, WardRange);
                Player.Spellbook.CastSpell(ward.SpellSlot, posPlace);
                switch (mode)
                {
                    case 0:
                        lastPlaceTime = Variables.TickCount + 1100;
                        break;
                    case 1:
                        lastPlaceTime = LastInsecWardTime = LastInsecJumpTme = Variables.TickCount;
                        break;
                }
                lastPlacePos = posPlace;
            }

            private static void Jump(Vector3 pos)
            {
                if (!W.IsReady() || !IsWOne || Variables.TickCount - lastW <= 500)
                {
                    return;
                }
                var wardObj =
                    GameObjects.AllyWards.Where(
                        i => i.LSIsValidTarget(W.Range, false) && i.IsWard() && i.Distance(pos) < 80)
                        .MinOrDefault(i => i.Distance(pos));
                if (wardObj != null && W.CastOnUnit(wardObj))
                {
                    lastW = Variables.TickCount;
                }
            }

            #endregion
        }
    }
}