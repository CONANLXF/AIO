using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

 namespace AsunaCondemn
{
    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class Drawings
    {
        /// <summary>
        ///     Loads the range drawings.
        /// </summary>
        public static void Initialize()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            Drawing.OnDraw += delegate
            {
                /// <summary>
                ///     Loads the E drawing.
                /// </summary>
                if (Vars.E.IsReady() &&
                    Vars.DrawingsMenu["e"].Cast<CheckBox>().CurrentValue)
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(t => t.LSIsValidTarget(Vars.E.Range)))
                    {
                        /// <summary>
                        ///     The Position Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(ObjectManager.Player.Position).X,
                            Drawing.WorldToScreen(ObjectManager.Player.Position).Y,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).X,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).Y,
                            1,
                            (target.Position + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).LSIsWall()
                                ? Color.Green 
                                : Color.Red
                        );

                        /// <summary>
                        ///     The Angle-Check Position Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).X,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).Y,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*440).X,
                            Drawing.WorldToScreen(target.Position + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*440).Y,
                            1,
                            (target.Position + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*440).LSIsWall()
                                ? Color.Green 
                                : Color.Red
                        );

                        /// <summary>
                        ///     The Prediction Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(ObjectManager.Player.Position).X,
                            Drawing.WorldToScreen(ObjectManager.Player.Position).Y,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).X,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).Y,
                            1,
                            (Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).LSIsWall()
                                ? Color.Green
                                : Color.Red
                        );

                        /// <summary>
                        ///     The Angle-Check Prediction Line.
                        /// </summary>
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).X,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*420).Y,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*440).X,
                            Drawing.WorldToScreen(Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*440).Y,
                            1,
                            (Vars.E.GetPrediction(target).UnitPosition + Vector3.Normalize(target.Position - ObjectManager.Player.Position)*440).LSIsWall()
                                ? Color.Green
                                : Color.Red);

                        /// <summary>
                        ///     The Flash Position.
                        /// </summary>
                        Render.Circle.DrawCircle(
                            ObjectManager.Player.Position.LSExtend(target.Position, Vars.Flash.Range),
                            50,
                            target.LSIsValidTarget(Vars.E.Range) &&
                            !target.LSIsValidTarget(ObjectManager.Player.BoundingRadius) &&
                            ObjectManager.Player.Distance(ObjectManager.Player.ServerPosition.LSExtend(target.ServerPosition, Vars.Flash.Range)) > ObjectManager.Player.Distance(target) + target.BoundingRadius
                                ? Color.Green
                                : Color.Red,
                            1
                        );
                    }
                }
            };
        }
    }
}