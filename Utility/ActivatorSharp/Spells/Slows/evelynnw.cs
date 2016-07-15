using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;

 namespace Activators.Spells.Slows
{
    class evelynnw : CoreSpell
    {
        internal override string Name => "evelynnw";
        internal override string DisplayName => "Dark Frenzy | W";
        internal override MenuType[] Category => new[] { MenuType.SlowRemoval, MenuType.ActiveCheck };

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Player.HasBuffOfType(BuffType.Slow) && Menu["use" + Name + "sr"].Cast<CheckBox>().CurrentValue)
            {
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + Player.NetworkId] == null)
                    return;
                if (!Activator.amenu[Parent.UniqueMenuId + "useon" + Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    return;

                UseSpell(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
            }
        }
    }
}
