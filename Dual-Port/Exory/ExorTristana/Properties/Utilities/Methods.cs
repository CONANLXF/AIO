using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Tristana
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
            Game.OnUpdate += Tristana.OnUpdate;
            Events.OnGapCloser += Tristana.OnGapCloser;
            Obj_AI_Base.OnBuffGain += Tristana.OnBuffAdd;
            Orbwalker.OnPreAttack += Tristana.Orbwalker_OnPreAttack;
        }
    }
}