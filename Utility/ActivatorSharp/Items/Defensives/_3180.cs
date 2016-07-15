using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Items.Defensives
{
    class _3180 : CoreItem
    {
        internal override int Id => 3180;
        internal override int Priority => 4;
        internal override string Name => "Odyns";
        internal override string DisplayName => "Odyn's Veil";
        internal override int Duration => 1000;
        internal override float Range => 525f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfCount };
        internal override MapType[] Maps => new[] { MapType.CrystalScar };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;
            if (Activator.dmenu[Activator.dmenu.UniqueMenuId + "useon" + Player.NetworkId] == null)
            {
                return;
            }
            if (!Activator.dmenu[Activator.dmenu.UniqueMenuId + "useon" + Player.NetworkId].Cast<CheckBox>().CurrentValue)
                return;

            if (Player.Health/Player.MaxHealth*100 <= Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
            {
                UseItem();
            }

            if (Player.LSCountEnemiesInRange(Range) >= Menu["selfcount" + Name].Cast<Slider>().CurrentValue)
            {
                UseItem();
            }
        }
    }
}
