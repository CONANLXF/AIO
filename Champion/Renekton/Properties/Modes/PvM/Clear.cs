using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Renekton
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
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "clear"))
            {
                if (Targets.Minions.Any() &&
                    Targets.Minions.Count() >= 3)
                {
                    Vars.Q.Cast();
                }
                else if (Targets.JungleMinions.Any())
                {
                    if (!Vars.W.IsReady() &&
                        !GameObjects.Player.HasBuff("RenektonPreExecute"))
                    {
                        Vars.Q.Cast();
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
            if (PortAIO.OrbwalkerManager.LastTarget() as Obj_AI_Minion == null)
            {
                return;
            }

            /// <summary>
            ///     The W JungleClear Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "jungleclear") &&
                Targets.JungleMinions.Contains(PortAIO.OrbwalkerManager.LastTarget() as Obj_AI_Minion))
            {
                Vars.W.Cast();
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
            ///     The W BuildingClear Logic.
            /// </summary>
            if (Vars.W.IsReady() && Vars.getCheckBoxItem(Vars.WMenu, "buildings"))
            {
                Vars.W.Cast();
            }
        }
    }
}