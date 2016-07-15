using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Color = SharpDX.Color;

using TargetSelector = PortAIO.TSManager; namespace KarthusSharp
{
    /*
    * LaneClear:
    * - allow AA on Tower, Ward (don't Q wards)
    * - improve somehow
    * - https://github.com/trus/L-/blob/master/TRUSt%20in%20my%20Karthus/Program.cs
    * 
    * Ult KS:
    * - don't KS anymore if enemy is recalling and would arrive base before ult went through (have to include BaseUlt functionality)
    * - It also ulted while taking hits from enemy tower.
    * 
    * Misc:
    * - add don't use spells until lvl x etc.
    * - Recode
    * - Onspellcast if q farm enabled, disable AA in beforeattack and start timer that lasts casttime

    *  10.9.2015
    * - Added Ulti damage indicator
    * - Added toggle harass Q + permashow status
    * - Added ping notification
    * - Added ulti ks permashow status
    * */

    internal class Karthus
    {
        private readonly Menu _menu;
        public static Menu comboMenu, harassMenu, farmMenu, miscMenu, drawMenu, notifyMenu;

        private readonly Spell _spellQ;
        private readonly Spell _spellW;
        private readonly Spell _spellE;
        private readonly Spell _spellR;

        private const float SpellQWidth = 160f;
        private const float SpellWWidth = 160f;

        private bool _comboE;
        private static Vector2 PingLocation;
        private static int LastPingT = 0;

        public Karthus()
        {
            if (ObjectManager.Player.ChampionName != "Karthus")
                return;

            _menu = MainMenu.AddMenu("KarthusSharp", "KarthusSharp");


            comboMenu = _menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("comboQ", new CheckBox("Use Q", true));
            comboMenu.Add("comboW", new CheckBox("Use W", true));
            comboMenu.Add("comboE", new CheckBox("Use E", true));
            comboMenu.Add("comboAA", new CheckBox("Disable AA?", false));
            comboMenu.Add("comboWPercent", new Slider("Use W until Mana %", 10, 0, 100));
            comboMenu.Add("comboEPercent", new Slider("Use E until Mana %", 15, 0, 100));


            harassMenu = _menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("harassQ", new CheckBox("Use Q", true));
            harassMenu.Add("harassQPercent", new Slider("Use Q until Mana %", 15, 0, 100));
            harassMenu.Add("harassQLasthit", new CheckBox("Prioritize Last Hit", true));
            harassMenu.Add("harassQToggle", new KeyBind("Toggle Q", false, KeyBind.BindTypes.PressToggle, 'H'));


            farmMenu = _menu.AddSubMenu("Farming", "Farming");
            farmMenu.Add("farmQ", new ComboBox("Last Hit", 1, "Last Hit", "Lane Clear", "No"));
            farmMenu.Add("farmE", new CheckBox("Use E in Lane Clear", true));
            farmMenu.Add("farmAA", new CheckBox("Disable AA in Lane Clear", false));
            farmMenu.Add("farmQPercent", new Slider("Use Q until Mana %", 15, 0, 100));
            farmMenu.Add("farmEPercent", new Slider("Use E until Mana %", 20, 0, 100));


            notifyMenu = _menu.AddSubMenu("Notify on R killable", "Notify");
            notifyMenu.Add("notifyR", new CheckBox("Text Notify", true));
            notifyMenu.Add("notifyPing", new CheckBox("Ping Notify", false));


            drawMenu = _menu.AddSubMenu("Drawing", "Drawing");
            drawMenu.Add("drawQ", new CheckBox("Draw Q range", true));
            drawMenu.Add("DamageAfterCombo", new CheckBox("Damage After Combo", true));


            miscMenu = _menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ultKS", new CheckBox("Ultimate KS", true));
            miscMenu.Add("autoCast", new CheckBox("Auto Combo/LaneClear if dead", false));
            miscMenu.Add("packetCast", new CheckBox("Packet Cast", false));



            _spellQ = new Spell(SpellSlot.Q, 875);
            _spellW = new Spell(SpellSlot.W, 1000);
            _spellE = new Spell(SpellSlot.E, 505);
            _spellR = new Spell(SpellSlot.R, 20000f);

            _spellQ.SetSkillshot(1f, 160, float.MaxValue, false, SkillshotType.SkillshotCircle);
            _spellW.SetSkillshot(.5f, 70, float.MaxValue, false, SkillshotType.SkillshotCircle);
            _spellE.SetSkillshot(1f, 505, float.MaxValue, false, SkillshotType.SkillshotCircle);
            _spellR.SetSkillshot(3f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);


            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            LSEvents.BeforeAttack += Orbwalking_BeforeAttack;

            Chat.Print(
                "<font color=\"#1eff00\">KarthusSharp by Beaving</font> - <font color=\"#00BFFF\">Loaded</font>");
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

        private void Game_OnUpdate(EventArgs args)
        {
            if (getCheckBoxItem(miscMenu, "ultKS"))
                UltKs();

            if (getCheckBoxItem(notifyMenu, "notifyPing"))
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            t =>
                                ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready &&
                                t.LSIsValidTarget() && _spellR.GetDamage(t) > t.Health &&
                                t.LSDistance(ObjectManager.Player.Position) > _spellQ.Range))
                {
                    Ping(enemy.Position.To2D());
                }


            if (_spellQ.IsReady() && getKeyBindItem(harassMenu, "harassQToggle") &&
                PortAIO.OrbwalkerManager.isComboActive)
            {
                CastQ(TargetSelector.GetTarget(_spellQ.Range, DamageType.Magical),
                    getSliderItem(harassMenu, "harassQPercent"));
            }

            if (getCheckBoxItem(comboMenu, "comboAA") || ObjectManager.Player.Mana < 100)
            {
                PortAIO.OrbwalkerManager.SetAttack(false);
            }
            else
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
            }

            if (getCheckBoxItem(farmMenu, "farmAA") || ObjectManager.Player.Mana < 100)
            {
                PortAIO.OrbwalkerManager.SetAttack(false);
            }
            else
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
            }

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo();
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Harass();
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                LaneClear();
            }

            if (PortAIO.OrbwalkerManager.isLastHitActive)
            {
                LastHit();
            }

            RegulateEState();

            if (getCheckBoxItem(miscMenu, "autoCast"))
                if (IsInPassiveForm())
                    if (!Combo())
                        LaneClear(true);
        }

        private void Orbwalking_BeforeAttack(BeforeAttackArgs args)
        {
            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                args.Process = !_spellQ.IsReady();
            }
            else if (PortAIO.OrbwalkerManager.isLastHitActive)
            {
                bool farmQ = getBoxItem(farmMenu, "farmQ") == 0 ||
                             getBoxItem(farmMenu, "farmQ") == 2;
                args.Process =
                    !(farmQ && _spellQ.IsReady() &&
                      GetManaPercent() >= getSliderItem(farmMenu, "farmQPercent"));
            }
        }
        
        public float GetManaPercent()
        {
            return (ObjectManager.Player.Mana/ObjectManager.Player.MaxMana)*100f;
        }

        public bool PacketsNoLel()
        {
            return getCheckBoxItem(miscMenu, "packetCast");
        }

        private bool Combo()
        {
            bool anyQTarget = false;

            if (getCheckBoxItem(comboMenu, "comboW"))
                CastW(TargetSelector.GetTarget(_spellW.Range, DamageType.Magical),
                    getSliderItem(comboMenu, "comboWPercent"));

            if (getCheckBoxItem(comboMenu, "comboE") && _spellE.IsReady() && !IsInPassiveForm())
            {
                var target = TargetSelector.GetTarget(_spellE.Range, DamageType.Magical);

                if (target != null)
                {
                    var enoughMana = GetManaPercent() >= getSliderItem(comboMenu, "comboEPercent");

                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                    {
                        if (ObjectManager.Player.LSDistance(target.ServerPosition) <= _spellE.Range && enoughMana)
                        {
                            _comboE = true;
                            _spellE.Cast();
                        }
                    }
                    else if (!enoughMana)
                        RegulateEState(true);
                }
                else
                    RegulateEState();
            }

            if (getCheckBoxItem(comboMenu, "comboQ") && _spellQ.IsReady())
            {
                var target = TargetSelector.GetTarget(_spellQ.Range, DamageType.Magical);

                if (target != null)
                {
                    anyQTarget = true;
                    CastQ(target);
                }
            }

            return anyQTarget;
        }

        private void Harass()
        {
            if (getCheckBoxItem(harassMenu, "harassQLasthit"))
                LastHit();

            if (getCheckBoxItem(harassMenu, "harassQ"))
                CastQ(TargetSelector.GetTarget(_spellQ.Range, DamageType.Magical),
                    getSliderItem(harassMenu, "harassQPercent"));
        }

        private void LaneClear(bool ignoreConfig = false)
        {
            var farmQ = ignoreConfig || getBoxItem(farmMenu, "farmQ") == 1 ||
                        getBoxItem(farmMenu, "farmQ") == 2;
            var farmE = ignoreConfig || getCheckBoxItem(farmMenu, "farmE");

            List<Obj_AI_Base> minions;

            bool jungleMobs;
            if (farmQ && _spellQ.IsReady())
            {
                minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellQ.Range, MinionTypes.All,
                    MinionTeam.NotAlly);
                minions.RemoveAll(x => x.MaxHealth <= 5); //filter wards the ghetto method lel

                jungleMobs = minions.Any(x => x.Team == GameObjectTeam.Neutral);

                _spellQ.Width = SpellQWidth;
                var farmInfo = _spellQ.GetCircularFarmLocation(minions, _spellQ.Width);

                if (farmInfo.MinionsHit >= 1)
                    CastQ(farmInfo.Position, jungleMobs ? 0 : getSliderItem(farmMenu, "farmQPercent"));
            }

            if (!farmE || !_spellE.IsReady() || IsInPassiveForm())
                return;
            _comboE = false;

            minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellE.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            minions.RemoveAll(x => x.MaxHealth <= 5); //filter wards the ghetto method lel

            jungleMobs = minions.Any(x => x.Team == GameObjectTeam.Neutral);

            var enoughMana = GetManaPercent() > getSliderItem(farmMenu, "farmEPercent");

            if (enoughMana &&
                ((minions.Count >= 3 || jungleMobs) &&
                 ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1))
                _spellE.CastOnUnit(ObjectManager.Player);
            else if (!enoughMana ||
                     ((minions.Count <= 2 && !jungleMobs) &&
                      ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 2))
                RegulateEState(!enoughMana);
        }

        private void LastHit()
        {
            var farmQ = getBoxItem(farmMenu, "farmQ") == 0 ||
                        getBoxItem(farmMenu, "farmQ") == 2;

            if (!farmQ || !_spellQ.IsReady())
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellQ.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            minions.RemoveAll(x => x.MaxHealth <= 5); //filter wards the ghetto method lel

            foreach (
                var minion in
                    minions.Where(
                        x =>
                            ObjectManager.Player.LSGetSpellDamage(x, SpellSlot.Q, 1) >=
                            //FirstDamage = multitarget hit, differentiate! (check radius around mob predicted pos)
                            HealthPrediction.GetHealthPrediction(x, (int) (_spellQ.Delay*1000))))
            {
                CastQ(minion, getSliderItem(farmMenu, "farmQPercent"));
            }
        }

        private void UltKs()
        {
            if (!_spellR.IsReady())
                return;
            var time = Utils.TickCount;

            List<AIHeroClient> ultTargets = new List<AIHeroClient>();

            foreach (
                var target in
                    Program.Helper.EnemyInfo.Where(
                        x => //need to check if recently recalled (for cases when no mana for baseult)
                            x.Player.IsValid &&
                            !x.Player.IsDead &&
                            x.Player.IsEnemy &&
                            //!(x.RecallInfo.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallStarted && x.RecallInfo.GetRecallCountdown() < 3100) && //let BaseUlt handle this one
                            ((!x.Player.IsVisible && time - x.LastSeen < 10000) ||
                             (x.Player.IsVisible && x.Player.LSIsValidTarget())) &&
                            ObjectManager.Player.LSGetSpellDamage(x.Player, SpellSlot.R) >=
                            Program.Helper.GetTargetHealth(x, (int) (_spellR.Delay*1000f))))
            {
                if (target.Player.IsVisible || (!target.Player.IsVisible && time - target.LastSeen < 2750))
                    //allies still attacking target? prevent overkill
                    if (Program.Helper.OwnTeam.Any(x => !x.IsMe && x.LSDistance(target.Player) < 1600))
                        continue;

                if (IsInPassiveForm() ||
                    !Program.Helper.EnemyTeam.Any(
                        x =>
                            x.IsValid && !x.IsDead &&
                            (x.IsVisible || (!x.IsVisible && time - Program.Helper.GetPlayerInfo(x).LastSeen < 2750)) &&
                            ObjectManager.Player.LSDistance(x) < 1600))
                    //any other enemies around? dont ult unless in passive form
                    ultTargets.Add(target.Player);
            }

            int targets = ultTargets.Count();

            if (targets > 0)
            {
                //dont ult if Zilean is nearby the target/is the target and his ult is up
                var zilean =
                    Program.Helper.EnemyTeam.FirstOrDefault(
                        x =>
                            x.ChampionName == "Zilean" &&
                            (x.IsVisible || (!x.IsVisible && time - Program.Helper.GetPlayerInfo(x).LastSeen < 3000)) &&
                            (x.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready ||
                             (x.Spellbook.GetSpell(SpellSlot.R).Level > 0 &&
                              x.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Surpressed &&
                              x.Mana >= x.Spellbook.GetSpell(SpellSlot.R).SData.Mana)));

                if (zilean != null)
                {
                    int inZileanRange = ultTargets.Count(x => x.LSDistance(zilean) < 2500);
                        //if multiple, shoot regardless

                    if (inZileanRange > 0)
                        targets--; //remove one target, because zilean can save one
                }

                if (targets > 0)
                    _spellR.Cast();
            }
        }

        private void RegulateEState(bool ignoreTargetChecks = false)
        {
            if (!_spellE.IsReady() || IsInPassiveForm() ||
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 2)
                return;
            var target = TargetSelector.GetTarget(_spellE.Range, DamageType.Magical);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellE.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            if (!ignoreTargetChecks && (target != null || (!_comboE && minions.Count != 0)))
                return;
            _spellE.CastOnUnit(ObjectManager.Player);
            _comboE = false;
        }

        private void CastQ(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!_spellQ.IsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            _spellQ.Width = GetDynamicQWidth(target);
            _spellQ.Cast(target);

        }

        private void CastQ(Vector2 pos, int minManaPercent = 0)
        {
            if (!_spellQ.IsReady())
                return;
            if (GetManaPercent() >= minManaPercent)
                _spellQ.Cast(pos);
        }

        private void CastW(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!_spellW.IsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            _spellW.Width = GetDynamicWWidth(target);
            _spellW.Cast(target);
        }

        private float GetDynamicWWidth(Obj_AI_Base target)
        {
            return Math.Max(70, (1f - (ObjectManager.Player.LSDistance(target)/_spellW.Range))*SpellWWidth);
        }

        private float GetDynamicQWidth(Obj_AI_Base target)
        {
            return Math.Max(30, (1f - (ObjectManager.Player.LSDistance(target)/_spellQ.Range))*SpellQWidth);
        }

        private static bool IsInPassiveForm()
        {
            return ObjectManager.Player.IsZombie; //!ObjectManager.Player.IsHPBarRendered;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                var drawQ = getCheckBoxItem(drawMenu, "drawQ");

                if (drawQ)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _spellQ.Range, System.Drawing.Color.LightBlue);
            }

            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level > 0)
            {
                var time = Utils.TickCount;

                var victims =
                    Program.Helper.EnemyInfo.Where(
                        x =>
                            x.Player.IsValid && !x.Player.IsDead && x.Player.IsEnemy &&
                            ((!x.Player.IsVisible && time - x.LastSeen < 10000) ||
                             (x.Player.IsVisible && x.Player.LSIsValidTarget())) &&
                            ObjectManager.Player.LSGetSpellDamage(x.Player, SpellSlot.R) >=
                            Program.Helper.GetTargetHealth(x, (int) (_spellR.Delay*1000f)))
                        .Aggregate("", (current, target) => current + (target.Player.ChampionName + " "));

                if (victims != "" && _spellR.IsReady())
                {
                    if (getCheckBoxItem(notifyMenu, "notifyR"))
                    {
                        Drawing.DrawText(Drawing.Width*0.44f, Drawing.Height*0.7f, System.Drawing.Color.GreenYellow,
                            "Ult can kill: " + victims);

                        //use when pos works
//                        var x = new Render.Text((int) (Drawing.Width*0.44f), (int) (Drawing.Height*0.7f), "Ult can kill: " + victims, 30, SharpDX.Color.Red); //.Add()


                    }
                }
            }
        }

        private static void Ping(Vector2 position)
        {
            if (LeagueSharp.Common.Utils.TickCount - LastPingT < 30*1000)
            {
                return;
            }

            LastPingT = LeagueSharp.Common.Utils.TickCount;
            PingLocation = position;
            SimplePing();

            LeagueSharp.Common.Utility.DelayAction.Add(150, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(300, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(400, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(800, SimplePing);
        }

        private static void SimplePing()
        {
            TacticalMap.ShowPing(PingCategory.Fallback, PingLocation, true);
        }

        private float ComboDamage(AIHeroClient t)
        {
            return _spellR.GetDamage(t);
        }
    }
}
