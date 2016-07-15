using System;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Pantheon
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Pantheon
    {
        /// <summary>
        ///     Loads Pantheon.
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

            if (GameObjects.Player.HasBuff("pantheonesound"))
            {
                PortAIO.OrbwalkerManager.SetAttack(false);
                PortAIO.OrbwalkerManager.SetMovement(false);
            }
            else
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
                PortAIO.OrbwalkerManager.SetMovement(true);
            }

            /// <summary>
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);

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
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (Vars.W.IsReady() &&
                args.Sender.LSIsValidTarget(Vars.W.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Physical, false) &&
                Vars.getCheckBoxItem(Vars.WMenu, "interrupter"))
            {
                Vars.W.CastOnUnit(args.Sender);
            }
        }

        public static void Orbwalker_OnPreAttack(LeagueSharp.Common.BeforeAttackArgs args)
        {
            /// <summary>
            ///     Stop attack commands while channeling E.
            /// </summary>
            if (GameObjects.Player.HasBuff("pantheonesound"))
            {
                args.Process = false;
            }
        }

        public static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            /// <summary>
            ///     Stop movement commands while channeling E.
            /// </summary>
            if (GameObjects.Player.HasBuff("pantheonesound"))
            {
                args.Process = false;
            }
        }
    }
}