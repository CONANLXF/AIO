using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.UI;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

 namespace ExorAIO.Champions.Vayne
{
    /// <summary>
    ///     The prediction drawings class.
    /// </summary>
    internal class PredictionDrawings
    {
        /// <summary>
        ///     Loads the range drawings.
        /// </summary>
        public static void Initialize()
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            Drawing.OnDraw += delegate
            {
                /// <summary>
                ///     Loads the E drawing.
                /// </summary>
                if (Vars.E != null &&
                    Vars.E.IsReady() &&
                    Vars.DrawingsMenu["epred"] != null &&
                    Vars.getCheckBoxItem(Vars.DrawingsMenu, "epred"))
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(t => t.LSIsValidTarget(Vars.E.Range)))
                    {
                        /// <summary>
                        ///     The Position Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(GameObjects.Player.Position).X,
                            Drawing.WorldToScreen(GameObjects.Player.Position).Y,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                            1,
                            (target.Position + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).LSIsWall()
                                ? Color.Green
                                : Color.Red
                        );

                        /// <summary>
                        ///     The Angle-Check Position Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).X,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).Y,
                            1,
                            (target.Position + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).LSIsWall()
                                ? Color.Green
                                : Color.Red
                        );

                        /// <summary>
                        ///     The Prediction Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(GameObjects.Player.Position).X,
                            Drawing.WorldToScreen(GameObjects.Player.Position).Y,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                            1,
                            (Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).LSIsWall()
                                ? Color.Green
                                : Color.Red
                        );

                        /// <summary>
                        ///     The Angle-Check Prediction Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).X,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).Y,
                            1,
                            (Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).LSIsWall()
                                ? Color.Green
                                : Color.Red
                        );

                        /// <summary>
                        ///     The Prediction Assurance Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(GameObjects.Player.Position).X,
                            Drawing.WorldToScreen(GameObjects.Player.Position).Y,
                            Drawing.WorldToScreen(Vars.E2.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                            Drawing.WorldToScreen(Vars.E2.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                            1,
                            (Vars.E2.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).LSIsWall()
                                ? Color.Green
                                : Color.Red
                        );

                        /// <summary>
                        ///     The Angle-Check Prediction Assurance Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(Vars.E2.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                            Drawing.WorldToScreen(Vars.E2.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                            Drawing.WorldToScreen(Vars.E2.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).X,
                            Drawing.WorldToScreen(Vars.E2.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).Y,
                            1,
                            (Vars.E2.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).LSIsWall()
                                ? Color.Green
                                : Color.Red
                        );
                    }
                }
            };
        }
    }
}