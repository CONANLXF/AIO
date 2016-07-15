#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Syndra
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using System.Drawing;

    using Color = SharpDX.Color;

    internal static class Program
    {
        public const string ChampionName = "Syndra";
        //Spells
        public static List<LeagueSharp.Common.Spell> SpellList = new List<LeagueSharp.Common.Spell>();

        public static LeagueSharp.Common.Spell Q;

        public static LeagueSharp.Common.Spell W;

        public static LeagueSharp.Common.Spell E;

        public static LeagueSharp.Common.Spell Eq;

        public static LeagueSharp.Common.Spell R;

        public static SpellSlot IgniteSlot;

        //Menu
        public static Menu Config, DrawMenu, menuKeys, menuCombo, menuHarass, menuFarm, menuJungle, menuMisc;

        private static int qeComboT;

        private static int weComboT;

        public static AIHeroClient Player;

        public static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.CharData.BaseSkinName != ChampionName) return;

            //Create the spells
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 790);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 925);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 700);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 675);
            Eq = new LeagueSharp.Common.Spell(SpellSlot.Q, Q.Range + 500);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");

            Q.SetSkillshot(0.6f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, (float)(45 * 0.5), 2500f, false, SkillshotType.SkillshotCircle);
            Eq.SetSkillshot(float.MaxValue, 55f, 2000f, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            //Create the menu
            Config = MainMenu.AddMenu(ChampionName, ChampionName);

            menuKeys = Config.AddSubMenu("Keys", "Keys");
            {
                menuKeys.Add("Key.HarassT", new KeyBind("Harass (toggle)!", false, KeyBind.BindTypes.PressToggle, 'Y'));
                menuKeys.Add("Key.InstantQE", new KeyBind("Instant Q-E to Enemy", false, KeyBind.BindTypes.HoldActive, 'T'));
            }

            menuCombo = Config.AddSubMenu("Combo", "Combo");
            {
                menuCombo.Add("UseQCombo", new CheckBox("Use Q"));
                menuCombo.Add("UseWCombo", new CheckBox("Use W"));
                menuCombo.Add("UseECombo", new CheckBox("Use E"));
                menuCombo.Add("UseQECombo", new CheckBox("Use QE"));
                menuCombo.Add("UseRCombo", new CheckBox("Use R"));
                menuCombo.Add("UseIgniteCombo", new CheckBox("Use Ignite"));
            }

            menuHarass = Config.AddSubMenu("Harass", "Harass");
            {
                menuHarass.Add("UseQHarass", new CheckBox("Use Q"));
                menuHarass.Add("UseWHarass", new CheckBox("Use W", false));
                menuHarass.Add("UseEHarass", new CheckBox("Use E", false));
                menuHarass.Add("UseQEHarass", new CheckBox("Use QE", false));
                menuHarass.Add("Harass.Mana", new Slider("Don't harass if mana < %", 0));
            }

            menuFarm = Config.AddSubMenu("Lane Farm", "Farm");
            {
                menuFarm.Add("EnabledFarm", new CheckBox("Enable! (On/Off: Mouse Scroll)"));
                menuFarm.Add("UseQFarm", new ComboBox("Use Q", 2, "Last Hit", "LaneClear", "Both", "No"));
                menuFarm.Add("UseWFarm", new ComboBox("Use W", 1, "Last Hit", "LaneClear", "Both", "No"));
                menuFarm.Add("Lane.Mana", new Slider("Don't harass if mana < %", 0));
            }

            menuJungle = Config.AddSubMenu("Jungle Farm", "JungleFarm");
            {
                menuJungle.Add("UseQJFarm", new CheckBox("Use Q"));
                menuJungle.Add("UseWJFarm", new CheckBox("Use W"));
                menuJungle.Add("UseEJFarm", new CheckBox("Use E"));
            }

            menuMisc = Config.AddSubMenu("Misc", "Misc");
            {
                menuMisc.Add("InterruptSpells", new CheckBox("Interrupt spells"));
                menuMisc.Add("CastQE", new KeyBind("QE closest to cursor", false, KeyBind.BindTypes.HoldActive, 'T'));

                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                    menuMisc.Add("DontUlt" + enemy.CharData.BaseSkinName, new CheckBox("Don't Ult : " + enemy.CharData.BaseSkinName, false));
            }


            DrawMenu = Config.AddSubMenu("Drawings", "Drawings");
            {
                DrawMenu.Add("QRange", new CheckBox("Q range", false));//.SetValue(new Circle(false, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
                DrawMenu.Add("WRange", new CheckBox("W range"));//.SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
                DrawMenu.Add("ERange", new CheckBox("E range", false));//.SetValue(new Circle(false, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
                DrawMenu.Add("RRange", new CheckBox("R range", false));//.SetValue(new Circle(false, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
                DrawMenu.Add("QERange", new CheckBox("QE range"));//.SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 255))));

                ManaBarIndicator.Initialize();
            }

            //Add the events we are going to use:
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnWndProc += Game_OnWndProc;
            LSEvents.BeforeAttack += Orbwalking_BeforeAttack;

            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != 0x20a) return;

            if (ObjectManager.Player.InShop() || ObjectManager.Player.InFountain())
                return;

            menuFarm["EnabledFarm"].Cast<CheckBox>().CurrentValue = !menuFarm["EnabledFarm"].Cast<CheckBox>().CurrentValue;
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!menuMisc["InterruptSpells"].Cast<CheckBox>().CurrentValue) return;

            if (Player.LSDistance(sender) < E.Range && E.IsReady())
            {
                Q.Cast(sender.ServerPosition);
                E.Cast(sender.ServerPosition);
            }
            else if (Player.LSDistance(sender) < Eq.Range && E.IsReady() && Q.IsReady())
            {
                UseQe(sender);
            }
        }
        
        // ReSharper disable once InconsistentNaming
        private static void Orbwalking_BeforeAttack(BeforeAttackArgs args)
        {
            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                args.Process = !(Q.IsReady() || W.IsReady());
            }
        }

        private static void InstantQe2Enemy()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var t = TargetSelector.GetTarget(Eq.Range, DamageType.Magical);
            if (t.LSIsValidTarget() && E.IsReady() && Q.IsReady())
            {
                UseQe(t);
            }
        }

        private static void Combo()
        {
            UseSpells(
                menuCombo["UseQCombo"].Cast<CheckBox>().CurrentValue,
                menuCombo["UseWCombo"].Cast<CheckBox>().CurrentValue,
                menuCombo["UseECombo"].Cast<CheckBox>().CurrentValue,
                menuCombo["UseRCombo"].Cast<CheckBox>().CurrentValue,
                menuCombo["UseQECombo"].Cast<CheckBox>().CurrentValue,
                menuCombo["UseIgniteCombo"].Cast<CheckBox>().CurrentValue,
                false);
        }

        private static void Harass()
        {
            if (Player.ManaPercent < menuHarass["Harass.Mana"].Cast<Slider>().CurrentValue)
            {
                return;
            }

            UseSpells(
                menuHarass["UseQHarass"].Cast<CheckBox>().CurrentValue,
                menuHarass["UseWHarass"].Cast<CheckBox>().CurrentValue,
                menuHarass["UseEHarass"].Cast<CheckBox>().CurrentValue,
                false,
                menuHarass["UseQEHarass"].Cast<CheckBox>().CurrentValue,
                false,
                true);
        }

        private static void UseE(Obj_AI_Base enemy)
        {
            foreach (var orb in OrbManager.GetOrbs(true))
                if (Player.LSDistance(orb) < E.Range + 100)
                {
                    var startPoint = orb.LSTo2D().LSExtend(Player.ServerPosition.LSTo2D(), 100);
                    var endPoint = Player.ServerPosition.LSTo2D()
                        .LSExtend(orb.LSTo2D(), Player.LSDistance(orb) > 200 ? 1300 : 1000);
                    Eq.Delay = E.Delay + Player.LSDistance(orb) / E.Speed;
                    Eq.From = orb;
                    var enemyPred = Eq.GetPrediction(enemy);
                    if (enemyPred.Hitchance >= HitChance.High
                        && enemyPred.UnitPosition.LSTo2D().LSDistance(startPoint, endPoint, false)
                        < Eq.Width + enemy.BoundingRadius)
                    {
                        E.Cast(orb, true);
                        W.LastCastAttemptT = Utils.TickCount;
                        return;
                    }
                }
        }

        private static void UseQe(Obj_AI_Base enemy)
        {
            Eq.Delay = E.Delay + Q.Range / E.Speed;
            Eq.From = Player.ServerPosition.LSTo2D().LSExtend(enemy.ServerPosition.LSTo2D(), Q.Range).To3D();

            var prediction = Eq.GetPrediction(enemy);
            if (prediction.Hitchance >= HitChance.High)
            {
                Q.Cast(Player.ServerPosition.LSTo2D().LSExtend(prediction.CastPosition.LSTo2D(), Q.Range - 100));
                qeComboT = Utils.TickCount;
                W.LastCastAttemptT = Utils.TickCount;
            }
        }

        private static Vector3 GetGrabableObjectPos(bool onlyOrbs)
        {
            if (!onlyOrbs)
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.LSIsValidTarget(W.Range)))
                {
                    return minion.ServerPosition;
                }
            }
            return OrbManager.GetOrbToGrab((int)W.Range);
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;
            damage += Q.IsReady(420) ? Q.GetDamage(enemy) : 0;
            damage += W.IsReady() ? W.GetDamage(enemy) : 0;
            damage += E.IsReady() ? E.GetDamage(enemy) : 0;

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            }

            if (R.IsReady())
            {
                damage += Math.Min(7, Player.Spellbook.GetSpell(SpellSlot.R).Ammo) * Player.LSGetSpellDamage(enemy, SpellSlot.R, 1);
            }
            return (float)damage;
        }

        private static void UseSpells(bool useQ, bool useW, bool useE, bool useR, bool useQe,
            bool useIgnite, bool isHarass)
        {
            var qTarget = TargetSelector.GetTarget(Q.Range + (isHarass ? Q.Width / 3 : Q.Width), DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range + W.Width, DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            var qeTarget = TargetSelector.GetTarget(Eq.Range, DamageType.Magical);
            var comboDamage = rTarget != null ? GetComboDamage(rTarget) : 0;

            //Q
            if (qTarget != null && useQ)
            {
                var qPred = Q.GetPrediction(qTarget);
                if (qPred.Hitchance >= HitChance.High)
                    Q.Cast(qPred.CastPosition);
            }

            //E
            if (Utils.TickCount - W.LastCastAttemptT > Game.Ping + 150 && E.IsReady() && useE)
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (enemy.LSIsValidTarget(Eq.Range))
                    {
                        UseE(enemy);
                    }
                }
            }


            //W
            if (useW)
            {
                if (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1 && W.IsReady() && qeTarget != null)
                {
                    var gObjectPos = GetGrabableObjectPos(wTarget == null);

                    if (gObjectPos.LSTo2D().IsValid() && Utils.TickCount - W.LastCastAttemptT > Game.Ping + 300
                        && Utils.TickCount - E.LastCastAttemptT > Game.Ping + 600)
                    {
                        W.Cast(gObjectPos);
                        W.LastCastAttemptT = Utils.TickCount;
                    }
                }
                else if (wTarget != null && Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1 && W.IsReady()
                         && Utils.TickCount - W.LastCastAttemptT > Game.Ping + 100)
                {
                    if (OrbManager.WObject(false) != null)
                    {
                        W.From = OrbManager.WObject(false).ServerPosition;
                        W.Cast(wTarget, false, true);
                    }
                }
            }


            if (rTarget != null && useR)
            {
                useR = (menuMisc["DontUlt" + rTarget.CharData.BaseSkinName] != null && menuMisc["DontUlt" + rTarget.CharData.BaseSkinName].Cast<CheckBox>().CurrentValue == false);
            }


            if (rTarget != null && useR && R.IsReady() && comboDamage > rTarget.Health && !rTarget.IsZombie && !Q.IsReady())
            {
                R.Cast(rTarget);
            }

            //Ignite
            if (rTarget != null && useIgnite && IgniteSlot != SpellSlot.Unknown
                && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (comboDamage > rTarget.Health)
                {
                    Player.Spellbook.CastSpell(IgniteSlot, rTarget);
                }
            }

            //QE
            if (wTarget == null && qeTarget != null && Q.IsReady() && E.IsReady() && useQe)
            {
                UseQe(qeTarget);
            }

            //WE
            if (wTarget == null && qeTarget != null && E.IsReady() && useE && OrbManager.WObject(true) != null)
            {
                Eq.Delay = E.Delay + Q.Range / W.Speed;
                Eq.From = Player.ServerPosition.LSTo2D().LSExtend(qeTarget.ServerPosition.LSTo2D(), Q.Range).To3D();
                var prediction = Eq.GetPrediction(qeTarget);
                if (prediction.Hitchance >= HitChance.High)
                {
                    W.Cast(Player.ServerPosition.LSTo2D().LSExtend(prediction.CastPosition.LSTo2D(), Q.Range - 100));
                    weComboT = Utils.TickCount;
                }
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Utils.TickCount - qeComboT < 500 && args.SData.Name.Equals("SyndraQ", StringComparison.InvariantCultureIgnoreCase))
            {
                W.LastCastAttemptT = Utils.TickCount + 400;
                E.Cast(args.End, true);
            }

            if (Utils.TickCount - weComboT < 500
                && (args.SData.Name.Equals("SyndraW", StringComparison.InvariantCultureIgnoreCase) || args.SData.Name.Equals("SyndraWCast", StringComparison.InvariantCultureIgnoreCase)))
            {
                W.LastCastAttemptT = Utils.TickCount + 400;
                E.Cast(args.End, true);
            }
        }

        private static void Farm(bool laneClear)
        {
            if (!menuFarm["EnabledFarm"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (Player.ManaPercent < menuFarm["Lane.Mana"].Cast<Slider>().CurrentValue)
            {
                return;
            }
            if (!PortAIO.OrbwalkerManager.CanMove(0))
            {
                return;
            }

            var rangedMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width + 30, MinionTypes.Ranged);
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width + 30);
            var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 30, MinionTypes.Ranged);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 30);

            var useQi = menuFarm["UseQFarm"].Cast<ComboBox>().CurrentValue;
            var useWi = menuFarm["UseWFarm"].Cast<ComboBox>().CurrentValue;
            var useQ = (laneClear && (useQi == 1 || useQi == 2)) || (!laneClear && (useQi == 0 || useQi == 2));
            var useW = (laneClear && (useWi == 1 || useWi == 2)) || (!laneClear && (useWi == 0 || useWi == 2));

            if (useQ && Q.IsReady())
                if (laneClear)
                {
                    var fl1 = Q.GetCircularFarmLocation(rangedMinionsQ, Q.Width);
                    var fl2 = Q.GetCircularFarmLocation(allMinionsQ, Q.Width);

                    if (fl1.MinionsHit >= 3)
                    {
                        Q.Cast(fl1.Position);
                    }

                    else if (fl2.MinionsHit >= 2 || allMinionsQ.Count == 1)
                    {
                        Q.Cast(fl2.Position);
                    }
                }
                else
                {
                    foreach (var minion in allMinionsQ.Where(minion => !Orbwalking.InAutoAttackRange(minion) && minion.Health < 0.75 * Q.GetDamage(minion)))
                    {
                        Q.Cast(minion);
                    }
                }

            if (useW && W.IsReady() && allMinionsW.Count > 3)
            {
                if (laneClear)
                {
                    if (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                    {
                        //WObject
                        var gObjectPos = GetGrabableObjectPos(false);

                        if (gObjectPos.LSTo2D().IsValid() && Utils.TickCount - W.LastCastAttemptT > Game.Ping + 150)
                        {
                            W.Cast(gObjectPos);
                        }
                    }
                    else if (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1)
                    {
                        var fl1 = Q.GetCircularFarmLocation(rangedMinionsW, W.Width);
                        var fl2 = Q.GetCircularFarmLocation(allMinionsW, W.Width);

                        if (fl1.MinionsHit >= 3 && W.IsInRange(fl1.Position.To3D()))
                        {
                            W.Cast(fl1.Position);
                        }

                        else if (fl2.MinionsHit >= 1 && W.IsInRange(fl2.Position.To3D()) && fl1.MinionsHit <= 2)
                        {
                            W.Cast(fl2.Position);
                        }
                    }
                }
            }
        }

        private static void JungleFarm()
        {
            var useQ = menuJungle["UseQJFarm"].Cast<CheckBox>().CurrentValue;
            var useW = menuJungle["UseWJFarm"].Cast<CheckBox>().CurrentValue;
            var useE = menuJungle["UseEJFarm"].Cast<CheckBox>().CurrentValue;

            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count > 0)
            {
                var mob = mobs[0];

                if (Q.IsReady() && useQ)
                {
                    Q.Cast(mob);
                }

                if (W.IsReady() && useW && Utils.TickCount - Q.LastCastAttemptT > 800)
                {
                    W.Cast(mob);
                }

                if (useE && E.IsReady())
                {
                    E.Cast(mob);
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            //Update the R range
            R.Range = R.Level == 3 ? 750 : 675;

            if (menuMisc["CastQE"].Cast<KeyBind>().CurrentValue && E.IsReady() && Q.IsReady())
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies
                            .Where(
                                enemy =>
                                enemy.LSIsValidTarget(Eq.Range) && Game.CursorPos.LSDistance(enemy.ServerPosition) < 300))
                {
                    UseQe(enemy);
                }
            }

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo();
            }
            else
            {
                if (PortAIO.OrbwalkerManager.isHarassActive || menuKeys["Key.HarassT"].Cast<KeyBind>().CurrentValue)
                    Harass();

                var lc = PortAIO.OrbwalkerManager.isLaneClearActive;
                if (lc || PortAIO.OrbwalkerManager.isLastHitActive)
                {
                    Farm(lc);

                    if (!PortAIO.OrbwalkerManager.isLastHitActive)
                        JungleFarm();
                }
            }

            if (menuKeys["Key.InstantQE"].Cast<KeyBind>().CurrentValue)
            {
                InstantQe2Enemy();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            //Draw the ranges of the spells.
            var menuItem = DrawMenu["QERange"].Cast<CheckBox>().CurrentValue;
            if (menuItem) Render.Circle.DrawCircle(Player.Position, Eq.Range, System.Drawing.Color.FromArgb(100, 255, 0, 255));

            foreach (var spell in SpellList)
            {
                menuItem = DrawMenu[spell.Slot + "Range"].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                {
                    Render.Circle.DrawCircle(Player.Position, spell.Range, System.Drawing.Color.FromArgb(100, 255, 0, 255));
                }
            }

            if (OrbManager.WObject(false) != null)
                Render.Circle.DrawCircle(OrbManager.WObject(false).Position, 100, System.Drawing.Color.White);
        }
    }
}