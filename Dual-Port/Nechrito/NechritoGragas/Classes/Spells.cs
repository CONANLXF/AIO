using EloBuddy;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

using TargetSelector = PortAIO.TSManager; namespace Nechrito_Gragas
{
    class Spells
    {
        private static AIHeroClient Player = ObjectManager.Player;
        public static SpellSlot Ignite, Smite;
        public static Spell Q, W, E, R;
        public static void Initialise()
        {
            Q = new Spell(SpellSlot.Q, 775f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R, 1050f);

            Q.SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.15f, 50f, 900f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.3f, 700f, 1000f, false, SkillshotType.SkillshotCircle);

            Ignite = Player.GetSpellSlot("SummonerDot");
        }
    }
}
