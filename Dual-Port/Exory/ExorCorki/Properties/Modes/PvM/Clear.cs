using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Corki
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
            ///     The Clear E Logics.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                Vars.getSliderItem(Vars.EMenu, "clear") != 101)
            {
                if (Targets.Minions.Count() >= 3 ||
                    Targets.JungleMinions.Any())
                {
                    Vars.E.Cast();
                }
            }

            /// <summary>
            ///     The Clear Q Logics.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "clear")) &&
                Vars.getSliderItem(Vars.QMenu, "clear") != 101)
            {
                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                if (Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3)
                {
                    Vars.Q.Cast(Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width).Position);
                }

                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }
                return;
            }

            /// <summary>
            ///     The Clear R Logics.
            /// </summary>
            if (Vars.R.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.R.Slot, Vars.getSliderItem(Vars.RMenu, "clear")) &&
                Vars.getSliderItem(Vars.RMenu, "clear") != 101)
            {
                /// <summary>
                ///     The LaneClear R Logic.
                /// </summary>
                if (Vars.R.GetLineFarmLocation(Targets.Minions, Vars.R.Width).MinionsHit >= 2)
                {
                    Vars.R.Cast(Vars.R.GetLineFarmLocation(Targets.Minions, Vars.R.Width).Position);
                }

                /// <summary>
                ///     The JungleClear R Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.R.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }
        }
    }
}