using System;
using ExorAIO.Utilities;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;
using System.Linq;
using LeagueSharp.Data;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.MissFortune
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class MissFortune
    {
        /// <summary>
        ///     Loads Miss Fortune.
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
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

            if (GameObjects.Player.HasBuff("missfortunebulletsound"))
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
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (AutoAttack.IsAutoAttack(args.SData.Name))
                {
                    /// <summary>
                    ///     Initializes the orbwalkingmodes.
                    /// </summary>

                    if (PortAIO.OrbwalkerManager.isComboActive)
                    {
                        Logics.Weaving(sender, args);
                    }

                    if (PortAIO.OrbwalkerManager.isLaneClearActive)
                    {
                        Logics.JungleClear(sender, args);
                        Logics.BuildingClear(sender, args);
                    }
           
                    Vars.PassiveTarget = args.Target as AttackableUnit;
                }
                else
                {
                    switch (args.SData.Name)
                    {
                        case "MissFortuneRicochetShot":
                        //case "MissFortuneRicochetShotMissile":
                            Vars.PassiveTarget = args.Target as AttackableUnit;
                            break;

                        default:
                            break;
                    }
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
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.EMenu, "gapcloser"))
            {
                Vars.E.Cast(args.End);
            }
        }

        public static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            /// <summary>
            ///     Stop movement commands while channeling R.
            /// </summary>
            if (GameObjects.Player.HasBuff("missfortunebulletsound"))
            {
                args.Process = false;
            }
        }

        public static void Orbwalker_OnPreAttack(LeagueSharp.Common.BeforeAttackArgs args)
        {
            /// <summary>
            ///     Stop attack commands while channeling R.
            /// </summary>
            if (GameObjects.Player.HasBuff("missfortunebulletsound"))
            {
                args.Process = false;
            }

            /// <summary>
            ///     The Target Switching Logic (Passive Stacks).
            /// </summary>
            if (args.Target is AIHeroClient &&
                args.Target.NetworkId == Vars.PassiveTarget.NetworkId &&
                Vars.getCheckBoxItem(Vars.MiscMenu, "passive"))
            {
                if (Vars.GetRealHealth(args.Target as AIHeroClient) >
                        GameObjects.Player.GetAutoAttackDamage(args.Target as AIHeroClient) * 3)
                {
                    if (!GameObjects.EnemyHeroes.Any(
                        t =>
                            t.IsValidTarget(Vars.AARange) &&
                            t.NetworkId != Vars.PassiveTarget.NetworkId))
                    {
                        PortAIO.OrbwalkerManager.ForcedTarget(null);
                        return;
                    }

                    args.Process = false;
                    PortAIO.OrbwalkerManager.ForcedTarget(GameObjects.EnemyHeroes.Where(
                        t =>
                            t.IsValidTarget(Vars.AARange) &&
                            t.NetworkId != Vars.PassiveTarget.NetworkId).OrderByDescending(o => TargetSelector.GetPriority(o)).First());
                }
            }
        }
    }
}