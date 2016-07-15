using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Data.Enumerations;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Prediction = Challenger_Series.Utils.Prediction;
using Challenger_Series.Utils;
using LeagueSharp.SDK.Enumerations;

using TargetSelector = PortAIO.TSManager; namespace Challenger_Series.Plugins
{
    public class Caitlyn : CSPlugin
    {

        public LeagueSharp.SDK.Spell Q2 { get; set; }
        public LeagueSharp.SDK.Spell W2 { get; set; }
        public LeagueSharp.SDK.Spell E2 { get; set; }
        public LeagueSharp.SDK.Spell R2 { get; set; }

        public Caitlyn()
        {
            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 1200);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 820);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 770);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 2000);

            Q.SetSkillshot(0.25f, 60f, 2000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.00f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(3.00f, 50f, 1000f, false, SkillshotType.SkillshotLine);

            InitMenu();
            LeagueSharp.Common.LSEvents.AfterAttack += Orbwalker_OnPostAttack;
            LeagueSharp.Common.LSEvents.BeforeAttack += Orbwalker_OnPreAttack;
            DelayedOnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Events.OnGapCloser += EventsOnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
        }

        public override void OnUpdate(EventArgs args)
        {
            if (Q.IsReady()) this.QLogic();
            if (W.IsReady()) this.WLogic();
            if (R.IsReady()) this.RLogic();
        }

        private void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe && args.Animation == "Spell3")
            {
                var target = TargetSelector.GetTarget(1000, DamageType.Physical);
                var pred = Prediction.GetPrediction(target, Q);
                if (AlwaysQAfterE)
                {
                    if ((int)pred.Item1 >= (int)HitChance.Medium
                        && pred.Item2.Distance(ObjectManager.Player.ServerPosition) < 1100) Q.Cast(pred.Item2);
                }
                else
                {
                    if ((int)pred.Item1 > (int)HitChance.Medium
                        && pred.Item2.Distance(ObjectManager.Player.ServerPosition) < 1100) Q.Cast(pred.Item2);
                }
            }
        }

        private void EventsOnGapCloser(object oSender, Events.GapCloserEventArgs args)
        {
            var sender = args.Sender;
            if (UseEAntiGapclose)
            {
                if (args.IsDirectedToPlayer && args.Sender.Distance(ObjectManager.Player) < 750)
                {
                    if (E.IsReady() && ShouldE(sender.ServerPosition))
                    {
                        E.Cast(sender.ServerPosition);
                    }
                }
            }
        }

        private void OnInterruptableTarget(object oSender, Events.InterruptableTargetEventArgs args)
        {
            var sender = args.Sender;
            if (!GameObjects.AllyMinions.Any(m => !m.IsDead && m.CharData.BaseSkinName.Contains("trap") && m.Distance(sender.ServerPosition) < 100) && ObjectManager.Player.Distance(sender) < 550)
            {
                W.Cast(sender.ServerPosition);
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            base.OnProcessSpellCast(sender, args);
            if (sender is AIHeroClient && sender.IsEnemy)
            {
                if (args.SData.Name == "summonerflash" && args.End.Distance(ObjectManager.Player.ServerPosition) < 650)
                {
                    var pred = Prediction.GetPrediction((AIHeroClient)args.Target, E);
                    if (!pred.Item3.Any(o => o.IsMinion && !o.IsDead && !o.IsAlly) && ShouldE(args.End))
                    {
                        E.Cast(args.End);
                    }
                }
            }
        }

        public override void OnDraw(EventArgs args)
        {
            var drawRange = DrawRange;
            if (drawRange > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, drawRange, Color.Gold);
            }
            if (DrawQRange == true)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1200, Color.ForestGreen);
            }
            if (DrawRRange == true)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 2000, Color.ForestGreen);
            }
            var victims =
                   GameObjects.EnemyHeroes.Where(
                       x =>
                           x.IsValid && !x.IsDead && x.IsEnemy &&
                           (x.IsVisible && x.LSIsValidTarget()) &&
                           R.GetDamage(x) > x.Health - 100)
                       .Aggregate("", (current, target) => current + (target.ChampionName + " " + (target.Spellbook.Spells.Any(s => s.Name.Contains("heal") && s.IsReady()) ? "(Has Heal) " : "") + (target.Spellbook.Spells.Any(s => s.Name.Contains("barrier") && s.IsReady()) ? "(Has Barrier)" : "")));

            if (victims != "" && R.IsReady())
            {
                Drawing.DrawText(Drawing.Width * 0.44f, Drawing.Height * 0.7f, System.Drawing.Color.GreenYellow,
                    "Ult can kill: " + victims);
            }
        }

        private void Orbwalker_OnPreAttack(LeagueSharp.Common.BeforeAttackArgs args)
        {
            /*if (orbwalkingActionArgs.Target is Obj_AI_Minion && HasPassive && FocusOnHeadShotting &&
                Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
            {
                var target = orbwalkingActionArgs.Target as Obj_AI_Minion;
                if (target != null && !target.CharData.BaseSkinName.Contains("MinionSiege") && target.Health > 60)
                {
                    var tg = (AIHeroClient)TargetSelector.GetTarget(715, DamageType.Physical);
                    if (tg != null && tg.IsHPBarRendered)
                    {
                        Orbwalker.ForceTarget = tg;
                        orbwalkingActionArgs.Process = false;
                    }
                }
            }*/
        }

        private void Orbwalker_OnPostAttack(LeagueSharp.Common.AfterAttackArgs args)
        {
            PortAIO.OrbwalkerManager.ForcedTarget(null);
            if (E.IsReady() && this.UseECombo)
            {
                if (!OnlyUseEOnMelees)
                {
                    var eTarget = TargetSelector.GetTarget(UseEOnEnemiesCloserThanSlider, DamageType.Physical);
                    if (eTarget != null)
                    {
                        var pred = Prediction.GetPrediction(eTarget, E);
                        if (pred.Item3.Count == 0 && (int)pred.Item1 >= (int)HitChance.High && ShouldE(pred.Item2))
                        {
                            E.Cast(pred.Item2);
                        }
                    }
                }
                else
                {
                    var eTarget =
                        ValidTargets.FirstOrDefault(
                            e =>
                            e.IsMelee && e.Distance(ObjectManager.Player) < UseEOnEnemiesCloserThanSlider
                            && !e.IsZombie);
                    var pred = Prediction.GetPrediction(eTarget, E);
                    if (pred.Item3.Count == 0 && (int)pred.Item1 > (int)HitChance.Medium && ShouldE(pred.Item2))
                    {
                        E.Cast(pred.Item2);
                    }
                }
            }
        }

        private Menu ComboMenu;
        private Menu AutoWConfig;
        private bool UseQCombo;
        private bool UseWCombo;
        private bool UseECombo;
        private bool UseRCombo;
        private bool AlwaysQAfterE;
        private bool FocusOnHeadShotting;
        private bool UseWInterrupt;
        private bool OnlyUseEOnMelees;
        private bool UseEAntiGapclose;
        private int UseEOnEnemiesCloserThanSlider;
        private int DrawRange;
        private bool DrawQRange;
        private bool DrawRRange;
        private int QHarassMode;


        public void InitMenu()
        {
            ComboMenu = MainMenu.AddSubMenu("Combo Settings: ", "Combo Settings: ");
            ComboMenu.Add("caitqcombo", new CheckBox("Use Q", true));
            ComboMenu.Add("caitwcombo", new CheckBox("Use W", true));
            ComboMenu.Add("caitecombo", new CheckBox("Use E", true));
            ComboMenu.Add("caitrcombo", new KeyBind("Use R", false, KeyBind.BindTypes.HoldActive, 'R'));

            AutoWConfig = MainMenu.AddSubMenu("W Settings: ");
            AutoWConfig.Add("caitusewinterrupt", new CheckBox("Use W to Interrupt", true));

            //new Utils.Logic.PositionSaver(AutoWConfig, W);

            MainMenu.Add("caitfocusonheadshottingenemies", new CheckBox("Try to save Headshot for poking", true));
            MainMenu.Add("caitalwaysqaftere", new CheckBox("Always Q after E (EQ combo)", true));
            MainMenu.Add("caitqharassmode", new ComboBox("Q HARASS MODE: ", 0, "FULLDAMAGE", "ALLOWMINIONS", "DISABLED"));
            MainMenu.Add("caiteantigapclose", new CheckBox("Use E AntiGapclose", false));
            MainMenu.Add("caitecomboshit", new Slider("Use E on enemies closer than", 770, 200, 770));
            MainMenu.Add("caiteonlymelees", new CheckBox("Only use E on melees", false));
            MainMenu.Add("caitdrawrange", new Slider("Draw a circle with radius: ", 800, 0, 1240));
            MainMenu.Add("caitqrange", new CheckBox("Draw Q Range", true));
            MainMenu.Add("caitrrange", new CheckBox("Draw R Range", true));

            UseQCombo = getCheckBoxItem(ComboMenu, "caitqcombo");
            UseWCombo = getCheckBoxItem(ComboMenu, "caitwcombo");
            UseECombo = getCheckBoxItem(ComboMenu, "caitecombo");
            UseRCombo = getKeyBindItem(ComboMenu, "caitrcombo");

            QHarassMode = getBoxItem(MainMenu, "caitqharassmode");
            UseEOnEnemiesCloserThanSlider = getSliderItem(MainMenu, "caitecomboshit");

            DrawRange = getSliderItem(MainMenu, "caitdrawrange");
            DrawQRange = getCheckBoxItem(MainMenu, "caitqrange");
            DrawRRange = getCheckBoxItem(MainMenu, "caitrrange");

            AlwaysQAfterE = getCheckBoxItem(MainMenu, "caitalwaysqaftere");
            FocusOnHeadShotting = getCheckBoxItem(MainMenu, "caitfocusonheadshottingenemies");
            UseWInterrupt = getCheckBoxItem(AutoWConfig, "caitusewinterrupt");
            OnlyUseEOnMelees = getCheckBoxItem(MainMenu, "caiteonlymelees");
            UseEAntiGapclose = getCheckBoxItem(MainMenu, "caiteantigapclose");

        }

        #region Logic
        
        void QLogic()
        {
            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                if (UseQCombo && Q.IsReady() && ObjectManager.Player.CountEnemyHeroesInRange(800) == 0
                    && ObjectManager.Player.CountEnemyHeroesInRange(1100) > 0)
                {
                    var goodQTarget =
                        ValidTargets.FirstOrDefault(
                            t =>
                            t.Distance(ObjectManager.Player) < 950 && t.Health < Q.GetDamage(t)
                            || SquishyTargets.Contains(t.CharData.BaseSkinName));
                    if (goodQTarget != null)
                    {
                        var pred = Prediction.GetPrediction(goodQTarget, Q);
                        if ((int)pred.Item1 > (int)HitChance.Medium && pred.Item2.Distance(ObjectManager.Player.Position) < 1100)
                        {
                            Q.Cast(pred.Item2);
                        }
                    }
                }
            }
            if (!PortAIO.OrbwalkerManager.isNoneActive && !PortAIO.OrbwalkerManager.isComboActive
                && ObjectManager.Player.CountEnemyHeroesInRange(850) == 0)
            {
                var qHarassMode = QHarassMode;
                if (qHarassMode != 2)
                {
                    var qTarget = TargetSelector.GetTarget(1100, DamageType.Physical);
                    if (qTarget != null)
                    {
                        var pred = Prediction.GetPrediction(qTarget, Q);
                        if ((int)pred.Item1 > (int)HitChance.Medium && pred.Item2.Distance(ObjectManager.Player.Position) < 1100)
                        {
                            if (qHarassMode == 1)
                            {
                                Q.Cast(pred.Item2);
                            }
                            else if (pred.Item3.Count == 0)
                            {
                                Q.Cast(pred.Item2);
                            }
                        }
                    }
                }
            }
        }

        void WLogic()
        {
            var goodTarget =
                ValidTargets.FirstOrDefault(
                    e =>
                    !e.IsDead && e.HasBuffOfType(BuffType.Knockup) || e.HasBuffOfType(BuffType.Snare)
                    || e.HasBuffOfType(BuffType.Stun) || e.HasBuffOfType(BuffType.Suppression) || e.IsCharmed
                    || e.IsCastingInterruptableSpell() || !e.CanMove);
            if (goodTarget != null)
            {
                var pos = goodTarget.ServerPosition;
                if (!GameObjects.AllyMinions.Any(m => !m.IsDead && m.CharData.BaseSkinName.Contains("trap") && m.Distance(goodTarget.ServerPosition) < 100) && pos.Distance(ObjectManager.Player.ServerPosition) < 820)
                {
                    W.Cast(goodTarget.ServerPosition);
                }
            }
            foreach (var enemyMinion in
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        m =>
                        m.IsEnemy && m.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < W.Range
                        && m.HasBuff("teleport_target")))
            {

                W.Cast(enemyMinion.ServerPosition);
            }
            if (UseWCombo)
            {
                foreach (var hero in GameObjects.EnemyHeroes.Where(h => h.Distance(ObjectManager.Player) < W.Range))
                {
                    var pred = Prediction.GetPrediction(hero, W);
                    if (
                        !GameObjects.AllyMinions.Any(
                            m => !m.IsDead && m.CharData.BaseSkinName.Contains("trap") && m.Distance(pred.Item2) < 100) &&
                        (int)pred.Item1 > (int)HitChance.Medium && ObjectManager.Player.Distance(pred.Item2) < W.Range)
                    {
                        W.Cast(pred.Item2);
                    }
                }
            }
        }

        void RLogic()
        {
            if (UseRCombo && ObjectManager.Player.CountEnemyHeroesInRange(900) == 0)
            {
                foreach (var rTarget in
                    ValidTargets.Where(
                        e =>
                        SquishyTargets.Contains(e.CharData.BaseSkinName) && R.GetDamage(e) > 0.15 * e.MaxHealth
                        || R.GetDamage(e) > e.Health))
                {
                    if (rTarget.Distance(ObjectManager.Player) > 1400)
                    {
                        var pred = Prediction.GetPrediction(rTarget, R);
                        if (!pred.Item3.Any(obj => obj is AIHeroClient))
                        {
                            R.CastOnUnit(rTarget);
                        }
                        break;
                    }
                    R.CastOnUnit(rTarget);
                }
            }
        }

        #endregion

        private bool HasPassive => ObjectManager.Player.HasBuff("caitlynheadshot");

        private string[] SquishyTargets =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia",
                "Corki", "Draven", "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus",
                "Katarina", "Kennen", "KogMaw", "Kindred", "Leblanc", "Lucian", "Lux",
                "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon", "Teemo",
                "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "Velkoz",
                "Viktor", "Xerath", "Zed", "Ziggs", "Jhin", "Soraka"
            };

        private bool ShouldE(Vector3 predictedPos)
        {
            var rect = new Utils.Geometry.Rectangle(ObjectManager.Player.ServerPosition, predictedPos, 80f);
            if (GameObjects.EnemyMinions.Any(m => m.Distance(ObjectManager.Player) < 900 && !m.Position.IsOutside(rect)))
            {
                return false;
            }
            return true;
        }
    }
}
