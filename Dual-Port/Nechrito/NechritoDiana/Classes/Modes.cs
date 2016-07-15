using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SPrediction;
using System;
using System.Linq;

using TargetSelector = PortAIO.TSManager; namespace Nechrito_Diana
{
    class Modes
    {
        public static void ComboLogic()
        {
            var Target = TargetSelector.GetTarget(Spells.R.Range, DamageType.Magical);

            if (Target.LSIsValidTarget() && !Target.IsDead && !Target.IsInvulnerable && Target != null)
            {
                var arcPred = Spells.Q.GetArcSPrediction(Target).CastPosition;

                if (Target.Health < Dmg.ComboDmg(Target) || MenuConfig.Misaya)
                {
                    if (Spells.Q.IsReady() && Spells.R.IsReady())
                    {
                        if (Spells.R.IsInRange(Target))
                        {
                            Spells.R.Cast(Target);
                        }

                        if (Spells.Q.GetPrediction(Target).Hitchance >= HitChance.Medium)
                        {
                            Spells.Q.Cast(arcPred);
                        }
                    }
                }

                if (Spells.Q.IsReady() && Spells.Q.GetPrediction(Target).Hitchance >= HitChance.Medium)
                {
                    if (true)
                    {
                        Spells.Q.Cast(arcPred);
                    }
                }

                if (Spells.R.IsReady() && (ObjectManager.Player.LSDistance(Target.Position) <= Spells.R.Range))
                {
                    if (Target.HasBuff("dianamoonlight") && MenuConfig.ComboR)
                    {
                        Spells.R.Cast(Target);
                    }
                    if (!MenuConfig.ComboR)
                    {
                        Spells.R.Cast(Target);
                    }
                }


                if (Spells.W.IsReady() && (ObjectManager.Player.LSDistance(Target.Position) <= ObjectManager.Player.AttackRange + 30))
                {
                    Spells.W.Cast(Target);
                }

                if (MenuConfig.ComboE && ObjectManager.Player.ManaPercent >= 15)
                {
                    if (Spells.E.IsReady() && (ObjectManager.Player.LSDistance(Target.Position) <= Spells.E.Range - 30))
                    {
                        Spells.E.Cast(Target);
                    }
                }
            }
        }
        public static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(800, DamageType.Magical);
            var arcPred = Spells.Q.GetArcSPrediction(target).CastPosition;

            if (target != null && target.LSIsValidTarget() && !target.IsZombie)
            {
                if (Spells.Q.IsReady())
                {
                    Spells.Q.Cast(arcPred);
                }
                if (Spells.W.IsReady() && ObjectManager.Player.LSDistance(target.Position) <= ObjectManager.Player.AttackRange)
                {
                    Spells.W.Cast();
                }
            }
        }
        
        public static void JungleLogic()
        {
            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                var mobs = MinionManager.GetMinions(800 + ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mobs.Count == 0) return;
                if (ObjectManager.Player.Spellbook.IsAutoAttacking) return;

                if (Spells.Q.IsReady() && Spells.R.IsReady())
                {
                    var m = MinionManager.GetMinions(800 + ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                    if (m != null && MenuConfig.jnglQR && (ObjectManager.Player.LSDistance(m[0].Position) <= 700f) && (ObjectManager.Player.LSDistance(m[0].Position) >= 400f)
                        && ObjectManager.Player.ManaPercent > 20)

                    {
                        Spells.Q.Cast(m[0]);
                        Spells.R.Cast(m[0]);
                    }
                }

                if (Spells.W.IsReady() && (ObjectManager.Player.LSDistance(mobs[0].Position) <= 300f) && MenuConfig.jnglW)
                {
                    Spells.W.Cast();
                }

                if (Spells.E.IsReady())
                {
                    var minion = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.E.Range);
                    foreach (var m in mobs)
                    {
                        if (m.IsAttackingPlayer && ObjectManager.Player.HealthPercent <= MenuConfig.jnglE)
                        {
                            Spells.E.Cast(m);
                        }
                    }
                }
            }
        }
        public static void LaneLogic()
        {
            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                var minions = MinionManager.GetMinions(800f).FirstOrDefault();
                if (minions == null)
                    return;

                if (Spells.W.IsReady() && MenuConfig.LaneW && (ObjectManager.Player.LSDistance(minions.Position) <= 250f))
                {
                    Spells.W.Cast();
                }

                if (Spells.Q.IsReady() && MenuConfig.LaneQ && ObjectManager.Player.ManaPercent >= 45 && (ObjectManager.Player.LSDistance(minions.Position) <= 550f))
                {
                    var minion = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.W.Range);
                    foreach (var m in minion)
                    {
                        if (m.Health < Spells.Q.GetDamage(m) && minion.Count > 2 && !ObjectManager.Player.IsInAutoAttackRange(m))
                            Spells.Q.Cast(m);
                    }
                }
            }
        }

        public static void Flee()
        {
            if (!MenuConfig.FleeMouse)
            {
                return;
            }

            var jump = Program.JumpPos.Where(x => x.Value.LSDistance(ObjectManager.Player.Position) < 300f && x.Value.LSDistance(Game.CursorPos) < 700f).FirstOrDefault();
            var monster = MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health).FirstOrDefault();
            var mobs = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.NotAlly);

            if (jump.Value.IsValid())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, jump.Value);

                foreach (var junglepos in Program.JunglePos)
                {
                    if (Game.CursorPos.LSDistance(junglepos) <= 350 && ObjectManager.Player.Position.LSDistance(junglepos) <= 850 && Spells.Q.IsReady() && Spells.R.IsReady())
                    {
                        Spells.Q.Cast(junglepos);
                        Spells.R.Cast(monster);
                    }
                }
            }
            else
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            foreach (var junglepos in Program.JunglePos)
            {
                if (Game.CursorPos.LSDistance(junglepos) <= 350 && ObjectManager.Player.Position.LSDistance(junglepos) <= 900 && Spells.Q.IsReady() && Spells.R.IsReady())
                {
                    Spells.Q.Cast(junglepos);
                    Spells.R.Cast(monster);
                }
                else if (Spells.R.IsReady() && !Spells.Q.IsReady() && monster.LSDistance(ObjectManager.Player.Position) > 600f && monster.LSDistance(Game.CursorPos) <= 350f)
                {
                    Spells.R.Cast(monster);
                }
            }

            if (!mobs.Any()) return;

            var mob = mobs.MaxOrDefault(x => x.MaxHealth);

            if (mob.LSDistance(Game.CursorPos) <= 750 && mob.LSDistance(ObjectManager.Player) >= 475)
            {
                if (Spells.Q.IsReady() && Spells.R.IsReady() && ObjectManager.Player.Mana > Spells.R.ManaCost + Spells.Q.ManaCost && mob.Health > Spells.Q.GetDamage(mob))
                {
                    Spells.Q.Cast(mob);
                    Spells.R.Cast(mob);
                }

                if (Spells.R.IsReady())
                {
                    Spells.R.Cast(mob);
                }
            }
        }
    }
}