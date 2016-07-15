using System;
using Activators.Base;
using Activators.Handlers;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace Activators.Items.Cleansers
{
    class _3222 : CoreItem
    {
        internal override int Id => 3222;
        internal override string Name => "Mikaels";
        internal override string DisplayName => "Mikael's Crucible";
        internal override int Priority => 7;
        internal override int Duration => 1000;
        internal override float Range => 750f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.Cleanse, MenuType.ActiveCheck  };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 5;
        internal override int DefaultMP => 0;
        public double[] array = { .50, .75, 1.0, 1.25, 1.5, 1.75, 2.0 };
        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (Activator.cmenu[Activator.cmenu.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                {
                    continue;
                }
                if (!Activator.cmenu[Activator.cmenu.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) > Range)
                    continue;

                if (hero.ForceQSS)
                {
                    UseItem(hero.Player);
                    hero.MikaelsBuffCount = 0;
                    hero.MikaelsHighestBuffTime = 0;
                }

                Buffs.CheckMikaels(hero.Player);

                if (hero.MikaelsBuffCount >= Menu["use" + Name + "number"].Cast<Slider>().CurrentValue &&
                    hero.MikaelsHighestBuffTime >= Menu["use" + Name + "time"].Cast<Slider>().CurrentValue)
                {
                    if (!Menu["use" + Name + "od"].Cast<CheckBox>().CurrentValue)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + Menu["use" + Name + "delay"].Cast<Slider>().CurrentValue, delegate
                            {
                                UseItem(hero.Player, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                                hero.MikaelsBuffCount = 0;
                                hero.MikaelsHighestBuffTime = 0;
                            });
                    }
                }

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                    Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    if (hero.IncomeDamage > 0)
                    {
                        UseItem(hero.Player, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                        hero.MikaelsBuffCount = 0;
                        hero.MikaelsHighestBuffTime = 0;
                    }
                }
            }        
        }
    }
}
