using System;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Summoners
{
    internal class mana : CoreSum
    {
        internal override string Name => "summonermana";
        internal override string DisplayName => "Clarity";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => 600f;
        internal override int Duration => 1000;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Activator.smenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                if (hero.Player.MaxMana <= 200 || hero.Player.LSDistance(Player.ServerPosition) > Range)
                    continue;

                if (hero.Player.Mana/hero.Player.MaxMana*100 <= Menu["selflowmp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    if (!hero.Player.LSIsRecalling() && !hero.Player.InFountain())
                        UseSpell();
                }
            }
        }
    }
}
