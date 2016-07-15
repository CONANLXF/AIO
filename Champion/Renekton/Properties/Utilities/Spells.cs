using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Renekton
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
            Vars.Q = new Spell(SpellSlot.Q, GameObjects.Player.BoundingRadius + 225f);
            Vars.W = new Spell(SpellSlot.W, GameObjects.Player.BoundingRadius + 175f);
            Vars.E = new Spell(SpellSlot.E, 450f);
            Vars.R = new Spell(SpellSlot.R);
        }
    }
}