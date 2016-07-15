using System; using EloBuddy.SDK.Menu.Values; using EloBuddy;
using Activators.Base;

 namespace Activators.Spells.Evaders
{
    class vladimirsanguinepool : CoreSpell
    {
        internal override string Name => "vladimirsanguinepool";
        internal override string DisplayName => "Sanguine Pool | W";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas };
        internal override int DefaultHP => 0;
        internal override int DefaultMP => 45;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId != Player.NetworkId)
                    continue;
                if (Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId] == null)
                    continue;
                if (!Activator.amenu[Parent.UniqueMenuId + "useon" + hero.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

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
