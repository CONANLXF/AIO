using System.Collections.Generic;
using System.Linq;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

 namespace Firestorm_AIO.Helpers
{
    public static class AntiGapcloser
    {
        public static List<Spell> SpellToAntiGapcloser = new List<Spell>();

        public static void IsAntiGapCloser(this Spell spell)
        {
            SpellToAntiGapcloser.Add(spell);
        }

        public static void Load()

        {
            Events.OnGapCloser += Events_OnGapCloser;
        }

        private static void Events_OnGapCloser(object sender, Events.GapCloserEventArgs e)
        {
            if (SpellToAntiGapcloser == null || e.Sender.IsAlly) return;

            var spellToUse = SpellToAntiGapcloser.OrderByDescending(s => s.Range).FirstOrDefault(s => s.IsReady());

            if (e.IsDirectedToPlayer)
            {
                spellToUse?.CastIfHitchanceMinimum(e.Sender, HitChance.Medium);
            }
        }
    }
}