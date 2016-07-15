using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankCombo
    {
        public static int LastCondition;
        public static int Estack { get { return BadaoMainVariables.E.Instance.Ammo; } }
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!PortAIO.OrbwalkerManager.isComboActive)
                return;
            if (Environment.TickCount - LastCondition >= 100 + Game.Ping)
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget()))
                {
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (BadaoMainVariables.Q.IsReady() && BadaoMainVariables.E.IsReady())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.QableBarrels(350))
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 660 /*+ hero.BoundingRadius*/)
                                && !nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                var mbarrels = nbarrels.FirstOrDefault(x => x.Bottle.LSDistance(pred) <= 660);
                                var pos = mbarrels.Bottle.Position.LSExtend(pred, 660);
                                if (Player.LSDistance(pos) < BadaoMainVariables.E.Range)
                                {
                                    PortAIO.OrbwalkerManager.SetAttack(false);
                                    PortAIO.OrbwalkerManager.SetMovement(false);
                                    LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () =>
                                    {
                                        PortAIO.OrbwalkerManager.SetAttack(true);
                                        PortAIO.OrbwalkerManager.SetMovement(true);
                                    });
                                    BadaoMainVariables.E.Cast(pos);
                                    LastCondition = Environment.TickCount;
                                    return;
                                }
                            }
                        }
                    }
                }
                foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget()))
                {
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (Orbwalker.CanAutoAttack && BadaoMainVariables.E.IsReady())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.AttackableBarrels(350))
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 660 /*+ hero.BoundingRadius*/)
                                && !nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                PortAIO.OrbwalkerManager.SetAttack(false);
                                PortAIO.OrbwalkerManager.SetMovement(false);
                                LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () =>
                                {
                                    PortAIO.OrbwalkerManager.SetAttack(true);
                                    PortAIO.OrbwalkerManager.SetMovement(true);
                                });
                                var mbarrels = nbarrels.FirstOrDefault(x => x.Bottle.LSDistance(pred) <= 660);
                                var pos = mbarrels.Bottle.Position.LSExtend(pred, 660);
                                BadaoMainVariables.E.Cast(pos);
                                LastCondition = Environment.TickCount;
                                return;
                            }
                        }
                    }
                }
                foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget() && x.LSDistance(ObjectManager.Player) < 2000))
                {
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (BadaoMainVariables.Q.IsReady())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.QableBarrels())
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                PortAIO.OrbwalkerManager.SetMovement(false);
                                PortAIO.OrbwalkerManager.SetAttack(false);
                                LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () =>
                                {
                                    PortAIO.OrbwalkerManager.SetMovement(true);
                                    PortAIO.OrbwalkerManager.SetAttack(true);
                                });
                                BadaoMainVariables.Q.Cast(barrel.Bottle);
                                if (BadaoMainVariables.Q.Cast(barrel.Bottle) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                                {
                                    BadaoGangplankCombo.LastCondition = Environment.TickCount;
                                    return;
                                }
                            }
                        }
                    }
                }

                foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget() && x.LSDistance(ObjectManager.Player) < 2000 && x.IsHPBarRendered))
                {
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    foreach (var barrel in BadaoGangplankBarrels.AttackableBarrels())
                    {
                        var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                        if (nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 330 /*+ hero.BoundingRadius*/))
                        {
                            Console.WriteLine("1");
                            PortAIO.OrbwalkerManager.SetMovement(false);
                            PortAIO.OrbwalkerManager.SetAttack(false);
                            Console.WriteLine("2");
                            LeagueSharp.Common.Utility.DelayAction.Add(300 + Game.Ping, () =>
                            {
                                PortAIO.OrbwalkerManager.SetMovement(true);
                                PortAIO.OrbwalkerManager.SetAttack(true);
                            });
                            Console.WriteLine("3");
                            PortAIO.OrbwalkerManager.ForcedTarget(barrel.Bottle;
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle);
                            Console.WriteLine("4");
                            if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle))
                            {
                                BadaoGangplankCombo.LastCondition = Environment.TickCount;
                                return;
                            }
                        }
                    }
                }
            }
            if (Estack >= 2 && BadaoMainVariables.E.IsReady() && BadaoGangplankVariables.ComboE1)
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.E.Range, DamageType.Physical);
                if (target.BadaoIsValidTarget())
                {
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(target, 0.5f).UnitPosition;
                    if (!BadaoGangplankBarrels.Barrels.Any(x => x.Bottle.LSDistance(pred) <= 660 /*+ target.BoundingRadius*/))
                        BadaoMainVariables.E.Cast(pred);
                }
            }
            if (BadaoMainVariables.Q.IsReady())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, DamageType.Physical);
                if (target.BadaoIsValidTarget())
                {
                    bool useQ = true;
                    foreach (var barrel in BadaoGangplankBarrels.DelayedBarrels(1000))
                    {
                        var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                        if (BadaoMainVariables.E.IsReady()
                            && nbarrels.Any(x => x.Bottle.LSDistance(target.Position) <= 660 + target.BoundingRadius)
                            && !nbarrels.Any(x => x.Bottle.LSDistance(target.Position) <= 330 + target.BoundingRadius))
                        {
                            useQ = false;
                        }
                        else if (nbarrels.Any(x => x.Bottle.LSDistance(target.Position) <= 330 + target.BoundingRadius))
                        {
                            useQ = false;
                        }
                    }
                    if (useQ)
                    {
                        BadaoMainVariables.Q.Cast(target);
                    }
                }
            }
        }
    }
}
