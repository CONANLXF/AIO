using TargetSelector = PortAIO.TSManager; namespace Snitched
{
    using System.Reflection;

    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;    /// <summary>
                                       ///     Handles configuration settings.
                                       /// </summary>
    internal class Config
    {
        #region Static Fields

        /// <summary>
        ///     The instance
        /// </summary>
        private static Config instance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="Config" /> class from being created.
        /// </summary>
        private Config()
        {
            this.Menu = MainMenu.AddMenu("Snitched 3.0", "Snitched3");
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static Config Instance => instance ?? (instance = new Config());

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        public Menu buffMenu, objectiveMenu, ksMenu, miscMenu;

        #endregion

        #region Public Indexers

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public void CreateMenu()
        {
            buffMenu = this.Menu.AddSubMenu("Buff Stealing Settings", "BuffStealSettings");
            AddSpellsToMenu(buffMenu, "BuffSteal");
            buffMenu.Add("StealBlueBuff", new CheckBox("Steal Blue Buff"));
            buffMenu.Add("StealRedBuff", new CheckBox("Steal Red Buff"));
            buffMenu.Add("StealAllyBuffs", new CheckBox("Steal Ally Buffs", false));

            objectiveMenu = this.Menu.AddSubMenu("Objective Stealing Settings", "ObjectiveStealSettings");
            AddSpellsToMenu(objectiveMenu, "ObjectiveSteal");
            objectiveMenu.Add("StealBaron", new CheckBox("Steal Baron"));
            objectiveMenu.Add("StealDragon", new CheckBox("Steal Dragon"));
            objectiveMenu.Add("SmartObjectiveSteal", new CheckBox("Smart Objective Steal"));
            objectiveMenu.Add("StealObjectiveKeyBind", new KeyBind("Steal Objectives", false, KeyBind.BindTypes.HoldActive, 90));

            ksMenu = this.Menu.AddSubMenu("Kill Stealing Settings", "KillStealingSettings");
            ksMenu.AddGroupLabel("KS Champs : ");
            HeroManager.Enemies.ForEach(x => ksMenu.Add("KS" + x.ChampionName, new CheckBox("KS : " + x.ChampionName)));
            AddSpellsToMenu(ksMenu, "KS");
            ksMenu.Add("DontStealOnCombo", new CheckBox("Dont Steal if Combo'ing"));

            miscMenu = this.Menu.AddSubMenu("Miscellaneous Settings", "MiscSettings");
            miscMenu.Add("ETALimit", new Slider("Missile Arrival Time Limit (MS)", 3000, 0, 15000));
            miscMenu.Add("DistanceLimit", new Slider("Distance Limit", 5000, 0, 15000));
            miscMenu.Add("StealFOW", new CheckBox("Steal in FOW", false));
            miscMenu.Add("DrawADPS", new CheckBox("Draw Average DPS"));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the spells to menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <param name="name">The name.</param>
        private static void AddSpellsToMenu(Menu rootMenu, string name)
        {
            SpellLoader.GetUsableSpells().ForEach(x => rootMenu.Add(name + x.Slot, new CheckBox("Use " + x.Slot)));
        }

        #endregion
    }
}