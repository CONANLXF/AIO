using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;

 namespace ExorAIO.Champions.Nunu
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
            Game.OnUpdate += Nunu.OnUpdate;
            Orbwalker.OnPreAttack += Nunu.OnAction;
        }
    }
}