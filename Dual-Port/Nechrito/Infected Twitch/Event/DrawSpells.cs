#region

using System;
using System.Drawing;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

#endregion

 namespace Infected_Twitch.Event
{
    internal class DrawSpells : Core.Core
    {
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (HasPassive)
            {
                var passiveTime = Math.Max(0, Player.GetBuff("TwitchHideInShadows").EndTime) - Game.Time;

                if (!MenuConfig.DrawTimer) return;
                Drawing.DrawText(heropos.X - 30, heropos.Y + 60, Color.White, "Q Time: " + passiveTime);
                Render.Circle.DrawCircle(Player.Position, passiveTime * Player.MoveSpeed, Color.Gray);
            }
            if (Target == null || Target.IsDead || Target.IsInvulnerable || !Target.LSIsValidTarget()) return;

            if (!MenuConfig.DrawKillable) return;

            if (Target.Health <= Dmg.EDamage(Target))
            {
                Drawing.DrawText(heropos.X - 60, heropos.Y + 120, Color.White, Target.Name + " Is Killable By Passive");
            }
        }
    }
}
