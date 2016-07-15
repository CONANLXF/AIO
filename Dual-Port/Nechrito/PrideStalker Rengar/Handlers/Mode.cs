#region

using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using PrideStalker_Rengar.Main;
using LeagueSharp.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

#endregion

using TargetSelector = PortAIO.TSManager; namespace PrideStalker_Rengar.Handlers
{
    internal class Mode : Core
    {
        private static int _lastTick;
        public static bool HasPassive => Player.Buffs.Any(x => x.Name.ToLower().Contains("rengarpassivebuff"));

        #region Combo
        public static void Combo()
        {
            var target = TargetSelector.GetTarget(1000, DamageType.Physical);


            if (target == null || !target.LSIsValidTarget() || target.IsZombie) return;
            if (Player.Mana == 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && HasPassive)
                {
                    ITEM.CastYomu();
                }
                if (Spells.E.IsReady() && target.Distance(Player) < Player.AttackRange)
                {

                    Spells.E.Cast(target);
                }
                if (Spells.Q.IsReady() && target.Distance(Player) < Player.AttackRange && !Spells.E.IsReady())
                {
                    Spells.Q.Cast(target);
                }
            }
            if (Player.Mana < 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && HasPassive)
                {
                    ITEM.CastYomu();
                }
                if (Spells.E.IsReady() && !HasPassive && target.Distance(Player) < Spells.E.Range)
                {
                    Spells.E.Cast(target);
                }
                if (target.Distance(Player) <= Spells.W.Range)
                {
                    if (Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(target);
                    }
                    if (MenuConfig.UseItem)
                    {
                        ITEM.CastHydra();
                    }
                    if (Spells.W.IsReady())
                    {
                        Spells.W.Cast(target);
                    }
                }
            }
        }
        #endregion
        #region ApCombo
        public static void ApCombo()
        {
            var target = TargetSelector.GetTarget(Spells.E.Range, DamageType.Magical);

            if (target == null || !target.LSIsValidTarget() || target.IsZombie) return;

            if (Player.Mana == 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && HasPassive)
                {
                    ITEM.CastYomu();
                }
                if (target.Distance(Player) <= Spells.W.Range)
                {
                    if (MenuConfig.UseItem && Spells.W.IsReady())
                    {
                        ITEM.CastHydra();
                    }
                    if (Spells.W.IsReady())
                    {
                        Spells.W.Cast(target);
                    }
                }
            }
            if (!(Player.Mana < 5)) return;
            if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && HasPassive)
            {
                ITEM.CastYomu();
            }
            if (MenuConfig.UseItem && !HasPassive)
            {
                ITEM.CastProtobelt();
            }
            if (!(target.Distance(Player) <= Spells.W.Range)) return;
            if (MenuConfig.UseItem && Spells.W.IsReady())
            {
                ITEM.CastHydra();
            }
            if (Spells.W.IsReady())
            {
                Spells.W.Cast(target);
                Spells.W.Cast(target);
            }
            else if (Spells.Q.IsReady())
            {
                Spells.Q.Cast(target);
            }
            else if (Spells.E.IsReady() && target.Distance(Player) <= Spells.W.Range + 225)
            {
                if (MenuConfig.IgnoreE)
                {
                    Spells.E.Cast(target.ServerPosition);
                }
                else
                {
                    Spells.E.CastIfHitchanceEquals(target, HitChance.Collision);
                }
            }
        }
        #endregion

        #region TripleQ
        public static void TripleQ()
        {
            var target = TargetSelector.GetTarget(Spells.E.Range, DamageType.Physical);

            if (target == null || !target.LSIsValidTarget() || target.IsZombie) return;

            if (Player.Mana == 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && HasPassive)
                {
                    ITEM.CastYomu();
                }

                if (Spells.Q.IsReady() && target.Distance(Player) <= Spells.W.Range)
                {
                    if (!MenuConfig.TripleQAAReset)
                    {
                        Spells.Q.Cast();
                    }
                }
            }

            if (Player.Mana < 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady())
                {
                    ITEM.CastYomu();
                }
                if (Spells.Q.IsReady() && target.Distance(Player) <= Spells.W.Range)
                {
                    if (!MenuConfig.TripleQAAReset)
                    {
                        Spells.Q.Cast();
                    }
                }
                if (Spells.E.IsReady() && !Spells.Q.IsReady() && target.Distance(Player) < Player.AttackRange)
                {
                    if (MenuConfig.IgnoreE)
                    {
                        Spells.E.Cast(target.ServerPosition);
                    }
                    else
                    {
                        Spells.E.CastIfHitchanceEquals(target, HitChance.Collision);
                    }
                }
                if (Spells.W.IsReady() && !Spells.Q.IsReady() && Player.Distance(target) <= Spells.W.Range)
                {
                    if (MenuConfig.UseItem)
                    {
                        ITEM.CastHydra();
                    }
                    Spells.W.Cast(target);
                }
            }
        }
        #endregion

        #region OneShot
        public static void OneShot()
        {
            var target = TargetSelector.GetTarget(Spells.E.Range, DamageType.Physical);
            GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.LSIsValidTarget(Spells.W.Range)).ToList();

            if (target == null || !target.LSIsValidTarget() || target.IsZombie) return;
            if (Player.Mana == 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && HasPassive)
                {
                    ITEM.CastYomu();
                }
                if (Spells.Q.IsReady() && target.Distance(Player) <= Spells.E.Range)
                {
                    Spells.Q.Cast();
                }
            }
            if (Player.Mana < 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady())
                {
                    ITEM.CastYomu();
                }
                if (Spells.E.IsReady() && target.Distance(Player) <= Spells.W.Range + 225)
                {
                    if (MenuConfig.IgnoreE)
                    {
                        Spells.E.Cast(target.ServerPosition);
                    }
                    else
                    {
                        Spells.E.CastIfHitchanceEquals(target, HitChance.Collision);
                    }
                }
                if (Spells.Q.IsReady() && target.Distance(Player) <= Spells.W.Range)
                {
                    Spells.Q.Cast();
                }
                if (Spells.W.IsReady() && Player.Distance(target) <= Spells.W.Range)
                {
                    if (MenuConfig.UseItem)
                    {
                        ITEM.CastHydra();
                    }
                    Spells.W.Cast(target);
                }
            }
        }

        #endregion


        #region Lane
        public static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.LSIsValidTarget(Spells.W.Range)).ToList();

            if (Player.Mana == 5 && MenuConfig.Passive)
            {
                return;
            }

            foreach (var m in minions)
            {
                if (Player.Mana == 5)
                {
                    if (Mode.getBoxItem(MenuConfig.comboMenu, "ComboMode") == 2)
                    {
                        if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                        {
                            Spells.W.Cast(m.ServerPosition);
                        }
                    }
                    else
                    {
                        if (Spells.Q.IsReady() && m.Distance(Player) < Player.AttackRange)
                        {
                            if (MenuConfig.UseItem)
                            {
                                ITEM.CastHydra();
                            }
                            Spells.Q.Cast(m);
                        }
                    }
                }
                if (Player.Mana < 5)
                {
                    if (Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(m);
                    }

                    if (Spells.E.IsReady() && !HasPassive)
                    {
                        if (Spells.Q.IsReady() || Spells.W.IsReady())
                        {
                            Spells.E.Cast(m);
                        }
                    }

                    if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEM.CastHydra();
                        }
                        Spells.W.Cast(m);
                    }
                }
            }

        }
        #endregion

        #region Jungle
        public static void Jungle()
        {
            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget(Spells.W.Range)).ToList();

            if (Player.Mana == 5 && MenuConfig.Passive)
            {
                return;
            }

            foreach (var m in mob)
            {
                if (Player.Mana == 5)
                {
                    if (Mode.getBoxItem(MenuConfig.comboMenu, "ComboMode") == 2)
                    {
                        if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                        {
                            Spells.W.Cast(m.ServerPosition);
                        }
                    }
                    else
                    {
                        if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range && Player.HealthPercent < 80)
                        {
                            if (MenuConfig.UseItem)
                            {
                                ITEM.CastHydra();
                            }
                            Spells.W.Cast(m.ServerPosition);
                        }
                    }
                }
                if (Player.Mana < 5)
                {

                    if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEM.CastHydra();
                        }
                        Spells.W.Cast(m.ServerPosition);
                    }
                    if (Spells.E.IsReady())
                    {
                        if (!Spells.Q.IsReady() || Spells.W.IsReady()) return;

                        Spells.E.Cast(m.ServerPosition);
                    }
                }
            }
        }
        #endregion


        #region LastHit
        public static void LastHit()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.LSIsValidTarget(Player.AttackRange)).ToList();


            if (Player.Mana == 5 && MenuConfig.Passive)
            {
                return;
            }
            if (!MenuConfig.StackLastHit) return;
            {
                foreach (var m in minions)
                {
                    if (m.Health < Spells.Q.GetDamage(m) + (float)Player.GetAutoAttackDamage(m))
                    {
                        Spells.Q.Cast();
                    }
                }
            }
        }
        #endregion

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        #region ComboMode
        public static void ChangeComboMode()
        {
            var changetime = Environment.TickCount - _lastTick;


            if (MenuConfig.ChangeComboMode)
            {
                if (getBoxItem(MenuConfig.comboMenu, "ComboMode") == 0 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 1;
                }

                if (getBoxItem(MenuConfig.comboMenu, "ComboMode") == 1 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 2;
                }
                if (getBoxItem(MenuConfig.comboMenu, "ComboMode") == 2 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 3;
                }
                if (getBoxItem(MenuConfig.comboMenu, "ComboMode") == 3 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 0;
                }
            }

        }

        #endregion
    }
}
