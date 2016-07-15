using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Lucian
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
            Game.OnUpdate += Lucian.OnUpdate;
            Obj_AI_Base.OnSpellCast += Lucian.OnDoCast;
            Events.OnGapCloser += Lucian.OnGapCloser;
            Obj_AI_Base.OnPlayAnimation += Lucian.OnPlayAnimation;
            LeagueSharp.Common.LSEvents.BeforeAttack += Lucian.Orbwalker_OnPreAttack;
        }
    }
}