using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Items.Defensives
{
    class _3069 : CoreItem
    {
        internal override int Id => 3069;
        internal override int Priority => 4;
        internal override string Name => "Talisman";
        internal override string DisplayName => "Talisman of Ascension";
        internal override int Duration => 1000;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.EnemyLowHP, MenuType.SelfLowHP, MenuType.Zhonyas };
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
                        if (hero.IncomeDamage > 0 && hero.Attacker != null &&
                            hero.Attacker.LSDistance(hero.Player.ServerPosition) <= 600)
                            UseItem();    
                    }
                }
            }

            if (Tar != null)
            {
                if (Tar.Player.Health / Tar.Player.MaxHealth * 100 <= Menu["enemylowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    UseItem();
                }
            }
        }
    }
}
