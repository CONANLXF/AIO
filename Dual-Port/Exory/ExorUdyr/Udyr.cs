using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Udyr
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Udyr
    {
        /// <summary>
        ///     Loads Udyr.
        /// </summary>
        public void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initializes the spells.
            /// </summary>
            Spells.Initialize();

            /// <summary>
            ///     Initializes the methods.
            /// </summary>
            Methods.Initialize();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();
        }
        
        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            if (GameObjects.Player.IsDead ||
                GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Logics.Combo(args);
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Logics.Clear(args);
                Logics.BuildingClear(args);
            }
        }
    }
}