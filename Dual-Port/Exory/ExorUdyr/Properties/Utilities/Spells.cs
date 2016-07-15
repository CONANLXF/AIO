using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Udyr
{
    /// <summary>
    ///     The spells class.
    /// </summary>
    internal class Spells
    {
        /// <summary>
        ///     Sets the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q);
            Vars.W = new Spell(SpellSlot.W);
            Vars.E = new Spell(SpellSlot.E);
            Vars.R = new Spell(SpellSlot.R, 625f);
        }
    }
}