using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;

 namespace Activators.Spells.Health
{
    class threshw : CoreSpell
    {
        internal override string Name => "threshw";
        internal override string DisplayName => "Dark Passage | W";
        internal override float Range => 950f;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas, MenuType.SelfCount };
        internal override int DefaultHP => 20;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.IsMe)
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) <= Range)
                {
                    if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                        continue;
                    if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                        continue;

                    if (!Player.HasBuffOfType(BuffType.Invulnerability))
                    {
                        if (Menu["use" + Name + "norm"].Cast<CheckBox>().CurrentValue)
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                                if (hero.Attacker != null)
                                    UseSpellTowards(Prediction.GetPrediction(hero.Player, 0.25f).UnitPosition);

                        if (Menu["use" + Name + "ulti"].Cast<CheckBox>().CurrentValue)
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                                if (hero.Attacker != null)
                                    UseSpellTowards(Prediction.GetPrediction(hero.Player, 0.25f).UnitPosition);

                        if (hero.Player.LSCountEnemiesInRange(300) >=
                            Menu["selfcount" + Name].Cast<Slider>().CurrentValue)
                                UseSpellTowards(Prediction.GetPrediction(hero.Player, 0.25f).UnitPosition);
                    }
                }
            }
        }
    }
}
