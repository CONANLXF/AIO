using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using SharpDX.Direct3D9;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

 namespace ThreshWarden {
	class ThreshWarden {

		public static AIHeroClient Player = ObjectManager.Player;
		public static LeagueSharp.Common.Spell Q, W, E, R;
		public static List<Vector3> MobList = new List<Vector3>();
		public static Obj_AI_Base QTarget = null;
		public static Menu Config;
//		public static Orbwalking.Orbwalker Orbwalker;
		public static List<AIHeroClient> Qignored = new List<AIHeroClient>();

		public static void OnLoad()
        {
			if (Player.ChampionName != "Thresh") return;

			LoadSpell();
			LoadMenu();

			//font = new Font(Drawing.Direct3DDevice,new FontDescription { FaceName = "微软雅黑", Height = 30 });

			Game.OnUpdate += Game_OnUpdate;
			Drawing.OnDraw += Drawing_OnDraw;
			CustomEvents.Unit.OnDash += Unit_OnDash;
			AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
			Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
			Spellbook.OnCastSpell += Spellbook_OnCastSpell;
			Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
			Game.OnWndProc += Game_OnWndProc;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }
        public static void LoadSpell()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1075);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 950);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 450);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 430);

            Q.SetSkillshot(0.5f, 80, 1900f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 100, float.MaxValue, false, SkillshotType.SkillshotLine);
        }
        private static void LoadMenu()
        {

            Config = MainMenu.AddMenu("Thresh", "thresh_menu");

            SpellConfig = Config.AddSubMenu("Spell Settings", "SpellSettings");
            SpellConfig.Add("autoQ", new CheckBox("Auto Q Dash Enemy", true));
            SpellConfig.Add("dontQ2", new CheckBox("Don't Auto Q2", true));
            SpellConfig.Add("dQ2if", new Slider("Don't Q2 if Enemies > allies", 1, 0, 5));
            SpellConfig.Add("farmE", new CheckBox("Farm with E", true));
            SpellConfig.AddLabel("Q BlackList :");
            foreach (var hero in HeroManager.Enemies)
            {
                SpellConfig.Add("QList" + hero.NetworkId, new CheckBox("BlackList: " + hero.ChampionName, false));
            }

            SpellConfig.Add("FlayPush", new KeyBind("Flay Push Key", false, KeyBind.BindTypes.HoldActive, 'T'));
            SpellConfig.Add("FlayPull", new KeyBind("Flay Pull Key", false, KeyBind.BindTypes.HoldActive, 'H'));

            FleeConfig = Config.AddSubMenu("Flee Settings", "FleeSettings");
            FleeConfig.Add("flee", new KeyBind("Flee", false, KeyBind.BindTypes.HoldActive, 'Z'));
            FleeConfig.Add("autoEpush", new CheckBox("Auto E push", true));

            PredictConfig = Config.AddSubMenu("Predict Settings", "PredictSettings");
            PredictConfig.Add("PredictionMode", new ComboBox("Prediction Mode", 0, "Common", "OKTW", "SDK", "SPrediction"));            
            PredictConfig.Add("HitChance", new ComboBox("Hit Chance", 0, "Very High", "High", "Medium"));

            BoxConfig = Config.AddSubMenu("Box Settings", "BoxSettings");
            BoxConfig.Add("BoxCount", new Slider("Box Count", 2, 1, 6));
            BoxConfig.Add("BoxMode", new ComboBox("Box Mode", 0, "Prediction", "Now"));

            SupportConfig = Config.AddSubMenu("Support Mode", "SupportMode");
            SupportConfig.Add("SupportMode", new CheckBox("Suppor tMode", true));
            SupportConfig.Add("SupportModeRange", new Slider("Support Mode Range", (int)Player.AttackRange + 200, (int)Player.AttackRange, 2000));
            SupportConfig.Add("AttackADC", new CheckBox("Attack ADC's Target [TEST]", true));


            DrawConfig = Config.AddSubMenu("Drawing Settings", "DrawingSettings");
            DrawConfig.Add("Drawwhenskillisready", new CheckBox("Draw when skill is ready", true));
            DrawConfig.Add("drawQ", new CheckBox("Draw Q Range", true));
            DrawConfig.Add("drawW", new CheckBox("Draw W Range", true));
            DrawConfig.Add("drawE", new CheckBox("Draw E Range", true));
            DrawConfig.Add("drawR", new CheckBox("Draw R Range", true));
            DrawConfig.Add("drawtg", new CheckBox("Draw Target", true));

            SmartKeyConfig = Config.AddSubMenu("Smart Cast", "SmartCast");
            SmartKeyConfig.Add("EnableFollow", new CheckBox("Enable Follow Options,Prss Q/W/E Auto Cast Spell", true));
            SmartKeyConfig.Add("SmartCastQ", new CheckBox("Smart Cast Q", true));
            SmartKeyConfig.Add("SmartCastW", new CheckBox("Smart Cast W", true));
            SmartKeyConfig.Add("SmartCastE", new CheckBox("Smart Cast E", true));

            TowerConfig = Config.AddSubMenu("Turret Settings", "TurretSettings");
            TowerConfig.Add("QEallyTurrettarget", new CheckBox("Q/E ally Turret’s target", true));
            TowerConfig.Add("QEtargetintoallyturret", new CheckBox("Q/E target into ally turret", true));
            TowerConfig.Add("DontQ2inenemyturret", new CheckBox("Don't Q2 in enemy turret", true));
        }
        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {

               if (getCheckBoxItem(SupportConfig, "SupportMode")
                && GetAdc(getSliderItem(SupportConfig, "SupportModeRange")) != null
				&& (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)))
			{
				args.Process = false;
			}
		}
        

        private static void Game_OnWndProc(WndEventArgs args) {
			if (args.Msg == 'Q')
			{
				var Qtarget = Q.GetTarget(0, Qignored);
				if (Qtarget!=null && SpellQ.GetState() == QState.ThreshQ)
				{
					SpellQ.CastQ1(Qtarget);
				}
				else
				{
					args.Process = false;
				}
			}
			if (args.Msg == 'W')
			{
				var FurthestAlly = GetFurthestAlly();
				if (FurthestAlly != null)
				{
					W.Cast(LeagueSharp.Common.Prediction.GetPrediction(FurthestAlly, W.Delay).CastPosition);
				}

			}
			if (args.Msg == 'E')
			{
				var Etarget = E.GetTarget();
				if (Etarget!=null)
				{
					ELogic(Etarget);
				}
				else
				{
					args.Process = false;
				}
			}
		}

		private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

			#region 自动QE塔下敌人
			if (getCheckBoxItem(TowerConfig, "QEallyTurrettarget") && sender.IsAlly && sender is Obj_AI_Turret && args.Target.IsEnemy && args.Target.Type == GameObjectType.AIHeroClient)
			{
				var target = args.Target as AIHeroClient;
				var turret = sender as Obj_AI_Turret;

				if (turret.IsAlly && E.CanCast(target) && target.LSDistance(turret) < turret.AttackRange + E.Range)
				{
					if (target.LSDistance(turret) < Player.LSDistance(turret))
					{
						E.Cast(target);
					}
					else
					{
						E.CastToReverse(target);
					}
				}
				if (Player.LSDistance(turret) < turret.AttackRange && SpellQ.GetState() == QState.ThreshQ)
				{
					SpellQ.CastQ1(target);
				}
			}
			#endregion

			#region 自动W
			if (!W.IsReady() || !sender.IsEnemy || !sender.LSIsValidTarget(1500))
				return;
			double value = 20 + (Player.Level * 20) + (0.4 * Player.FlatMagicDamageMod);

			foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead && Player.LSDistance(ally.ServerPosition) < W.Range + 200))
			{
				double dmg = 0;
				if (args.Target != null && args.Target.NetworkId == ally.NetworkId)
				{
					dmg = dmg + sender.LSGetSpellDamage(ally, args.SData.Name);
				}
				else
				{
					var castArea = ally.LSDistance(args.End) * (args.End - ally.ServerPosition).LSNormalized() + ally.ServerPosition;
					if (castArea.LSDistance(ally.ServerPosition) < ally.BoundingRadius / 2)
						dmg = dmg + sender.LSGetSpellDamage(ally, args.SData.Name);
					else
						continue;
				}

				if (dmg > 0)
				{
					if (dmg > value)
						W.Cast(ally.Position);
					else if (Player.Health - dmg < Player.LSCountEnemiesInRange(700) * Player.Level * 20)
						W.Cast(ally.Position);
					else if (ally.Health - dmg < ally.Level * 10)
						W.Cast(ally.Position);
				}
			}
			#endregion

		}

		private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args) {
			if ((args.Slot == SpellSlot.E||args.Slot== SpellSlot.R) && sender.Owner.LSIsDashing())
			{
				args.Process = false;
			}

			if (getCheckBoxItem(TowerConfig, "DontQ2inenemyturret"))
			{
				if (sender.Owner.IsMe && args.Slot == SpellSlot.Q && SpellQ.GetState()== QState.threshqleap && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
				{
					if (QTarget.UnderTurret(true) || QTarget.InFountain())
					{
						args.Process = false;
					}
					var tower = QTarget.GetMostCloseTower();
					if ((tower != null && QTarget.IsInTurret(tower) && tower.IsEnemy) || (QTarget.Type == GameObjectType.AIHeroClient && ((AIHeroClient)QTarget).InFountain()))
					{
						args.Process = false;
					}
				}
			}
		}

		private static void Game_OnUpdate(EventArgs args) {
            #region 设置Q到的目标
            var Etarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (QTarget == null || QTarget.IsValid || QTarget.IsDead)
			{
				foreach (var unit in ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsEnemy && o.LSDistance(Player) < Q.Range + 100))
				{
					if (unit.HasBuff("ThreshQ"))
					{
						QTarget = unit;
						break;
					}
					else
					{
						QTarget = null;
					}
				}
			}
			#endregion

			Flee();

			AutoPushTower();

			AutoBox();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                LaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Combo();

            if (getKeyBindItem(SpellConfig, "FlayPush") || getKeyBindItem(SpellConfig, "FlayPull"))
            {
                Orbwalker.MoveTo(Game.CursorPos);
            }

            if (getKeyBindItem(SpellConfig, "FlayPush") && Etarget != null &&
                E.IsReady())
            {
                Push(Etarget);
            }

            if (getKeyBindItem(SpellConfig, "FlayPull") && Etarget != null &&
                E.IsReady())
            {
                Pull(Etarget);
            }
        }

		private static void Flee()
        {
			if (getKeyBindItem(FleeConfig,"flee"))
			{
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

				if (getKeyBindItem(FleeConfig, "autoEpush"))
				{
					foreach (var enemy in HeroManager.Enemies.Where(e => !e.IsDead && !e.HasBuffOfType(BuffType.SpellShield)))
					{
						if (E.CanCast(enemy))
						{
							E.Cast(enemy);
						}
					}
				}
			}
		}


        private static void Pull(Obj_AI_Base target)
        {
            var pos = target.Position.LSExtend(Player.Position, Player.LSDistance(target.Position) + 200);
            E.Cast(pos);
        }

        private static void Push(Obj_AI_Base target)
        {
            var pos = target.Position.LSExtend(Player.Position, Player.LSDistance(target.Position) - 200);
            E.Cast(pos);
        }

        private static void AutoBox() {

            var AutoBoxCount = getSliderItem(BoxConfig, "BoxCount");
            var EnemiesCount = getBoxItem(BoxConfig, "BoxMode") == 0
				? Player.CountEnemiesInRangeDeley(R.Range, R.Delay - 0.1f)
				: Player.LSCountEnemiesInRange(R.Range);

			if (R.IsReady() && EnemiesCount >= AutoBoxCount)
			{
				R.Cast();
			}
		}
        private static Obj_AI_Base Marked;
        private static void Obj_AI_Base_OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsEnemy && args.Buff.Name == "ThreshQ")
            {
                Marked = null;
            }
        }

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsEnemy && args.Buff.Name == "ThreshQ")
            {
                Marked = sender;
            }
        }

        private static void Combo()
        {
       //     var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            var QT = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (QT != null && getCheckBoxItem(SpellConfig, "QList" + QT.NetworkId))
            {
                return;
            }


            if (QT != null && QT.IsValid)
			{
				ELogic(QT);

                if (QT.HasBuffOfType(BuffType.Stun) || QT.HasBuffOfType(BuffType.Knockup) || QT.HasBuffOfType(BuffType.Fear) || QT.HasBuffOfType(BuffType.Slow))
                    SpellQ.CastQ1(QT);
                //Q2逻辑
                if (SpellQ.GetState() == QState.threshqleap && QTarget.Position.LSCountEnemiesInRange(700) - Player.Position.LSCountEnemiesInRange(700) <= getSliderItem(SpellConfig, "dQ2if"))
				{
                    
					SpellQ.CastQ2();
				}

				//Q1逻辑
				if (!E.IsInRange(QT) && SpellQ.GetState() == QState.ThreshQ)
				{
					SpellQ.CastQ1(QT);
                    
                }

				if (SpellQ.GetState() == QState.threshqleap)
				{
					//W拉最远队友
					var FurthestAlly = GetFurthestAlly();
					if (FurthestAlly != null)
					{
						W.Cast(LeagueSharp.Common.Prediction.GetPrediction(FurthestAlly, W.Delay).CastPosition);
					}
				}
            }
		}

 
        private static void ELogic(AIHeroClient target)
        {
			if (!E.CanCast(target) || target.HasBuffOfType(BuffType.SpellShield)) return;
			
			var tower = target.GetMostCloseTower();
			if (tower != null && tower.IsAlly && E.CanCast(target) && target.LSDistance(tower)<tower.AttackRange + E.Range)
			{
				if (target.LSDistance(tower) < Player.LSDistance(tower))
				{
					E.Cast(target);
               //     Chat.Print("E1");
                }
				else
				{
					E.CastToReverse(target);
               //     Chat.Print("E2");
                }
			}

			var adc = GetAdc();
			if (adc!=null)
			{
				if (target.IsFleeing(Player))
				{
					E.Cast(target);
                //    Chat.Print("E3");
                }
				else if (target.IsHunting(Player))
				{
					E.CastToReverse(target);
                //    Chat.Print("E4");
                }
			}
			if (target.LSDistance(Player)>E.Range/2 || Player.HealthPercent<50)
			{
				E.CastToReverse(target);
            //    Chat.Print("E5");
			}
			
		}

		private static AIHeroClient GetFurthestAlly() {
			AIHeroClient FurthestAlly = null;
			foreach (var ally in HeroManager.Allies.Where(a => a.LSDistance(Player)> W.Range/2 + 100 &&  a.LSDistance(Player) < W.Range + 100 && !a.IsDead && !a.IsMe))
			{
				if (FurthestAlly == null)
				{
					FurthestAlly = ally;
				}
				else if (FurthestAlly != null && Player.LSDistance(ally) > Player.LSDistance(FurthestAlly))
				{
					FurthestAlly = ally;
				}
			}
			return FurthestAlly;
		}

		private static void LaneClear()
        {
            if (getCheckBoxItem(SpellConfig, "farmE"))
            {
                if (E.IsReady() && Player.Mana > Q.ManaCost + W.ManaCost + E.ManaCost + R.ManaCost)
                {
                    var minions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
                    var Efarm = Q.GetLineFarmLocation(minions, E.Width);
                    if (Efarm.MinionsHit >= 3)
                    {
                        E.Cast(Efarm.Position);
                    }
                }
            }
		}
		private static void AutoPushTower()
        {
			var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

			if (target == null) return;

			var tower = target.GetMostCloseTower();
			if (tower != null && tower.IsAlly)
			{
				if (Player.IsInTurret(tower) && target.LSDistance(tower) < Q.Range / 2 && SpellQ.GetState()== QState.ThreshQ)
				{
					SpellQ.CastQ1(target);
				}

				if (tower != null && tower.IsAlly && E.CanCast(target) && target.LSDistance(tower) < tower.AttackRange + E.Range)
				{
					if (target.LSDistance(tower) < Player.LSDistance(tower))
					{
						E.Cast(target);
					}
					else
					{
						E.CastToReverse(target);
					}
				}
			}
		}

		private static Obj_AI_Base GetAdc(float range = 1075)
        {
			Obj_AI_Base Adc = null;
			foreach (var ally in HeroManager.Allies.Where(a =>!a.IsMe && !a.IsDead))
			{
				if (Adc == null)
				{
					Adc = ally;
				}
				else if (Adc.TotalAttackDamage < ally.TotalAttackDamage)
				{
					Adc = ally;
				}
			}
			return Adc;
		}

       


        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {

			if (sender.IsEnemy && Player.LSDistance(args.EndPos) > Player.LSDistance(args.StartPos))
			{
				if (E.IsInRange(args.StartPos))
				{
					E.Cast(sender);
				}

				if (getCheckBoxItem(SpellConfig, "autoQ") && SpellQ.GetState() == QState.ThreshQ && Q.IsInRange(args.EndPos)&& !E.IsInRange(args.EndPos) && Math.Abs(args.Duration - args.EndPos.LSDistance(sender) / Q.Speed * 1000) < 150)
				{
					List<Vector2> to = new List<Vector2>();
					to.Add(args.EndPos);
					var QCollision = Q.GetCollision(Player.Position.LSTo2D(), to);
					if (QCollision == null || QCollision.Count == 0 || QCollision.All(a => !a.IsMinion))
					{
						if (Q.Cast(args.EndPos))
						{
							return;
						}
					}
				}
			}
		}

		private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser) {
			if (gapcloser.Sender.ChampionName == "MasterYi" && gapcloser.Slot == SpellSlot.Q)
			{
				return;
			}
			
            if (E.CanCast(gapcloser.Sender) && E.CastToReverse(gapcloser.Sender))
			{
				return;
			}
			else if (Q.CanCast(gapcloser.Sender) && SpellQ.GetState()== QState.ThreshQ)
			{
				if (gapcloser.Sender.ChampionName == "JarvanIV" && gapcloser.Slot == SpellSlot.Q)
				{
					return;
				}
				SpellQ.CastQ1(gapcloser.Sender);
			}
		}

		private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args) {

			if (E.CanCast(sender))
			{
				if (Player.CountAlliesInRange(E.Range + 50) < sender.CountAlliesInRange(E.Range + 50))
				{
					E.Cast(sender);
				}
				else
				{
					E.CastToReverse(sender);
				}
			}
			if (Q.CanCast(sender) && SpellQ.GetState()== QState.ThreshQ)
			{
				Q.Cast(sender);
			}

		}
		
        public static Menu SpellConfig, FleeConfig, PredictConfig, BoxConfig, SupportConfig, DrawConfig, SmartKeyConfig, TowerConfig;

        

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
        private static void Drawing_OnDraw(EventArgs args)
        {
            #region 技能范围
            var QShow = getCheckBoxItem(DrawConfig, "drawQ");
            if (QShow)
            {
                if (getCheckBoxItem(DrawConfig, "Drawwhenskillisready"))
                {
                    if (Q.IsReady())
                    {
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                    }

                }
                else
                {
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
            }

            var WShow = getCheckBoxItem(DrawConfig, "drawW");
            if (WShow)
            {
                if (getCheckBoxItem(DrawConfig, "Drawwhenskillisready"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                {
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Cyan, 1, 1);

                }
            }

            var EShow = getCheckBoxItem(DrawConfig, "drawE");
            if (EShow)
            {
                if (getCheckBoxItem(DrawConfig, "Drawwhenskillisready"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                {
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Cyan, 1, 1);
                }
            }

            var RShow = getCheckBoxItem(DrawConfig, "drawR");
            if (RShow)
            {
                if (getCheckBoxItem(DrawConfig, "Drawwhenskillisready"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Cyan, 1, 1);
                  
                }
                else
                {
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Cyan, 1, 1);
                  
                }
            }
            #endregion
        }
    }
}
