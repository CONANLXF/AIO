using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Ezreal
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
            Game.OnUpdate += Ezreal.OnUpdate;
            Obj_AI_Base.OnSpellCast += Ezreal.OnDoCast;
            Events.OnGapCloser += Ezreal.OnGapCloser;
            Obj_AI_Base.OnBuffGain += Ezreal.OnBuffAdd;
        }
    }
}