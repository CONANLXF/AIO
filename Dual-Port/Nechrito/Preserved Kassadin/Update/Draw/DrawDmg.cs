using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using SharpDX;
using Preserved_Kassadin.Cores;

 namespace Preserved_Kassadin.Update.Draw
{
    class DrawDmg
    {
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();

        public static void Draw(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.LSIsValidTarget(1400) && !ene.IsZombie))
            {
                if (!MenuConfig.DrawDmg) continue;

                Indicator.unit = enemy;
                Indicator.drawDmg(Dmg.Damage(enemy), new ColorBGRA(255, 204, 0, 170));
            }
        }
    }
}
