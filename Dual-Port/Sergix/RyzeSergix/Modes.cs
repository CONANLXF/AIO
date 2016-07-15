using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TargetSelector = PortAIO.TSManager; namespace RyzeAssembly
{
    class Modes
    {
        
        public void Update(RyzeMain ryze)
        {
            ryze.Spells.igniteCast();

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo(ryze);
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                mixed(ryze);
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                JungleClear(ryze);
                LaneClear(ryze);
            }
        }

        private void LaneClear(RyzeMain ryze)
        {
            var Mana = Menu._laneclearMenu["ManaL"].Cast<Slider>().CurrentValue;
            var laneclearQ = Menu._laneclearMenu["QL"].Cast<CheckBox>().CurrentValue;
            var laneclearW = Menu._laneclearMenu["WL"].Cast<CheckBox>().CurrentValue;
            var laneclearE = Menu._laneclearMenu["EL"].Cast<CheckBox>().CurrentValue;
            var laneclearR = Menu._laneclearMenu["RL"].Cast<CheckBox>().CurrentValue;
            var minion = MinionManager.GetMinions(ryze.Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (ryze.Hero.ManaPercent > Mana)
            {
                if (minion != null)
                {
                    if (laneclearQ && ryze.Spells.Q.IsReady())
                    {
                        var Qpred = ryze.Spells.Q.GetPrediction(minion);
                        ryze.Spells.Q.Cast(Qpred.UnitPosition);
                    }
                    if (laneclearE && ryze.Spells.E.IsReady())
                    {
                        ryze.Spells.E.Cast(minion);
                    }
                    if (laneclearW && ryze.Spells.W.IsReady())
                    {
                        ryze.Spells.W.Cast(minion);
                    }
                    if (laneclearR && ryze.Spells.R.IsReady() && (ryze.GetPassiveBuff >= 4 || ryze.Hero.HasBuff("ryzepassivecharged")))
                    {
                        ryze.Spells.R.Cast();
                    }
                }
            }
        }

        private static void JungleClear(RyzeMain ryze)
        {
            var Mana = Menu._jungleclearMenu["ManaJ"].Cast<Slider>().CurrentValue;
            var jungleclearQ = Menu._jungleclearMenu["QJ"].Cast<CheckBox>().CurrentValue;
            var jungleclearW = Menu._jungleclearMenu["WJ"].Cast<CheckBox>().CurrentValue;
            var jungleclearE = Menu._jungleclearMenu["EJ"].Cast<CheckBox>().CurrentValue;
            var jungleclearR = Menu._jungleclearMenu["RJ"].Cast<CheckBox>().CurrentValue;
            var minion = MinionManager.GetMinions(ryze.Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (ryze.Hero.ManaPercent > Mana)
            {
                if (minion != null)
                {
                    if (jungleclearQ && ryze.Spells.Q.IsReady())
                    {
                        var Qpred = ryze.Spells.Q.GetPrediction(minion);
                        ryze.Spells.Q.Cast(Qpred.UnitPosition);
                    }
                    if (jungleclearE && ryze.Spells.E.IsReady())
                    {
                        ryze.Spells.E.Cast(minion);
                    }
                    if (jungleclearW && ryze.Spells.W.IsReady())
                    {
                        ryze.Spells.W.Cast(minion);
                    }
                    if (jungleclearR && ryze.Spells.R.IsReady() && (ryze.GetPassiveBuff >= 4 || ryze.Hero.HasBuff("ryzepassivecharged")))
                    {
                        ryze.Spells.R.Cast();
                    }
                }
            }
        }
        private void mixed(RyzeMain ryze)
        {
            var Q = Menu._harrashMenu["QH"].Cast<CheckBox>().CurrentValue;
            var Mana = Menu._harrashMenu["ManaH"].Cast<Slider>().CurrentValue;
            if (ryze.Hero.ManaPercent > Mana)
                if (Q)
                    ryze.Spells.qCastPred();
        }

        private List<String> functions = new List<String>();
        public List<String> Functions
        {
            get
            {
                return functions;
            }
        }
        private int i;
        public int I
        {
            get
            {
                return i;
            }
        }
        private bool rev;
        public bool Rev
        {
            get
            {
                return rev;

            }
            set
            {
                rev = value;
            }
        }
        private bool qcast;
        public bool Qcast
        {
            get
            {
                return qcast;
            }
            set
            {
                qcast = value;
            }
        }
        public void Combo(RyzeMain ryze)
        {
            var Heal = Menu._miscMenu["%R"].Cast<Slider>().CurrentValue;
            if (functions != null)
            {
                if (i < functions.Count)
                {

                    sendSpell(functions[i], ryze);
                    if (rev)
                    {

                        i++;
                        rev = false;
                    }
                }
                else
                {

                    i = 0;
                    functions = null;
                    rev = false;
                }

            }
            else
            {
                if (ryze.Hero.HealthPercent <= Heal)
                {
                    ryze.Spells.R.Cast();
                }
            }

            var target = TargetSelector.GetTarget(600, DamageType.Magical);

            if (target != null)
            {
                if (functions == null)
                {
                    if (ryze.Spells.Q.IsReady() && ryze.Spells.W.IsReady() && ryze.Spells.E.IsReady() && ryze.Spells.R.IsReady() && ryze.GetPassiveBuff > 0)
                    {
                        switch (ryze.GetPassiveBuff)
                        {
                            case 1:
                                functions = new List<String> { "R", "E", "Q", "W", "Q", "E", "Q", "W", "Q", "E", "Q" };
                                break;
                            case 2:
                                functions = new List<String> { "R", "Q", "W", "Q", "E", "Q", "W", "Q", "E", "Q" };
                                break;
                            case 3:
                                functions = new List<String> { "R", "W", "Q", "E", "Q", "W", "Q", "E", "Q", "W", "Q" };
                                break;
                            case 4:
                                functions = new List<String> { "R", "W", "Q", "E", "Q", "W", "Q", "E" };
                                break;
                        }
                    }

                    else if ((ryze.Spells.Q.IsReady()) && (ryze.Spells.W.IsReady()) && (ryze.Spells.E.IsReady()) && !(ryze.Spells.R.IsReady()) && ryze.GetPassiveBuff > 1)
                    {
                        switch (ryze.GetPassiveBuff)
                        {
                            case 2:
                                functions = new List<String> { "Q", "E", "W", "Q", "E", "Q", "W", "Q", "E" };
                                break;
                            case 3:
                                functions = new List<String> { "Q", "W", "Q", "E", "Q", "W", "Q", "E" };
                                break;
                            case 4:
                                functions = new List<String> { "W", "Q", "E", "Q", "W", "Q", "E", "Q", "W", "Q", "E", "Q" };
                                break;
                        }
                    }
                    else
                    {
                        if (ryze.Hero.HasBuff("ryzepassivecharged"))
                        {
                            if (qcast)
                            {
                                if (ryze.Spells.Q.IsReady())
                                    ryze.Spells.qCast();
                                else if (ryze.Spells.R.IsReady())
                                {
                                    ryze.Spells.rCast();
                                }
                            }
                            else
                            {
                                if (ryze.Spells.W.IsReady())
                                {

                                    ryze.Spells.wCast();
                                }

                                else if (ryze.Spells.E.IsReady())
                                {
                                    ryze.Spells.eCast();
                                }
                                else if (ryze.Spells.R.IsReady())
                                {

                                    ryze.Spells.rCast();
                                }
                            }
                        }
                        else
                        {

                            if (ryze.Spells.Q.IsReady())
                            {
                                ryze.Spells.qCast();
                            }
                            else if (ryze.Spells.W.IsReady())
                            {
                                ryze.Spells.wCast();
                            }
                            else if (ryze.Spells.E.IsReady())
                            {
                                ryze.Spells.eCast();
                            }
                        }
                    }

                }
            }
        }
        public bool sendSpell(string s, RyzeMain ryze)
        {
            switch (s)
            {
                case "Q":
                    return ryze.Spells.qCast();
                case "W":
                    return ryze.Spells.wCast();
                case "E":
                    return ryze.Spells.eCast();
                case "R":
                    return ryze.Spells.rCast();
            }
            return false;
        }
    }
}
