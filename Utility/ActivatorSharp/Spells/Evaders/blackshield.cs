using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp.Common;

using TargetSelector = PortAIO.TSManager; namespace Activators.Spells.Evaders
{
    class blackshield : CoreSpell
    {
        internal override string Name => "blackshield";
        internal override string DisplayName => "Black Shield | E";
        internal override float Range => 750f;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas, MenuType.SpellShield, MenuType.SelfMinMP };
        internal override int DefaultHP => 0;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Player.Mana/Player.MaxMana*100 <
                Menu["selfminmp" + Name + "pct"].Cast<Slider>().CurrentValue)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    continue;
                if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) > Range)
                    continue;

                if (Menu["ss" + Name + "all"].Cast<CheckBox>().CurrentValue)
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                        UseSpellOn(hero.Player);                 

                if (Menu["ss" + Name + "cc"].Cast<CheckBox>().CurrentValue)
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
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
