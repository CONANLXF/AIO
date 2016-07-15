using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firestorm_AIO.Bases;
using Firestorm_AIO.Enums;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace Firestorm_AIO.DataBases
{
    public static class Spells
    {
        public static List<SpellBase> SpellsDB = new List<SpellBase>
        {
            #region AAtrox
            new SpellBase(Firestorm_AIO.Enums.Champion.Aatrox)
            {
                Q = new Spell(SpellSlot.Q),
                W = new Spell(SpellSlot.W),
                E = new Spell(SpellSlot.E),
                R = new Spell(SpellSlot.R),
            },
            #endregion AAtrox
        };
    }
}