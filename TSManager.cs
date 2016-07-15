using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using PortAIO.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortAIO
{
    class TSManager
    {
        public static bool isEBActive
        {
            get
            {
                return Loader.orbwalkerCB == 0;
            }
        }

        public static AIHeroClient GetTarget(float range, DamageType d)
        {
            return isEBActive ? TargetSelector.GetTarget(range, d) : LSTargetSelector.GetTarget(range, d);
        }

        public static AIHeroClient SelectedTarget
        {
            get
            {
                return isEBActive ? TargetSelector.SelectedTarget : LSTargetSelector.SelectedTarget;
            }
        }

        public static AIHeroClient GetSelectedTarget()
        {
            return isEBActive ? TargetSelector.SelectedTarget : LSTargetSelector.GetSelectedTarget();
        }

        public static void SetTarget(AIHeroClient hero)
        {
            if (!isEBActive)
            {
                LSTargetSelector.SetTarget(hero);
            }
        }

        public static float GetPriority(AIHeroClient hero)
        {
            return isEBActive ? TargetSelector.GetPriority(hero) : LSTargetSelector.GetPriority(hero);
        }

        public static bool IsInvulnerable(Obj_AI_Base target, DamageType damageType, bool ignoreShields = true)
        {
            return LSTargetSelector.IsInvulnerable(target, damageType, ignoreShields);
        }
    }
}
