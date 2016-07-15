using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.Data.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Kalista
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Kalista
    {
        /// <summary>
        ///     Loads Kalista.
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
            ///     Initializes the damage drawings.
            /// </summary>
            Healthbars.Initialize();
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

            if (GameObjects.Player.Spellbook.IsAutoAttacking ||
                GameObjects.Player.IsDashing())
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

        public static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            /// <summary>
            ///     The E against Non-Killable Minions Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Bools.IsPerfectRendTarget(target as Obj_AI_Minion) &&
                Vars.GetRealHealth(target as Obj_AI_Minion) <
                    (float)GameObjects.Player.LSGetSpellDamage(target as Obj_AI_Minion, SpellSlot.E) +
                    (float)GameObjects.Player.LSGetSpellDamage(target as Obj_AI_Minion, SpellSlot.E, DamageStage.Buff))
            {
                Vars.E.Cast();
            }
        }

        public static void Orbwalker_OnPreAttack(LeagueSharp.Common.BeforeAttackArgs args)
        {
            /// <summary>
            ///     The Target Forcing Logic.
            /// </summary>
            if (args.Target is AIHeroClient)
            {
                if (!GameObjects.EnemyHeroes.Any(
                    t =>
                        t.LSIsValidTarget(Vars.AARange) &&
                        t.HasBuff("kalistacoopstrikemarkally")))
                {
                    PortAIO.OrbwalkerManager.ForcedTarget(null);
                    return;
                }

                PortAIO.OrbwalkerManager.ForcedTarget(GameObjects.EnemyHeroes.FirstOrDefault(
                    t =>
                        t.LSIsValidTarget(Vars.AARange) &&
                        t.HasBuff("kalistacoopstrikemarkally")));
                return;
            }
        }
    }
}