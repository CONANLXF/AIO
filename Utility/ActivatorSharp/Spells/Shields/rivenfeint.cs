using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;

 namespace Activators.Spells.Shields
{
    class rivenfeint : CoreSpell
    {
        internal override string Name => "rivenfeint";
        internal override string DisplayName => "Valor | E";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP };
        internal override int DefaultHP => 65;
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

                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu["selfmuchhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                            UseSpellTowards(Game.CursorPos);

                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > hero.Player.Health)
                            UseSpellTowards(Game.CursorPos);
                    }
                }
            }
        }
    }
}
