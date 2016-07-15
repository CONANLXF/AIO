using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using SharpDX;

 namespace ExorAIO.Champions.Jhin
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
            ///     The KillSteal R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.R.Instance.Name.Equals("JhinRShot") &&
                Vars.getCheckBoxItem(Vars.RMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        !Vars.Cone.IsOutside((Vector2)t.ServerPosition) &&
                        t.LSIsValidTarget(Vars.R.Range) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.R, (Vars.ShotsCount == 3
                                ? DamageStage.Empowered
                                : DamageStage.Default))))
                {
                    Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                    return;
                }
            }

            if (Vars.R.Instance.Name.Equals("JhinRShot"))
            {
                return;
            }

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
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q)))
                {
                    Vars.Q.CastOnUnit(target);
                    return;
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
                        !t.LSIsValidTarget(Vars.AARange) &&
                        t.LSIsValidTarget(Vars.W.Range - 100f) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.W, DamageStage.Empowered)))
                {
                    Vars.W.Cast(Vars.W.GetPrediction(target).UnitPosition);
                }
            }
        }
    }
}