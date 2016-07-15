using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using System.Linq;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Akali
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
                !Targets.Target.LSIsValidTarget() ||
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
                Vars.Q.CastOnUnit(Targets.Target);
            }

            /// <summary>
            ///     The R Gapclose Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                !Targets.Target.LSIsValidTarget(Vars.R.Range) &&
                Targets.Target.LSIsValidTarget(Vars.R.Range * 2) &&
                GameObjects.Player.GetBuffCount("AkaliShadowDance") >=
                    Vars.getSliderItem(Vars.MiscMenu, "gapclose") &&
                Vars.getSliderItem(Vars.MiscMenu, "gapclose") != 4)
            {
                foreach (var minion in Targets.Minions.Where(
                    m =>
                        m.LSIsValidTarget(Vars.R.Range) &&
                        m.Distance(Targets.Target) < Vars.Q.Range))
                {
                    Vars.R.CastOnUnit(minion);
                }
            }
        }
    }
}