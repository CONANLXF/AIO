using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.Data.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Twitch
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
            ///     The LaneClear W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                !GameObjects.Player.HasBuff("TwitchFullAutomatic") &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "clear")) &&
                Vars.getSliderItem(Vars.WMenu, "clear") != 101)
            {
                /// <summary>
                ///     The W LaneClear Logic.
                /// </summary>
                if (Vars.W.GetCircularFarmLocation(Targets.Minions.Where(m => m.GetBuffCount("twitchdeadlyvenom") <= 4).ToList(), Vars.W.Width).MinionsHit >= 3)
                {
                    Vars.W.Cast(Vars.W.GetCircularFarmLocation(Targets.Minions.Where(m => m.GetBuffCount("twitchdeadlyvenom") <= 4).ToList(), Vars.W.Width).Position);
                }

                /// <summary>
                ///     The W JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any(m => m.GetBuffCount("twitchdeadlyvenom") <= 4))
                {
                    Vars.W.Cast(Targets.JungleMinions.FirstOrDefault(m => m.GetBuffCount("twitchdeadlyvenom") <= 4).ServerPosition);
                }
            }

            /// <summary>
            ///     The LaneClear E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Minions.Any() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "laneclear")) &&
                Vars.getSliderItem(Vars.EMenu, "laneclear") != 101)
            {
                if (Targets.Minions.Count(
                    m =>
                        m.LSIsValidTarget(Vars.E.Range) &&
                        m.Health <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E) +
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E, DamageStage.Buff)) >= 3)
                {
                    Vars.E.Cast();
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
            if (Orbwalker.LastTarget as Obj_AI_Minion == null ||
                !Targets.JungleMinions.Contains(Orbwalker.LastTarget as Obj_AI_Minion))
            {
                return;
            }

            /// <summary>
            ///     The Q JungleClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "jungleclear")) &&
                Vars.getSliderItem(Vars.QMenu, "jungleclear") != 101)
            {
                Vars.Q.Cast();
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void BuildingClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(Orbwalker.LastTarget is Obj_HQ) &&
                !(Orbwalker.LastTarget is Obj_AI_Turret) &&
                !(Orbwalker.LastTarget is Obj_BarracksDampener))
            {
                return;
            }

            /// <summary>
            ///     The Q BuildingClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "buildings")) &&
                Vars.getSliderItem(Vars.QMenu, "buildings") != 101)
            {
                Vars.Q.Cast();
            }
        }
    }
}