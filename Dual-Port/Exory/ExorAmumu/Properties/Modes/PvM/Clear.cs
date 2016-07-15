using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.UI;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Amumu
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
            ///     The Q JungleGrab Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "junglegrab")) &&
                Vars.getSliderItem(Vars.QMenu, "junglegrab") != 101)
            {
                if (Targets.JungleMinions.Any(m => !m.LSIsValidTarget(Vars.E.Range)))
                {
                    Vars.Q.Cast(Targets.JungleMinions.FirstOrDefault(m => !m.LSIsValidTarget(Vars.E.Range)).ServerPosition);
                }
            }

            /// <summary>
            ///     The E Clear Logics.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                Vars.getSliderItem(Vars.EMenu, "clear") != 101)
            {
                /// <summary>
                ///     The E LaneClear Logic.
                /// </summary>
                if (Targets.Minions.Count() >= 3)
                {
                    Vars.E.Cast();
                }

                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.E.Cast();
                }
            }
        }
    }
}