using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

using TargetSelector = PortAIO.TSManager; namespace ElEkko
{
    internal class Drawings
    {
        public static void OnDraw(EventArgs args)
        {
            var drawOff = ElEkkoMenu.Misc["ElEkko.Draw.off"].Cast<CheckBox>().CurrentValue;
            var drawQ = ElEkkoMenu.Misc["ElEkko.Draw.Q"].Cast<CheckBox>().CurrentValue;
            var drawE = ElEkkoMenu.Misc["ElEkko.Draw.E"].Cast<CheckBox>().CurrentValue;
            var drawR = ElEkkoMenu.Misc["ElEkko.Draw.R"].Cast<CheckBox>().CurrentValue;

            if (drawOff)
            {
                return;
            }

            if (drawQ)
            {
                if (ElEkko.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(EloBuddy.ObjectManager.Player.Position, ElEkko.spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawE)
            {
                if (ElEkko.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(EloBuddy.ObjectManager.Player.Position, ElEkko.spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR)
            {
                if (ElEkko.spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(EloBuddy.ObjectManager.Player.Position, ElEkko.spells[Spells.R].Range, Color.White);
                }
            }
        }
    }
}
