using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

namespace ExorAIO.Champions.Ryze
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
            if (!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target, DamageType.Magical))
            {
                return;
            }
            
            if (Bools.HasSheenBuff())
            {
                if (Targets.Target.LSIsValidTarget(Vars.AARange))
                {
                    return;
                }
            }

            /// <summary>
            ///     Dynamic Combo Logic.
            /// </summary>
            switch (Vars.RyzeStacks)
            {
                case 0:
                case 1:
                    if (Vars.RyzeStacks == 0 || (GameObjects.Player.HealthPercent > Vars.getSliderItem(Vars.QMenu, "shield")) || Vars.getSliderItem(Vars.QMenu, "shield") == 0)
                    {
                        /// <summary>
                        ///     The Q Combo Logic.
                        /// </summary>
                        if (Vars.Q.IsReady() &&
                            Targets.Target.LSIsValidTarget(Vars.Q.Range-100f) &&
                            Vars.getCheckBoxItem(Vars.QMenu, "combo"))
                        {
                            if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any())
                            {
                                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                            }
                        }
                    }

                    /// <summary>
                    ///     The W Combo Logic.
                    /// </summary>
                    if (Targets.Target.HasBuff("RyzeE") ||
                        (GameObjects.Player.HealthPercent >
                            Vars.getSliderItem(Vars.QMenu, "shield")) ||
                        Vars.getSliderItem(Vars.QMenu, "shield") == 0)
                    {
                        if (Vars.W.IsReady() &&
                            Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                            Vars.getCheckBoxItem(Vars.WMenu, "combo"))
                        {
                            Vars.W.CastOnUnit(Targets.Target);
                            
                            if (Vars.RyzeStacks == 1 &&
                                (GameObjects.Player.HealthPercent >
                                    Vars.getSliderItem(Vars.QMenu, "shield")) ||
                                Vars.getSliderItem(Vars.QMenu, "shield") == 0)
                            {
                                return;
                            }
                        }
                    }

                    /// <summary>
                    ///     The E Combo Logic.
                    /// </summary>
                    if (Vars.E.IsReady() &&
                        Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                        Vars.getCheckBoxItem(Vars.EMenu, "combo"))
                    {
                        Vars.E.CastOnUnit(Targets.Target);
                    }
                    break;

                default:
                    /// <summary>
                    ///     The Q Combo Logic.
                    /// </summary>
                    if (Vars.Q.IsReady() &&
                        Targets.Target.LSIsValidTarget(Vars.Q.Range-100f) &&
                        Vars.getCheckBoxItem(Vars.QMenu, "combo"))
                    { 
                        Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                    }
                    break;
            }
        }
    }
}