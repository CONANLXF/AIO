using System;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Items.Offensives
{
    class _3042 : CoreItem
    {
        internal override int Id => 3042;
        internal override int Priority => 5;
        internal override string Name => "Muramana";
        internal override string DisplayName => "Muramana";
        internal override float Range => float.MaxValue;
        internal override int Duration => 100;
        internal override MenuType[] Category => new[] { MenuType.SelfMinMP,  MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.SummonersRift, MapType.HowlingAbyss };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 35;

        public _3042()
        {
            // Obj_AI_Base.OnProcessSpellCast += OnCast;
        }

        //private bool muramana;
        public override void OnTick(EventArgs args)
        {
            return;
            /*
            if (muramana)
            {
                if (Player.Mana / Player.MaxMana * 100 <
                    Menu["selfminmp" + Name + "pct"].Cast<Slider>().CurrentValue)
                    return;

                if (Menu["mode" + Name].Cast<ComboBox>().CurrentValue != 1 ||
                    Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
                {
                    var manamune = Player.GetSpellSlot("Muramana");
                    if (manamune != SpellSlot.Unknown && !Player.HasBuff("Muramana"))
                    {
                        Player.Spellbook.CastSpell(manamune);
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => muramana = false);
                    }
                }
            }

            if (!muramana && !Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
            {
                var manamune = Player.GetSpellSlot("Muramana");
                if (manamune != SpellSlot.Unknown && Player.HasBuff("Muramana"))
                {
                    Player.Spellbook.CastSpell(manamune);
                }
            }
            */
        }
    }
}
