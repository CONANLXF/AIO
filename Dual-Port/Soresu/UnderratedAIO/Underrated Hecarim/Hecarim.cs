using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using SPrediction;
using UnderratedAIO.Helpers;
using UnderratedAIO.Helpers.SkillShot;
using Environment = UnderratedAIO.Helpers.Environment;
using Prediction = LeagueSharp.Common.Prediction;
using EloBuddy.SDK;

 namespace UnderratedAIO.Champions
{
    internal class Hecarim
    {
        public static Menu config, drawMenu, comboMenu, harassMenu, miscMenu, farmMenu;
        public static LeagueSharp.Common.Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public Hecarim()
        {
            InitHecarim();
            InitMenu();
            //Game.PrintChat("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Hecarim</font>");
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender == null)
            {
                return;
            }
            if (getCheckBoxItem(miscMenu, "AutoRinterrupt") && R.CanCast(sender))
            {
                R.Cast(sender);
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawqq"), E.Range, Color.FromArgb(180, 87, 244, 255));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawww"), W.Range, Color.FromArgb(180, 87, 244, 255));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawrr"), R.Range, Color.FromArgb(180, 87, 244, 255));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawee"), getSliderItem(comboMenu, "useeRange"), Color.FromArgb(180, 255, 222, 5));
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = getCheckBoxItem(drawMenu, "drawcombo");
        }
        
        private void Game_OnUpdate(EventArgs args)
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
                Clear();
            }

            if (W.IsReady())
            {
                var dTaken =
                    HeroManager.Enemies.Where(a => a.LSDistance(player) < W.Range)
                        .Sum(a => Program.IncDamages.GetEnemyData(a.NetworkId).DamageTaken);
                if (dTaken * 0.2f > getSliderItem(miscMenu, "AutoW"))
                {
                    W.Cast();
                }
            }
        }

        private void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(1000, DamageType.Magical);
            float perc = getSliderItem(harassMenu, "minmanaH") / 100f;
            if (player.Mana < player.MaxMana * perc || target == null)
            {
                return;
            }
            if (getCheckBoxItem(harassMenu, "useqH") && Q.CanCast(target))
            {
                Q.Cast();
            }
        }

        private void Clear()
        {
            float perc = getSliderItem(farmMenu, "minmana") / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            var jungleMobQ = Jungle.GetNearest(player.Position, Q.Range);
            var Qminis = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (getCheckBoxItem(farmMenu, "useqLC") && Q.IsReady() &&
                (Qminis.Count >= getSliderItem(farmMenu, "qMinHit") || jungleMobQ != null ||
                 (Qminis.Count(m => m.Health < Q.GetDamage(m)) > 0 && !Orbwalker.CanAutoAttack)))
            {
                Q.Cast();
            }
            if (getCheckBoxItem(farmMenu, "usewLC") && W.IsReady() &&
                (MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly).Count >=
                 getSliderItem(farmMenu, "wMinHit") || jungleMobQ != null) &&
                Program.IncDamages.GetAllyData(player.NetworkId).DamageTaken > 50 && player.HealthPercent < 98)
            {
                W.Cast();
            }
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(1000, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(comboMenu, "useq") && Q.CanCast(target))
            {
                Q.Cast();
            }
            if (getCheckBoxItem(comboMenu, "usew") && W.IsInRange(target) &&
                Program.IncDamages.GetAllyData(player.NetworkId).DamageTaken > 50 && player.HealthPercent < 98)
            {
                W.Cast();
            }
            if (getCheckBoxItem(comboMenu, "usee") &&
                player.LSDistance(target) > Orbwalking.GetRealAutoAttackRange(target) + 50 &&
                player.LSDistance(target) < getSliderItem(comboMenu, "useeRange") && E.IsReady())
            {
                E.Cast();
            }
            if (getCheckBoxItem(comboMenu, "user") && R.IsReady() && R.CanCast(target))
            {
                if (getCheckBoxItem(comboMenu, "useRbeforeCC") &&
                    Program.IncDamages.GetAllyData(player.NetworkId).AnyCC)
                {
                    R.CastIfHitchanceEquals(target, HitChance.High);
                }
                R.CastIfWillHit(target, getSliderItem(comboMenu, "useRMinHit"));
            }

            var ignitedmg = (float)player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(comboMenu, "useIgnite") &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 1000) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) &&
                ((player.HealthPercent < 35) ||
                 (target.LSDistance(player) > Orbwalking.GetRealAutoAttackRange(target) + 25)))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }


        private void InitMenu()
        {
            config = MainMenu.AddMenu("Hecarim", "Hecarim");

            drawMenu = config.AddSubMenu("Drawings", "dsettings");
            drawMenu.Add("drawqq", new CheckBox("Draw Q range")); //Color.FromArgb(180, 87, 244, 255)
            drawMenu.Add("drawww", new CheckBox("Draw W range")); //Color.FromArgb(180, 87, 244, 255)
            drawMenu.Add("drawee", new CheckBox("Draw E range")); //Color.FromArgb(180, 87, 244, 255)
            drawMenu.Add("drawrr", new CheckBox("Draw R range")); //Color.FromArgb(180, 87, 244, 255)
            drawMenu.Add("drawcombo", new CheckBox("Draw combo damage"));


            comboMenu = config.AddSubMenu("Combo", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q"));
            comboMenu.Add("usew", new CheckBox("Use W"));
            comboMenu.Add("usee", new CheckBox("Use E"));
            comboMenu.Add("user", new CheckBox("Use R", false));
            comboMenu.Add("useeRange", new Slider("Max Range", 700, 350, 1000));
            comboMenu.Add("useRMinHit", new Slider("Min hit", 3, 1, 6));
            comboMenu.Add("useRbeforeCC", new CheckBox("Before CC"));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite"));


            harassMenu = config.AddSubMenu("Harass", "hsettings");
            harassMenu.Add("useqH", new CheckBox("Use Q"));
            harassMenu.Add("minmanaH", new Slider("Keep X% mana", 1, 0, 100));


            farmMenu = config.AddSubMenu("LaneClear", "Lcsettings");
            farmMenu.Add("useqLC", new CheckBox("Use Q"));
            farmMenu.Add("qMinHit", new Slider("Min hit for Q", 3, 1, 6));
            farmMenu.Add("usewLC", new CheckBox("Use W"));
            farmMenu.Add("wMinHit", new Slider("Min hit for W", 3, 1, 6));
            farmMenu.Add("minmana", new Slider("Keep X% mana", 20, 1, 100));


            miscMenu = config.AddSubMenu("Misc", "Msettings");
            miscMenu.Add("AutoRinterrupt", new CheckBox("Use R to interrupt"));
            miscMenu.Add("AutoW", new Slider("Use W to heal min", 100, 50, 500));

        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += LeagueSharp.Common.Damage.LSGetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += LeagueSharp.Common.Damage.LSGetSpellDamage(player, hero, SpellSlot.W);
            }
            if (E.IsReady())
            {
                damage += LeagueSharp.Common.Damage.LSGetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                damage += LeagueSharp.Common.Damage.LSGetSpellDamage(player, hero, SpellSlot.R);
            }
            var ignitedmg = player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float)damage;
        }

        private void InitHecarim()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 350);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 525);
            E = new LeagueSharp.Common.Spell(SpellSlot.E);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1000);
            R.SetSkillshot(0.5f, 300, 1200, true, SkillshotType.SkillshotCircle);
        }
    }
}