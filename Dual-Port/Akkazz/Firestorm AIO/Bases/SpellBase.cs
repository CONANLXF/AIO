using Firestorm_AIO.Enums;
using LeagueSharp.SDK;

 namespace Firestorm_AIO.Bases
{
    public class SpellBase
    {
        public Champion Champ;
        public Spell Q;
        public Spell W;
        public Spell E;
        public Spell R;

        public SpellBase(Champion champ)
        {
            Champ = champ;
        }
    }
}