using TargetSelector = PortAIO.TSManager; namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;
    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    public class Barrier : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the barrier spell.
        /// </summary>
        /// <value>
        ///     The barrier spell.
        /// </value>
        public Spell BarrierSpell { get; set; }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerbarrier") == SpellSlot.Unknown)
            {
                return;
            }

            var barrierMenu = rootMenu.AddSubMenu("Barrier", "Barrier");
            {
                barrierMenu.Add("Barrier.Activated", new CheckBox("Barrier activated"));
                barrierMenu.Add("barrier.min-health", new Slider("Health percentage", 20, 1));
                barrierMenu.Add("barrier.min-damage", new Slider("Heal on % incoming damage", 20, 1));
            }

            this.Menu = barrierMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            var barrierSlot = this.Player.GetSpellSlot("summonerbarrier");

            if (barrierSlot == SpellSlot.Unknown)
            {
                return;
            }

            IncomingDamageManager.RemoveDelay = 500;
            IncomingDamageManager.Skillshots = true;
            this.BarrierSpell = new Spell(barrierSlot, 550);

            Game.OnUpdate += OnUpdate;
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

        #region Methods

        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead || !this.BarrierSpell.IsReady() || this.Player.HasBuff("ChronoShift") || this.Player.InFountain() || this.Player.LSIsRecalling() || !getCheckBoxItem(this.Menu, "Barrier.Activated"))
                {
                    return;
                }

                var enemies = this.Player.LSCountEnemiesInRange(750f);
                var totalDamage = IncomingDamageManager.GetDamage(this.Player) * 1.1f;

                if (this.Player.HealthPercent <= getSliderItem(this.Menu, "barrier.min-health") &&
                    this.BarrierSpell.IsInRange(this.Player) && enemies >= 1)
                {
                    if ((int)(totalDamage / this.Player.Health) > getSliderItem(this.Menu, "barrier.min-damage")
                        || this.Player.HealthPercent < getSliderItem(this.Menu, "barrier.min-health"))
                    {
                        this.Player.Spellbook.CastSpell(this.BarrierSpell.Slot);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}
