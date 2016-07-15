using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp.Common;

 namespace Activators.Spells.Shields
{
    class obduracy : CoreSpell
    {
        internal override string Name => "obduracy";
        internal override string DisplayName => "Brutal Strikes | W";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.SelfMinMP };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 45;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Player.Mana / Player.MaxMana * 100 <
                Menu["selfminmp" + Name + "pct"].Cast<Slider>().CurrentValue)
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
                            UseSpell();

                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > hero.Player.Health)
                            UseSpell();
                    }
                }
            }
        }
    }
}
