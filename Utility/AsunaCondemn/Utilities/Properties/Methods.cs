using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace AsunaCondemn
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     The methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Condem.OnUpdate;
            Events.OnGapCloser += Condem.OnGapCloser;
        }
    }
}