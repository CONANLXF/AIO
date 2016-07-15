using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Evelynn
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
            Vars.Q = new Spell(SpellSlot.Q, 500f);
            Vars.W = new Spell(SpellSlot.W, 700f);
            Vars.E = new Spell(SpellSlot.E, GameObjects.Player.BoundingRadius + 225f);
            Vars.R = new Spell(SpellSlot.R, 650f);

            Vars.E.SetTargetted(0.25f, 1000f);
            Vars.R.SetSkillshot(0.25f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }
    }
}