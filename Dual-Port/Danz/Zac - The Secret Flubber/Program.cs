using System;
using System.Collections;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Collections.Generic;
using System.Threading;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;

using TargetSelector = PortAIO.TSManager; namespace Zac_The_Secret_Flubber
{
    class Program
    {

        private const string Champion = "Zac";

        private static Spell Q, W, E, R;

        private static List<Spell> SpellList = new List<Spell>();

        private static Menu Config;

        public static Menu comboMenu, harassMenu, drawMenu, jungleMenu, miscMenu;

        public static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        private static float GetComboDamage(Obj_AI_Base Target)
        {
            var ComboDamage = 0d;

            if (Q.IsReady())
                ComboDamage += Player.LSGetSpellDamage(Target, SpellSlot.Q);


            if (E.IsReady())
                ComboDamage += Player.LSGetSpellDamage(Target, SpellSlot.E);

            if (R.IsReady())
                ComboDamage += Player.LSGetSpellDamage(Target, SpellSlot.R);


            return (float)ComboDamage;
        }


        public static void Game_OnGameLoad()
        {
            Chat.Print("Zac - The Secret Flubber by DanZ and Drunkenninja");
            if (ObjectManager.Player.BaseSkinName != Champion) return;

            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 1550);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(550, 120, int.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(1550, 250, 1500, true, SkillshotType.SkillshotCone);
            E.SetCharged("ZacE", "ZacE", 1150, 1550, 1.5f);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);


            Config = MainMenu.AddMenu("Zac", "Zac_menu");


            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q", true));
            comboMenu.Add("UseWCombo", new CheckBox("Use W", true));
            comboMenu.Add("UseRCombo", new CheckBox("Use R", true));


            harassMenu = Config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q", true));
            harassMenu.Add("UseWHarass", new CheckBox("Use W", true));


            jungleMenu = Config.AddSubMenu("Jungle Clear", "JGClear");
            jungleMenu.Add("QJGClear", new CheckBox("Use Q", true));
            jungleMenu.Add("WJGClear", new CheckBox("Use W", true));


            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("KSQ", new CheckBox("KS with Q", true));


            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("drawEnable", new CheckBox("Enable Drawing", true));
            drawMenu.Add("drawQ", new CheckBox("Draw Q", true));
            drawMenu.Add("drawW", new CheckBox("Draw W", true));
            drawMenu.Add("drawE", new CheckBox("Draw E", true));
            drawMenu.Add("drawR", new CheckBox("Draw R", true));


            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;

        }


        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null) return;

            var comboDamage = GetComboDamage(target);
            var useQ = getCheckBoxItem(comboMenu, "UseQCombo");
            var useW = getCheckBoxItem(comboMenu, "UseWCombo");
            var useR = getCheckBoxItem(comboMenu, "UseRCombo");

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (W.IsReady() && useW && target.IsValidTarget(W.Range))
            {
                W.Cast();
            }
            if (R.IsReady() && useR)
                if (Q.IsReady())
                {
                    return;
                }
                else
                {
                    R.Cast();
                }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null) return;

            var comboDamage = GetComboDamage(target);
            var useQ = getCheckBoxItem(harassMenu, "UseQHarass");
            var useW = getCheckBoxItem(harassMenu, "UseWHarass");


            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (W.IsReady() && useW && target.IsValidTarget(W.Range))
            {
                W.Cast(target);
            }
        }

        private static void KSQ()
        {
            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(unit => unit.LSIsValidTarget(Q.Range)))
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target == null) return;

                var prediction = Q.GetPrediction(target);

                if (Q.IsReady())
                {

                    if (target.Health < GetQDamage(target))
                    {

                        Q.Cast(prediction.CastPosition);


                    }
                }
            }
        }
        private static float GetQDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (Q.IsReady()) damage += Player.LSGetSpellDamage(enemy, SpellSlot.Q);

            return (float)damage * 2;
        }
        private static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (Q.IsReady() && getCheckBoxItem(jungleMenu, "WJGClear"))
                {
                    Q.Cast(mob);
                }

                if (W.IsReady() && getCheckBoxItem(jungleMenu, "WJGClear"))
                {
                    W.Cast();
                }


            }
        }

        // private static void LaneClear()






        private static void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "drawEnable"))
            {
                if (getCheckBoxItem(drawMenu, "drawQ"))
                {

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Aqua, 1);
                }

                if (getCheckBoxItem(drawMenu, "drawW"))
                {

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Aqua, 1);
                }

                if (getCheckBoxItem(drawMenu, "drawE"))
                {

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Aqua, 1);
                }
                if (getCheckBoxItem(drawMenu, "drawR"))
                {

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Aqua, 1);
                }
            }
        }
        
        private static void OnGameUpdate(EventArgs args)
        {

            if (E.IsCharging)
            {
                PortAIO.OrbwalkerManager.SetMovement(false);
            }


            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo();
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                JungleClear();
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Harass();
            }
        }
    }
}

