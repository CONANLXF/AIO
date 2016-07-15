#region

using System;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Event;
using NechritoRiven.Menus;
using EloBuddy;

#endregion

 namespace NechritoRiven.Draw
{
    internal class DrawWallSpot : Core.Core
    {
        public static void WallDraw(EventArgs args)
        {
            var end = Player.ServerPosition.LSExtend(Game.CursorPos, Spells.Q.Range);
            var IsWallDash = FleeLogic.IsWallDash(end, Spells.Q.Range);

            var WallPoint = FleeLogic.GetFirstWallPoint(Player.ServerPosition, end);

            if (IsWallDash && MenuConfig.FleeSpot)
            {
                if (WallPoint.LSDistance(Player.ServerPosition) <= 600)
                {
                    Render.Circle.DrawCircle(WallPoint, 60, System.Drawing.Color.White);
                    Render.Circle.DrawCircle(end, 60, System.Drawing.Color.Green);
                }
            }
        }
    }
}
