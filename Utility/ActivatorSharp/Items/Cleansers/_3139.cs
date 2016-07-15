using System;
using Activators.Base;
using Activators.Handlers;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace Activators.Items.Cleansers
{
    class _3139 : CoreItem
    {
        internal override int Id => 3139;
        internal override string Name => "Mercurial";
        internal override string DisplayName => "Mercurial Scimitar";
        internal override int Priority => 6;
        internal override int Duration => 1000;
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.Cleanse, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.HowlingAbyss, MapType.SummonersRift };
        internal override int DefaultHP => 5;
        internal override int DefaultMP => 0;

        public double[] array = { .50, .75, 1.0, 1.25, 1.5, 1.75, 2.0 };


        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (Activator.cmenu[Activator.cmenu.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    {
                        continue;
                    }
                    if (!Activator.cmenu[Activator.cmenu.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                        continue;

                    Buffs.CheckMercurial(hero.Player);

                    if (hero.MercurialBuffCount >= Menu["use" + Name + "number"].Cast<Slider>().CurrentValue &&
                        hero.MercurialHighestBuffTime >= Menu["use" + Name + "time"].Cast<Slider>().CurrentValue)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + Menu["use" + Name + "delay"].Cast<Slider>().CurrentValue, delegate
                        {
                            UseItem(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                            hero.MercurialBuffCount = 0;
                            hero.MercurialHighestBuffTime = 0;
                        });
                    }
                }
            }
        }
    }
}
