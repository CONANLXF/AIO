using System;
using System.CodeDom;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using PortAIO.Properties;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

 namespace Slutty_ryze
{
    internal class Program
    {
        static readonly Random Seeder = new Random();

        public static int rRange { get; private set; }
        #region onload

        public static void OnLoad()
        {
            if (GlobalManager.GetHero.ChampionName != Champion.ChampName)
                return;

            Console.WriteLine(@"Loading Your Slutty Ryze");

            Humanizer.AddAction("generalDelay", 35.0f);

            Champion.Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 865);
            Champion.Qn = new LeagueSharp.Common.Spell(SpellSlot.Q, 865);
            Champion.W = new LeagueSharp.Common.Spell(SpellSlot.W, 585);
            Champion.E = new LeagueSharp.Common.Spell(SpellSlot.E, 585);
            Champion.R = new LeagueSharp.Common.Spell(SpellSlot.R);

            Champion.Q.SetSkillshot(0.25f, 50f, 1700f, true, SkillshotType.SkillshotLine);
            Champion.Qn.SetSkillshot(0.25f, 50f, 1700f, false, SkillshotType.SkillshotLine);

            //assign menu from MenuManager to Config
            Console.WriteLine(@"Loading Your Slutty Menu...");
            MenuManager.GetMenu();
            //GlobalManager.Config.AddToMainMenu();
            Printmsg("Ryze Assembly Loaded! Make sure to test new combo!");
            //Printmsg1("Current Version: " + typeof(Program).Assembly.GetName().Version);
            //Printmsg2("Don't Forget To " + "<font color='#00ff00'>[Upvote]</font> <font color='#FFFFFF'>" + "The Assembly In The Databse" + "</font>");
            //Other damge inficators in MenuManager ????
            //GlobalManager.DamageToUnit = Champion.GetComboDamage;

            Drawing.OnDraw += DrawManager.Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += Champion.OnGapClose;
            Interrupter2.OnInterruptableTarget += Champion.RyzeInterruptableSpell;
            Orbwalker.OnPreAttack += Champion.Orbwalking_BeforeAttack;
            ShowDisplayMessage();

        }

