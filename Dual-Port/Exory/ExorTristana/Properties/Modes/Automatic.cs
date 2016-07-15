using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Tristana
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
            if (Bools.HasSheenBuff() ||
                !(Orbwalker.LastTarget as Obj_AI_Base).IsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.Spellbook.IsAutoAttacking &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                if (!Vars.E.IsReady() ||
                    (Orbwalker.LastTarget as Obj_AI_Base).HasBuff("TristanaECharge"))
                {
                    Vars.Q.Cast();
                }
            }
        }
    }
}