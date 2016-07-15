using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp.Common;

 namespace Activators.Spells.Heals
{
    class judicatorblessing : CoreSpell
    {
        internal override string Name => "judicatorblessing";
        internal override string DisplayName => "Divine Blessing | W";
        internal override float Range => 900f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.SelfMinMP };
        internal override int DefaultHP => 90;
        internal override int DefaultMP => 55;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Player.Mana/Player.MaxMana * 100 <
                Menu["selfminmp" + Name + "pct"].Cast<Slider>().CurrentValue)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    continue;
                if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue; 

                if (hero.Player.LSDistance(Player.ServerPosition) <= Range)
                {
                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                        UseSpellOn(hero.Player);

                    if (hero.IncomeDamage/hero.Player.MaxHealth*100 >=
                        Menu["selfmuchhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                        UseSpellOn(hero.Player);
                }
            }
        }
    }
}
