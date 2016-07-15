using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using DZAwarenessAIO.Utility;
using DZAwarenessAIO.Utility.Logs;
using DZAwarenessAIO.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Geometry = DZAwarenessAIO.Utility.Geometry;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

 namespace DZAwarenessAIO.Modules.Ranges
{
    /// <summary>
    /// The Ranges Tracking Class
    /// </summary>
    class RangesBase : ModuleBase
    {

        public static Menu moduleMenu;

        /// <summary>
        /// Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            try
            {
                var RootMenu = Variables.Menu;
                moduleMenu = RootMenu.AddSubMenu("AA Range Tracking", "dz191.dza.ranges");
                {
                    moduleMenu.AddBool("dz191.dza.ranges.ally", "Ally Ranges");
                    moduleMenu.AddBool("dz191.dza.ranges.enemy", "Enemy Ranges");
                }
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("Ranges_Base", e));
            }

        }

        /// <summary>
        /// Initializes the events.
        /// </summary>
        public override void InitEvents()
        {
            try
            {
                Drawing.OnDraw += Drawing_OnDraw;
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("Ranges_Base", e));
            }
        }

        /// <summary>
        /// The OnDraw event delegate.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (ShouldRun())
                {
                    if (moduleMenu["dz191.dza.ranges.enemy"].Cast<CheckBox>().CurrentValue)
                    {
                        DrawEnemyZone();
                    }

                    if (moduleMenu["dz191.dza.ranges.ally"].Cast<CheckBox>().CurrentValue)
                    {
                        DrawAllyZone();
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("Ranges_Base", e));
            }
        }


        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <returns></returns>
        public override ModuleTypes GetModuleType()
        {
            return ModuleTypes.General;
        }

        /// <summary>
        /// Shoulds the module run.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldRun()
        {
            return moduleMenu["dz191.dza.ranges.enemy"].Cast<CheckBox>().CurrentValue || moduleMenu["dz191.dza.ranges.ally"].Cast<CheckBox>().CurrentValue;
        }

        /// <summary>
        /// Called On Update
        /// </summary>
        public override void OnTick() { }

        /// <summary>
        /// Draws the enemy zone.
        /// </summary>
        public void DrawEnemyZone()
        {
            var currentPath = GetEnemyPoints().Select(v2 => new IntPoint(v2.X, v2.Y)).ToList();
            var currentPoly = Geometry.ToPolygon(currentPath);
            currentPoly.Draw(Color.Red);
        }

        /// <summary>
        /// Draws the ally zone.
        /// </summary>
        public void DrawAllyZone()
        {
            var currentPath = GetAllyPoints().Select(v2 => new IntPoint(v2.X, v2.Y)).ToList();
            var currentPoly = Geometry.ToPolygon(currentPath);
            currentPoly.Draw(Color.Green);
        }

        /// <summary>
        /// Gets the enemy points.
        /// </summary>
        /// <param name="dynamic">if set to <c>true</c> it will use a dynamic range.</param>
        /// <returns></returns>
        private List<Vector2> GetEnemyPoints(bool dynamic = true)
        {
            var staticRange = 360f;
            var polygonsList = Variables.EnemiesClose.Select(enemy => new Geometry.Circle(enemy.ServerPosition.LSTo2D(), (dynamic ? (enemy.IsMelee ? enemy.AttackRange * 1.5f : enemy.AttackRange) : staticRange) + enemy.BoundingRadius + 20).ToPolygon()).ToList();
            var pathList = Geometry.ClipPolygons(polygonsList);
            var pointList = pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y)).Where(currentPoint => !currentPoint.LSIsWall()).ToList();
            return pointList;
        }

        /// <summary>
        /// Gets the ally points.
        /// </summary>
        /// <param name="dynamic">if set to <c>true</c> it will use a dynamic range.</param>
        /// <returns></returns>
        private List<Vector2> GetAllyPoints(bool dynamic = true)
        {
            var staticRange = 360f;
            var polygonsList = Variables.AlliesClose.Select(enemy => new Geometry.Circle(enemy.ServerPosition.LSTo2D(), (dynamic ? (enemy.IsMelee ? enemy.AttackRange * 1.5f : enemy.AttackRange) : staticRange) + enemy.BoundingRadius + 20).ToPolygon()).ToList();
            var pathList = Geometry.ClipPolygons(polygonsList);
            var pointList = pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y)).Where(currentPoint => !currentPoint.LSIsWall()).ToList();
            return pointList;
        }
    }
}
