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
        public static AIHeroClient GetTarget(float range, DamageType d)
        {
            return TargetSelector.GetTarget(range, d);
        }

        public static AIHeroClient SelectedTarget
        {
            get
            {
                return TargetSelector.SelectedTarget;
            }
        }

        public static AIHeroClient GetSelectedTarget()
        {
            return TargetSelector.SelectedTarget;
        }

        public static float GetPriority(AIHeroClient hero)
        {
            return TargetSelector.GetPriority(hero);
        }
    }
}
