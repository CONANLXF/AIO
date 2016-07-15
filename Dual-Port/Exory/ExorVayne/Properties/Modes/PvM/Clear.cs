using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Vayne
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void Clear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /// <summary>
            ///     The Q Clear Logics.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     The Q FarmHelper Logic.
                /// </summary>
                if (Vars.getSliderItem(Vars.QMenu, "farmhelper") != 101 &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "farmhelper")))
                {
                    if (Targets.Minions.Any() &&
                        Targets.Minions.Count(
                            m =>
                                Vars.GetRealHealth(m) <
                                    GameObjects.Player.GetAutoAttackDamage(m) +
                                    (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)) > 1)
                    {
                        Vars.Q.Cast(Game.CursorPos);
                    }
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
            if (PortAIO.OrbwalkerManager.LastTarget() as Obj_AI_Minion == null ||
                !Targets.JungleMinions.Contains(PortAIO.OrbwalkerManager.LastTarget() as Obj_AI_Minion))
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
                Vars.Q.Cast(Game.CursorPos);
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void BuildingClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(PortAIO.OrbwalkerManager.LastTarget() is Obj_HQ) &&
                !(PortAIO.OrbwalkerManager.LastTarget() is Obj_AI_Turret) &&
                !(PortAIO.OrbwalkerManager.LastTarget() is Obj_BarracksDampener))
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
                Vars.Q.Cast(Game.CursorPos);
            }
        }
    }
}