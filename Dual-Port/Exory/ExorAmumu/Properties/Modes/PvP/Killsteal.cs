using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

 namespace ExorAIO.Champions.Amumu
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
            ///     The E KillSteal Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getCheckBoxItem(Vars.EMenu, "killsteal"))
            {
                if (GameObjects.EnemyHeroes.Any(
                    t =>
                        t.LSIsValidTarget(Vars.E.Range) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.E) &&
                        !Invulnerable.Check(t, DamageType.Magical)))
                {
                    Vars.E.Cast();
                    return;
                }
            }

            /// <summary>
            ///     The Q KillSteal Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        t.LSIsValidTarget(Vars.Q.Range) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q) &&
                        !Invulnerable.Check(t, DamageType.Magical)))
                {
                    if (!Vars.Q.GetPrediction(target).CollisionObjects.Any(
                        c =>
                            GameObjects.EnemyHeroes.Contains(c) ||
                            GameObjects.EnemyMinions.Contains(c)))
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(target).UnitPosition);
                        return;
                    }
                }
            }

            /// <summary>
            ///     The R KillSteal Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "killsteal"))
            {
                if (GameObjects.EnemyHeroes.Any(
                    t =>
                        t.LSIsValidTarget(Vars.R.Range) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.R) &&
                        !Invulnerable.Check(t, DamageType.Magical)))
                {
                    Vars.R.Cast();
                }
            }
        }
    }
}