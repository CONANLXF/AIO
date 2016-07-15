using System.Linq;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.SDK;
using ExorAIO.Utilities;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Jhin
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
                /// <summary>
                ///     Loads the R Cone drawing.
                /// </summary>
                if (Vars.End != Vector3.Zero &&
                    Vars.R.Instance.Name.Equals("JhinRShot") &&
                    Vars.getCheckBoxItem(Vars.DrawingsMenu, "rc"))
                {
                    Vars.Cone.Draw(
                        GameObjects.EnemyHeroes.Any(
                            t =>
                                !Vars.Cone.IsOutside((Vector2)t.ServerPosition))
                                    ? Color.Green
                                    : Color.Red,
                        1);
                }
            };
        }
    }
}