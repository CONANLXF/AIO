using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Karma
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
            if (Bools.HasSheenBuff() ||
                !Targets.Target.LSIsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                !Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                Targets.Target.LSIsValidTarget(Vars.W.Range+200f) &&
                !Invulnerable.Check(Targets.Target, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.EMenu, "engager"))
            {
                Vars.E.CastOnUnit(GameObjects.Player);
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                !Invulnerable.Check(Targets.Target, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                if (Vars.R.IsReady() &&
                    Vars.getSliderItem(Vars.WMenu, "lifesaver") > GameObjects.Player.HealthPercent)
                {
                    Vars.R.Cast();
                }

                Vars.W.CastOnUnit(Targets.Target);
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range-100f) &&
                !Invulnerable.Check(Targets.Target, DamageType.Magical) &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                {
                    if (Vars.R.IsReady() &&
                        Vars.getCheckBoxItem(Vars.RMenu, "empq"))
                    {
                        Vars.R.Cast();
                    }

                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                }
            }
        }
    }
}