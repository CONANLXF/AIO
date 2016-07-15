using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms.VisualStyles;
using Arcane_Ryze.Modes;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Geometry = LeagueSharp.Common.Geometry;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

 namespace HoolaLucian
{
    public class Program
    {
        private static Menu Menu;
        public static Menu comboMenu, harassMenu, miscMenu, drawMenu, laneMenu, jungleMenu, ksMenu, autoMenu;
        private static AIHeroClient Player = ObjectManager.Player;
        private static HpBarIndicator Indicator = new HpBarIndicator();
        private static Spell Q, Q1, W, E, R;
        private static bool AAPassive;
        private static bool HEXQ => harassMenu["HEXQ"].Cast<CheckBox>().CurrentValue;
        private static bool KillstealQ => ksMenu["KillstealQ"].Cast<CheckBox>().CurrentValue;
        private static bool CQ => comboMenu["CQ"].Cast<CheckBox>().CurrentValue;
        private static bool CW => comboMenu["CW"].Cast<CheckBox>().CurrentValue;
        private static int CE => comboMenu["CE"].Cast<ComboBox>().CurrentValue;
        private static bool HQ => harassMenu["HQ"].Cast<CheckBox>().CurrentValue;
        private static bool HW => harassMenu["HW"].Cast<CheckBox>().CurrentValue;
        private static int HE => harassMenu["HE"].Cast<ComboBox>().CurrentValue;
        private static int HMinMana => harassMenu["HMinMana"].Cast<Slider>().CurrentValue;
        private static bool JQ => jungleMenu["JQ"].Cast<CheckBox>().CurrentValue;
        private static bool JW => jungleMenu["JW"].Cast<CheckBox>().CurrentValue;
        private static bool JE => jungleMenu["JE"].Cast<CheckBox>().CurrentValue;
        private static bool LHQ => laneMenu["LHQ"].Cast<CheckBox>().CurrentValue;
        private static int LQ => laneMenu["LQ"].Cast<Slider>().CurrentValue;
        private static bool LW => laneMenu["LW"].Cast<CheckBox>().CurrentValue;
        private static bool LE => laneMenu["LE"].Cast<CheckBox>().CurrentValue;
        private static int LMinMana => laneMenu["LMinMana"].Cast<Slider>().CurrentValue;
        private static bool Dind => drawMenu["Dind"].Cast<CheckBox>().CurrentValue;
        private static bool DEQ => drawMenu["DEQ"].Cast<CheckBox>().CurrentValue;
        private static bool DQ => drawMenu["DQ"].Cast<CheckBox>().CurrentValue;
        private static bool DW => drawMenu["DW"].Cast<CheckBox>().CurrentValue;
        private static bool DE => drawMenu["DE"].Cast<CheckBox>().CurrentValue;
        static bool AutoQ => autoMenu["AutoQ"].Cast<KeyBind>().CurrentValue;
        private static int MinMana => autoMenu["MinMana"].Cast<Slider>().CurrentValue;
        private static int HHMinMana => harassMenu["HHMinMana"].Cast<Slider>().CurrentValue;
        private static int Humanizer => miscMenu["Humanizer"].Cast<Slider>().CurrentValue;
        static bool ForceR => comboMenu["ForceR"].Cast<KeyBind>().CurrentValue;
        static bool LT => laneMenu["LT"].Cast<KeyBind>().CurrentValue;
        private static bool CY => comboMenu["CY"].Cast<CheckBox>().CurrentValue;

