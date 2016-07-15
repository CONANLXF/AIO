using System;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Akali
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Akali
    {
        /// <summary>
        ///     Loads Akali.
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
                /// <summary>
                ///     Initializes the orbwalkingmodes.
                /// </summary>
                if (PortAIO.OrbwalkerManager.isComboActive)
                {
                    if (AutoAttack.IsAutoAttack(args.SData.Name))
                    {
                        Logics.Weaving(sender, args);
                        return;
                    }
                    else
                    {
                        switch (args.SData.Name)
                        {
                            case "AkaliMota":
                                if (Vars.R.IsReady() &&
                                    Targets.Target.IsValidTarget(Vars.R.Range) &&
                                    !Targets.Target.IsValidTarget(Vars.AARange) &&
                                    Vars.getCheckBoxItem(Vars.RMenu, "combo") &&
                                    Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
                                {
                                    if (!Targets.Target.IsUnderEnemyTurret() ||
                                        !Vars.getCheckBoxItem(Vars.MiscMenu, "safe"))
                                    {
                                        Vars.R.CastOnUnit(Targets.Target);
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }

                if (PortAIO.OrbwalkerManager.isLaneClearActive)
                {
                    Logics.JungleClear(sender, args);
                }
            }
        }
    }
}