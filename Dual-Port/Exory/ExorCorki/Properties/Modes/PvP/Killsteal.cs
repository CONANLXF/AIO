using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

 namespace ExorAIO.Champions.Corki
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
                    Vars.Q.Cast(Vars.Q.GetPrediction(target).CastPosition);
                    return;
                }
            }

            /// <summary>
            ///     The KillSteal R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.R.Range) &&
                        !t.LSIsValidTarget(Vars.AARange) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.R, (ObjectManager.Player.HasBuff("corkimissilebarragecounterbig")
                                ? DamageStage.Empowered
                                : DamageStage.Default))))
                {
                    if (!Vars.R.GetPrediction(target).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                    {
                        Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                    }
                }
            }
        }
    }
}