        #endregion
        private static void Printmsg(string message)
        {
            Chat.Print(
                "<font color='#6f00ff'>[Slutty Ryze]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg1(string message)
        {
            Chat.Print(
                "<font color='#ff00ff'>[Slutty Ryze]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg2(string message)
        {
            Chat.Print(
                "<font color='#00abff'>[Slutty Ryze]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        #region onGameUpdate

        private static void ShowDisplayMessage()
        {
            var r = new Random();

            var txt = Resources.display.Split('\n');
            switch (r.Next(1, 3))
            {
                case 2:
                    txt = Resources.display2.Split('\n');
                    break;
                case 3:
                    txt = Resources.display3.Split('\n');
                    break;
            }

            foreach (var s in txt)
                Console.WriteLine(s);
            #region L# does not allow D:
            //try
            //{
            //    var sr = new System.IO.StreamReader(System.Net.WebRequest.Create(string.Format("http://www.fiikus.net/asciiart/pokemon/{0}{1}{2}.txt", r.Next(0, 1), r.Next(0, 3), r.Next(0, 9))).GetResponse().GetResponseStream());
            //    string line;
            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        Console.WriteLine(line);
            //    }
            //}

            //catch
            //{
            //    // ignored
            //}
            #endregion
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
        
        private static void Game_OnUpdate(EventArgs args)
        {
            try // lazy
            {
                //var target2 = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical);
                //if (target2.IsValidTarget())
                //    Game.PrintChat(Champion.Q.GetPrediction(target2).CollisionObjects.Count.ToString());


                if (getKeyBindItem(MenuManager.chase, "chase"))
                {
                    switch (Champion.R.Level)
                    {
                        case 1:
                            rRange = 1500;
                            break;
                        case 2:
                            rRange = 3000;
                            break;
                    }
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    var targets = TargetSelector.GetTarget(Champion.W.Range + 200, DamageType.Magical);
                    if (targets == null)
                        return;

                    if (getCheckBoxItem(MenuManager.chase, "usewchase") && targets.IsValidTarget(Champion.E.Range))
                        LaneOptions.CastW(targets);

                    var target1 = TargetSelector.SelectedTarget;
                    if (!target1.LSIsValidTarget(rRange)) return;
                    if (getCheckBoxItem(MenuManager.chase, "chaser") &&
                        target1.LSDistance(GlobalManager.GetHero) > Champion.W.Range + 200 &&
                        targets.LSDistance(GlobalManager.GetHero) < rRange
                        && Champion.R.IsReady())
                    {
                        Champion.R.Cast(GlobalManager.GetHero.Position.LSExtend(target1.Position,
                            target1.LSDistance(GlobalManager.GetHero.Position) + 260));
                    }
                }

                if (GlobalManager.GetHero.IsDead)
                    return;

                if (GlobalManager.GetHero.IsRecalling())
                    return;

                PortAIO.OrbwalkerManager.SetAttack(true);

                var target = TargetSelector.GetTarget(Champion.Q.Range, DamageType.Magical);


                if (getCheckBoxItem(MenuManager.humanizerMenu, "doHuman"))
                {
                    if (!Humanizer.CheckDelay("generalDelay")) // Wait for delay for all other events
                    {
                        return;
                    }
                    //Console.WriteLine("Seeding Human Delay");
                    var nDelay = Seeder.Next(getSliderItem(MenuManager.humanizerMenu, "minDelay"), (getSliderItem(MenuManager.humanizerMenu, "maxDelay"))); // set a new random delay :D
                    Humanizer.ChangeDelay("generalDelay", nDelay);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {

                    var expires = (GlobalManager.GetHero.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires);
                    var CD =
                        (int)
                            (expires -
                             (Game.Time - 1));
                    if (Champion.W.IsReady() && !(CD < 2.5f))
                    {
                        //MenuManager.PortAIO.OrbwalkerManager.SetAttack(true);
                        PortAIO.OrbwalkerManager.SetAttack(true);
                    }
                    else
                    {
                        //MenuManager.PortAIO.OrbwalkerManager.SetAttack(false);
                        PortAIO.OrbwalkerManager.SetAttack(false);
                    }

                    Champion.AABlock();
                    LaneOptions.Combo();

                    PortAIO.OrbwalkerManager.SetAttack(!(target.LSDistance(GlobalManager.GetHero) >= getSliderItem(MenuManager.combo1Menu, "minaarange")));
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    LaneOptions.Mixed();
                    //MenuManager.PortAIO.OrbwalkerManager.SetAttack(true);
                    PortAIO.OrbwalkerManager.SetAttack(true);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                        LaneOptions.LaneClear();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                    LaneOptions.LastHit();


                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                {
                    if (getKeyBindItem(MenuManager.itemMenu, "tearS"))
                        ItemManager.TearStack();

                    ItemManager.Potion();
                    //MenuManager.PortAIO.OrbwalkerManager.SetAttack(true);
                    PortAIO.OrbwalkerManager.SetAttack(true);
                }

                if (getCheckBoxItem(MenuManager.mixedMenu, "UseQauto") && target != null)
                {
                    if (Champion.Q.IsReady() && target.LSIsValidTarget(Champion.Q.Range))
                        Champion.Q.Cast(target);
                }


                // Seplane();
                ItemManager.Item();
                Champion.KillSteal();
                ItemManager.Potion();

                if (getCheckBoxItem(MenuManager.eventMenu, "level"))
                {
                    AutoLevelManager.LevelUpSpells();
                }

                if (!getCheckBoxItem(MenuManager.eventMenu, "autow") || !target.UnderTurret(true)) return;

                if (target == null)
                    return;

                if (!ObjectManager.Get<Obj_AI_Turret>()
                    .Any(turret => turret.LSIsValidTarget(300) && turret.IsAlly && turret.Health > 0))
                    return;

                Champion.W.CastOnUnit(target);
                // DebugClass.ShowDebugInfo(true);
            }
            catch
            {
                // ignored
            }
        }
        #endregion

        /*
        private static void Seplane()
        {
            if (GlobalManager.GetHero.IsValid &&
                GlobalManager.Config.Item("seplane").GetValue<KeyBind>().Active)
            {
                ObjectManager.GlobalManager.GetHero.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                LaneClear();
            }
        }
         */



    }
}
