using EloBuddy;
using LeagueSharp;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.DrMundo
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
            Game.OnUpdate += DrMundo.OnUpdate;
            Obj_AI_Base.OnSpellCast += DrMundo.OnDoCast;
        }
    }
}