// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoonDraven.cs" company="ChewyMoon">
//   Copyright (C) 2015 ChewyMoon
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The MoonDraven class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MoonDraven
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;/// <summary>
                       ///     The MoonDraven class.
                       /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    internal class MoonDraven
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the e.
        /// </summary>
        /// <value>
        ///     The e.
        /// </value>
        public LeagueSharp.Common.Spell E { get; set; }

        /// <summary>
        ///     Gets the mana percent.
        /// </summary>
        /// <value>
        ///     The mana percent.
        /// </value>
        public float ManaPercent
        {
            get
            {
                return this.Player.Mana / this.Player.MaxMana * 100;
            }
        }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        public AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets or sets the q.
        /// </summary>
        /// <value>
        ///     The q.
        /// </value>
        public LeagueSharp.Common.Spell Q { get; set; }

        /// <summary>
        ///     Gets the q count.
        /// </summary>
        /// <value>
        ///     The q count.
        /// </value>
        public int QCount
        {
            get
            {
                return (this.Player.HasBuff("dravenspinning") ? 1 : 0)
                       + (this.Player.HasBuff("dravenspinningleft") ? 1 : 0) + this.QReticles.Count;
            }
        }

        /// <summary>
        ///     Gets or sets the q reticles.
        /// </summary>
        /// <value>
        ///     The q reticles.
        /// </value>
        public List<QRecticle> QReticles { get; set; }

        /// <summary>
        ///     Gets or sets the r.
        /// </summary>
        /// <value>
        ///     The r.
        /// </value>
        public LeagueSharp.Common.Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the w.
        /// </summary>
        /// <value>
        ///     The w.
        /// </value>
        public LeagueSharp.Common.Spell W { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the last axe move time.
        /// </summary>
        private int LastAxeMoveTime { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            // Create spells
            this.Q = new LeagueSharp.Common.Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(this.Player));
            this.W = new LeagueSharp.Common.Spell(SpellSlot.W);
            this.E = new LeagueSharp.Common.Spell(SpellSlot.E, 1050);
            this.R = new LeagueSharp.Common.Spell(SpellSlot.R);

            this.E.SetSkillshot(0.25f, 130, 1400, false, SkillshotType.SkillshotLine);
            this.R.SetSkillshot(0.4f, 160, 2000, true, SkillshotType.SkillshotLine);

            this.QReticles = new List<QRecticle>();

            this.CreateMenu();

            //Obj_AI_Base.OnNewPath += this.Obj_AI_Base_OnNewPath;
            GameObject.OnCreate += this.GameObjectOnOnCreate;
            GameObject.OnDelete += this.GameObjectOnOnDelete;
            AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloserOnOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += this.Interrupter2OnOnInterruptableTarget;
            Drawing.OnDraw += this.DrawingOnOnDraw;
            Game.OnUpdate += this.GameOnOnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called on an enemy gapcloser.
        /// </summary>
        /// <param name="gapcloser">The gapcloser.</param>
        private void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!miscMenu["UseEGapcloser"].Cast<CheckBox>().CurrentValue || !this.E.IsReady()
                || !gapcloser.Sender.LSIsValidTarget(this.E.Range))
            {
                return;
            }

            this.E.Cast(gapcloser.Sender);
        }

        /// <summary>
        ///     Catches the axe.
        /// </summary>
        private void CatchAxe()
        {
            var catchOption = axeMenu["AxeMode"].Cast<ComboBox>().CurrentValue;

            if (((catchOption == 0 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) || (catchOption == 1 && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))) || catchOption == 2)
            {
                var bestReticle =
                    this.QReticles.Where(
                        x =>
                        x.Object.Position.LSDistance(Game.CursorPos)
                        < axeMenu["CatchAxeRange"].Cast<Slider>().CurrentValue)
                        .OrderBy(x => x.Position.LSDistance(this.Player.ServerPosition))
                        .ThenBy(x => x.Position.LSDistance(Game.CursorPos))
                        .ThenBy(x => x.ExpireTime)
                        .FirstOrDefault();

                if (bestReticle != null && bestReticle.Object.Position.LSDistance(this.Player.ServerPosition) > 100)
                {
                    var eta = 1000 * (this.Player.LSDistance(bestReticle.Position) / this.Player.MoveSpeed);
                    var expireTime = bestReticle.ExpireTime - Environment.TickCount;

                    if (eta >= expireTime && axeMenu["UseWForQ"].Cast<CheckBox>().CurrentValue)
                    {
                        this.W.Cast();
                    }

                    if (axeMenu["DontCatchUnderTurret"].Cast<CheckBox>().CurrentValue)
                    {
                        // If we're under the turret as well as the axe, catch the axe
                        if (this.Player.UnderTurret(true) && bestReticle.Object.Position.UnderTurret(true))
                        {
                            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, bestReticle.Position);
                            }
                            else
                            {
                                Orbwalker.DisableMovement = false;
                                Orbwalker.DisableAttacking = true;
                                Orbwalker.OrbwalkTo(bestReticle.Position);
                                Orbwalker.DisableMovement = true;
                                Orbwalker.DisableAttacking = false;
                            }
                        }
                        else if (!bestReticle.Position.UnderTurret(true))
                        {
                            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, bestReticle.Position);
                            }
                            else
                            {
                                Orbwalker.DisableMovement = false;
                                Orbwalker.DisableAttacking = true;
                                Orbwalker.OrbwalkTo(bestReticle.Position);
                                Orbwalker.DisableMovement = true;
                                Orbwalker.DisableAttacking = false;
                            }
                        }
                    }
                    else
                    {
                        if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, bestReticle.Position);
                        }
                        else
                        {
                            Orbwalker.DisableMovement = false;
                            Orbwalker.DisableAttacking = true;
                            Orbwalker.OrbwalkTo(bestReticle.Position);
                            Orbwalker.DisableMovement = true;
                            Orbwalker.DisableAttacking = false;
                        }
                    }
                }
                else
                {
                    //Orbwalker.OrbwalkTo(Game.CursorPos);
                }
            }
            else
            {
                //Orbwalker.OrbwalkTo(Game.CursorPos);
            }
        }

        /// <summary>
        ///     Does the combo.
        /// </summary>
        private void Combo()
        {
            var target = TargetSelector.GetTarget(this.E.Range, DamageType.Physical);

            if (!target.LSIsValidTarget() || target == null)
            {
                return;
            }

            var useQ = comboMenu["UseQCombo"].Cast<CheckBox>().CurrentValue;
            var useW = comboMenu["UseWCombo"].Cast<CheckBox>().CurrentValue;
            var useE = comboMenu["UseECombo"].Cast<CheckBox>().CurrentValue;
            var useR = comboMenu["UseRCombo"].Cast<CheckBox>().CurrentValue;

            if (useQ && this.QCount < axeMenu["MaxAxes"].Cast<Slider>().CurrentValue - 1 && this.Q.IsReady()
                && ObjectManager.Player.LSDistance(target) < ObjectManager.Player.GetAutoAttackRange() && !this.Player.Spellbook.IsAutoAttacking)
            {
                this.Q.Cast();
            }

            if (useW && this.W.IsReady()
                && this.ManaPercent > miscMenu["UseWManaPercent"].Cast<Slider>().CurrentValue)
            {
                if (miscMenu["UseWSetting"].Cast<CheckBox>().CurrentValue)
                {
                    this.W.Cast();
                }
                else
                {
                    if (!this.Player.HasBuff("dravenfurybuff"))
                    {
                        this.W.Cast();
                    }
                }
            }

            if (useE && this.E.IsReady())
            {
                this.E.Cast(target);
            }

            if (!useR || !this.R.IsReady())
            {
                return;
            }

            // Patented Advanced Algorithms D321987
            var killableTarget =
                HeroManager.Enemies.Where(x => x.LSIsValidTarget(2000))
                    .FirstOrDefault(
                        x =>
                        this.Player.LSGetSpellDamage(x, SpellSlot.R) * 2 > x.Health
                        && (!(ObjectManager.Player.LSDistance(x) < ObjectManager.Player.GetAutoAttackRange()) || this.Player.LSCountEnemiesInRange(this.E.Range) > 2));

            if (killableTarget != null)
            {
                this.R.Cast(killableTarget);
            }
        }

        public static Menu comboMenu, harassMenu, laneClearMenu, axeMenu, drawMenu, miscMenu;

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            this.Menu = MainMenu.AddMenu("MoonDraven", "cmMoonDraven");

            // Combo
            comboMenu = this.Menu.AddSubMenu("Combo", "combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseECombo", new CheckBox("Use E"));
            comboMenu.Add("UseRCombo", new CheckBox("Use R"));

            // Harass
            harassMenu = this.Menu.AddSubMenu("Harass", "harass");
            harassMenu.Add("UseEHarass", new CheckBox("Use E"));
            harassMenu.Add("UseHarassToggle", new KeyBind("Harass! (Toggle)", false, KeyBind.BindTypes.PressToggle, 84));

            // Lane Clear
            laneClearMenu = this.Menu.AddSubMenu("Wave Clear", "waveclear");
            laneClearMenu.Add("UseQWaveClear", new CheckBox("Use Q"));
            laneClearMenu.Add("UseWWaveClear", new CheckBox("Use W"));
            laneClearMenu.Add("UseEWaveClear", new CheckBox("Use E", false));
            laneClearMenu.Add("WaveClearManaPercent", new Slider("Mana Percent", 50));

            // Axe Menu
            axeMenu = this.Menu.AddSubMenu("Axe Settings", "axeSetting");
            axeMenu.Add("AxeMode", new ComboBox("Catch Axe on Mode:", 2, "Combo", "Any", "Always"));
            axeMenu.Add("CatchAxeRange", new Slider("Catch Axe Range", 800, 120, 1500));
            axeMenu.Add("MaxAxes", new Slider("Maximum Axes", 2, 1, 3));
            axeMenu.Add("UseWForQ", new CheckBox("Use W if Axe too far"));
            axeMenu.Add("DontCatchUnderTurret", new CheckBox("Don't Catch Axe Under Turret"));

            // Drawing
            drawMenu = this.Menu.AddSubMenu("Drawing", "draw");
            drawMenu.Add("DrawE", new CheckBox("Draw E"));
            drawMenu.Add("DrawAxeLocation", new CheckBox("Draw Axe Location"));
            drawMenu.Add("DrawAxeRange", new CheckBox("Draw Axe Catch Range"));

            // Misc Menu
            miscMenu = this.Menu.AddSubMenu("Misc", "misc");
            miscMenu.Add("UseWSetting", new CheckBox("Use W Instantly(When Available)", false));
            miscMenu.Add("UseEGapcloser", new CheckBox("Use E on Gapcloser"));
            miscMenu.Add("UseEInterrupt", new CheckBox("Use E to Interrupt"));
            miscMenu.Add("UseWManaPercent", new Slider("Use W Mana Percent", 50));
            miscMenu.Add("UseWSlow", new CheckBox("Use W if Slowed"));
        }

        /// <summary>
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void DrawingOnOnDraw(EventArgs args)
        {
            var drawE = drawMenu["DrawE"].Cast<CheckBox>().CurrentValue;
            var drawAxeLocation = drawMenu["DrawAxeLocation"].Cast<CheckBox>().CurrentValue;
            var drawAxeRange = drawMenu["DrawAxeRange"].Cast<CheckBox>().CurrentValue;

            if (drawE)
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    this.E.Range,
                    this.E.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawAxeLocation)
            {
                var bestAxe =
                    this.QReticles.Where(
                        x =>
                        x.Position.LSDistance(Game.CursorPos) < axeMenu["CatchAxeRange"].Cast<Slider>().CurrentValue)
                        .OrderBy(x => x.Position.LSDistance(this.Player.ServerPosition))
                        .ThenBy(x => x.Position.LSDistance(Game.CursorPos))
                        .FirstOrDefault();

                if (bestAxe != null)
                {
                    Render.Circle.DrawCircle(bestAxe.Position, 120, Color.LimeGreen);
                }

                foreach (var axe in
                    this.QReticles.Where(x => x.Object.NetworkId != (bestAxe == null ? 0 : bestAxe.Object.NetworkId)))
                {
                    Render.Circle.DrawCircle(axe.Position, 120, Color.Yellow);
                }
            }

            if (drawAxeRange)
            {
                Render.Circle.DrawCircle(
                    Game.CursorPos,
                    axeMenu["CatchAxeRange"].Cast<Slider>().CurrentValue,
                    Color.DodgerBlue);
            }
        }

        /// <summary>
        ///     Called when a game object is created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("Draven_Base_Q_reticle_self.troy"))
            {
                return;
            }

            this.QReticles.Add(new QRecticle(sender, Environment.TickCount + 1800));
            LeagueSharp.Common.Utility.DelayAction.Add(1800, () => this.QReticles.RemoveAll(x => x.Object.NetworkId == sender.NetworkId));
        }

        /// <summary>
        ///     Called when a game object is deleted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void GameObjectOnOnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("Draven_Base_Q_reticle_self.troy"))
            {
                return;
            }

            this.QReticles.RemoveAll(x => x.Object.NetworkId == sender.NetworkId);
        }

        /// <summary>
        ///     Called when the game updates.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void GameOnOnUpdate(EventArgs args)
        {
            this.QReticles.RemoveAll(x => x.Object.IsDead);

            this.CatchAxe();

            if (this.W.IsReady() && miscMenu["UseWSlow"].Cast<CheckBox>().CurrentValue && this.Player.HasBuffOfType(BuffType.Slow))
            {
                this.W.Cast();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                this.Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                this.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                this.LaneClear();
            }

            if (harassMenu["UseHarassToggle"].Cast<KeyBind>().CurrentValue)
            {
                this.Harass();
            }
        }

        /// <summary>
        ///     Harasses the enemy.
        /// </summary>
        private void Harass()
        {
            var target = TargetSelector.GetTarget(this.E.Range, DamageType.Physical);

            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (harassMenu["UseEHarass"].Cast<CheckBox>().CurrentValue && this.E.IsReady())
            {
                this.E.Cast(target);
            }
        }

        /// <summary>
        ///     Interrupts an interruptable target.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Interrupter2.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        private void Interrupter2OnOnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!miscMenu["UseEInterrupt"].Cast<CheckBox>().CurrentValue || !this.E.IsReady() || !sender.LSIsValidTarget(this.E.Range))
            {
                return;
            }

            if (args.DangerLevel == Interrupter2.DangerLevel.Medium || args.DangerLevel == Interrupter2.DangerLevel.High)
            {
                this.E.Cast(sender);
            }
        }

        /// <summary>
        ///     Clears the lane of minions.
        /// </summary>
        private void LaneClear()
        {
            var useQ = laneClearMenu["UseQWaveClear"].Cast<CheckBox>().CurrentValue;
            var useW = laneClearMenu["UseWWaveClear"].Cast<CheckBox>().CurrentValue;
            var useE = laneClearMenu["UseEWaveClear"].Cast<CheckBox>().CurrentValue;

            if (this.ManaPercent < laneClearMenu["WaveClearManaPercent"].Cast<Slider>().CurrentValue)
            {
                return;
            }

            if (useQ && this.QCount < axeMenu["MaxAxes"].Cast<Slider>().CurrentValue - 1 && this.Q.IsReady()
                && Orbwalker.LastTarget is Obj_AI_Minion && !this.Player.Spellbook.IsAutoAttacking
                && !Orbwalker.IsAutoAttacking)
            {
                this.Q.Cast();
            }

            if (useW && this.W.IsReady()
                && this.ManaPercent > miscMenu["UseWManaPercent"].Cast<Slider>().CurrentValue)
            {
                if (miscMenu["UseWSetting"].Cast<CheckBox>().CurrentValue)
                {
                    this.W.Cast();
                }
                else
                {
                    if (!this.Player.HasBuff("dravenfurybuff"))
                    {
                        this.W.Cast();
                    }
                }
            }

            if (!useE || !this.E.IsReady())
            {
                return;
            }

            var bestLocation = this.E.GetLineFarmLocation(MinionManager.GetMinions(this.E.Range));

            if (bestLocation.MinionsHit > 1)
            {
                this.E.Cast(bestLocation.Position);
            }
        }

        /// <summary>
        ///     Fired when the OnNewPath event is called.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectNewPathEventArgs" /> instance containing the event data.</param>
        private void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            this.CatchAxe();
        }

        #endregion

        /// <summary>
        ///     A represenation of a Q circle on Draven.
        /// </summary>
        internal class QRecticle
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="QRecticle" /> class.
            /// </summary>
            /// <param name="rectice">The rectice.</param>
            /// <param name="expireTime">The expire time.</param>
            public QRecticle(GameObject rectice, int expireTime)
            {
                this.Object = rectice;
                this.ExpireTime = expireTime;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the expire time.
            /// </summary>
            /// <value>
            ///     The expire time.
            /// </value>
            public int ExpireTime { get; set; }

            /// <summary>
            ///     Gets or sets the object.
            /// </summary>
            /// <value>
            ///     The object.
            /// </value>
            public GameObject Object { get; set; }

            /// <summary>
            ///     Gets the position.
            /// </summary>
            /// <value>
            ///     The position.
            /// </value>
            public Vector3 Position
            {
                get
                {
                    return this.Object.Position;
                }
            }

            #endregion
        }
    }
}