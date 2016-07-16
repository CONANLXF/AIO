using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

namespace ExorAIO.Champions.Ryze
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
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The KillSteal Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        t.LSIsValidTarget(Vars.Q.Range - 50f) &&
                        !Invulnerable.Check(t, DamageType.Magical) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q) * (1 + (t.HasBuff("RyzeE")
                                ? new double[] { 40, 55, 70, 85, 100, 100 }[GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).Level] / 100
                                : 0))))
                {
                    if (!Vars.Q.GetPrediction(target).CollisionObjects.Any())
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(target).UnitPosition);
                        return;
                    }
                }
            }

            /// <summary>
            ///     The KillSteal W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        t.LSIsValidTarget(Vars.W.Range) &&
                        !Invulnerable.Check(t, DamageType.Magical, false) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.W)))
                {
                    Vars.W.CastOnUnit(target);
                    return;
                }
            }

            /// <summary>
            ///     The KillSteal E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getCheckBoxItem(Vars.EMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        t.LSIsValidTarget(Vars.E.Range) &&
                        !Invulnerable.Check(t, DamageType.Magical, false) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.E)))
                {
                    Vars.E.CastOnUnit(target);
                }
            }
        }
    }
}