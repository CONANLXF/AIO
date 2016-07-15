using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace ThreshTherulerofthesoul
{
    public static class Extensions
    {
        #region Obj_AI_Hero

        public static float HpPercents(this AIHeroClient hero)
        {
            return hero.Health / hero.MaxHealth * 100;
        }

        public static float ManaPercents(this AIHeroClient hero)
        {
            return hero.Mana / hero.MaxMana * 100;
        }

        #endregion

    }
}
