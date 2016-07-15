using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Ryze
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
            ///     The Clear R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent > 20 &&
                Vars.getCheckBoxItem(Vars.RMenu, "clear"))
            {
                /// <summary>
                ///     The LaneClear R Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    if (Targets.Minions.Count() >= 3)
                    {
                        Vars.R.Cast();
                    }
                }

                /// <summary>
                ///     The JungleClear R Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.R.Cast();
                }
            }

            /// <summary>
            ///     The Clear Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "clear")) &&
                Vars.getSliderItem(Vars.QMenu, "clear") != 101)
            {
                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    Vars.Q.Cast(Targets.Minions[0].ServerPosition);
                }

                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }

            /// <summary>
            ///     The Clear W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "clear")) &&
                Vars.getSliderItem(Vars.WMenu, "clear") != 101)
            {
                /// <summary>
                ///     The LaneClear W Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    Vars.W.CastOnUnit(Targets.Minions[0]);
                }

                /// <summary>
                ///     The JungleClear W Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Vars.W.CastOnUnit(Targets.JungleMinions[0]);
                }
            }

            /// <summary>
            ///     The Clear E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                Vars.getSliderItem(Vars.EMenu, "clear") != 101)
            {
                /// <summary>
                ///     The LaneClear E Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    Vars.E.CastOnUnit(Targets.Minions[0]);
                }

                /// <summary>
                ///     The JungleClear E Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.E.CastOnUnit(Targets.JungleMinions[0]);
                }
            }
        }
    }
}