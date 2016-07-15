using System;
using System.Linq;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Summoners
{
    internal class exhuast : CoreSum
    {
        internal override string Name => "summonerexhaust";
        internal override string DisplayName => "Exhaust";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => 650f;
        internal override int Duration => 100;
        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            var hid = Activator.Heroes
                .OrderByDescending(h => h.Player.TotalAttackDamage)
                .FirstOrDefault(h => h.Player.LSIsValidTarget(Range + 250));

            foreach (var hero in Activator.Allies())
            {
                var attacker = hero.Attacker as AIHeroClient;
                if (attacker == null || hid == null)
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) > Range)
                    continue;

                if (attacker.LSDistance(hero.Player.ServerPosition) <= 1250)
                {
                    if (hero.HitTypes.Contains(HitType.ForceExhaust))
                    {
                        UseSpellOn(attacker);
                    }

                    if (!Activator.smenu[Parent.UniqueMenuId + "useon" + attacker.NetworkId].Cast<CheckBox>().CurrentValue)
                        continue;

                    if (Essentials.GetRole(attacker) == PrimaryRole.Support)
                        continue;

                    if (Menu["use" + Name + "ulti"].Cast<CheckBox>().CurrentValue)
                    {
                        if (hero.HitTypes.Contains(HitType.Ultimate))
                        {
                            if (Menu["f" + Name].Cast<CheckBox>().CurrentValue)
                                UseSpellOn(attacker);

                            else if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >= 45)
                                UseSpellOn(attacker, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);

                            else if (hero.Player.Health / hero.Player.MaxHealth * 100 <= 50)
                                UseSpellOn(attacker, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);

                            else if (hero.IncomeDamage >= hero.Player.Health)
                                UseSpellOn(attacker, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                        }
                    }

                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu["a" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if (!hero.Player.LSIsFacing(attacker))
                        {
                            if (attacker.NetworkId == hid.Player.NetworkId)
                            {
                                UseSpellOn(attacker, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                            }
                        }
                    }

                    if (attacker.Health / attacker.MaxHealth * 100 <=
                        Menu["e" + Name + "pct"].Cast<Slider>().CurrentValue)
                    {
                        if (!attacker.LSIsFacing(hero.Player))
                        {
                            UseSpellOn(attacker, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                        }
                    }
                }
            }
        }
    }
}
