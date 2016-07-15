using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.DrMundo
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
        public static void Automatic(EventArgs args)
        {
            /// <summary>
            ///     The Automatic Q LastHit Logics.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                GameObjects.Player.HealthPercent >
                    ManaManager.GetNeededHealth(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "logical")) &&
                Vars.getSliderItem(Vars.QMenu, "logical") != 101)
            {
                foreach (var minion in GameObjects.EnemyMinions.Where(
                    m =>
                        m.LSIsValidTarget(Vars.Q.Range) &&
                        !m.LSIsValidTarget(Vars.AARange) &&
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)))
                {
                    if (!Vars.Q.GetPrediction(minion).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                    {
                        Vars.Q.Cast(minion.ServerPosition);
                    }
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady())
            {
                /// <summary>
                ///     If the player doesn't have the W Buff.
                /// </summary>
                if (!GameObjects.Player.HasBuff("BurningAgony"))
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        if (GameObjects.Player.HealthPercent >= ManaManager.GetNeededHealth(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "clear")) && Vars.getSliderItem(Vars.WMenu, "clear") != 101)
                        {
                            if (Targets.JungleMinions.Any() ||
                                Targets.Minions.Count() >= 2)
                            {
                                Vars.W.Cast();
                            }
                        }
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        if (GameObjects.Player.CountEnemyHeroesInRange(Vars.W.Range) > 0 && Vars.getCheckBoxItem(Vars.WMenu, "combo"))
                        {
                            Vars.W.Cast();
                        }
                    }
                }

                /// <summary>
                ///     If the player has the W Buff.
                /// </summary>
                else
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        if (GameObjects.Player.HealthPercent < ManaManager.GetNeededHealth(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "clear")) || !Targets.JungleMinions.Any() && Targets.Minions.Count() < 2 || Vars.getSliderItem(Vars.WMenu, "clear") == 101)
                        {
                            Vars.W.Cast();
                        }
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        if (GameObjects.Player.CountEnemyHeroesInRange(Vars.W.Range) == 0 || !Vars.getCheckBoxItem(Vars.WMenu, "combo"))
                        {
                            Vars.W.Cast();
                        }
                    }
                }
            }

            /// <summary>
            ///     The R Lifesaver Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                GameObjects.Player.CountEnemyHeroesInRange(700) > 0 &&
                Vars.getCheckBoxItem(Vars.RMenu, "lifesaver") &&
                    Health.GetPrediction(GameObjects.Player, (int)(250 + Game.Ping / 2f)) <= GameObjects.Player.MaxHealth / 5)
            {
                Vars.R.Cast();
            }
        }
    }
}