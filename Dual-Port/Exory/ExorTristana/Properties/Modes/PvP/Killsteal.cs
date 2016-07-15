using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.Data.Enumerations;
using EloBuddy;

 namespace ExorAIO.Champions.Tristana
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
                Vars.getCheckBoxItem(Vars.RMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.R.Range)))
                {
                    if (Vars.GetRealHealth(target) <
                            (float)GameObjects.Player.LSGetSpellDamage(target, SpellSlot.R) + (target.HasBuff("TristanaECharge")
                                ? (float)GameObjects.Player.LSGetSpellDamage(target, SpellSlot.E) +
                                  (float)GameObjects.Player.LSGetSpellDamage(target, SpellSlot.E, DamageStage.Buff)
                                : 0))
                    {
                        Vars.R.CastOnUnit(target);
                    }
                }
            }
        }
    }
}