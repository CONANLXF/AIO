using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp.Common;

 namespace Activators.Spells.Shields
{
    class fioraw : CoreSpell
    {
        internal override string Name => "fioraw";
        internal override string DisplayName => "Riposte | W";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfMuchHP, MenuType.Zhonyas, MenuType.SpellShield };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 55;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    continue;
                if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) <= hero.Player.BoundingRadius)
                {
                    if (hero.IncomeDamage >= hero.Player.Health && hero.Attacker != null)
                        UseSpellTowards(hero.Attacker.ServerPosition);

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu["selfmuchhp" + Name + "pct"].Cast<Slider>().CurrentValue && hero.Attacker != null)
                            UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu["ss" + Name + "all"].Cast<CheckBox>().CurrentValue)
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell) && hero.Attacker != null)
                            UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu["ss" + Name + "cc"].Cast<CheckBox>().CurrentValue)
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl) && hero.Attacker != null)
                            UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu["use" + Name + "norm"].Cast<CheckBox>().CurrentValue)
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger) && hero.Attacker != null)
                                UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu["use" + Name + "ulti"].Cast<CheckBox>().CurrentValue)
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate) && hero.Attacker != null)
                                UseSpellTowards(hero.Attacker.ServerPosition);
                }
            }
        }
    }
}
