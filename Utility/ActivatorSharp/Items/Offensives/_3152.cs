using System;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Items.Offensives
{
    class _3152 : CoreItem
    {
        internal override int Id => 3152;
        internal override int Priority => 5;
        internal override string Name => "Protobelt-01";
        internal override string DisplayName => "Hextech Protobelt-01";
        internal override int Duration => 100;
        internal override float Range => 300f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            if (Tar != null)
            {
                if (Activator.omenu[Activator.omenu.UniqueMenuId + "useon" + Tar.Player.NetworkId] == null)
                {
                    return;
                }
                if (!Activator.omenu[Activator.omenu.UniqueMenuId + "useon" + Tar.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    return;

                if ((Tar.Player.Health / Tar.Player.MaxHealth * 100) <= Menu["enemylowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    if (Tar.Player.LSDistance(Player.ServerPosition) > Range - 100 && !Tar.Player.LSIsFacing(Player) && !Tar.Player.IsMelee)
                    {
                        var endpos = Player.ServerPosition.LSTo2D() + Player.Direction.LSTo2D().LSPerpendicular() * Range;
                        if (endpos.To3D().LSCountEnemiesInRange(Range + (1 + Player.AttackRange + Player.LSDistance(Player.BBox.Minimum))) > 0)
                        {
                            UseItem(true);
                        }
                    }
                }

                if ((Player.Health / Player.MaxHealth * 100) <= Menu["selflowhp" + Name + "pct"].Cast<Slider>().CurrentValue)
                {
                    if (!Player.LSIsFacing(Tar.Player))
                    {
                        var endpos = Player.ServerPosition.LSTo2D() + Player.Direction.LSTo2D().LSPerpendicular() * Range;
                        if (endpos.To3D().LSCountEnemiesInRange(Range + (1 + Player.AttackRange + Player.LSDistance(Player.BBox.Minimum))) <= 1)
                        {
                            UseItem(true);
                        }
                    }
                }
            }
        }
    }
}
