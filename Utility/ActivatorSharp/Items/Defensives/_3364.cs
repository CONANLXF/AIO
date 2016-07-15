using System;
using Activators.Base;
using Activators.Handlers;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Items.Defensives
{
    class _3364 : CoreItem
    {
        internal override int Id => 3364;
        internal override int Priority => 4;
        internal override string Name => "Oracles";
        internal override string DisplayName => "Oracle's Lens";
        internal override int Duration => 1000;
        internal override float Range => 600f;
        internal override int DefaultHP => 99;
        internal override MenuType[] Category => new[] { MenuType.Stealth, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.SummonersRift };

        public _3364()
        {
            Stealth.Init();
        }

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

                if (hero.Player.LSDistance(Player.ServerPosition) > Range)
                    continue;

                if (hero.HitTypes.Contains(HitType.Stealth))
                {
                    UseItem(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                }
            }
        }
    }
}
