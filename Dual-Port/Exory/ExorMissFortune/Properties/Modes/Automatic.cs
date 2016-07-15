using System;
using System.Linq;
using ExorAIO.Utilities;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.MissFortune
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
            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            Orbwalker.DisableAttacking = GameObjects.Player.HasBuff("missfortunebulletsound");
            Orbwalker.DisableMovement = GameObjects.Player.HasBuff("missfortunebulletsound");

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "bool"))
            {
                if (Targets.Target.LSIsValidTarget(Vars.E.IsReady()
                        ? Vars.E.Range
                        : Vars.R.Range) &&
                    !GameObjects.Player.HasBuff("missfortunebulletsound") &&
                    Vars.getKeyBindItem(Vars.RMenu, "key"))
                {
                    if (Vars.E.IsReady())
                    {
                        Vars.E.Cast(Targets.Target.ServerPosition);
                    }
                    Vars.R.Cast(Targets.Target.ServerPosition);
                }
                else if (GameObjects.Player.HasBuff("missfortunebulletsound") &&
                    !Vars.getKeyBindItem(Vars.RMenu, "key"))
                {
                    Orbwalker.MoveTo(Game.CursorPos);
                }
            }
        }
    }
}