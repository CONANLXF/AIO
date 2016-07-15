using LeagueSharp;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

 namespace HastaKalistaBaby
{
    class DamageIndicator
    {
        private static Menu draw = Program.draw;
        private const int BarWidth = 104;
        private const int LineThickness = 9;

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private static DamageToUnitDelegate DamageToUnit { get; set; }

        private static readonly SharpDX.Vector2 BarOffset = new SharpDX.Vector2(-9, 11);

        private static Color _drawingColor;
        public static Color DrawingColor
        {
            get { return _drawingColor; }
            set { _drawingColor = Color.FromArgb(170, value); }
        }

        public static void Init(DamageToUnitDelegate damageToUnit)
        {
            // Apply needed field delegate for damage calculation
            DamageToUnit = damageToUnit;
            DrawingColor = Color.Green;

            // Register event handlers
            Drawing.OnDraw += DrawChampion;
        }

        private static void DrawChampion(EventArgs args)
        {
            if (!draw["healthp1"].Cast<CheckBox>().CurrentValue || !Program.E.IsReady())
            {
                return;
            }
            // For every enemis in E range
            foreach (var unit in HeroManager.Enemies.Where(unit => unit.IsValid && unit.IsHPBarRendered && Program.E.IsInRange(unit)))
            {
                const int xOffset = 10;
                const int yOffset = 20;
                const int width = 103;
                const int height = 8;

                var barPos = unit.HPBarPosition;
                var damage = Damage.GetEdamage(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + yOffset;
                var xPosDamage = barPos.X + xOffset + width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + xOffset + width * unit.Health / unit.MaxHealth;

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + height, 1, Color.Black);

                var differenceInHp = xPosCurrentHp - xPosDamage;
                var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                for (var i = 0; i < differenceInHp; i++)
                {
                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + height, 1, System.Drawing.Color.Purple);
                }
            }

        }
    }
}
