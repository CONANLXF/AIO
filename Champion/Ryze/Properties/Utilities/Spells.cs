using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

namespace ExorAIO.Champions.Ryze
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
            Vars.Q = new Spell(SpellSlot.Q, 1000f);
            Vars.W = new Spell(SpellSlot.W, Vars.AARange);
            Vars.E = new Spell(SpellSlot.E, Vars.AARange);
            Vars.R = new Spell(SpellSlot.R, 1500 * GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).Level);

            Vars.Q.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine); // Original Width: 55
        }
    }
}