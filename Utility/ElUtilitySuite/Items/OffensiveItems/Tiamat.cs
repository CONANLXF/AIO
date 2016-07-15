using TargetSelector = PortAIO.TSManager; namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    internal class Tiamat : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id
        {
            get
            {
                return ItemId.Tiamat_Melee_Only;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name
        {
            get
            {
                return "Tiamat";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            if (getCheckBoxItem(this.Menu, "Tiamataa"))
            {
                return false;
            }
            return getCheckBoxItem(this.Menu, "Tiamatcombo") && this.ComboModeActive
                   && HeroManager.Enemies.Any(x => x.LSDistance(this.Player) < 400 && !x.IsDead && !x.IsZombie) && (EloBuddy.SDK.Item.HasItem(this.Id) && EloBuddy.SDK.Item.CanUseItem(this.Id));
        }

        public override bool AfterOrb()
        {
            return getCheckBoxItem(this.Menu, "Tiamataa") && this.ComboModeActive && HeroManager.Enemies.Any(x => x.LSDistance(this.Player) < 400 && !x.IsDead && !x.IsZombie) && (EloBuddy.SDK.Item.HasItem(this.Id) && EloBuddy.SDK.Item.CanUseItem(this.Id));
        }

        #endregion
    }
}