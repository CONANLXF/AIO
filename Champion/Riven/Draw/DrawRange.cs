#region

using System;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;
using EloBuddy;

#endregion

using TargetSelector = PortAIO.TSManager; namespace NechritoRiven.Draw
{
    internal class DrawRange : Core.Core
    {
        public static void RangeDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var heropos = Drawing.WorldToScreen(Player.Position);

            if (MenuConfig.DrawCb)
                Render.Circle.DrawCircle(Player.Position, 250 + Player.AttackRange + 70,
                    Spells.E.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);

            if (MenuConfig.DrawBt && Spells.Flash != SpellSlot.Unknown)
                Render.Circle.DrawCircle(Player.Position, 750,
                    Spells.R.IsReady() && Spells.Flash.IsReady()
                        ? System.Drawing.Color.FromArgb(120, 0, 170, 255)
                        : System.Drawing.Color.IndianRed);

            if (MenuConfig.DrawFh)
                Render.Circle.DrawCircle(Player.Position, 450 + Player.AttackRange + 70,
                    Spells.E.IsReady() && Spells.Q.IsReady()
                        ? System.Drawing.Color.FromArgb(120, 0, 170, 255)
                        : System.Drawing.Color.IndianRed);

            if (MenuConfig.DrawHs)
                Render.Circle.DrawCircle(Player.Position, 400,
                    Spells.Q.IsReady() && Spells.W.IsReady()
                        ? System.Drawing.Color.FromArgb(120, 0, 170, 255)
                        : System.Drawing.Color.IndianRed);

            if (MenuConfig.DrawAlwaysR)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 20, System.Drawing.Color.Cyan, "Force R  (     )");
                Drawing.DrawText(heropos.X + 53, heropos.Y + 20,
                    MenuConfig.AlwaysR ? System.Drawing.Color.White : System.Drawing.Color.Red, MenuConfig.AlwaysR ? "On" : "Off");
            }
            if (MenuConfig.ForceFlash)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.Cyan, "Force Flash  (     )");
                Drawing.DrawText(heropos.X + 83, heropos.Y + 40,
                    MenuConfig.AlwaysF ? System.Drawing.Color.White : System.Drawing.Color.Red, MenuConfig.AlwaysF ? "On" : "Off");
            }
        }
    }
}
