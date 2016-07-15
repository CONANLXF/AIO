using System;
using Activators.Base;
using Activators.Handlers;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Items.Consumables
{
    class _2043 : CoreItem
    {
        internal override int Id => 2043;
        internal override string Name => "Vision Ward";
        internal override string DisplayName => "Vision Ward";
        internal override int Duration => 101;
        internal override int Priority => 5;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.Stealth, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.SummonersRift };
        internal override int DefaultHP => 0;
        internal override int DefaultMP => 0;

        public _2043()
        {
            Stealth.Init();
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.LSDistance(Player.ServerPosition) > Range)
                    continue;

                if (hero.HitTypes.Contains(HitType.Stealth))
                {
                    UseItem(hero.Player.ServerPosition, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                }
            }
        }
    }
}
