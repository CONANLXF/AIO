using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

 namespace ezEvade
{
    internal class Evade
    {
        public static AIHeroClient myHero { get { return ObjectManager.Player; } }

        public static SpellDetector spellDetector;
        private static SpellDrawer spellDrawer;
        private static PingTester pingTester;
        private static EvadeSpell evadeSpell;

        public static SpellSlot lastSpellCast;
        public static float lastSpellCastTime = 0;

        public static float lastWindupTime = 0;

        public static float lastTickCount = 0;
        public static float lastStopEvadeTime = 0;

        public static Vector3 lastMovementBlockPos = Vector3.Zero;
        public static float lastMovementBlockTime = 0;

        public static float lastEvadeOrderTime = 0;
        public static float lastIssueOrderGameTime = 0;
        public static float lastIssueOrderTime = 0;
        public static PlayerIssueOrderEventArgs lastIssueOrderArgs = null;

        public static Vector2 lastMoveToPosition = Vector2.Zero;
        public static Vector2 lastMoveToServerPos = Vector2.Zero;
        public static Vector2 lastStopPosition = Vector2.Zero;

        public static DateTime assemblyLoadTime = DateTime.Now;

        public static bool isDodging = false;
        public static bool dodgeOnlyDangerous = false;

        public static bool hasGameEnded = false;
        public static bool isChanneling = false;
        public static Vector2 channelPosition = Vector2.Zero;

        public static PositionInfo lastPosInfo;

        public static EvadeCommand lastEvadeCommand = new EvadeCommand { isProcessed = true, timestamp = EvadeUtils.TickCount };

        public static EvadeCommand lastBlockedUserMoveTo = new EvadeCommand { isProcessed = true, timestamp = EvadeUtils.TickCount };
        public static float lastDodgingEndTime = 0;

        public static Menu menu;

        public static float sumCalculationTime = 0;
        public static float numCalculationTime = 0;
        public static float avgCalculationTime = 0;

        public Evade()
        {
            LoadAssembly();
        }

