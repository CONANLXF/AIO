using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.SDK.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Veigar
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
            ///     The Tear Stacking Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Bools.HasTear(GameObjects.Player) &&
                !GameObjects.Player.LSIsRecalling() &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                GameObjects.Player.CountEnemyHeroesInRange(1500) == 0 &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.MiscMenu, "tear")) &&
                Vars.getSliderItem(Vars.MiscMenu, "tear") != 101)
            {
                Vars.Q.Cast(Game.CursorPos);
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        t.LSIsValidTarget(Vars.W.Range) &&
                        !Invulnerable.Check(t, DamageType.Magical)))
                {
                    Vars.W.Cast(target.ServerPosition);
                }
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.E.GetPrediction(Targets.Target).AoeTargetsHitCount >=
                    Vars.getSliderItem(Vars.EMenu, "enemies") &&
                Vars.getSliderItem(Vars.EMenu, "enemies") != 6)
            {
                Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).CastPosition);
            }
        }
    }
}