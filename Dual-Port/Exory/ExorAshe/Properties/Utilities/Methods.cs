using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Ashe
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
            Game.OnUpdate += Ashe.OnUpdate;
            Events.OnGapCloser += Ashe.OnGapCloser;
            Events.OnInterruptableTarget += Ashe.OnInterruptableTarget;
        }
    }
}