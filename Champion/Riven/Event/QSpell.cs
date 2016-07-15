using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using EloBuddy;
using EloBuddy.SDK;

 namespace NechritoRiven.Event
{
    class QSpell
    {
        
        public static void OnSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
            {
                Orbwalker.ResetAutoAttack();
            }
        }
    }
}
