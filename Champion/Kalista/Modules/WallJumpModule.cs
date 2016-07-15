namespace iKalistaReborn.Modules
{
    using System;

    using DZLib.Modules;

    using iKalistaReborn.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using EloBuddy;
    class WallJumpModule : IModule
    {
        #region Fields

        private float lastMovementTick;

        #endregion

        #region Public Methods and Operators

        public static Vector3 GetFirstWallPoint(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i);
                if (tempPosition.LSIsWall())
                {
                    return tempPosition.LSExtend(start, -35);
                }
            }

            return Vector3.Zero;
        }

        public static float GetWallLength(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            var firstPosition = Vector3.Zero;
            var lastPosition = Vector3.Zero;

            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i);
                if (tempPosition.LSIsWall() && firstPosition == Vector3.Zero)
                {
                    firstPosition = tempPosition;
                }

                lastPosition = tempPosition;
                if (!lastPosition.LSIsWall() && firstPosition != Vector3.Zero)
                {
                    break;
                }
            }

            return Vector3.Distance(firstPosition, lastPosition);
        }

        public static bool IsOverWall(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i).LSTo2D();
                if (tempPosition.LSIsWall())
                {
                    return true;
                }
            }

            return false;
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public string GetName()
        {
            return "Wall Jump Module";
        }

        public void MoveToLimited(Vector3 where)
        {
            if (Game.Time - lastMovementTick < 90f)
            {
                return;
            }

            lastMovementTick = Game.Time;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, where);
        }

        public void OnExecute()
        {
            if (IsOverWall(ObjectManager.Player.ServerPosition, Game.CursorPos)
                && GetWallLength(ObjectManager.Player.ServerPosition, Game.CursorPos) >= 35f)
            {
                MoveToLimited(GetFirstWallPoint(ObjectManager.Player.ServerPosition, Game.CursorPos));
            }
            else
            {
                MoveToLimited(Game.CursorPos);
            }

            var directionExtension = ObjectManager.Player.ServerPosition.LSTo2D()
                                     + ObjectManager.Player.Direction.LSTo2D().LSPerpendicular()
                                     * (ObjectManager.Player.BoundingRadius * 1.10f);
            var oppositeDirectionExtension = ObjectManager.Player.ServerPosition.LSTo2D()
                                             + ObjectManager.Player.Direction.LSTo2D().LSPerpendicular()
                                             * -(ObjectManager.Player.BoundingRadius * 2f);
            var extendedPosition = ObjectManager.Player.ServerPosition.LSTo2D()
                                   + ObjectManager.Player.Direction.LSTo2D().LSPerpendicular() * (300 + 65f);

            if (directionExtension.LSIsWall() && IsOverWall(ObjectManager.Player.ServerPosition, extendedPosition.To3D())
                && SpellManager.Spell[SpellSlot.Q].IsReady()
                && IsOverWall(ObjectManager.Player.ServerPosition, Game.CursorPos)
                && GetWallLength(ObjectManager.Player.ServerPosition, extendedPosition.To3D()) <= (280f - 65f / 2f))
            {
                SpellManager.Spell[SpellSlot.Q].Cast(oppositeDirectionExtension);
            }
        }

        public void OnLoad()
        {
            Console.WriteLine("Wall Jump Loaded");
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.Q].IsReady();
        }

        #endregion
    }
}