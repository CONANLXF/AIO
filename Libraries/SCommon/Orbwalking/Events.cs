using System;
using LeagueSharp;
using EloBuddy;

namespace SCommon.Orbwalking
{
    public static class Events
    {
    }

    /// <summary>
    /// Before Attack Event Args
    /// </summary>
    public class BeforeAttackArgs : EventArgs
    {
        /// <summary>
        /// Orbwalking Instance
        /// </summary>
        public Orbwalker Instance;

        /// <summary>
        /// Target to attack in next aa
        /// </summary>
        public AttackableUnit Target;

        /// <summary>
        /// Process next aa if true
        /// </summary>
        public bool Process = true;
    }

    /// <summary>
    /// After Attack Event Args
    /// </summary>
    public class AfterAttackArgs : EventArgs
    {
        /// <summary>
        /// Orbwalking Instance
        /// </summary>
        public Orbwalker Instance;

        /// <summary>
        /// The target which attacked last
        /// </summary>
        public AttackableUnit Target;

        /// <summary>
        /// Call reset aa timer in next update
        /// </summary>
        public bool ResetAATimer = false;
    }

    /// <summary>
    /// On Attack Event Args
    /// </summary>
    public class OnAttackArgs : EventArgs
    {
        /// <summary>
        /// Orbwalking Instance
        /// </summary>
        public Orbwalker Instance;

        /// <summary>
        /// The target which currently attacking
        /// </summary>
        public AttackableUnit Target;

        /// <summary>
        /// Cancel winduping aa
        /// </summary>
        public bool Cancel = false;
    }
}