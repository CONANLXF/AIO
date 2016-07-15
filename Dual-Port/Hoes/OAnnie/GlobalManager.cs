using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

 namespace OAnnie
{
    class GlobalManager : Annie
    {
        private static DamageToUnitDelegate _damageToUnit;
        public static bool EnableDrawingDamage { get; set; }
        public static System.Drawing.Color DamageFillColor { get; set; }
        public delegate float DamageToUnitDelegate(AIHeroClient hero);


        public static float GetComboDamage(AIHeroClient enemy)
        {
            var damage = 0d;
            if (Q.IsReady())
                damage += Player.LSGetSpellDamage(enemy, SpellSlot.Q);

            if (R.IsReady())
                damage += Player.LSGetSpellDamage(enemy, SpellSlot.R);

            if (W.IsReady())
                damage += Player.LSGetSpellDamage(enemy, SpellSlot.W);
            if (Ignite.IsReady())
                damage += IgniteDamage(enemy);

            return (float)damage;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += DrawManager.Drawing_OnDrawChamp;
                }
                _damageToUnit = value;
            }
        }
    }
}
