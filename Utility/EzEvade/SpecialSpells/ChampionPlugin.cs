using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TargetSelector = PortAIO.TSManager; namespace ezEvade
{
    interface ChampionPlugin
    {
        void LoadSpecialSpell(SpellData spellData);
    }
}
