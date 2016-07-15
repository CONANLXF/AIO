using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Summoners
{
    internal class barrier : CoreSum
    {
        internal override string Name => "summonerbarrier";
        internal override string DisplayName => "Barrier";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => float.MaxValue;
        internal override int Duration => 1500;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId != Player.NetworkId)
                    continue;

                if (Activator.smenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                {
                    continue;
                }

                if (!Activator.smenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                    Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    if (hero.IncomeDamage > 0 && !hero.Player.LSIsRecalling() && !hero.Player.InFountain())
                        UseSpell(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);

                    if (hero.TowerDamage > 0 && Menu["use" + Name + "tower"].Cast<CheckBox>().CurrentValue)
                        UseSpell(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                }

                if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                    Menu["selfmuchhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    UseSpell(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);

                if (Menu["use" + Name + "ulti"].Cast<CheckBox>().CurrentValue)
                {
                    if (hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        if (Menu["f" + Name].Cast<CheckBox>().CurrentValue)
                            UseSpell();

                        else if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                                 Menu["selfmuchhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                        {
                            UseSpell();
                        }

                        else if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                                 Math.Min(100, Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue + 20))
                        {
                            UseSpell();
                        }

                        else if (hero.IncomeDamage >= hero.Player.Health)
                        {
                            UseSpell();
                        }
                    }
                }
            }
        }
    }
}