        public static void OnGameLoad()
        {
            if (Player.ChampionName != "Lucian") return;
            Chat.Print("Hoola Lucian - Loaded Successfully, Good Luck! :)");
            Q = new Spell(SpellSlot.Q, 675);
            Q1 = new Spell(SpellSlot.Q, 1200);
            W = new Spell(SpellSlot.W, 1200, DamageType.Magical);
            E = new Spell(SpellSlot.E, 475f);
            R = new Spell(SpellSlot.R, 1400);

            OnMenuLoad();

            Q.SetTargetted(0.25f, 1400f);
            Q1.SetSkillshot(0.5f, 50, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.30f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.2f, 110f, 2500, true, SkillshotType.SkillshotLine);

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Obj_AI_Base.OnSpellCast += OnDoCast;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnSpellCast += OnDoCastLC;
        }
        private static void OnMenuLoad()
        {
            Menu = MainMenu.AddMenu("Hoola Lucian", "hoolalucian");


            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("CQ", new CheckBox("Use Q"));
            comboMenu.Add("CW", new CheckBox("Use W"));
            comboMenu.Add("CE", new ComboBox("Use E Mode", 0, "Side", "Cursor", "Enemy", "Never"));
            comboMenu.Add("CY", new CheckBox("Use Yomuu's in R", false));
            comboMenu.Add("ForceR", new KeyBind("Force R On Target Selector", false, KeyBind.BindTypes.HoldActive, 'T'));


            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("HEXQ", new CheckBox("Use Extended Q"));
            harassMenu.Add("HMinMana", new Slider("Extended Q Min Mana (%)", 80, 0, 100));
            harassMenu.Add("HQ", new CheckBox("Use Q"));
            harassMenu.Add("HW", new CheckBox("Use W"));
            harassMenu.Add("HE", new ComboBox("Use E Mode", 0, "Side", "Cursor", "Enemy", "Never"));
            harassMenu.Add("HHMinMana", new Slider("Harass Min Mana (%)", 80, 0, 100));


            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("Humanizer", new Slider("Humanizer Delay", 5, 5, 300));
            miscMenu.Add("Nocolision", new CheckBox("Nocolision W"));


            laneMenu = Menu.AddSubMenu("LaneClear", "LaneClear");
            laneMenu.Add("LT", new KeyBind("Use Spell LaneClear (Toggle)", false, KeyBind.BindTypes.PressToggle, 'J'));
            laneMenu.Add("LHQ", new CheckBox("Use Extended Q For Harass"));
            laneMenu.Add("LQ", new Slider("Use Q (0 = Don't)", 0, 0, 5));
            laneMenu.Add("LW", new CheckBox("Use W"));
            laneMenu.Add("LE", new CheckBox("Use E"));
            laneMenu.Add("LMinMana", new Slider("Min Mana (%)", 80, 0, 100));


            jungleMenu = Menu.AddSubMenu("JungleClear", "JungleClear");
            jungleMenu.Add("JQ", new CheckBox("Use Q"));
            jungleMenu.Add("JW", new CheckBox("Use W"));
            jungleMenu.Add("JE", new CheckBox("Use E"));


            autoMenu = Menu.AddSubMenu("Auto", "Auto");
            autoMenu.Add("AutoQ", new KeyBind("Auto Extended Q (Toggle)", false, KeyBind.BindTypes.PressToggle, 'G'));
            autoMenu.Add("MinMana", new Slider("Min Mana (%)", 80, 0, 100));


            drawMenu = Menu.AddSubMenu("Draw", "Draw");
            drawMenu.Add("Dind", new CheckBox("Draw Damage Incidator"));
            drawMenu.Add("DEQ", new CheckBox("Draw Extended Q"));
            drawMenu.Add("DQ", new CheckBox("Draw Q"));
            drawMenu.Add("DW", new CheckBox("Draw W"));
            drawMenu.Add("DE", new CheckBox("Draw E"));


            ksMenu = Menu.AddSubMenu("killsteal", "killsteal");
            ksMenu.Add("KillstealQ", new CheckBox("Killsteal Q"));

        }

        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spellName = args.SData.Name;
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(spellName)) return;

