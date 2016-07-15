using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Tristana
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Tristana
    {
        /// <summary>
        ///     Loads Tristana.
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
        ///     Fired when a buff is added.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Obj_AI_BaseBuffAddEventArgs" /> instance containing the event data.</param>
        public static void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe &&
                Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "antigrab"))
            {
                if (args.Buff.Name.Equals("ThreshQ") ||
                    args.Buff.Name.Equals("rocketgrab2"))
                {
                    Vars.W.Cast(GameObjects.Player.ServerPosition.LSExtend(GameObjects.Player.ServerPosition, -Vars.W.Range));
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
            if (Vars.W.IsReady() &&
                args.Sender.IsMelee &&
                args.IsDirectedToPlayer &&
                args.Sender.LSIsValidTarget(Vars.W.Range) &&
                Vars.getCheckBoxItem(Vars.WMenu, "gapcloser"))
            {
                Vars.W.Cast(GameObjects.Player.ServerPosition.LSExtend(args.Sender.ServerPosition, -Vars.W.Range));
            }
        }

        public static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            /// <summary>
            ///     The Target Forcing Logic.
            /// </summary>
            if (args.Target is AIHeroClient)
            {
                if (!GameObjects.EnemyHeroes.Any(
                    t =>
                        t.LSIsValidTarget(Vars.AARange) &&
                        t.HasBuff("TristanaECharge")))
                {
                    Orbwalker.ForcedTarget =(null);
                    return;
                }

                Orbwalker.ForcedTarget =(GameObjects.EnemyHeroes.FirstOrDefault(
                    t =>
                        t.IsValidTarget(Vars.AARange) &&
                        t.HasBuff("TristanaECharge")));
            }
        }
    }
}