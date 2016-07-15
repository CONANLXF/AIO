using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp.Common;

 namespace Activators.Spells.Evaders
{
    class judicatorintervention : CoreSpell
    {
        internal override string Name => "judicatorintervention";
        internal override string DisplayName => "Intervention | R";
        internal override float Range => 900f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP,  MenuType.Zhonyas };
        internal override int DefaultHP => 10;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    continue;
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                {
                    if (hero.Player.LSDistance(Player.ServerPosition) <= Range)
                    {
                        if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                            Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                            if (hero.IncomeDamage > 0)
                                UseSpellOn(hero.Player);

                        if (Menu["use" + Name + "norm"].Cast<CheckBox>().CurrentValue)
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                                UseSpellOn(hero.Player);

                        if (Menu["use" + Name + "ulti"].Cast<CheckBox>().CurrentValue)
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                                UseSpellOn(hero.Player);
                    }
                }
            }
        }
    }
}
