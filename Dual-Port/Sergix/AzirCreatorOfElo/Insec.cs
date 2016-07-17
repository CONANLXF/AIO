using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using System.Text.RegularExpressions;
using Color = System.Drawing.Color;
using Azir_Creator_of_Elo;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

 namespace Azir_Free_elo_Machine
{
    class Insec
    {
        public enum Steps
        {
            firstCalcs = 0,
            w = 1,
            e = 2,
            q = 3,
            R = 4,
        }
        private Steps steps;
        Azir_Creator_of_Elo.AzirMain azir;
        public Insec(AzirMain azir)
        {
            steps = Steps.firstCalcs;
            Clickposition = new Vector3(0, 0, 0);
            this.azir = azir;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            var target = TargetSelector.SelectedTarget;
            /*     var posWs = GeoAndExten.GetWsPosition(target.Position.To2D()).Where(x => x != null);
                 foreach (var posW in posWs)
                 {
                 }*/
            if (Clickposition == new Vector3(0, 0, 0))
            {
                if (target != null)
                {
                    if (target.IsVisible && target.IsValid)
                    {
                        var pos = target.ServerPosition.LSExtend(Game.CursorPos, -300);
                        Render.Circle.DrawCircle(pos, 100, System.Drawing.Color.GreenYellow);
                    }
                }
            }
            else
            {
                var pos = target.ServerPosition.LSExtend(Clickposition, -300);
                Render.Circle.DrawCircle(pos, 100, System.Drawing.Color.GreenYellow);
                Render.Circle.DrawCircle(Clickposition, 100, System.Drawing.Color.GreenYellow);
            }

        }
        Vector3 Clickposition;
        private void Game_OnWndProc(WndEventArgs args)
        {




            if (args.Msg != (uint)WindowsMessages.WM_KEYDOWN)
            {
                return;
            }
            switch (args.WParam)
            {
                case 'G':
                    if (Clickposition == new Vector3(0, 0, 0))
                        Clickposition = Game.CursorPos;
                    else
                        Clickposition = new Vector3(0, 0, 0);

                    break;


            }




        }
        private void Game_OnUpdate(EventArgs args)
        {
            return;
            /*
            if (!azir.Spells.R.IsReady()) return;
            var insecPoint = new Vector3(0, 2, 3);
            if (Clickposition == new Vector3(0, 0, 0))
                insecPoint = Game.CursorPos;
            else
                insecPoint = Clickposition;

            if (!Menu._jumpMenu["inseckey"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }
            azir.Orbwalk(Game.CursorPos);

            if (!insecPoint.IsValid())
                return;
            var target = TargetSelector.SelectedTarget;
            if (!target.IsValidTarget() || target.IsZombie)
            {

                steps = Steps.firstCalcs;
                return;
            }

            var insecPos = new Vector3(0, 0, 0);
            if (Clickposition == new Vector3(0, 0, 0))
            {

                insecPos = target.ServerPosition.LSExtend(Game.CursorPos, -300);
            }
            else
            {
                insecPos = target.ServerPosition.LSExtend(insecPoint, -300);
            }
            switch (steps)
            {
                case Steps.firstCalcs:
                    if (insecPoint.Distance(HeroManager.Player.ServerPosition) > azir.Spells.Q.Range)
                    {
                        azir._modes.jump.fleeTopos(insecPoint);
                        steps = steps = Steps.R;
                    }
                    break;
                case Steps.w:
                    break;
                case Steps.e:
                    break;
                case Steps.q:
                    break;
                case Steps.R:
                    Vector2 start1 = HeroManager.Player.Position.To2D().Extend(insecPos.To2D(), -300);
                    Vector2 end1 = start1.Extend(HeroManager.Player.Position.To2D(), 750);
                    float width1 = HeroManager.Player.Level == 3 ? 125 * 6 / 2 :
                               HeroManager.Player.Level == 2 ? 125 * 5 / 2 :
                               125 * 4 / 2;
                    var Rect1 = new LeagueSharp.Common.Geometry.Polygon.Rectangle(start1, end1, width1 - 100);
                    var Predicted1 = LeagueSharp.Common.Prediction.GetPrediction(target, Game.Ping / 1000f + 0.25f).UnitPosition;
                    if (Rect1.IsInside(target.Position) && Rect1.IsInside(Predicted1))
                    {
                        azir.Spells.R.Cast(insecPoint);
                        return;
                    }
                    break;
            }
            */
        }

        private void castWOnAngle(Vector2 playerPos, Vector2 targetPos, float ag)
        {
            var posW = playerPos.LSExtend(targetPos, azir.Spells.W.Range);
            if (!RotatePoint(posW, playerPos, ag).LSIsWall())
                azir.Spells.W.Cast(RotatePoint(posW, playerPos, ag));
        }
        public Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angleInRadians)
        {
            double cosTheta = System.Math.Cos(angleInRadians);
            double sinTheta = System.Math.Sin(angleInRadians);
            return new Vector2
            {
                X =
                    (float)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (float)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
}
