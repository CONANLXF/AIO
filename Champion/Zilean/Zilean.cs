using TargetSelector = PortAIO.TSManager; namespace ElZilean
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Collections.Generic;
    using System.Drawing;

    using EloBuddy;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;
    using Spell = LeagueSharp.Common.Spell;
    using Utility = LeagueSharp.Common.Utility;
    using Damage = LeagueSharp.Common.Damage;


    internal class Zilean
    {

        #region Constructors and Destructors

        static Zilean()
        {
            Spells = new List<InitiatorSpell>
                         {
                             new InitiatorSpell { ChampionName = "Monkeyking", SDataName = "monkeykingnimbus" },
                             new InitiatorSpell { ChampionName = "Monkeyking", SDataName = "monkeykingdecoy" },
                             new InitiatorSpell { ChampionName = "Monkeyking", SDataName = "monkeykingspintowin" },
                             new InitiatorSpell { ChampionName = "Olaf", SDataName = "olafragnarok" },
                             new InitiatorSpell { ChampionName = "Gragas", SDataName = "gragase" },
                             new InitiatorSpell { ChampionName = "Hecarim", SDataName = "hecarimult" },
                             new InitiatorSpell { ChampionName = "Hecarim", SDataName = "HecarimRamp" },
                             new InitiatorSpell { ChampionName = "Ekko", SDataName = "ekkoe" },
                             new InitiatorSpell { ChampionName = "Malphite", SDataName = "ufslash " },
                             new InitiatorSpell { ChampionName = "Vi", SDataName = "viq" },
                             new InitiatorSpell { ChampionName = "Vi", SDataName = "vir" },
                             new InitiatorSpell { ChampionName = "Volibear", SDataName = "volibearq" },
                             new InitiatorSpell { ChampionName = "Lissandra", SDataName = "lissandrae" },
                             new InitiatorSpell { ChampionName = "Gnar", SDataName = "gnare" },
                             new InitiatorSpell { ChampionName = "Fiora", SDataName = "fioraq" },
                             new InitiatorSpell { ChampionName = "Sion", SDataName = "sionr" },
                             new InitiatorSpell { ChampionName = "Zac", SDataName = "zace" },
                             new InitiatorSpell { ChampionName = "KhaZix", SDataName = "khazixe" },
                             new InitiatorSpell { ChampionName = "KhaZix", SDataName = "khazixelong" },
                             new InitiatorSpell { ChampionName = "Kennen", SDataName = "kennenlightningrush" },
                             new InitiatorSpell { ChampionName = "Jax", SDataName = "jaxleapstrike" },
                             new InitiatorSpell { ChampionName = "Leona", SDataName = "leonazenithblademissle" },
                             new InitiatorSpell { ChampionName = "Shen", SDataName = "shene" },
                             new InitiatorSpell { ChampionName = "Ryze", SDataName = "ryzer" },
                             new InitiatorSpell { ChampionName = "Lucian", SDataName = "luciane" },
                             new InitiatorSpell { ChampionName = "Elise", SDataName = "elisespidereinitial" },
                             new InitiatorSpell { ChampionName = "Diana", SDataName = "dianateleport" },
                             new InitiatorSpell { ChampionName = "Akali", SDataName = "akalishadowdance" },
                             new InitiatorSpell { ChampionName = "Renekton", SDataName = "renektonsliceanddice" },
                             new InitiatorSpell { ChampionName = "Thresh", SDataName = "threshqleap" },
                             new InitiatorSpell { ChampionName = "Rengar", SDataName = "rengarr" },
                             new InitiatorSpell { ChampionName = "Shyvana", SDataName = "shyvanatransformcast" },
                             new InitiatorSpell { ChampionName = "Shyvana", SDataName = "shyvanatransformleap" },
                             new InitiatorSpell { ChampionName = "Shyvana", SDataName = "ShyvanaImmolationAura" },
                             new InitiatorSpell { ChampionName = "Udyr", SDataName = "udyrbearstance" },
                             new InitiatorSpell { ChampionName = "Kassadin", SDataName = "riftwalk" },
                             new InitiatorSpell { ChampionName = "JarvanIV", SDataName = "jarvanivdragonstrike" },
                             new InitiatorSpell { ChampionName = "Irelia", SDataName = "ireliagatotsu" },
                             new InitiatorSpell { ChampionName = "DrMundo", SDataName = "Sadism" },
                             new InitiatorSpell { ChampionName = "MasterYi", SDataName = "Highlander" },
                             new InitiatorSpell { ChampionName = "Shaco", SDataName = "Deceive" },
                             new InitiatorSpell { ChampionName = "Ahri", SDataName = "AhriTumble" },
                             new InitiatorSpell { ChampionName = "LeeSin", SDataName = "blindmonkqtwo" },
                             new InitiatorSpell { ChampionName = "Yasuo", SDataName = "yasuorknockupcombow" },
                             new InitiatorSpell { ChampionName = "Evelynn", SDataName = "evelynnw" },
                             new InitiatorSpell { ChampionName = "FiddleSticks", SDataName = "Crowstorm" },
                             new InitiatorSpell { ChampionName = "Sivir", SDataName = "SivirR" }
                         };
        }

        #endregion

        #region Public Properties


        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<InitiatorSpell> Spells { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the E spell
        /// </summary>
        /// <value>
        ///     The E spell
        /// </value>
        private static Spell E { get; set; }

        /// <summary>
        ///     Check if Zilean has speed passive
        /// </summary>
        private static bool HasSpeedBuff => Player.Buffs.Any(x => x.Name.ToLower().Contains("timewarp"));

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        private static Spell IgniteSpell { get; set; }

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
        /// <value>
        ///     The menu
        /// </value>
        private static Menu Menu { get; set; }
        public static Menu comboMenu, harassMenu, fleeMenu, ultMenu, laneMenu, drawingsMenu, miscMenu, initiatorMenu;

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets or sets the Q spell
        /// </summary>
        /// <value>
        ///     The Q spell
        /// </value>
        private static Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the R spell.
        /// </summary>
        /// <value>
        ///     The R spell
        /// </value>
        private static Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the W spell
        /// </summary>
        /// <value>
        ///     The W spell
        /// </value>
        private static Spell W { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Fired when the game loads.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnGameLoad()
        {
            try
            {
                if (Player.ChampionName != "Zilean")
                {
                    return;
                }

                var igniteSlot = Player.GetSpellSlot("summonerdot");
                if (igniteSlot != SpellSlot.Unknown)
                {
                    IgniteSpell = new Spell(igniteSlot, 600f);
                }

                foreach (var ally in HeroManager.Allies)
                {
                    IncomingDamageManager.AddChampion(ally);
                }

                IncomingDamageManager.Skillshots = true;


                Q = new Spell(SpellSlot.Q, 900f);
                W = new Spell(SpellSlot.W, Player.GetAutoAttackRange(Player));
                E = new Spell(SpellSlot.E, 700f);
                R = new Spell(SpellSlot.R, 900f);

                Q.SetSkillshot(0.7f, 140f - 25f, int.MaxValue, false, SkillshotType.SkillshotCircle);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                LSEvents.BeforeAttack += BeforeAttack;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the menu
        /// </summary>
        /// <value>
        ///     Creates the menu
        /// </value>
        private static void GenerateMenu()
        {
            try
            {
                Menu = MainMenu.AddMenu("ElZilean", "ElZilean");


                comboMenu = Menu.AddSubMenu("Combo", "Combo");
                {
                    comboMenu.Add("ElZilean.Combo.Q", new CheckBox("Use Q", true));
                    comboMenu.Add("ElZilean.Combo.Focus.Bomb", new CheckBox("Focus target with Q", true));
                    comboMenu.Add("ElZilean.Combo.W", new CheckBox("Use W", true));
                    comboMenu.Add("ElZilean.Combo.E", new CheckBox("Use E", true));
                    comboMenu.Add("ElZilean.Ignite", new CheckBox("Use Ignite", true));
                    comboMenu.Add("ElZilean.Combo.W2", new CheckBox("Always reset Q", false));
                    comboMenu.Add("ElZilean.DoubleBombMouse", new KeyBind("Double bomb to mouse", false, KeyBind.BindTypes.HoldActive, 'Y'));
                }


                harassMenu = Menu.AddSubMenu("Harass", "Harass");
                {
                    harassMenu.Add("ElZilean.Harass.Q", new CheckBox("Use Q", true));
                    harassMenu.Add("ElZilean.Harass.W", new CheckBox("Use W", true));
                }


                ultMenu = Menu.AddSubMenu("Ultimate", "Ultimate");
                {
                    ultMenu.Add("min-health", new Slider("Health percentage", 20, 0, 100));
                    ultMenu.Add("min-damage", new Slider("Heal on % incoming damage", 20, 0, 100));
                    ultMenu.Add("ElZilean.Ultimate.R", new CheckBox("Use R", true));
                    ultMenu.AddLabel("Ultimate Wihtelist");
                    foreach (var x in HeroManager.Allies)
                    {
                        ultMenu.Add($"R{x.ChampionName}", new CheckBox("Use R on " + x.ChampionName));
                    }
                }


                laneMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
                {
                    laneMenu.Add("ElZilean.laneclear.Q", new CheckBox("Use Q", true));
                    laneMenu.Add("ElZilean.laneclear.W", new CheckBox("Use W", true));
                    laneMenu.Add("ElZilean.laneclear.Mana", new Slider("Minimum mana", 20, 0, 100));
                    laneMenu.Add("ElZilean.laneclear.QMouse", new CheckBox("Cast Q to mouse", false));
                }


                initiatorMenu = Menu.AddSubMenu("Initiators", "Initiators");
                {
                    // todo filter out champs that have no speed stuff
                    foreach (var ally in HeroManager.Allies)
                    {
                        initiatorMenu.Add($"Initiator{ally.CharData.BaseSkinName}", new CheckBox("Initiator E: " + ally.ChampionName, true));
                    }
                }


                fleeMenu = Menu.AddSubMenu("Flee", "Flee");
                {
                    fleeMenu.Add("ElZilean.Flee.Mana", new Slider("Minimum mana", 20, 0, 100));
                }

                miscMenu = Menu.AddSubMenu("Misc", "Misc");
                {
                    miscMenu.Add("ElZilean.Combo.AA", new CheckBox("Don't AA before Q", true));
                    miscMenu.Add("ElZilean.Q.Stun", new CheckBox("Auto Q on stunned targets", true));
                    miscMenu.Add("ElZilean.Q.Interrupt", new CheckBox("Interrupt spells with Q", true));
                    miscMenu.Add("ElZilean.E.Slow", new CheckBox("Speed up slowed allies", true));
                }

                drawingsMenu = Menu.AddSubMenu("Drawings", "Drawings");
                {
                    drawingsMenu.Add("ElZilean.Draw.Off", new CheckBox("Disable drawings", false));
                    drawingsMenu.Add("ElZilean.Draw.Q", new CheckBox("Draw Q", true));
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
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


        /// <summary>
        ///     
        /// </summary>
        /// <param name="args"></param>
        private static void BeforeAttack(BeforeAttackArgs args)
        {
            if (getCheckBoxItem(miscMenu, "ElZilean.Combo.AA"))
            {
                if (PortAIO.OrbwalkerManager.isComboActive || PortAIO.OrbwalkerManager.isHarassActive)
                {
                    if (Q.IsReady())
                    {
                        args.Process = false;
                    }
                }
            }
        }

        /// <summary>
        ///     The ignite killsteal logic
        /// </summary>
        private static void HandleIgnite()
        {
            if (Player.GetSpellSlot("summonerdot") == SpellSlot.Unknown)
            {
                return;
            }

            var kSableEnemy =
                HeroManager.Enemies.FirstOrDefault(
                    hero =>
                    hero.LSIsValidTarget(550f) && !hero.HasBuff("summonerdot") && !hero.IsZombie
                    && Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) >= hero.Health);

            if (kSableEnemy != null)
            {
                Player.Spellbook.CastSpell(IgniteSpell.Slot, kSableEnemy);
            }
        }

        private static void MouseCombo()
        {
            if (getCheckBoxItem(comboMenu, "ElZilean.Combo.Q") && Q.IsReady())
            {
                Q.Cast(Game.CursorPos);
                Utility.DelayAction.Add(100, () => W.Cast());
            }
        }

        /// <summary>
        ///     Combo logic
        /// </summary>
        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (getCheckBoxItem(comboMenu, "ElZilean.Combo.E") && E.IsReady())
            {
               /* if (!Q.IsReady() || Q.Instance.CooldownExpires - Game.Time < 2)
                {
                    return;
                } */

                if (Player.GetEnemiesInRange(E.Range).Any())
                {
                    var closestEnemy =
                        Player.GetEnemiesInRange(E.Range)
                            .OrderByDescending(h => (h.PhysicalDamageDealtPlayer + h.MagicDamageDealtPlayer))
                            .FirstOrDefault();

                    if (closestEnemy == null)
                    {
                        return;
                    }

                    if (closestEnemy.HasBuffOfType(BuffType.Stun))
                    {
                        return;
                    }

                    E.Cast(closestEnemy);
                }

                if (Player.GetAlliesInRange(E.Range).Any() && Player.GetEnemiesInRange(800f).Count >= 1)
                {
                    var closestToTarget =
                        Player.GetAlliesInRange(E.Range)
                            .OrderByDescending(h => (h.PhysicalDamageDealtPlayer + h.MagicDamageDealtPlayer))
                            .FirstOrDefault();

                    E.Cast(closestToTarget);
                }
            }

            if (getCheckBoxItem(comboMenu, "ElZilean.Combo.Q") && Q.IsReady() && target.LSIsValidTarget(Q.Range) && !target.IsZombie)
            {
                var pred = Q.GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(pred.CastPosition);
                }
            }

            // Check if target has a bomb
            var isBombed = HeroManager.Enemies.Find(x => x.HasBuff("ZileanQEnemyBomb") && x.LSIsValidTarget(Q.Range));

            if (!isBombed.LSIsValidTarget())
            {
                return;
            }

            if (isBombed != null && isBombed.LSIsValidTarget(Q.Range))
            {
                if (Q.Instance.CooldownExpires - Game.Time < 3)
                {
                    return;
                }

                if (isBombed != null)
                {
                    if (getCheckBoxItem(comboMenu, "ElZilean.Combo.W"))
                    {
                        Utility.DelayAction.Add(300, () => W.Cast());
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "ElZilean.Combo.W") && getCheckBoxItem(comboMenu, "ElZilean.Combo.W2") && W.IsReady() && !Q.IsReady())
            {
                W.Cast();
            }

            if (getCheckBoxItem(comboMenu, "ElZilean.Ignite") && isBombed != null)
            {
                if (Player.GetSpellSlot("summonerdot") == SpellSlot.Unknown)
                {
                    return;
                }

                if (Q.GetDamage(isBombed) + IgniteSpell.GetDamage(isBombed) > isBombed.Health)
                {
                    if (isBombed.LSIsValidTarget(Q.Range))
                    {
                        Player.Spellbook.CastSpell(IgniteSpell.Slot, isBombed);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawingsMenu, "ElZilean.Draw.Off"))
            {
                return;
            }

            if (getCheckBoxItem(drawingsMenu, "ElZilean.Draw.Q"))
            {
                if (Q.Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.DodgerBlue);
                }
            }
        }

        /// <summary>
        ///     E Flee to mouse
        /// </summary>
        private static void OnFlee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (E.IsReady() && Player.Mana > getSliderItem(fleeMenu, "ElZilean.Flee.Mana"))
            {
                E.Cast();
            }

            if (!E.IsReady() && W.IsReady())
            {
                if (HasSpeedBuff)
                {
                    return;
                }

                W.Cast();
            }
        }

        /// <summary>
        ///     Harass logic
        /// </summary>
        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (getCheckBoxItem(harassMenu, "ElZilean.Harass.Q") && Q.IsReady() && target.LSIsValidTarget(Q.Range))
            {
                var pred = Q.GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(pred.UnitPosition);
                }
            }

            if (getCheckBoxItem(harassMenu, "ElZilean.Harass.W") && W.IsReady() && !Q.IsReady())
            {
                W.Cast();
            }

            // Check if target has a bomb
            var isBombed =
                HeroManager.Enemies.FirstOrDefault(x => x.HasBuff("ZileanQEnemyBomb") && x.LSIsValidTarget(Q.Range));

            if (isBombed.LSIsValidTarget())
            {
                if (getCheckBoxItem(harassMenu, "ElZilean.Harass.W"))
                {
                    Utility.DelayAction.Add(100, () => W.Cast());
                }
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender == null || !sender.LSIsValidTarget(Q.Range) || !sender.IsEnemy)
            {
                return;
            }

            if (sender.IsValid && args.DangerLevel == Interrupter2.DangerLevel.High && getCheckBoxItem(miscMenu, "ElZilean.Q.Interrupt"))
            {
                if (Q.IsReady() && sender.LSIsValidTarget(Q.Range))
                {
                    var prediction = Q.GetPrediction(sender);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
                Utility.DelayAction.Add(100, () => W.Cast());
            }
        }

        /// <summary>
        ///     The laneclear logic
        /// </summary>
        private static void OnLaneclear()
        {
            var minion = MinionManager.GetMinions(Player.Position, Q.Range + Q.Width);
            if (minion == null)
            {
                return;
            }

            if (Player.ManaPercent < getSliderItem(laneMenu, "ElZilean.laneclear.Mana"))
            {
                return;
            }

            var farmLocation =
                MinionManager.GetBestCircularFarmLocation(
                    MinionManager.GetMinions(Q.Range).Select(x => x.ServerPosition.LSTo2D()).ToList(),
                    Q.Width,
                    Q.Range);

            if (farmLocation.MinionsHit == 0)
            {
                return;
            }

            if (getCheckBoxItem(laneMenu, "ElZilean.laneclear.Q") && getCheckBoxItem(laneMenu, "ElZilean.laneclear.QMouse") && Q.IsReady())
            {
                Q.Cast(Game.CursorPos);
            }

            if (getCheckBoxItem(laneMenu, "ElZilean.laneclear.Q") && Q.IsReady() && !getCheckBoxItem(laneMenu, "ElZilean.laneclear.QMouse")
                && farmLocation.MinionsHit >= 3)
            {
                Q.Cast(farmLocation.Position.To3D());
            }

            if (getCheckBoxItem(laneMenu, "ElZilean.laneclear.W") && W.IsReady())
            {
                W.Cast();
            }
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var hero = sender;
            if (hero == null || !sender.IsAlly || !(sender is AIHeroClient))
            {
                return;
            }

            if (!sender.IsAlly || !getCheckBoxItem(initiatorMenu, $"Initiator{sender.CharData.BaseSkinName}"))
            {
                return;
            }

            var initiatorChampionSpell =
                Spells.FirstOrDefault(x => x.SDataName.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase));

            if (initiatorChampionSpell != null)
            {
                if (args.Start.LSDistance(Player.Position) <= E.Range && args.End.LSDistance(Player.Position) <= E.Range
                    && HeroManager.Enemies.Any(
                        e =>
                        e.LSIsValidTarget(E.Range, false) && !e.IsDead
                        && (e.Position.LSDistance(args.End) < 600f || e.Position.LSDistance(args.Start) < 800f)))
                {
                    if (E.IsReady() && E.IsInRange(hero))
                    {
                        E.CastOnUnit(hero);
                    }
                }
            }
        }
        
        /// <summary>
        ///     Called when the game updates
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                {
                    return;
                }


                if (getCheckBoxItem(comboMenu, "ElZilean.Combo.Focus.Bomb"))
                {
                    var passiveTarget = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget() && x.HasBuff("ZileanQEnemyBomb") && x.LSIsValidTarget(Q.Range + 100));

                    if (passiveTarget != null)
                    {
                        PortAIO.OrbwalkerManager.ForcedTarget(passiveTarget);
                    }
                    else
                    {
                        PortAIO.OrbwalkerManager.ForcedTarget(null);
                    }
                }

                if (PortAIO.OrbwalkerManager.isComboActive)
                {
                    OnCombo();
                }
                if (PortAIO.OrbwalkerManager.isHarassActive)
                {
                    OnHarass();
                }
                if (PortAIO.OrbwalkerManager.isLaneClearActive)
                {
                    OnLaneclear();
                }

                if (getCheckBoxItem(comboMenu, "ElZilean.Ignite"))
                {
                    HandleIgnite();
                }

                if (getKeyBindItem(comboMenu, "ElZilean.DoubleBombMouse"))
                {
                    MouseCombo();
                }

                if (PortAIO.OrbwalkerManager.isFleeActive)
                {
                    OnFlee();
                }

                if (getCheckBoxItem(miscMenu, "ElZilean.E.Slow"))
                {
                    foreach (var slowedAlly in
                        HeroManager.Allies.Where(x => x.HasBuffOfType(BuffType.Slow) && x.LSIsValidTarget(Q.Range, false))
                        )
                    {
                        if (E.IsReady() && E.IsInRange(slowedAlly))
                        {
                            E.CastOnUnit(slowedAlly);
                        }
                    }
                }

                if (getCheckBoxItem(miscMenu, "ElZilean.Q.Stun"))
                {
                    var target =
                        HeroManager.Enemies.FirstOrDefault(
                            h =>
                            h.LSIsValidTarget(Q.Range) && h.HasBuffOfType(BuffType.Slow)
                            || h.HasBuffOfType(BuffType.Knockup) || h.HasBuffOfType(BuffType.Charm)
                            || h.HasBuffOfType(BuffType.Stun));

                    if (target != null)
                    {
                        if (Q.IsReady() && target.LSIsValidTarget(Q.Range))
                        {
                            var prediction = Q.GetPrediction(target);
                            if (prediction.Hitchance >= HitChance.VeryHigh)
                            {
                                Q.Cast(prediction.CastPosition);
                                LeagueSharp.Common.Utility.DelayAction.Add(100, () => W.Cast());
                            }
                        }
                    }
                }

                foreach (var ally in HeroManager.Allies.Where(a => a.LSIsValidTarget(R.Range, false)))
                {
                    if (!getCheckBoxItem(ultMenu, $"R{ally.ChampionName}") || ally.IsRecalling() || ally.IsInvulnerable
                        || !ally.LSIsValidTarget(R.Range, false))
                    {
                        return;
                    }

                    var enemies = ally.LSCountEnemiesInRange(750f);
                    var totalDamage = IncomingDamageManager.GetDamage(ally) * 1.1f;
                    if (ally.HealthPercent <= getSliderItem(ultMenu, "min-health") && !ally.IsDead
                        && enemies >= 1 && ally.LSIsValidTarget(R.Range, false))
                    {
                        if ((int)(totalDamage / ally.Health) > getSliderItem(ultMenu, "min-damage")
                            || ally.HealthPercent < getSliderItem(ultMenu, "min-health"))
                        {
                            if (ally.Buffs.Any(b => b.DisplayName == "judicatorintervention" || b.DisplayName == "undyingrage" || b.DisplayName == "kindredrnodeathbuff" || b.DisplayName == "zhonyasringshield" || b.DisplayName == "willrevive"))
                            {
                                return;
                            }

                            R.Cast(ally);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        /// <summary>
        ///     Represents a spell that an item should be casted on.
        /// </summary>
        public class InitiatorSpell
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the name of the champion.
            /// </summary>
            /// <value>
            ///     The name of the champion.
            /// </value>
            public string ChampionName { get; set; }

            /// <summary>
            ///     Gets or sets the name of the s data.
            /// </summary>
            /// <value>
            ///     The name of the s data.
            /// </value>
            public string SDataName { get; set; }

            #endregion
        }
    }
}