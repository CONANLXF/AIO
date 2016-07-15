using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;

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
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Q Clear Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "clear")) &&
                Vars.getSliderItem(Vars.QMenu, "clear") != 101)
            {
                if (Targets.Minions.Any() ||
                    Targets.JungleMinions.Any())
                {
                    Vars.Q.Cast();
                }
            }

            /// <summary>
            ///     The E JungleClear Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.JungleMinions.Any() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "jungleclear")) &&
                Vars.getSliderItem(Vars.EMenu, "jungleclear") != 101)
            {
                Vars.E.CastOnUnit(Targets.JungleMinions[0]);
            }
        }
    }
}