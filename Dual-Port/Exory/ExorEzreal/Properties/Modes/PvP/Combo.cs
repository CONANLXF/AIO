using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Ezreal
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
        public static void Combo(EventArgs args)
        {
            /// <summary>
            ///     The R Logics.
            /// </summary>
            if (Vars.R.IsReady() &&
                GameObjects.Player.CountEnemyHeroesInRange(Vars.AARange) == 0)
            {
                /// <summary>
                ///     The R Combo Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.RMenu, "combo"))
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(
                        t =>
                            !Invulnerable.Check(t) &&
                            t.LSIsValidTarget(2000f) &&
                            Vars.getCheckBoxItem(Vars.WhiteList2Menu, t.ChampionName.ToLower())))
                    {
                        Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                        return;
                    }
                }

                /// <summary>
                ///     The Automatic R Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.RMenu, "logical"))
                {
                    if (Bools.IsImmobile(Targets.Target) &&
                        !Invulnerable.Check(Targets.Target))
                    {
                        Vars.R.Cast(Targets.Target.ServerPosition);
                        return;
                    }
                }
            }


            if (!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&            
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any())
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                    return;
                }
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                !Targets.Target.LSIsValidTarget(Vars.AARange) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                if (GameObjects.Player.CountAllyHeroesInRange(Vars.W.Range) < 2 &&
                    GameObjects.Player.TotalAttackDamage <
                        GameObjects.Player.TotalMagicalDamage)
                {
                    Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).UnitPosition);
                }
            }
        }
    }
}