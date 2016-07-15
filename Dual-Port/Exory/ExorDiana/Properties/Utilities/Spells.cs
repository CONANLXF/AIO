using ExorAIO.Utilities;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.SDK.Enumerations;

 namespace ExorAIO.Champions.Diana
{
    /// <summary>
    ///     The spell class.
    /// </summary>
    internal class Spells
    {
        /// <summary>
        ///     Initializes the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q, 830f + GameObjects.Player.BoundingRadius);
            Vars.W = new Spell(SpellSlot.W, 200f + GameObjects.Player.BoundingRadius);
            Vars.E = new Spell(SpellSlot.E, 350f);
            Vars.R = new Spell(SpellSlot.R, 825f);
            Vars.R2 = new Spell(SpellSlot.R, 750f);

            Vars.Q.SetSkillshot(0.80f, 195f, 1400f, false, SkillshotType.SkillshotCircle);
        }
    }
}