using System;
using Activators.Handlers;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;

 namespace Activators.Summoners
{
    internal class boost : CoreSum
    {
        internal override string Name => "summonerboost";
        internal override string DisplayName => "Cleanse";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => float.MaxValue;
        internal override int Duration => 3000;
        public double[] array = { .50, .75, 1.0, 1.25, 1.5, 1.75, 2.0 };
        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (Activator.smenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    {
                        continue;
                    }

                    if (!Activator.smenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                        return;

                    if (hero.Player.LSDistance(Player.ServerPosition) > Range)
                        return;

                    Buffs.CheckCleanse(hero.Player);

                    if (hero.CleanseBuffCount >= Menu["use" + Name + "number"].Cast<Slider>().CurrentValue &&
                        hero.CleanseHighestBuffTime >= Menu["use" + Name + "time"].Cast<Slider>().CurrentValue)
                    {
                        //if (!Menu["use" + Name + "od"].Cast<CheckBox>().CurrentValue)
                        //{
                        LeagueSharp.Common.Utility.DelayAction.Add(
                                Game.Ping + Menu["use" + Name + "delay"].Cast<Slider>().CurrentValue, delegate
                                {
                                    UseSpell(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                                    hero.CleanseBuffCount = 0;
                                    hero.CleanseHighestBuffTime = 0;
                                });
                        //}
                    }
                }
            }
        }
    }
}
