using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp.SDK;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.SDK.Core.Utils;
using SkillshotType = LeagueSharp.SDK.Enumerations.SkillshotType;
using LeagueSharp.SDK.Enumerations;

 namespace Taliyah
{
    class Program
    {
        private static Menu main_menu;
        private static LeagueSharp.SDK.Spell Q, W, E;
        private static Vector3 lastE;
        private static int lastETick = Environment.TickCount;
        private static bool Q5x = true;
        private static bool EWCasting = false;
        private static GameObject selectedGObj = null;
        private static bool pull_push_enemy = false;
        private static Menu comboMenu, harassMenu, laneclearMenu, drawing;

        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Taliyah")
                return;

            main_menu = MainMenu.AddMenu("Taliyah", "taliyah");

            comboMenu = main_menu.AddSubMenu("Combo", "taliyah.combo");
            comboMenu.Add("taliyah.combo.useq", new CheckBox("Use Q"));
            comboMenu.Add("taliyah.combo.usew", new CheckBox("Use W"));
            comboMenu.Add("taliyah.combo.usee", new CheckBox("Use E"));

            harassMenu = main_menu.AddSubMenu("Harass", "taliyah.harass");
            harassMenu.Add("taliyah.harass.useq", new CheckBox("Use Q"));
            harassMenu.Add("taliyah.harass.manaperc", new Slider("Min. Mana", 40, 0, 100));

            laneclearMenu = main_menu.AddSubMenu("LaneClear", "taliyah.laneclear");
            laneclearMenu.Add("taliyah.laneclear.useq", new CheckBox("Use Q"));
            laneclearMenu.Add("taliyah.laneclear.useew", new CheckBox("Use EW", false));
            laneclearMenu.Add("taliyah.laneclear.minq", new Slider("Min. Q Hit", 3, 1, 6));
            laneclearMenu.Add("taliyah.laneclear.minew", new Slider("Min. EW Hit", 5, 1, 6));
            laneclearMenu.Add("taliyah.laneclear.manaperc", new Slider("Min. Mana", 40));

            drawing = main_menu.AddSubMenu("Drawings", "taliyah.drawing");
            drawing.Add("taliyah.drawing.drawq", new CheckBox("Draw Q", true));
            drawing.Add("taliyah.drawing.draww", new CheckBox("Draw W", true));
            drawing.Add("taliyah.drawing.drawe", new CheckBox("Draw E", true));
            drawing.Add("taliyah.drawing.drawr", new CheckBox("Draw R Minimap", true));

