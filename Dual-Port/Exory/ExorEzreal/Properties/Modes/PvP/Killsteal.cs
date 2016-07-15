using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

 namespace ExorAIO.Champions.Ezreal
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
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.Q.Range) &&
                        !t.LSIsValidTarget(Vars.AARange) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q)))
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
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.W.Range) &&
                        !t.LSIsValidTarget(Vars.AARange) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.W)))
                {
                    Vars.W.Cast(Vars.W.GetPrediction(target).UnitPosition);
                    return;
                }
            }

            /// <summary>
            ///     The KillSteal R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                GameObjects.Player.CountEnemyHeroesInRange(Vars.AARange) == 0 &&
                Vars.getCheckBoxItem(Vars.RMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.R.Range) &&
                        Vars.GetRealHealth(t) >
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.R)))
                {
                    Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                }
            }
        }
    }
}