using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Vayne
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
                if (Vars.getCheckBoxItem(Vars.MiscMenu, "wstacks") &&
                    (args.Target as AIHeroClient).GetBuffCount("vaynesilvereddebuff") != 1)
                {
                    return;
                }

                if (!Vars.getCheckBoxItem(Vars.MiscMenu, "alwaysq"))
                {
                    if (GameObjects.Player.Distance(Game.CursorPos) > Vars.AARange &&
                        GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, 300f).CountEnemyHeroesInRange(1000f) < 3 &&
                        Targets.Target.Distance(GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, 300f)) < Vars.AARange)
                    {
                        Vars.Q.Cast(Game.CursorPos);
                    }
                }
                else
                {
                    Vars.Q.Cast(Game.CursorPos);
                }
            }
        }
    }
}