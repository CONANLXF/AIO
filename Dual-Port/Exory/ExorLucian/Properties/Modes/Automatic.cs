using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

 namespace ExorAIO.Champions.Lucian
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
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic R Orbwalking.
            /// </summary>
            if (GameObjects.Player.HasBuff("LucianR"))
            {
                DelayAction.Add((int) (100 + Game.Ping / 2f), () =>
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                });
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "bool"))
            {
                if (!GameObjects.Player.HasBuff("LucianR") &&
                    Targets.Target.LSIsValidTarget(Vars.R.Range) &&
                    Vars.getKeyBindItem(Vars.RMenu, "key"))
                {
					if (!Vars.W.GetPrediction(Targets.Target).CollisionObjects.Any())
					{
						Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).UnitPosition);
					}
                    Vars.R.Cast(Vars.R.GetPrediction(Targets.Target).UnitPosition);
                }
                else if (GameObjects.Player.HasBuff("LucianR") &&
                    !Vars.getKeyBindItem(Vars.RMenu, "key"))
                {
                    Vars.R.Cast();
                }
            }
        }
    }
}