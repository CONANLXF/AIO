using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Veigar
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Veigar
    {
        /// <summary>
        ///     Loads Veigar.
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
            if (Vars.E.IsReady() &&
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.EMenu, "gapcloser"))
            {
                Vars.E.Cast(args.Sender.ServerPosition);
            }
        }

        /// <summary>
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (Vars.E.IsReady() &&
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.EMenu, "interrupter"))
            {
                Vars.E.Cast(args.Sender.ServerPosition);
            }
        }

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(LeagueSharp.Common.BeforeAttackArgs args)
        {
            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                /// <summary>
                ///     The 'No AA in Combo' Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.MiscMenu, "noaacombo"))
                {
                    if (Vars.Q.IsReady() ||
                        Vars.W.IsReady() ||
                        Vars.E.IsReady() ||
                        !Bools.HasSheenBuff() ||
                        GameObjects.Player.ManaPercent > 10)
                    {
                        args.Process = false;
                    }
                }
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive || PortAIO.OrbwalkerManager.isLastHitActive)
            {
                /// <summary>
                ///     The 'No AA if Q Ready' Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.MiscMenu, "qfarmmode"))
                {
                    if (Vars.Q.IsReady())
                    {
                        args.Process = false;
                    }
                }

                /// <summary>
                ///     The 'Support Mode' Logic.
                /// </summary>
                else if (Vars.getCheckBoxItem(Vars.MiscMenu, "support"))
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