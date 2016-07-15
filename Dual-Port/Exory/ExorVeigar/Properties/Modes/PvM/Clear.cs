using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;

 namespace ExorAIO.Champions.Veigar
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
                if (Targets.Minions.Any())
                {
                    if (Vars.Q.GetLineFarmLocation(Targets.Minions.Where(
                        m =>
                            m.Health < (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)).ToList(), Vars.Q.Width).MinionsHit == 2)
                    {
                        Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions.Where(
                            m =>
								m.Health < (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)).ToList(), Vars.Q.Width).Position);
                    }
                }
                else if (Targets.JungleMinions.Any(
					m =>
						m.Health < (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)))
                {
                    Vars.Q.Cast(Targets.JungleMinions.FirstOrDefault(
                        m =>
                            m.Health < (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)).ServerPosition);
                }
            }

            /// <summary>
            ///     The W Clear Logics.
            /// </summary>
            if (Vars.W.IsReady())
            {
                /// <summary>
                ///     The W LaneClear Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    if (GameObjects.Player.ManaPercent >
                            ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "laneclear")) &&
                        Vars.getSliderItem(Vars.WMenu, "laneclear") != 101)
                    {
                        if (Vars.W.GetCircularFarmLocation(Targets.Minions, Vars.W.Width).MinionsHit >=
                                Vars.getSliderItem(Vars.WMenu, "minionshit"))
                        {
                            Vars.W.Cast(Vars.W.GetCircularFarmLocation(Targets.Minions, Vars.W.Width).Position);
                        }
                    }
                }

                /// <summary>
                ///     The W JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    if (GameObjects.Player.ManaPercent >
                            ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "jungleclear")) &&
                        Vars.getSliderItem(Vars.WMenu, "jungleclear") != 101)
                    {
                        if (!Targets.JungleMinions.Any(
							m =>
								m.Health < (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.W)))
                        {
                            Vars.W.Cast(Targets.JungleMinions[0].ServerPosition);
                        }
                    }
                }
            }
        }
    }
}