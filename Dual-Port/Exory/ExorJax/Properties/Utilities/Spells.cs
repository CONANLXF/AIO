using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Jax
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
            Vars.Q = new Spell(SpellSlot.Q, 700f);
            Vars.W = new Spell(SpellSlot.W, Vars.AARange);
            Vars.E = new Spell(SpellSlot.E, LeagueSharp.Common.Orbwalking.GetRealAutoAttackRange(null) + 95);
            Vars.R = new Spell(SpellSlot.R);
        }
    }
}