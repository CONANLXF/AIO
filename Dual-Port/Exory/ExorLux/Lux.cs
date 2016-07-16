using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Lux
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Lux
    {
        /// <summary>
        ///     Defines the missile object for the E.
        /// </summary>
        public static GameObject EMissile = null;

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
        ///     Called when an object gets created by the game.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                /// <summary>
                ///     Defines the missile object for the E.
                /// </summary>
                if (obj.Name.Contains("Lux_Base_E_tar"))
                {
                    EMissile = obj;
                }
            }
        }

        /// <summary>
        ///     Called when an object gets deleted by the game.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                /// <summary>
                ///     Removes the missile object for the E.
                /// </summary>
                if (obj.Name.Contains("Lux_Base_E_tar"))
                {
                    EMissile = null;
                }
            }
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

            if (Vars.W.IsReady() &&
                (args.Target as AIHeroClient).LSIsValidTarget(Vars.W.Range, false) &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical") &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, (args.Target as AIHeroClient).ChampionName.ToLower()))
            {
                Vars.W.Cast(Vars.W.GetPrediction(args.Target as AIHeroClient).UnitPosition);
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
                args.Sender.LSIsValidTarget(Vars.Q.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.QMenu, "gapcloser"))
            {
                Vars.Q.Cast(args.Sender.ServerPosition);
            }
        }

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {

            /// <summary>
            ///     The Target Forcing Logic.
            /// </summary>
            if (args.Target is AIHeroClient)
            {
                if (!GameObjects.EnemyHeroes.Any(
                    t =>
                        t.LSIsValidTarget(Vars.AARange) &&
                        t.HasBuff("luxilluminatingfraulein")))
                {
                    Orbwalker.ForcedTarget =(null);
                    return;
                }

                Orbwalker.ForcedTarget =(GameObjects.EnemyHeroes.FirstOrDefault(
                    t =>
                        t.LSIsValidTarget(Vars.AARange) &&
                        t.HasBuff("luxilluminatingfraulein")));
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
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