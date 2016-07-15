using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp.Common;

using TargetSelector = PortAIO.TSManager; namespace Activators.Spells.Heals
{
    class gangplankw : CoreSpell
    {
        internal override string Name => "gangplankw";
        internal override string DisplayName => "Remove Scurvy | W";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMinMP };
        internal override int DefaultHP => 20;
        internal override int DefaultMP => 55;

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
                    return;
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    continue;
                if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue; 

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                    Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    UseSpell();
            }
        }
    }
}
