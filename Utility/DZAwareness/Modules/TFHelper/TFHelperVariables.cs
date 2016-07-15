using System;
using System.Collections.Generic;
using System.Linq;
using DZAwarenessAIO.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace DZAwarenessAIO.Modules.TFHelper
{
    class TFHelperVariables
    {
        /// <summary>
        /// Gets the enemies close.
        /// </summary>
        /// <value>
        /// The enemies close.
        /// </value>
        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    HeroManager.Enemies.Where(
                        m =>
                            m.LSDistance(ObjectManager.Player, true) <= Math.Pow(TFRange, 2) && m.LSIsValidTarget(TFRange, false) &&
                            m.LSCountEnemiesInRange(m.IsMelee() ? m.AttackRange * 1.5f : m.AttackRange + 20 * 1.5f) > 0);
            }
        }

        /// <summary>
        /// The teamfight calculation range
        /// </summary>
        public static int TFRange => TFHelperBase.moduleMenu["dz191.dza.tf.range"].Cast<Slider>().CurrentValue;

        /// <summary>
        /// Gets the allies close.
        /// </summary>
        /// <value>
        /// The allies close.
        /// </value>
        public static IEnumerable<AIHeroClient> AlliesClose
        {
            get
            {
                return
                    HeroManager.Allies.Where(
                        m => m.LSDistance(ObjectManager.Player, true) <= Math.Pow(TFRange, 2) && m.LSIsValidTarget(TFRange, false));
            }
        }

        /// <summary>
        /// Gets or sets the ally bar sprite.
        /// </summary>
        /// <value>
        /// The ally bar sprite.
        /// </value>
        public static Render.Sprite AllyBarSprite { get; set; }

        /// <summary>
        /// Gets or sets the enemy bar sprite.
        /// </summary>
        /// <value>
        /// The enemy bar sprite.
        /// </value>
        public static Render.Sprite EnemyBarSprite { get; set; }

        /// <summary>
        /// Gets or sets the ally strength % text.
        /// </summary>
        /// <value>
        /// The ally strength % text.
        /// </value>
        public static Render.Text AllyStrengthText { get; set; }

        /// <summary>
        /// Gets or sets the enemy strength % text.
        /// </summary>
        /// <value>
        /// The enemy strength % text.
        /// </value>
        public static Render.Text EnemyStrengthText { get; set; }

        /// <summary>
        /// Gets or sets the teams info text.
        /// </summary>
        /// <value>
        /// The teams info text.
        /// </value>
        public static Render.Text TeamsVSText { get; set; }
    }
}
