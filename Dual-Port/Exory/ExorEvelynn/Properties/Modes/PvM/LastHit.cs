using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;

 namespace ExorAIO.Champions.Evelynn
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
            ///     The Q LastHit Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "lasthit")) &&
                Vars.getSliderItem(Vars.QMenu, "lasthit") != 101)
            {
                foreach (var minion in Targets.Minions.Where(m => Vars.GetRealHealth(m) < (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)))
                {
                    Vars.Q.Cast();
                }
            }
        }
    }
}