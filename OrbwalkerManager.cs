using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;

namespace PortAIO
{
    using EloBuddy;
    using LeagueSharp.Common;
    using PortAIO.Utility;
    using SharpDX;
    class OrbwalkerManager
    {

        public static string GetActiveMode()
        {
            return Orbwalker.ActiveModesFlags.ToString().ToLower();
        }

        public static void SetAttack(bool b)
        {
            Orbwalker.DisableAttacking = !b;
        }

        public static void SetMovement(bool b)
        {
            Orbwalker.DisableMovement = !b;
        }

        public static void SetActiveCombo()
        {
            Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.Combo;
        }

        public static void SetActiveClear()
        {
            Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.LaneClear;
        }
    }
}
