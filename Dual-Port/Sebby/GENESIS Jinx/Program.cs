using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

 namespace Jinx_Genesis
{
    class Program
    {
        private static string ChampionName = "Jinx";

        public static Menu Config;
        public static Menu qMenu, wMenu, eMenu, rMenu, harassMenu, manaMenu, drawMenu, predMenu;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Spell Q, W, E, R;
        private static float WMANA, EMANA ,RMANA;
        private static bool FishBoneActive= false, Combo = false, Farm = false;
        private static AIHeroClient blitz = null;
        private static float WCastTime = Game.Time;

        private static string[] Spells =
        {
            "katarinar","drain","consume","absolutezero", "staticfield","reapthewhirlwind","jinxw","jinxr","shenstandunited","threshe","threshrpenta","threshq","meditate","caitlynpiltoverpeacemaker", "volibearqattack",
            "cassiopeiapetrifyinggaze","ezrealtrueshotbarrage","galioidolofdurand","luxmalicecannon", "missfortunebullettime","infiniteduress","alzaharnethergrasp","lucianq","velkozr","rocketgrabmissile"
        };

        private static List<AIHeroClient> Enemies = new List<AIHeroClient>();

        /// <summary>
        /// Calculates the Damage done with R - KARMAPANDA
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>Returns the Damage done with useR</returns>
        private static float RDamage(Obj_AI_Base target)
        {
            var distance = ObjectManager.Player.Distance(target);
            var increment = Math.Floor(distance / 100f);

            if (increment > 15)
            {
                increment = 15;
            }

            var extraPercent = Math.Floor((10f + (increment * 6f))) / 10f;

            if (extraPercent > 10)
            {
                extraPercent = 10;
            }

            var damage = (new[] { 0f, 25f, 35f, 45f }[R.Level] * (extraPercent)) +
                         ((extraPercent / 100f) * ObjectManager.Player.FlatPhysicalDamageMod) +
                         ((new[] { 0f, 0.25f, 0.3f, 0.35f }[R.Level] * (target.MaxHealth - target.Health)));

            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical, (float)damage);
        }

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != ChampionName) return;

            LoadMenu();
            Q = new Spell(SpellSlot.Q, Player.AttackRange);
            W = new Spell(SpellSlot.W, 1490f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 2500f);

            W.SetSkillshot(0.6f, 75f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 1f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.IsEnemy)
                {
                    Enemies.Add(hero);
                }
                else if(hero.ChampionName.Equals("Blitzcrank"))
                {
                    blitz = hero;
                }
            }

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPreAttack += BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Chat.Print("<font color=\"#00BFFF\">GENESIS </font>Jinx<font color=\"#000000\"> by Sebby </font> - <font color=\"#FFFFFF\">Loaded</font>");
        }
       
        private static void LoadMenu()
        {
            Config = MainMenu.AddMenu(ChampionName + "GENESIS", ChampionName + "GENESIS");

            drawMenu = Config.AddSubMenu("Draw", "Draw");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));


            qMenu = Config.AddSubMenu("Q Config", "Q");
            qMenu.Add("Qcombo", new CheckBox("Combo Q", true));
            qMenu.Add("Qharass", new CheckBox("Harass Q", true));
            qMenu.Add("farmQout", new CheckBox("Farm Q out range AA minion", true));
            qMenu.Add("Qlaneclear", new Slider("Lane clear x minions", 4, 10, 2));
            qMenu.Add("Qchange", new ComboBox("Q change mode FishBone -> MiniGun", 1, "Real Time", "Before AA"));
            qMenu.Add("Qaoe", new Slider("Force FishBone if can hit x target", 3, 5, 0));
            qMenu.Add("QmanaIgnore", new Slider("Ignore mana if can kill in x AA", 4, 10, 0));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                qMenu.Add("harasQ" + enemy.ChampionName, new CheckBox("Harass Q enemy: " + enemy.ChampionName));



            wMenu = Config.AddSubMenu("W Config", "W");
            wMenu.Add("Wcombo", new CheckBox("Combo W", true));
            wMenu.Add("Wharass", new CheckBox("harass W", true));
            wMenu.Add("Wks", new CheckBox("KS W", true));
            wMenu.Add("Wts", new ComboBox("Harass mode", 0, "Target selector", "All in range"));
            wMenu.Add("Wmode", new ComboBox("W mode", 0, "Out range MiniGun", "Out range FishBone", "Custome range"));
            wMenu.Add("Wcustome", new Slider("Custome minimum range", 600, 1500, 0));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                wMenu.Add("haras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));


            eMenu = Config.AddSubMenu("E Config", "E");
            eMenu.Add("Ecombo", new CheckBox("Combo E", true));
            eMenu.Add("Etel", new CheckBox("E on enemy teleport", true));
            eMenu.Add("Ecc", new CheckBox("E on CC", true));
            eMenu.Add("Eslow", new CheckBox("E on slow", true));
            eMenu.Add("Edash", new CheckBox("E on dash", true));
            eMenu.Add("Espell", new CheckBox("E on special spell detection", true));
            eMenu.Add("Eaoe", new Slider("E if can catch x enemies", 3, 5, 0));
            eMenu.Add("EmodeGC", new ComboBox("Gap Closer position mode", 0, "Dash end position", "Jinx position"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                eMenu.Add("EGCchampion" + enemy.ChampionName, new CheckBox("Cast on enemy:" + enemy.ChampionName));


            rMenu = Config.AddSubMenu("R Config", "R");
            rMenu.Add("Rks", new CheckBox("R KS", true));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("semiMode", new ComboBox("Semi-manual cast mode", 0, "Low hp target", "AOE"));
            rMenu.Add("Rmode", new ComboBox("R mode", 0, "Out range MiniGun ", "Out range FishBone ", "Custome range"));
            rMenu.Add("Rcustome", new Slider("Custome minimum range", 1000, 1600, 0));
            rMenu.Add("RcustomeMax", new Slider("Max range", 3000, 10000, 0));
            rMenu.Add("Raoe", new Slider("R if can hit x target and can kill", 2, 5, 0));
            rMenu.AddLabel("Overkill Protection");
            rMenu.Add("Rover", new Slider("Don't R if allies near target in x range", 500, 1000, 0));
            rMenu.Add("RoverAA", new CheckBox("Don't R if Jinx winding up", true));
            rMenu.Add("RoverW", new CheckBox("Don't R if can W KS", true));


            manaMenu = Config.AddSubMenu("Mana Config", "Mana");
            manaMenu.Add("QmanaCombo", new Slider("Q combo mana", 20, 100, 0));
            manaMenu.Add("QmanaHarass", new Slider("Q harass mana", 40, 100, 0));
            manaMenu.Add("QmanaLC", new Slider("Q lane clear mana", 80, 100, 0));
            manaMenu.Add("WmanaCombo", new Slider("W combo mana", 20, 100, 0));
            manaMenu.Add("WmanaHarass", new Slider("W harass mana", 40, 100, 0));
            manaMenu.Add("EmanaCombo", new Slider("E mana", 20, 100, 0));

      
            predMenu = Config.AddSubMenu("Prediction Config", "Prediction");
            predMenu.Add("PredictionMODE", new ComboBox("Prediction MODE", 1, "Common prediction", "OKTW© PREDICTION"));
            predMenu.Add("Wpred", new ComboBox("W Hit Chance", 0, "VeryHigh W", "High W"));
            predMenu.Add("Epred", new ComboBox("E Hit Chance", 0, "VeryHigh E", "High E"));
            predMenu.Add("Rpred", new ComboBox("R Hit Chance", 0, "VeryHigh R", "High R"));


            harassMenu = Config.AddSubMenu("Harass Config", "Harass");
            harassMenu.Add("LaneClearHarass", new CheckBox("LaneClear Harass", true));
            harassMenu.Add("LastHitHarass", new CheckBox("LastHit Harass", true));
            harassMenu.Add("MixedHarass", new CheckBox("Mixed Harass", true));

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

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.ManaPercent < getSliderItem(manaMenu, "EmanaCombo"))
                return;

            if (E.IsReady())
            {
                var t = gapcloser.Sender;
                if (t.LSIsValidTarget(E.Range) && getCheckBoxItem(eMenu, "EGCchampion" + t.ChampionName))
                {
                    if(getBoxItem(eMenu, "EmodeGC") == 0)
                        E.Cast(gapcloser.End);
                    else
                        E.Cast(Player.ServerPosition);
                }
            }
        }
        
        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!FishBoneActive)
                return;

            if (Q.IsReady() && args.Target is AIHeroClient && getBoxItem(qMenu, "Qchange") == 1)
            {
                var t = (AIHeroClient)args.Target;
                if ( t.LSIsValidTarget())
                {
                    FishBoneToMiniGun(t);
                }
            }

            if (!Combo && args.Target is Obj_AI_Minion)
            {
                var t = (Obj_AI_Minion)args.Target;
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && Player.ManaPercent > getSliderItem(manaMenu, "QmanaLC") && CountMinionsInRange(250, t.Position) >= getSliderItem(qMenu, "Qlaneclear"))
                {
                    
                }
                else if (GetRealDistance(t) < GetRealPowPowRange(t))
                {
                    args.Process = false;
                    if (Q.IsReady())
                        Q.Cast();
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            if (sender.IsMinion)
                return;

            if (sender.IsMe)
            {
                if (args.SData.Name == "JinxWMissile")
                    WCastTime = Game.Time;
            }

            if (!E.IsReady() || !sender.IsEnemy || !getCheckBoxItem(eMenu, "Espell") || Player.ManaPercent < getSliderItem(manaMenu, "EmanaCombo") || !sender.IsValid<AIHeroClient>() || !sender.LSIsValidTarget(E.Range) )
                return;

            var foundSpell = Spells.Find(x => args.SData.Name.ToLower() == x);
            if (foundSpell != null)
            {
                E.Cast(sender.Position);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            SetValues();

            if (Q.IsReady())
                Qlogic();
            if (W.IsReady())
                Wlogic();
            if (E.IsReady())
                Elogic();
            if (R.IsReady())
                Rlogic();
        }

        private static void Rlogic()
        {
            R.Range = getSliderItem(rMenu, "RcustomeMax");

            if (getKeyBindItem(rMenu, "useR"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.LSIsValidTarget() && t.IsVisible && t.IsHPBarRendered && !t.IsDead)
                {
                    if(getBoxItem(rMenu, "semiMode") == 0)
                    {
                        R.Cast(t);
                    }
                    else
                    {
                        R.CastIfWillHit(t, 2);
                        R.Cast(t, true, true);
                    }
                }   
            }

            if (getCheckBoxItem(rMenu, "Rks"))
            {
                bool cast = false;
                

                if (getCheckBoxItem(rMenu, "RoverAA") && (!Orbwalker.CanAutoAttack || Player.Spellbook.IsAutoAttacking))
                    return;

                foreach (var target in Enemies.Where(target => target.LSIsValidTarget(R.Range) && ValidUlt(target) && target.IsVisible && target.IsHPBarRendered))
                {
                    
                    float predictedHealth = target.Health + target.HPRegenRate * 2;

                    var Rdmg = RDamage(target);
                    if(Player.LSDistance(target.Position) < 1500)
                    {

                        Rdmg = Rdmg * (Player.LSDistance(target.Position) / 1500);
                       
                    }

                    if (Rdmg > predictedHealth)
                    {
                        cast = true;
                        PredictionOutput output = R.GetPrediction(target);
                        Vector2 direction = output.CastPosition.LSTo2D() - Player.Position.LSTo2D();
                        direction.Normalize();

                        foreach (var enemy in Enemies.Where(enemy => enemy.LSIsValidTarget()))
                        {
                            if (enemy.NetworkId == target.NetworkId || !cast)
                                continue;
                            PredictionOutput prediction = R.GetPrediction(enemy);
                            Vector3 predictedPosition = prediction.CastPosition;
                            Vector3 v = output.CastPosition - Player.ServerPosition;
                            Vector3 w = predictedPosition - Player.ServerPosition;
                            double c1 = Vector3.Dot(w, v);
                            double c2 = Vector3.Dot(v, v);
                            double b = c1 / c2;
                            Vector3 pb = Player.ServerPosition + ((float)b * v);
                            float length = Vector3.Distance(predictedPosition, pb);
                            if (length < (R.Width + 150 + enemy.BoundingRadius / 2) && Player.LSDistance(predictedPosition) < Player.LSDistance(target.ServerPosition))
                                cast = false;
                        }

                        if (cast && target.IsVisible && target.IsHPBarRendered && !target.IsDead)
                        {
                            if (getCheckBoxItem(rMenu, "RoverW") && target.LSIsValidTarget(W.Range) && W.GetDamage(target) > target.Health && W.Instance.Cooldown - (W.Instance.CooldownExpires - Game.Time) < 1.1)
                                return;

                            if (target.LSCountEnemiesInRange(400) > getSliderItem(rMenu, "Raoe"))
                                CastSpell(R, target);

                            if (RValidRange(target) && target.CountAlliesInRange(getSliderItem(rMenu, "Rover")) == 0)
                                CastSpell(R, target);
                        }
                    }
                }
            }
        }

        private static bool RValidRange(Obj_AI_Base t)
        {
            var range = GetRealDistance(t);

            if (getBoxItem(rMenu, "Rmode") == 0)
            {
                if (range > GetRealPowPowRange(t))
                    return true;
                else
                    return false;

            }
            else if (getBoxItem(rMenu, "Rmode") == 1)
            {
                if (range > Q.Range)
                    return true;
                else
                    return false;
            }
            else if (getBoxItem(rMenu, "Rmode") == 2)
            {
                if (range > getSliderItem(rMenu, "Rcustome") && !Orbwalking.InAutoAttackRange(t))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private static void Elogic()
        {
            if (Player.ManaPercent < getSliderItem(manaMenu, "EmanaCombo"))
                return;

            if (blitz != null && blitz.Distance(Player.Position) < E.Range)
            {
                foreach (var enemy in Enemies.Where(enemy => enemy.LSIsValidTarget(2000) && enemy.HasBuff("RocketGrab")))
                {
                    E.Cast(blitz.Position.Extend(enemy.Position, 30));
                    return;
                }
            }

            foreach (var enemy in Enemies.Where(enemy => enemy.LSIsValidTarget(E.Range) ))
            {

                E.CastIfWillHit(enemy, getSliderItem(eMenu, "Eaoe"));

                if(getCheckBoxItem(eMenu, "Ecc"))
                {
                    if (!CanMove(enemy))
                        E.Cast(enemy.Position);
                    E.CastIfHitchanceEquals(enemy, HitChance.Immobile);
                }

                if(enemy.MoveSpeed < 250 && getCheckBoxItem(eMenu, "Eslow"))
                    E.Cast(enemy);
                if (getCheckBoxItem(eMenu, "Edash"))
                    E.CastIfHitchanceEquals(enemy, HitChance.Dashing);
            }
            

            if (getCheckBoxItem(eMenu, "Etel"))
            {
                foreach (var Object in ObjectManager.Get<Obj_AI_Base>().Where(Obj => Obj.IsEnemy && Obj.LSDistance(Player.ServerPosition) < E.Range && (Obj.HasBuff("teleport_target") || Obj.HasBuff("Pantheon_GrandSkyfall_Jump"))))
                {
                    E.Cast(Object.Position);
                }
            }

            if (Combo && Player.IsMoving && getCheckBoxItem(eMenu, "Ecombo"))
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (t.LSIsValidTarget(E.Range) && E.GetPrediction(t).CastPosition.Distance(t.Position) > 200)
                {
                    if (Player.Position.Distance(t.ServerPosition) > Player.Position.Distance(t.Position))
                    {
                        if (t.Position.Distance(Player.ServerPosition) < t.Position.Distance(Player.Position))
                            CastSpell(E, t);
                    }
                    else
                    {
                        if (t.Position.Distance(Player.ServerPosition) > t.Position.Distance(Player.Position))
                            CastSpell(E, t);
                    }
                }
            }
        }

        private static bool WValidRange(Obj_AI_Base t)
        {
            var range = GetRealDistance(t);

            if (getBoxItem(wMenu, "Wmode") == 0)
            {
                if (range > GetRealPowPowRange(t) && Player.LSCountEnemiesInRange(GetRealPowPowRange(t)) == 0)
                    return true;
                else
                    return false;

            }
            else if (getBoxItem(wMenu, "Wmode") == 1)
            {
                if (range > Q.Range + 50 && Player.LSCountEnemiesInRange(Q.Range + 50) == 0)
                    return true;
                else
                    return false;
            }
            else if (getBoxItem(wMenu, "Wmode") == 2)
            {
                if(range > getSliderItem(wMenu, "Wcustome") && Player.LSCountEnemiesInRange(getSliderItem(wMenu, "Wcustome")) == 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private static void Wlogic()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.LSIsValidTarget() && WValidRange(t))
            {
                if (getCheckBoxItem(wMenu, "Wks") && GetKsDamage(t, W) > t.Health && ValidUlt(t))
                {
                    CastSpell(W, t);
                }

                if (Combo && getCheckBoxItem(wMenu, "Wcombo") && Player.ManaPercent > getSliderItem(manaMenu, "WmanaCombo"))
                {
                    CastSpell(W, t);
                }
                else if (Farm && Orbwalker.CanAutoAttack && !Player.Spellbook.IsAutoAttacking && getCheckBoxItem(wMenu, "Wharass") && Player.ManaPercent > getSliderItem(manaMenu, "WmanaHarass"))
                {
                    if (getBoxItem(wMenu, "Wts") == 0)
                    {
                        if (getCheckBoxItem(wMenu, "haras" + t.ChampionName))
                            CastSpell(W, t);
                    }
                    else
                    {
                        foreach (var enemy in Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && WValidRange(t) && getCheckBoxItem(wMenu, "haras" + t.ChampionName)))
                            CastSpell(W, enemy);
                    }
                }
                
            }
        }

        private static void Qlogic()
        {
            if (FishBoneActive)
            {
                var orbT = Orbwalker.LastTarget;
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && Player.ManaPercent > getSliderItem(manaMenu, "QmanaLC") && orbT.IsValid<Obj_AI_Minion>())
                {
                    
                }
                else if (getBoxItem(qMenu, "Qchange") == 0 && orbT.IsValid<AIHeroClient>())
                {
                    var t = (AIHeroClient)Orbwalker.LastTarget;
                    FishBoneToMiniGun(t);
                }  
                else
                {
                    if (!Combo && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                        Q.Cast();
                }
            }
            else
            {
                var t = TargetSelector.GetTarget(Q.Range + 40, DamageType.Physical);
                if (t.LSIsValidTarget())
                {
                    if ((!Orbwalking.InAutoAttackRange(t) || t.CountEnemiesInRange(250) >= getSliderItem(qMenu, "Qaoe")))
                    {
                        if (Combo && getCheckBoxItem(qMenu, "Qcombo") && (Player.ManaPercent > getSliderItem(manaMenu, "QmanaCombo") || Player.LSGetAutoAttackDamage(t) * getSliderItem(qMenu, "QmanaIgnore") > t.Health))
                        {
                            Q.Cast();
                        }
                        if (Farm && Orbwalker.CanAutoAttack && !Player.Spellbook.IsAutoAttacking && getCheckBoxItem(qMenu, "harasQ" + t.ChampionName) && getCheckBoxItem(qMenu, "Qharass") && (Player.ManaPercent > getSliderItem(manaMenu, "QmanaHarass") || Player.LSGetAutoAttackDamage(t) * getSliderItem(qMenu, "QmanaIgnore") > t.Health))
                        {
                            Q.Cast();
                        }
                    }
                }
                else
                {
                    if (Combo && Player.ManaPercent > getSliderItem(manaMenu, "QmanaCombo"))
                    {
                        Q.Cast();
                    }
                    else if (Farm && !Player.Spellbook.IsAutoAttacking && getCheckBoxItem(qMenu, "farmQout") && Orbwalker.CanAutoAttack)
                    {
                        foreach (var minion in MinionManager.GetMinions(Q.Range + 30).Where(
                        minion => !Orbwalking.InAutoAttackRange(minion) && minion.Health < Player.LSGetAutoAttackDamage(minion) * 1.2 && GetRealPowPowRange(minion) < GetRealDistance(minion) && Q.Range < GetRealDistance(minion)))
                        {
                            Orbwalker.ForcedTarget =(minion);
                            Q.Cast();
                            return;
                        }
                    }
                    if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && Player.ManaPercent > getSliderItem(manaMenu, "QmanaLC"))
                    {
                        var orbT = Orbwalker.LastTarget;
                        if (orbT.IsValid<Obj_AI_Minion>() && CountMinionsInRange(250, orbT.Position) >= getSliderItem(qMenu, "Qlaneclear"))
                        {
                            Q.Cast();
                        }
                    }
                }
            }
            Orbwalker.ForcedTarget =(null);
        }

        private static int CountMinionsInRange(float range, Vector3 pos)
        {
            var minions = MinionManager.GetMinions(pos, range);
            int count = 0;
            foreach (var minion in minions)
            {
                count++;
            }
            return count;
        }

        public static float GetKsDamage(Obj_AI_Base t, Spell QWER)
        {
            var totalDmg = QWER.GetDamage(t);

            if (Player.HasBuff("summonerexhaust"))
                totalDmg = totalDmg * 0.6f;

            if (t.HasBuff("ferocioushowl"))
                totalDmg = totalDmg * 0.7f;

            if (t is AIHeroClient)
            {
                var champion = (AIHeroClient)t;
                if (champion.ChampionName == "Blitzcrank" && !champion.HasBuff("BlitzcrankManaBarrierCD") && !champion.HasBuff("ManaBarrier"))
                {
                    totalDmg -= champion.Mana / 2f;
                }
            }

            var extraHP = t.Health - HealthPrediction.GetHealthPrediction(t, 500);

            totalDmg += extraHP;
            totalDmg -= t.HPRegenRate;
            totalDmg -= t.PercentLifeStealMod * 0.005f * t.FlatPhysicalDamageMod;

            return totalDmg;
        }

        public static bool ValidUlt(Obj_AI_Base target)
        {
            if (target.HasBuffOfType(BuffType.PhysicalImmunity)
                || target.HasBuffOfType(BuffType.SpellImmunity)
                || target.IsZombie
                || target.IsInvulnerable
                || target.HasBuffOfType(BuffType.Invulnerability)
                || target.HasBuffOfType(BuffType.SpellShield)
                || target.HasBuff("deathdefiedbuff")
                || target.HasBuff("Undying Rage")
                || target.HasBuff("Chrono Shift")
                )
                return false;
            else
                return true;
        }

        private static bool CanMove(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned || target.IsChannelingImportantSpell() || target.MoveSpeed < 50f)
            {
                return false;
            }
            else
                return true;
        }

        private static void CastSpell(Spell QWER, Obj_AI_Base target)
        {
            if (getBoxItem(predMenu, "PredictionMODE") == 0)
            {
                if (QWER.Slot == SpellSlot.W)
                {
                    if (getBoxItem(predMenu, "Wpred") == 0)
                        QWER.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    else
                        QWER.Cast(target);
                }
                if (QWER.Slot == SpellSlot.R)
                {
                    if (getBoxItem(predMenu, "Rpred") == 0)
                        QWER.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    else
                        QWER.Cast(target);
                }
                if (QWER.Slot == SpellSlot.E)
                {
                    if (getBoxItem(predMenu, "Epred") == 0)
                        QWER.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    else
                        QWER.Cast(target);
                }
            }
            else
            {
                Core.SkillshotType CoreType2 = Core.SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (QWER.Type == SkillshotType.SkillshotCircle)
                {
                    CoreType2 = Core.SkillshotType.SkillshotCircle;
                    aoe2 = true;
                }

                var predInput2 = new Core.PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };

                var poutput2 = Core.Prediction.GetPrediction(predInput2);

                if (QWER.Slot == SpellSlot.W)
                {
                    if (getBoxItem(predMenu, "Wpred") == 0)
                    {
                        if (poutput2.Hitchance >= Core.HitChance.VeryHigh)
                            QWER.Cast(poutput2.CastPosition);
                    }
                    else
                    {
                        if (poutput2.Hitchance >= Core.HitChance.High)
                            QWER.Cast(poutput2.CastPosition);
                    }
                }
                if (QWER.Slot == SpellSlot.R)
                {
                    if (getBoxItem(predMenu, "Rpred") == 0)
                    {
                        if (poutput2.Hitchance >= Core.HitChance.VeryHigh)
                            QWER.Cast(poutput2.CastPosition);
                    }
                    else
                    {
                        if (poutput2.Hitchance >= Core.HitChance.High)
                            QWER.Cast(poutput2.CastPosition);
                    }
                }
                if (QWER.Slot == SpellSlot.E)
                {
                    if (getBoxItem(predMenu, "Epred") == 0)
                    {
                        if (poutput2.Hitchance >= Core.HitChance.VeryHigh)
                            QWER.Cast(poutput2.CastPosition);
                    }
                    else
                    {
                        if (poutput2.Hitchance >= Core.HitChance.High)
                            QWER.Cast(poutput2.CastPosition);
                    }
                }
            }
        }

        private static void FishBoneToMiniGun(Obj_AI_Base t)
        {
            var realDistance = GetRealDistance(t);

            if(realDistance < GetRealPowPowRange(t) && t.LSCountEnemiesInRange(250) < getSliderItem(qMenu, "Qaoe"))
            {
                if (Player.ManaPercent < getSliderItem(manaMenu, "QmanaCombo") || Player.LSGetAutoAttackDamage(t) * getSliderItem(qMenu, "QmanaIgnore") < t.Health)
                    Q.Cast();

            }
        }

        private static float GetRealDistance(Obj_AI_Base target) { return Player.ServerPosition.LSDistance(target.ServerPosition) + Player.BoundingRadius + target.BoundingRadius; }

        private static float GetRealPowPowRange(GameObject target) { return 650f + Player.BoundingRadius + target.BoundingRadius; }

        private static void SetValues()
        {
            if (getBoxItem(rMenu, "Rmode") == 2)
                getSliderItem(wMenu, "Wcustome");
            else
                getSliderItem(wMenu, "Wcustome");

            if (getBoxItem(rMenu, "Rmode") == 2)
                getSliderItem(rMenu, "Rcustome");
            else
                getSliderItem(rMenu, "Rcustome");


            if (Player.HasBuff("JinxQ"))
                FishBoneActive = true;
            else
                FishBoneActive = false;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Combo = true;
            else
                Combo = false;

            if (
                (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && getCheckBoxItem(harassMenu, "LaneClearHarass")) ||
                (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && getCheckBoxItem(harassMenu, "LastHitHarass")) || 
                (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && getCheckBoxItem(harassMenu, "MixedHarass"))
               )
                Farm = true;
            else
                Farm = false;

            Q.Range = 685f + Player.BoundingRadius + 25f * Player.Spellbook.GetSpell(SpellSlot.Q).Level;

            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;
            RMANA = R.Instance.SData.Mana;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (FishBoneActive)
                    Utility.DrawCircle(Player.Position, 590f + Player.BoundingRadius, System.Drawing.Color.Gray, 1, 1);
                else
                    Utility.DrawCircle(Player.Position, Q.Range - 40, System.Drawing.Color.Gray, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Gray, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }
}
