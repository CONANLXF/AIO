using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.KogMaw
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class KogMaw
    {
        /// <summary>
        ///     Loads Kog'Maw.
        /// </summary>
        public void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

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
            ///     Updates the spells.
            /// </summary>
            Spells.Initialize();

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
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Logics.Combo(args);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Logics.Harass(args);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
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
            if (Vars.Q.IsReady() &&
                args.IsDirectedToPlayer &&
                !Invulnerable.Check(args.Sender) &&
                args.Sender.LSIsValidTarget(Vars.Q.Range) &&
                Vars.getCheckBoxItem(Vars.QMenu, "gapcloser"))
            {
                Vars.Q.Cast(args.End);
            }

            if (Vars.E.IsReady() &&
                !Invulnerable.Check(args.Sender) &&
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                Vars.getCheckBoxItem(Vars.EMenu, "gapcloser"))
            {
                Vars.E.Cast(args.End);
            }
        }
    }
}