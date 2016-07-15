
using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Items.Offensives
{
    class _3800 : CoreItem
    {
        internal override int Id => 3800;
        internal override int Priority => 5;
        internal override string Name => "Righteous";
        internal override string DisplayName => "Righteous Glory";
        internal override int Duration => 1000;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.EnemyLowHP, MenuType.SelfLowHP, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Player.Health / Player.MaxHealth * 100 <= Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp >= 3000 || Player.LSCountEnemiesInRange(Range) >= 1)
                {
                    UseItem();
                }
            }

            if (Tar != null)
            {
                if (Activator.omenu[Activator.omenu.UniqueMenuId + "useon" + Tar.Player.NetworkId] == null)
                {
                    return;
                }
                if (!Activator.omenu[Activator.omenu.UniqueMenuId + "useon" + Tar.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }

                if (Tar.Player.Health / Tar.Player.MaxHealth * 100 <= Menu["enemylowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp >= 3000)
                    {
                        UseItem();
                    }
                    else if (Player.LSCountEnemiesInRange(Range) >= 1)
                    {
                        UseItem(Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                    }
                }
            }
        }
    }
}
