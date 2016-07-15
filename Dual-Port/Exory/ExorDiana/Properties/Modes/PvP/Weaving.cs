using ExorAIO.Utilities;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Diana
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void Weaving(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient) ||
                Invulnerable.Check(args.Target as AIHeroClient, DamageType.Magical))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Weaving Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                (args.Target as AIHeroClient).LSIsValidTarget(Vars.Q.Range) &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(args.Target as AIHeroClient).CastPosition);
                return;
            }

            /// <summary>
            ///     The R Combo Weaving Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                (args.Target as AIHeroClient).HasBuff("dianamoonlight") &&
                (args.Target as AIHeroClient).LSIsValidTarget(Vars.R.Range) &&
                Vars.getCheckBoxItem(Vars.RMenu, "combo"))
            {
                Vars.R.CastOnUnit(args.Target as AIHeroClient);
            }
        }
    }
}