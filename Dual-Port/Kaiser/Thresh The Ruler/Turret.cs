using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;

using TargetSelector = PortAIO.TSManager; namespace ThreshTherulerofthesoul
{
    class Turret
    {
        public static bool IsUnderEnemyTurret(AIHeroClient hero)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.LSDistance(hero.Position) < 950 && turret.IsEnemy);
        }

        public static bool IsUnderAllyTurret(AIHeroClient hero)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.LSDistance(hero.Position) < 950 && turret.IsAlly);
        }
    }
}