        private void LoadAssembly()
        {
            DelayAction.Add(0, () =>
            {
                if (Game.Mode == GameMode.Running)
                {
                    Game_OnGameLoad(new EventArgs());
                }
                else
                {
                    Game.OnLoad += Game_OnGameLoad;
                }
            });
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                Player.OnIssueOrder += Game_OnIssueOrder;
                Spellbook.OnCastSpell += Game_OnCastSpell;
                Game.OnUpdate += Game_OnGameUpdate;

                AIHeroClient.OnProcessSpellCast += Game_OnProcessSpell;

                Game.OnEnd += Game_OnGameEnd;
                SpellDetector.OnProcessDetectedSpells += SpellDetector_OnProcessDetectedSpells;
                Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;

                /*Console.WriteLine("<font color=\"#66CCFF\" >Yomie's </font><font color=\"#CCFFFF\" >ezEvade</font> - " +
                   "<font color=\"#FFFFFF\" >Version " + Assembly.GetExecutingAssembly().GetName().Version + "</font>");
                */

                menu = MainMenu.AddMenu("ezEvade", "ezEvade");
                ObjectCache.menuCache.AddMenuToCache(menu);

                mainMenu = menu.AddSubMenu("Main", "Main");
                ObjectCache.menuCache.AddMenuToCache(mainMenu);
                mainMenu.Add("DodgeSkillShots", new KeyBind("Dodge SkillShots", true, KeyBind.BindTypes.PressToggle, 'K'));
                mainMenu.Add("ActivateEvadeSpells", new KeyBind("Use Evade Spells", true, KeyBind.BindTypes.PressToggle, 'K'));
                mainMenu.Add("DodgeDangerous", new CheckBox("Dodge Only Dangerous", false));
                mainMenu.Add("DodgeFOWSpells", new CheckBox("Dodge FOW SkillShots"));
                mainMenu.Add("DodgeCircularSpells", new CheckBox("Dodge Circular SkillShots"));

                spellDetector = new SpellDetector(menu);
                evadeSpell = new EvadeSpell(menu);

                keyMenu = menu.AddSubMenu("Key Settings", "KeySettings");
                ObjectCache.menuCache.AddMenuToCache(keyMenu);
                keyMenu.Add("DodgeDangerousKeyEnabled", new CheckBox("Enable Dodge Only Dangerous Keys", false));
                keyMenu.Add("DodgeDangerousKey", new KeyBind("Dodge Only Dangerous Key (On Hold)", false, KeyBind.BindTypes.HoldActive, 32));
                keyMenu.Add("DodgeDangerousKey2", new KeyBind("Dodge Only Dangerous Key 2 (Toggle)", false, KeyBind.BindTypes.PressToggle, 'V'));
                keyMenu.AddSeparator();
                keyMenu.Add("DodgeOnlyOnComboKeyEnabled", new CheckBox("Enable Dodge Only On Combo Key", false));
                keyMenu.Add("DodgeComboKey", new KeyBind("Dodge Only Combo Key", false, KeyBind.BindTypes.HoldActive, 32));
                keyMenu.AddSeparator();
                keyMenu.Add("DontDodgeKeyEnabled", new CheckBox("Enable Don't Dodge Key", false));
                keyMenu.Add("DontDodgeKey", new KeyBind("Don't Dodge Key", false, KeyBind.BindTypes.HoldActive, 'Z'));

                miscMenu = menu.AddSubMenu("Misc Settings", "MiscSettings");
                ObjectCache.menuCache.AddMenuToCache(miscMenu);
                miscMenu.AddGroupLabel("Misc : ");
                miscMenu.Add("HigherPrecision", new CheckBox("Enhanced Dodge Precision", false));
                miscMenu.Add("RecalculatePosition", new CheckBox("Recalculate Path"));
                miscMenu.Add("ContinueMovement", new CheckBox("Continue Last Movement"));
                miscMenu.Add("CalculateWindupDelay", new CheckBox("Calculate Windup Delay"));
                miscMenu.Add("CheckSpellCollision", new CheckBox("Check Spell Collision", false));
                miscMenu.Add("DodgeCheckHP", new CheckBox("Check My Hero HP%", false));
                miscMenu.Add("PreventDodgingUnderTower", new CheckBox("Prevent Dodging Under Tower", false));
                miscMenu.Add("PreventDodgingNearEnemy", new CheckBox("Prevent Dodging Near Enemies"));
                miscMenu.Add("AdvancedSpellDetection", new CheckBox("Advanced Spell Detection", false));
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("Profile : ");
                miscMenu.Add("EvadeMode", new ComboBox("Evade Mode", 0, "Smooth", "Fastest", "Very Smooth", "Hawk", "Kurisu"));
                miscMenu["EvadeMode"].Cast<ComboBox>().OnValueChange += OnEvadeModeChange;
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("Humanizer");
                miscMenu.Add("ClickOnlyOnce", new CheckBox("Click Only Once", true));
                miscMenu.Add("EnableEvadeDistance", new CheckBox("Extended Evade", false));
                miscMenu.Add("TickLimiter", new Slider("Tick Limiter", 100, 0, 500));
                miscMenu.Add("SpellDetectionTime", new Slider("Spell Detection Time", 0, 0, 1000));
                miscMenu.Add("ReactionTime", new Slider("Reaction Time", 0, 0, 500));
                miscMenu.Add("DodgeInterval", new Slider("Dodge Interval", 0, 0, 2000));
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("Fast Evade");
                miscMenu.Add("FastMovementBlock", new CheckBox("Fast Movement Block", false));
                miscMenu.Add("FastEvadeActivationTime", new Slider("FastEvade Activation Time", 65, 0, 500));
                miscMenu.Add("SpellActivationTime", new Slider("Spell Activation Time", 200, 0, 1000));
                miscMenu.Add("RejectMinDistance", new Slider("Collision Distance Buffer", 10, 0, 100));
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("Extra Buffers");
                miscMenu.Add("ExtraPingBuffer", new Slider("Extra Ping Buffer", 65, 0, 200));
                miscMenu.Add("ExtraCPADistance", new Slider("Extra Collision Distance", 10, 0, 150));
                miscMenu.Add("ExtraSpellRadius", new Slider("Extra Spell Radius", 0, 0, 100));
                miscMenu.Add("ExtraEvadeDistance", new Slider("Extra Evade Distance", 100, 0, 300));
                miscMenu.Add("ExtraAvoidDistance", new Slider("Extra Avoid Distance", 50, 0, 300));
                miscMenu.Add("MinComfortZone", new Slider("Min Distance to Champion", 550, 0, 1000));
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("Tests");
                miscMenu.Add("LoadPingTester", new CheckBox("Load Ping Tester", false));
                miscMenu["LoadPingTester"].Cast<CheckBox>().OnValueChange += OnLoadPingTesterChange;

                spellDrawer = new SpellDrawer(menu);

                var initCache = ObjectCache.myHeroCache;

                //spellTester = new SpellTester();

                Console.WriteLine("ezEvade Loaded");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //private static SpellTester spellTester;
        public static Menu miscMenu, keyMenu, mainMenu;

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

        private void OnEvadeModeChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            var mode = args.NewValue;

            if (mode == 2)
            {
                miscMenu["FastEvadeActivationTime"].Cast<Slider>().CurrentValue = 0;
                miscMenu["RejectMinDistance"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ExtraCPADistance"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ExtraPingBuffer"].Cast<Slider>().CurrentValue = 40;
            }
            else if (mode == 0)
            {
                miscMenu["FastEvadeActivationTime"].Cast<Slider>().CurrentValue = 65;
                miscMenu["RejectMinDistance"].Cast<Slider>().CurrentValue = 10;
                miscMenu["ExtraCPADistance"].Cast<Slider>().CurrentValue = 10;
                miscMenu["ExtraPingBuffer"].Cast<Slider>().CurrentValue = 65;
            }
            else if (mode == 3)
            {
                mainMenu["DodgeDangerous"].Cast<CheckBox>().CurrentValue = false;
                mainMenu["DodgeFOWSpells"].Cast<CheckBox>().CurrentValue = false;
                mainMenu["DodgeCircularSpells"].Cast<CheckBox>().CurrentValue = false;
                keyMenu["DodgeDangerousKeyEnabled"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["HigherPrecision"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["RecalculatePosition"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["ContinueMovement"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["CalculateWindupDelay"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["CheckSpellCollision"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["PreventDodgingUnderTower"].Cast<CheckBox>().CurrentValue = false;
                miscMenu["PreventDodgingNearEnemy"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["AdvancedSpellDetection"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["ClickOnlyOnce"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["EnableEvadeDistance"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["TickLimiter"].Cast<Slider>().CurrentValue = 200;
                miscMenu["SpellDetectionTime"].Cast<Slider>().CurrentValue = 375;
                miscMenu["ReactionTime"].Cast<Slider>().CurrentValue = 285;
                miscMenu["DodgeInterval"].Cast<Slider>().CurrentValue = 235;
                miscMenu["FastEvadeActivationTime"].Cast<Slider>().CurrentValue = 0;
                miscMenu["SpellActivationTime"].Cast<Slider>().CurrentValue = 200;
                miscMenu["RejectMinDistance"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ExtraPingBuffer"].Cast<Slider>().CurrentValue = 65;
                miscMenu["ExtraCPADistance"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ExtraSpellRadius"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ExtraEvadeDistance"].Cast<Slider>().CurrentValue = 200;
                miscMenu["ExtraAvoidDistance"].Cast<Slider>().CurrentValue = 200;
                miscMenu["MinComfortZone"].Cast<Slider>().CurrentValue = 550;
            }

            else if (mode == 4)
            {
                mainMenu["DodgeDangerous"].Cast<CheckBox>().CurrentValue = false;
                mainMenu["DodgeFOWSpells"].Cast<CheckBox>().CurrentValue = false;
                mainMenu["DodgeCircularSpells"].Cast<CheckBox>().CurrentValue = true;
                keyMenu["DodgeDangerousKeyEnabled"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["HigherPrecision"].Cast<CheckBox>().CurrentValue = false;
                miscMenu["RecalculatePosition"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["ContinueMovement"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["CalculateWindupDelay"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["CheckSpellCollision"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["PreventDodgingUnderTower"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["PreventDodgingNearEnemy"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["AdvancedSpellDetection"].Cast<CheckBox>().CurrentValue = false;
                miscMenu["ClickOnlyOnce"].Cast<CheckBox>().CurrentValue = true;
                miscMenu["EnableEvadeDistance"].Cast<CheckBox>().CurrentValue = false;

                miscMenu["TickLimiter"].Cast<Slider>().CurrentValue = 100;
                miscMenu["SpellDetectionTime"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ReactionTime"].Cast<Slider>().CurrentValue = 0;
                miscMenu["DodgeInterval"].Cast<Slider>().CurrentValue = 250;
                miscMenu["FastEvadeActivationTime"].Cast<Slider>().CurrentValue = 45;
                miscMenu["SpellActivationTime"].Cast<Slider>().CurrentValue = 200;
                miscMenu["RejectMinDistance"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ExtraPingBuffer"].Cast<Slider>().CurrentValue = 65;
                miscMenu["ExtraCPADistance"].Cast<Slider>().CurrentValue = 10;
                miscMenu["ExtraSpellRadius"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ExtraEvadeDistance"].Cast<Slider>().CurrentValue = 165;
                miscMenu["ExtraAvoidDistance"].Cast<Slider>().CurrentValue = 20;
                miscMenu["MinComfortZone"].Cast<Slider>().CurrentValue = 330;
            }
        }

        private void OnLoadPingTesterChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            miscMenu["LoadPingTester"].Cast<CheckBox>().CurrentValue = false;
            if (pingTester == null)
            {
                pingTester = new PingTester();
            }
        }

        private void Game_OnGameEnd(GameEndEventArgs args)
        {
            hasGameEnded = true;
        }

        private void Game_OnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {
            if (!spellbook.Owner.IsMe)
                return;

            var sData = spellbook.GetSpell(args.Slot);
            string name;

            if (SpellDetector.channeledSpells.TryGetValue(sData.Name, out name))
            {
                //Evade.isChanneling = true;
                //Evade.channelPosition = ObjectCache.myHeroCache.serverPos2D;
                lastStopEvadeTime = EvadeUtils.TickCount + ObjectCache.gamePing + 100;
            }

            //block spell commmands if evade spell just used
            if (EvadeSpell.lastSpellEvadeCommand != null &&
                EvadeSpell.lastSpellEvadeCommand.timestamp + ObjectCache.gamePing + 150 > EvadeUtils.TickCount)
            {
                args.Process = false;
            }

            lastSpellCast = args.Slot;
            lastSpellCastTime = EvadeUtils.TickCount;

            //moved from processPacket

            /*if (args.Slot == SpellSlot.Recall)
            {
                lastStopPosition = myHero.ServerPosition.To2D();
            }*/

            if (Situation.ShouldDodge())
            {
                if (isDodging && SpellDetector.spells.Any())
                {
                    foreach (KeyValuePair<String, SpellData> entry in SpellDetector.windupSpells)
                    {
                        SpellData spellData = entry.Value;

                        if (spellData.spellKey == args.Slot) //check if it's a spell that we should block
                        {
                            args.Process = false;
                            return;
                        }
                    }
                }
            }

            foreach (var evadeSpell in EvadeSpell.evadeSpells)
            {
                if (evadeSpell.isItem == false &&
                    evadeSpell.spellKey == args.Slot && evadeSpell.untargetable == false)
                {
                    if (evadeSpell.evadeType == EvadeType.Blink)
                    {
                        var dir = (args.StartPosition.To2D() - myHero.ServerPosition.To2D()).Normalized();

                        var end = myHero.ServerPosition.To2D() + dir * myHero.ServerPosition.To2D().Distance(Game.CursorPos.To2D());
                        if (evadeSpell.fixedRange || end.Distance(myHero.ServerPosition.To2D()) > evadeSpell.range)
                        {
                            end = myHero.ServerPosition.To2D() + dir * evadeSpell.range;
                        }

                        var posInfo = EvadeHelper.CanHeroWalkToPos(end, evadeSpell.speed, ObjectCache.gamePing, 0);
                        if (posInfo.posDangerCount > 0)
                        {
                            args.Process = false;
                        }
                        else
                        {
                            lastPosInfo.position = end;
                            lastDodgingEndTime = EvadeUtils.TickCount;

                            if (isDodging || EvadeUtils.TickCount < lastDodgingEndTime + 500)
                            {
                                EvadeCommand.MoveTo(Game.CursorPos.To2D()); //block moveto
                                lastStopEvadeTime = EvadeUtils.TickCount + ObjectCache.gamePing + 100;
                            }

                            return;
                        }
                    }

                    if (evadeSpell.evadeType == EvadeType.Dash)
                    {
                        var dashPos = args.StartPosition.To2D();

                        if (args.Target != null)
                        {
                            dashPos = args.Target.Position.To2D();
                        }

                        if (evadeSpell.fixedRange || dashPos.Distance(myHero.ServerPosition.To2D()) > evadeSpell.range)
                        {
                            var dir = (dashPos - myHero.ServerPosition.To2D()).Normalized();
                            dashPos = myHero.ServerPosition.To2D() + dir * evadeSpell.range;
                        }

                        var posInfo = EvadeHelper.CanHeroWalkToPos(dashPos, evadeSpell.speed, ObjectCache.gamePing, 0);
                        if (posInfo.posDangerLevel > 0)
                        {
                            args.Process = false;
                        }
                        else
                        {
                            lastPosInfo.position = dashPos;
                            lastDodgingEndTime = EvadeUtils.TickCount;

                            if (isDodging || EvadeUtils.TickCount < lastDodgingEndTime + 500)
                            {
                                EvadeCommand.MoveTo(Game.CursorPos.To2D()); //block moveto
                                lastStopEvadeTime = EvadeUtils.TickCount + ObjectCache.gamePing + 100;
                            }

                            return;
                        }
                    }

                    lastPosInfo = PositionInfo.SetAllUndodgeable();
                    return;
                }
            }


        }
        private void Game_OnIssueOrder(Obj_AI_Base hero, PlayerIssueOrderEventArgs args)
        {
            if (!hero.IsMe)
                return;

            if (!Situation.ShouldDodge())
                return;

            if (args.Order == GameObjectOrder.MoveTo)
            {
                if (isDodging && SpellDetector.spells.Any())
                {
                    CheckHeroInDanger();

                    lastBlockedUserMoveTo = new EvadeCommand
                    {
                        order = EvadeOrderCommand.MoveTo,
                        targetPosition = args.TargetPosition.To2D(),
                        timestamp = EvadeUtils.TickCount,
                        isProcessed = false,
                    };

                    args.Process = false;
                }
                else
                {
                    var movePos = args.TargetPosition.To2D();
                    var extraDelay = ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;

                    if (EvadeHelper.CheckMovePath(movePos, ObjectCache.gamePing + extraDelay))
                    {
                        /*if (ObjectCache.menuCache.cache["AllowCrossing"].Cast<CheckBox>().CurrentValue)
                        {
                            var extraDelayBuffer = ObjectCache.menuCache.cache["ExtraPingBuffer"]
                                .GetValue<Slider>().Value + 30;
                            var extraDist = ObjectCache.menuCache.cache["ExtraCPADistance"]
                                .GetValue<Slider>().Value + 10;
                            var tPosInfo = EvadeHelper.CanHeroWalkToPos(movePos, ObjectCache.myHeroCache.moveSpeed, extraDelayBuffer + ObjectCache.gamePing, extraDist);
                            if (tPosInfo.posDangerLevel == 0)
                            {
                                lastPosInfo = tPosInfo;
                                return;
                            }
                        }*/

                        lastBlockedUserMoveTo = new EvadeCommand
                        {
                            order = EvadeOrderCommand.MoveTo,
                            targetPosition = args.TargetPosition.To2D(),
                            timestamp = EvadeUtils.TickCount,
                            isProcessed = false,
                        };

                        args.Process = false; //Block the command

                        if (EvadeUtils.TickCount - lastMovementBlockTime < 500 && lastMovementBlockPos.Distance(args.TargetPosition) < 100)
                        {
                            return;
                        }

                        lastMovementBlockPos = args.TargetPosition;
                        lastMovementBlockTime = EvadeUtils.TickCount;

                        var posInfo = EvadeHelper.GetBestPositionMovementBlock(movePos);
                        if (posInfo != null)
                        {
                            EvadeCommand.MoveTo(posInfo.position);
                        }
                        return;
                    }
                    else
                    {
                        lastBlockedUserMoveTo.isProcessed = true;
                    }
                }
            }
            else //need more logic
            {
                if (isDodging)
                {
                    args.Process = false; //Block the command
                }
                else
                {
                    if (args.Order == GameObjectOrder.AttackUnit)
                    {
                        var target = args.Target;
                        if (target != null && target.IsValid<Obj_AI_Base>())
                        {
                            var baseTarget = target as Obj_AI_Base;
                            if (ObjectCache.myHeroCache.serverPos2D.Distance(baseTarget.ServerPosition.To2D()) >
                                myHero.AttackRange + ObjectCache.myHeroCache.boundingRadius + baseTarget.BoundingRadius)
                            {
                                var movePos = args.TargetPosition.To2D();
                                var extraDelay = ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;
                                if (EvadeHelper.CheckMovePath(movePos, ObjectCache.gamePing + extraDelay))
                                {
                                    args.Process = false; //Block the command
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (args.Process == true)
            {
                lastIssueOrderGameTime = Game.Time * 1000;
                lastIssueOrderTime = EvadeUtils.TickCount;
                lastIssueOrderArgs = args;

                if (args.Order == GameObjectOrder.MoveTo)
                {
                    lastMoveToPosition = args.TargetPosition.To2D();
                    lastMoveToServerPos = myHero.ServerPosition.To2D();
                }

                if (args.Order == GameObjectOrder.Stop)
                {
                    lastStopPosition = myHero.ServerPosition.To2D();
                }
            }
        }

        private void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (isDodging)
            {
                args.Process = false; //Block orbwalking
            }
        }

        private void Game_OnProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (!hero.IsMe)
            {
                return;
            }

            if (hero.NetworkId != myHero.NetworkId)
            {
                Console.WriteLine("IS NOT ME");
            }

            /*if (args.SData.Name.Contains("Recall"))
            {
                var distance = lastStopPosition.Distance(args.Start.To2D());
                float moveTime = 1000 * distance / myHero.MoveSpeed;
                Console.WriteLine("Extra dist: " + distance + " Extra Delay: " + moveTime);
            }*/

            string name;
            if (SpellDetector.channeledSpells.TryGetValue(args.SData.Name, out name))
            {
                Evade.isChanneling = true;
                Evade.channelPosition = myHero.ServerPosition.To2D();
            }

            if (ObjectCache.menuCache.cache["CalculateWindupDelay"].Cast<CheckBox>().CurrentValue)
            {
                var castTime = (hero.Spellbook.CastTime - Game.Time) * 1000;

                if (castTime > 0 && !Orbwalking.IsAutoAttack(args.SData.Name)
                    && Math.Abs(castTime - myHero.AttackCastDelay * 1000) > 1)
                {
                    Evade.lastWindupTime = EvadeUtils.TickCount + castTime - Game.Ping / 2;

                    if (Evade.isDodging)
                    {
                        SpellDetector_OnProcessDetectedSpells(); //reprocess
                    }
                }
            }


        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                ObjectCache.myHeroCache.UpdateInfo();
                CheckHeroInDanger();

                if (isChanneling && channelPosition.Distance(ObjectCache.myHeroCache.serverPos2D) > 50
                    && !myHero.IsChannelingImportantSpell())
                {
                    isChanneling = false;
                }
                var limitDelay = ObjectCache.menuCache.cache["TickLimiter"].Cast<Slider>().CurrentValue; //Tick limiter                
                if (EvadeHelper.fastEvadeMode || EvadeUtils.TickCount - lastTickCount > limitDelay && EvadeUtils.TickCount > lastStopEvadeTime)
                {
                    DodgeSkillShots(); //walking           
                    ContinueLastBlockedCommand();
                    lastTickCount = EvadeUtils.TickCount;
                }

                EvadeSpell.UseEvadeSpell(); //using spells
                CheckDodgeOnlyDangerous();
                RecalculatePath();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RecalculatePath()
        {
            if (ObjectCache.menuCache.cache["RecalculatePosition"].Cast<CheckBox>().CurrentValue && isDodging)//recheck path
            {
                if (lastPosInfo != null && !lastPosInfo.recalculatedPath)
                {
                    var path = myHero.Path;
                    if (path.Length > 0)
                    {
                        var movePos = path.Last().To2D();

                        if (movePos.Distance(lastPosInfo.position) < 5) //more strict checking
                        {
                            var posInfo = EvadeHelper.CanHeroWalkToPos(movePos, ObjectCache.myHeroCache.moveSpeed, 0, 0, false);
                            if (posInfo.posDangerCount > lastPosInfo.posDangerCount)
                            {
                                lastPosInfo.recalculatedPath = true;

                                if (EvadeSpell.PreferEvadeSpell())
                                {
                                    lastPosInfo = PositionInfo.SetAllUndodgeable();
                                }
                                else
                                {
                                    var newPosInfo = EvadeHelper.GetBestPosition();
                                    if (newPosInfo.posDangerCount < posInfo.posDangerCount)
                                    {
                                        lastPosInfo = newPosInfo;
                                        CheckHeroInDanger();
                                        DodgeSkillShots();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ContinueLastBlockedCommand()
        {
            if (ObjectCache.menuCache.cache["ContinueMovement"].Cast<CheckBox>().CurrentValue
                && Situation.ShouldDodge())
            {
                var movePos = lastBlockedUserMoveTo.targetPosition;
                var extraDelay = ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;

                if (isDodging == false && lastBlockedUserMoveTo.isProcessed == false
                    && EvadeUtils.TickCount - lastEvadeCommand.timestamp > ObjectCache.gamePing + extraDelay
                    && EvadeUtils.TickCount - lastBlockedUserMoveTo.timestamp < 1500)
                {
                    movePos = movePos + (movePos - ObjectCache.myHeroCache.serverPos2D).LSNormalized()
                        * EvadeUtils.random.NextFloat(1, 65);

                    if (!EvadeHelper.CheckMovePath(movePos, ObjectCache.gamePing + extraDelay))
                    {
                        //Console.WriteLine("Continue Movement");
                        //myHero.IssueOrder(GameObjectOrder.MoveTo, movePos.To3D());
                        EvadeCommand.MoveTo(movePos);
                        lastBlockedUserMoveTo.isProcessed = true;
                    }
                }
            }
        }

        private void CheckHeroInDanger()
        {
            bool playerInDanger = false;
            foreach (KeyValuePair<int, Spell> entry in SpellDetector.spells)
            {
                Spell spell = entry.Value;

                if (lastPosInfo != null && lastPosInfo.dodgeableSpells.Contains(spell.spellID))
                {
                    if (myHero.ServerPosition.LSTo2D().InSkillShot(spell, ObjectCache.myHeroCache.boundingRadius))
                    {
                        playerInDanger = true;
                        break;
                    }

                    if (ObjectCache.menuCache.cache["EnableEvadeDistance"].Cast<CheckBox>().CurrentValue &&
                        EvadeUtils.TickCount < lastPosInfo.endTime)
                    {
                        playerInDanger = true;
                        break;
                    }
                }
            }

            if (isDodging && !playerInDanger)
            {
                lastDodgingEndTime = EvadeUtils.TickCount;
            }

            if (isDodging == false && !Situation.ShouldDodge())
                return;

            isDodging = playerInDanger;
        }

        private void DodgeSkillShots()
        {
            if (!Situation.ShouldDodge())
            {
                isDodging = false;
                return;
            }

            /*
            if (isDodging && playerInDanger == false) //serverpos test
            {
                myHero.IssueOrder(GameObjectOrder.HoldPosition, myHero, false);
            }*/

            if (isDodging)
            {

                if (lastPosInfo != null)
                {

                    /*foreach (KeyValuePair<int, Spell> entry in SpellDetector.spells)
                    {
                        Spell spell = entry.Value;
                        Console.WriteLine("" + (int)(TickCount-spell.startTime));
                    }*/


                    Vector2 lastBestPosition = lastPosInfo.position;

                    if (ObjectCache.menuCache.cache["ClickOnlyOnce"].Cast<CheckBox>().CurrentValue == false
                        || !(myHero.Path.Count() > 0 && lastPosInfo.position.Distance(myHero.Path.Last().To2D()) < 5))
                    //|| lastPosInfo.timestamp > lastEvadeOrderTime)
                    {
                        EvadeCommand.MoveTo(lastBestPosition);
                        lastEvadeOrderTime = EvadeUtils.TickCount;
                    }
                }
            }
            else //if not dodging
            {
                //Check if hero will walk into a skillshot
                var path = myHero.Path;
                if (path.Length > 0)
                {
                    var movePos = path[path.Length - 1].To2D();

                    if (EvadeHelper.CheckMovePath(movePos))
                    {
                        /*if (ObjectCache.menuCache.cache["AllowCrossing"].Cast<CheckBox>().CurrentValue)
                        {
                            var extraDelayBuffer = ObjectCache.menuCache.cache["ExtraPingBuffer"]
                                .Cast<Slider>().CurrentValue + 30;
                            var extraDist = ObjectCache.menuCache.cache["ExtraCPADistance"]
                                .Cast<Slider>().CurrentValue + 10;
                            var tPosInfo = EvadeHelper.CanHeroWalkToPos(movePos, ObjectCache.myHeroCache.moveSpeed, extraDelayBuffer + ObjectCache.gamePing, extraDist);
                            if (tPosInfo.posDangerLevel == 0)
                            {
                                lastPosInfo = tPosInfo;
                                return;
                            }
                        }*/

                        var posInfo = EvadeHelper.GetBestPositionMovementBlock(movePos);
                        if (posInfo != null)
                        {
                            EvadeCommand.MoveTo(posInfo.position);
                        }
                        return;
                    }
                }
            }
        }
        public void CheckLastMoveTo()
        {
            if (EvadeHelper.fastEvadeMode || ObjectCache.menuCache.cache["FastMovementBlock"].Cast<CheckBox>().CurrentValue)
            {
                if (isDodging == false && lastIssueOrderArgs != null
                && lastIssueOrderArgs.Order == GameObjectOrder.MoveTo
                && Game.Time * 1000 - lastIssueOrderGameTime < 500)
                {
                    Game_OnIssueOrder(myHero, lastIssueOrderArgs);
                    lastIssueOrderArgs = null;
                }
            }
        }

        public static bool isDodgeDangerousEnabled()
        {
            if (ObjectCache.menuCache.cache["DodgeDangerous"].Cast<CheckBox>().CurrentValue == true)
            {
                return true;
            }

            if (ObjectCache.menuCache.cache["DodgeDangerousKeyEnabled"].Cast<CheckBox>().CurrentValue == true)
            {
                if (ObjectCache.menuCache.cache["DodgeDangerousKey"].Cast<KeyBind>().CurrentValue == true || ObjectCache.menuCache.cache["DodgeDangerousKey2"].Cast<KeyBind>().CurrentValue == true)
                    return true;
            }

            return false;
        }

        public static void CheckDodgeOnlyDangerous() //Dodge only dangerous event
        {
            bool bDodgeOnlyDangerous = isDodgeDangerousEnabled();

            if (dodgeOnlyDangerous == false && bDodgeOnlyDangerous)
            {
                spellDetector.RemoveNonDangerousSpells();
                dodgeOnlyDangerous = true;
            }
            else
            {
                dodgeOnlyDangerous = bDodgeOnlyDangerous;
            }
        }

        public static void SetAllUndodgeable()
        {
            lastPosInfo = PositionInfo.SetAllUndodgeable();
        }

        private void SpellDetector_OnProcessDetectedSpells()
        {
            ObjectCache.myHeroCache.UpdateInfo();

            if (ObjectCache.menuCache.cache["DodgeSkillShots"].Cast<KeyBind>().CurrentValue == false)
            {
                lastPosInfo = PositionInfo.SetAllUndodgeable();
                EvadeSpell.UseEvadeSpell();
                return;
            }

            if (ObjectCache.myHeroCache.serverPos2D.CheckDangerousPos(0)
                || ObjectCache.myHeroCache.serverPos2DExtra.CheckDangerousPos(0))
            {
                if (EvadeSpell.PreferEvadeSpell())
                {
                    lastPosInfo = PositionInfo.SetAllUndodgeable();
                }
                else
                {

                    var posInfo = EvadeHelper.GetBestPosition();

                    var calculationTimer = EvadeUtils.TickCount;
                    var caculationTime = EvadeUtils.TickCount - calculationTimer;

                    //computing time
                    /*if (numCalculationTime > 0)
                    {
                        sumCalculationTime += caculationTime;
                        avgCalculationTime = sumCalculationTime / numCalculationTime;
                    }
                    numCalculationTime += 1;*/

                    //Console.WriteLine("CalculationTime: " + caculationTime);

                    /*if (EvadeHelper.GetHighestDetectedSpellID() > EvadeHelper.GetHighestSpellID(posInfo))
                    {
                        return;
                    }*/
                    if (posInfo != null)
                    {
                        lastPosInfo = posInfo.CompareLastMovePos();

                        var travelTime = ObjectCache.myHeroCache.serverPos2DPing.Distance(lastPosInfo.position) / myHero.MoveSpeed;

                        lastPosInfo.endTime = EvadeUtils.TickCount + travelTime * 1000 - 100;
                    }

                    CheckHeroInDanger();
                    DodgeSkillShots(); //walking
                    CheckLastMoveTo();
                    EvadeSpell.UseEvadeSpell(); //using spells
                }
            }
            else
            {
                lastPosInfo = PositionInfo.SetAllDodgeable();
                CheckLastMoveTo();
            }


            //Console.WriteLine("SkillsDodged: " + lastPosInfo.dodgeableSpells.Count + " DangerLevel: " + lastPosInfo.undodgeableSpells.Count);            
        }
    }
}
