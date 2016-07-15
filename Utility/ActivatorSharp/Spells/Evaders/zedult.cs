using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;

 namespace Activators.Spells.Evaders
{
    class zedult : CoreSpell
    {
        internal override string Name => "ZedR";
        internal override string DisplayName => "Death Mark | R";
        internal override float Range => 625f;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas };

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Player.GetSpell(SpellSlot.R).Name != "ZedR")
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                        continue;

                    if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                        continue;

                    if (hero.Attacker == null)
                        continue;

                    if (hero.Attacker.LSDistance(hero.Player.ServerPosition) > Range)
                        continue;

                    if (Menu["use" + Name + "norm"].Cast<CheckBox>().CurrentValue)
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            CastOnBestTarget((AIHeroClient) hero.Attacker);

                    if (Menu["use" + Name + "ulti"].Cast<CheckBox>().CurrentValue)
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            CastOnBestTarget((AIHeroClient) hero.Attacker);
                }
            }
        }
    }
}
