using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp.Common;

 namespace Activators.Spells.Shields
{
    class mordekaiserw : CoreSpell
    {
        internal override string Name => "mordekaisercreepindeathcast";
        internal override string DisplayName => "Harvester of Sorrow | W";
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.SelfMinHP };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 55;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Player.Health / Player.MaxHealth * 100 <
                Menu["selfminhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    continue;
                if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) <= Range)
                {
                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu["selfmuchhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                            UseSpellOn(hero.Player);

                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > hero.Player.Health)
                            UseSpellOn(hero.Player);
                    }
                }
            }
        }
    }
}
