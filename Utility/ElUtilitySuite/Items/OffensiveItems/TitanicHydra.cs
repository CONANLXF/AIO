using TargetSelector = PortAIO.TSManager; namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK;
    internal class TitanicHydra : ElUtilitySuite.Items.Item
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
                return (ItemId)3053;
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
                return "Titanic Hydra";
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
            if (getCheckBoxItem(this.Menu, "Titanic Hydraaa"))
            {
                return false;
            }
            return getCheckBoxItem(this.Menu, "Titanic Hydracombo") && this.ComboModeActive && !Orbwalker.CanAutoAttack && (EloBuddy.SDK.Item.HasItem(this.Id) && EloBuddy.SDK.Item.CanUseItem(this.Id));
        }

        public override bool AfterOrb()
        {
            return getCheckBoxItem(this.Menu, "Titanic Hydraaa") && this.ComboModeActive && (EloBuddy.SDK.Item.HasItem(this.Id) && EloBuddy.SDK.Item.CanUseItem(this.Id));
        }

        #endregion
    }
}