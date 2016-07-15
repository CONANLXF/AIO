using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

 namespace ExorAIO.Champions.Vayne
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
            Vars.Q = new Spell(SpellSlot.Q, Vars.AARange + 300f);
            Vars.W = new Spell(SpellSlot.W);
            Vars.E = new Spell(SpellSlot.E, 550f + GameObjects.Player.BoundingRadius);
            Vars.E2 = new Spell(SpellSlot.E, 550f + GameObjects.Player.BoundingRadius);
            Vars.R = new Spell(SpellSlot.R);

            Vars.E.SetSkillshot(0.40f, 50f, 1000f, false, SkillshotType.SkillshotLine);
            Vars.E2.SetSkillshot(0.55f, 50f, 1000f, false, SkillshotType.SkillshotLine);
        }
    }
}