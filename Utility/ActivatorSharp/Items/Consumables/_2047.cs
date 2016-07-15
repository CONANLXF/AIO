using System;
using Activators.Base;
using Activators.Handlers;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Items.Consumables
{
    class _2047 : CoreItem
    {
        internal override int Id => 2047;
        internal override string Name => "Oracle's Extract";
        internal override string DisplayName => "Oracle's Extract";
        internal override int Duration => 101;
        internal override int Priority => 5;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.Stealth, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.TwistedTreeline, MapType.CrystalScar, MapType.HowlingAbyss };
        internal override int DefaultHP => 0;
        internal override int DefaultMP => 0;

        public _2047()
        {
            Stealth.Init();
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.LSDistance(Player.ServerPosition) > Range)
                    continue;

                if (hero.HitTypes.Contains(HitType.Stealth))
                {
                    UseItem(hero.Player.ServerPosition, Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1);
                }
            }
        }
    }
}