            if (args.Target is AIHeroClient)
            {
                var target = (Obj_AI_Base)args.Target;
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                {
                    Utility.DelayAction.Add(Humanizer, () => OnDoCastDelayed(args));
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && target.IsValid)
                {
                    Utility.DelayAction.Add(Humanizer, () => OnDoCastDelayed(args));
                }
            }
            if (args.Target is Obj_AI_Minion)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && args.Target.IsValid)
                {
                    Utility.DelayAction.Add(Humanizer, () => OnDoCastDelayed(args));
                }
            }
        }
        private static void OnDoCastLC(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spellName = args.SData.Name;
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(spellName)) return;

            if (args.Target is Obj_AI_Minion)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && args.Target.IsValid)
                {
                    Utility.DelayAction.Add(Humanizer, () => OnDoCastDelayedLC(args));
                }
            }
        }

        static void killsteal()
        {
            if (KillstealQ && Q.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Q.GetDamage(target) && (!target.HasBuff("kindrednodeathbuff") && !target.HasBuff("Undying Rage") && !target.HasBuff("JudicatorIntervention")))
                        Q.Cast(target);
                }
            }
        }
        private static void OnDoCastDelayedLC(GameObjectProcessSpellCastEventArgs args)
        {
            AAPassive = false;
            if (args.Target is Obj_AI_Minion && args.Target.IsValid)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && Player.ManaPercent > LMinMana)
                {
                    var Minions = MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(Player), MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                    if (Minions[0].IsValid && Minions.Count != 0)
                    {
                        if (!LT) return;

                        if (E.IsReady() && !AAPassive && LE) E.Cast(Player.Position.LSExtend(Game.CursorPos, 70));
                        if (Q.IsReady() && (!E.IsReady() || (E.IsReady() && !LE)) && LQ != 0 && !AAPassive)
                        {
                            var QMinions = MinionManager.GetMinions(Q.Range);
                            var exminions = MinionManager.GetMinions(Q1.Range);
                            foreach (var Minion in QMinions)
                            {
                                var QHit = new Geometry.Polygon.Rectangle(Player.Position, Player.Position.LSExtend(Minion.Position, Q1.Range), Q1.Width);
                                if (exminions.Count(x => !QHit.IsOutside(x.Position.LSTo2D())) >= LQ)
                                {
                                    Q.Cast(Minion);
                                    break;
                                }
                            }
                        }
                        if ((!E.IsReady() || (E.IsReady() && !LE)) && (!Q.IsReady() || (Q.IsReady() && LQ == 0)) && LW && W.IsReady() && !AAPassive) W.Cast(Minions[0].Position);
                    }
                }
            }
        }
        public static Vector2 Deviation(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI / 180.0;
            Vector2 temp = Vector2.Subtract(point2, point1);
            Vector2 result = new Vector2(0);
            result.X = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4;
            result.Y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4;
            result = Vector2.Add(result, point1);
            return result;
        }
        private static void OnDoCastDelayed(GameObjectProcessSpellCastEventArgs args)
        {
            AAPassive = false;
            if (args.Target is AIHeroClient)
            {
                var target = (Obj_AI_Base)args.Target;
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                {
                    if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()) ItemData.Youmuus_Ghostblade.GetItem().Cast();
                    if (E.IsReady() && !AAPassive && CE == 0) E.Cast((Deviation(Player.Position.LSTo2D(), target.Position.LSTo2D(), 65).To3D()));
                    if (E.IsReady() && !AAPassive && CE == 1) E.Cast(Game.CursorPos);
                    if (E.IsReady() && !AAPassive && CE == 2) E.Cast(Player.Position.LSExtend(target.Position, 50));
                    if (Q.IsReady() && (!E.IsReady() || (E.IsReady() && CE == 3)) && CQ && !AAPassive) Q.Cast(target);
                    if ((!E.IsReady() || (E.IsReady() && CE == 3)) && (!Q.IsReady() || (Q.IsReady() && !CQ)) && CW && W.IsReady() && !AAPassive) W.Cast(target.Position);
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && target.IsValid)
                {
                    if (Player.ManaPercent < HHMinMana) return;

                    if (E.IsReady() && !AAPassive && HE == 0) E.Cast((Deviation(Player.Position.LSTo2D(), target.Position.LSTo2D(), 65).To3D()));
                    if (E.IsReady() && !AAPassive && HE == 1) E.Cast(Player.Position.LSExtend(Game.CursorPos, 50));
                    if (E.IsReady() && !AAPassive && HE == 2) E.Cast(Player.Position.LSExtend(target.Position, 50));
                    if (Q.IsReady() && (!E.IsReady() || (E.IsReady() && HE == 3)) && HQ && !AAPassive) Q.Cast(target);
                    if ((!E.IsReady() || (E.IsReady() && HE == 3)) && (!Q.IsReady() || (Q.IsReady() && !HQ)) && HW && W.IsReady() && !AAPassive) W.Cast(target.Position);
                }
            }
            if (args.Target is Obj_AI_Minion && args.Target.IsValid)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    var Mobs = MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(Player), MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                    if (Mobs[0].IsValid && Mobs.Count != 0)
                    {
                        if (E.IsReady() && !AAPassive && JE) E.Cast(Player.Position.LSExtend(Game.CursorPos, 70));
                        if (Q.IsReady() && (!E.IsReady() || (E.IsReady() && !JE)) && JQ && !AAPassive) Q.Cast(Mobs[0]);
                        if ((!E.IsReady() || (E.IsReady() && !JE)) && (!Q.IsReady() || (Q.IsReady() && !JQ)) && JW && W.IsReady() && !AAPassive) W.Cast(Mobs[0].Position);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Player.ManaPercent < HMinMana) return;

            if (Q.IsReady() && HEXQ)
            {
                var target = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
                var Minions = MinionManager.GetMinions(Q.Range);
                foreach (var Minion in Minions)
                {
                    var QHit = new Geometry.Polygon.Rectangle(Player.Position, Player.Position.LSExtend(Minion.Position, Q1.Range), Q1.Width);
                    var QPred = Q1.GetPrediction(target);
                    if (!QHit.IsOutside(QPred.UnitPosition.LSTo2D()) && QPred.Hitchance == HitChance.High)
                    {
                        Q.Cast(Minion);
                        break;
                    }
                }
            }
        }
        private static void LaneClear()
        {
            if (Player.ManaPercent < LMinMana) return;

            if (Q.IsReady() && LHQ)
            {
                var extarget = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
                var Minions = MinionManager.GetMinions(Q.Range);
                foreach (var Minion in Minions)
                {
                    var QHit = new Geometry.Polygon.Rectangle(Player.Position, Player.Position.LSExtend(Minion.Position, Q1.Range), Q1.Width);
                    var QPred = Q1.GetPrediction(extarget);
                    if (!QHit.IsOutside(QPred.UnitPosition.LSTo2D()) && QPred.Hitchance == HitChance.High)
                    {
                        Q.Cast(Minion);
                        break;
                    }
                }
            }
        }
        private static void AutoUseQ()
        {
            if (Q.IsReady() && AutoQ && Player.ManaPercent > MinMana)
            {
                var extarget = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
                var Minions = MinionManager.GetMinions(Q.Range);
                foreach (var Minion in Minions)
                {
                    var QHit = new Geometry.Polygon.Rectangle(Player.Position, Player.Position.LSExtend(Minion.Position, Q1.Range), Q1.Width);
                    var QPred = Q1.GetPrediction(extarget);
                    if (!QHit.IsOutside(QPred.UnitPosition.LSTo2D()) && QPred.Hitchance == HitChance.High)
                    {
                        Q.Cast(Minion);
                        break;
                    }
                }
            }
        }

        private static void UseRTarget()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (ForceR && R.IsReady() && target.IsValid && target is AIHeroClient && !Player.HasBuff("LucianR")) R.Cast(target.Position);
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            W.Collision = miscMenu["Nocolision"].Cast<CheckBox>().CurrentValue;
            AutoUseQ();

            if (ForceR) UseRTarget();
            killsteal();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) LaneClear();
        }

        

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E) AAPassive = true;
            if (args.Slot == SpellSlot.E) Orbwalker.ResetAutoAttack();
            if (args.Slot == SpellSlot.R && CY)
                ItemData.Youmuus_Ghostblade.GetItem().Cast();
        }

        private static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                if (E.IsReady()) damage = damage + (float)Player.LSGetAutoAttackDamage(enemy) * 2;
                if (W.IsReady()) damage = damage + W.GetDamage(enemy) + (float)Player.LSGetAutoAttackDamage(enemy);
                if (Q.IsReady())
                {
                    damage = damage + Q.GetDamage(enemy) + (float)Player.LSGetAutoAttackDamage(enemy);
                }
                damage = damage + (float)Player.LSGetAutoAttackDamage(enemy);

                return damage;
            }
            return 0;
        }

        private static void OnDraw(EventArgs args)
        {
            if (DEQ) Render.Circle.DrawCircle(Player.Position, Q1.Range, Q.IsReady() ? Color.LimeGreen : Color.IndianRed);
            if (DQ) Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.LimeGreen : Color.IndianRed);
            if (DW) Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.LimeGreen : Color.IndianRed);
            if (DE) Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.LimeGreen : Color.IndianRed);
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Dind)
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
                {
                    Indicator.unit = enemy;
                    Indicator.drawDmg(getComboDamage(enemy), new ColorBGRA(255, 204, 0, 160));

                }
            }
        }
    }
}