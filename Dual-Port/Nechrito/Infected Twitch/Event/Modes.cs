#region

using System;
using System.Linq;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using EloBuddy;
using LeagueSharp.SDK;
using static Infected_Twitch.Core.Spells;
using EloBuddy.SDK;
#endregion

 namespace Infected_Twitch.Event
{
    internal class Modes : Core.Core
    {
        
        public static void Update(EventArgs args)
        {
            AutoE();


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Lane();
                Jungle();
            }

        }

        public static readonly string[] Monsters =
        {
            "SRU_Red", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", "SRU_Murkwolf"
        };

        public static readonly string[] Dragons =
        {
            "SRU_Dragon_Air", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Earth", "SRU_Dragon_Elder", "SRU_Baron",
            "SRU_RiftHerald"
        };

        private static void AutoE()
        {
            if (!E.IsReady()) return;

            if (MenuConfig.StealEpic)
            {
                foreach (var m in ObjectManager.Get<Obj_AI_Base>().Where(x => Dragons.Contains(x.CharData.BaseSkinName) && !x.IsDead))
                {
                    if (m.Health < E.GetDamage(m))
                    {
                        E.Cast();
                    }
                }
            }

            if (!MenuConfig.StealRed) return;

            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget(E.Range)).ToList();

            foreach (var m in mob)
            {
                if (!m.CharData.BaseSkinName.Contains("SRU_Red")) continue;

                if (m.Health < E.GetDamage(m))
                {
                    E.Cast();
                }
            }

            if (!SafeTarget(Target)) return;
            if (!MenuConfig.KillstealE || MenuConfig.UseExploit) return;
            if (!(EDamage(Target) >= Target.Health + Target.AttackShield)) return;

            E.Cast();

            if (MenuConfig.Debug)
            {
                Chat.Print("Killteal E Active");
            }
        }

        /* Doc7 - Twitch for calculations */

        public static int[] stack = { 0, 15, 20, 25, 30, 35 };
        public static int[] _base = { 0, 20, 35, 50, 65, 80 };

        private static float EDamage(Obj_AI_Base target)
        {
            var stacks = Stack(target);
            return Player.CalculateDamageOnUnit(target, DamageType.Physical, _base[E.Level] + stacks * (0.25f * ObjectManager.Player.FlatPhysicalDamageMod + 0.2f * ObjectManager.Player.FlatMagicDamageMod + stack[E.Level]));
        }

        private static int Stack(Obj_AI_Base obj)
        {
            var Ec = 0;
            for (var t = 1; t < 7; t++)
            {
                if (ObjectManager.Get<Obj_GeneralParticleEmitter>().Any(s => s.Position.Distance(obj.ServerPosition) <= 175 && s.Name == "twitch_poison_counter_0" + t + ".troy"))
                {
                    Ec = t;
                }
            }
            return Ec;
        }

        private static void Combo()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) return;

            if (!SafeTarget(Target)) return;

            if (MenuConfig.ComboE)
            {
                if (!E.IsReady()) return;
                if (Target.Health + Target.AttackShield <= EDamage(Target))
                {
                    E.Cast();

                    if (MenuConfig.Debug)
                    {
                        Chat.Print("Combo => Casting E");
                    }

                }
            }


            if (MenuConfig.UseYoumuu && Target.LSIsValidTarget(Player.AttackRange))
            {
                Usables.CastYomu();
            }

            if (Target.HealthPercent <= 70 && !MenuConfig.UseExploit)
            {
                Usables.Botrk();
            }


            if (!MenuConfig.ComboW) return;
            if (!W.IsReady()) return;
            if (!Target.LSIsValidTarget(W.Range))
                if (Target.Health <= Player.GetAutoAttackDamage(Target) * 2 && Target.Distance(Player) < Player.AttackRange) return;
            if (!(Player.ManaPercent >= 7.5)) return;

            var wPred = W.GetPrediction(Target).CastPosition;

            W.Cast(wPred);
        }

        private static void Harass()
        {
            if (Target == null || Target.IsInvulnerable || !Target.LSIsValidTarget()) return;

            if (Dmg.Stacks(Target) >= MenuConfig.HarassE && Target.Distance(Player) >= Player.AttackRange + 50)
            {
                E.Cast();
            }

            if (!MenuConfig.HarassW) return;

            var wPred = W.GetPrediction(Target).CastPosition;

            W.Cast(wPred);
        }

        private static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.LSIsValidTarget(Player.AttackRange)).ToList();
            if (!MenuConfig.LaneW) return;
            if (!W.IsReady()) return;

            var wPred = W.GetCircularFarmLocation(minions);

            if (wPred.MinionsHit >= 4)
            {
                W.Cast(wPred.Position);
            }
        }

        private static void Jungle()
        {
            if (Player.Level == 1) return;
            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && !GameObjects.JungleSmall.Contains(m) && m.LSIsValidTarget(E.Range)).ToList();

            if (MenuConfig.JungleW && Player.ManaPercent >= 20)
            {
                if (mob.Count == 0) return;

                var wPrediction = W.GetCircularFarmLocation(mob);
                if (wPrediction.MinionsHit >= 3)
                {
                    W.Cast(wPrediction.Position);
                }
            }

            if (!MenuConfig.JungleE) return;
            if (!E.IsReady()) return;

            foreach (var m in ObjectManager.Get<Obj_AI_Base>().Where(x => Monsters.Contains(x.CharData.BaseSkinName) && !x.IsDead))
            {
                if (m.Health < Dmg.EDamage(m))
                {
                    E.Cast();
                }
            }
        }
    }
}