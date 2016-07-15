using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Cassiopeia
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
            Game.OnUpdate += Cassiopeia.OnUpdate;
            Events.OnGapCloser += Cassiopeia.OnGapCloser;
            Events.OnInterruptableTarget += Cassiopeia.OnInterruptableTarget;
        }
    }
}