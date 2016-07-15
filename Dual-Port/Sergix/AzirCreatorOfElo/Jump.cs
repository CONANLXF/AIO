using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace Azir_Creator_of_Elo
{

    class JumpLogic
    {
        AzirMain azir;

        public JumpLogic(AzirMain azir)
        {
            this.azir = azir;
        }
        public void updateLogic(Vector3 position)
        {
            if (azir.Spells.W.IsReady() && azir.Spells.Q.IsReady() && azir.Spells.E.IsReady())//&&R.IsReady())
            {
                //   if (azir.soldierManager.ActiveSoldiers.Count == 0|| azir.soldierManager.ActiveSoldiers.Min(t=>t.Distance(Game.CursorPos))>azir.Hero.Distance(Game.CursorPos) )
                // {
                azir.Spells.W.Cast(HeroManager.Player.Position.LSExtend(position, 450));
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 150, () => azir.Spells.E.Cast(azir.soldierManager.Soldiers[azir.soldierManager.Soldiers.Count - 1].ServerPosition));
                //}
                //else
                //{
                //    Utility.DelayAction.Add(Game.Ping + 150, () => azir.Spells.E.Cast(azir.soldierManager.Soldiers[azir.soldierManager.Soldiers.Count - 1].ServerPosition));
                // }

                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 400, () => fleeq());
            }
        }
        public void fleeTopos(Vector3 position)
        {
            if (azir.Spells.W.IsReady() && azir.Spells.Q.IsReady() && azir.Spells.E.IsReady())//&&R.IsReady())
            {
                azir.Spells.W.Cast(HeroManager.Player.Position.LSExtend(position, 450));
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 150, () => azir.Spells.E.Cast(azir.soldierManager.Soldiers[azir.soldierManager.Soldiers.Count - 1].ServerPosition));
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 400, () => fleeq(position));
            }
        }
        public void fleeq()
        {
            if (Vector2.Distance(HeroManager.Player.ServerPosition.LSTo2D(), Game.CursorPos.LSTo2D()) < azir.Spells.Q.Range)
            {
                azir.Spells.Q.Cast(Game.CursorPos);
            }
            else
            {
                azir.Spells.Q.Cast(HeroManager.Player.Position.LSExtend(Game.CursorPos, 1150));
            }
        }
        public void fleeq(Vector3 position)
        {
            azir.Spells.Q.Cast(position);
        }
        public void insec(AIHeroClient target)
        {

            if (azir.Hero.LSDistance(target) <= azir.Spells.R.Range)
            {

                var tower = ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(it => it.IsAlly && it.LSIsValidTarget(1000));

                if (tower != null)
                {
                    if (azir.Spells.R.Cast(tower.ServerPosition)) return;
                }

                if (azir.Spells.R.Cast(Game.CursorPos)) return;



            }
            else
            {
                var pos = Game.CursorPos.LSExtend(target.Position, Game.CursorPos.LSDistance(target.Position) - 250);
                if (pos.LSDistance(azir.Hero.ServerPosition) <= 1300)
                {
                    fleeTopos(pos);
                }
            }

        }
    }
}
