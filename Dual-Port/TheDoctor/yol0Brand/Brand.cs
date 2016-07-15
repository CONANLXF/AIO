using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

 namespace yol0Brand
{
    internal class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static LeagueSharp.Common.Spell _Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1050);
        private static LeagueSharp.Common.Spell _W = new LeagueSharp.Common.Spell(SpellSlot.W, 900);
        private static LeagueSharp.Common.Spell _E = new LeagueSharp.Common.Spell(SpellSlot.E, 625);
        private static LeagueSharp.Common.Spell _R = new LeagueSharp.Common.Spell(SpellSlot.R, 750);

        private static LeagueSharp.Common.Spell _Ignite = new LeagueSharp.Common.Spell(SpellSlot.Unknown, 600);

        private static Menu Config, comboMenu, harassMenu, farmMenu, ksMenu, miscMenu, drawMenu;
        private static AIHeroClient comboTarget;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Brand")
                return;

            Config = MainMenu.AddMenu("yol0 Brand", "yol0Brand");

            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.Add("useR", new CheckBox("Use R"));
            comboMenu.Add("blaze", new CheckBox("Use E before Q", false));

            harassMenu = Config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("useQ", new CheckBox("Use Q"));
            harassMenu.Add("useW", new CheckBox("Use W"));
            harassMenu.Add("useE", new CheckBox("Use E"));
            harassMenu.Add("mana", new Slider("Mana Manager", 40));

            farmMenu = Config.AddSubMenu("Lane Clear", "Farm");
            farmMenu.Add("useQ", new CheckBox("Use Q", false));
            farmMenu.Add("useW", new CheckBox("Use W"));
            farmMenu.Add("useE", new CheckBox("Use E", false));

            ksMenu = Config.AddSubMenu("KS", "KS");
            ksMenu.Add("ksQ", new CheckBox("KS with Q"));
            ksMenu.Add("ksW", new CheckBox("KS with W"));
            ksMenu.Add("ksE", new CheckBox("KS with E"));
            ksMenu.Add("ksR", new CheckBox("KS with R"));

            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("gapclose", new CheckBox("Auto Stun Gapclosers"));
            miscMenu.Add("interrupt", new CheckBox("Auto Interrupt"));
            miscMenu.Add("ignite", new CheckBox("Auto Ignite"));

            drawMenu = Config.AddSubMenu("Drawing", "Drawing");
            drawMenu.Add("drawQ", new CheckBox("Draw Q Range"));//.SetValue(new Circle(true, Color.Green)));
            drawMenu.Add("drawW", new CheckBox("Draw W Range"));//.SetValue(new Circle(true, Color.Green)));
            drawMenu.Add("drawE", new CheckBox("Draw E Range"));//.SetValue(new Circle(true, Color.Green)));
            drawMenu.Add("drawR", new CheckBox("Draw R Range"));//.SetValue(new Circle(true, Color.Green)));

            _Q.SetSkillshot(0.625f, 50f, 1600f, true, SkillshotType.SkillshotLine);
            _W.SetSkillshot(1.0f, 240f, int.MaxValue, false, SkillshotType.SkillshotCircle);

            var ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                _Ignite.Slot = ignite.Slot;


            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!miscMenu["gapcloser"].Cast<CheckBox>().CurrentValue)
                return;

            if (gapcloser.Sender.HasBuff("brandablaze") && _Q.IsReady())
            {
                _Q.Cast(gapcloser.Sender);
            }
            else
            {
                if (_E.IsReady() && _Q.IsReady())
                {
                    _E.CastOnUnit(gapcloser.Sender);
                }
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!miscMenu["interrupt"].Cast<CheckBox>().CurrentValue)
                return;

            if (sender.HasBuff("brandablaze") && _Q.IsReady())
            {
                _Q.Cast(sender);
            }
            else
            {
                if (_E.IsReady() && _Q.IsReady())
                {
                    _E.CastOnUnit(sender);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.Green);
            }
            if (drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Render.Circle.DrawCircle(Player.Position, _W.Range, Color.Green);
            }
            if (drawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Render.Circle.DrawCircle(Player.Position, _E.Range, Color.Green);
            }
            if (drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Render.Circle.DrawCircle(Player.Position, _R.Range, Color.Green);
            }
        }
        
        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                comboTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                if (comboTarget.IsValid && comboTarget.LSIsValidTarget())
                {
                    Combo(comboTarget);
                }
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (farmMenu["useW"].Cast<CheckBox>().CurrentValue)
                    CastWFarm();

                if (farmMenu["useQ"].Cast<CheckBox>().CurrentValue)
                    CastQFarm();

                if (farmMenu["useE"].Cast<CheckBox>().CurrentValue)
                    CastEFarm();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                comboTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                if (comboTarget.IsValid && comboTarget.LSIsValidTarget())
                {
                    Harass(comboTarget);
                }
            }
            KS();
        }

        private static void Harass(AIHeroClient target)
        {
            if (harassMenu["useE"].Cast<CheckBox>().CurrentValue && CheckMana(_E))
                CastE(target);
            if (harassMenu["useQ"].Cast<CheckBox>().CurrentValue && CheckMana(_Q))
                CastQ(target);
            if (harassMenu["useW"].Cast<CheckBox>().CurrentValue && CheckMana(_W))
                CastW(target);
        }

        private static bool CheckMana(LeagueSharp.Common.Spell spell)
        {
            return (Player.Mana - spell.Instance.SData.Mana) / Player.MaxMana >= harassMenu["mana"].Cast<Slider>().CurrentValue / 100;
        }

        private static void Combo(AIHeroClient target)
        {
            if (comboMenu["useE"].Cast<CheckBox>().CurrentValue)
                CastE(target);
            if (comboMenu["useQ"].Cast<CheckBox>().CurrentValue)
                CastQ(target);
            if (comboMenu["useW"].Cast<CheckBox>().CurrentValue)
                CastW(target);
            if (comboMenu["useR"].Cast<CheckBox>().CurrentValue)
                CastR(target);
        }

        private static void KS()
        {
            foreach (var enemy in HeroManager.Enemies)
            {
                if (_Ignite.Slot != SpellSlot.Unknown && miscMenu["ignite"].Cast<CheckBox>().CurrentValue && Player.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Ignite) > enemy.Health && enemy.LSIsValidTarget(_Ignite.Range))
                {
                    _Ignite.CastOnUnit(enemy);
                }
                else if (ksMenu["ksQ"].Cast<CheckBox>().CurrentValue && Player.LSGetSpellDamage(enemy, SpellSlot.Q) > enemy.Health && _Q.IsReady() && enemy.LSIsValidTarget(_Q.Range))
                {
                    _Q.Cast(enemy);
                }
                else if (ksMenu["ksW"].Cast<CheckBox>().CurrentValue && Player.LSGetSpellDamage(enemy, SpellSlot.W) > enemy.Health && _W.IsReady() && enemy.LSIsValidTarget(_W.Range))
                {
                    _W.Cast(enemy);
                }
                else if (ksMenu["ksE"].Cast<CheckBox>().CurrentValue && Player.LSGetSpellDamage(enemy, SpellSlot.E) > enemy.Health && _E.IsReady() && enemy.LSIsValidTarget(_E.Range))
                {
                    _E.Cast(enemy);
                }
                else if (ksMenu["ksR"].Cast<CheckBox>().CurrentValue && Player.LSGetSpellDamage(enemy, SpellSlot.R) > enemy.Health && _R.IsReady() && enemy.LSIsValidTarget(_R.Range))
                {
                    _R.Cast(enemy);
                }
            }
        }

        private static void CastQ(Obj_AI_Base target)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.HasBuff("brandablaze") && comboMenu["blaze"].Cast<CheckBox>().CurrentValue)
            {
                if (_Q.IsReady() && target.LSIsValidTarget(_Q.Range))
                {
                    _Q.Cast(target);
                }
            }
            else
            {
                if (_Q.IsReady() && target.LSIsValidTarget(_Q.Range))
                {
                    _Q.Cast(target);
                }
            }
        }

        private static void CastW(Obj_AI_Base target)
        {
            if (_W.IsReady() && target.LSIsValidTarget(_W.Range))
            {
                _W.Cast(target, aoe: true);
            }
        }

        private static void CastE(Obj_AI_Base target)
        {
            if (_E.IsReady() && target.LSIsValidTarget(_E.Range))
            {
                _E.CastOnUnit(target);
            }
        }

        private static void CastR(Obj_AI_Base target)
        {
            if (_R.IsReady() && target.LSIsValidTarget(_R.Range))
            {
                _R.CastOnUnit(target);
            }
        }

        private static void CastWFarm()
        {
            if (!_W.IsReady())
                return;

            var minions = MinionManager.GetMinions(_W.Range);
            var positions = new List<Vector2>();
            foreach (var minion in minions)
            {
                positions.Add(minion.ServerPosition.LSTo2D());
            }

            var location = MinionManager.GetBestCircularFarmLocation(positions, 240, _W.Range);
            if (location.MinionsHit >= 3)
            {
                _W.Cast(location.Position);
            }
        }

        private static void CastQFarm()
        {
            if (!_Q.IsReady())
                return;

            var minions = MinionManager.GetMinions(_Q.Range);
            foreach (var minion in minions)
            {
                if (Player.LSGetSpellDamage(minion, SpellSlot.Q) > minion.Health)
                {
                    _Q.Cast(minion);
                }
            }
        }

        private static void CastEFarm()
        {
            if (!_E.IsReady())
                return;

            var minions = MinionManager.GetMinions(_E.Range);
            foreach (var minion in minions)
            {
                if (Player.LSGetSpellDamage(minion, SpellSlot.E) > minion.Health)
                {
                    _E.CastOnUnit(minion);
                }
            }
        }
    }
}