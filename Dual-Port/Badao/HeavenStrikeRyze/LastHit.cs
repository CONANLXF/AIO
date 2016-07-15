using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK;


namespace HeavenStrikeRyze
{
    public static class LastHit
    {
        public static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static void BadaoActivate()
        {

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                return; 
            if (Player.Mana * 100 / Player.MaxMana > Program.ManaLastHit)
            {
                var targetq = MinionManager.GetMinions(Player.Position, Program._q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
                if (Program._q.IsReady() && Program.QlastHit && targetq != null && Orbwalker.CanMove && targetq.Health < Helper.Qdamage(targetq))
                {
                    Program._q.Cast(targetq);
                }
            }
        }
    }
}
