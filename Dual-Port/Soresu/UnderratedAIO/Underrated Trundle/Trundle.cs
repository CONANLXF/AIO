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
    internal class Trundle
    {
        public static Menu config;
        public static Menu drawMenu, comboMenu, harassMenu, miscMenu, farmMenu;
        public static LeagueSharp.Common.Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public Trundle()
        {
            InitTrundle();
            InitMenu();
            Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Trundle</font>");
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnDamage += Obj_AI_Base_OnDamage;
            Orbwalker.OnPostAttack += AfterAttack;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "AutoEDash"))
            {
                return;
            }
            if (!E.IsReady())
            {
                return;
            }
            var hero = sender as AIHeroClient;
            if (hero != null && hero.ChampionName == "Tristana" && args.Slot == SpellSlot.W)
            {
                var dashIntPoint = sender.Position.Extend(args.End, 300);
                if (dashIntPoint.Distance(player.Position) < 1000)
                {
                    E.Cast(dashIntPoint);
                }
            }
        }

        private void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!getCheckBoxItem(miscMenu, "AutoEDash"))
            {
                return;
            }
            if (args.IsBlink || !E.IsReady())
            {
                return;
            }
            if (!sender.IsEnemy || !(sender is AIHeroClient))
            {
                return;
            }
            var hero = sender as AIHeroClient;
            if (hero.ChampionName == "Tristana")
            {
                return;
            }
            var steps = 6f;
            var stepLength = args.StartPos.LSDistance(args.EndPos) / steps;
            for (int i = 1; i < steps + 1; i++)
            {
                var p = args.StartPos.LSExtend(args.EndPos, stepLength * i);
                if (p.IsWall() && p.LSDistance(args.StartPos) > args.Speed * 0.25f - E.Width && p.LSDistance(player) < 1000)
                {
                    E.Cast(p.LSExtend(args.StartPos, E.Width));
                    return;
                }
            }
            var predRange = Math.Min(args.Speed * 0.35f, args.StartPos.LSDistance(args.EndPos));
            var dashIntPoint = args.StartPos.LSExtend(args.EndPos, predRange);
            if (dashIntPoint.LSDistance(player) < 1000)
            {
                E.Cast(dashIntPoint);
            }
        }
        
        private void AfterAttack(AttackableUnit target, EventArgs args)
        {
            AIHeroClient targetO = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (Q.IsReady() && target != null)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) &&
                    getCheckBoxItem(farmMenu, "useqLC"))
                {
                    var minis = MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition, 600, MinionTypes.All, MinionTeam.NotAlly);

                    float perc = getSliderItem(farmMenu, "minmana") / 100f;
                    if (player.Mana > player.MaxMana * perc &&
                        (minis.Count() > 1 || player.GetAutoAttackDamage((Obj_AI_Base) target, true) < target.Health))
                    {
                        Q.Cast();
                        Orbwalker.ResetAutoAttack();
                    }
                }
                if (targetO != null && targetO.NetworkId != target.NetworkId)
                {
                    return;
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                    getCheckBoxItem(harassMenu, "useqH"))
                {
                    float perc = getSliderItem(harassMenu, "minmanaH") / 100f;
                    if (player.Mana > player.MaxMana * perc)
                    {
                        Q.Cast();
                        Orbwalker.ResetAutoAttack();
                    }
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                    getCheckBoxItem(comboMenu, "useq"))
                {
                    Q.Cast();
                    Orbwalker.ResetAutoAttack();
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender == null)
            {
                return;
            }
            if (getCheckBoxItem(miscMenu, "AutoEinterrupt") && E.CanCast(sender))
            {
                E.Cast(sender.Position);
            }
        }

        private void Obj_AI_Base_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (getCheckBoxItem(miscMenu, "AutoETower") && E.IsReady())
            {
                var t = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => h.NetworkId == args.Source.NetworkId);
                var s = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => h.NetworkId == args.Target.NetworkId);
                if (t == null || s == null)
                {
                    return;
                }
                var turret =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .FirstOrDefault(tw => tw.LSDistance(t) < 1000 && tw.LSDistance(s) < 1000 && tw.IsAlly);
                if (s is AIHeroClient && t is AIHeroClient && s.IsAlly && turret != null && E.CanCast(t))
                {
                    E.Cast(t.Position.LSExtend(turret.Position, -(t.BoundingRadius + E.Width)));
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawrw"), W.Range, Color.AliceBlue);
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawre"), E.Range, Color.AliceBlue);
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawrr"), R.Range, Color.AliceBlue);
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = getCheckBoxItem(drawMenu, "drawcombo");
        }

        private void Game_OnUpdate(EventArgs args)
        {

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }

            if (false && E.IsReady() && getCheckBoxItem(miscMenu, "AutoEDash"))
            {
                foreach (var data in HeroManager.Allies.Select(a => Program.IncDamages.GetAllyData(a.NetworkId)))
                {
                    foreach (var skillshot in
                        data.Damages.Where(
                            d =>
                                d.SkillShot != null && d.SkillShot.SkillshotData.Slot == SpellSlot.R &&
                                d.SkillShot.SkillshotData.ChampionName == "Blitzcrank"))
                    {
                        E.Cast(skillshot.Target.Position.LSExtend(skillshot.SkillShot.StartPosition.To3D(), E.Range * 2));
                    }
                }
            }
        }
    
        private void Clear()
        {
            float perc = getSliderItem(farmMenu, "minmana") / 100f;
            if (player.Mana < player.MaxMana * perc || player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            if (getCheckBoxItem(farmMenu, "useqLC") && Q.IsReady() && !Orbwalker.CanAutoAttack)
            {
                Q.Cast();
            }
            if (getCheckBoxItem(farmMenu, "usewLC") && W.IsReady())
            {
                var minis = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, 600, MinionTypes.All, MinionTeam.NotAlly);
                MinionManager.FarmLocation bestPositionE = W.GetCircularFarmLocation(minis, 600);
                if (bestPositionE.MinionsHit >= getSliderItem(farmMenu, "wMinHit") ||
                    minis.Any(m => m.MaxHealth > 1500))
                {
                    W.Cast(bestPositionE.Position);
                }
            }
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (player.Spellbook.IsAutoAttacking || target == null || !Orbwalker.CanMove)
            {
                return;
            }
            if (getCheckBoxItem(comboMenu, "usee") &&
                player.LSDistance(target) < getSliderItem(comboMenu, "useeRange") && E.IsReady())
            {
                var pos = GetVectorE(target);
                if (player.LSDistance(pos) < E.Range)
                {
                    E.Cast(pos);
                }
            }
            if (getCheckBoxItem(comboMenu, "usew") && W.IsReady() &&
                (!target.UnderTurret(true) || player.UnderTurret(true)))
            {
                var pos = player.Position.LSExtend(Prediction.GetPrediction(target, 700).UnitPosition, W.Range / 2);
                if (player.LSDistance(pos) < W.Range)
                {
                    W.Cast(pos);
                }
            }
            if (getCheckBoxItem(comboMenu, "user") && R.IsReady())
            {
                AIHeroClient targetR = null;
                switch (getBoxItem(comboMenu, "userTarget"))
                {
                    case 0:
                        targetR =
                            HeroManager.Enemies.Where(e => e.LSIsValidTarget(R.Range))
                                .OrderByDescending(e => (e.Armor + e.FlatMagicReduction))
                                .FirstOrDefault();
                        break;
                    case 1:
                        targetR = target.LSIsValidTarget(R.Range) ? target : null;
                        break;
                }

                if (targetR != null)
                {
                    var userTime = getBoxItem(comboMenu, "userTime");
                    if (userTime == 0 || userTime == 2)
                    {
                        if (player.LSCountEnemiesInRange(R.Range) >= 2)
                        {
                            R.Cast(targetR);
                        }
                    }
                    if (userTime == 1 || userTime == 2)
                    {
                        var data = Program.IncDamages.GetAllyData(player.NetworkId);
                        if (data.DamageTaken > player.Health * 0.4 || data.IsAboutToDie ||
                            (player.HealthPercent < 60 && target.HealthPercent < 60 &&
                             player.LSDistance(target) < Orbwalking.GetRealAutoAttackRange(target)))
                        {
                            R.Cast(targetR);
                        }
                    }
                }
            }

            var ignitedmg = (float) player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
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

        private Vector3 GetVectorE(AIHeroClient target)
        {
            var pos = Vector3.Zero;
            var pred = Prediction.GetPrediction(target, 0.28f);
            if (!target.IsMoving)
            {
                return pos;
            }
            var distW = E.Width / 2 + target.BoundingRadius;
            var points = CombatHelper.PointsAroundTheTarget(pred.UnitPosition, distW);
            var walls =
                points.Where(p => p.LSIsWall() && player.LSDistance(target) > target.BoundingRadius)
                    .OrderBy(p => p.LSDistance(pred.UnitPosition));
            var wall = walls.FirstOrDefault();
            if (wall.IsValid() && wall.LSDistance(target.Position) < 350 &&
                walls.Any(w => w.LSDistance(target.Position) < distW))
            {
                pos = wall.LSExtend(pred.UnitPosition, (target.BoundingRadius + distW));
            }
            if (getCheckBoxItem(comboMenu, "useeWall"))
            {
                return pos;
            }
            if (pred.Hitchance < HitChance.Medium || target.LSDistance(player) < Orbwalking.GetRealAutoAttackRange(target))
            {
                return pos;
            }
            if (pred.UnitPosition.LSDistance(player.Position) > player.LSDistance(target))
            {
                var dist = target.BoundingRadius + E.Width;
                var predPos = pred.UnitPosition;
                if (target.LSDistance(predPos) < dist)
                {
                    predPos = target.Position.LSExtend(predPos, dist);
                }
                pos = predPos.LSExtend(target.Position, -dist);
            }
            return pos;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private void InitMenu()
        {
            config = MainMenu.AddMenu("Trundle", "Trundle");

            drawMenu = config.AddSubMenu("Drawings", "dsettings");
            drawMenu.Add("drawww", new CheckBox("Draw W range"));
            drawMenu.Add("drawee", new CheckBox("Draw E range"));
            drawMenu.Add("drawrr", new CheckBox("Draw R range"));
            drawMenu.Add("drawcombo", new CheckBox("Draw combo damage"));


            comboMenu = config.AddSubMenu("Combo", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q"));
            comboMenu.Add("usew", new CheckBox("Use W"));
            comboMenu.Add("usee", new CheckBox("Use E"));
            comboMenu.Add("useeWall", new CheckBox("Only Near wall", false));
            comboMenu.Add("useeRange", new Slider("Max Range", 600, (int)player.AttackRange, 1000));
            comboMenu.Add("user", new CheckBox("Use R"));
            comboMenu.Add("userTarget", new ComboBox("R target", 0, "Highest def", "Only target"));
            comboMenu.Add("userTime", new ComboBox("R usage", 1, ">= 2 enemy", "Before high damage", "Both"));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite"));


            harassMenu = config.AddSubMenu("Harass", "hsettings");
            harassMenu.Add("useqH", new CheckBox("Use Q"));
            harassMenu.Add("minmanaH", new Slider("Keep X% mana", 1, 0, 100));


            farmMenu = config.AddSubMenu("LaneClear", "Lcsettings");
            farmMenu.Add("useqLC", new CheckBox("Use Q"));
            farmMenu.Add("usewLC", new CheckBox("Use W"));
            farmMenu.Add("wMinHit", new Slider("Min hit", 3, 1, 6));
            farmMenu.Add("minmana", new Slider("Keep X% mana", 20, 1, 100));


            miscMenu = config.AddSubMenu("Misc", "Msettings");
            miscMenu.Add("AutoETower", new CheckBox("Use E on tower aggro"));
            miscMenu.Add("AutoEinterrupt", new CheckBox("Use E interrupt"));
            miscMenu.Add("AutoEDash", new CheckBox("Use E ond Dash"));
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += LeagueSharp.Common.Damage.LSGetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (R.IsReady())
            {
                damage += LeagueSharp.Common.Damage.LSGetSpellDamage(player, hero, SpellSlot.R);
            }
            // damage += ItemHandler.GetItemsDamage(hero);
            var ignitedmg = player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void InitTrundle()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1000);
            W.SetSkillshot(0.25f, 1000, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1000);
            E.SetSkillshot(0.25f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 650);
        }
    }
}