using System.Collections.Generic;
using LeagueSharp;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace BlackFeeder
{
    public class ChampWrapper
    {
        public string Name { get; set; }

        public List<SpellSlot> SpellSlots { get; set; }
    }
}