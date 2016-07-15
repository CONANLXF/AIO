using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Vayne
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
            Game.OnUpdate += Vayne.OnUpdate;
            Obj_AI_Base.OnSpellCast += Vayne.OnDoCast;
            Events.OnGapCloser += Vayne.OnGapCloser;
            Events.OnInterruptableTarget += Vayne.OnInterruptableTarget;
            Orbwalker.OnPreAttack += Vayne.Orbwalker_OnPreAttack;
        }
    }
}