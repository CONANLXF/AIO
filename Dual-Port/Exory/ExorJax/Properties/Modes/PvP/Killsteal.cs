using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

 namespace ExorAIO.Champions.Jax
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
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q) +
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.W)))
                {
                    if (Vars.W.IsReady() &&
                        Vars.GetRealHealth(target) > (float)GameObjects.Player.LSGetSpellDamage(target, SpellSlot.Q))
                    {
                        Vars.W.Cast();
                    }
                    
                    Vars.Q.CastOnUnit(target);
                }
            }
        }
    }
}