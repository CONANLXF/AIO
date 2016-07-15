using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace KappaSeries
{
    class Aatrox
    {
        public Aatrox()
        {
            CustomEvents.Game.OnGameLoad += Load;
        }

        public static readonly List<LeagueSharp.Common.Spell> SpellList = new List<LeagueSharp.Common.Spell>();
        private static LeagueSharp.Common.Spell _q;
        private static LeagueSharp.Common.Spell _w;
        private static LeagueSharp.Common.Spell _e;
        private static LeagueSharp.Common.Spell _r;
        public static SpellSlot IgniteSlot;
        public static SpellSlot SmiteSlot;
        private static Menu _cfg, comboMenu, harassMenu, laneClearMenu, jungleClearMenu, ksMenu, drawMenu, miscMenu;
        private static AIHeroClient _player;

        static void Load(EventArgs args)
        {
            _player = ObjectManager.Player;

            _q = new LeagueSharp.Common.Spell(SpellSlot.Q, 676f);
            _w = new LeagueSharp.Common.Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(_player));
            _e = new LeagueSharp.Common.Spell(SpellSlot.E, 980f);
            _r = new LeagueSharp.Common.Spell(SpellSlot.R, 550f);

            _q.SetSkillshot(_q.Instance.SData.SpellCastTime, 280f, _q.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            _e.SetSkillshot(_e.Instance.SData.SpellCastTime, _e.Instance.SData.LineWidth, _e.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotLine);

            SpellList.Add(_q);
            SpellList.Add(_w);
            SpellList.Add(_e);
            SpellList.Add(_r);

            IgniteSlot = _player.GetSpellSlot("SummonerDot");
            SmiteSlot = _player.GetSpellSlot("summonersmite");

            _cfg = MainMenu.AddMenu("Aatrox", "Aatrox");

            comboMenu = _cfg.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseECombo", new CheckBox("Use E"));
            comboMenu.Add("UseRCombo", new CheckBox("Use R"));
            comboMenu.Add("minW", new Slider("min HP % W", 50, 0, 100));
            comboMenu.Add("maxW", new Slider("Max HP % W", 80, 0, 100));
            comboMenu.Add("minR", new Slider("Min Enemies to R", 2, 0, 5));
            comboMenu.Add("DontQ", new CheckBox("Don't Q at enemy tower"));
            comboMenu.Add("Dive", new CheckBox("Dive Tower when target HP is lower then %"));
            comboMenu.Add("DiveMHP", new Slider("My HP % to Towerdive", 60));
            comboMenu.Add("DiveTHP", new Slider("Target HP % to Towerdive", 10));
            comboMenu.Add("UseItems", new CheckBox("Use Items"));

            harassMenu = _cfg.AddSubMenu("Harass", "Harass");
            harassMenu.Add("HarQ", new CheckBox("Use Q In Harass", false));
            harassMenu.Add("HarE", new CheckBox("Use E In Harass"));

            laneClearMenu = _cfg.AddSubMenu("LaneClear", "LaneClear");
            laneClearMenu.Add("UseQLane", new CheckBox("Use Q", false));
            laneClearMenu.Add("UseWLane", new CheckBox("Use W"));
            laneClearMenu.Add("UseELane", new CheckBox("Use E"));

            jungleClearMenu = _cfg.AddSubMenu("JungleClear", "JungleClear");
            jungleClearMenu.Add("UseQJungle", new CheckBox("Use Q"));
            jungleClearMenu.Add("UseWJungle", new CheckBox("Use W"));
            jungleClearMenu.Add("UseEJungle", new CheckBox("Use E"));

            ksMenu = _cfg.AddSubMenu("KillSteal", "KillSteal");
            ksMenu.Add("SmartKS", new CheckBox("Smart KillSteal"));
            ksMenu.Add("RKS", new CheckBox("Use R in KS", false));

            drawMenu = _cfg.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("Qdraw", new CheckBox("Draw Q Range"));
            drawMenu.Add("Edraw", new CheckBox("Draw E Range"));
            drawMenu.Add("LagFree", new CheckBox("Lag Free Cirlces"));
            drawMenu.Add("CircleThickness", new Slider("Circles Thickness", 1, 1, 10));

            miscMenu = _cfg.AddSubMenu("Misc", "Misc");
            miscMenu.Add("TowerQ", new CheckBox("Auto Q Under Turret", false));
            miscMenu.Add("IntQ", new CheckBox("Auto Interrupt with Q", false));
            miscMenu.Add("IntMed", new CheckBox("Interrupt Medium Danger Spells", false));
            miscMenu.Add("SmartW", new CheckBox("Smart W Logic"));

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
            LSEvents.AfterAttack += OrbwalkingAfterAttack;

        }
        
        private static void OrbwalkingAfterAttack(AfterAttackArgs args)
        {
            if (PortAIO.OrbwalkerManager.isComboActive || PortAIO.OrbwalkerManager.isLaneClearActive && comboMenu["UseItems"].Cast<CheckBox>().CurrentValue)
            {
                var hydId = ItemData.Ravenous_Hydra_Melee_Only.Id;
                var tiaId = ItemData.Tiamat_Melee_Only.Id;

                if (Items.HasItem(hydId))
                {
                    if (Items.CanUseItem(hydId))
                    {
                        Items.UseItem(hydId);
                    }
                }

                if (Items.HasItem(tiaId))
                {
                    if (Items.CanUseItem(tiaId))
                    {
                        Items.UseItem(tiaId);
                    }
                }
            }
        }

        private static void OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!miscMenu["IntQ"].Cast<CheckBox>().CurrentValue || !_q.IsReady() || !sender.LSIsValidTarget(_q.Range))
            {
                return;
            }
            if (args.DangerLevel == Interrupter2.DangerLevel.High || args.DangerLevel == Interrupter2.DangerLevel.Medium && miscMenu["IntMed"].Cast<CheckBox>().CurrentValue)
            {
                _q.Cast(sender);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (drawMenu["LagFree"].Cast<CheckBox>().CurrentValue)
            {
                if (drawMenu["Qdraw"].Cast<CheckBox>().CurrentValue)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.Cyan,
                        drawMenu["CircleThickness"].Cast<Slider>().CurrentValue);
                }

                if (drawMenu["Edraw"].Cast<CheckBox>().CurrentValue)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.Crimson,
                        drawMenu["CircleThickness"].Cast<Slider>().CurrentValue);
                }

            }
            else
            {
                if (drawMenu["Qdraw"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.Cyan);
                }

                if (drawMenu["Edraw"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.Crimson);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (_player.IsDead)
            {
                return;
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
                Jungleclear();
                Laneclear();
            }
            if (ksMenu["SmartKS"].Cast<CheckBox>().CurrentValue)
            {
                Smartks();
            }
            if (PortAIO.OrbwalkerManager.isFleeActive)
            {
                Flee();
            }
            if (miscMenu["TowerQ"].Cast<CheckBox>().CurrentValue)
            {
                Towerq();
            }
        }


        private static void Towerq()
        {
            var allyturret = ObjectManager.Get<Obj_AI_Turret>().First(obj => obj.IsAlly && obj.LSDistance(_player) <= 775f);
            var minUnderTur = MinionManager.GetMinions(allyturret.ServerPosition, 775, MinionTypes.All, MinionTeam.Enemy);

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => target.LSIsValidTarget(_e.Range)))
            {
                if (allyturret != null && minUnderTur == null && target.LSIsValidTarget())
                {
                    _q.Cast(target);
                }
            }
        }

        private static void Flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (_q.IsReady())
            {
                _q.Cast(Game.CursorPos);
            }
            var t = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
            if (_e.IsReady() && t.LSIsValidTarget(_e.Range))
            {
                _e.Cast(t);
            }
        }

        private static void Smartks()
        {
            foreach (var t in ObjectManager.Get<AIHeroClient>().Where(t => t.IsEnemy).Where(t => t.LSIsValidTarget(_q.Range)))
            {
                #region e
                if (t.Health < _e.GetDamage(t) && _e.IsReady())
                {
                    _e.Cast(t);
                }
                #endregion
                #region q
                else if (t.Health < _q.GetDamage(t) && _q.IsReady())
                {
                    _q.Cast(t.ServerPosition);

                }
                #endregion
                #region eq
                else if (t.Health < (_q.GetDamage(t) + _e.GetDamage(t)) && _e.IsReady() && _q.IsReady())
                {
                    if (_e.Cast(t) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                    {
                        _q.Cast(t.ServerPosition, false);
                    }
                }
                #endregion
                #region eq ignite
                else if (t.Health < (_q.GetDamage(t) + _e.GetDamage(t) + _player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite)) && _e.IsReady() && _q.IsReady() && IgniteSlot.IsReady() && IgniteSlot != SpellSlot.Unknown)
                {
                    _e.Cast(t);
                    if (!_e.IsCharging)
                    {
                        _q.Cast(t, false, true);
                    }
                    _player.Spellbook.CastSpell(IgniteSlot, t);
                }
                #endregion
                #region eq smite
                else if (t.Health < (_q.GetDamage(t) + _e.GetDamage(t) + Smitedamage()) && _e.IsReady() && _q.IsReady() && SmiteSlot.IsReady() && SmiteSlot != SpellSlot.Unknown)
                {
                    _e.Cast(t);
                    if (!_e.IsCharging)
                    {
                        _q.Cast(t, false, true);
                    }
                    _player.Spellbook.CastSpell(SmiteSlot, t);
                }
                #endregion
                #region eq smite R
                else if (ksMenu["RKS"].Cast<CheckBox>().CurrentValue && t.Health < (_q.GetDamage(t) + _e.GetDamage(t) + Smitedamage() + _r.GetDamage(t)) && _e.IsReady() && _q.IsReady() && SmiteSlot.IsReady() && _r.IsReady() && SmiteSlot != SpellSlot.Unknown)
                {
                    _e.Cast(t);
                    if (!_e.IsCharging)
                    {
                        _q.Cast(t, false, true);
                    }
                    if (_player.LSDistance(t) < 500)
                    {
                        _player.Spellbook.CastSpell(SmiteSlot, t);
                    }

                    if (_player.LSDistance(t) < _r.Range && !_e.IsCharging && !_q.IsCharging)
                    {
                        _r.Cast();
                    }
                }
                #endregion
                #region eq ignite R
                else if (ksMenu["RKS"].Cast<CheckBox>().CurrentValue && t.Health < (_q.GetDamage(t) + _e.GetDamage(t) + _player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite) + _r.GetDamage(t)) && _e.IsReady() && _q.IsReady() && IgniteSlot.IsReady() && _r.IsReady() && IgniteSlot != SpellSlot.Unknown)
                {
                    _e.Cast(t);
                    if (!_e.IsCharging)
                    {
                        _q.Cast(t, false, true);
                    }
                    if (_player.LSDistance(t) < 600)
                    {
                        _player.Spellbook.CastSpell(IgniteSlot, t);
                    }

                    if (_player.LSDistance(t) < _r.Range && !_e.IsCharging && !_q.IsCharging)
                    {
                        _r.Cast();
                    }

                }
                #endregion
                else return;
            }
        }

        private static void Laneclear()
        {
            var minion = MinionManager.GetMinions(_player.ServerPosition, _q.Range);

            if (minion.Count < 3)
                return;

            if (laneClearMenu["UseQLane"].Cast<CheckBox>().CurrentValue && _q.IsReady())
            {
                _q.Cast(minion[0].ServerPosition);
            }
            if (laneClearMenu["UseWLane"].Cast<CheckBox>().CurrentValue && _w.IsReady() && _player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2)
            {
                _w.Cast();
            }
            if (laneClearMenu["UseELane"].Cast<CheckBox>().CurrentValue && _e.IsReady())
            {
                _e.Cast(minion[0].ServerPosition);
            }
        }

        private static void Jungleclear()
        {
            var junglemonster = MinionManager.GetMinions(_player.ServerPosition, _q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (junglemonster.Count == 0) return;

            if (jungleClearMenu["UseEJungle"].Cast<CheckBox>().CurrentValue && _e.IsReady())
            {
                _e.Cast(junglemonster[0].ServerPosition);
            }
            if (jungleClearMenu["UseQJungle"].Cast<CheckBox>().CurrentValue && _q.IsReady())
            {
                _q.Cast(junglemonster[0].ServerPosition);
            }
            if (jungleClearMenu["UseWJungle"].Cast<CheckBox>().CurrentValue && _player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2 && _w.IsReady())
            {
                _w.Cast();
            }

        }

        private static void Harass()
        {
            var t = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
            if (t == null) return;

            if (t.LSIsValidTarget() && _e.IsReady() && harassMenu["HarE"].Cast<CheckBox>().CurrentValue)
            {
                _e.Cast(t);
            }
            if (t.LSIsValidTarget() && _e.IsReady() && harassMenu["HarQ"].Cast<CheckBox>().CurrentValue && !t.UnderTurret())
            {
                _q.Cast(t);
            }

        }

        private static float Smitedamage()
        {
            int lvl = _player.Level;
            int smitedamage = (20 + 8 * lvl);

            return smitedamage;
        }

        private static float GetHealthPercent(AIHeroClient player)
        {
            return player.Health * 100 / player.MaxHealth;
        }

        private static void Combo()
        {
            var smitedmg = Smitedamage();
            var t = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
            var youm = ItemData.Youmuus_Ghostblade;
            var bil = ItemData.Bilgewater_Cutlass;
            var botrk = ItemData.Blade_of_the_Ruined_King;

            #region Items
            if (comboMenu["UseItems"].Cast<CheckBox>().CurrentValue)
            {
                if (Items.HasItem(youm.Id))
                {
                    if (Items.CanUseItem(youm.Id) && _player.LSDistance(t) <= 200f)
                    {
                        Items.UseItem(youm.Id);
                    }
                }
                if (Items.HasItem(bil.Id))
                {
                    if (Items.CanUseItem(bil.Id) && _player.LSDistance(t) <= bil.Range)
                    {
                        Items.UseItem(bil.Id, t);
                    }
                }
                if (Items.HasItem(botrk.Id))
                {
                    if (Items.CanUseItem(botrk.Id) && _player.LSDistance(t) <= botrk.Range)
                    {
                        Items.UseItem(botrk.Id, t);
                    }
                }
            }
            #endregion

            if (t == null) return;

            #region E
            if (_e.IsReady() && comboMenu["UseECombo"].Cast<CheckBox>().CurrentValue && _player.LSDistance(t) <= _e.Range)
            {
                _e.Cast(t);
            }
            #endregion
            #region Q

            if (_q.IsReady() && comboMenu["UseQCombo"].Cast<CheckBox>().CurrentValue && _player.LSDistance(t) <= _q.Range)
            {
                if (comboMenu["DontQ"].Cast<CheckBox>().CurrentValue && t.UnderTurret())
                {
                    if (comboMenu["Dive"].Cast<CheckBox>().CurrentValue)
                    {
                        if (GetHealthPercent(t) <= comboMenu["DiveTHP"].Cast<Slider>().CurrentValue)
                        {
                            if (GetHealthPercent(_player) >= comboMenu["DiveMHP"].Cast<Slider>().CurrentValue)
                            {
                                if (_q.Cast(t.ServerPosition))
                                    return;
                            }
                        }
                    }
                }
                else
                {
                    _q.Cast(t.ServerPosition);

                }

            }


            #endregion
            #region W
            if (_w.IsReady() && comboMenu["UseWCombo"].Cast<CheckBox>().CurrentValue)
            {
                #region Smart W
                if (miscMenu["SmartW"].Cast<CheckBox>().CurrentValue)
                {
                    if (_player.Health < (_player.MaxHealth * 0.95))
                    {
                        if (_player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2)
                        {
                            _w.Cast();
                        }
                    }
                    else if (_player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                    {
                        _w.Cast();
                    }
                }
                #endregion
                #region not smart W
                else if (!miscMenu["SmartW"].Cast<CheckBox>().CurrentValue)
                {
                    if (_w.IsReady() && GetHealthPercent(_player) < (comboMenu["minW"].Cast<Slider>().CurrentValue))
                    {
                        if (_player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2)
                        {
                            _w.Cast();
                        }
                    }
                    if (_w.IsReady() && GetHealthPercent(_player) > (comboMenu["maxW"].Cast<Slider>().CurrentValue))
                    {
                        if (_player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                        {
                            _w.Cast();
                        }
                    }
                }

            }
            #endregion
            #endregion
            #region R
            if (_r.IsReady() && comboMenu["UseRCombo"].Cast<CheckBox>().CurrentValue && _player.LSCountEnemiesInRange(_r.Range) >= comboMenu["minR"].Cast<Slider>().CurrentValue && _player.LSDistance(t) <= _r.Range)
            {
                _r.Cast();
            }
            #endregion
        }
    }
}