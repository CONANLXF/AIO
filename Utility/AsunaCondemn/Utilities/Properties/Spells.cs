using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using TargetSelector = PortAIO.TSManager; namespace AsunaCondemn
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
            Vars.E = new Spell(SpellSlot.E, Vars.AARange);
            Vars.Flash = new Spell(ObjectManager.Player.GetSpellSlot("SummonerFlash"), 425f);

            Vars.E.SetSkillshot(0.42f, 60f, 1200f, false, SkillshotType.SkillshotLine);
        }
    }
}