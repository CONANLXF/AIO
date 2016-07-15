using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Cassiopeia
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Cassiopeia
    {
        /// <summary>
        ///     Loads Cassiopeia.
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
        ///     Called when the game updates itself.
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

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Logics.Combo(args);
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Logics.Harass(args);
            }

            if (PortAIO.OrbwalkerManager.isLastHitActive)
            {
                Logics.LastHit(args);
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Logics.Clear(args);
            }
        }

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (!args.Sender.IsMelee ||
                Invulnerable.Check(args.Sender, DamageType.Magical))
            {
                return;
            }

            if (Vars.R.IsReady() &&
                args.Sender.IsValidTarget(Vars.R.Range) &&
                args.Sender.IsFacing(GameObjects.Player) &&
                Vars.getCheckBoxItem(Vars.RMenu, "gapcloser"))
            {
                Vars.R.Cast(args.Start);
            }

            if (Vars.W.IsReady() &&
                args.Sender.IsValidTarget(Vars.W.Range) &&
                GameObjects.Player.Distance(args.End) > 500 &&
                Vars.getCheckBoxItem(Vars.WMenu, "gapcloser"))
            {
                Vars.W.Cast(args.End);
                return;
            }
        }

        /// <summary>
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (Vars.R.IsReady() &&
                args.Sender.IsValidTarget(Vars.R.Range) &&
                args.Sender.IsFacing(GameObjects.Player) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical) &&
                Vars.getCheckBoxItem(Vars.RMenu, "interrupter"))
            {
                Vars.R.Cast(args.Sender.ServerPosition);
            }
        }
    }
}