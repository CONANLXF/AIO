using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

 namespace HastaKalistaBaby
{
    internal class Program
    {
        public static readonly AIHeroClient Player = ObjectManager.Player;
        public static Menu root, draw, q, w, e, r, lvl;
        public static LeagueSharp.Common.Spell Q, W, E, R;
        static Items.Item botrk = new Items.Item(3153, 550);
        static Items.Item yom = new Items.Item(3142, 750);
        static Items.Item bilgwat = new Items.Item(3144, 550);
        static Items.Item healthpotion = new Items.Item(2003, 0);
        static Items.Item manapotion = new Items.Item(2004, 0);
        static Items.Item flask = new Items.Item(2041, 0);
        private static Vector3 Wlast;
        public static EarlyEvade ee;
        public static int wcount = 0;

        public static Font Text;
        public static float grabT = Game.Time, lastecast = 0f;
        public static AIHeroClient soulmate = null;


        public static void OnGameLoad()
        {
            if (Player.ChampionName != "Kalista")
            {
                return;
            }

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1130);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 5200);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1000);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1400f);

            Q.SetSkillshot(0.25f, 30f, 1700f, true, SkillshotType.SkillshotLine);

            Text = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Arial", Height = 35, Width = 12, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default });
            root = MainMenu.AddMenu("HastaKalistaBaby", "hkalista");

            Create();
            DamageIndicator.Init(Damage.GetEdamage);
            ee = new EarlyEvade();
            AutoLevel.Init();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Helper.OnProcessSpellCast;
            Spellbook.OnCastSpell += Helper.OnCastSpell;

        }

        public static void Create()
        {

            q = root.AddSubMenu("Q Settings", "spell.q");
            q.Add("AutoQ", new CheckBox("Enable Q"));
            q.Add("AutoQH", new CheckBox("Auto Q Harass"));
            q.Add("AutoQM", new CheckBox("Auto Q Across Minions"));

            w = root.AddSubMenu("W Settings", "spell.w");
            w.Add("AutoW", new CheckBox("Auto W"));
            w.Add("WAll", new KeyBind("Cast W on Nearest Monster", false, KeyBind.BindTypes.HoldActive, 'A'));
            w.Add("WBaron", new KeyBind("Cast W on Baron", false, KeyBind.BindTypes.HoldActive, 'T'));
            w.Add("WDrake", new KeyBind("Cast W on Drake", false, KeyBind.BindTypes.HoldActive, 'Y'));

            e = root.AddSubMenu("E Settings", "spell.e");
            e.AddGroupLabel("Auto E Settings");
            e.Add("AutoEChamp", new CheckBox("Auto E On Champions"));
            e.Add("AutoEDead", new CheckBox("Auto E Before Death"));
            e.Add("AutoEDeadS", new Slider("Auto E Before Death if Health % <=", 5, 1, 20));
            e.AddSeparator();
            e.AddGroupLabel("Jungle Settings");
            e.Add("BlueM", new CheckBox("Auto E Blue"));
            e.Add("RedM", new CheckBox("Auto E Red"));
            e.Add("BaronM", new CheckBox("Auto E Baron"));
            e.Add("DrakeM", new CheckBox("Auto E Dragon"));
            e.Add("SmallM", new CheckBox("Auto E Smalls", false));
            e.Add("OtherM", new CheckBox("Auto E Gromp/Wolf/Krug/Raptor"));
            e.Add("MidM", new CheckBox("Auto E Crab"));
            e.AddSeparator();
            e.AddGroupLabel("LaneClear Settings");
            e.Add("AutoEMinions", new CheckBox("Auto E Minions"));
            e.Add("minAutoEMinions", new Slider("Min minions", 2, 1, 5));
            e.Add("BigMinionFinisher", new CheckBox("Auto E Big Minions"));
            e.Add("AutoEMinionsTower", new CheckBox("Auto E Big Minions Under Tower"));

            r = root.AddSubMenu("R Settings", "spell.r");
            r.Add("AutoR", new CheckBox("Auto R Saver"));
            r.Add("KBS", new CheckBox("Auto R BlitzCrank/Skarner/Kench"));

            draw = root.AddSubMenu("Drawings", "drawings");
            draw.Add("fps", new CheckBox("Lag Free"));
            draw.Add("Qrange", new CheckBox("Draw Q Range", false));
            draw.Add("Wrange", new CheckBox("Draw W Range", false));
            draw.Add("Erange", new CheckBox("Draw E Range"));
            draw.Add("Rrange", new CheckBox("Draw R Range"));
            draw.Add("healthp", new CheckBox("Show Health Percent"));
            draw.Add("healthp1", new CheckBox("Show Damage HealthBar"));//.SetValue(new Circle(true, System.Drawing.Color.Purple)));
            draw.Add("TargetA", new CheckBox("Draw Target Attack Range"));

            lvl = root.AddSubMenu("Level Settigns", "lvl");
            lvl.Add("Lvlon", new CheckBox("Enable Level Up"));
            lvl.Add("1", new ComboBox("1", 3, "Q", "W", "E", "R"));
            lvl.Add("2", new ComboBox("2", 1, "Q", "W", "E", "R"));
            lvl.Add("3", new ComboBox("3", 1, "Q", "W", "E", "R"));
            lvl.Add("4", new ComboBox("4", 1, "Q", "W", "E", "R"));
            lvl.Add("s", new Slider("Start at level", 2, 1, 5));
        }
        
        public static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Qlogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (q["AutoQH"].Cast<CheckBox>().CurrentValue)
                {
                    Qlogic();
                }
            }

            WLogic();
            WHelper();
            RLogic();
            ELogic();
            LaneClear();
            JungleClear();
        }



        private static void Qlogic()
        {
            if (!Q.IsReady() || !q["AutoQ"].Cast<CheckBox>().CurrentValue || Helper.GetMana(Q) < 80)
            {
                return;
            }

            var target = TargetSelector.GetTarget(E.Range * 1.2f, DamageType.Physical);
            if (target.LSIsValidTarget())
            {
                var predout = Q.GetPrediction(target);
                var coll = predout.CollisionObjects;

                if (coll.Count < 1)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }
                if (coll.Count == 1 && q["AutoQM"].Cast<CheckBox>().CurrentValue)
                {
                    foreach (var c in coll)
                    {
                        if (Damage.GetEdamage(c) > c.Health)
                        {
                            Q.Cast(predout.CastPosition);
                        }
                    }
                }
            }
        }

        private static void WLogic()
        {
            if (!W.IsReady() || Helper.GetMana(W) < 80 || Player.LSIsRecalling() || Player.LSCountEnemiesInRange(1500) > 0)
            {
                return;
            }

            if (w["WBaron"].Cast<KeyBind>().CurrentValue)
            {
                Vector3 baronPos;
                baronPos.X = 5232;
                baronPos.Y = 10788;
                baronPos.Z = 0;
                if (Player.LSDistance(baronPos) < 5000)
                {
                    W.Cast(baronPos);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Wlast);
                }
            }

            if (w["WDrake"].Cast<KeyBind>().CurrentValue)
            {
                Vector3 dragonPos;
                dragonPos.X = 9919f;
                dragonPos.Y = 4475f;
                dragonPos.Z = 0f;
                if (Player.LSDistance(dragonPos) < 5000)
                {
                    W.Cast(dragonPos);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Wlast);
                }
            }

            if ((w["AutoW"].Cast<CheckBox>().CurrentValue || w["WAll"].Cast<KeyBind>().CurrentValue))
            {
                if (wcount > 0)
                {
                    Vector3 baronPos;
                    baronPos.X = 5232;
                    baronPos.Y = 10788;
                    baronPos.Z = 0;
                    if (Player.LSDistance(baronPos) < 5000)
                    {
                        W.Cast(baronPos);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Wlast);
                    }
                }
                if (wcount == 0)
                {
                    Vector3 dragonPos;
                    dragonPos.X = 9919f;
                    dragonPos.Y = 4475f;
                    dragonPos.Z = 0f;
                    if (Player.LSDistance(dragonPos) < 5000)
                    {
                        W.Cast(dragonPos);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Wlast);
                    }
                    else
                        wcount++;
                    return;
                }

                if (wcount == 1)
                {
                    Vector3 redPos;
                    redPos.X = 8022;
                    redPos.Y = 4156;
                    redPos.Z = 0;
                    if (Player.LSDistance(redPos) < 5000)
                    {
                        W.Cast(redPos);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Wlast);
                    }
                    else
                        wcount++;
                    return;
                }
                if (wcount == 2)
                {
                    Vector3 bluePos;
                    bluePos.X = 11396;
                    bluePos.Y = 7076;
                    bluePos.Z = 0;
                    if (Player.LSDistance(bluePos) < 5000)
                    {
                        W.Cast(bluePos);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Wlast);
                    }
                    else
                        wcount++;
                    return;
                }
                if (wcount > 2)
                {
                    wcount = 0;
                }
            }
        }

        private static void ELogic()
        {
            if (!E.IsReady())
            {
                return;
            }

            if (e["AutoEDead"].Cast<CheckBox>().CurrentValue && Player.HealthPercent < e["AutoEDeadS"].Cast<Slider>().CurrentValue && HeroManager.Enemies.Any(o => o.LSIsValidTarget() && Helper.hasE(o) && E.IsInRange(o)))
            {
                E.Cast();
            }

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && Helper.hasE(x) && !Helper.Unkillable(x) && x.LSDistance(Player) < 900 && !x.IsDead))
            {
                if (e["AutoEChamp"].Cast<CheckBox>().CurrentValue)
                {
                    if (Damage.GetEdamage(enemy) > Helper.GetHealth(enemy))
                    {
                        CastE();
                    }
                }
            }
        }

        private static void RLogic()
        {
            if (Player.LSIsRecalling() || Player.InFountain() || !R.IsReady() || Helper.GetMana(R) < 80 || !r["AutoR"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (soulmate == null)
            {
                foreach (var ally in HeroManager.Allies.Where(x => !x.IsDead && !x.IsMe && x.HasBuff("kalistacoopstrikeally")))
                {
                    soulmate = ally;
                    break;
                }
            }
            else if (soulmate.IsVisible && soulmate.LSDistance(Player) < R.Range)
            {
                if (soulmate.Health < Helper.CountEnemy(soulmate.Position, 600) * soulmate.Level * 30 || Helper.IncomingDamage > soulmate.Health)
                {
                    R.Cast();
                }
                if (soulmate.ChampionName == "Blitzcrank" && Player.LSDistance(soulmate.Position) > 300)
                {
                    if (Game.Time - grabT < 0.7)
                    {
                        return;
                    }
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.HasBuff("rocketgrab2")))
                    {
                        R.Cast();
                    }
                }
                if (soulmate.ChampionName == "TahmKench" && Player.LSDistance(soulmate.Position) > 300)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.HasBuff("tahmkenchwdevoured")))
                    {
                        R.Cast();
                    }
                }
                if (soulmate.ChampionName == "Skarner" && Player.LSDistance(soulmate.Position) > 300)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.HasBuff("skarnerimpale")))
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (!E.IsReady() || Helper.GetMana(E) < 80)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(Player), MinionTypes.All, MinionTeam.Enemy).ToList();

            if (minions.Count == 0)
            {
                return;
            }
            if (e["AutoEMinions"].Cast<CheckBox>().CurrentValue || e["BigMinionFinisher"].Cast<CheckBox>().CurrentValue || e["AutoEMinionsTower"].Cast<CheckBox>().CurrentValue)
            {
                int killable = 0;
                foreach (var m in minions)
                {
                    if (Damage.GetEdamage(m) > m.Health && HealthPrediction.GetHealthPrediction(m, 500, 250) > Player.LSGetAutoAttackDamage(m) && m.GetBuff("kalistaexpungemarker").EndTime > 0.5)
                    {
                        killable++;
                        if (killable >= e["minAutoEMinions"].Cast<Slider>().CurrentValue && e["AutoEMinions"].Cast<CheckBox>().CurrentValue || (m.CharData.BaseSkinName.ToLower().Contains("siege") || m.CharData.BaseSkinName.ToLower().Contains("super")) && e["BigMinionFinisher"].Cast<CheckBox>().CurrentValue)
                        {
                            CastE();
                            break;
                        }
                        if ((m.CharData.BaseSkinName.ToLower().Contains("siege") || m.CharData.BaseSkinName.ToLower().Contains("super")) && e["AutoEMinionsTower"].Cast<CheckBox>().CurrentValue && killable > 0 && m.UnderTurret())
                        {
                            CastE();
                            break;
                        }
                    }
                }
            }
        }

        private static void WHelper()
        {
            if (Player.GetWaypoints().LastOrDefault().LSDistance(Player.Position) > 250)
            {
                Wlast = Player.GetWaypoints().LastOrDefault().To3D();
            }
        }

        private static void JungleClear()
        {

            foreach (var jungle in MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral))
            {
                if (Damage.GetEdamage(jungle) > jungle.Health)
                {
                    if (jungle.Name.Contains("Red") && e["RedM"].Cast<CheckBox>().CurrentValue && !jungle.Name.Contains("RedMini"))
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Blue") && e["BlueM"].Cast<CheckBox>().CurrentValue && !jungle.Name.Contains("BlueMini"))
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Baron") && e["BaronM"].Cast<CheckBox>().CurrentValue)
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Dragon") && e["DrakeM"].Cast<CheckBox>().CurrentValue)
                    {
                        CastE();
                    }
                    if ((jungle.Name.Contains("Krug") || jungle.Name.Contains("Razor") || jungle.Name.Contains("wolf") || jungle.Name.Contains("Gromp")) && e["OtherM"].Cast<CheckBox>().CurrentValue && !jungle.Name.Contains("Mini"))
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Crab") && e["MidM"].Cast<CheckBox>().CurrentValue)
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Mini") && e["SmallM"].Cast<CheckBox>().CurrentValue && !jungle.Name.Contains("Crab"))
                    {
                        CastE();
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (draw["Qrange"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady())
                {
                    if (draw["fps"].Cast<CheckBox>().CurrentValue)
                    {
                        Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Violet);
                    }
                    else
                    {
                        Drawing.DrawCircle(Player.Position, Q.Range, Color.Violet);
                    }
                }
            }

            if (draw["Wrange"].Cast<CheckBox>().CurrentValue)
            {
                if (W.IsReady())
                {
                    if (draw["fps"].Cast<CheckBox>().CurrentValue)
                    {
                        Render.Circle.DrawCircle(Player.Position, W.Range, Color.Cyan);
                    }
                    else
                    {
                        Drawing.DrawCircle(Player.Position, W.Range, Color.Cyan);
                    }
                }
            }

            if (draw["Erange"].Cast<CheckBox>().CurrentValue)
            {
                if (E.IsReady())
                {
                    if (draw["fps"].Cast<CheckBox>().CurrentValue)
                    {
                        Render.Circle.DrawCircle(Player.Position, E.Range, Color.Orange);
                    }
                    else
                    {
                        Drawing.DrawCircle(Player.Position, E.Range, Color.Orange);
                    }
                }
            }

            if (draw["Rrange"].Cast<CheckBox>().CurrentValue)
            {
                if (R.IsReady())
                {
                    if (draw["fps"].Cast<CheckBox>().CurrentValue)
                    {
                        Render.Circle.DrawCircle(Player.Position, R.Range, Color.Gray);
                    }
                    else
                    {
                        Drawing.DrawCircle(Player.Position, R.Range, Color.Gray);
                    }
                }
            }

            if (draw["healthp"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Base>().Where(x => (x.LSIsValidTarget(E.Range) && Helper.hasE(x) && !x.IsMinion && !x.Name.Contains("Mini")) && (x.IsEnemy || x.Name.Contains("Krug") || x.Name.Contains("Razor") || x.Name.Contains("wolf") || x.Name.Contains("Gromp") || x.Name.Contains("Crab") || x.Name.Contains("Blue") || x.Name.Contains("Red"))))
                {
                    float hp = Helper.GetHealth(enemy) - Damage.GetEdamage(enemy);
                    var dmg = ((int)((Damage.GetEdamage(enemy) / Helper.GetHealth(enemy)) * 100));

                    if (dmg <= 9)
                    {
                        Text.DrawText(null, dmg.ToString(), (int)enemy.HPBarPosition.X + 108, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, "%", (int)enemy.HPBarPosition.X + 125, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, dmg.ToString() + "%", (int)enemy.HPBarPosition.X + 110, (int)enemy.HPBarPosition.Y + 40, SharpDX.Color.WhiteSmoke);
                    }
                    if (dmg >= 10)
                    {
                        Text.DrawText(null, dmg.ToString(), (int)enemy.HPBarPosition.X + 108, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, "%", (int)enemy.HPBarPosition.X + 138, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, dmg.ToString() + "%", (int)enemy.HPBarPosition.X + 110, (int)enemy.HPBarPosition.Y + 40, SharpDX.Color.WhiteSmoke);
                    }
                }
            }

            if (draw["TargetA"].Cast<CheckBox>().CurrentValue && !Player.IsDead)
            {

                var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);

                if (t == null)
                    return;

                if (Player.LSDistance(t) > Helper.GetAttackRange(t))
                {
                    Render.Circle.DrawCircle(t.Position, Helper.GetAttackRange(t), Color.ForestGreen);
                }
                else
                {
                    Render.Circle.DrawCircle(t.Position, Helper.GetAttackRange(t), Color.OrangeRed);
                }
            }
        }



        private static void CastE()
        {
            if (Game.Time - lastecast < 0.700)
            {
                return;
            }

            E.Cast();
        }
    }
}
