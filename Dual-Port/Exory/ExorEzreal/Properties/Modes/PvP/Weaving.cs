using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Ezreal
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
                Invulnerable.Check(args.Target as AIHeroClient))
            {
                return;
            }

            /// <summary>
            ///     The Q Weaving Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                if (!Vars.Q.GetPrediction(args.Target as AIHeroClient).CollisionObjects.Any())
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(args.Target as AIHeroClient).UnitPosition);
                    return;
                }
            }

            /// <summary>
            ///     The W Weaving Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                Vars.W.Cast(Vars.W.GetPrediction(args.Target as AIHeroClient).UnitPosition);
            }
        }
    }
}