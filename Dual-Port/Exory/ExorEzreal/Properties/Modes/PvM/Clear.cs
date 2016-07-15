using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Ezreal
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
                if (Targets.Minions.Any())
                {
                    if (Items.HasItem(3025) &&
                        Targets.Minions.Count(m => m.Distance(Targets.Minions[0]) < 100f) >= 2)
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Minions[0]).UnitPosition);
                    }
                    else if (Targets.Minions.Any(
                        m =>
                            Vars.GetRealHealth(m) <
                                (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)))
                    {
                        Vars.Q.Cast(Targets.Minions.FirstOrDefault(
                            m =>
                                Vars.GetRealHealth(m) <
                                    (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)));
                    }
                }

                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any() &&
                    !Targets.JungleMinions.Any(m => m.LSIsValidTarget(Vars.AARange)))
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void JungleClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(Orbwalker.LastTarget is Obj_AI_Minion) ||
                !Targets.JungleMinions.Contains(Orbwalker.LastTarget as Obj_AI_Minion))
            {
                return;
            }

            /// <summary>
            ///     The Q JungleClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "clear")) &&
                Vars.getSliderItem(Vars.QMenu, "clear") != 101)
            {
                Vars.Q.Cast((Orbwalker.LastTarget as Obj_AI_Minion).ServerPosition);
            }
        }
    }
}