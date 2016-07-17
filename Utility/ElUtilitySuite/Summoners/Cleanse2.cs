namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    public class Cleanse2// : IPlugin
    {
        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets the buff indexes handled.
        /// </summary>
        /// <value>
        ///     The buff indexes handled.
        /// </value>
        private Dictionary<int, List<int>> BuffIndexesHandled { get; } = new Dictionary<int, List<int>>();

        /// <summary>
        ///     Gets or sets the buffs to cleanse.
        /// </summary>
        /// <value>
        ///     The buffs to cleanse.
        /// </value>
        private IEnumerable<BuffType> BuffsToCleanse { get; set; }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<CleanseItem> Items { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the random.
        /// </summary>
        /// <value>
        ///     The random.
        /// </value>
        private Random Random { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        public void CreateMenu(Menu rootMenu)
        {
            this.CreateItems();
            this.BuffsToCleanse = this.Items.SelectMany(x => x.WorksOn).Distinct();

            this.Menu = rootMenu.AddSubMenu("Cleanse RELOADED", "BuffTypeStyleCleanser");

            this.Menu.Add("MinDuration", new Slider("Minimum Duration (MS)", 500, 0, 25000));
            this.Menu.Add("CleanseEnabled", new CheckBox("Enabled"));

            this.Menu.AddGroupLabel("Humanizer Settings");
            this.Menu.Add("MinHumanizerDelay", new Slider("Min Humanizer Delay (MS)", 100, 0, 500));
            this.Menu.Add("MaxHumanizerDelay", new Slider("Max Humanizer Delay (MS)", 150, 0, 500));
            this.Menu.Add("HumanizerEnabled", new CheckBox("Enabled", false));

            this.Menu.AddGroupLabel("Buff Types");
            foreach (var buffType in this.BuffsToCleanse.Select(x => x.ToString()))
            {
                this.Menu.Add($"Cleanse{buffType}", new CheckBox(buffType));
            }
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Random = new Random(Environment.TickCount);
            HeroManager.Allies.ForEach(x => this.BuffIndexesHandled.Add(x.NetworkId, new List<int>()));

            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the items.
        /// </summary>
        private void CreateItems()
        {
            this.Items = new List<CleanseItem>
                             {
                                 new CleanseItem
                                     {
                                         Slot =
                                             () =>
                                             Player.GetSpellSlot("summonerboost") == SpellSlot.Unknown
                                                 ? SpellSlot.Unknown
                                                 : Player.GetSpellSlot("summonerboost"),
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                     BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                     BuffType.Taunt, BuffType.Damage
                                                 },
                                         Priority = 2
                                     },
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Quicksilver_Sash.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             },
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee,
                                                     BuffType.Slow, BuffType.Polymorph, BuffType.Silence,
                                                     BuffType.Snare, BuffType.Stun, BuffType.Taunt,
                                                     BuffType.Damage, BuffType.CombatEnchancer
                                                 },
                                         Priority = 0
                                     },
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Dervish_Blade.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             },
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee,
                                                     BuffType.Slow, BuffType.Polymorph, BuffType.Silence,
                                                     BuffType.Snare, BuffType.Stun, BuffType.Taunt,
                                                     BuffType.Damage, BuffType.CombatEnchancer
                                                 },
                                         Priority = 0
                                     },
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Mercurial_Scimitar.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             },
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee,
                                                     BuffType.Slow, BuffType.Polymorph, BuffType.Silence,
                                                     BuffType.Snare, BuffType.Stun, BuffType.Taunt,
                                                     BuffType.Damage, BuffType.CombatEnchancer
                                                 },
                                         Priority = 0
                                     },
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Mikaels_Crucible.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             },
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Stun, BuffType.Snare, BuffType.Taunt,
                                                     BuffType.Silence, BuffType.Slow, BuffType.CombatEnchancer,
                                                     BuffType.Fear
                                                 },
                                         WorksOnAllies = true, Priority = 1
                                     }
                             };
        }

        /// <summary>
        ///     Gets the best cleanse item.
        /// </summary>
        /// <param name="ally">The ally.</param>
        /// <param name="buff">The buff.</param>
        /// <returns></returns>
        private Spell GetBestCleanseItem(GameObject ally, BuffInstance buff)
        {
            return
                this.Items.OrderBy(x => x.Priority)
                    .Where(
                        x =>
                        x.WorksOn.Any(y => buff.Type.HasFlag(y)) && (ally.IsMe || x.WorksOnAllies) && x.Spell.IsReady()
                        && x.Spell.IsInRange(ally) && x.Spell.Slot != SpellSlot.Unknown)
                    .Select(x => x.Spell)
                    .FirstOrDefault();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!this.Menu["CleanseEnabled"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            foreach (var ally in HeroManager.Allies)
            {
                foreach (var buff in ally.Buffs.Where(x => this.BuffsToCleanse.Contains(x.Type)))
                {
                    if (!this.Menu[$"Cleanse{buff.Type}"].Cast<CheckBox>().CurrentValue
                        || this.Menu["MinDuration"].Cast<Slider>().CurrentValue / 1000f
                        > buff.EndTime - buff.StartTime || this.BuffIndexesHandled[ally.NetworkId].Contains(buff.Index))
                    {
                        continue;
                    }

                    var cleanseItem = this.GetBestCleanseItem(ally, buff);

                    if (cleanseItem == null)
                    {
                        continue;
                    }

                    this.BuffIndexesHandled[ally.NetworkId].Add(buff.Index);

                    if (this.Menu["HumanizerEnabled"].Cast<CheckBox>().CurrentValue)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            (int)
                            Math.Min(
                                this.Random.Next(
                                    this.Menu["MinHumanizerDelay"].Cast<Slider>().CurrentValue,
                                    this.Menu["MaxHumanizerDelay"].Cast<Slider>().CurrentValue),
                                (buff.StartTime - buff.EndTime) * 1000),
                            () =>
                            {
                                cleanseItem.Cast(ally);
                                this.BuffIndexesHandled[ally.NetworkId].Remove(buff.Index);
                            });
                    }
                    else
                    {
                        cleanseItem.Cast(ally);
                    }

                    return;
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     An item/spell that can be used to cleanse a spell.
    /// </summary>
    public class CleanseItem
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CleanseItem" /> class.
        /// </summary>
        public CleanseItem()
        {
            this.Range = float.MaxValue;
            this.WorksOnAllies = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the priority.
        /// </summary>
        /// <value>
        ///     The priority.
        /// </value>
        public int Priority { get; set; }

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        /// <value>
        ///     The range.
        /// </value>
        public float Range { get; set; }

        /// <summary>
        ///     Gets or sets the slot delegate.
        /// </summary>
        /// <value>
        ///     The slot delegate.
        /// </value>
        public Func<SpellSlot> Slot { get; set; }

        /// <summary>
        ///     Gets or sets the spell.
        /// </summary>
        /// <value>
        ///     The spell.
        /// </value>
        public Spell Spell => new Spell(this.Slot(), this.Range);

        /// <summary>
        ///     Gets or sets what the spell works on.
        /// </summary>
        /// <value>
        ///     The buff types the spell works on.
        /// </value>
        public BuffType[] WorksOn { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the spell works on allies.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the spell works on allies; otherwise, <c>false</c>.
        /// </value>
        public bool WorksOnAllies { get; set; }

        #endregion
    }
}