using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy.SDK;
using EloBuddy;

 namespace ExorAIO.Champions.Karma
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Karma
    {
        /// <summary>
        ///     Loads Lux.
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
        ///     Called when a <see cref="AttackableUnit" /> takes/gives damage.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AttackableUnitDamageEventArgs" /> instance containing the event data.</param>
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender as AIHeroClient == null &&
                sender as Obj_AI_Turret == null &&
                !Targets.JungleMinions.Contains(sender as Obj_AI_Minion))
            {
                return;
            }

            if (sender.IsAlly ||
                args.Target as AIHeroClient == null ||
                !(args.Target as AIHeroClient).IsAlly)
            {
                return;
            }

            if (Vars.E.IsReady() &&
                (args.Target as AIHeroClient).IsValidTarget(Vars.E.Range, false) &&
                Vars.getCheckBoxItem(Vars.EMenu, "logical") &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, (args.Target as AIHeroClient).ChampionName.ToLower()))
            {
                Vars.E.CastOnUnit(args.Target as AIHeroClient);
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
                GameObjects.Player.Distance(args.End) < 750 &&
                Vars.getCheckBoxItem(Vars.EMenu, "gapcloser"))
            {
                if (Vars.R.IsReady() &&
                    Vars.getCheckBoxItem(Vars.RMenu, "empe") &&
                    GameObjects.AllyHeroes.Count(a => a.IsValidTarget(600f, false)) >= 2)
                {
                    Vars.R.Cast();
                }

                Vars.E.Cast();
            }
        }

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                /// <summary>
                ///     The 'Support Mode' Logic.
                /// </summary>
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