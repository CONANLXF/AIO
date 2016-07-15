using System;
using System.Linq;
using System.Reflection;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace ThreshTherulerofthesoul
{
    class Program
    {
        #region Init

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Thresh")
                return;
            
            LoadSpellData();
            LoadMenu();

            Chat.Print("<font color=\"#66CCFF\" >Kaiser's Thresh -The ruler of the soul</font><font color=\"#CCFFFF\" >{0}</font> - " +
               "<font color=\"#FFFFFF\" >Version " + Assembly.GetExecutingAssembly().GetName().Version + "</font>", Player.ChampionName);

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            EscapeBlocker.OnDetectEscape += EscapeBlocker_OnDetectEscape;
        }
        
        public static Menu config;
        public static Menu Qmenu, Wmenu, Emenu, Rmenu, comboMenu, harassMenu, ksMenu, miscMenu, drawMenu;
        static Spell Q, W, E, R;
        static AIHeroClient catchedUnit = null;
        static int qTimer;
        static readonly AIHeroClient Player = ObjectManager.Player;

        //Mana
        static int QMana { get { return 80; } }
        static int WMana { get { return 50 * W.Level; } }
        static int EMana { get { return 60 * E.Level; } }
        static int RMana { get { return R.Level > 0 ? 100 : 0; } }

        static void LoadSpellData()
        {
            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 380);

            Q.SetSkillshot(0.5f, 60f, 1900f, true, SkillshotType.SkillshotLine);
        }

        static void LoadMenu()
        {
            config = MainMenu.AddMenu("Kaiser's Thresh", "Thresh");

            Qmenu = config.AddSubMenu("Q", "Q");
            Qmenu.Add("Predict", new ComboBox("Set Predict", 0, "L#Predict", "L#Predict2"));
            Qmenu.Add("C-UseQ", new CheckBox("Use Q"));
            Qmenu.Add("C-UseQ2", new CheckBox("Use Q2 AutoMatical"));


            Wmenu = config.AddSubMenu("W", "W");
            Wmenu.Add("C-UseHW", new CheckBox("Use Hooeked W"));
            Wmenu.Add("Use-SafeLantern", new CheckBox("Use SafeLantern for our team"));
            Wmenu.Add("C-UseSW", new CheckBox("Use Shield W Min 3"));


            Emenu = config.AddSubMenu("E", "E");
            Emenu.Add("C-UseE", new CheckBox("Use E"));


            Rmenu = config.AddSubMenu("R", "R");
            Rmenu.Add("C-UseR", new CheckBox("Use Auto R"));
            Rmenu.Add("minNoEnemies", new Slider("Min No. Of Enemies R", 2, 1, 5));


            comboMenu = config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("FlayPush", new KeyBind("Flay Push Key", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("FlayPull", new KeyBind("Flay Pull Key", false, KeyBind.BindTypes.HoldActive, 'H'));
            comboMenu.Add("SafeLanternKey", new KeyBind("Safe Lantern Key", false, KeyBind.BindTypes.HoldActive, 'U'));


            harassMenu = config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("H-UseQ", new CheckBox("Use Q"));
            harassMenu.Add("H-UseE", new CheckBox("Use E"));
            harassMenu.Add("Mana", new Slider("ManaManager", 30, 0, 100));


            ksMenu = config.AddSubMenu("KS", "KS");
            ksMenu.Add("KS-UseQ", new CheckBox("Use Q KS"));
            ksMenu.Add("KS-UseE", new CheckBox("Use E KS"));
            ksMenu.Add("KS-UseR", new CheckBox("Use R KS"));


            miscMenu = config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("UseEGapCloser", new CheckBox("Use E On Gap Closer"));
            miscMenu.Add("UseQInterrupt", new CheckBox("Use Q On Interrupt"));
            miscMenu.Add("UseEInterrupt", new CheckBox("Use E On Interrupt"));
            miscMenu.Add("AntiRengar", new CheckBox("Use E AntiGapCloser (Rengar Passive)(Beta)"));
            miscMenu.Add("DebugMode", new CheckBox("Debug Mode"));
            miscMenu.Add("BlockEscapeE", new CheckBox("Use E When Enemy have to Use Escape Skills"));

            Items.LoadItems();


            drawMenu = config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("Qcircle", new CheckBox("Q Range"));
            drawMenu.Add("Wcircle", new CheckBox("W Range"));
            drawMenu.Add("Ecircle", new CheckBox("E Range"));
            drawMenu.Add("Rcircle", new CheckBox("R Range"));
            drawMenu.Add("DrawTarget", new CheckBox("Draw Target"));

        }

        #endregion

        #region Logic

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
        
        static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var Etarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                if (target != null)
                {
                    if (CastQ2())
                    {
                        CastCatchedLatern();
                    }
                    if (getCheckBoxItem(Qmenu, "C-UseQ"))
                    {
                        CastQ(target);
                    }
                    if (getCheckBoxItem(Wmenu, "C-UseSW"))
                    {
                        ShieldLantern();
                    }
                    KSCheck(target);
                }

                if (Etarget != null)
                {
                    if (getCheckBoxItem(Emenu, "C-UseE") && E.IsReady())
                    {
                        CastE(Etarget);
                    }
                }
            }

            if (getKeyBindItem(comboMenu, "FlayPush") || getKeyBindItem(comboMenu, "FlayPull"))
            {
                PortAIO.OrbwalkerManager.MoveA(Game.CursorPos);
            }

            if (getKeyBindItem(comboMenu, "FlayPush") && Etarget != null && 
                E.IsReady())
            {
                Push(Etarget);
            }

            if (getKeyBindItem(comboMenu, "FlayPull") && Etarget != null &&
                E.IsReady())
            {
                Pull(Etarget);
            }

            if (getKeyBindItem(comboMenu, "SafeLanternKey"))
            {
                SafeLanternKeybind();
            }

            if (getCheckBoxItem(Wmenu, "Use-SafeLantern"))
            {
                SafeLantern();
            }
        }

        static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var Etarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            var mana = getSliderItem(harassMenu, "Mana");

            if (Player.ManaPercents() < mana)
                return;

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                if (getCheckBoxItem(harassMenu, "H-UseE") && E.IsReady() && Etarget != null)
                {
                    CastE(Etarget);
                }
                if (getCheckBoxItem(harassMenu, "H-UseQ") && Q.IsReady() && target != null)
                {
                    CastQ(target);
                }
            }
        }

        static void KSCheck(AIHeroClient target)
        {
            if (target != null)
            {
                if (getCheckBoxItem(ksMenu, "KS-UseQ"))
                {
                    var myDmg = Player.LSGetSpellDamage(target, SpellSlot.Q);
                    if (myDmg >= target.Health)
                    {
                        CastQ(target);
                    }
                }
                if (getCheckBoxItem(ksMenu, "KS-UseE"))
                {
                    var myDmg = Player.LSGetSpellDamage(target, SpellSlot.E);
                    if (myDmg >= target.Health)
                    {
                        CastE(target);
                    }
                }
                if (getCheckBoxItem(ksMenu, "KS-UseR"))
                {
                    var myDmg = Player.LSGetSpellDamage(target, SpellSlot.R);
                    if (myDmg >= target.Health)
                    {
                        if (Player.LSDistance(target.Position) <= R.Range)
                        {
                            R.Cast();
                        }
                    }
                }
            }
        }

        #endregion

        #region Q

        static void CastQ(AIHeroClient target)
        {
            if (!Q.IsReady() || target == null || Helper.EnemyHasShield(target) || !target.LSIsValidTarget())
                return;

            var Catched = IsPulling().Item1;
            var CatchedQtarget = IsPulling().Item2;

            if (!Catched && qTimer == 0)
            {
                if (!E.IsReady() || (E.IsReady() && 
                    E.Range < Player.LSDistance(target.Position)))
                {
                    var Mode = getBoxItem(Qmenu, "Predict");
                    
                    switch(Mode)
                    {
                        #region L# Predict
                        case 0:
                            {
                                var b = Q.GetPrediction(target);

                                if (b.Hitchance >= HitChance.High &&
                                    Player.LSDistance(target.ServerPosition) < Q.Range)
                                {
                                    Q.Cast(target);
                                }
                            }
                            break;
                        #endregion

                        #region L# Predict2
                        case 1:
                            {
                                if (Player.LSDistance(target.ServerPosition) < Q.Range)
                                {
                                    Q.CastIfHitchanceEquals(target, HitChance.High);
                                }
                            }
                            break;
                        #endregion
                    }
                }
            }
            else if (Catched && Environment.TickCount > qTimer - 200 && CastQ2() && CatchedQtarget.Type == GameObjectType.AIHeroClient && getCheckBoxItem(Qmenu, "C-UseQ2"))
            {
                Q.Cast();
            }
        }

        static bool CastQ2()
        {
            var status = false;
            var Catched = IsPulling().Item1;
            var CatchedQtarget = IsPulling().Item2;

            if (Catched && CatchedQtarget != null && 
                CatchedQtarget.Type == GameObjectType.AIHeroClient && 
                !Turret.IsUnderEnemyTurret(CatchedQtarget))
            {
                var EnemiesCount = Helper.GetEnemiesNearTarget(CatchedQtarget).Count();
                var AlliesCount = GetAlliesNearTarget(CatchedQtarget).Item1;
                var CanKill = GetAlliesNearTarget(CatchedQtarget).Item2;

                if (CanKill)
                {
                    EnemiesCount = EnemiesCount - 1;
                }

                if (EnemiesCount == 0)
                {
                    status = true;
                }
                else if (AlliesCount >= EnemiesCount)
                {
                    status = true;
                }
                else if (E.IsReady() && Turret.IsUnderAllyTurret(CatchedQtarget))
                {
                    status = true;
                }
            }
            return status;
        }

        static Tuple<int, bool> GetAlliesNearTarget(AIHeroClient target)
        {
            var Count = 0;
            var status = false;
            double dmg = 0;
            double allyDmg = 0;

            foreach (AIHeroClient allyhero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsDead))
            {
                if (allyhero.LSDistance(target.Position) <= 900)
                {
                    Count += 1;

                    dmg = dmg + Player.LSGetAutoAttackDamage(target, true);
                    dmg = dmg + E.GetDamage(target);
                    dmg = R.IsReady() ? dmg + R.GetDamage(target) : dmg;

                    if (allyhero.ChampionName != Player.ChampionName)
                    {
                        allyDmg = allyDmg + Helper.GetAlliesComboDmg(target, allyhero);
                    }

                }
            }

            if (E.IsReady())
            {
                dmg = dmg * 2;
                allyDmg = allyDmg * 1.5;
            }

            var totalDmg = dmg + allyDmg;

            if (totalDmg > target.Health)
            {
                status = true;
            }

            return new Tuple<int, bool>(Count, status);
        }

        #endregion

        #region W

        static void CastW(Vector3 Position)
        {
            if (!W.IsReady() || Player.LSDistance(Position) > W.Range)
                return;

            W.Cast(Position);
        }

        static void CastCatchedLatern()
        {
            if (!W.IsReady() || !getCheckBoxItem(Wmenu, "C-UseHW"))
                return;

            bool Catched = IsPulling().Item1;
            AIHeroClient CatchedQtarget = IsPulling().Item2;

            if (Catched && CatchedQtarget != null && CatchedQtarget.Type == GameObjectType.AIHeroClient)
            {
                var Wtarget = GetFurthestAlly(CatchedQtarget);
                if (Wtarget != null)
                {
                    if (Player.LSDistance(Wtarget.Position) <= W.Range)
                    {
                        CastW(Wtarget.Position);
                    }
                    else if (Player.LSDistance(Wtarget.Position) > W.Range)
                    {
                        var Pos = Player.Position + (Wtarget.Position - Player.Position).LSNormalized() * W.Range;

                        CastW(Pos);
                    }
                }
            }
        }

        static AIHeroClient GetFurthestAlly(AIHeroClient target)
        {
            AIHeroClient Wtarget = null;
            float distance = 0;

            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe && !x.IsDead))
            {
                if (Player.LSDistance(hero.Position) <= 1500 &&
                    hero.LSDistance(target.Position) > Player.LSDistance(target.Position))
                {
                    var temp = Player.LSDistance(hero.Position);

                    if (distance == 0 && Wtarget == null)
                    {
                        Wtarget = hero;
                        distance = Player.LSDistance(hero.Position);
                    }
                    else if (temp > distance)
                    {
                        Wtarget = hero;
                        distance = Player.LSDistance(hero.Position);
                    }
                }
            }
            return Wtarget;
        }

        static void ShieldLantern()
        {
            int count = 0;
            AIHeroClient target = null;
            foreach (var allyhero in ObjectManager.Get<AIHeroClient>().Where
                (x => x.IsAlly &&
                    Player.LSDistance(x.Position) < W.Range &&
                    !x.IsDead && !x.HasBuff("Recall")))
            {
                var tmp = LeagueSharp.Common.Utility.CountAlliesInRange(allyhero, 200);

                if (count == 0)
                {
                    count = tmp;
                }
                else if (tmp > count)
                {
                    count = tmp;
                }
            }

            if (count > 2 && target != null)
            {
                CastW(target.Position);
            }
        }

        static void SafeLantern()
        {
            if (!ManaManager())
                return;

            foreach (var hero in ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsAlly && !x.IsDead && !x.IsMe &&
                Player.LSDistance(x.Position) < 1500 && 
                !x.HasBuff("Recall")))
            {
                if (hero.HpPercents() < 25)
                {
                    if (Player.LSDistance(hero.Position) <= W.Range)
                    {
                        var Pos = W.GetPrediction(hero).CastPosition;

                        CastW(Pos);
                    }
                }
                else if (hero.HasBuffOfType(BuffType.Suppression) ||
                    hero.HasBuffOfType(BuffType.Taunt) ||
                    hero.HasBuffOfType(BuffType.Knockup) ||
                    hero.HasBuffOfType(BuffType.Flee))
                {
                    if (Player.LSDistance(hero.Position) <= W.Range)
                    {
                        CastW(hero.Position);
                    }
                }
            }
        }

        static void SafeLanternKeybind()
        {
            AIHeroClient Wtarget = null;
            float Hp = 0;

            foreach (var hero in ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsAlly && !x.IsDead && !x.IsMe &&
                Player.LSDistance(x.Position) < 1500 &&
                !x.HasBuff("Recall")))
            {
                var temp = hero.HpPercents();

                if (hero.HasBuffOfType(BuffType.Suppression) ||
                    hero.HasBuffOfType(BuffType.Taunt) ||
                    hero.HasBuffOfType(BuffType.Knockup) ||
                    hero.HasBuffOfType(BuffType.Flee))
                {
                    if (Player.LSDistance(hero.Position) <= W.Range)
                    {
                        CastW(hero.Position);
                    }
                }

                if (Wtarget == null && Hp == 0)
                {
                    Wtarget = hero;
                    Hp = temp;
                }
                else if (temp < Hp)
                {
                    Wtarget = hero;
                    Hp = temp;
                }
            }

            if (Wtarget != null)
            {
                CastW(Wtarget.Position);
            }
        }

        #endregion

        #region E

        static void CastE(AIHeroClient target)
        {
            if (!E.IsReady() || target == null || !target.LSIsValidTarget())
                return;

            bool Catched = IsPulling().Item1;
            AIHeroClient CatchedQtarget = IsPulling().Item2;

            if (!Catched && qTimer == 0)
            {
                if (Player.LSDistance(target.Position) <= E.Range)
                {
                    if (Player.HpPercents() < 20 && 
                        target.HpPercents() > 20)
                    {
                        Push(target);
                    }
                    else
                    {
                        Pull(target);
                    }
                }
            }
            else if (Catched && CatchedQtarget != null)
            {
                if (Environment.TickCount > qTimer - 200 && Player.LSDistance(CatchedQtarget.Position) <= E.Range)
                {
                    Pull(CatchedQtarget);
                }
            }
        }

        static void Pull(Obj_AI_Base target)
        {
            var pos = target.Position.LSExtend(Player.Position, Player.LSDistance(target.Position) + 200);
            E.Cast(pos);
        }

        static void Push(Obj_AI_Base target)
        {
            var pos = target.Position.LSExtend(Player.Position, Player.LSDistance(target.Position) - 200);
            E.Cast(pos);
        }

        #endregion

        #region R

        static void AutoR()
        {
            if (!R.IsReady() && getCheckBoxItem(Rmenu, "C-UseR"))
                return;

            // Menu Count
            int RequireCount = getSliderItem(Rmenu, "minNoEnemies");

            // Enemeis count in R range
            var hit = HeroManager.Enemies.Where(i => i.LSIsValidTarget(R.Range) && i.IsVisible && i.IsHPBarRendered).ToList();

            if (RequireCount <= hit.Count && R.IsReady())
            {
                R.Cast();
            }
        }

        #endregion

        #region Others

        static Tuple<bool, AIHeroClient> IsPulling()
        {
            bool Catched;
            AIHeroClient CatchedQtarget;

            if (catchedUnit != null)
            {
                Catched = true;
                CatchedQtarget = catchedUnit;
            }
            else
            {
                Catched = false;
                CatchedQtarget = null;
            }

            return new Tuple<bool, AIHeroClient>(Catched, CatchedQtarget);
        }

        static void CheckBuff()
        {
            if (Player.IsDead)
                return;

            foreach (AIHeroClient enemyhero in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy && !enemy.IsDead && enemy.IsValid))
            {
                if (enemyhero.HasBuff("ThreshQ") || enemyhero.HasBuff("threshqfakeknockup"))
                {
                    catchedUnit = enemyhero;
                    return;
                }
            }

            if (catchedUnit != null)
            {
                if (!catchedUnit.HasBuff("ThreshQ"))
                {
                    catchedUnit = null;
                }
            }
        }

        static bool ManaManager()
        {
            var status = false;
            var ReqMana = R.IsReady() ? QMana + EMana + RMana : QMana + EMana; 

            if (ReqMana < Player.Mana)
            {
                status = true;
            }
            else if (Player.MaxHealth * 0.3 > Player.Health)
            {
                status = true;
            }

            return status;
        }

        static bool Debug()
        {
            return getCheckBoxItem(miscMenu, "DebugMode");
        }

        public static void Debug(string s)
        {
            if (Debug())
            {
                Console.WriteLine("" + s);
            }
        }

        static void Debug(Vector3 pos)
        {
            if (!Debug())
                return;

            Drawing.OnDraw += delegate(EventArgs args)
            {
                Render.Circle.DrawCircle(pos, 150, System.Drawing.Color.Yellow);
            };
        }

        #endregion 

        #region Events

        static void Game_OnUpdate(EventArgs args)
        {
            CheckBuff();

            Combo();
            Harass();

            AutoR();
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "UseEInterrupt"))
                return;

            if (Player.LSDistance(sender.Position) < E.Range && sender.IsEnemy)
            {
                if (E.IsReady())
                {
                    Chat.Print("Debug : EInterrupt");
                    Pull(sender);
                }
            }

            if (Player.LSDistance(sender.ServerPosition) < Q.Range && 
                (!E.IsReady() || (E.IsReady() && E.Range < Player.LSDistance(sender.Position))) && 
                sender.IsEnemy && args.DangerLevel == Interrupter2.DangerLevel.High && 
                args.EndTime > Utils.TickCount + Q.Delay + (Player.LSDistance(sender.Position) / Q.Speed))
            {
                if (Q.IsReady())
                {
                    CastQ(sender);
                }
            }
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!getCheckBoxItem(miscMenu, "UseEGapCloser"))
                return;

            if (Player.LSDistance(gapcloser.Sender.Position) < E.Range && gapcloser.Sender.IsEnemy)
            {
                if (E.IsReady())
                {
                    Push(gapcloser.Sender);
                    Debug("AntiGapclose");
                }
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe && Player.LSDistance(x.Position) < E.Range))
            {
                if (gapcloser.End.LSDistance(hero.Position) < 100 &&
                    E.IsReady())
                {
                    Push(gapcloser.Sender);
                    Debug("AntiGapclose");
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            var QCircle = getCheckBoxItem(drawMenu, "Qcircle");
            var WCircle = getCheckBoxItem(drawMenu, "Wcircle");
            var ECircle = getCheckBoxItem(drawMenu, "Ecircle");
            var RCircle = getCheckBoxItem(drawMenu, "Rcircle");
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (QCircle)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.FromArgb(100, 255, 0, 255));
            }

            if (WCircle)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.FromArgb(100, 255, 0, 255));
            }

            if (ECircle)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.FromArgb(100, 255, 0, 255));
            }

            if (RCircle)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.FromArgb(100, 255, 0, 255));
            }

            if (getCheckBoxItem(drawMenu, "DrawTarget") && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 150, System.Drawing.Color.Red);
            }
        }

        static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Animation.ToLower() == "spell1_in")
                {
                    qTimer = Environment.TickCount + 1200;
                }
                else if (args.Animation.ToLower() == "spell1_out")
                {
                    qTimer = 0;
                }
                else if (args.Animation.ToLower() == "spell1_pull1")
                {
                    qTimer = Environment.TickCount + 900;
                }
                else if (args.Animation.ToLower() == "spell1_pull2")
                {
                    qTimer = Environment.TickCount + 900;
                }
                else if (qTimer > 0 && Environment.TickCount > qTimer)
                {
                    qTimer = 0;
                }
            }

            if (getCheckBoxItem(miscMenu, "AntiRengar"))
                return;

            if (!(sender is AIHeroClient))
                return;

            var _sender = sender as AIHeroClient;
            var dis = _sender.GetBuffCount("rengartrophyicon1") > 5 ? 600 : 750;
            
            if (_sender.ChampionName == "Rengar" && args.Animation == "Spell5" &&
                Player.LSDistance(_sender.Position) < dis && E.IsReady())
            {
                Push(_sender);
            }
        }
        
        static void EscapeBlocker_OnDetectEscape(AIHeroClient sender, GameObjectEscapeDetectorEventArgs args)
        {
            if (!sender.IsEnemy)
                return;

            #region BLockFlashEscape
            /*
            if (Menubool("BlockEscapeFlash") && sender.IsEnemy &&
                args.SpellData == "summonerflash")
            {
                if (Player.LSDistance(args.End) < Q.Range && Q.IsReady() &&
                    Player.LSDistance(args.End) > E.Range)
                {
                    Debug(args.End);
                    Debug("flash");

                    var predict = Q.GetPrediction(sender);

                    if (predict.Hitchance != HitChance.Collision)
                    {
                        Debug("EscapeFlash");
                        Q.Cast(args.End);
                    }
                }
            }
            */
            #endregion

            #region BLockSpellsEscape

            if (args.SpellData == "summonerflash")
                return;

            if (Player.LSDistance(args.Start) < E.Range && E.IsReady() &&
                    Player.LSDistance(args.End) > E.Range &&
                    getCheckBoxItem(miscMenu, "BlockEscapeE"))
            {
                Debug(args.End);
                Debug("EscapeE");
                Pull(sender);
            }
                /*
            else if ((!E.IsReady() || Player.LSDistance(args.Start) > E.Range) &&
                Player.LSDistance(args.End) < Q.Range && Q.IsReady() &&
                Player.LSDistance(args.End) > E.Range &&
                Menubool("BlockEscapeQ"))
            {
                var predict = Q.GetPrediction(sender);

                if (predict.Hitchance != HitChance.Collision)
                {
                    Debug(args.End);
                    Debug("EscapeQ");
                    Q.Cast(args.End);
                }
            }
            */
            #endregion
        }
        
        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Debug() && sender.IsEnemy && sender is AIHeroClient)
            {
                var _sender = sender as AIHeroClient;
                Console.WriteLine(": " + args.SData.Name + " - " + _sender.ChampionName + _sender.GetSpellSlot(args.SData.Name));
            }
        }

        #endregion
    }
}
