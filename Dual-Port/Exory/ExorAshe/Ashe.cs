using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Ashe
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Ashe
    {
        /// <summary>
        ///     Loads Ashe.
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
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);

            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

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
                Logics.BuildingClear(args);
            }
        }

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (Vars.R.IsReady() &&
                args.Sender.IsMelee &&
                args.Sender.IsValidTarget(Vars.R.Range) &&
                args.SkillType == LeagueSharp.Data.Enumerations.GapcloserType.Targeted &&
                Vars.getCheckBoxItem(Vars.RMenu, "gapcloser"))
            {
                if (args.Target.IsMe)
                {
                    Vars.R.Cast(args.Sender.ServerPosition);
                }
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
                !Invulnerable.Check(args.Sender) &&
                args.Sender.LSIsValidTarget(Vars.R.Range) &&
                Vars.getCheckBoxItem(Vars.RMenu, "interrupter"))
            {
                Vars.R.Cast(args.Sender.ServerPosition);
            }
        }
    }
}