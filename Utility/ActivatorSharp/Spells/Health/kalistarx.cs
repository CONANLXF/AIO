using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using System.Linq;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;

 namespace Activators.Spells.Health
{
    internal class kalistarx : CoreSpell
    {
        internal override string Name => "kalistarx";
        internal override string DisplayName => "Fate's Call | R";
        internal override float Range => 1200f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP };
        internal override int DefaultHP => 20;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            var cooptarget =
                ObjectManager.Get<AIHeroClient>()
                    .FirstOrDefault(hero => hero.HasBuff("kalistacoopstrikeally"));

            foreach (var hero in Activator.Allies())
            {
                if (cooptarget?.NetworkId == hero.Player.NetworkId)
                {
                    if (hero.Player.LSDistance(cooptarget.ServerPosition) <= Range)
                    {
                        if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                            continue;
                        if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                            continue;

                        if (!cooptarget.HasBuffOfType(BuffType.Invulnerability))
                        {
                            if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                                Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                            {
                                if (hero.IncomeDamage > 0)
                                    UseSpell();
                            }
                        }
                    }
                }
            }
        }
    }
}
