using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Graves
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
            Game.OnUpdate += Graves.OnUpdate;
            Obj_AI_Base.OnSpellCast += Graves.OnDoCast;
            Events.OnGapCloser += Graves.OnGapCloser;
        }
    }
}