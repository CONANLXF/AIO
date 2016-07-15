using System;
using ExorAIO.Utilities;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Diana
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
            ///     The Automatic Misaya Orbwalking.
            /// </summary>
            if (Vars.getKeyBindItem(Vars.RMenu, "key"))
            {
                DelayAction.Add((int)(100 + Game.Ping / 2f), () =>
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                });
            }

            /// <summary>
            ///     The Misaya Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.Q.IsReady() &&
                GameObjects.Player.Mana >
                    Vars.R.Instance.SData.Mana +
                    Vars.Q.Instance.SData.Mana &&
                Targets.Target.LSIsValidTarget(Vars.R2.Range) &&
                Vars.getKeyBindItem(Vars.RMenu, "key") &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
            {
                Vars.R.CastOnUnit(Targets.Target);
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target as AIHeroClient).CastPosition);
            }

            if (GameObjects.Player.IsDashing())
            {
                return;
            }

            /// <summary>
            ///     The AoE E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.CountEnemyHeroesInRange(Vars.E.Range) >=
                    Vars.getSliderItem(Vars.EMenu, "aoe") &&
                Vars.getSliderItem(Vars.RMenu, "aoe") != 101)
            {
                Vars.E.Cast();
            }
        }
    }
}