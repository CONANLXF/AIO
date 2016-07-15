using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace HeavenStrikeRyze
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        

        public static LeagueSharp.Common.Spell _q, _q2, _w, _e, _r;

        public static Menu _menu, comboMenu, autoMenu, drawMenu, laneMenu, jungleMenu, lasthitMenu;

        public static void Game_OnGameLoad()
        {
            //Verify Champion
            if (Player.ChampionName != "Ryze")
                return;

            //Spells
            _q = new LeagueSharp.Common.Spell(SpellSlot.Q, 900);
            _q2 = new LeagueSharp.Common.Spell(SpellSlot.Q, 900); // xxx bounce range
            _w = new LeagueSharp.Common.Spell(SpellSlot.W, 600); // 600
            _e = new LeagueSharp.Common.Spell(SpellSlot.E, 600); // 200 bounce 
            _r = new LeagueSharp.Common.Spell(SpellSlot.R); // xx ramge

            _q.SetSkillshot(0.26f, 50f, 1700f, true, SkillshotType.SkillshotLine);
            _q2.SetSkillshot(0.26f, 50f, 1700f, false, SkillshotType.SkillshotLine);
            _q.MinHitChance = HitChance.Medium;
            _q2.MinHitChance = HitChance.Medium;


            _menu = MainMenu.AddMenu(Player.ChampionName, Player.ChampionName);


            comboMenu = _menu.AddSubMenu("Auto", "Auto");
            comboMenu.Add("Block", new CheckBox("Smart Block AutoAttack"));
            comboMenu.Add("ComboSwitch", new KeyBind("Combo Mode Switch", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("ComboMode", new ComboBox("Combo Mode", 0, "Burst", "AoE/Shield"));


            autoMenu = _menu.AddSubMenu("Combo", "Combo");
            autoMenu.Add("Wantigap", new CheckBox("W anti gap"));
            autoMenu.Add("Winterrupt", new CheckBox("W interrupt"));


            jungleMenu = _menu.AddSubMenu("JungleClear", "JungleClear");
            jungleMenu.Add("QJC", new CheckBox("Use Q Jungle Clear"));
            jungleMenu.Add("WJC", new CheckBox("Use W Jungle Clear"));
            jungleMenu.Add("EJC", new CheckBox("Use E Jungle Clear"));
            jungleMenu.Add("ManaJC", new Slider("Min mana for JungleClear", 40, 0, 100));


            laneMenu = _menu.AddSubMenu("LaneClear", "LaneClear");
            laneMenu.Add("QLC", new CheckBox("Use Q Lane Clear"));
            laneMenu.Add("WJC", new CheckBox("Use W Lane Clear"));
            laneMenu.Add("ELC", new CheckBox("Use E Lane Clear"));
            laneMenu.Add("ManaLC", new Slider("Min mana for LaneClear", 40, 0, 100));


            lasthitMenu = _menu.AddSubMenu("LastHit", "LastHit");
            lasthitMenu.Add("QLH", new CheckBox("Use Q Last Hit"));
            lasthitMenu.Add("ManaLH", new Slider("Min mana for LastHit", 40, 0, 100));


            drawMenu = _menu.AddSubMenu("Drawing", "Drawing");
            drawMenu.Add("DQ", new CheckBox("Draw Q"));
            drawMenu.Add("DW", new CheckBox("Draw W"));
            drawMenu.Add("DE", new CheckBox("Draw E"));
            drawMenu.Add("DrawMode", new CheckBox("Draw Combo Mode"));

            //Listen to events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += oncast;

            //modes
            HeavenStrikeRyze.Combo.BadaoActivate();
            HeavenStrikeRyze.Jungle.BadaoActivate();
            HeavenStrikeRyze.Lane.BadaoActivate();
            HeavenStrikeRyze.LastHit.BadaoActivate();
        }
        //combo
        public static bool BlockAA {get { return comboMenu["Block"].Cast<CheckBox>().CurrentValue; } }
        public static int mode { get { return comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue; } }
        // auto
        public static bool WAntiGap { get { return autoMenu["Wantigap"].Cast<CheckBox>().CurrentValue; } }
        public static bool WInterrupt { get { return autoMenu["Winterrupt"].Cast<CheckBox>().CurrentValue; } }
        // jungleclear
        public static bool QjungClear { get { return jungleMenu["QJC"].Cast<CheckBox>().CurrentValue; } }
        public static bool WjungClear { get { return jungleMenu["WJC"].Cast<CheckBox>().CurrentValue; } }
        public static bool EjungClear { get { return jungleMenu["EJC"].Cast<CheckBox>().CurrentValue; } }
        public static int ManaJungClear { get { return jungleMenu["ManaJC"].Cast<Slider>().CurrentValue; } }
        // lane clear
        public static bool QlaneClear { get { return laneMenu["QLC"].Cast<CheckBox>().CurrentValue; } }
        public static bool WlaneClear { get { return laneMenu["WLC"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElaneClear { get { return laneMenu["ELC"].Cast<CheckBox>().CurrentValue; } }
        public static int ManaLaneClear { get { return laneMenu["ManaLC"].Cast<Slider>().CurrentValue; } }
        // last hit
        public static bool QlastHit { get { return lasthitMenu["QLH"].Cast<CheckBox>().CurrentValue; } }
        public static int ManaLastHit { get { return lasthitMenu["ManaLH"].Cast<Slider>().CurrentValue; } }
        // draw
        public static bool DrawQ { get { return drawMenu["DQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawW { get { return drawMenu["DW"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawE { get { return drawMenu["DE"].Cast<CheckBox>().CurrentValue; } }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            //foreach (var item in ObjectManager.Get<Obj_AI_Base>().Where(x => x.HasBuff("RyzeE")))
            //{
            //    Render.Circle.DrawCircle(item.Position, 75, Color.Aqua);
            //}
            if (DrawQ)
                Render.Circle.DrawCircle(Player.Position, _q.Range, Color.Aqua);
            if (DrawW)
                Render.Circle.DrawCircle(Player.Position, _w.Range, Color.Purple);
            if (DrawE)
                Render.Circle.DrawCircle(Player.Position, _e.Range, Color.Yellow);

            var x = Drawing.WorldToScreen(Player.Position);
            if (drawMenu["DrawMode"].Cast<CheckBox>().CurrentValue)
            {
                if (comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue == 0) Drawing.DrawText(x[0] - 45, x[1] + 20, Color.PaleTurquoise, "Burst");
                else if (comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue == 1) Drawing.DrawText(x[0] - 45, x[1] + 20, Color.PaleTurquoise, "AoE/Shield");
            }
        }

        public static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
            {
                return;
            }
            //Game.PrintChat(spell.Name);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            // use W against gap closer
            var target = gapcloser.Sender;
            if (_w.IsReady() && target.LSIsValidTarget(_w.Range) && WAntiGap)
            {
                _w.Cast(target);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            // interrupt with W
            if (_w.IsReady() && sender.LSIsValidTarget(_w.Range) && !sender.IsZombie && WInterrupt)
            {
                _w.Cast(sender);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            ComboModeSwitch();
            //if (Helper.CanShield())
            //Game.PrintChat(Helper.CanShield().ToString());
            //Game.PrintChat(Helper.BonusMana.ToString());
            //Game.PrintChat(Helper.Qstack().ToString());
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var target = Orbwalker.LastTarget;
                if ((_w.IsReady() || (Player.Mana >= _q.ManaCost + _e.ManaCost)) && BlockAA
                    && target.LSIsValidTarget() && (!target.LSIsValidTarget(350) || Player.LSCountEnemiesInRange(800) >= 2)
                    || !target.LSIsValidTarget())
                {
                    PortAIO.OrbwalkerManager.SetAttack(false);
                }
                else
                {
                    PortAIO.OrbwalkerManager.SetAttack(true);
                }
            }
            else
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
            }
            foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget(_q.Range) && !x.IsZombie))
            {
                if (_q.IsReady() && Helper.Qdamage(hero) >= hero.Health)
                    Helper.CastQTarget(hero);
                if (_w.IsReady() && Helper.Wdamge(hero) >= hero.Health)
                    _w.Cast(hero);
                if (_e.IsReady() && Helper.Edamge(hero) >= hero.Health)
                    _e.Cast(hero);
            }
        }

        private static int _lastTick;
        private static void ComboModeSwitch()
        {
            var comboMode = mode;
            var lasttime = Utils.GameTimeTickCount - _lastTick;
            if (!comboMenu["ComboSwitch"].Cast<KeyBind>().CurrentValue ||
                lasttime <= Game.Ping)
            {
                return;
            }

            switch (comboMode)
            {
                case 0:
                    comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 1;
                    _lastTick = Utils.GameTimeTickCount + 300;
                    break;
                case 1:
                    comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 0;
                    _lastTick = Utils.GameTimeTickCount + 300;
                    break;
            }
        }
    }
}