using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp.Common;

using TargetSelector = PortAIO.TSManager; namespace Activators.Spells.Evaders
{
    class nocturneshroudofdarkness : CoreSpell
    {
        internal override string Name => "nocturneshroudofdarkness";
        internal override string DisplayName => "Shrowd of Darkness | W";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SpellShield,  MenuType.Zhonyas, MenuType.SelfMinMP };
        internal override int DefaultHP => 30;
        internal override int DefaultMP => 45;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Player.Mana / Player.MaxMana * 100 <
                Menu["selfminmp" + Name + "pct"].Cast<Slider>().CurrentValue)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId != Player.NetworkId)
                    continue;
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    continue;
                if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                if (Menu["ss" + Name + "all"].Cast<CheckBox>().CurrentValue)
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                        UseSpell();
                }

                if (Menu["ss" + Name + "cc"].Cast<CheckBox>().CurrentValue)
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                        UseSpell();
                }

                if (Menu["use" + Name + "norm"].Cast<CheckBox>().CurrentValue)
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        UseSpell();        
                }

                if (Menu["use" + Name + "ulti"].Cast<CheckBox>().CurrentValue)
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        UseSpell();
                }
            }
        }
    }
}
