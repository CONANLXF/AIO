using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Items.Offensives
{
    class _3092 : CoreItem
    {
        internal override int Id => 3092;
        internal override int Priority => 5;
        internal override string Name => "Queens";
        internal override string DisplayName => "Frost Queen's Claim";
        internal override float Range => 1550f;
        internal override int Duration => 2000;
        internal override int DefaultHP => 55;
        internal override MenuType[] Category => new[] { MenuType.EnemyLowHP, MenuType.SelfLowHP, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.Common };

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
           {
               if (hero.Attacker != null && hero.Attacker.LSIsValidTarget(900))
               {
                    if (Activator.omenu[Activator.omenu.UniqueMenuId + "useon" + Tar.Player.NetworkId] == null)
                    {
                        return;
                    }
                    if (!Activator.omenu[Activator.omenu.UniqueMenuId + "useon" + hero.Attacker.NetworkId].Cast<CheckBox>().CurrentValue)
                       return;

                   if (hero.Player.LSDistance(Player.ServerPosition) <= Range)
                   {
                       if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                           Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                       {
                            UseItem();
                       }
                   }
               }
           }

            if (Tar != null)
            {
                if (!Activator.omenu[Activator.omenu.UniqueMenuId + "useon" + Tar.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    return;

                if (Tar.Player.Health / Tar.Player.MaxHealth * 100 <= Menu["enemylowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    UseItem(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                }
            }
        }
    }
}
