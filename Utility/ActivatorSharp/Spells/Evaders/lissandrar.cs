using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp.Common;

 namespace Activators.Spells.Evaders
{
    class lissandrar : CoreSpell
    {
        internal override string Name => "lissandrar";
        internal override string DisplayName => "Frozen Tomb | R";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas };
        internal override int DefaultHP => 30;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                        continue;
                    if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                        continue;

                    if (hero.Player.LSCountEnemiesInRange(425) >= 1)
                    {
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
