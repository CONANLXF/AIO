using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

 namespace SurvivorMalzahar
{
    class Program
    {
        public const string ChampionName = "Malzahar";
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        //Menu
        public static Menu Menu, combo, harass, lc, DrawingMenu, miscMenu;
        //Spells
        public static List<LeagueSharp.Common.Spell> SpellList = new List<LeagueSharp.Common.Spell>();
        private static float Rtime = 0;
        public static LeagueSharp.Common.Spell Q, W, E, R;
        private const float SpellQWidth = 400f;
        public static SpellSlot igniteSlot;
        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Malzahar") return;

            igniteSlot = Player.GetSpellSlot("summonerdot");
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 900f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 650f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 650f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 700f);

            Q.SetSkillshot(0.75f, 80, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.5f, 80, 20, false, SkillshotType.SkillshotCircle);

            Menu = MainMenu.AddMenu("SurvivorMalzahar", "SurvivorMalzahar");

            #region Combo/Harass/LaneClear/OneShot
            //Combo Menu
            combo = Menu.AddSubMenu("Combo", "Combo");
            combo.Add("useQ", new CheckBox("Use Q"));
            combo.Add("useW", new CheckBox("Use W"));
            combo.Add("useE", new CheckBox("Use E"));
            combo.Add("useR", new CheckBox("Use R"));
            combo.Add("useIgniteInCombo", new CheckBox("Use Ignite if Killable"));

            //Harass Menu
            harass = Menu.AddSubMenu("Harass", "Harass");
            harass.Add("autoharass", new CheckBox("Auto Harrass with E"));
            harass.Add("autoharassuseQ", new CheckBox("Auto Harrass with Q", false));
            harass.Add("autoharassminimumMana", new Slider("Minimum Mana%", 30));

            //LaneClear Menu
            lc = Menu.AddSubMenu("Laneclear", "Laneclear");
            lc.Add("laneclearE", new CheckBox("Use E to LaneClear"));
            lc.Add("laneclearQ", new CheckBox("Use Q to LaneClear"));
            lc.Add("laneclearW", new CheckBox("Use W to LaneClear"));
            lc.Add("LaneClearMinions", new Slider("LaneClear Minimum Minions for Q", 2, 0, 10));
            lc.Add("LaneClearEMinMinions", new Slider("LaneClear Minimum Minions for E", 2, 0, 10));
            lc.Add("laneclearEMinimumMana", new Slider("Minimum E Mana%", 30));
            lc.Add("laneclearQMinimumMana", new Slider("Minimum Q Mana%", 30));
            lc.Add("laneclearWMinimumMana", new Slider("Minimum W Mana%", 30));

            // Drawing Menu
            DrawingMenu = Menu.AddSubMenu("Drawings", "Drawings");
            DrawingMenu.Add("drawQ", new CheckBox("Draw Q range", false));
            DrawingMenu.Add("drawW", new CheckBox("Draw W range", false));
            DrawingMenu.Add("drawE", new CheckBox("Draw E range", false));
            DrawingMenu.Add("drawR", new CheckBox("Draw R range"));

