using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Items.Defensives
{
    class _3190 : CoreItem
    {
        internal override int Id => 3190;
        internal override int Priority => 5;
        internal override string Name => "Locket";
        internal override string DisplayName => "Locket of Iron Solari";
        internal override int Duration => 2000;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.Zhonyas };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (Activator.dmenu[Activator.dmenu.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                {
                    continue;
                }
                if (!Activator.dmenu[Activator.dmenu.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) <= Range)
                {
                    if (Menu["use" + Name + "norm"].Cast<CheckBox>().CurrentValue)
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            UseItem();

                    if (Menu["use" + Name + "ulti"].Cast<CheckBox>().CurrentValue)
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            UseItem();

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if (hero.TowerDamage > 0  || hero.IncomeDamage > 0 || 
                            hero.MinionDamage > hero.Player.Health)
                            UseItem();
                    }

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu["selfmuchhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                        UseItem();
                }
            }
        }
    }
}
