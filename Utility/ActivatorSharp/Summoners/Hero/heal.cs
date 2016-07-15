using System;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Summoners
{
    internal class heal : CoreSum
    {
        internal override string Name => "summonerheal";
        internal override string DisplayName => "Heal";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => 850f;
        internal override int Duration => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (Activator.smenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                {
                    continue;
                }
                if (!Activator.smenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) <= Range)
                {
                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if (hero.IncomeDamage > 0 && !hero.Player.LSIsRecalling() && !hero.Player.InFountain())
                            UseSpell(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);

                        if (hero.TowerDamage > 0 && Menu["use" + Name + "tower"].Cast<CheckBox>().CurrentValue)
                            UseSpell(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                    }

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >= Menu["selfmuchhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if (hero.Player.MaxHealth - hero.Player.Health > 75 + 15 * Math.Min(Activator.Player.Level, 18))
                        {
                            if (!hero.Player.LSIsRecalling() && !hero.Player.InFountain())
                                UseSpell(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                        }
                    }
                }
            }
        }
    }
}
