using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.Data.Enumerations;
using EloBuddy;

 namespace ExorAIO.Champions.Cassiopeia
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
            ///     The E Clear Logics.
            /// </summary>
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    if (GameObjects.Player.ManaPercent >
                            ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                        Vars.getSliderItem(Vars.EMenu, "clear") != 101)
                    {
                        DelayAction.Add(Vars.getSliderItem(Vars.EMenu, "delay"), () =>
                        {
                            foreach (var minion in Targets.Minions.Where(m => m.HasBuffOfType(BuffType.Poison)))
                            {
                                Vars.E.CastOnUnit(minion);
                            }
                        });
                    }
                }

                /// <summary>
                ///     The E LaneClear Logics.
                /// </summary>
                else if (Targets.Minions.Any())
                {
                    if (GameObjects.Player.ManaPercent <
                            ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "lasthit")) &&
                        Vars.getSliderItem(Vars.EMenu, "lasthit") != 101)
                    {
                        DelayAction.Add(Vars.getSliderItem(Vars.EMenu, "delay"), () =>
                        {
                            foreach (var minion in Targets.Minions.Where(
                                m =>
                                    Vars.GetRealHealth(m) <
                                        (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E) +
                                        (m.HasBuffOfType(BuffType.Poison)
                                            ? (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E, DamageStage.Empowered)
                                            : 0)))
                            {
                                Vars.E.CastOnUnit(minion);
                            }
                        });
                    }
                    else if (GameObjects.Player.ManaPercent >=
                            ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                        Vars.getSliderItem(Vars.EMenu, "clear") != 101)
                    {
                        DelayAction.Add(Vars.getSliderItem(Vars.EMenu, "delay"), () =>
                        {
                            foreach (var minion in Targets.Minions.Where(m => m.HasBuffOfType(BuffType.Poison)))
                            {
                                Vars.E.CastOnUnit(minion);
                            }
                        });
                    }
                }
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
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The Q LaneClear Logic.
                /// </summary>
                else if (Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3)
                {
                    Vars.Q.Cast(Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width).Position);
                }

            }

            /// <summary>
            ///     The W Clear Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "clear")) &&
                Vars.getSliderItem(Vars.WMenu, "clear") != 101)
            {
                /// <summary>
                ///     The W JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any(m => !m.HasBuffOfType(BuffType.Poison)))
                {
                    Vars.W.Cast(Targets.JungleMinions.Where(m => !m.HasBuffOfType(BuffType.Poison)).FirstOrDefault().ServerPosition);
                }

                /// <summary>
                ///     The W LaneClear Logic.
                /// </summary>
                else if (Vars.W.GetCircularFarmLocation(Targets.Minions, Vars.W.Width).MinionsHit >= 3)
                {
                    Vars.W.Cast(Vars.W.GetCircularFarmLocation(Targets.Minions.Where(m => !m.HasBuffOfType(BuffType.Poison)).ToList(), Vars.W.Width).Position);
                }
            }
        }
    }
}