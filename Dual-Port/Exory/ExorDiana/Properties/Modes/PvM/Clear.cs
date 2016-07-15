using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Diana
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
            ///     The Clear Q Logic.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                if (Targets.Minions.Any() &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "laneclear")) &&
                    Vars.getSliderItem(Vars.QMenu, "laneclear") != 101 &&
                    Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3)
                {
                    Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).Position);
                }

                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any() &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "jungleclear")) &&
                    Vars.getSliderItem(Vars.QMenu, "jungleclear") != 101)
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }

            /// <summary>
            ///     The Clear W Logic.
            /// </summary>
            if (Vars.W.IsReady())
            {
                /// <summary>
                ///     The LaneClear W Logic.
                /// </summary>
                if (Targets.Minions.Any() &&
                    Targets.Minions.Count(m => m.LSIsValidTarget(Vars.W.Range)) >= 3 &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "laneclear")) &&
                    Vars.getSliderItem(Vars.WMenu, "laneclear") != 101)
                {
                    Vars.W.Cast();
                }

                /// <summary>
                ///     The JungleClear W Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any(m => m.LSIsValidTarget(Vars.W.Range)) &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "jungleclear")) &&
                    Vars.getSliderItem(Vars.WMenu, "jungleclear") != 101)
                {
                    Vars.W.Cast();
                }
            }

            /// <summary>
            ///     The JungleClear R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Targets.JungleMinions.Any(m => m.HasBuff("dianamoonlight")) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.R.Slot, Vars.getSliderItem(Vars.RMenu, "jungleclear")) &&
                Vars.getSliderItem(Vars.RMenu, "jungleclear") != 101)
            {
                Vars.R.CastOnUnit(Targets.JungleMinions.FirstOrDefault(m => m.HasBuff("dianamoonlight")));
            }
        }
    }
}