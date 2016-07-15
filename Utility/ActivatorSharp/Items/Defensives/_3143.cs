using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Items.Defensives
{
    class _3143 : CoreItem
    {
        internal override int Id => 3143;
        internal override int Priority => 4;
        internal override string Name => "Randuins";
        internal override string DisplayName => "Randuin's Omen";
        internal override int Duration => 1000;
        internal override float Range => 500f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfCount };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Player.LSCountEnemiesInRange(Range) >= Menu["selfcount" + Name].Cast<Slider>().CurrentValue)
            {
                UseItem();
            }
        }
    }
}
