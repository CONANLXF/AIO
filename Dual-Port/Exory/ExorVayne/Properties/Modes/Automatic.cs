using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using EloBuddy;

 namespace ExorAIO.Champions.Vayne
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
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                !GameObjects.Player.IsDashing() &&
                Vars.getCheckBoxItem(Vars.EMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !t.IsDashing() &&
                        t.LSIsValidTarget(Vars.E.Range) &&
                        !Invulnerable.Check(t, DamageType.True, false) &&
                        !t.LSIsValidTarget(GameObjects.Player.BoundingRadius) &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, t.ChampionName.ToLower())))
                {
                    for (var i = 1; i < 10; i++)
                    {
                        var vector = Vector3.Normalize(target.ServerPosition - GameObjects.Player.ServerPosition);

                        if ((target.ServerPosition + vector * (float)(i * 42.5)).LSIsWall() &&
                            (Vars.E.GetPrediction(target).UnitPosition + vector * (float)(i * 42.5)).LSIsWall() &&
                            (Vars.E2.GetPrediction(target).UnitPosition + vector * (float)(i * 42.5)).LSIsWall() &&

                            (target.ServerPosition + vector * i * 44).LSIsWall() &&
                            (Vars.E.GetPrediction(target).UnitPosition + vector * i * 44).LSIsWall() &&
                            (Vars.E2.GetPrediction(target).UnitPosition + vector * i * 44).LSIsWall())
                        {
                            Vars.E.CastOnUnit(target);
                        }
                    }
                }
            }
        }
    }
}