            main_menu.Add("taliyah.onlyq5", new CheckBox("Only cast 5x Q", false));
            main_menu.Add("taliyah.antigap", new CheckBox("Auto E to Gapclosers"));
            main_menu.Add("taliyah.interrupt", new CheckBox("Auto W to interrupt spells"));
            main_menu.Add("taliyah.pullenemy", new KeyBind("Pull Selected Target", false, KeyBind.BindTypes.HoldActive, 'T'));
            main_menu.Add("taliyah.pushenemy", new KeyBind("Push Selected Target", false, KeyBind.BindTypes.HoldActive, 'G'));


            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(0f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 800f);
            W.SetSkillshot(0.5f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 700f);
            E.SetSkillshot(0.25f, 150f, 2000f, false, SkillshotType.SkillshotLine);

            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Events.OnGapCloser += Events_OnGapCloser;
            Events.OnInterruptableTarget += Events_OnInterruptableTarget;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (drawing["taliyah.drawing.drawr"].Cast<CheckBox>().CurrentValue && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).IsReady())
            {
                float radius = 3000f + 1500f * (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level - 1);
                var pointList = new List<Vector3>();
                for (var i = 0; i < 23; i++)
                {
                    var angle = i * Math.PI * 2 / 23;
                    pointList.Add(
                        new Vector3(
                            ObjectManager.Player.ServerPosition.X + radius * (float)Math.Cos(angle), ObjectManager.Player.ServerPosition.Y + radius * (float)Math.Sin(angle),
                            ObjectManager.Player.ServerPosition.Z));
                }

                for (var i = 0; i < pointList.Count; i++)
                {
                    var a = pointList[i];
                    var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];

                    var aonScreen = Drawing.WorldToMinimap(a);
                    var bonScreen = Drawing.WorldToMinimap(b);

                    Drawing.DrawLine(aonScreen.X, aonScreen.Y, bonScreen.X, bonScreen.Y, 1, System.Drawing.Color.White);
                }

            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.LBUTTONDOWN)
            {
                selectedGObj = ObjectManager.Get<Obj_AI_Base>().Where(p => p.IsValid && !p.IsMe && !p.IsDead && p.Distance(Game.CursorPos.ToVector2()) < 200 && p.IsAlly).FirstOrDefault();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (drawing["taliyah.drawing.drawq"].Cast<CheckBox>().CurrentValue)
                Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Red);

            if (drawing["taliyah.drawing.draww"].Cast<CheckBox>().CurrentValue)
                Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Green);

            if (drawing["taliyah.drawing.drawe"].Cast<CheckBox>().CurrentValue)
                Drawing.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Blue);
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.E)
                lastETick = Environment.TickCount;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Name == "Taliyah_Base_Q_aoe_bright.troy")
                Q5x = false;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Name == "Taliyah_Base_Q_aoe_bright.troy")
                Q5x = true;
        }

        private static void Combo()
        {
            if (W.Instance.Name == "TaliyahWNoClick")
            {
                //
            }
            else
            {
                if (W.IsReady()) //killable W
                {
                    var target = W.GetTarget();
                    if (target != null && target.Health < WDamage(target) - 50)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.High)
                            W.Cast(pred.UnitPosition, ObjectManager.Player.ServerPosition);
                    }

                }

                if (!EWCasting)
                {
                    if (E.IsReady() && getCheckBoxItem(comboMenu, "taliyah.combo.usee"))
                    {
                        if (W.IsReady() && getCheckBoxItem(comboMenu, "taliyah.combo.usew"))
                        {
                            //e w combo
                            var target = W.GetTarget();
                            if (target != null)
                            {
                                var pred = W.GetPrediction(target);
                                if (pred.Hitchance >= HitChance.High)
                                {
                                    lastE = ObjectManager.Player.ServerPosition;
                                    E.Cast(ObjectManager.Player.ServerPosition.ToVector2() + (pred.CastPosition.ToVector2() - ObjectManager.Player.ServerPosition.ToVector2()).Normalized() * (E.Range - 200));
                                    DelayAction.Add(250, () => { W.Cast(pred.UnitPosition, lastE); EWCasting = false; });
                                    EWCasting = true;
                                }
                            }
                            return;
                        }
                        else
                        {
                            var target = E.GetTarget();
                            if (target != null)
                            {
                                E.Cast(target);
                                lastE = ObjectManager.Player.ServerPosition;
                            }
                        }
                    }
                }
                if (W.IsReady() && getCheckBoxItem(comboMenu, "taliyah.combo.usew") && !EWCasting)
                {
                    var target = W.GetTarget();
                    if (target != null)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.High)
                            W.Cast(pred.UnitPosition, pred.UnitPosition);
                    }
                }
            }
            var qTarget = Q.GetTarget();
            if (qTarget != null && getCheckBoxItem(comboMenu, "taliyah.combo.useq") && (!getCheckBoxItem(main_menu, "taliyah.onlyq5") || Q5x))
                Q.Cast(qTarget);

        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(harassMenu, "taliyah.harass.manaperc"))
                return;

            if (getCheckBoxItem(harassMenu, "taliyah.harass.useq"))
            {
                var target = Q.GetTarget();
                if (target != null)
                    Q.Cast(target);
            }
        }

        private static void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(laneclearMenu, "taliyah.laneclear.manaperc"))
                return;

            if (getCheckBoxItem(laneclearMenu, "taliyah.laneclear.useq") && Q.IsReady())
            {
                var farm = Q.GetCircularFarmLocation(ObjectManager.Get<Obj_AI_Minion>().Where(p => p.IsEnemy && p.DistanceToPlayer() < Q.Range).ToList());
                if (farm.MinionsHit >= getSliderItem(laneclearMenu, "taliyah.laneclear.minq"))
                    Q.Cast(farm.Position);
            }

            if (getCheckBoxItem(laneclearMenu, "taliyah.laneclear.useew") && W.IsReady() && E.IsReady())
            {
                var farm = W.GetCircularFarmLocation(ObjectManager.Get<Obj_AI_Minion>().Where(p => p.IsEnemy && p.DistanceToPlayer() < W.Range).ToList());
                if (farm.MinionsHit >= getSliderItem(laneclearMenu, "taliyah.laneclear.minew"))
                {
                    E.Cast(farm.Position);
                    lastE = ObjectManager.Player.ServerPosition;
                    if (W.Instance.Name == "TaliyahW")
                        W.Cast(farm.Position, lastE.ToVector2());
                }
            }
        }

        private static void CheckKeyBindings()
        {
            if (main_menu["taliyah.pullenemy"].Cast<KeyBind>().CurrentValue || main_menu["taliyah.pushenemy"].Cast<KeyBind>().CurrentValue)
            {
                Orbwalker.MoveTo(Game.CursorPos);
                if (!pull_push_enemy && TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.IsValidTarget(W.Range))
                {
                    Vector3 push_position = ObjectManager.Player.ServerPosition;

                    if (main_menu["taliyah.pushenemy"].Cast<KeyBind>().CurrentValue)
                    {
                        if (selectedGObj != null && selectedGObj.Distance(ObjectManager.Player) < 1000)
                            push_position = selectedGObj.Position;
                        else
                            push_position = TargetSelector.SelectedTarget.ServerPosition + (TargetSelector.SelectedTarget.ServerPosition - ObjectManager.Player.ServerPosition).Normalized() * 50f;
                    }
                    var pred = W.GetPrediction(TargetSelector.SelectedTarget);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        pull_push_enemy = true;
                        W.Cast(pred.UnitPosition, push_position);
                        DelayAction.Add(250, () => pull_push_enemy = false);
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
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
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                CheckKeyBindings();
            }

            if (W.Instance.Name == "TaliyahWNoClick")
            {
                EWCasting = false;
            }

        }
        
        private static void Events_OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs e)
        {
            if (getCheckBoxItem(main_menu, "taliyah.interrupt"))
            {
                if (e.Sender.DistanceToPlayer() < W.Range)
                    W.Cast(e.Sender.ServerPosition);
            }
        }

        private static void Events_OnGapCloser(object sender, Events.GapCloserEventArgs e)
        {
            if (getCheckBoxItem(main_menu, "taliyah.antigap"))
            {
                if (e.End.DistanceToPlayer() < E.Range)
                    E.Cast(e.Sender.ServerPosition);
            }
        }

        private static float WDamage(Obj_AI_Base target)
        {
            return (float)ObjectManager.Player.CalculateDamage(target, DamageType.Magical, new int[] { 60, 80, 100, 120, 140 }[W.Level] + ObjectManager.Player.TotalMagicalDamage * 0.4f);
        }
    }
}