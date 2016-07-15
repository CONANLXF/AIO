using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Veigar
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
            Game.OnUpdate += Veigar.OnUpdate;
            Events.OnGapCloser += Veigar.OnGapCloser;
            Events.OnInterruptableTarget += Veigar.OnInterruptableTarget;
            Orbwalker.OnPreAttack += Veigar.OnAction;
        }
    }
}