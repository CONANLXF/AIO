using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

 namespace ExorAIO.Champions.Tryndamere
{
    /// <summary>
    ///     The spell class.
    /// </summary>
    internal class Spells
    {
        /// <summary>
        ///     Sets the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q);
            Vars.W = new Spell(SpellSlot.W, 400f);
            Vars.E = new Spell(SpellSlot.E, 660f);
            Vars.R = new Spell(SpellSlot.R);

            Vars.E.SetSkillshot(0.25f, 93f, 1300f, false, SkillshotType.SkillshotLine);
        }
    }
}