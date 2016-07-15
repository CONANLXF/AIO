using EloBuddy;
using LeagueSharp.SDK;
using Spell = LeagueSharp.SDK.Spell;

 namespace NabbActivator
{
    /// <summary>
    ///     The spells class.
    /// </summary>
    internal class ISpells
    {
        /// <summary>
        ///     Sets the spells.
        /// </summary>
        public static void Initialize()
        {
            if (GameObjects.Player.ChampionName.Equals("GangPlank"))
            {
                Vars.W = new Spell(SpellSlot.W);
            }
            Vars.Smite = new Spell(SpellSlots.GetSmiteSlot(), 500f + GameObjects.Player.BoundingRadius);
        }
    }
}