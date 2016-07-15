using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;

 namespace NechritoRiven.Event
{
    class Gapclose : Core.Core
    {
        public static void gapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;
            if (t.IsEnemy && Spells.W.IsReady() && t.LSIsValidTarget() && !t.IsZombie)
            {
                if (t.LSIsValidTarget(Spells.W.Range + t.BoundingRadius))
                {
                    Spells.W.Cast(t);
                }
            }
        }
    }
}
