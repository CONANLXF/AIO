using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Karma
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
                /// <summary>
                ///     The Q JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    if (Vars.R.IsReady() &&
                        Vars.getCheckBoxItem(Vars.RMenu, "empq"))
                    {
                        Vars.R.Cast();
                    }

                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                else if (Vars.Q.GetCircularFarmLocation(Targets.Minions, 125f).MinionsHit >= 3)
                {
                    if (Vars.R.IsReady() &&
                        Vars.getCheckBoxItem(Vars.RMenu, "empq"))
                    {
                        Vars.R.Cast();
                    }

                    Vars.Q.Cast(Vars.Q.GetCircularFarmLocation(Targets.Minions, 125f).Position);
                }
            }

            /// <summary>
            ///     The W JungleClear Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Targets.JungleMinions.Any() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "jungleclear")) &&
                Vars.getSliderItem(Vars.WMenu, "jungleclear") != 101)
            {
                if (Vars.R.IsReady() &&
                    Vars.getSliderItem(Vars.WMenu, "lifesaver") >
                        GameObjects.Player.HealthPercent)
                {
                    Vars.R.Cast();
                }

                Vars.W.CastOnUnit(Targets.JungleMinions[0]);
            }
        }
    }
}