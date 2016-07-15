using System;
using Firestorm_AIO.Helpers;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using static Firestorm_AIO.Helpers.Helpers;
using EloBuddy;

 namespace Firestorm_AIO.Champions.Yasuo
{
    public static class DashManager
    {
        private static int startTime;
        private static int endTime;
        private static Vector3 startPosition;
        private static Vector3 endPosition;

        public static void Load()
        {
            Events.OnDash += Events_OnDash;
        }

        private static void Events_OnDash(object sender, Events.DashArgs e)
        {
            var hero = sender as AIHeroClient;
            if (hero == null || !hero.IsMe) return;

            startTime = e.StartTick;
            endTime = e.EndTick;
            startPosition = e.StartPos.ToVector3();
            endPosition = e.EndPos.ToVector3();
        }

        public static Vector3 GetPlayerPosition(int time = 0)
        {
            if (Me.IsDashing() && endTime < Environment.TickCount + time)
            {
                return
                    startPosition.LSExtend(endPosition,
                        475*
                        ((endTime - (Environment.TickCount + time))/
                         ((startTime - endTime) == 0 ? 1 : startTime - endTime)));
            }
            return Me.Position;
        }

        public static Vector3 GetPosAfterE(this Obj_AI_Base target)
        {
            return Me.Position.LSExtend(Movement.GetPrediction(target, 250).UnitPosition, 475);
        }

        public static bool IsSafeToE(this Obj_AI_Base target)
        {
            var position = target.GetPosAfterE();

            if (position.IsUnderTower()) return false;

            if (position.CountEnemyHeroesInRange(900) >= 3) return false;

            if (position.CountEnemyHeroesInRange(900) >= 2 && Me.HealthPercent < 40) return false;

            return true;
        }
    }
}
