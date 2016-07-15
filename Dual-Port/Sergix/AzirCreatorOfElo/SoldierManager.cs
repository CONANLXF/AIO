using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace Azir_Creator_of_Elo
{
    class SoldierManager
    {
        private static Dictionary<int, string> Animations = new Dictionary<int, string>();
        private static List<Obj_AI_Minion> _soldiers = new List<Obj_AI_Minion>();
        public List<Obj_AI_Minion> Soldiers
        {
            get
            {
                return _soldiers;
            }
        }
        public SoldierManager()
        {
            Obj_AI_Minion.OnCreate += Obj_AI_Minion_OnCreate;
            Obj_AI_Minion.OnDelete += Obj_AI_Minion_OnDelete;
            Obj_AI_Minion.OnPlayAnimation += Obj_AI_Minion_OnPlayAnimation;

            Drawing.OnDraw += Drawing_OnDraw;

        }
        public List<Obj_AI_Minion> ActiveSoldiers
        {
            get { return _soldiers.Where(s => s.IsValid && !s.IsDead && !s.IsMoving && (!Animations.ContainsKey(s.NetworkId) || Animations[s.NetworkId] != "Inactive")).ToList(); }
        }
        public Obj_AI_Minion getSoldierOnRange(int range)
        {
            foreach (Obj_AI_Minion sol in ActiveSoldiers)
            {
                if (sol.LSDistance(HeroManager.Player.ServerPosition) <= range)
                {
                    return sol;
                }
            }

            return null;
        }

        public int SoldiersAttackingn(AzirMain azir)
        {
            var soldiersa = 0;
            foreach (Obj_AI_Minion m in azir.soldierManager.Soldiers)
            {
                if (!m.IsDead)
                    foreach (AIHeroClient h in HeroManager.Enemies)
                    {
                        if (!h.IsDead)
                        if (m.LSDistance(h) > 315)
                        {
                            soldiersa++;
                            break;
                        }
                        else
                        {


                        }
                    }
            }
            return soldiersa;
        }
        private void Obj_AI_Minion_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender is Obj_AI_Minion && IsSoldier((Obj_AI_Minion)sender))
            {
                Animations[sender.NetworkId] = args.Animation;
            }
        }

        private void Obj_AI_Minion_OnDelete(GameObject sender, EventArgs args)
        {
            _soldiers.RemoveAll(s => s.NetworkId == sender.NetworkId);
            //   Animations.Remove(sender.NetworkId);
        }
        private bool IsSoldier(Obj_AI_Minion soldier)
        {
            return soldier.IsAlly && String.Equals(soldier.BaseSkinName, "azirsoldier", StringComparison.InvariantCultureIgnoreCase);
        }
        private void Obj_AI_Minion_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender is Obj_AI_Minion && IsSoldier((Obj_AI_Minion)sender))
            {
                _soldiers.Add((Obj_AI_Minion)sender);
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {

            //     Game.PrintChat(""+ ActiveSoldiers.Count);
        }

        public bool SoldiersAttacking(AzirMain azir)
        {
            foreach (Obj_AI_Minion m in azir.soldierManager.Soldiers)
            {
                foreach (AIHeroClient h in HeroManager.Enemies)
                {
                    if (m.LSDistance(h) > 315)
                    {

                        return true;
                    }
                }
            }

            return false;
        }

    }
}
