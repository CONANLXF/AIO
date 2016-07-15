using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Caitlyn
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     Initializes the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Caitlyn.OnUpdate;
            Spellbook.OnCastSpell += Caitlyn.OnCastSpell;
            Events.OnGapCloser += Caitlyn.OnGapCloser;
            Events.OnInterruptableTarget += Caitlyn.OnInterruptableTarget;
            Obj_AI_Base.OnSpellCast += Caitlyn.OnDoCast;
            Obj_AI_Base.OnProcessSpellCast += Caitlyn.OnProcessSpellCast;
        }
    }
}