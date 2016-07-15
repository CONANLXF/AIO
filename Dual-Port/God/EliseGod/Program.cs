using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using TargetSelector = PortAIO.TSManager; namespace EliseGod
{
    internal class Program
    {
        public static Spell Q, W, E, R, Q1, W1, E1;
        private static Menu Menu;
        public static Menu comboMenu, ksMenu, miscMenu, drawMenu, harassMenu, laneClearMenu, jungleClearMenu;
        private static readonly AIHeroClient Player = ObjectManager.Player;
        private static readonly float[] HumanQcd = { 6, 6, 6, 6, 6 };
        private static readonly float[] HumanWcd = { 12, 12, 12, 12, 12 };
        private static readonly float[] HumanEcd = { 14, 13, 12, 11, 10 };
        private static readonly float[] SpiderQcd = { 6, 6, 6, 6, 6 };
        private static readonly float[] SpiderWcd = { 12, 12, 12, 12, 12 };
        private static readonly float[] SpiderEcd = { 26, 23, 20, 17, 14 };
        private static float _qCd, _wCd, _eCd;
        private static float _q1Cd, _w1Cd, _e1Cd;
        private static float realcdQ, realcdW, realcdE, realcdSQ, realcdSW, realcdSE;
        //private static Obj_AI_Minion spider;

        private static bool Human()
        {
            return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ";
        }

        public static void OnGameLoad()
        {
            if (Player.CharData.BaseSkinName != "Elise" && Player.CharData.BaseSkinName != "elisespider") return;

            InitMenu();
            InitializeSpells();

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            LSEvents.OnAttack += OnAttack;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            LSEvents.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            //GameObject.OnCreate += OnCreateObject;
            //GameObject.OnDelete += OnDeleteObject;
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender == null) return;
            if (Human())
            {
                if (getCheckBoxItem(miscMenu, "interrupt"))
                {
                    if (E.IsReady() && sender.LSIsValidTarget(E.Range))
                    {
                        E.Cast(sender);
                    }
                }
            }
            else
            {
                if (getCheckBoxItem(miscMenu, "switchInterrupt"))
                {
                    if (realcdE == 0 && sender.LSIsValidTarget(E.Range) && R.IsReady())
                    {
                        R.Cast();
                        E.Cast(sender);
                    }
                }
            }
        }


        //private static void OnDeleteObject(GameObject sender, EventArgs args)
        //{
        //    if (sender.Type != GameObjectType.obj_AI_Minion) return;
        //    if (sender.Name == ("Spiderling"))
        //    {
        //        spider = null;
        //    }
        //}

        //private static void OnCreateObject(GameObject sender, EventArgs args)
        //{
        //    if (sender.Type != GameObjectType.obj_AI_Minion) return;
        //    if (sender.Name == ("Spiderling"))
        //    {
        //        spider = (Obj_AI_Minion)sender;
        //    }
        //}

        //private static void OnAggro(Obj_AI_Base sender, GameObjectAggroEventArgs args)
        //{
        //    if (sender == null || args == null) return;
        //    {
        //        if (args.NetworkId == ObjectManager.Player.NetworkId)
        //        {
        //            if (!Human() && !Player.IsWindingUp)
        //            {
        //                Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position.Extend(sender.Position, -40));
        //            }
        //            // idk find out when spider is aggrod and run back in range of sender
        //        }
        //    }
        //}

        
        private static void OnUpdate(EventArgs args)
        {
            Killsteal();
            Cooldowns();
            Rappel();

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo();
            }
            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Harass();
            }
            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                JungleClear();
            }
            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                LaneClear();
            }

        }

        private static void JungleClear()
        {
            var jungleMinions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            foreach (var minion in jungleMinions)
            {
                if (Human())
                {
                    if (Q.IsReady())
                        Q.CastOnUnit(minion);

                    if (W.IsReady())
                    {
                        if (W.GetPrediction(minion).CollisionObjects.Count >= 1)
                            W.Cast(minion);

                        else
                            W.Cast(minion);
                    }

                    if (!Q.IsReady() && !W.IsReady() && R.IsReady())
                    {
                        R.Cast();
                    }
                }
                else
                {
                    if (Q1.IsReady())
                        Q1.CastOnUnit(minion);

                    if (realcdSQ > 1 && realcdSW > 1 && !Player.HasBuff("EliseSpiderW") && R.IsReady())
                        if (realcdQ < 1 || realcdW < 1)
                            R.Cast();
                }
            }

        }

        private static void LaneClear()
        {
            if (Human() && Player.ManaPercent <= getSliderItem(laneClearMenu, "laneclear.mana")) return;
            var minions = MinionManager.GetMinions(Player.ServerPosition, W.Range).FirstOrDefault();
            if (minions == null) return;

            if (Human())
            {
                if (Q.IsReady() && getCheckBoxItem(laneClearMenu, "laneclear.q") && minions.LSDistance(Player.Position) <= Q.Range)
                    Q.CastOnUnit(minions);

                if (W.IsReady() && getCheckBoxItem(laneClearMenu, "laneclear.w"))
                    W.Cast(minions);
            }
            else
            {
                if (Q.IsReady() && getCheckBoxItem(laneClearMenu, "laneclear.q.spider") && minions.LSDistance(Player.Position) <= Q1.Range)
                    Q1.CastOnUnit(minions);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null) return;
            if (!target.LSIsValidTarget()) return;

            if (Human())
            {
                if (W.IsReady() && getCheckBoxItem(comboMenu, "wComboH") &&
                    target.LSDistance(Player.Position) <= W.Range)
                {
                    var wprediction = W.GetPrediction(target);

                    switch (wprediction.Hitchance)
                    {
                        case HitChance.Medium:
                        case HitChance.High:
                        case HitChance.VeryHigh:
                        case HitChance.Immobile:

                            W.Cast(wprediction.CastPosition);
                            break;

                        case HitChance.Collision:

                            var colliding = wprediction.CollisionObjects.OrderBy(o => o.LSDistance(Player, true)).ToList();
                            if (colliding.Count > 0)
                            {
                                if (colliding[0].LSDistance(target, true) <= 25000 ||
                                    colliding[0].Type == GameObjectType.AIHeroClient)
                                {
                                    W.Cast(wprediction.CastPosition);
                                }
                                else if (colliding[0].Type != GameObjectType.AIHeroClient &&
                                         colliding[0].LSDistance(target, true) > 25000 && R.IsReady() && realcdSQ <= 1 &&
                                         target.LSDistance(Player.Position) <= Q1.Range + 200)
                                {
                                    var playerPosition = ObjectManager.Player.Position.LSTo2D();
                                    var direction = ObjectManager.Player.Direction.LSTo2D().LSPerpendicular();
                                    const int distance = 600;
                                    const int stepSize = 40;

                                    for (var step = 0f; step < 360; step += stepSize)
                                    {
                                        var currentAngel = step * (float)Math.PI / 180;
                                        var currentCheckPoint = playerPosition +
                                                                distance * direction.LSRotated(currentAngel);

                                        var collision =
                                            Collision.GetCollision(new List<Vector3> { currentCheckPoint.To3D() },
                                                new PredictionInput { Delay = 0.25f, Radius = 200, Speed = 1000 });

                                        if (collision.Count == 0)
                                        {
                                            Q.CastOnUnit(target);
                                            W.Cast(currentCheckPoint);
                                            R.Cast();
                                            //if (Q.IsReady() && Config.Item("qComboH").GetValue<bool>() &&
                                            //    target.LSDistance(Player.Position) <= Q.Range)
                                            //{
                                            //    
                                            //}
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }


                if (E.IsReady() && getCheckBoxItem(comboMenu, "eComboH") &&
                    target.LSDistance(Player.Position) <= E.Range)
                {
                    var eprediction = E.GetPrediction(target);
                    switch (eprediction.Hitchance)
                    {

                        case HitChance.VeryHigh:
                        case HitChance.Immobile:

                            E.Cast(eprediction.CastPosition);
                            break;

                        case HitChance.Collision:
                            var colliding = eprediction.CollisionObjects.OrderBy(o => o.LSDistance(Player, true)).ToList();
                            if (colliding.Count >= 1 && colliding[0].Type == GameObjectType.AIHeroClient)
                                E.Cast(eprediction.CastPosition);
                            break;
                    }
                }

                if (Q.IsReady() && getCheckBoxItem(comboMenu, "qComboH") &&
                    target.LSDistance(Player.Position) <= Q.Range)
                    Q.CastOnUnit(target);

                if (getCheckBoxItem(comboMenu, "rCombo") && !Q.IsReady() && !W.IsReady() && !E.IsReady() &&
                    R.IsReady() && target.LSDistance(Player.Position) <= Q1.Range)
                    if (realcdSQ == 0 || realcdSW == 0 || realcdSE == 0)
                        R.Cast();
            }
            else
            {
                if (Q1.IsReady() && getCheckBoxItem(comboMenu, "qCombo") &&
                    target.LSDistance(Player.Position) <= Q1.Range)
                {
                    Q1.CastOnUnit(target);
                }

                if (E1.IsReady() && getCheckBoxItem(comboMenu, "eCombo") &&
                    target.LSDistance(Player.Position) <= E1.Range &&
                    target.LSDistance(Player.Position) >= getSliderItem(comboMenu, "eMin"))
                    E1.CastOnUnit(target);

                if (getCheckBoxItem(comboMenu, "rCombo") && !Q.IsReady() && !W.IsReady() && !E.IsReady() && R.IsReady())
                    if (!Player.HasBuff("EliseSpiderW") || target.LSDistance(Player.Position) >= Player.GetAutoAttackRange(target) + 100)
                        if (realcdQ <= 1 || realcdW <= 1 || realcdE <= 1)
                            R.Cast();
            }
        }

        private static void Harass()
        {
            if (Human() && Player.ManaPercent <= getSliderItem(harassMenu, "harassMana")) return;
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null) return;
            if (!target.LSIsValidTarget()) return;

            if (Human())
            {
                if (W.IsReady() && getCheckBoxItem(harassMenu, "wHarassH") &&
                    target.LSDistance(Player.Position) <= W.Range)
                {
                    var wprediction = W.GetPrediction(target);

                    switch (wprediction.Hitchance)
                    {
                        case HitChance.Medium:
                        case HitChance.High:
                        case HitChance.VeryHigh:
                        case HitChance.Immobile:

                            W.Cast(wprediction.CastPosition);
                            break;

                        case HitChance.Collision:

                            var colliding = wprediction.CollisionObjects.OrderBy(o => o.LSDistance(Player, true)).ToList();
                            if (colliding.Count > 0)
                            {
                                if (colliding[0].LSDistance(target, true) <= 25000 ||
                                    colliding[0].Type == GameObjectType.AIHeroClient)
                                {
                                    W.Cast(wprediction.CastPosition);
                                }
                                else if (colliding[0].Type != GameObjectType.AIHeroClient &&
                                         colliding[0].LSDistance(target, true) > 25000 && R.IsReady() && realcdSQ <= 1 &&
                                         target.LSDistance(Player.Position) <= Q1.Range + 200 && getCheckBoxItem(harassMenu, "rComboHarass"))
                                {
                                    var playerPosition = ObjectManager.Player.Position.LSTo2D();
                                    var direction = ObjectManager.Player.Direction.LSTo2D().LSPerpendicular();
                                    const int distance = 600;
                                    const int stepSize = 40;

                                    for (var step = 0f; step < 360; step += stepSize)
                                    {
                                        var currentAngel = step * (float)Math.PI / 180;
                                        var currentCheckPoint = playerPosition +
                                                                distance * direction.LSRotated(currentAngel);

                                        var collision =
                                            Collision.GetCollision(new List<Vector3> { currentCheckPoint.To3D() },
                                                new PredictionInput { Delay = 0.25f, Radius = 200, Speed = 1000 });

                                        if (collision.Count == 0)
                                        {
                                            Q.CastOnUnit(target);
                                            W.Cast(currentCheckPoint);
                                            R.Cast();
                                            //if (Q.IsReady() && Config.Item("qHarassH").GetValue<bool>() &&
                                            //    target.LSDistance(Player.Position) <= Q.Range)
                                            //{
                                            //    
                                            //}
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }


                if (E.IsReady() && getCheckBoxItem(harassMenu, "eHarassH") &&
                    target.LSDistance(Player.Position) <= E.Range)
                {
                    var eprediction = E.GetPrediction(target);
                    switch (eprediction.Hitchance)
                    {

                        case HitChance.VeryHigh:
                        case HitChance.Immobile:

                            E.Cast(eprediction.CastPosition);
                            break;

                        case HitChance.Collision:
                            var colliding = eprediction.CollisionObjects.OrderBy(o => o.LSDistance(Player, true)).ToList();
                            if (colliding.Count >= 1 && colliding[0].Type == GameObjectType.AIHeroClient)
                                E.Cast(eprediction.CastPosition);
                            break;
                    }
                }

                if (Q.IsReady() && getCheckBoxItem(harassMenu, "qHarassH") &&
                    target.LSDistance(Player.Position) <= Q.Range)
                    Q.CastOnUnit(target);

                if (getCheckBoxItem(harassMenu, "rHarass") && !Q.IsReady() && !W.IsReady() && !E.IsReady() &&
                    R.IsReady() && target.LSDistance(Player.Position) <= Q1.Range)
                    if (realcdSQ == 0 || realcdSW == 0 || realcdSE == 0)
                        R.Cast();
            }
            else
            {
                if (Q1.IsReady() && getCheckBoxItem(harassMenu, "qHarass") &&
                    target.LSDistance(Player.Position) <= Q1.Range)
                {
                    Q1.CastOnUnit(target);
                }

                if (E1.IsReady() && getCheckBoxItem(harassMenu, "eHarass") &&
                    target.LSDistance(Player.Position) <= E1.Range &&
                    target.LSDistance(Player.Position) >= getSliderItem(harassMenu, "eMinHarass"))
                    E1.CastOnUnit(target);

                if (getCheckBoxItem(harassMenu, "rHarass") && !Q.IsReady() && !W.IsReady() && !E.IsReady() && R.IsReady())
                    if (!Player.HasBuff("EliseSpiderW") || target.LSDistance(Player.Position) >= Player.AttackRange + 100)
                        if (realcdQ == 0 || realcdW == 0 || realcdE == 0)
                            R.Cast();
            }
        }

        private static void Killsteal()
        {
            foreach (
                var enemy in HeroManager.Enemies.Where(e => e.LSIsValidTarget() && e.LSDistance(Player.Position) <= E.Range)
                )
            {
                if (Human())
                {
                    if (getCheckBoxItem(ksMenu, "qKSH") && getCheckBoxItem(ksMenu, "wKSH") && Q.IsReady()
                        && W.IsReady())
                    {
                        if (enemy.LSDistance(Player.Position) <= Q.Range
                            && enemy.Health <= Q.GetDamage(enemy) + W.GetDamage(enemy))
                        {
                            W.Cast(enemy);
                            Q.CastOnUnit(enemy);
                            return;
                        }
                    }

                    if (getCheckBoxItem(ksMenu, "qKSH"))
                    {
                        if (Q.IsReady() && enemy.LSDistance(Player.Position) <= Q.Range &&
                            enemy.Health <= Q.GetDamage(enemy))
                        {
                            Q.CastOnUnit(enemy);
                            return;
                        }
                    }

                    if (getCheckBoxItem(ksMenu, "qKS") && getCheckBoxItem(ksMenu, "switchKS"))
                    {
                        if (realcdSQ == 0 && enemy.LSDistance(Player.Position) <= Q1.Range &&
                            enemy.Health <= Q1.GetDamage(enemy, 1))
                        {
                            if (R.IsReady())
                            {
                                R.Cast();
                                Q1.CastOnUnit(enemy);
                            }
                            return;
                        }
                    }

                    if (getCheckBoxItem(ksMenu, "wKSH"))
                    {
                        if (W.IsReady() && enemy.LSDistance(Player.Position) <= W.Range &&
                            enemy.Health <= W.GetDamage(enemy))
                        {
                            W.Cast(enemy);
                            return;
                        }
                    }
                }
                else if (!Human())
                {

                    if (getCheckBoxItem(ksMenu, "qKSH") && getCheckBoxItem(ksMenu, "wKSH") && realcdW == 0
                        && realcdQ == 0)
                    {
                        if (enemy.LSDistance(Player.Position) <= Q.Range
                            && enemy.Health <= Q.GetDamage(enemy) + W.GetDamage(enemy))
                        {
                            R.Cast();
                            W.Cast(enemy);
                            Q.CastOnUnit(enemy);
                            return;
                        }
                    }

                    if (getCheckBoxItem(ksMenu, "qKS"))
                    {
                        if (Q1.IsReady() && enemy.LSDistance(Player.Position) <= Q1.Range &&
                            enemy.Health <= Q1.GetDamage(enemy, 1))
                        {
                            Q1.CastOnUnit(enemy);
                            return;
                        }
                    }

                    if (getCheckBoxItem(ksMenu, "qKSH") && getCheckBoxItem(ksMenu, "switchKS"))
                    {
                        if (realcdQ == 0 && enemy.LSDistance(Player.Position) <= Q.Range &&
                            enemy.Health <= Q.GetDamage(enemy))
                        {
                            if (R.IsReady())
                            {
                                R.Cast();
                                Q.CastOnUnit(enemy);
                            }
                            return;
                        }
                    }

                    if (getCheckBoxItem(ksMenu, "wKSH") && getCheckBoxItem(ksMenu, "switchKS"))
                    {
                        if (realcdW == 0 && enemy.LSDistance(Player.Position) <= W.Range &&
                            enemy.Health <= W.GetDamage(enemy))
                        {
                            if (R.IsReady())
                            {
                                R.Cast();
                                W.Cast(enemy);
                            }
                            return;
                        }
                    }
                }
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Animation.Contains("Ult_E"))
                {
                    _e1Cd = Game.Time + CalculateCd(SpiderEcd[E.Level - 1]);
                    Utility.DelayAction.Add(100, PortAIO.OrbwalkerManager.ResetAutoAttackTimer);
                }
            }
        }

        private static void BeforeAttack(BeforeAttackArgs args)
        {
            if (getCheckBoxItem(miscMenu, "bAuto") && Human() && Q.IsReady() &&
                Player.LSDistance(args.Target.Position) >= Player.AttackRange)
            {
                args.Process = false;
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && Human() && gapcloser.Sender.LSIsValidTarget(E.Range) && getCheckBoxItem(miscMenu, "hGC"))
            {
                E.Cast(gapcloser.Sender);
                return;
            }

            if (realcdSE == 0 && gapcloser.Sender.LSIsValidTarget(E1.Range) && getCheckBoxItem(miscMenu, "fGC") &&
                gapcloser.End.LSDistance(Player.Position) >= getSliderItem(comboMenu, "eMin"))
            {
                if (Human() && R.IsReady())
                {
                    R.Cast();
                    E1.Cast(gapcloser.Sender);
                }
                else if (!Human())
                    E1.Cast(gapcloser.Sender);
            }
        }

        public static void OnAttack(OnAttackArgs args)
        {
            var target = args.Target;
            if (realcdSW > 0) return;
            if (!target.IsMe || target.Name.Contains("elisespiderling")) return;

            var aaDelay = Player.AttackDelay * 200 + Game.Ping / 2f;

            if (getCheckBoxItem(comboMenu, "wCombo"))
                if (target.Type == GameObjectType.AIHeroClient &&
                    PortAIO.OrbwalkerManager.isComboActive)
                    Utility.DelayAction.Add((int)aaDelay, () => W1.Cast());

            if (getCheckBoxItem(harassMenu, "wHarass"))
                if (target.Type == GameObjectType.AIHeroClient &&
                    PortAIO.OrbwalkerManager.isHarassActive)
                    Utility.DelayAction.Add((int)aaDelay, () => W1.Cast());

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                if (getCheckBoxItem(jungleClearMenu, "jungleclear.w.spider")
                    && target.Type == GameObjectType.NeutralMinionCamp)
                    Utility.DelayAction.Add((int)aaDelay, () => W1.Cast());

                if (getCheckBoxItem(laneClearMenu, "laneclear.w.spider")
                         && target.Type == GameObjectType.obj_AI_Minion)
                    Utility.DelayAction.Add((int)aaDelay, () => W1.Cast());
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                GetCDs(args);
            }
        }

        private static void Rappel()
        {
            if (getKeyBindItem(miscMenu, "rappel"))
            {
                if (Human() && R.IsReady() && realcdSE == 0)
                {
                    R.Cast();
                    E1.Cast();
                }
                else if (!Human() && realcdSE == 0)
                    E1.Cast();
            }
        }

        private static float CalculateCd(float time)
        {
            return time + (time * Player.PercentCooldownMod);
        }

        private static void Cooldowns()
        {
            realcdQ = ((_qCd - Game.Time) > 0) ? (_qCd - Game.Time) : 0;
            realcdW = ((_wCd - Game.Time) > 0) ? (_wCd - Game.Time) : 0;
            realcdE = ((_eCd - Game.Time) > 0) ? (_eCd - Game.Time) : 0;
            realcdSQ = ((_q1Cd - Game.Time) > 0) ? (_q1Cd - Game.Time) : 0;
            realcdSW = ((_w1Cd - Game.Time) > 0) ? (_w1Cd - Game.Time) : 0;
            realcdSE = ((_e1Cd - Game.Time) > 0) ? (_e1Cd - Game.Time) : 0;
        }

        private static void GetCDs(GameObjectProcessSpellCastEventArgs spell)
        {
            if (Human())
            {
                if (spell.SData.Name == "EliseHumanQ")
                    _qCd = Game.Time + CalculateCd(HumanQcd[Q.Level - 1]);
                if (spell.SData.Name == "EliseHumanW")
                    _wCd = Game.Time + CalculateCd(HumanWcd[W.Level - 1]);
                if (spell.SData.Name == "EliseHumanE")
                    _eCd = Game.Time + CalculateCd(HumanEcd[E.Level - 1]);
            }
            else
            {
                if (spell.SData.Name == "EliseSpiderQCast")
                    _q1Cd = Game.Time + CalculateCd(SpiderQcd[Q.Level - 1]);
                if (spell.SData.Name == "EliseSpiderW")
                    _w1Cd = Game.Time + CalculateCd(SpiderWcd[W.Level - 1]);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var wts = Drawing.WorldToScreen(Player.Position);
            if (!Human())
            {
                if (getCheckBoxItem(drawMenu, "drawSQ"))
                    Drawing.DrawCircle(Player.Position, Q1.Range, Color.LightCoral);
                if (getCheckBoxItem(drawMenu, "drawSE"))
                    Drawing.DrawCircle(Player.Position, E1.Range, Color.LightCoral);

                if (!getCheckBoxItem(drawMenu, "drawSpellCDs")) return;
                if (realcdQ == 0)
                    Drawing.DrawText(wts[0] - 120, wts[1] - 30, Color.White, "Q: Ready");
                else
                    Drawing.DrawText(wts[0] - 120, wts[1] - 30, Color.Orange, "Q: " + realcdQ.ToString("0.0"));
                if (realcdW == 0)
                    Drawing.DrawText(wts[0] - 40, wts[1] - 30, Color.White, "W: Ready");
                else
                    Drawing.DrawText(wts[0] - 40, wts[1] - 30, Color.Orange, "W: " + realcdW.ToString("0.0"));

                if (realcdE == 0)
                    Drawing.DrawText(wts[0] + 40, wts[1] - 30, Color.White, "E: Ready");
                else
                    Drawing.DrawText(wts[0] + 40, wts[1] - 30, Color.Orange, "E: " + realcdE.ToString("0.0"));
            }
            else
            {
                if (getCheckBoxItem(drawMenu, "drawHQ"))
                    Drawing.DrawCircle(Player.Position, Q.Range, Color.LightCoral);
                if (getCheckBoxItem(drawMenu, "drawHW"))
                    Drawing.DrawCircle(Player.Position, W.Range, Color.LightCoral);
                if (getCheckBoxItem(drawMenu, "drawHE"))
                    Drawing.DrawCircle(Player.Position, E.Range, Color.LightCoral);

                if (!getCheckBoxItem(drawMenu, "drawSpellCDs")) return;
                if (realcdSQ == 0)
                    Drawing.DrawText(wts[0] - 120, wts[1] - 30, Color.White, "Q: Ready");
                else
                    Drawing.DrawText(wts[0] - 120, wts[1] - 30, Color.Orange, "Q: " + realcdSQ.ToString("0.0"));
                if (realcdSW == 0)
                    Drawing.DrawText(wts[0] - 40, wts[1] - 30, Color.White, "W: Ready");
                else
                    Drawing.DrawText(wts[0] - 40, wts[1] - 30, Color.Orange, "W: " + realcdSW.ToString("0.0"));

                if (realcdSE == 0)
                    Drawing.DrawText(wts[0] + 40, wts[1] - 30, Color.White, "E: Ready");
                else
                    Drawing.DrawText(wts[0] + 40, wts[1] - 30, Color.Orange, "E: " + realcdSE.ToString("0.0"));
            }
        }

        private static void InitializeSpells()
        {
            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 950f);
            W.SetSkillshot(0.25f, 100f, 1500, true, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E, 1100f);
            E.SetSkillshot(0.25f, 55f, 1600, true, SkillshotType.SkillshotLine);

            Q1 = new Spell(SpellSlot.Q, 475f);
            W1 = new Spell(SpellSlot.W);
            E1 = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R);
        }

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

        private static void InitMenu()
        {
            Menu = MainMenu.AddMenu("Elise God", "EliseGod");

            comboMenu = Menu.AddSubMenu("Combo settings", "Combo");
            comboMenu.Add("qComboH", new CheckBox("Use Human Q", true));
            comboMenu.Add("qCombo", new CheckBox("Use Spider Q", true));
            comboMenu.Add("wComboH", new CheckBox("Use Human W", true));
            comboMenu.Add("wCombo", new CheckBox("Use Spider W", true));
            comboMenu.Add("eComboH", new CheckBox("Use Human E", true));
            comboMenu.Add("eCombo", new CheckBox("Use Spider E", true));
            comboMenu.Add("eMin", new Slider("E minimum range", 400, 0, 750));
            comboMenu.Add("rCombo", new CheckBox("Use R", true));


            harassMenu = Menu.AddSubMenu("Harass settings", "Harass");
            harassMenu.Add("qHarassH", new CheckBox("Use Human Q", true));
            harassMenu.Add("qHarass", new CheckBox("Use Spider Q", true));
            harassMenu.Add("wHarassH", new CheckBox("Use Human W", true));
            harassMenu.Add("wHarass", new CheckBox("Use Spider W", true));
            harassMenu.Add("eHarassH", new CheckBox("Use Human E", true));
            harassMenu.Add("eHarass", new CheckBox("Use Spider E", true));
            harassMenu.Add("eMinHarass", new Slider("E minimum range", 400, 0, 750));
            harassMenu.Add("rComboHarass", new CheckBox("Use R", true));
            harassMenu.Add("eMinHarass", new Slider("Mana manager (%)", 40, 1, 100));


            laneClearMenu = Menu.AddSubMenu("LaneClear settings", "LaneClear");
            laneClearMenu.Add("laneclear.q", new CheckBox("Use Human Q", true));
            laneClearMenu.Add("laneclear.q.spider", new CheckBox("Use Spider Q", true));
            laneClearMenu.Add("laneclear.w", new CheckBox("Use Human W", true));
            laneClearMenu.Add("laneclear.w.spider", new CheckBox("Use Spider W", true));
            laneClearMenu.Add("laneclear.mana", new Slider("Mana manager (%)", 40, 1, 100));



            jungleClearMenu = Menu.AddSubMenu("JungleClear settings", "JungleClear");
            jungleClearMenu.Add("jungleclear.q", new CheckBox("Use Human Q", true));
            jungleClearMenu.Add("jungleclear.q.spider", new CheckBox("Use Spider Q", true));
            jungleClearMenu.Add("jungleclear.w", new CheckBox("Use Human W", true));
            jungleClearMenu.Add("jungleclear.w.spider", new CheckBox("Use Spider W", true));
            jungleClearMenu.Add("jungleclear.r", new CheckBox("Use R", true));
            jungleClearMenu.Add("jungleclear.mana", new Slider("Mana manager (%)", 40, 1, 100));



            ksMenu = Menu.AddSubMenu("Killsteal settings", "Killsteal");
            ksMenu.Add("qKSH", new CheckBox("Use Human Q", true));
            ksMenu.Add("qKS", new CheckBox("Use Spider Q", true));
            ksMenu.Add("wKSH", new CheckBox("Use Human W", true));
            ksMenu.Add("switchKS", new CheckBox("Switch forms to KS", true));


            miscMenu = Menu.AddSubMenu("Misc settings", "Misc");
            miscMenu.Add("bAuto", new CheckBox("Block human auto if > Q range", true));
            miscMenu.Add("rappel", new KeyBind("Instant rappel", false, KeyBind.BindTypes.HoldActive, 'T'));
            miscMenu.Add("hGC", new CheckBox("Human anti-GC", true));
            miscMenu.Add("fGC", new CheckBox("Spider follow-GC", true));
            miscMenu.Add("interrupt", new CheckBox("Interrupt", true));
            miscMenu.Add("switchInterrupt", new CheckBox("Interrupt", true));


            drawMenu = Menu.AddSubMenu("Drawing settings", "Drawing");
            drawMenu.Add("drawSpellCDs", new CheckBox("Draw other form cooldowns", true));
            drawMenu.Add("drawHQ", new CheckBox("Draw Human Q", true));
            drawMenu.Add("drawHW", new CheckBox("Draw Human W", true));
            drawMenu.Add("drawHE", new CheckBox("Draw Human E", true));
            drawMenu.Add("drawSQ", new CheckBox("Draw Spider Q", true));
            drawMenu.Add("drawSE", new CheckBox("Draw Spider E", true));

        }
    }
}