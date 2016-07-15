using TargetSelector = PortAIO.TSManager; namespace Flowers__Illaoi
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Core.Utils;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Spell = LeagueSharp.SDK.Spell;

    internal class Program
    {
        private static AIHeroClient Me;
        private static Menu Menu;
        public static Menu comboMenu, harassMenu, drawMenu, ksMenu, laneMenu, jungleMenu, itemMenu, blacklistMenu, miscMenu;
        private static Spell Q, W, E, R;
        private static SpellSlot Ignite = SpellSlot.Unknown;
        private static HpBarDraw DrawHpBar = new HpBarDraw();
        private static string[] AutoEnableList =
        {
             "Annie", "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana", "Evelynn", "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus",
             "Kassadin", "Katarina", "Kayle", "Kennen", "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna",
             "Ryze", "Sion", "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra", "Velkoz", "Azir", "Ekko",
             "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jayce", "Jinx", "KogMaw", "Lucian", "MasterYi", "MissFortune", "Quinn", "Shaco", "Sivir",
             "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Yasuo", "Zed", "Kindred", "AurelionSol"
        };


        private static void LoadSpell()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            W = new Spell(SpellSlot.W, 400f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 450f);

            Q.SetSkillshot(0.75f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.066f, 50f, 1900f, true, SkillshotType.SkillshotLine);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        public static void Load()
        {
            if (GameObjects.Player.ChampionName.ToLower() != "illaoi")
            {
                return;
            }

            Me = GameObjects.Player;

            Menu = MainMenu.AddMenu("Illaoi The Kraken Priestess", "Illaoi");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            {
                comboMenu.Add("Q", new CheckBox("Use Q", true));
                comboMenu.Add("QGhost", new CheckBox("Use Q | To Ghost", true));
                comboMenu.Add("W", new CheckBox("Use W", true));
                comboMenu.Add("WOutRange", new CheckBox("Use W | Out of Attack Range", true));
                comboMenu.Add("WUlt", new CheckBox("Use W | Ult Active", true));
                comboMenu.Add("E", new CheckBox("Use E", true));
                comboMenu.Add("R", new CheckBox("Use R", true));
                comboMenu.Add("RCount", new Slider("Use R | Counts Enemies >=", 2, 1, 5));
                comboMenu.Add("Ignite", new CheckBox("Use Ignite", true));
                comboMenu.Add("Item", new CheckBox("Use Items", true));
            }

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            {
                harassMenu.Add("Q", new CheckBox("Use Q", true));
                harassMenu.Add("WOutRange", new CheckBox("Use W | Only Out of Attack Range", true));
                harassMenu.Add("E", new CheckBox("Use E", true));
                harassMenu.Add("Mana", new Slider("Harass Mode | Min ManaPercent >=", 60, 1, 100));
            }

            laneMenu = Menu.AddSubMenu("LaneClear", "LaneClear");
            {
                laneMenu.Add("Q", new Slider("Use Q | Min Hit Count >=", 3, 1, 5));
                laneMenu.Add("Mana", new Slider("LaneClear Mode | Min ManaPercent >=", 60, 1, 100));
            }

            jungleMenu = Menu.AddSubMenu("JungleClear", "JungleClear");
            {
                jungleMenu.Add("Q", new CheckBox("Use Q", true));
                jungleMenu.Add("W", new CheckBox("Use W", true));
                jungleMenu.Add("Item", new CheckBox("Use Item", true));
                jungleMenu.Add("Mana", new Slider("JungleClear Mode | Min ManaPercent >=", 60, 1, 100));
            }

            ksMenu = Menu.AddSubMenu("KillSteal", "KillSteal");
            {
                ksMenu.Add("Q", new CheckBox("Use Q", true));
                ksMenu.Add("E", new CheckBox("Use E", true));
                ksMenu.Add("R", new CheckBox("Use R", true));
                ksMenu.AddGroupLabel("R List");
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => ksMenu.Add(i.ChampionName.ToLower(), new CheckBox(i.ChampionName)));
                }
            }

            blacklistMenu = Menu.AddSubMenu("E BlackList", "EBlackList");
            {
                blacklistMenu.AddGroupLabel("E BlackList");
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => blacklistMenu.Add(i.ChampionName.ToLower(), new CheckBox(i.ChampionName, false)));
                }
            }

            itemMenu = Menu.AddSubMenu("Items", "Items");
            {
                itemMenu.Add("Youmuus", new CheckBox("Use Youmuus", true));
                itemMenu.Add("Cutlass", new CheckBox("Use Cutlass", true));
                itemMenu.Add("Botrk", new CheckBox("Use Botrk", true));
                itemMenu.Add("Hydra", new CheckBox("Use Hydra", true));
                itemMenu.Add("Tiamat", new CheckBox("Use Tiamat", true));
            }

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            {
                miscMenu.Add("EGap", new CheckBox("Use E Anti GapCloset", true));
            }

            drawMenu = Menu.AddSubMenu("Draw", "Draw");
            {
                drawMenu.Add("Q", new CheckBox("Q Range", true));
                drawMenu.Add("W", new CheckBox("W Range", true));
                drawMenu.Add("E", new CheckBox("E Range", true));
                drawMenu.Add("R", new CheckBox("R Range", true));
                drawMenu.Add("DrawDamage", new CheckBox("Draw Combo Damage", true));
            }

            Events.OnGapCloser += OnGapCloser;
            Obj_AI_Base.OnSpellCast += OnDoCast;
            LeagueSharp.Common.LSEvents.AfterAttack += OnAction;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            LoadSpell();
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (miscMenu["EGap"].Cast<CheckBox>().CurrentValue && Args.IsDirectedToPlayer)
            {
                var sender = Args.Sender as AIHeroClient;

                if (sender.IsEnemy && (Args.End.DistanceToPlayer() <= 200 || sender.DistanceToPlayer() <= 250) && !blacklistMenu[sender.ChampionName.ToLower()].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast(sender);
                }
            }
        }
        
        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    if (comboMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && W.CanCast(target) && !AutoAttack.IsAutoAttack(Me.ChampionName))
                    {
                        if (comboMenu["WOutRange"].Cast<CheckBox>().CurrentValue && !InAutoAttackRange(target) && target.LSIsValidTarget(W.Range))
                        {
                            W.Cast();
                        }

                        if (comboMenu["WUlt"].Cast<CheckBox>().CurrentValue && Me.HasBuff("IllaoiR") && target.IsValidTarget(W.Range))
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        private static void OnAction(LeagueSharp.Common.AfterAttackArgs args)
        {
            var target = args.Target;
            if (target is AIHeroClient)
            {
                if (PortAIO.OrbwalkerManager.isComboActive)
                {
                    var target1 = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                    if (target1 != null && !target1.IsDead && !target1.IsZombie && target1.IsHPBarRendered)
                    {
                        if (comboMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && W.CanCast(target1) && !AutoAttack.IsAutoAttack(Me.ChampionName))
                        {
                            if (comboMenu["WOutRange"].Cast<CheckBox>().CurrentValue && !InAutoAttackRange(target1) && target1.LSIsValidTarget(W.Range))
                            {
                                W.Cast();
                            }

                            if (comboMenu["WUlt"].Cast<CheckBox>().CurrentValue && Me.HasBuff("IllaoiR") && target1.LSIsValidTarget(W.Range))
                            {
                                W.Cast();
                            }
                        }
                    }


                    if (PortAIO.OrbwalkerManager.isHarassActive)
                    {
                        if (Me.ManaPercent >= harassMenu["Mana"].Cast<Slider>().CurrentValue)
                        {
                            var target2 = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                            if (target2 != null && !target2.IsDead && !target2.IsZombie && target2.IsHPBarRendered)
                            {
                                if (harassMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && W.CanCast(target2) && !AutoAttack.IsAutoAttack(Me.ChampionName))
                                {
                                    if (harassMenu["WOutRange"].Cast<CheckBox>().CurrentValue && !InAutoAttackRange(target2) && target2.LSIsValidTarget(W.Range))
                                    {
                                        W.Cast();
                                    }
                                }
                            }
                        }
                    }

                    if (PortAIO.OrbwalkerManager.isLaneClearActive)
                    {
                        if (Me.ManaPercent >= jungleMenu["Mana"].Cast<Slider>().CurrentValue)
                        {
                            var Mobs = GameObjects.Jungle.Where(x => x.LSIsValidTarget(Q.Range) && !GameObjects.JungleSmall.Contains(x)).ToList();

                            if (Mobs.Count() > 0)
                            {
                                if (jungleMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && !AutoAttack.IsAutoAttack(Me.ChampionName))
                                {
                                    W.Cast();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

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
                Lane();
                Jungle();
            }

            KillSteal();
            Item();
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var Ghost = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsHPBarRendered && x.LSIsValidTarget(Q.Range)).FirstOrDefault(x => x.HasBuff("illaoiespirit"));
            if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
            {
                if (comboMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.LSIsValidTarget(Q.Range) && Q.CanCast(target) && !AutoAttack.IsAutoAttack(Me.ChampionName))
                {
                    Q.Cast(target);
                }

                if (comboMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && E.CanCast(target) && !AutoAttack.IsAutoAttack(Me.ChampionName) && !InAutoAttackRange(target) && target.LSIsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (comboMenu["R"].Cast<CheckBox>().CurrentValue && R.IsReady() && target.LSIsValidTarget(R.Range))
                {
                    if (target.Health <= GetDamage(target) + Me.GetAutoAttackDamage(target) * 3)
                    {
                        R.Cast(target);
                    }

                    if (Me.CountEnemyHeroesInRange(R.Range - 50) >= comboMenu["RCount"].Cast<Slider>().CurrentValue)
                    {
                        R.Cast();
                    }
                }

                if (comboMenu["Ignite"].Cast<CheckBox>().CurrentValue && Ignite != SpellSlot.Unknown && Ignite.IsReady() && target.LSIsValidTarget(600) && target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }
            }
            else if (target == null && Ghost != null)
            {
                if (Ghost != null && Q.IsReady() && comboMenu["QGhost"].Cast<CheckBox>().CurrentValue && comboMenu["Q"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(Ghost);
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= harassMenu["Mana"].Cast<Slider>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var Ghost = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsHPBarRendered && x.LSIsValidTarget(Q.Range)).FirstOrDefault(x => x.HasBuff("illaoiespirit"));
                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    if (harassMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.LSIsValidTarget(Q.Range) && Q.CanCast(target) && !AutoAttack.IsAutoAttack(Me.ChampionName))
                    {
                        Q.Cast(target);
                    }

                    if (harassMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && W.CanCast(target) && !AutoAttack.IsAutoAttack(Me.ChampionName))
                    {
                        if (harassMenu["WOutRange"].Cast<CheckBox>().CurrentValue && !InAutoAttackRange(target) && target.LSIsValidTarget(W.Range))
                        {
                            W.Cast();
                        }
                    }

                    if (harassMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && E.CanCast(target) && !blacklistMenu[target.ChampionName.ToLower()].Cast<CheckBox>().CurrentValue && !AutoAttack.IsAutoAttack(Me.ChampionName) && !InAutoAttackRange(target) && target.IsValidTarget(E.Range))
                    {
                        E.Cast(target);
                    }
                }
                else if (target == null && Ghost != null)
                {
                    if (Q.IsReady() && harassMenu["Q"].Cast<CheckBox>().CurrentValue)
                        Q.Cast(Ghost);

                    if (W.IsReady() && harassMenu["W"].Cast<CheckBox>().CurrentValue)
                    {
                        W.Cast();
                        PortAIO.OrbwalkerManager.ForcedTarget(Ghost);
                    }
                }
            }
        }

        private static void Lane()
        {
            if (Me.ManaPercent >= laneMenu["Mana"].Cast<Slider>().CurrentValue)
            {
                if (Q.IsReady())
                {
                    var Minions = GameObjects.EnemyMinions.Where(x => x.LSIsValidTarget(Q.Range)).ToList();

                    if (Minions.Count() > 0)
                    {
                        var QFarm = Q.GetLineFarmLocation(Minions, Q.Width);

                        if (QFarm.MinionsHit >= laneMenu["Q"].Cast<Slider>().CurrentValue)
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }
                }
            }
        }

        private static void Jungle()
        {
            if (Me.ManaPercent >= jungleMenu["Mana"].Cast<Slider>().CurrentValue)
            {
                var Mobs = GameObjects.Jungle.Where(x => x.LSIsValidTarget(Q.Range) && !GameObjects.JungleSmall.Contains(x)).ToList();

                if (Mobs.Count() > 0)
                {
                    if (jungleMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && !AutoAttack.IsAutoAttack(Me.ChampionName))
                    {
                        Q.Cast(Mobs.FirstOrDefault());
                    }

                    if (jungleMenu["Item"].Cast<CheckBox>().CurrentValue)
                    {
                        if (itemMenu["Hydra"].Cast<CheckBox>().CurrentValue && Items.HasItem(3074) && Mobs.FirstOrDefault().LSIsValidTarget(AttackRange()))
                        {
                            Items.UseItem(3074, Mobs.FirstOrDefault());
                        }

                        if (itemMenu["Tiamat"].Cast<CheckBox>().CurrentValue && Items.HasItem(3077) && Mobs.FirstOrDefault().LSIsValidTarget(AttackRange()))
                        {
                            Items.UseItem(3077, Mobs.FirstOrDefault());
                        }
                    }
                }
            }
        }

        private static void KillSteal()
        {
            if (ksMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var qt = GameObjects.EnemyHeroes.Where(x => x.LSIsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)).FirstOrDefault();

                if (qt != null)
                {
                    Q.Cast(qt);
                    return;
                }
            }

            if (ksMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var et = GameObjects.EnemyHeroes.Where(x => !blacklistMenu[x.ChampionName.ToLower()].Cast<CheckBox>().CurrentValue && x.LSIsValidTarget(E.Range) && x.Health < E.GetDamage(x)).FirstOrDefault();

                if (et != null)
                {
                    E.Cast(et);
                    return;
                }
            }

            if (ksMenu["R"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                var rt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range - 50) && x.Health < R.GetDamage(x) && ksMenu[x.ChampionName.ToLower()].Cast<CheckBox>().CurrentValue).FirstOrDefault();

                if (rt != null)
                {
                    R.Cast(rt);
                    return;
                }
            }
        }

        private static void Item()
        {
            if (comboMenu["Item"].Cast<CheckBox>().CurrentValue && PortAIO.OrbwalkerManager.isComboActive)
            {
                var target = TargetSelector.GetTarget(600, DamageType.Physical);

                if (itemMenu["Youmuus"].Cast<CheckBox>().CurrentValue && Items.HasItem(3142) && target.LSIsValidTarget(AttackRange() + 150))
                {
                    Items.UseItem(3142);
                }

                if (itemMenu["Cutlass"].Cast<CheckBox>().CurrentValue && Items.HasItem(3144) && target.LSIsValidTarget(AttackRange()))
                {
                    Items.UseItem(3144, target);
                }

                if (itemMenu["Bork"].Cast<CheckBox>().CurrentValue && Items.HasItem(3153) && target.LSIsValidTarget(AttackRange()))
                {
                    Items.UseItem(3153, target);
                }

                if (itemMenu["Hydra"].Cast<CheckBox>().CurrentValue && Items.HasItem(3074) && target.LSIsValidTarget(AttackRange()))
                {
                    Items.UseItem(3074, target);
                }

                if (itemMenu["Tiamat"].Cast<CheckBox>().CurrentValue && Items.HasItem(3077) && target.LSIsValidTarget(AttackRange()))
                {
                    Items.UseItem(3077, target);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (drawMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (drawMenu["W"].Cast<CheckBox>().CurrentValue && (W.IsReady() || Me.HasBuff("IllaoiW")))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.LightSeaGreen, 2);

            if (drawMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.LightYellow, 2);

            if (drawMenu["R"].Cast<CheckBox>().CurrentValue && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.OrangeRed, 2);

            if (drawMenu["DrawDamage"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(e => e.LSIsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg(GetDamage(target), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        private static float GetDamage(AIHeroClient target)
        {
            float Damage = 0f;

            if (Q.IsReady())
            {
                Damage += Q.GetDamage(target);
            }

            if (W.IsReady())
            {
                Damage += W.GetDamage(target);
            }

            if (E.IsReady())
            {
                Damage += E.GetDamage(target);
            }

            if (R.IsReady())
            {
                Damage += R.GetDamage(target);
            }

            return Damage;
        }

        //private static void CastSpell(Spell Spell, AIHeroClient target)
        //{
        //    var input = new PredictionInput
        //    {
        //        Range = Spell.Range,
        //        Delay = Spell.Delay,
        //        Radius = Spell.Width,
        //        Speed = Spell.Speed,
        //        Type = Spell.Type
        //    };

        //    var Pred = Movement.GetPrediction(input);

        //    if (Pred.Hitchance >= HitChance.VeryHigh)
        //    {
        //        Spell.Cast(Pred.CastPosition);
        //    }
        //}

        private static float AttackRange()
        {
            return Me.GetRealAutoAttackRange();
        }

        public static bool InAutoAttackRange(AttackableUnit target)
        {
            var baseTarget = (Obj_AI_Base)target;
            var myRange = AttackRange();

            if (baseTarget != null)
            {
                return baseTarget.IsHPBarRendered && Vector2.DistanceSquared(baseTarget.ServerPosition.ToVector2(), Me.ServerPosition.ToVector2()) <= myRange * myRange;
            }

            return target.LSIsValidTarget() && Vector2.DistanceSquared(target.Position.ToVector2(), Me.ServerPosition.ToVector2()) <= myRange * myRange;
        }

    }
}
