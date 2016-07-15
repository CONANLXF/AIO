using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Karma
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
            Game.OnUpdate += Karma.OnUpdate;
            Events.OnGapCloser += Karma.OnGapCloser;
            Obj_AI_Base.OnProcessSpellCast += Karma.OnProcessSpellCast;
            Orbwalker.OnPreAttack += Karma.OnAction;
        }
    }
}