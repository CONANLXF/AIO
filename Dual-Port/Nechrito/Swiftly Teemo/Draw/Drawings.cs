#region

using System;
using LeagueSharp.SDK.Core.Utils;
using Swiftly_Teemo.Main;
using LeagueSharp.SDK;
using EloBuddy;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Swiftly_Teemo.Draw
{
    internal class Drawings : Core
    {
        public static HpBarDraw DrawHpBar = new HpBarDraw();
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (MenuConfig.EngageDraw)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.Q.Range,
                    Spells.Q.IsReady()
                        ? System.Drawing.Color.FromArgb(120, 0, 170, 255)
                        : System.Drawing.Color.IndianRed);
            }

            if (!MenuConfig.DrawR) return;
            if (!Target.LSIsValidTarget() || Target == null || Target.IsDead) return;
            if (!Spells.R.IsReady()) return;

            var rPrediction = Spells.R.GetPrediction(Target).UnitPosition;
            var newPos = Player.ServerPosition.LSExtend(rPrediction, Spells.R.Range);
            var ammo = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            if (ammo == 3)
            {
                Render.Circle.DrawCircle(rPrediction, 75, System.Drawing.Color.GhostWhite);
            }
            if (ammo < 3)
            {
                Render.Circle.DrawCircle(newPos, 60, System.Drawing.Color.Cyan);
            }
        }
    }
}
