using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Akali
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
            ///     The Q JungleClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.JungleMinions.Any() &&
                GameObjects.Player.ManaPercent > 
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "jungleclear")) &&
                Vars.getSliderItem(Vars.QMenu, "jungleclear") != 101)
            {
                Vars.Q.CastOnUnit(Targets.JungleMinions[0]);
            }

            /// <summary>
            ///     The E LaneClear Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Minions.Count(m => m.LSIsValidTarget(Vars.E.Range)) >= 3 &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                Vars.getSliderItem(Vars.EMenu, "clear") != 101)
            {
                Vars.E.Cast();
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void JungleClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Orbwalker.LastTarget as Obj_AI_Minion == null ||
                !Targets.JungleMinions.Contains(Orbwalker.LastTarget as Obj_AI_Minion))
            {
                return;
            }

            /// <summary>
            ///     The E JungleClear Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                Vars.getSliderItem(Vars.EMenu, "clear") != 101)
            {
                Vars.E.Cast();
            }
        }
    }
}