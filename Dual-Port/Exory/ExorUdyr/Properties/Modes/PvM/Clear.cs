using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Udyr
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !(Orbwalker.LastTarget as Obj_AI_Minion).IsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The W Clear Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.Player.HealthPercent <
                Vars.getSliderItem(Vars.WMenu, "clear") &&
                Vars.getSliderItem(Vars.WMenu, "clear") != 101)
            {
                Vars.W.Cast();
                return;
            }

            /// <summary>
            ///     The JungleClear Logic.
            /// </summary>
            if (Targets.JungleMinions.Any())
            {
                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                if (Vars.E.IsReady() &&
                    GameObjects.Player.ManaPercent >=
                        ManaManager.GetNeededHealth(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "jungleclear")) &&
                    Vars.getSliderItem(Vars.EMenu, "jungleclear") != 101)
                {
                    if ((Orbwalker.LastTarget as Obj_AI_Minion).IsValidTarget(Vars.R.Range) &&
                        !(Orbwalker.LastTarget as Obj_AI_Minion).HasBuff("udyrbearstuncheck"))
                    {
                        Vars.E.Cast();
                    }
                }

                if (GameObjects.Player.HasBuff("itemmagicshankcharge") ||
                    GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                {
                    /// <summary>
                    ///     The R JungleClear Logic.
                    /// </summary>
                    if (Vars.R.IsReady() &&
                        GameObjects.Player.GetBuffCount("UdyrPhoenixStance") != 3 &&
                        GameObjects.Player.ManaPercent >=
                            ManaManager.GetNeededHealth(Vars.R.Slot, Vars.getSliderItem(Vars.RMenu, "clear")) &&
                        Vars.getSliderItem(Vars.RMenu, "clear") != 101)
                    {
                        Vars.R.Cast();
                    }
                }
                else
                {
                    /// <summary>
                    ///     The Q JungleClear Logic.
                    /// </summary>
                    if (Vars.Q.IsReady() &&
                        GameObjects.Player.ManaPercent >=
                            ManaManager.GetNeededHealth(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "clear")) &&
                        Vars.getSliderItem(Vars.QMenu, "clear") != 101)
                    {
                        Vars.Q.Cast();
                    }
                }
            }

            /// <summary>
            ///     The LaneClear R Logic.
            /// </summary>
            else if (Targets.Minions.Any() &&
                Targets.Minions.Count() >= 3)
            {
                if (Vars.R.IsReady() &&
                    GameObjects.Player.GetBuffCount("UdyrPhoenixStance") != 3 &&
                    GameObjects.Player.ManaPercent >=
                        ManaManager.GetNeededHealth(Vars.R.Slot, Vars.getSliderItem(Vars.RMenu, "clear")) &&
                    Vars.getSliderItem(Vars.RMenu, "clear") != 101)
                {
                    Vars.R.Cast();
                }
            }
        }

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void BuildingClear(EventArgs args)
        {
            if (Orbwalker.LastTarget as Obj_HQ == null &&
                Orbwalker.LastTarget as Obj_AI_Turret  == null &&
                Orbwalker.LastTarget as Obj_BarracksDampener == null)
            {
                return;
            }

            /// <summary>
            ///     The Q BuildingClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededHealth(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "buildings")) &&
                Vars.getSliderItem(Vars.QMenu, "buildings") != 101)
            {
                Vars.Q.Cast();
            }
        }
    }
}