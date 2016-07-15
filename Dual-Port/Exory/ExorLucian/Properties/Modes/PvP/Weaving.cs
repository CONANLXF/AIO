using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Lucian
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
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getBoxItem(Vars.EMenu, "mode") != 2)
            {
                if (!Game.CursorPos.IsUnderEnemyTurret() ||
                    (args.Target as AIHeroClient).Health <
                        GameObjects.Player.LSGetAutoAttackDamage(args.Target as AIHeroClient) * 2)
                {
                    switch (Vars.getBoxItem(Vars.EMenu, "mode"))
                    {
                        case 0:
                            Vars.E.Cast(GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos,
                                GameObjects.Player.Distance(Game.CursorPos) < Vars.AARange
                                    ? GameObjects.Player.BoundingRadius
                                    : 475f));
                            break;

                        case 1:
                            Vars.E.Cast(GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, 475f));
                            break;

                        default:
                            break;
                    }
                    return;
                }
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                (args.Target as AIHeroClient).LSIsValidTarget(Vars.Q.Range) &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                Vars.Q.CastOnUnit(args.Target as AIHeroClient);
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                (args.Target as AIHeroClient).LSIsValidTarget(Vars.W.Range) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                Vars.W.Cast(Vars.W.GetPrediction(args.Target as AIHeroClient).UnitPosition);
            }
        }
    }
}