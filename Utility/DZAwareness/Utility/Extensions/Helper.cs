using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAwarenessAIO.Utility.HudUtility;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

 namespace DZAwarenessAIO.Utility.Extensions
{
    /// <summary>
    /// The Helper Class
    /// </summary>
    class Helper
    {
        public static Vector3[] GetPolygonVertices(Vector3 centerPosition, int VerticesNumbers, float polygonHalfDiagonal, float baseAngle)
        {
            var PolygonPoints = new List<Vector3>();
            var currentAngle = baseAngle;
            var RotationStep = 360f / VerticesNumbers;

            for (var i = baseAngle; i <= baseAngle + 360f; i += RotationStep)
            {
                PolygonPoints.Add(ConvertToPosition(centerPosition, currentAngle, polygonHalfDiagonal));
                currentAngle += RotationStep;
            }

            return PolygonPoints.ToArray();
        }

        private static Vector3 ConvertToPosition(Vector3 from, float rotation, float polygonHalfDiagonal)
        {
            var angle = LeagueSharp.Common.Geometry.DegreeToRadian(rotation);
            return new Vector2(
                (int)(Math.Cos(angle) * polygonHalfDiagonal + from.X),
                (int)(Math.Sin(-angle) * polygonHalfDiagonal + from.Y)).To3D();
        }

        public static bool IsOverWall(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i).LSTo2D();
                if (tempPosition.LSIsWall() || (!NavMesh.IsWallOfGrass(start, 65) && NavMesh.IsWallOfGrass(tempPosition.To3D(), 65)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInside(Vector2 position, int x, int y, int w, int h)
        {
            return Utils.IsUnderRectangle(position, x, y, w, h);
        }

        public static int TextWidth(string text, Font f)
        {
            int textWidth;

            using (var bmp = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    textWidth = (int)g.MeasureString(text, f).Width;
                }
            }

            return textWidth;
        }

        public static int GetSize(string s, int w)
        {
            return TextWidth(s, new Font("Calibri", w));
        }
    }
}
