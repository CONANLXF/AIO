using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Cassiopeia
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
                Vars.getSliderItem(Vars.EMenu, "lasthit") != 101)
            {
                DelayAction.Add(Vars.getSliderItem(Vars.EMenu, "delay"), () =>
                {
                    foreach (var minion in Targets.Minions.Where(
                        m =>
                            Vars.GetRealHealth(m) <
                                (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E) + 
                                (m.HasBuffOfType(BuffType.Poison)
                                    ? (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E, DamageStage.Empowered)
                                    : 0)))
                    {
                        Vars.E.CastOnUnit(minion);
                    }
                });
            }
        }
    }
}