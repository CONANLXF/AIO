using System;
using System.Linq;
using System.Drawing;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using ExorAIO.Utilities;
using SharpDX;
using Geometry = ExorAIO.Utilities.Geometry;
using Color = System.Drawing.Color;

 namespace ExorAIO.Champions.MissFortune
{
    /// <summary>
    ///     The prediction drawings class.
    /// </summary>
    internal class ConeDrawings
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
                if (Vars.PassiveTarget.LSIsValidTarget() &&
                    Vars.getCheckBoxItem(Vars.DrawingsMenu, "p"))
                {
                    Render.Circle.DrawCircle(Vars.PassiveTarget.Position, Vars.PassiveTarget.BoundingRadius, Color.LightGreen, 1);
                }

                if (Vars.Q.IsReady() &&
                    Vars.getCheckBoxItem(Vars.DrawingsMenu, "qc"))
                {
                    foreach (var obj in ObjectManager.Get<Obj_AI_Base>().Where(m => m.LSIsValidTarget(Vars.Q.Range)))
                    {
                        var polygon = new Geometry.Sector(
                            (Vector2)obj.ServerPosition,
                            (Vector2)obj.ServerPosition.LSExtend(GameObjects.Player.ServerPosition,
                            -(Vars.Q2.Range - Vars.Q.Range)),
                            40f * (float)Math.PI / 180f,
                            (Vars.Q2.Range - Vars.Q.Range) - 50f);

                        var target = GameObjects.EnemyHeroes.FirstOrDefault(
                            t =>
                                !Invulnerable.Check(t) &&
                                t.LSIsValidTarget(Vars.Q2.Range - 50f) &&
                                ((Vars.PassiveTarget.LSIsValidTarget() &&
                                    t.NetworkId == Vars.PassiveTarget.NetworkId) ||
                                    !Targets.Minions.Any(m => !polygon.IsOutside((Vector2)m.ServerPosition))));

                        if (target != null)
                        {
                            polygon.Draw(
                                !polygon.IsOutside((Vector2)target.ServerPosition) &&
                                !polygon.IsOutside(
                                    (Vector2)Movement.GetPrediction(
                                        target,
                                        GameObjects.Player.Distance(target) / Vars.Q.Speed + Vars.Q.Delay).UnitPosition)

                                ? Color.Green
                                : Color.Red);
                        }
                    }
                }
            };
        }
    }
}