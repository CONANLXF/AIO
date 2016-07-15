using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Items.Offensives
{
    class _3184 : CoreItem
    {
        internal override int Id => 3184;
        internal override int Priority => 5;
        internal override string Name => "Entropy";
        internal override string DisplayName => "Entropy";
        internal override int Duration => 100;
        internal override float Range => 750f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP };
        internal override MapType[] Maps => new[] { MapType.CrystalScar };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Tar != null)
            {
                if (Player.Health / Player.MaxHealth * 100 <=
                    Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    UseItem(Tar.Player, true);
                }

                if (Activator.omenu[Activator.omenu.UniqueMenuId + "useon" + Tar.Player.NetworkId] == null)
                {
                    return;
                }

                if (!Activator.omenu[Activator.omenu.UniqueMenuId + "useon" + Tar.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    return;

                if (Tar.Player.Health/Tar.Player.MaxHealth*100 <=
                    Menu["enemylowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    UseItem(Tar.Player, true);
                }
            }
        }
    }
}
