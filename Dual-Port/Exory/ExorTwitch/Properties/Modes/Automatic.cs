using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.Data.Enumerations;
using EloBuddy;

 namespace ExorAIO.Champions.Twitch
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic E Logics.
            /// </summary>
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     The Automatic Enemy E Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.EMenu, "logical"))
                {
                    if (GameObjects.EnemyHeroes.Any(
                        t =>
                            !Invulnerable.Check(t) &&
                            t.LSIsValidTarget(Vars.E.Range) &&
                            t.GetBuffCount("twitchdeadlyvenom") == 6))
                    {
                        Vars.E.Cast();
                    }
                }

                /// <summary>
                ///     The Automatic JungleSteal E Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.EMenu, "junglesteal"))
                {
                    if (Targets.JungleMinions.Any(
                        m =>
                            m.LSIsValidTarget(Vars.E.Range) &&
                            m.Health <
                                (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E) +
                                (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E, DamageStage.Buff)))
                    {
                        Vars.E.Cast();
                    }
                }
            }
        }
    }
}