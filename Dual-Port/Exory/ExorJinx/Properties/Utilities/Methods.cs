using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Jinx
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
            Game.OnUpdate += Jinx.OnUpdate;
            Spellbook.OnCastSpell += Jinx.OnCastSpell;
            Events.OnGapCloser += Jinx.OnGapCloser;
        }
    }
}