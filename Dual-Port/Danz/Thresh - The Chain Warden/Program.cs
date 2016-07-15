using System;
using System.Collections;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Collections.Generic;
using System.Threading;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;

using TargetSelector = PortAIO.TSManager; namespace Thresh___The_Chain_Warden
{
    class Program
    {
        private static Spell Q, Q2, W, E, R; //Same declaration as every new line, null object variable

        private static SpellSlot FlashSlot = SpellSlot.Unknown;

        public static float FlashRange = 450f;

        private static readonly Dictionary<int, List<Vector2>> _waypoints = new Dictionary<int, List<Vector2>>();
        private static float _lastCheck = Environment.TickCount;
        private static List<Spell> SpellList = new List<Spell>() { Q, Q2, W, E, R }; //Instead of SpellList.Add();

        private static Menu Config;
        public static Menu comboMenu, harassMenu, miscMenu, lanternMenu, flayMenu, drawMenu, debugMenu;
        public static Vector2 oWp;
        public static Vector2 nWp;
        public static AIHeroClient Player = ObjectManager.Player;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Thresh") return;
            FlashSlot = Player.GetSpellSlot("SummonerFlash");

            Q = new Spell(SpellSlot.Q, 1100);
            Q2 = new Spell(SpellSlot.Q, 1400);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(0.500f, 70, 1900f, true, SkillshotType.SkillshotLine);
            Q2.SetSkillshot(0.500f, 70, 1900f, true, SkillshotType.SkillshotLine);

            Config = MainMenu.AddMenu("Thresh", "thresh_menu");


            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseECombo", new CheckBox("Use E"));
            comboMenu.Add("UseRCombo", new CheckBox("Use R"));
            comboMenu.Add("EPush", new CheckBox("E Push/Pull(on/off)"));


            harassMenu = Config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q"));
            harassMenu.Add("UseEHarass", new CheckBox("Use E"));


            flayMenu = Config.AddSubMenu("Flay", "Flay");
            flayMenu.Add("Push", new KeyBind("Push", false, KeyBind.BindTypes.HoldActive, 'I'));
            flayMenu.Add("Pull", new KeyBind("Pull", false, KeyBind.BindTypes.HoldActive, 'U'));


            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("FlashQCombo", new KeyBind("Flash + Hook", false, KeyBind.BindTypes.HoldActive, 'G'));
            miscMenu.Add("EInterrupt", new CheckBox("Interrupt Spells with E"));
            miscMenu.Add("EGapCloser", new CheckBox("Auto use E away on Gap Closers"));
            miscMenu.Add("RGapCloser", new CheckBox("Auto use R on Gap Closers", false));


            lanternMenu = Config.AddSubMenu("Lantern Settings", "LanternSettings");
            lanternMenu.Add("ThrowLantern", new KeyBind("Throw Lantern to Ally", false, KeyBind.BindTypes.HoldActive, 'T'));
            lanternMenu.Add("ThrowLanternNear", new CheckBox("Prioritize Nearest Ally", true));
            lanternMenu.Add("ThrowLanternLife", new CheckBox("Prioritize Low Ally", false));


            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("drawEnable", new CheckBox("Enable Drawing", true));
            drawMenu.Add("drawQpred", new CheckBox("Draw Q line prediction", true));
            drawMenu.Add("drawQ", new CheckBox("Draw Q", false));
            drawMenu.Add("drawW", new CheckBox("Draw W", false));
            drawMenu.Add("drawE", new CheckBox("Draw E", false));
            drawMenu.Add("drawR", new CheckBox("Draw R", false));


            debugMenu = Config.AddSubMenu("Debug", "Debug");
            debugMenu.Add("debugE", new CheckBox("Debug E", false));
            debugMenu.Add("debugFlash", new CheckBox("Debug flash+hook", false));


            LSEvents.BeforeAttack += OnBeforeAttack; //You can use OnBeforeAttack event here instead of declaring new delegate in function
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;

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


        private static void OnDraw(EventArgs args)
        {
            var myPos = Drawing.WorldToScreen(Player.Position);

            if (getCheckBoxItem(drawMenu, "drawEnable"))
            {
                if (getCheckBoxItem(drawMenu, "drawQ"))
                {

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Aqua, 1);
                }

                if (getCheckBoxItem(drawMenu, "drawW"))
                {

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Aqua, 1);
                }
                if (getCheckBoxItem(drawMenu, "drawE"))
                {

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Aqua, 1);
                }
                if (getCheckBoxItem(drawMenu, "drawR"))
                {

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Aqua, 1);
                }

            }
            var enemy = TargetSelector.GetTarget(1500, DamageType.Magical);
            List<Vector2> waypoints = enemy.GetWaypoints();
            for (int i = 0; i < waypoints.Count - 1; i++)
            {


                oWp = Drawing.WorldToScreen(waypoints[i].To3D());
                nWp = Drawing.WorldToScreen(waypoints[i + 1].To3D());
                if (!waypoints[i].IsOnScreen() && !waypoints[i + 1].IsOnScreen())
                {
                    continue;
                }
                //Drawing.DrawLine(oWp[0], oWp[1], nWp[0], nWp[1], 3, System.Drawing.Color.Red);


                //var pos = Player.Position + Vector3.Normalize(enemy.Position - Player.Position) * 100;
                //pos = Player.Position + Vector3.Normalize(enemy.Position - Player.Position) * Player.Distance3D(enemy);
                //var ePos = Drawing.WorldToScreen(pos);


                if (getCheckBoxItem(drawMenu, "drawQpred"))
                {
                    Drawing.DrawLine(myPos.X - 25, myPos.Y - 25, nWp[0] - 10, nWp[1] - 25, 1, Color.Red);
                    Drawing.DrawLine(myPos.X + 25, myPos.Y + 25, nWp[0] + 10, nWp[1] + 25, 1, Color.Red);
                }

                if (getCheckBoxItem(debugMenu, "debugFlash"))
                {
                    Q2.UpdateSourcePosition(V2E(ObjectManager.Player.Position, enemy.Position, 400).To3D());
                    var predPos = Q2.GetPrediction(enemy);
                    Render.Circle.DrawCircle(V2E(ObjectManager.Player.Position, enemy.Position, 400).To3D(), 100, Color.Aqua, 1);
                    Drawing.DrawLine(Drawing.WorldToScreen(V2E(ObjectManager.Player.Position, enemy.Position, 400).To3D()), Drawing.WorldToScreen(predPos.CastPosition), 2, Color.Aqua);
                    var toScreen = Drawing.WorldToScreen(enemy.Position);
                    Drawing.DrawText(toScreen.X + 70, toScreen.Y, Color.Aqua, predPos.Hitchance.ToString());
                }

                if (getCheckBoxItem(debugMenu, "debugE"))
                {
                    var target2 = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                    if (!getCheckBoxItem(comboMenu, "EPush"))
                    {
                        Render.Circle.DrawCircle(V2E(target2.Position, Player.Position, Player.LSDistance(target2.Position) + 400).To3D(), 100, Color.Red, 1);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(target2.Position, 100, Color.Red, 1);
                    }
                }
            }
        }



        private static void DrawLine(float x, float y, float x2, float y2, float thickness, System.Drawing.Color color)
        {
        }


        private static void ThrowLantern()
        {
            if (W.IsReady())
            {
                var NearAllies = Player.GetAlliesInRange(W.Range) //W.Range instead of 1200, also there is no "On most damaged"
                                .Where(x => !x.IsMe)
                                .Where(x => !x.IsDead)
                                .Where(x => x.LSDistance(Player.Position) <= W.Range + 250)
                                .FirstOrDefault();

                if (NearAllies == null) return;

                W.Cast(NearAllies.Position);


            }

        }

        private static void OnGameUpdate(EventArgs args)
        {
            var targetz = TargetSelector.GetTarget(5000, DamageType.Magical);
            DrawLine(Player.Position.X, Player.Position.Y, targetz.Position.X, targetz.Position.Y, 2, Color.Red);

            if (getKeyBindItem(flayMenu, "Push"))
            {
                Push();
            }
            if (getKeyBindItem(flayMenu, "Pull"))
            {
                Pull();
            }
            if (getKeyBindItem(miscMenu, "FlashQCombo"))
            {
                FlashQCombo();
            }
            if (getKeyBindItem(lanternMenu, "ThrowLantern"))
            {
                ThrowLantern();
            }

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo();
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Harass();
            }

        }

        private static void OnPossibleToInterrupt(AIHeroClient target, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (getCheckBoxItem(miscMenu, "EInterrupt") && E.IsReady() && E.IsInRange(target))
            {
                E.Cast(target.ServerPosition);
            }
        }
        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (getCheckBoxItem(miscMenu, "EGapCloser") && E.IsReady() && E.IsInRange(gapcloser.Start))
            {
                E.Cast(Player.Position.LSExtend(gapcloser.Sender.Position, 250));
            }
            if (getCheckBoxItem(miscMenu, "RGapCloser") && R.IsReady() && R.IsInRange(gapcloser.Start))
                R.Cast();
            {
            }
        }
        

        private static void OnBeforeAttack(BeforeAttackArgs args)
        {
          if (args.Target.IsValid<Obj_AI_Minion>() && PortAIO.OrbwalkerManager.isHarassActive)

              {
                 args.Process = false;
              }
          }
       
        static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.LSTo2D() + distance * Vector3.Normalize(direction - from).LSTo2D();
        }

        private static void Pull()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (E.IsReady() && Player.LSDistance(target.Position) < E.Range)
            {
                E.Cast(target.Position.Extend(Player.Position, Vector3.Distance(target.Position, Player.Position) + 400));
            }
        }

        private static void Push()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (E.IsReady() && Player.LSDistance(target.Position) < E.Range)
            {
                E.Cast(target.Position);
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(1300, DamageType.Magical);

            if (Q.IsReady() && (getCheckBoxItem(harassMenu, "UseQHarass")))
            {
                var Qprediction = Q.GetPrediction(target);

                if (Qprediction.Hitchance >= HitChance.High && Qprediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {
                    Q.Cast(Qprediction.CastPosition);
                }

            }

            if (E.IsReady() && getCheckBoxItem(harassMenu, "UseEHarass") && Player.LSDistance(target.Position) < E.Range)
            {
                E.Cast(V2E(target.Position, Player.Position, Player.LSDistance(target.Position) + 400));
            }
        }
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1300, DamageType.Magical);
            if (Q.IsReady() && (getCheckBoxItem(comboMenu, "UseQCombo")))
            {
                Q.CastIfHitchanceEquals(target, HitChance.Dashing, true);
                Q.CastIfHitchanceEquals(target, HitChance.Immobile, true);
                var Qprediction = Q.GetPrediction(target);

                if (Qprediction.Hitchance >= HitChance.High && Qprediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {
                    Q.Cast(Qprediction.CastPosition);
                }

            }

            if (E.IsReady() && getCheckBoxItem(comboMenu, "UseECombo") && Vector3.Distance(target.Position, Player.Position) < E.Range)
            {
                if (!getCheckBoxItem(comboMenu, "EPush"))
                {
                    E.Cast(target.Position.Extend(Player.Position, Vector3.Distance(target.Position, Player.Position) + 400));
                }
                else
                {
                    E.Cast(target.Position);
                }
            }

            if (R.IsReady() && (getCheckBoxItem(comboMenu, "UseRCombo")) && Player.LSCountEnemiesInRange(R.Range) >= 1)
            {
                R.Cast();
            }


        }
        private static void FlashQCombo()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = TargetSelector.GetTarget(Q2.Range, DamageType.Magical);

            if (Player.Distance3D(target) > Q.Range)
            {
                if (FlashSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(FlashSlot) == SpellState.Ready && Q.IsReady())
                {
                    Q2.UpdateSourcePosition(V2E(ObjectManager.Player.Position, target.Position, FlashRange).To3D());
                    var predPos = Q2.GetPrediction(target);
                    if (predPos.Hitchance != HitChance.VeryHigh) //What does "Madlife" mean?
                        return;
                    Player.Spellbook.CastSpell(FlashSlot, predPos.CastPosition);
                    Q.Cast(predPos.CastPosition);
                }
            }


        }
    }

}
