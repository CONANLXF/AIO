using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

 namespace ExorAIO.Champions.Quinn
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
                        !t.LSIsValidTarget(Vars.AARange) &&
                        t.LSIsValidTarget(Vars.Q.Range - 100f) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q)))
                {
                    if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                    }
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
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.E.Range) &&
                        !t.LSIsValidTarget(Vars.AARange) &&
                        Vars.GetRealHealth(t) <
                            GameObjects.Player.LSGetAutoAttackDamage(t)*2 +
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.E)))
                {
                    Vars.E.CastOnUnit(target);
                }
            }
        }
    }
}