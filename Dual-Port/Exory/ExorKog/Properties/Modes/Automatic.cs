using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.KogMaw
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
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.Q.Range) && t.IsVisible && t.IsHPBarRendered))
                {
                    if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                    {
                        Vars.Q.Cast(target.ServerPosition);
                    }
                }
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getCheckBoxItem(Vars.EMenu, "logical"))
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                            Bools.IsImmobile(t) &&
                            !Invulnerable.Check(t) &&
                            t.LSIsValidTarget(Vars.E.Range)))
                {
                    Vars.E.Cast(target.ServerPosition);
                }
            }

            /// <summary>
            ///     The Automatic R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.R.Slot, Vars.getSliderItem(Vars.RMenu, "logical")) &&
                Vars.getCheckBoxItem(Vars.RMenu, "logicalC") &&
                Vars.getSliderItem(Vars.RMenu, "logical") >
                    GameObjects.Player.GetBuffCount("kogmawlivingartillerycost"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        t.HealthPercent < 50 &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.R.Range)))
                {
                    Vars.R.Cast(target.ServerPosition);
                }
            }
        }
    }
}