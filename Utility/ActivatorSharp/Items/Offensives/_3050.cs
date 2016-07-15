using System;
using System.Linq;
using Activators.Base;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Items.Offensives
{
    class _3050 : CoreItem
    {
        internal override int Id => 3050;
        internal override string Name => "Zekes";
        internal override string DisplayName => "Zeke's Harbringer";
        internal override int Duration => 100;
        internal override int Priority => 5;
        internal override float Range => 1000f;
        internal override MenuType[] Category => new[] { MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 99;
        internal override int DefaultMP => 99;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            var highadhero =
                Activator.Heroes.Where(x => x.Player.IsAlly && !x.Player.IsDead && !x.Player.IsMelee)
                    .OrderByDescending(x => x.Player.FlatPhysicalDamageMod + x.Player.BaseAttackDamage)
                    .FirstOrDefault();

            if (!highadhero.Player.IsMe && highadhero.Player.LSDistance(Player.ServerPosition) <= Range)
            {
                if (!highadhero.Player.HasBuff("rallyingbanneraurafriend"))
                {
                    UseItem(highadhero.Player, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                }
            }
        }
    }
}
