#region

using System;
using System.Linq;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using EloBuddy;
using LeagueSharp.SDK;
using SharpDX;

#endregion

 namespace Infected_Twitch.Event
{
    internal class DrawDmg : Core.Core
    {
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        public static void Draw(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (!MenuConfig.DrawDmg) continue;

                Indicator.unit = enemy;
                Indicator.drawDmg(Dmg.EDamage(enemy), new ColorBGRA(255, 204, 0, 170));
            }
        }
    }
}
