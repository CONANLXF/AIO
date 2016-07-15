using System;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Jhin
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Jhin
    {
        /// <summary>
        ///     Loads Jhin.
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

            /// <summary>
            ///     Initializes the cone drawings.
            /// </summary>
            ConeDrawings.Initialize();
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

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Logics.Combo(args);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Logics.Harass(args);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                Logics.LastHit(args);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Logics.Clear(args);
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe &&
                AutoAttack.IsAutoAttack(args.SData.Name))
            {
                /// <summary>
                ///     Initializes the orbwalkingmodes.
                /// </summary>
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Logics.Weaving(sender, args);
                }
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
                !Invulnerable.Check(args.Sender) &&
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                Vars.getCheckBoxItem(Vars.EMenu, "gapcloser"))
            {
                Vars.E.Cast(args.End);
            }
        }

        /// <summary>
        ///     Handles the <see cref="E:ProcessSpell" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name.Equals("JhinR"))
                {
                    Vars.ShotsCount = 0;
                    Vars.End = args.End;
                }
                else if (args.SData.Name.Equals("JhinRShot"))
                {
                    Vars.ShotsCount++;
                }
            }
        }

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (Vars.R.Instance.Name.Equals("JhinRShot") && sender.IsMe)
            {
                args.Process = false;
            }
        }
    }
}