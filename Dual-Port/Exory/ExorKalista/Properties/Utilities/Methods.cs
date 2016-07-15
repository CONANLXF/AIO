using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Kalista
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
            Game.OnUpdate += Kalista.OnUpdate;
            LeagueSharp.Common.LSEvents.BeforeAttack += Kalista.Orbwalker_OnPreAttack;
            Orbwalker.OnUnkillableMinion += Kalista.Orbwalker_OnUnkillableMinion;
        }
    }
}