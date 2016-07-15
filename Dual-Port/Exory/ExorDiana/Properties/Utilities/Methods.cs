using EloBuddy;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Diana
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     Initializes the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Diana.OnUpdate;
            Obj_AI_Base.OnSpellCast += Diana.OnDoCast;
            Events.OnGapCloser += Diana.OnGapCloser;
            Events.OnInterruptableTarget += Diana.OnInterruptableTarget;
        }
    }
}