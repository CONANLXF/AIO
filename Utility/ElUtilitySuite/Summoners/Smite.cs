using TargetSelector = PortAIO.TSManager; namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = SharpDX.Color;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;    // ReSharper disable once ClassNeverInstantiated.Global
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK;
    public class Smite : IPlugin
    {
        #region Constants

        /// <summary>
        ///     The smite range
        /// </summary>
        public const float SmiteRange = 570f;

        #endregion

        #region Static Fields

        public static Obj_AI_Minion Minion;

        private static readonly string[] SmiteObjects =
            {
                "SRU_Red", "SRU_Blue",
                "SRU_Dragon_Water",  "SRU_Dragon_Fire", "SRU_Dragon_Earth", "SRU_Dragon_Air", "SRU_Dragon_Elder",
                "SRU_Baron", "SRU_Gromp", "SRU_Murkwolf",
                "SRU_Razorbeak", "SRU_RiftHerald",
                "SRU_Krug", "Sru_Crab", "TT_Spiderboss",
                "TT_NGolem", "TT_NWolf", "TT_NWraith"
            };

        #endregion

        #region Public Properties
        
        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive
            =>
                PortAIO.OrbwalkerManager.isComboActive;

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public LeagueSharp.Common.Spell SmiteSpell { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The stage.
        /// </value>
        public int Stage { get; set; }

        #endregion

        #region Properties

        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

            if (smiteSlot == null)
            {
                return;
            }

            var smiteMenu = rootMenu.AddSubMenu("Smite", "Smite");
            {
                smiteMenu.AddGroupLabel("Smite Settings : ");
                smiteMenu.Add("ElSmite.Activated", new KeyBind("Smite Activated", true, KeyBind.BindTypes.PressToggle, 'M'));
                smiteMenu.Add("Smite.Ammo", new CheckBox("Save 1 smite charge"));
                smiteMenu.AddSeparator();
                if (Game.MapId == GameMapId.SummonersRift)
                {
                    smiteMenu.AddGroupLabel("Big Mobs : ");
                    smiteMenu.Add("SRU_Baron", new CheckBox("Baron"));
                    smiteMenu.Add("SRU_Red", new CheckBox("Red buff"));
                    smiteMenu.Add("SRU_Blue", new CheckBox("Blue buff"));
                    smiteMenu.Add("SRU_RiftHerald", new CheckBox("Rift Herald"));
                    smiteMenu.AddSeparator();
                    smiteMenu.AddGroupLabel("Dragons : ");
                    smiteMenu.Add("SRU_Dragon_Air", new CheckBox("Air Dragon"));
                    smiteMenu.Add("SRU_Dragon_Earth", new CheckBox("Earth Dragon"));
                    smiteMenu.Add("SRU_Dragon_Fire", new CheckBox("Fire Dragon"));
                    smiteMenu.Add("SRU_Dragon_Water", new CheckBox("Water Dragon"));
                    smiteMenu.Add("SRU_Dragon_Elder", new CheckBox("Elder Dragon"));
                    smiteMenu.AddSeparator();
                    smiteMenu.AddGroupLabel("Small Mobs : ");
                    smiteMenu.Add("SRU_Gromp", new CheckBox("Gromp", false));
                    smiteMenu.Add("Sru_Crab", new CheckBox("Crab", false));
                    smiteMenu.Add("SRU_Murkwolf", new CheckBox("Wolves", false));
                    smiteMenu.Add("SRU_Krug", new CheckBox("Krug", false));
                    smiteMenu.Add("SRU_Razorbeak", new CheckBox("Chicken camp", false));
                    smiteMenu.AddSeparator();
                }

                if (Game.MapId == GameMapId.TwistedTreeline)
                {
                    smiteMenu.AddGroupLabel("Jungle Mobs : ");
                    smiteMenu.Add("TT_Spiderboss", new CheckBox("Vilemaw Enabled"));
                    smiteMenu.Add("TT_NGolem", new CheckBox("Golem Enabled"));
                    smiteMenu.Add("TT_NWolf", new CheckBox("Wolf Enabled"));
                    smiteMenu.Add("TT_NWraith", new CheckBox("Wraith Enabled"));
                    smiteMenu.AddSeparator();
                }

                // Champion Smite
                smiteMenu.AddGroupLabel("Champion smite");
                smiteMenu.Add("ElSmite.KS.Activated", new CheckBox("Use smite to killsteal"));
                smiteMenu.Add("ElSmite.KS.Combo", new CheckBox("Use smite in combo"));
                smiteMenu.AddSeparator();
                // Drawings
                smiteMenu.AddGroupLabel("Drawings");
                smiteMenu.Add("ElSmite.Draw.Range", new CheckBox("Draw smite Range"));
                smiteMenu.Add("ElSmite.Draw.Text", new CheckBox("Draw smite text"));
                smiteMenu.Add("ElSmite.Draw.Damage", new CheckBox("Draw smite Damage", false));
            }

            this.Menu = smiteMenu;
        }

        public void Load()
        {
            try
            {
                var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

                if (smiteSlot == null)
                {
                    return;
                }

                this.SmiteSpell = new LeagueSharp.Common.Spell(smiteSlot.Slot, SmiteRange, DamageType.True);

                Drawing.OnDraw += this.OnDraw;
                Game.OnUpdate += this.OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        #endregion

        #region Methods

        private void OnDraw(EventArgs args)
        {
            if (this.Player.IsDead)
            {
                return;
            }

            var smiteActive = this.Menu["ElSmite.Activated"].Cast<KeyBind>().CurrentValue;
            var drawSmite = this.Menu["ElSmite.Draw.Range"].Cast<CheckBox>().CurrentValue;
            var drawText = this.Menu["ElSmite.Draw.Text"].Cast<CheckBox>().CurrentValue;
            var playerPos = Drawing.WorldToScreen(this.Player.Position);
            var drawDamage = this.Menu["ElSmite.Draw.Damage"].Cast<CheckBox>().CurrentValue;

            if (smiteActive && this.SmiteSpell != null)
            {
                if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) == SpellState.Ready)
                {
                    Drawing.DrawText(
                        playerPos.X - 70,
                        playerPos.Y + 40,
                        System.Drawing.Color.GhostWhite,
                        "Smite active");
                }

                if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) != SpellState.Ready)
                {
                    Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, System.Drawing.Color.Red, "Smite cooldown");
                }

                if (drawDamage && Math.Abs(this.SmiteDamage()) > float.Epsilon)
                {
                    var minions =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                m =>
                                m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget()
                                && SmiteObjects.Contains(m.CharData.BaseSkinName));

                    foreach (var minion in minions.Where(m => m.IsHPBarRendered))
                    {
                        var hpBarPosition = minion.HPBarPosition;
                        var maxHealth = minion.MaxHealth;
                        var sDamage = this.SmiteDamage();
                        var x = this.SmiteDamage() / maxHealth;
                        int barWidth;

                        switch (minion.CharData.BaseSkinName)
                        {
                            case "SRU_RiftHerald":
                                barWidth = 145;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 17),
                                    new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 30),
                                    2f,
                                    System.Drawing.Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X - 22 + barWidth * x,
                                    hpBarPosition.Y - 5,
                                    System.Drawing.Color.Chartreuse,
                                    sDamage.ToString(CultureInfo.InvariantCulture));
                                break;

                            case "SRU_Dragon_Air":
                            case "SRU_Dragon_Water":
                            case "SRU_Dragon_Fire":
                            case "SRU_Dragon_Elder":
                            case "SRU_Dragon_Earth":
                                barWidth = 145;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 22),
                                    new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 30),
                                    2f,
                                    System.Drawing.Color.Orange);
                                Drawing.DrawText(
                                    hpBarPosition.X - 22 + barWidth * x,
                                    hpBarPosition.Y - 5,
                                    System.Drawing.Color.Chartreuse,
                                    sDamage.ToString(CultureInfo.InvariantCulture));
                                break;

                            case "SRU_Red":
                            case "SRU_Blue":
                                barWidth = 145;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 20),
                                    new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 30),
                                    2f,
                                    System.Drawing.Color.Orange);
                                Drawing.DrawText(
                                    hpBarPosition.X - 22 + barWidth * x,
                                    hpBarPosition.Y - 5,
                                    System.Drawing.Color.Chartreuse,
                                    sDamage.ToString(CultureInfo.InvariantCulture));
                                break;

                            case "SRU_Baron":
                                barWidth = 194;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 18 + barWidth * x, hpBarPosition.Y + 20),
                                    new Vector2(hpBarPosition.X + 18 + barWidth * x, hpBarPosition.Y + 35),
                                    2f,
                                    System.Drawing.Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X - 22 + barWidth * x,
                                    hpBarPosition.Y - 3,
                                    System.Drawing.Color.Chartreuse,
                                    sDamage.ToString(CultureInfo.InvariantCulture));
                                break;

                            case "SRU_Gromp":
                                barWidth = 87;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 11),
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                    2f,
                                    System.Drawing.Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + barWidth * x,
                                    hpBarPosition.Y - 15,
                                    System.Drawing.Color.Chartreuse,
                                    sDamage.ToString(CultureInfo.InvariantCulture));
                                break;

                            case "SRU_Murkwolf":
                                barWidth = 75;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 11),
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                    2f,
                                    System.Drawing.Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + barWidth * x,
                                    hpBarPosition.Y - 15,
                                    System.Drawing.Color.Chartreuse,
                                    sDamage.ToString(CultureInfo.InvariantCulture));
                                break;

                            case "Sru_Crab":
                                barWidth = 61;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 8),
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                    2f,
                                    System.Drawing.Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + barWidth * x,
                                    hpBarPosition.Y - 15,
                                    System.Drawing.Color.Chartreuse,
                                    sDamage.ToString(CultureInfo.InvariantCulture));
                                break;

                            case "SRU_Razorbeak":
                                barWidth = 75;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 11),
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                    2f,
                                    System.Drawing.Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + barWidth * x,
                                    hpBarPosition.Y - 15,
                                    System.Drawing.Color.Chartreuse,
                                    sDamage.ToString(CultureInfo.InvariantCulture));
                                break;

                            case "SRU_Krug":
                                barWidth = 81;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 11),
                                    new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                    2f,
                                    System.Drawing.Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + barWidth * x,
                                    hpBarPosition.Y - 15,
                                    System.Drawing.Color.Chartreuse,
                                    sDamage.ToString(CultureInfo.InvariantCulture));
                                break;
                        }
                    }
                }
            }
            else
            {
                if (drawText && this.SmiteSpell != null)
                {
                    Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, System.Drawing.Color.Red, "Smite not active!");
                }
            }

            var smiteSpell = this.SmiteSpell;
            if (smiteSpell != null)
            {
                if (smiteActive && drawSmite
                    && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) == SpellState.Ready)
                {
                    Render.Circle.DrawCircle(this.Player.Position, SmiteRange, System.Drawing.Color.Green);
                }

                if (drawSmite && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) != SpellState.Ready)
                {
                    Render.Circle.DrawCircle(this.Player.Position, SmiteRange, System.Drawing.Color.Red);
                }
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            if (this.Player.IsDead || this.SmiteSpell == null || !this.Menu["ElSmite.Activated"].Cast<KeyBind>().CurrentValue || !this.SmiteSpell.IsReady())
            {
                return;
            }

            var minion = EntityManager.MinionsAndMonsters.Monsters.FirstOrDefault(o => Vector3.Distance(ObjectManager.Player.Position, o.ServerPosition) <= 950f && o.Team == GameObjectTeam.Neutral && !o.CharData.BaseSkinName.ToLower().Contains("barrel") && !o.CharData.BaseSkinName.ToLower().Contains("mini") && !o.CharData.BaseSkinName.ToLower().Contains("respawn") && SmiteObjects.Contains(o.BaseSkinName) && o.LSIsValidTarget(SmiteRange) && this.Menu[o.CharData.BaseSkinName].Cast<CheckBox>().CurrentValue);
            if (minion != null && this.Player.GetSummonerSpellDamage(minion, LeagueSharp.Common.Damage.SummonerSpell.Smite) > minion.Health)
            {
                this.SmiteSpell.Cast(minion);
            }

            if (this.Menu["Smite.Ammo"].Cast<CheckBox>().CurrentValue && this.Player.GetSpell(this.SmiteSpell.Slot).Ammo == 1)
            {
                return;
            }

            if (this.Player.GetSpell(this.SmiteSpell.Slot).Name.ToLower() == "s5_summonersmiteduel" && this.SmiteSpell.IsReady())
            {
                if (this.Menu["ElSmite.KS.Combo"].Cast<CheckBox>().CurrentValue && this.ComboModeActive)
                {
                    var smiteComboEnemy = EntityManager.Heroes.Enemies.FirstOrDefault(hero => !hero.IsZombie && hero.LSIsValidTarget(500f) && hero.IsVisible && hero.IsHPBarRendered);
                    if (smiteComboEnemy != null)
                    {
                        this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, smiteComboEnemy);
                    }
                }
            }

            if (this.Player.GetSpell(this.SmiteSpell.Slot).Name.ToLower() != "s5_summonersmiteplayerganker")
            {
                return;
            }

            if (this.Menu["ElSmite.KS.Activated"].Cast<CheckBox>().CurrentValue && this.SmiteSpell.IsReady())
            {
                var kSableEnemy = HeroManager.Enemies.FirstOrDefault(hero => !hero.IsZombie && this.SmiteSpell.IsInRange(hero) && hero.LSIsValidTarget(SmiteRange) && this.SmiteDmg() >= hero.Health && hero.IsVisible && hero.IsHPBarRendered);
                if (kSableEnemy != null)
                {
                    this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, kSableEnemy);
                }
            }
        }

        private float SmiteDmg()
        {
            if (this.SmiteSpell.Slot == Extensions.GetSpellSlotFromName(ObjectManager.Player, "s5_summonersmiteduel"))
            {
                var damage = new int[] { 54 + 6 * ObjectManager.Player.Level };
                return EloBuddy.Player.CanUseSpell(this.SmiteSpell.Slot) == SpellState.Ready ? damage.Max() : 0;
            }

            if (this.SmiteSpell.Slot == Extensions.GetSpellSlotFromName(ObjectManager.Player, "s5_summonersmiteplayerganker"))
            {
                var damage = new int[] { 20 + 8 * ObjectManager.Player.Level };
                return EloBuddy.Player.CanUseSpell(this.SmiteSpell.Slot) == SpellState.Ready ? damage.Max() : 0;
            }
            return 0;
        }

        private float SmiteDamage()
        {
            return this.Player.Spellbook.GetSpell(this.SmiteSpell.Slot).State == SpellState.Ready
                       ? (float)this.Player.GetSummonerSpellDamage(Minion, LeagueSharp.Common.Damage.SummonerSpell.Smite)
                       : 0;
        }

        #endregion
    }
}