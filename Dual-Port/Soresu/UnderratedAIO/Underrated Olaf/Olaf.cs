using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using SPrediction;
using UnderratedAIO.Helpers;
using UnderratedAIO.Helpers.SkillShot;
using Environment = UnderratedAIO.Helpers.Environment;
using Prediction = LeagueSharp.Common.Prediction;
using Utility = LeagueSharp.Common.Utility;
using Damage = LeagueSharp.Common.Damage;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;

 namespace UnderratedAIO.Champions
{
    internal class Olaf
    {
        public static Menu config;
        public static Menu comboMenu, harassMenu, miscMenu, drawMenu, laneMenu;
        public static Spell Q, W, E, R;
        public static Vector3 lastQpos, lastQCast;
        public static readonly AIHeroClient player = ObjectManager.Player;


        public static void OnLoad()
        {
            InitOlaf();
            InitMenu();
            Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Olaf</font>");
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnDelete += Obj_AI_Base_OnDelete;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Console.WriteLine(Game.IP);
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("olaf_axe_totem_team_id_green.troy") &&
                lastQCast.LSDistance(sender.Position) < 700)
            {
                lastQpos = sender.Position;
            }
        }


        private static void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("olaf_axe_totem_team_id_green.troy") &&
                lastQpos.LSDistance(sender.Position) < 150)
            {
                lastQpos = Vector3.Zero;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Slot == SpellSlot.Q)
            {
                var pos = player.LSDistance(args.End) > 400 ? args.End : player.Position.LSExtend(args.End, 400);
                lastQCast = Environment.Map.ClosestWall(player.Position, pos);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (lastQpos.IsValid() && getCheckBoxItem(drawMenu, "drawaxe"))
            {
                Render.Circle.DrawCircle(
                    lastQpos, getSliderItem(miscMenu, "gotoAxeMaxDist"), Color.Cyan, 5);
            }
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawqq"), Q.Range, Color.FromArgb(180, 255, 222, 5));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawee"), E.Range, Color.FromArgb(180, 255, 222, 5));
            Utility.HpBarDamageIndicator.Enabled = getCheckBoxItem(drawMenu, "drawcombo");
        }
        
        private static void Game_OnUpdate(EventArgs args)
        {
            Orbwalker.MoveTo(Vector3.Zero);

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
        }

        private static void Clear()
        {
            var minis = MinionManager.GetMinions(325f, MinionTypes.All, MinionTeam.NotAlly);
            var killableWithE =
                minis.Where(m => m.Health < E.GetDamage(m)).OrderByDescending(m => m.MaxHealth).FirstOrDefault();
            if (getCheckBoxItem(laneMenu, "useeLC") && E.IsReady() && killableWithE != null &&
                (!player.Spellbook.IsAutoAttacking || killableWithE.MaxHealth > 2000))
            {
                E.Cast(killableWithE);
            }
            if (getCheckBoxItem(laneMenu, "gotoAxeLC"))
            {
                GotoAxe(Game.CursorPos);
            }
            float perc = getSliderItem(laneMenu, "minmana") / 100f;
            if (player.Mana < player.MaxMana * perc || player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            if (getCheckBoxItem(laneMenu, "useqLC") && Q.IsReady())
            {
                var minisForQ = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                var farmLocation = Q.GetLineFarmLocation(minisForQ);
                Obj_AI_Base targetMini = null;
                if (getSliderItem(laneMenu, "qMinHit") <= farmLocation.MinionsHit)
                {
                    targetMini =
                        minisForQ.Where(m => Q.CountHits(minisForQ, m.Position) >= farmLocation.MinionsHit)
                            .OrderBy(m => m.LSDistance(player))
                            .FirstOrDefault();
                }
                if (targetMini == null)
                {
                    targetMini =
                        minisForQ.Where(m => minisForQ.Where(b => b.LSDistance(m) < Q.Width).Sum(b => b.Health) > 700)
                            .OrderByDescending(b => Q.CountHits(minisForQ, b.Position))
                            .FirstOrDefault();
                }
                if (targetMini != null)
                {
                    Q.Cast(targetMini.Position);
                }
            }
            if (getCheckBoxItem(laneMenu, "usewLC") && W.IsReady())
            {
                if (minis.Sum(m => m.Health) > 750 || minis.Count > 3 || player.HealthPercent < 50)
                {
                    W.Cast();
                }
            }
        }

        private static void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (getCheckBoxItem(harassMenu, "useeH") && target != null && E.CanCast(target) &&
                !player.Spellbook.IsAutoAttacking)
            {
                E.Cast(target);
            }
            if (getCheckBoxItem(harassMenu, "gotoAxeH") && target != null)
            {
                GotoAxe(target.Position);
            }
            float perc = getSliderItem(harassMenu, "minmanaH") / 100f;
            if (player.Mana < player.MaxMana * perc || player.Spellbook.IsAutoAttacking || target == null)
            {
                return;
            }
            if (getCheckBoxItem(harassMenu, "useqH") && Q.IsReady())
            {
                CastQ(target);
            }
        }

        private static void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (player.Spellbook.IsAutoAttacking || target == null)
            {
                return;
            }
            if (getCheckBoxItem(comboMenu, "useq") && Q.CanCast(target))
            {
                CastQ(target);
            }
            if (getCheckBoxItem(comboMenu, "usee") && E.CanCast(target) &&
                (((E.GetDamage(target) > target.Health) || player.HealthPercent > 25) ||
                 Program.IncDamages.GetAllyData(player.NetworkId).IsAboutToDie))
            {
                E.Cast(target);
            }
            if (getCheckBoxItem(comboMenu, "usew") && W.IsReady() &&
                player.LSDistance(target) < Orbwalking.GetRealAutoAttackRange(target) + 50)
            {
                W.Cast();
            }
            if (getCheckBoxItem(comboMenu, "userCCed") && R.IsReady() && CombatHelper.IsCCed(player))
            {
                R.Cast();
            }
            if (getCheckBoxItem(comboMenu, "userbeforeCCed") && R.IsReady() &&
                Program.IncDamages.GetAllyData(player.NetworkId).AnyCC)
            {
                R.Cast();
            }
            if (getCheckBoxItem(comboMenu, "gotoAxe"))
            {
                GotoAxe(target.Position);
            }

            var ignitedmg = (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(comboMenu, "useIgnite") &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 1000) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) &&
                target.LSDistance(player) > Orbwalking.GetRealAutoAttackRange(target) + 25)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private static void GotoAxe(Vector3 target)
        {
            if (!lastQpos.IsValid())
            {
                return;
            }
            var maxDist = getSliderItem(miscMenu, "gotoAxeMaxDist");
            var orig = player.LSDistance(target);
            var ext = player.LSDistance(lastQpos) + lastQpos.LSDistance(target);
            if (player.LSDistance(lastQpos) < maxDist && !lastQpos.UnderTurret(true))
            //ext - orig < maxDist && Orbwalking.CanMove(100)
            {
                Orbwalker.MoveTo(lastQpos);
                //player.IssueOrder(GameObjectOrder.MoveTo, lastQpos);
            }
        }

        private static void CastQ(AIHeroClient target)
        {
            var ext = 0;
            if (player.LSDistance(target.ServerPosition) > 400)
            {
                ext = 100;
            }
            var pred = Q.GetPrediction(target, true);
            var pos = player.Position.LSExtend(pred.CastPosition, player.LSDistance(pred.CastPosition) + ext);
            if (pred.CastPosition.IsValid() && target.LSDistance(pos) < player.LSDistance(target) &&
                pred.Hitchance >= HitChance.Medium)
            {
                //Console.WriteLine(2 + " - " + " - " + pred.Hitchance);
                Q.Cast(pos);
            }
        }

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Olaf", "olaf");

            drawMenu = config.AddSubMenu("Drawings", "dsettings");
            drawMenu.Add("drawqq", new CheckBox("Draw Q range")); //Color.FromArgb(180, 255, 222, 5)
            drawMenu.Add("drawee", new CheckBox("Draw E range")); //Color.FromArgb(180, 255, 222, 5)
            drawMenu.Add("drawaxe", new CheckBox("Draw Axe position"));
            drawMenu.Add("drawcombo", new CheckBox("Draw combo damage"));


            comboMenu = config.AddSubMenu("Combo", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q", true));
            comboMenu.Add("gotoAxe", new CheckBox("Catch axe", true));
            comboMenu.Add("usew", new CheckBox("Use W", true));
            comboMenu.Add("usee", new CheckBox("Use E", true));
            comboMenu.Add("userCCed", new CheckBox("Use on CC", true));
            comboMenu.Add("userbeforeCCed", new CheckBox("Use before CC", true));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite", true));


            harassMenu = config.AddSubMenu("Harass", "Hsettings");
            harassMenu.Add("useqH", new CheckBox("Use Q", true));
            harassMenu.Add("gotoAxeH", new CheckBox("Catch axe", true));
            harassMenu.Add("useeH", new CheckBox("Use E", true));
            harassMenu.Add("minmanaH", new Slider("Keep X% mana", 1, 0, 100));


            laneMenu = config.AddSubMenu("LaneClear", "Lcsettings");
            laneMenu.Add("useqLC", new CheckBox("Use Q", true));
            laneMenu.Add("qMinHit", new Slider("Catch axe", 3, 1, 6));
            laneMenu.Add("gotoAxeLC", new CheckBox("Catch axe", true));
            laneMenu.Add("usewLC", new CheckBox("Use W", true));
            laneMenu.Add("useeLC", new CheckBox("Use E", false));
            laneMenu.Add("minmana", new Slider("Keep X% mana", 20, 1, 100));


            miscMenu = config.AddSubMenu("Misc", "Msettings");
            miscMenu.Add("gotoAxeMaxDist", new Slider("Max dist to catch axe", 450, 200, 600));

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
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (E.IsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.E);
            }
            //damage += ItemHandler.GetItemsDamage(target);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float)damage;
        }

        private static void InitOlaf()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            Q.SetSkillshot(0.25f, 105, 1600, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R);
        }
    }
}