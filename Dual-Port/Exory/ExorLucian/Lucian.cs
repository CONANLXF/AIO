using System;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.Data.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Lucian
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Lucian
    {
        /// <summary>
        ///     Loads Lucian.
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

            if (GameObjects.Player.HasBuff("LucianR") ||
				GameObjects.Player.HasBuff("LucianPassiveBuff"))
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
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe &&
                !GameObjects.Player.HasBuff("LucianR") &&
                AutoAttack.IsAutoAttack(args.SData.Name))
            {
                if (PortAIO.OrbwalkerManager.isComboActive)
                {
                    Logics.Weaving(sender, args);
                }

                if (PortAIO.OrbwalkerManager.isLaneClearActive)
                {
                    Logics.JungleClear(sender, args);
                    Logics.BuildingClear(sender, args);
                }
            }
        }

        /// <summary>
        ///     Called on animation trigger.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectPlayAnimationEventArgs" /> instance containing the event data.</param>
        public static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe &&
                !PortAIO.OrbwalkerManager.isNoneActive)
            {
                if (args.Animation.Equals("Spell1") ||
                    args.Animation.Equals("Spell2"))
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
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
                args.Sender.IsMelee &&
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                args.SkillType == GapcloserType.Targeted &&
                Vars.getCheckBoxItem(Vars.EMenu, "gapcloser"))
            {
                if (args.Target.IsMe)
                {
                    Vars.E.Cast(GameObjects.Player.ServerPosition.LSExtend(args.Sender.ServerPosition, -475f));
                }
            }
        }

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="Orbwalker.PreAttackArgs" /> instance containing the event data.</param>
        /// 
        public static void Orbwalker_OnPreAttack(LeagueSharp.Common.BeforeAttackArgs args)
        {
            if (GameObjects.Player.HasBuff("LucianR"))
            {
                args.Process = false;
            }
        }
    }
}