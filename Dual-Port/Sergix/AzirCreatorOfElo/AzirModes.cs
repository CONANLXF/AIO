using Azir_Free_elo_Machine;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir_Creator_of_Elo
{
    class AzirModes : Modes
    {
        public JumpLogic jump;
#pragma warning disable 0649
        public Insec insec;
#pragma warning restore 0649
        public AzirModes(AzirMain azir)
        {
            jump = new JumpLogic(azir);
        }
        public override void Update(AzirMain azir)
        {

            base.Update(azir);



            if (Menu._jumpMenu["fleekey"].Cast<KeyBind>().CurrentValue)
            {
                azir.Orbwalk(Game.CursorPos);
                Jump(azir);
            }
        }
        /*public void Insec(AzirMain azir)
        {
            var ts = TargetSelector.GetTarget(azir.Spells.Q.Range, DamageType.Magical);
            if (ts != null)
            {

                if (azir.Spells.R.IsReady())
                    jump.insec(ts);

            }
        }*/
        public void Jump(AzirMain azir)
        {
            jump.updateLogic(Game.CursorPos);

        }

        public override void Harash(AzirMain azir)
        {

            var wCount = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Ammo;
            var useQ = Menu._harashMenu["HQ"].Cast<CheckBox>().CurrentValue;
            var useW = Menu._harashMenu["HW"].Cast<CheckBox>().CurrentValue;
            var savew = Menu._harashMenu["HW2"].Cast<CheckBox>().CurrentValue;
            var nSoldiersToQ = Menu._harashMenu["hSoldiersToQ"].Cast<Slider>().CurrentValue;
            base.Harash(azir);
            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (target != null)
            {


                if (target.Distance(azir.Hero.ServerPosition) < 450)
                {
                    var pred = azir.Spells.W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        if (savew && (wCount == 1))
                        {

                        }
                        else
                        {
                            if (useW)
                                if (azir.Spells.W.IsReady())
                                    azir.Spells.W.Cast(pred.CastPosition);
                        }
                    }
                }
                else
                {
                    //    if (azir.Spells.Q.Level > 0 && azir.Spells.Q.IsReady())
                    //      if((!savew &&(savew&& (wCount > 0))))
                    if (savew && (wCount == 1))
                    {

                    }
                    else
                    {
                        if (useW)
                            azir.Spells.W.Cast(azir.Hero.Position.Extend(target.ServerPosition, 450));
                    }
                }
                if (azir.soldierManager.SoldiersAttackingn(azir) <= nSoldiersToQ)
                    StaticSpells.CastQ(azir, target, useQ, nSoldiersToQ);

            }
        }
        public override void Laneclear(AzirMain azir)
        {
            var useQ = Menu._laneClearMenu["LQ"].Cast<CheckBox>().CurrentValue;
            var useW = Menu._laneClearMenu["LW"].Cast<CheckBox>().CurrentValue;
            base.Laneclear(azir);
            var minion = MinionManager.GetMinions(azir.Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion != null)
            {
                if (azir.Spells.W.IsInRange(minion))
                {
                    var pred = azir.Spells.W.GetPrediction(minion);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        if (useW)
                            azir.Spells.W.Cast(pred.CastPosition);
                    }
                    if (azir.soldierManager.SoldiersAttacking(azir) == false && azir.soldierManager.ActiveSoldiers.Count > 0)
                    {
                        pred = azir.Spells.Q.GetPrediction(minion);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            if (useQ)
                                azir.Spells.Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }
        public override void Jungleclear(AzirMain azir)
        {
            var useW = Menu._JungleClearMenu["JW"].Cast<CheckBox>().CurrentValue;
            base.Jungleclear(azir);
            var minion = MinionManager.GetMinions(azir.Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion != null)
            {
                if (azir.Spells.W.IsInRange(minion))
                {
                    var pred = azir.Spells.W.GetPrediction(minion);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        if (useW)
                            azir.Spells.W.Cast(pred.CastPosition);
                    }


                }
            }
        }

        public override void Combo(AzirMain azir)
        {

            var useQ = Menu._comboMenu["CQ"].Cast<CheckBox>().CurrentValue;
            var useW = Menu._comboMenu["CW"].Cast<CheckBox>().CurrentValue;
            var nSoldiersToQ = Menu._comboMenu["SoldiersToQ"].Cast<Slider>().CurrentValue;
            base.Combo(azir);
            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (target == null) return;

            if (target.Distance(azir.Hero.ServerPosition) < 450)
            {
                if (target.isRunningOfYou())
                {
                    var pos = LeagueSharp.Common.Prediction.GetPrediction(target, 0.5f).UnitPosition;
                    azir.Spells.W.Cast(pos);
                }
                else
                {
                    var pred = azir.Spells.W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        if (useW)
                            azir.Spells.W.Cast(pred.CastPosition);
                    }
                }
            }
            else
            {
                if (azir.Spells.Q.Level > 0 && azir.Spells.Q.IsReady())
                    if (useW)
                        if (target.Distance(HeroManager.Player) <= 750)
                            azir.Spells.W.Cast(azir.Hero.Position.Extend(target.ServerPosition, 450));
            }
            //Q

            if (azir.Spells.Q.IsReady() && azir.soldierManager.Soldiers.Count > 0 && azir.soldierManager.SoldiersAttackingn(azir) >= nSoldiersToQ)
            {

                StaticSpells.CastQ(azir, target, useQ, nSoldiersToQ);
            }
            if (azir.Spells.Q.IsKillable(target) && useQ)
            {
                if (target.Health < azir.Spells.Q.GetDamage(target))
                {
                    var pred = azir.Spells.Q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        //       Game.PrintChat("Killeable with q");
                        azir.Spells.Q.Cast(pred.CastPosition);
                    }
                }
            }
            else if (azir.Spells.R.IsKillable(target))
            {
                if (Menu._comboMenu["CR"].Cast<CheckBox>().CurrentValue)
                {
                    if (target.Health < azir.Spells.R.GetDamage(target))
                    {
                        var pred = azir.Spells.R.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.High)
                        {

                            azir.Spells.R.Cast(pred.CastPosition);
                        }
                    }
                    //      azir.Spells.R.Cast(target);

                }
            }
        }
    }
}