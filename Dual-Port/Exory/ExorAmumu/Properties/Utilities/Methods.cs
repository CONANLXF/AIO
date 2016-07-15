using EloBuddy;
using LeagueSharp;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Amumu
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
            Game.OnUpdate += Amumu.OnUpdate;
        }
    }
}