using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Pantheon
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
            Game.OnUpdate += Pantheon.OnUpdate;
            Events.OnInterruptableTarget += Pantheon.OnInterruptableTarget;
            Player.OnIssueOrder += Pantheon.Player_OnIssueOrder;
            Orbwalker.OnPreAttack += Pantheon.Orbwalker_OnPreAttack;
        }
    }
}