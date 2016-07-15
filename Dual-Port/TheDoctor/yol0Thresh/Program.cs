using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;

// ReSharper disable InconsistentNaming

 namespace yol0Thresh
{
    internal class Program
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;

        private static readonly Spell _Q = new Spell(SpellSlot.Q, 1075);
        private static readonly Spell _W = new Spell(SpellSlot.W, 950);
        private static readonly Spell _E = new Spell(SpellSlot.E, 500);
        private static readonly Spell _R = new Spell(SpellSlot.R, 400);

        private static Menu Config;
        public static Menu comboMenu, harassMenu, flayMenu, miscMenu, boxMenu, ksMenu, drawMenu, lanternMenu;

        private static int qTick;
        private static int hookTick;
        private static Obj_AI_Base hookedUnit;


        private static List<Vector3> escapeSpots = new List<Vector3>();
        private static readonly List<GameObject> soulList = new List<GameObject>();

        private static AIHeroClient currentTarget
        {
            get
            {
                if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget is AIHeroClient && TargetSelector.SelectedTarget.Team != Player.Team)
                    return (AIHeroClient)TargetSelector.SelectedTarget;
                if (TargetSelector.SelectedTarget != null)
                    return TargetSelector.SelectedTarget;
                return TargetSelector.GetTarget(qRange + 175, DamageType.Physical);
            }
        }

        private static float qRange
        {
            get { return getSliderItem(miscMenu, "qRange"); }
        }

        public static void OnLoad()
        {
            if (Player.ChampionName != "Thresh")
                return;

            _Q.SetSkillshot(0.5f, 70, 1900, true, SkillshotType.SkillshotLine);
            _W.SetSkillshot(0f, 200, 1750, false, SkillshotType.SkillshotCircle);
            _E.SetSkillshot(0.3f, 60, float.MaxValue, false, SkillshotType.SkillshotLine);

            Config = MainMenu.AddMenu("yol0 Thresh", "yolothresh");


            comboMenu = Config.AddSubMenu("Combo Settings", "Combo");
            comboMenu.Add("useQ1", new CheckBox("Use Q1"));
            comboMenu.Add("useQ2", new CheckBox("Use Q2"));
            comboMenu.Add("useE", new CheckBox("Use Flay"));
            comboMenu.Add("useW", new CheckBox("Throw Lantern to Ally"));


            harassMenu = Config.AddSubMenu("Harass Settings", "Harass");
            harassMenu.Add("useQ1", new CheckBox("Use Q1"));
            harassMenu.Add("useE", new CheckBox("Use Flay"));
            harassMenu.Add("manaPercent", new Slider("Mana %", 40, 0, 100));


            flayMenu = Config.AddSubMenu("Flay Settings", "flay");
            flayMenu.Add("pullEnemy", new KeyBind("Pull Enemy", false, KeyBind.BindTypes.HoldActive, 'H'));
            flayMenu.Add("pushEnemy", new KeyBind("Push Enemy", false, KeyBind.BindTypes.HoldActive, 'T'));
            flayMenu.AddLabel("Per-Enemy Settings");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(unit => unit.Team != Player.Team))
            {
                flayMenu.Add(enemy.ChampionName, new ComboBox(enemy.ChampionName, 0, "Pull", "Push"));

            }

            lanternMenu = Config.AddSubMenu("Lantern Settings", "Lantern");
            lanternMenu.Add("useW", new CheckBox("Throw to Ally"));
            lanternMenu.Add("numEnemies", new Slider("Throw if # Enemies", 2, 1, 5));
            lanternMenu.Add("useWCC", new CheckBox("Throw to CC'd Ally"));


            boxMenu = Config.AddSubMenu("Box Settings", "Box");
            boxMenu.Add("useR", new CheckBox("Auto Use Box"));
            boxMenu.Add("minEnemies", new Slider("Minimum Enemies", 3, 1, 5));


            miscMenu = Config.AddSubMenu("Misc Settings", "Misc");
            miscMenu.Add("qRange", new Slider("Q Range", 1075, 700, 1075));
            miscMenu.Add("qHitChance", new ComboBox("Q Hitchance", 3, "Low", "Medium", "High", "Very High"));
            miscMenu.AddLabel("Gapclosers");
            if (ObjectManager.Get<AIHeroClient>().Any(unit => unit.Team != Player.Team && unit.ChampionName == "Rengar"))
            {
                miscMenu.Add("rengarleap", new CheckBox("Rengar - Unseen Predator"));

            }
            foreach (var spell in from spell in AntiGapcloser.Spells
                from enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(unit => unit.Team != Player.Team)
                        .Where(enemy => spell.ChampionName == enemy.ChampionName)
                select spell)
            {
                miscMenu.Add(spell.SpellName, new CheckBox(spell.ChampionName + " - " + spell.SpellName));

            }

            miscMenu.AddLabel("Interruptble Spells");
            foreach (var spell in Interrupter.Spells)
            {
                var spell1 = spell;
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(unit => unit.Team != Player.Team)
                            .Where(enemy => spell1.ChampionName == enemy.ChampionName))
                {
                    miscMenu.Add(spell.SpellName, new CheckBox(spell.ChampionName + " - " + spell.SpellName));
                    miscMenu.Add("enabled", new CheckBox("Enabled"));
                    miscMenu.Add("useE", new CheckBox("Interrupt with Flay"));
                    miscMenu.Add("useQ", new CheckBox("Interrupt with Hook"));

                }
            }

            ksMenu = Config.AddSubMenu("Killsteal Settings", "KS");
            ksMenu.Add("ksQ", new CheckBox("KS with Q", false));
            ksMenu.Add("ksE", new CheckBox("KS with E", false));


            drawMenu = Config.AddSubMenu("Draw Settings", "Draw");
            drawMenu.Add("drawQMax", new CheckBox("Draw Q Max Range", true));
            drawMenu.Add("drawQEffective", new CheckBox("Draw Q Effective", true));
            drawMenu.Add("drawW", new CheckBox("Draw W Range", true));
            drawMenu.Add("drawE", new CheckBox("Draw E Range", true));
            drawMenu.Add("drawQCol", new CheckBox("Draw Q Line", true));
            drawMenu.Add("drawTargetC", new CheckBox("Draw Target (Circle)", true));
            drawMenu.Add("drawTargetT", new CheckBox("Draw Target (Text)", true));
            drawMenu.Add("drawSouls", new CheckBox("Draw Circle on Souls", true));

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Obj_AI_Base.OnPlayAnimation += OnAnimation;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnGameUpdate;
            GameObject.OnCreate += OnCreateObj;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapCloser;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
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
        

        public static void OnGameUpdate(EventArgs args)
        {
            AutoBox();
            KS();
            Lantern();
            UpdateSouls();
            UpdateBuffs();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Combo();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                Harass();

            if (getKeyBindItem(flayMenu, "pullEnemy"))
            {
                Orbwalker.MoveTo(Game.CursorPos);
                var target = TargetSelector.GetTarget(_E.Range, DamageType.Physical);
                if (target != null)
                    PullFlay(target);
            }

            if (getKeyBindItem(flayMenu, "pushEnemy"))
            {
                Orbwalker.MoveTo(Game.CursorPos);
                var target = TargetSelector.GetTarget(_E.Range, DamageType.Physical);
                if (target != null)
                    PushFlay(target);
            }
        }

        public static void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "drawQMax") && !Player.IsDead)
            {
                Render.Circle.DrawCircle(Player.Position, _Q.Range,
                    Color.Blue);
            }

            if (getCheckBoxItem(drawMenu, "drawQEffective") && !Player.IsDead)
            {
                Render.Circle.DrawCircle(Player.Position, qRange,
                    Color.Blue);
            }

            if (getCheckBoxItem(drawMenu, "drawW") && !Player.IsDead)
            {
                Render.Circle.DrawCircle(Player.Position, _W.Range,
                    Color.Blue);
            }

            if (getCheckBoxItem(drawMenu, "drawE") && !Player.IsDead)
            {
                Render.Circle.DrawCircle(Player.Position, _E.Range,
                    Color.Blue);
            }

            if (getCheckBoxItem(drawMenu, "drawQCol") && !Player.IsDead)
            {
                if (currentTarget != null && Player.LSDistance(currentTarget) < qRange + 200)
                {
                    var playerPos = Drawing.WorldToScreen(Player.Position);
                    var targetPos = Drawing.WorldToScreen(currentTarget.Position);
                    Drawing.DrawLine(playerPos, targetPos, 4,
                        _Q.GetPrediction(currentTarget, overrideRange: qRange).Hitchance < GetSelectedHitChance()
                            ? Color.Red
                            : Color.Green);
                }
            }

            if (currentTarget != null &&
                (getCheckBoxItem(drawMenu, "drawTargetC") && currentTarget.IsVisible &&
                 !currentTarget.IsDead))
            {
                Render.Circle.DrawCircle(currentTarget.Position, currentTarget.BoundingRadius + 10, Color.Red);
                Render.Circle.DrawCircle(currentTarget.Position, currentTarget.BoundingRadius + 25, Color.Red);
                Render.Circle.DrawCircle(currentTarget.Position, currentTarget.BoundingRadius + 45, Color.Red);
            }

            if (currentTarget != null &&
                (getCheckBoxItem(drawMenu, "drawTargetT") && !currentTarget.IsDead))
            {
                Drawing.DrawText(100, 150, Color.Red, "Current Target: " + currentTarget.ChampionName);
            }

            if (getCheckBoxItem(drawMenu, "drawSouls") && !Player.IsDead)
            {
                foreach (var soul in soulList.Where(s => s.IsValid))
                {
                    Render.Circle.DrawCircle(soul.Position, 50,
                        Color.LightBlue);
                }
            }
        }

        public static void OnAnimation(GameObject unit, GameObjectPlayAnimationEventArgs args)
        {
            var hero = unit as AIHeroClient;
            if (hero != null)
            {
                if (hero.Team == Player.Team) return;
                if (hero.ChampionName == "Rengar" && args.Animation == "Spell5" && Player.LSDistance(hero) <= 725)
                {
                    if (_E.IsReady() &&
                        getCheckBoxItem(miscMenu, "rengarleap"))
                    {
                        _E.Cast(unit.Position);
                    }
                }
            }
        }

        public static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name == "ThreshQ")
            {
                qTick = Environment.TickCount + 500;
            }

            if (args.SData.Name == "ThreshE")
            {
                Orbwalker.ResetAutoAttack();
            }
        }

        public static void OnCreateObj(GameObject obj, EventArgs args)
        {
            if (obj.Name.Contains("Thresh_Base_soul"))
            {
                soulList.Add(obj);
            }
        }

        public static void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (getCheckBoxItem(miscMenu, "enabled"))
            {
                if (
                    getCheckBoxItem(miscMenu, "useE") && _E.IsReady() &&
                    Player.LSDistance(unit) < _E.Range)
                {
                    if (ShouldPull((AIHeroClient) unit))
                        PullFlay(unit);
                    else
                        PushFlay(unit);
                }
                else if (
                    getCheckBoxItem(miscMenu, "useQ") && _Q.IsReady() &&
                    !_Q.GetPrediction(unit).CollisionObjects.Any())
                {
                    _Q.Cast(unit);
                }
            }
        }

        public static void OnEnemyGapCloser(ActiveGapcloser gapcloser)
        {
            if (_E.IsReady() &&
                getCheckBoxItem(miscMenu, gapcloser.SpellName.ToLower()) &&
                Player.LSDistance(gapcloser.Sender) < _E.Range + 100)
            {
                if (gapcloser.SpellName == "LeonaZenithBlade")
                {
                    if (Player.LSDistance(gapcloser.Start) < Player.LSDistance(gapcloser.End))
                        PullFlay(gapcloser.Sender);
                    else
                        LeagueSharp.Common.Utility.DelayAction.Add(75, delegate { PushFlay(gapcloser.Sender); });
                }
                else
                {
                    if (Player.LSDistance(gapcloser.Start) < Player.LSDistance(gapcloser.End))
                        PullFlay(gapcloser.Sender);
                    else
                        PushFlay(gapcloser.Sender);
                }
            }
        }

        private static void UpdateBuffs()
        {
            if (hookedUnit == null)
            {
                foreach (
                    var obj in
                        ObjectManager.Get<Obj_AI_Base>()
                            .Where(unit => unit.Team != Player.Team)
                            .Where(obj => obj.HasBuff("threshqfakeknockup")))
                {
                    hookedUnit = obj;
                    hookTick = Environment.TickCount + 1500;
                    return;
                }
            }
            hookTick = 0;
            hookedUnit = null;
        }

        private static void UpdateSouls()
        {
            var remove = soulList.Where(soul => !soul.IsValid).ToList();
            foreach (var soul in remove)
            {
                soulList.Remove(soul);
            }
        }

        private static bool ShouldPull(AIHeroClient unit)
        {
            return
                getBoxItem(flayMenu, unit.ChampionName) == 0;
        }

        private static bool IsFirstQ()
        {
            return _Q.Instance.Name == "ThreshQ";
        }

        private static bool IsSecondQ()
        {
            return _Q.Instance.Name == "threshqleap";
        }

        private static bool IsImmune(Obj_AI_Base unit)
        {
            return unit.HasBuff("BlackShield") || unit.HasBuff("SivirE") || unit.HasBuff("NocturneShroudofDarkness") ||
                   unit.HasBuff("deathdefiedbuff");
        }

        private static void KS()
        {
            if (getCheckBoxItem(ksMenu, "ksE"))
            {
                foreach (
                    var enemy in
                        from enemy in
                            ObjectManager.Get<AIHeroClient>().Where(unit => unit.Team != Player.Team && !unit.IsDead)
                        let eDmg = Player.LSGetSpellDamage(enemy, SpellSlot.E)
                        where eDmg > enemy.Health && Player.LSDistance(enemy) <= _E.Range && _E.IsReady()
                        select enemy)
                {
                    PullFlay(enemy);
                    return;
                }
            }

            if (getCheckBoxItem(ksMenu, "ksQ"))
            {
                foreach (
                    var enemy in
                        from enemy in
                            ObjectManager.Get<AIHeroClient>().Where(unit => unit.Team != Player.Team && !unit.IsDead)
                        let qDmg = Player.LSGetSpellDamage(enemy, SpellSlot.Q)
                        where qDmg > enemy.Health && Player.LSDistance(enemy) <= qRange && IsFirstQ() && _Q.IsReady() &&
                              _Q.GetPrediction(enemy, overrideRange: qRange).Hitchance >= GetSelectedHitChance()
                        select enemy)
                {
                    _Q.Cast(enemy);
                    return;
                }
            }
        }

        private static HitChance GetSelectedHitChance()
        {
            switch (getBoxItem(miscMenu, "qHitChance"))
            {
                case 3:
                    return HitChance.VeryHigh;
                case 2:
                    return HitChance.High;
                case 1:
                    return HitChance.Medium;
                case 0:
                    return HitChance.Low;
            }
            return HitChance.Medium;
        }

        private static void AutoBox()
        {
            if (getCheckBoxItem(boxMenu, "useR") && _R.IsReady() &&
                ObjectManager.Get<AIHeroClient>()
                    .Count(unit => unit.Team != Player.Team && Player.LSDistance(unit) <= _R.Range) >=
                getSliderItem(boxMenu, "minEnemies"))
            {
                _R.Cast();
            }
        }

        private static void Combo()
        {
            if (getCheckBoxItem(comboMenu, "useE") && _E.IsReady() &&
                Player.LSDistance(currentTarget) < _E.Range &&
                (!_Q.IsReady() && Environment.TickCount > qTick || _Q.IsReady() && IsFirstQ()))
            {
                Flay(currentTarget);
            }
            else if (getCheckBoxItem(comboMenu, "useQ2") && Player.LSDistance(currentTarget) > _E.Range &&
                     _Q.IsReady() &&
                     Environment.TickCount >= hookTick - 500 && IsSecondQ() &&
                     ObjectManager.Get<AIHeroClient>().FirstOrDefault(unit => unit.HasBuff("ThreshQ")) != null)
            {
                _Q.Cast();
            }
            else if (getCheckBoxItem(comboMenu, "useQ2") &&
                     getCheckBoxItem(comboMenu, "useE") && _Q.IsReady() &&
                     _E.IsReady() &&
                     ObjectManager.Get<Obj_AI_Minion>()
                         .FirstOrDefault(unit => unit.HasBuff("ThreshQ") && unit.LSDistance(currentTarget) <= _E.Range) !=
                     null && IsSecondQ())
            {
                _Q.Cast();
            }

            if (getCheckBoxItem(comboMenu, "useQ1") && _Q.IsReady() && IsFirstQ() &&
                !IsImmune(currentTarget))
            {
                _Q.CastIfHitchanceEquals(currentTarget, GetSelectedHitChance());
            }

            if (getCheckBoxItem(comboMenu, "useW") && _W.IsReady() &&
                ObjectManager.Get<AIHeroClient>().FirstOrDefault(unit => unit.HasBuff("ThreshQ")) != null)
            {
                var nearAlly = GetNearAlly();
                if (nearAlly != null)
                {
                    _W.Cast(nearAlly);
                }
            }
        }

        private static void Harass()
        {
            var percentManaAfterQ = 100*((Player.Mana - _Q.Instance.SData.Mana)/Player.MaxMana);
            var percentManaAfterE = 100*((Player.Mana - _E.Instance.SData.Mana) /Player.MaxMana);
            var minPercentMana = getSliderItem(harassMenu, "manaPercent");

            if (getCheckBoxItem(harassMenu, "useQ1") && _Q.IsReady() && IsFirstQ() &&
                !IsImmune(currentTarget) && percentManaAfterQ >= minPercentMana)
            {
                if (_Q.GetPrediction(currentTarget, false, qRange).Hitchance >= GetSelectedHitChance())
                {
                    _Q.Cast(currentTarget);
                }
            }
            else if (getCheckBoxItem(harassMenu, "useE") && !IsImmune(currentTarget) && _E.IsReady() &&
                     Player.LSDistance(currentTarget) < _E.Range && percentManaAfterE >= minPercentMana)
            {
                Flay(currentTarget);
            }
        }

        private static void Lantern()
        {
            if (getCheckBoxItem(lanternMenu, "useWCC") && GetCCAlly() != null && _W.IsReady())
            {
                _W.Cast(GetCCAlly());
                return;
            }

            if (getCheckBoxItem(lanternMenu, "useW") && GetLowAlly() != null && _W.IsReady())
            {
                if (GetLowAlly().Position.LSCountEnemiesInRange(950) >=
                    getSliderItem(lanternMenu, "numEnemies"))
                {
                    _W.Cast(GetLowAlly());
                }
            }
        }

        private static AIHeroClient GetCCAlly()
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        unit =>
                            !unit.IsMe && unit.Team == Player.Team && !unit.IsDead &&
                            Player.LSDistance(unit) <= _W.Range + 200)
                    .FirstOrDefault(
                        ally =>
                            ally.HasBuffOfType(BuffType.Charm) || ally.HasBuffOfType(BuffType.CombatDehancer) ||
                            ally.HasBuffOfType(BuffType.Fear) || ally.HasBuffOfType(BuffType.Knockback) ||
                            ally.HasBuffOfType(BuffType.Knockup) || ally.HasBuffOfType(BuffType.Polymorph) ||
                            ally.HasBuffOfType(BuffType.Snare) || ally.HasBuffOfType(BuffType.Stun) ||
                            ally.HasBuffOfType(BuffType.Suppression) || ally.HasBuffOfType(BuffType.Taunt));
        }

        private static AIHeroClient GetLowAlly()
        {
            AIHeroClient lowAlly = null;
            foreach (
                var ally in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            unit => unit.Team == Player.Team && !unit.IsDead && Player.LSDistance(unit) <= _W.Range + 200)
                )
            {
                if (lowAlly == null)
                    lowAlly = ally;
                else if (!lowAlly.IsDead && ally.Health/ally.MaxHealth < lowAlly.Health/lowAlly.MaxHealth)
                    lowAlly = ally;
            }
            return lowAlly;
        }

        private static AIHeroClient GetNearAlly()
        {
            if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget is AIHeroClient && TargetSelector.SelectedTarget.Team == Player.Team &&
                Player.LSDistance(TargetSelector.SelectedTarget.Position) <= _W.Range + 200)
            {
                return (AIHeroClient)TargetSelector.SelectedTarget;
            }

            AIHeroClient nearAlly = null;
            foreach (
                var ally in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            unit => unit.Team == Player.Team && !unit.IsDead && Player.LSDistance(unit) <= _W.Range + 200)
                )
            {
                if (nearAlly == null)
                    nearAlly = ally;
                else if (!nearAlly.IsDead && Player.LSDistance(ally) < Player.LSDistance(nearAlly))
                    nearAlly = ally;
            }
            return nearAlly;
        }

        private static void PushFlay(Obj_AI_Base unit)
        {
            if (Player.LSDistance(unit) <= _E.Range)
            {
                _E.Cast(unit.ServerPosition);
            }
        }

        private static void PullFlay(Obj_AI_Base unit)
        {
            if (Player.LSDistance(unit) <= _E.Range)
            {
                var pX = Player.Position.X + (Player.Position.X - unit.Position.X);
                var pY = Player.Position.Y + (Player.Position.Y - unit.Position.Y);
                _E.Cast(new Vector2(pX, pY));
            }
        }

        private static void Flay(AIHeroClient unit)
        {
            if (ShouldPull(unit))
            {
                PullFlay(unit);
            }
            else
            {
                PushFlay(unit);
            }
        }
    }
}