            // Misc Menu
            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ksE", new CheckBox("Use E to KillSteal"));
            miscMenu.Add("ksQ", new CheckBox("Use Q to KillSteal"));
            miscMenu.Add("interruptQ", new CheckBox("Interrupt Spells Q", true));
            miscMenu.Add("useQAntiGapCloser", new CheckBox("Use Q on GapClosers"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("Gapclose R : ");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                miscMenu.Add("gapcloserR" + enemy.ChampionName, new CheckBox(enemy.ChampionName, false));
            miscMenu.AddGroupLabel("Burst : ");
            miscMenu.AddLabel("[Burst Info] If you don't have mana to cast Q/W/E/R spells all together it won't cast the spells. Use Combo Instead.");
            miscMenu.Add("oneshot", new KeyBind("Burst Combo", false, KeyBind.BindTypes.HoldActive, 'T'));

            #endregion
            #region Subscriptions
            Game.OnUpdate += OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += OnDraw;
            #endregion
        }
        private static void OnDraw(EventArgs args)
        {
            if (DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.DarkRed, 3);
            }
            if (DrawingMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Render.Circle.DrawCircle(Player.Position, 450f, System.Drawing.Color.LightBlue, 3);
            }
            if (DrawingMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Purple, 3);
            }
            if (DrawingMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.LightPink, 3);
            }
        }
        
        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.LSIsRecalling())
            {
                return;
            }

            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.HasBuff("malzaharrsound"))
            {
                PortAIO.OrbwalkerManager.SetAttack(false);
                PortAIO.OrbwalkerManager.SetMovement(false);
                return;
            }
            else
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
                PortAIO.OrbwalkerManager.SetMovement(true);
            }
            if (E.IsReady() && miscMenu["ksE"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var h in HeroManager.Enemies.Where(h => h.LSIsValidTarget(E.Range) && h.Health < Player.LSGetSpellDamage(h, SpellSlot.E)))
                {
                    E.Cast(h);
                }
            }
            if (Q.IsReady() && miscMenu["ksQ"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var h in HeroManager.Enemies.Where(h => h.LSIsValidTarget(Q.Range) && h.Health < Player.LSGetSpellDamage(h, SpellSlot.Q)))
                {
                    var pred = Q.GetPrediction(h);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            //Combo
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            //Burst
            if (miscMenu["oneshot"].Cast<KeyBind>().CurrentValue)
            {
                Oneshot();
            }
            //Lane
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Lane();
            }
            //AutoHarass
            AutoHarass();
        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.HasBuff("malzaharrsound"))
            {
                PortAIO.OrbwalkerManager.SetAttack(false);
                PortAIO.OrbwalkerManager.SetMovement(false);
                return;
            }
            else
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
                PortAIO.OrbwalkerManager.SetMovement(true);
            }
            if (!miscMenu["interruptQ"].Cast<CheckBox>().CurrentValue || !Q.IsReady())
                return;

            if (t.LSIsValidTarget(Q.Range))
            {
                Q.Cast(t);
            }
        }

        #region Q Range/Placement Calculations (BETA)
        /*private void CastQ(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!Q.IsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            Q.Width = GetDynamicQWidth(target);
            Q.Cast(target);
        }
        public static float GetManaPercent()
        {
            return (ObjectManager.Player.Mana / ObjectManager.Player.MaxMana) * 100f;
        }
        private static float GetDynamicQWidth(Obj_AI_Base target)
        {
            return Math.Max(70, (1f - (ObjectManager.Player.Distance(target) / Q.Range)) * SpellQWidth);
        }*/
        #endregion

        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.HasBuff("malzaharrsound"))
            {
                PortAIO.OrbwalkerManager.SetAttack(false);
                PortAIO.OrbwalkerManager.SetMovement(false);
                return;
            }
            else
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
                PortAIO.OrbwalkerManager.SetMovement(true);
            }
            // Improved AntiGap Closer
            var sender = gapcloser.Sender;
            if (!gapcloser.Sender.LSIsValidTarget())
            {
                return;
            }

            if (miscMenu["useQAntiGapCloser"].Cast<CheckBox>().CurrentValue && sender.LSIsValidTarget(Q.Range))
            {
                Q.Cast(gapcloser.End);
            }
            if (R.IsReady() && miscMenu["gapcloserR" + gapcloser.Sender.ChampionName].Cast<CheckBox>().CurrentValue && sender.LSIsValidTarget(R.Range) && gapcloser.End == Player.ServerPosition)
            {
                R.CastOnUnit(sender);
            }
        }
        private static float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;
            if (igniteSlot != SpellSlot.Unknown || Player.Spellbook.CanUseSpell(igniteSlot) == SpellState.Ready)
            {
                if (combo["useIgniteInCombo"].Cast<CheckBox>().CurrentValue)
                {
                    damage += (float)Player.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
                }
            }
            double ultdamage = 0;

            if (Q.IsReady())
            {
                damage += Q.GetDamage(enemy);
            }

            if (W.IsReady())
            {
                damage += W.GetDamage(enemy);
            }

            if (E.IsReady())
            {
                damage += E.GetDamage(enemy);
            }

            if (R.IsReady())
            {
                ultdamage += Player.LSGetSpellDamage(enemy, SpellSlot.R);
            }
            return damage + ((float)ultdamage * 2);
        }
        private static void AutoHarass()
        {
            if (Player.ManaPercent < harass["autoharassminimumMana"].Cast<Slider>().CurrentValue)
                return;
            var m = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            var pred = Q.GetPrediction(m);
            if (m != null && harass["autoharass"].Cast<CheckBox>().CurrentValue)
                E.CastOnUnit(m);
            if (m != null && harass["autoharassuseQ"].Cast<CheckBox>().CurrentValue)
                if (pred.Hitchance >= HitChance.High)
                {
                    Q.Cast(pred.CastPosition);
                }
        }
        private static bool HasRBuff()
        {
            return (Player.IsChannelingImportantSpell() || Player.HasBuff("AiZaharNetherGrasp") || Player.HasBuff("MalzaharR") || Player.HasBuff("MalzaharRSound") || R.IsChanneling);
        }
        //Combo
        private static void Combo()
        {
            var useQ = (combo["useQ"].Cast<CheckBox>().CurrentValue);
            var useW = (combo["useW"].Cast<CheckBox>().CurrentValue);
            var useE = (combo["useE"].Cast<CheckBox>().CurrentValue);
            var useR = (combo["useR"].Cast<CheckBox>().CurrentValue);
            var m = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var pred = Q.GetPrediction(m);
            if (!m.LSIsValidTarget())
            {
                return;
            }
            if (Player.Mana > E.ManaCost + W.ManaCost + R.ManaCost)
            {
                if (useE && E.IsReady() && E.IsInRange(m)) E.CastOnUnit(m);
                if (useQ && Q.IsReady() && Player.Mana > Q.ManaCost && Q.IsInRange(m))
                {
                    if (pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
                if (useW && W.IsReady()) W.Cast(m);
                if (useR && R.IsReady() && m != null && E.IsInRange(m)) R.CastOnUnit(m);
            }
            else
            {
                if (useE && E.IsReady() && E.IsInRange(m)) E.CastOnUnit(m);
                if (useQ && Q.IsReady() && Player.Mana > Q.ManaCost && Q.IsInRange(m))
                {
                    if (pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
                if (useW && W.IsReady() && Player.Mana > W.ManaCost && W.IsInRange(m)) W.Cast(m);
            }
            if (combo["useIgniteInCombo"].Cast<CheckBox>().CurrentValue)
            {
                if (m.Health < Player.GetSummonerSpellDamage(m, LeagueSharp.Common.Damage.SummonerSpell.Ignite))
                {
                    Player.Spellbook.CastSpell(igniteSlot, m);
                }
            }
        }
        //Burst
        public static void Oneshot()
        {
            // If player doesn't have mana don't execute the OneShot Combo
            if (Player.Mana < Q.ManaCost + W.ManaCost + E.ManaCost + R.ManaCost)
                return;


            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.HasBuff("malzaharrsound"))
            {
                PortAIO.OrbwalkerManager.SetAttack(false);
                PortAIO.OrbwalkerManager.SetMovement(false);
                return;
            }
            else
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
                PortAIO.OrbwalkerManager.SetMovement(true);
            }

            Orbwalker.MoveTo(Game.CursorPos);
            var m = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (!m.LSIsValidTarget())
            {
                return;
            }
            var pred = Q.GetPrediction(m);
            if (Q.IsReady() && Q.IsInRange(m))
            {
                if (pred.Hitchance >= HitChance.High)
                {
                    Q.Cast(pred.CastPosition);
                }
            }
            if (E.IsReady() && E.IsInRange(m)) E.CastOnUnit(m);
            if (W.IsReady()) W.Cast(m);
            Player.Spellbook.CastSpell(igniteSlot, m);
            if (R.IsReady() && !E.IsReady() && !W.IsReady() && R.IsInRange(m)) R.CastOnUnit(m);
        }
        //Lane
        private static void Lane()
        {
            if (Player.ManaPercent < lc["laneclearEMinimumMana"].Cast<Slider>().CurrentValue || Player.ManaPercent < lc["laneclearQMinimumMana"].Cast<Slider>().CurrentValue || Player.ManaPercent < lc["laneclearWMinimumMana"].Cast<Slider>().CurrentValue)
                return;

            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 450f);
            if (allMinions.Count > lc["LaneClearEMinMinions"].Cast<Slider>().CurrentValue)
            {
                if (lc["laneclearE"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.LSIsValidTarget() && !minion.HasBuff("malzahare") && minion.Health < E.GetDamage(minion))
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                }
            }
            if (lc["laneclearW"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                foreach (var minion in allMinionsW)
                {
                    if (minion.IsValidTarget())
                    {
                        W.Cast(minion);
                    }
                }
            }
            if (lc["laneclearQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
                var farmPos = Q.GetCircularFarmLocation(allMinionsQ, 150);
                if (farmPos.MinionsHit > lc["LaneClearMinions"].Cast<Slider>().CurrentValue)
                    Q.Cast(farmPos.Position);
            }
        }
    }
}
