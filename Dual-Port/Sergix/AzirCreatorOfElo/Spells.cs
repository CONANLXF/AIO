
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using Azir_Free_elo_Machine.Math;

using TargetSelector = PortAIO.TSManager; namespace Azir_Creator_of_Elo
{
    class Spells
    {
        private Spell _q, _w, _e, _r;
        public Spell Q { get { return _q; } }
        public Spell W { get { return _w; } }
        public Spell E { get { return _e; } }
        public Spell R { get { return _r; } }
        public Spells()
        {
            _q = new Spell(SpellSlot.Q, 825);


            _w = new Spell(SpellSlot.W, 450);
            _e = new Spell(SpellSlot.E, 1250);
            _r = new Spell(SpellSlot.R, 450);

            _q.SetSkillshot(0, 70, 1600, false, SkillshotType.SkillshotCircle);
            _e.SetSkillshot(0, 100, 1700, false, SkillshotType.SkillshotLine);
            _r.SetSkillshot(0.5f, 0, 1400, false, SkillshotType.SkillshotLine);
            //  ignite = ObjectManager.Player.GetSpellSlot("SummonerDot");
        }

        public void castQ(AzirMain azir, AIHeroClient target, bool useQ, int nSoldiersToQ)
        {
            var points = Azir_Free_elo_Machine.Math.Geometry.PointsAroundTheTarget(target.ServerPosition, 320);
            List<Azir_Free_elo_Machine.Math.Points> pointsAttack = new List<Points>();
            foreach (Vector3 point in points)
            {
                if (point.LSDistance(azir.Hero.ServerPosition) <= azir.Spells.Q.Range)
                {


                    pointsAttack.Add(new Points(Azir_Free_elo_Machine.Math.Geometry.Nattacks(azir, point, target), point));
                    //    var spaceAzirQ =azir.Spells.Q.Speed*time;
                    //  var spacetargetpos = Prediction.GetPrediction(target, time);

                }
            }
            if (pointsAttack.MaxOrDefault(x => x.hits).hits > 0)
            {
                //    Game.PrintChat("Attacks : "+ pointsAttack.MaxOrDefault(x => x.hits).hits);
                Q.Cast(pointsAttack.MaxOrDefault(x => x.hits).point);
            }
        }
    }
}