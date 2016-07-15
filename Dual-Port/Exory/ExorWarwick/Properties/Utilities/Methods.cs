using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Warwick
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
            Game.OnUpdate += Warwick.OnUpdate;
            Events.OnInterruptableTarget += Warwick.OnInterruptableTarget;
        }
    }
}