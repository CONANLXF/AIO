using ExorAIO.Utilities;
using LeagueSharp.SDK.Enumerations;
using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Olaf
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void LastHit(EventArgs args)
        {
            /// <summary>
            ///     The E LastHit Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getCheckBoxItem(Vars.EMenu, "lasthit"))
            {
                foreach (var minion in Targets.Minions.Where(
                    m =>
                        m.LSIsValidTarget(Vars.E.Range) &&
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E)))
                {
                    if (minion.GetMinionType() == MinionTypes.Siege ||
                        minion.GetMinionType() == MinionTypes.Super)
                    {
                        Vars.E.CastOnUnit(minion);
                    }
                }
            }
        }
    }
}