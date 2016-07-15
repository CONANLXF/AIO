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
        public static LSOrbwalker LSOrbwalker = PortAIO.Init.LSOrbwalker;

        public static string GetActiveMode()
        {
            if (isEBActive)
            {
                return Orbwalker.ActiveModesFlags.ToString().ToLower();
            }
            else
            {
                return LSOrbwalker.ActiveMode.ToString().ToLower();
            }
        }

        public static bool isEBActive
        {
            get
            {
                return Loader.orbwalkerCB == 0;
            }
        }

        public static bool isComboActive
        {
            get
            {
                return isEBActive ? Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) : LSOrbwalker.ActiveMode == LSOrbwalker.OrbwalkingMode.Combo;
            }
        }

        public static bool isHarassActive
        {
            get
            {
                return isEBActive ? Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) : LSOrbwalker.ActiveMode == LSOrbwalker.OrbwalkingMode.Mixed;
            }
        }

        public static bool isCustomKeyActive
        {
            get
            {
                return LSOrbwalker.ActiveMode == LSOrbwalker.OrbwalkingMode.CustomMode;
            }
        }

        public static bool isLaneClearActive
        {
            get
            {
                return isEBActive ? (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) : LSOrbwalker.ActiveMode == LSOrbwalker.OrbwalkingMode.LaneClear;
            }
        }

        public static bool isLastHitActive
        {
            get
            {
                return isEBActive ? Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) : LSOrbwalker.ActiveMode == LSOrbwalker.OrbwalkingMode.LastHit;
            }
        }

        public static bool isFleeActive
        {
            get
            {
                return isEBActive ? Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee) : LSOrbwalker.ActiveMode == LSOrbwalker.OrbwalkingMode.Flee;
            }
        }

        public static bool isNoneActive
        {
            get
            {
                return isEBActive ? Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) : LSOrbwalker.ActiveMode == LSOrbwalker.OrbwalkingMode.None;
            }
        }

        public static void SetAttack(bool b)
        {
            if (isEBActive)
            {
                Orbwalker.DisableAttacking = !b;
            }
            else
            {
                LSOrbwalker.SetAttack(b);
            }
        }

        public static void SetMovement(bool b)
        {
            if (isEBActive)
            {
                Orbwalker.DisableMovement = !b;
            }
            else
            {
                LSOrbwalker.SetMovement(b);
            }
        }

        public static void ResetAutoAttackTimer()
        {
            if (isEBActive)
            {
                Orbwalker.ResetAutoAttack();
            }
            else
            {
                LSOrbwalker.ResetAutoAttackTimer();
            }
        }

        public static AttackableUnit ForcedTarget()
        {
            if (isEBActive)
            {
                return Orbwalker.ForcedTarget;
            }
            else
            {
                return LSOrbwalker.ForceTarget();
            }
        }

        public static AttackableUnit LastTarget()
        {
            if (isEBActive)
            {
                return Orbwalker.LastTarget;
            }
            else
            {
                return LSOrbwalker.LastTarget;
            }
        }

        public static void ForcedTarget(Obj_AI_Base target)
        {
            if (isEBActive)
            {
                Orbwalker.ForcedTarget = target;
            }
            else
            {
                LSOrbwalker.ForceTarget(target);
            }
        }

        public static bool CanMove(int time = 0)
        {
            if (isEBActive)
            {
                return Orbwalker.CanMove;
            }
            else
            {
                return LSOrbwalker.CanMove();
            }
        }

        public static bool CanAttack()
        {
            if (isEBActive)
            {
                return Orbwalker.CanAutoAttack;
            }
            else
            {
                return LSOrbwalker.CanAttack();
            }
        }

        public static bool ShouldWait()
        {
            if (isEBActive)
            {
                return Orbwalker.ShouldWait;
            }
            else
            {
                return LSOrbwalker.ShouldWait();
            }
        }

        public static void MoveA(Vector3 pos)
        {
            if (isEBActive)
            {
                Orbwalker.MoveTo(pos);
            }
            else
            {
                if (LSOrbwalker.ActiveMode == LSOrbwalker.OrbwalkingMode.None)
                {
                    LSOrbwalker.MoveA(pos);
                }
                else
                {
                    LSOrbwalker.SetOrbwalkingPoint(pos);
                }
            }
        }

        public static void SetOrbwalkingPoint(Vector3 pos) // lel only L#
        {
            if (!isEBActive)
            {
                LSOrbwalker.SetOrbwalkingPoint(pos);
            }
        }

        public static void SetActiveCombo()
        {
            if (isEBActive)
            {
                Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.Combo;
            }
            else
            {
                LSOrbwalker.ActiveMode = LSOrbwalker.OrbwalkingMode.Combo;
            }
        }

        public static void SetActiveClear()
        {
            if (isEBActive)
            {
                Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.LaneClear;
            }
            else
            {
                LSOrbwalker.ActiveMode = LSOrbwalker.OrbwalkingMode.LaneClear;
            }
        }
    }
}
