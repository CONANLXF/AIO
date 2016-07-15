using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Items.Consumables
{
    class _2031 : CoreItem
    {
        internal override int Id => 2031;
        internal override int Priority => 3;
        internal override string Name => "Refillable Pot";
        internal override string DisplayName => "Refillable Pot";
        internal override int Duration => 101;
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.Player.HasBuff("ItemCrystalFlask"))
                        return;

                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if ((hero.IncomeDamage > 0 || hero.MinionDamage > 0 || hero.TowerDamage > 0) ||
                            !Menu["use" + Name + "cbat"].Cast<CheckBox>().CurrentValue)
                        {
                            if (!hero.Player.LSIsRecalling() && !hero.Player.InFountain())
                                UseItem();
                        }
                    }

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu["selfmuchhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if (!hero.Player.LSIsRecalling() && !hero.Player.InFountain())
                            UseItem();
                    }
                }
            }
        }
    }
}
