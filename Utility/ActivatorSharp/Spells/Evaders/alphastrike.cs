using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;

using TargetSelector = PortAIO.TSManager; namespace Activators.Spells.Evaders
{
    class alphastrike : CoreSpell
    {
        internal override string Name => "alphastrike";
        internal override string DisplayName => "Alpha Strike | Q";
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.SpellShield, MenuType.Zhonyas };

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.Attacker == null)
                        continue;
                    if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                        continue;
                    if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                        continue;

                    if (hero.Attacker.LSDistance(hero.Player.ServerPosition) <= Range)
                    {
                        if (Menu["ss" + Name + "all"].Cast<CheckBox>().CurrentValue)
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                                CastOnBestTarget((AIHeroClient) hero.Attacker);

                        if (Menu["ss" + Name + "cc"].Cast<CheckBox>().CurrentValue)
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                                CastOnBestTarget((AIHeroClient) hero.Attacker);

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
}
