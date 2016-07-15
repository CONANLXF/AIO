using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Draven
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     Sets the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Draven.OnUpdate;
            Events.OnGapCloser += Draven.OnGapCloser;
            Events.OnInterruptableTarget += Draven.OnInterruptableTarget;
        }
    }
}