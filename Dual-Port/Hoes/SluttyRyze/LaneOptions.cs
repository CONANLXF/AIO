using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace Slutty_ryze
{
    internal class LaneOptions
    {
        #region Public Functions

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static bool QSpell
        {
            get { return getCheckBoxItem(MenuManager.combo1Menu, "useQ"); }
        }

        private static bool ESpell
        {
            get { return getCheckBoxItem(MenuManager.combo1Menu, "useE"); }
        }

        private static bool WSpell
        {
            get { return getCheckBoxItem(MenuManager.combo1Menu, "useW"); }
        }

        private static bool RSpell
        {
            get { return getCheckBoxItem(MenuManager.combo1Menu, "useR"); }
        }

        private static bool RwwSpell
        {
            get { return getCheckBoxItem(MenuManager.combo1Menu, "useRww"); }
        }

        private static readonly Random Seeder = new Random();

        //struct MinionHealthPerSecond
        //{
        //    public float LastHp;
        //    public float DamagePerSecond;
        //}

        //private MinionHealthPerSecond[] calcMinionHealth(Obj_AI_Base[] minionsBase)
        //{
        //    MinionHealthPerSecond[] minionsStruct = new MinionHealthPerSecond[minionsBase.Length];
        //    const int checkDelay = 2;
        //    for (int i = 0; checkDelay > i; i++)
        //    {
        //        var startTime = Utils.TickCount;
        //        var endTime = startTime + 1;
        //        if (Utils.TickCount < endTime)

        //        for (int index = 0; index < minionsBase.Length; index++)
        //        {
        //            if (minionsBase[index].IsDead)
        //                    continue;

        //             var cMinionHP = minionsBase[index].Health;

        //             if (Math.Abs(minionsStruct[index].LastHp) > 1)
        //                minionsStruct[index].DamagePerSecond = (minionsStruct[index].LastHp - minionsBase[index].Health/checkDelay);

        //            minionsStruct[index].LastHp = minionsBase[index].Health;
        //        }
        //    }

        //    return minionsStruct;
        //}

        public static void LaneClear()
        {
            if (!getKeyBindItem(MenuManager.laneMenu, "disablelane"))
                return;

            var qlchSpell = getCheckBoxItem(MenuManager.laneMenu, "useQlc");
            var elchSpell = getCheckBoxItem(MenuManager.laneMenu, "useElc");
            var wlchSpell = getCheckBoxItem(MenuManager.laneMenu, "useWlc");

            var qlcSpell = getCheckBoxItem(MenuManager.laneMenu, "useQ2L");
            var elcSpell = getCheckBoxItem(MenuManager.laneMenu, "useE2L");
            var wlcSpell = getCheckBoxItem(MenuManager.laneMenu, "useW2L");

            var minMana = getSliderItem(MenuManager.laneMenu, "useEPL");

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            if (GlobalManager.GetHero.ManaPercent <= minMana)
                return;

            foreach (var minion in minionCount)
            {
                
                if (qlcSpell && Champion.Q.IsReady())
                {
                    Champion.Q.Cast(minion);
                }

                if (elcSpell && Champion.E.IsReady() && minion.IsValidTarget(Champion.E.Range))
                {
                    Champion.E.Cast(minion);
                }

                if (wlcSpell && Champion.W.IsReady() && minion.IsValidTarget(Champion.W.Range))
                {
                    Champion.W.Cast(minion);
                }           
                var minionHp = minion.Health; // Reduce Calls and add in randomization buffer.

                if (minion.IsDead) return;

                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.LSIsValidTarget(Champion.Q.Range)
                    && minionHp <= Champion.Q.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                    Champion.Q.Cast(minion);

                else if (wlchSpell
                         && Champion.W.IsReady()
                         && minion.LSIsValidTarget(Champion.W.Range)
                         && minionHp <= Champion.W.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                    Champion.W.CastOnUnit(minion);

                else if (elchSpell
                         && Champion.E.IsReady()
                         && minion.LSIsValidTarget(Champion.E.Range)
                         && minionHp <= Champion.E.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                    Champion.E.Cast(minion);


            }
        }

        //get assembly version
        public static void JungleClear()
        {

            var qlchSpell = getCheckBoxItem(MenuManager.jungleMenu, "useQj");
            var elchSpell = getCheckBoxItem(MenuManager.jungleMenu, "useEj");
            var wlchSpell = getCheckBoxItem(MenuManager.jungleMenu, "useWj");
            //Convert to use new system later
            var mSlider = getSliderItem(MenuManager.jungleMenu, "useJM");

            if (GlobalManager.GetHero.ManaPercent < mSlider)
                return;


            var jungle = MinionManager.GetMinions(Champion.Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            foreach (var jung in jungle)
            {
                if (qlchSpell && Champion.Q.IsReady())
                {
                    Champion.Q.Cast(jung);
                }

                if (elchSpell && Champion.E.IsReady())
                {
                    Champion.E.Cast(jung);
                }

                if (wlchSpell && Champion.W.IsReady())
                {
                    Champion.W.Cast(jung);
                }
            }

        }


        public static void LastHit()
        {
            #region Old

            var qlchSpell = getCheckBoxItem(MenuManager.lastMenu, "useQl2h");
            var elchSpell = getCheckBoxItem(MenuManager.lastMenu, "useEl2h");
            var wlchSpell = getCheckBoxItem(MenuManager.lastMenu, "useWl2h");

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            foreach (var minion in minionCount)
            {
                var minionHp = minion.Health; // Reduce Calls and add in randomization buffer.

                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.LSIsValidTarget(Champion.Q.Range - 20)
                    && minionHp < Champion.Q.GetDamage(minion))
                    Champion.Q.Cast(minion);

                if (wlchSpell
                    && Champion.W.IsReady()
                    && minion.LSIsValidTarget(Champion.W.Range - 10)
                    && minionHp < Champion.W.GetDamage(minion))
                    Champion.W.Cast(minion);

                if (elchSpell
                    && Champion.E.IsReady()
                    && minion.LSIsValidTarget(Champion.E.Range - 10)
                    && minionHp < Champion.E.GetDamage(minion))
                    Champion.E.Cast(minion);
            }

            #endregion
        }

        public static void Mixed()
        {
            #region Old
            var qSpell = getCheckBoxItem(MenuManager.mixedMenu, "UseQM");
            var qlSpell = getCheckBoxItem(MenuManager.mixedMenu, "UseQMl");
            var eSpell = getCheckBoxItem(MenuManager.mixedMenu, "UseEM");
            var wSpell = getCheckBoxItem(MenuManager.mixedMenu, "UseWM");
            var minMana = getSliderItem(MenuManager.laneMenu, "useEPL");

            if (GlobalManager.GetHero.ManaPercent < getSliderItem(MenuManager.mixedMenu, "mMin"))
                return;


            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (qSpell
                && Champion.Q.IsReady()
                && target.LSIsValidTarget(Champion.Q.Range))
                Champion.Q.Cast(target);

            if (wSpell
                && Champion.W.IsReady()
                && target.LSIsValidTarget(Champion.W.Range))
                Champion.W.Cast(target);

            if (eSpell
                && Champion.E.IsReady()
                && target.LSIsValidTarget(Champion.E.Range))
                Champion.E.Cast(target);

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            {
                if (GlobalManager.GetHero.ManaPercent <= minMana)
                    return;

                foreach (
                    var minion in
                        minionCount.Where(
                            minion =>
                                qlSpell && Champion.Q.IsReady() && minion.Health < Champion.Q.GetDamage(minion) &&
                                GlobalManager.CheckTarget(minion)))
                {
                    Champion.Q.Cast(minion);
                }
            }

            #endregion


        }

        public static void CastQ(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !QSpell) return;
            if (target.LSIsValidTarget(Champion.Q.Range)
                && Champion.Q.IsReady())
                Champion.Q.Cast(target);
        }

        public static void CastQn(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !QSpell) return;
            if (target.LSIsValidTarget(Champion.Qn.Range)
                && Champion.Qn.IsReady())
                Champion.Qn.Cast(target);
        }

        public static void CastW(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !WSpell) return;
            if (target.LSIsValidTarget(Champion.W.Range)
                && Champion.W.IsReady())
                Champion.W.Cast(target);
        }

        public static void CastE(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !ESpell) return;
            if (target.LSIsValidTarget(Champion.E.Range)
                && Champion.E.IsReady())
                Champion.E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !RSpell) return;
            if (!Champion.R.IsReady())
                return;
            if (target.LSIsValidTarget(Champion.W.Range)
                && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
            {
                if (target.HasBuff("RyzeW"))
                    Champion.R.Cast();
            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Champion.Q.Range, DamageType.Magical);

            if (!target.LSIsValidTarget())
                return;

            if (Champion.E.IsReady() && Champion.W.IsReady() && !Champion.Q.IsReady())
            {
                Champion.E.Cast(target);
            }
            if (Champion.Q.IsReady())
            {
                Champion.Q.Cast(target);
            }

            if (Champion.E.IsReady() && target.LSIsValidTarget(Champion.E.Range) &&
                (!Champion.Q.IsReady() || Champion.Q.GetPrediction(target).CollisionObjects.Count != 0))
            {
                Champion.E.Cast(target);
            }

            if (Champion.W.IsReady() && target.LSIsValidTarget(Champion.W.Range) &&
                (!Champion.Q.IsReady() || Champion.Q.GetPrediction(target).CollisionObjects.Count != 0))
            {
                if (Champion.E.IsReady() && Champion.W.IsReady() && !Champion.Q.IsReady())
                    return;
                Champion.W.Cast(target);
            }


            //if ((!target.HasBuff("RyzeE") && Champion.E.IsReady()) ||
            //    (Champion.E.IsReady() && target.IsValidTarget(Champion.E.Range)) ||
            //    (!Champion.Q.IsReady() && !Champion.W.IsReady()))
            //{
            //    if (target.IsValidTarget(Champion.E.Range) && Champion.E.IsReady())
            //    {
            //        Champion.E.Cast(target);
            //    }
            //}
            //else
            //{
            //    if (target.IsValidTarget(Champion.W.Range) && Champion.W.IsReady())
            //    {
            //            Champion.W.Cast(target);
            //    }
            //    else if (target.IsValidTarget(Champion.Q.Range) && Champion.Q.IsReady() &&
            //             (!Champion.W.IsReady() || !target.IsValidTarget(Champion.W.Range))) 
            //    {
            //            Champion.Q.Cast(target);

            //    }
            //}
        }

    }
}



#endregion