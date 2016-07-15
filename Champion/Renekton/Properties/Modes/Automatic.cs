using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using ExorAIO.Utilities;
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.LSIsRecalling()) {}

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >= 50 &&
                !GameObjects.Player.IsUnderEnemyTurret() &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                if (GameObjects.Player.HasBuff("RenektonPreExecute") ||
                    PortAIO.OrbwalkerManager.isComboActive)
                {
                    return;
                }

                foreach (var target in GameObjects.EnemyHeroes.Where(t => t.LSIsValidTarget(Vars.Q.Range)))
                {
                    if (!Vars.W.IsReady() ||
                        !target.LSIsValidTarget(Vars.W.Range))
                    {
                        Vars.Q.Cast();
                    }
                }
            }

            /// <summary>
            ///     The Automatic R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                GameObjects.Player.CountEnemyHeroesInRange(700f) > 0)
            {
                if (Health.GetPrediction(GameObjects.Player, (int)(250 + Game.Ping/2f)) <= GameObjects.Player.MaxHealth/6 &&
                    Vars.getCheckBoxItem(Vars.RMenu, "lifesaver"))
                {
                    Vars.R.Cast();
                }
                else if (GameObjects.Player.CountEnemyHeroesInRange(Vars.R.Range) >= 2 &&
                    Vars.getCheckBoxItem(Vars.RMenu, "aoe"))
                //aoe
                {
                    Vars.R.Cast();
                }
            }
        }
    }
}