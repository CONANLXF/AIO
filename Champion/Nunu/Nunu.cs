using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using EloBuddy.SDK;
using EloBuddy;
using System.Linq;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Nunu
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Nunu
    {
        /// <summary>
        ///     Loads Nunu.
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
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

            /// <summary>
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);

            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            /// <summary>
            ///     Initializes the orbwalkingmodes.
            /// </summary>
            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Logics.Combo(args);
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Logics.Harass(args);
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Logics.Clear(args);
            }

        }
        
        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(LeagueSharp.Common.BeforeAttackArgs args)
        {
            if (PortAIO.OrbwalkerManager.isHarassActive || PortAIO.OrbwalkerManager.isLastHitActive || PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                if (Vars.getCheckBoxItem(Vars.MiscMenu, "support"))
                {
                    if (args.Target is Obj_AI_Minion &&
                        GameObjects.AllyHeroes.Any(a => a.Distance(GameObjects.Player) < 2500))
                    {
                        args.Process = false;
                    }
                }
            }
        }
    }
}