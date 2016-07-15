using System;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using Color = SharpDX.Color;
using static Firestorm_AIO.Helpers.Helpers;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using Firestorm_AIO.DataBases;

 namespace Firestorm_AIO.Bases
{
    public abstract class ChampionBase
    {
        public static AIHeroClient Target;

        public static Firestorm_AIO.Enums.Champion MyChampion = Me.GetChampion();

        public static bool HasMana = true;

        #region Spells

        public static LeagueSharp.SDK.Spell Q;
        public static LeagueSharp.SDK.Spell W;
        public static LeagueSharp.SDK.Spell E;
        public static LeagueSharp.SDK.Spell R;

        #endregion Spells

        #region Modes

        public abstract void Init();
        public abstract void Menu();
        public abstract void Active();
        public abstract void Combo();
        public abstract void Mixed();
        public abstract void LaneClear();
        public abstract void LastHit();
        public abstract void KillSteal();
        //
        public abstract void Draw();

        #endregion Modes

        #region Menus

        public Menu MainMenu;
        public Menu ComboMenu;
        public Menu MixedMenu;
        public Menu LaneClearMenu;
        public Menu JungleClearMenu;
        public Menu LastHitMenu;
        public Menu KillstealMenu;
        public Menu DrawingMenu;
        public Menu MiscMenu;

        //Colors
        private bool DrawReady;

        private bool DrawQ;
        public System.Drawing.Color QColor = System.Drawing.Color.Black;

        private bool DrawW;
        public System.Drawing.Color WColor = System.Drawing.Color.Blue;

        private bool DrawE;
        public System.Drawing.Color EColor = System.Drawing.Color.DarkKhaki;

        private bool DrawR;
        public System.Drawing.Color RColor = System.Drawing.Color.ForestGreen;

        public void DrawSpell(LeagueSharp.SDK.Spell spell)
        {
            if (spell == null) return;

            var color = System.Drawing.Color.White;
            var CanDraw = true;

            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    color = QColor;
                    CanDraw = DrawQ;
                    break;
                case SpellSlot.W:
                    color = WColor;
                    CanDraw = DrawW;
                    break;
                case SpellSlot.E:
                    color = EColor;
                    CanDraw = DrawE;
                    break;
                case SpellSlot.R:
                    color = RColor;
                    CanDraw = DrawR;
                    break;
            }

            if (CanDraw && (!DrawReady || spell.IsReady())) Render.Circle.DrawCircle(Me.Position, spell.Range, color);
        }

        #endregion Menus

        #region Functions
        public bool GetBoolValue(LeagueSharp.SDK.Spell spell, Menu menu)
        {
            return menu["use" + spell.Slot].Cast<CheckBox>().CurrentValue;
        }

        public bool GetBoolValue(string name, Menu menu)
        {
            return menu[name].Cast<CheckBox>().CurrentValue;
        }

        #endregion Functions

        public void Load()
        {
            Init();

            #region InitialMenu

            MainMenu = EloBuddy.SDK.Menu.MainMenu.AddMenu("Firestorm AIO: " + Me.ChampionName, "aioFire" + Me.ChampionName);

            ComboMenu = MainMenu.AddSubMenu("Combo Menu", "comboMenu" + Me.ChampionName);
            MixedMenu = MainMenu.AddSubMenu("Mixed Menu", "mixedMenu" + Me.ChampionName);
            LaneClearMenu = MainMenu.AddSubMenu("LaneClear Menu", "laneMenu" + Me.ChampionName);
            JungleClearMenu = MainMenu.AddSubMenu("JungleClear Menu", "jungleMenu" + Me.ChampionName);
            LastHitMenu = MainMenu.AddSubMenu("LastHit Menu", "lastMenu" + Me.ChampionName);
            KillstealMenu = MainMenu.AddSubMenu("KillSteal Menu", "ksMenu" + Me.ChampionName);

            DrawingMenu = MainMenu.AddSubMenu("Drawing Menu", "drawMenu" + Me.ChampionName);
            var drawReady = DrawingMenu.Add("drawReady", new CheckBox("Only draw spell if it is ready"));
            DrawReady = drawReady.Cast<CheckBox>().CurrentValue;
            drawReady.OnValueChange += (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args) =>
            {
                DrawReady = args.NewValue;
            };

            MiscMenu = MainMenu.AddSubMenu("Misc Menu", "miscMenu" + Me.ChampionName);

            if (HasMana)
            {
                MixedMenu.Add("mixedMana", new Slider("Mana percent must be >= x%", 30, 0, 101));
                LaneClearMenu.Add("laneClearMana", new Slider("Mana percent must be >= x%", 50, 0, 101));
                JungleClearMenu.Add("jungleClearMana", new Slider("Mana percent must be >= x%", 20, 0, 101));
                LastHitMenu.Add("lastHitMana", new Slider("Mana percent must be >= x%", 40, 0, 101));
            }

            if (Q != null)
            {
                var qDraw = DrawingMenu.Add("qDraw", new CheckBox("Draw Q ?", true));
                DrawQ = qDraw.Cast<CheckBox>().CurrentValue;
                qDraw.OnValueChange += (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args) => { DrawQ = args.NewValue; };
            }

            if (W != null)
            {
                var wDraw = DrawingMenu.Add("wDraw", new CheckBox("Draw W ?", true));
                DrawW = wDraw.Cast<CheckBox>().CurrentValue;
                wDraw.OnValueChange += (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args) => { DrawW = args.NewValue; };
            }

            if (E != null)
            {
                var eDraw = DrawingMenu.Add("eDraw", new CheckBox("Draw E ?", true));
                DrawE = eDraw.Cast<CheckBox>().CurrentValue;
                eDraw.OnValueChange += (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args) => { DrawE = args.NewValue; };
            }

            if (R != null)
            {
                var rDraw = DrawingMenu.Add("rDraw", new CheckBox("Draw R ?", true));
                DrawR = rDraw.Cast<CheckBox>().CurrentValue;
                rDraw.OnValueChange += (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args) => { DrawR = args.NewValue; };
            }

            #endregion InitialMenu

            Menu();

            Game.OnUpdate += Game_OnUpdate;

            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            Active();

            if (Me.IsDead) return;

            KillSteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Target == null) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Mixed();
            }

            if(!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) return;


        }
        
        private void Drawing_OnDraw(EventArgs args)
        {
            if (Me.IsDead) return;

            Draw();

            DrawSpell(Q);
            DrawSpell(W);
            DrawSpell(E);
            DrawSpell(R);
        }
    }
}
