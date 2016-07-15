using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Twitch
{
    /// <summary>
    ///     The settings class.
    /// </summary>
    internal class Spells
    {
        /// <summary>
        ///     Sets the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q);
            Vars.W = new Spell(SpellSlot.W, 950f);
            Vars.E = new Spell(SpellSlot.E, 1200f);
            Vars.R = new Spell(SpellSlot.R, Vars.AARange + 300f);

            Vars.W.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
        }
    }
}