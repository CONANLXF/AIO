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
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() &&
                GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.AARange)))
            {
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.W.Range)) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                Vars.W.Cast();
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
                GameObjects.Player.Mana >
                    Vars.Q.Instance.SData.Mana +
                    Vars.W.Instance.SData.Mana &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo") && Targets.Target.IsVisible && Targets.Target.IsHPBarRendered)
            {
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any())
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                }
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range - 100f) &&
                GameObjects.Player.Mana >
                    Vars.Q.Instance.SData.Mana +
                    Vars.W.Instance.SData.Mana &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo"))
            {
                Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).UnitPosition);
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Targets.Target.HealthPercent < 50 &&
                Targets.Target.LSIsValidTarget(Vars.R.Range) &&
                GameObjects.Player.Mana >
                    Vars.Q.Instance.SData.Mana +
                    Vars.W.Instance.SData.Mana &&
                Vars.getCheckBoxItem(Vars.RMenu, "comboC") &&
                Vars.getSliderItem(Vars.RMenu, "combo") >
                    GameObjects.Player.GetBuffCount("kogmawlivingartillerycost"))
            {
                Vars.R.Cast(Vars.R.GetPrediction(Targets.Target).CastPosition);
            }
        }
    }
}