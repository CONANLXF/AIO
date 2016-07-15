using TargetSelector = PortAIO.TSManager; namespace iTwitch
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Spell = LeagueSharp.Common.Spell;
    using Utility = LeagueSharp.Common.Utility;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using EloBuddy;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy.SDK;

    public class Twitch
    {
        #region Static Fields

        /// <summary>
        ///     The dictionary to call the Spell slot and the Spell Class
        /// </summary>
        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
                                                                         {
                                                                             { SpellSlot.Q, new Spell(SpellSlot.Q) },
                                                                             { SpellSlot.W, new Spell(SpellSlot.W, 950f) },
                                                                             { SpellSlot.E, new Spell(SpellSlot.E, 1100) },
                                                                             { SpellSlot.R, new Spell(SpellSlot.R) },
                                                                         };

        #endregion

        #region Fields

        private static Menu Menu;
        public static Menu comboOptions, harassOptions, laneclearOptions, miscOptions, drawOptions;

        #endregion

        #region Public Methods and Operators

        public static void LoadMenu()
        {
            Menu = MainMenu.AddMenu("iTwitch 2.0", "com.itwitch");

            comboOptions = Menu.AddSubMenu("iTwitch 2.0 - Combo", "com.itwitch.combo");
            comboOptions.Add("com.itwitch.combo.useW", new CheckBox("Use W", true));
            comboOptions.Add("com.itwitch.combo.useEKillable", new CheckBox("Use E Killable", true));



            harassOptions = Menu.AddSubMenu("iTwitch 2.0 - Harass", "com.itwitch.harass");
            harassOptions.Add("com.itwitch.harass.useW", new CheckBox("Use W", true));
            harassOptions.Add("com.itwitch.harass.useEKillable", new CheckBox("Use E", true));


            miscOptions = Menu.AddSubMenu("iTwitch 2.0 - Misc", "com.itwitch.misc");
            miscOptions.Add("com.itwitch.misc.autoYo", new CheckBox("Youmuus with R", true));
            miscOptions.Add("com.itwitch.misc.noWTurret", new CheckBox("Don't W Under Tower", true));
            miscOptions.Add("com.itwitch.misc.noWAA", new Slider("No W if x aa can kill", 2, 0, 10));
            miscOptions.Add("com.itwitch.misc.saveManaE", new CheckBox("Save Mana for E", true));
            miscOptions.Add("com.itwitch.misc.recall", new KeyBind("Stealth Recall", false, KeyBind.BindTypes.HoldActive, 'T'));


            drawOptions = Menu.AddSubMenu("iTwitch 2.0 - Drawing", "com.itwitch.drawing");
            drawOptions.Add("com.itwitch.drawing.drawERange", new CheckBox("Draw E Range", true));
            drawOptions.Add("com.itwitch.drawing.drawRRange", new CheckBox("Draw R Range", true));
            drawOptions.Add("com.itwitch.drawing.drawQTime", new CheckBox("Draw Q Time", true));
            drawOptions.Add("com.itwitch.drawing.drawEStacks", new CheckBox("Draw E Stacks", true));
            drawOptions.Add("com.itwitch.drawing.drawEStackT", new CheckBox("Draw E Stack Time", true));
            drawOptions.Add("com.itwitch.drawing.drawRTime", new CheckBox("Draw R Time", true));
            drawOptions.Add("com.itwitch.drawing.eDamage", new CheckBox("Draw E Damage on Enemies", true));


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

        public static void LoadSpells()
        {
            Spells[SpellSlot.W].SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
        }

        public static void OnCombo()
        {
            if (getCheckBoxItem(comboOptions, "com.itwitch.combo.useW") && Spells[SpellSlot.W].IsReady())
            {
                if (getCheckBoxItem(miscOptions, "com.itwitch.misc.saveManaE")
                    && ObjectManager.Player.Mana <= Spells[SpellSlot.E].ManaCost + Spells[SpellSlot.W].ManaCost)
                {
                    return;
                }

                if (getCheckBoxItem(miscOptions, "com.itwitch.misc.noWTurret") && ObjectManager.Player.UnderTurret(true))
                {
                    return;
                }

                var wTarget = TargetSelector.GetTarget(Spells[SpellSlot.W].Range, DamageType.Physical);

                if (wTarget != null
                    && wTarget.Health
                    < ObjectManager.Player.LSGetAutoAttackDamage(wTarget, true)
                    * getSliderItem(miscOptions, "com.itwitch.misc.noWAA"))
                    return;

                if (wTarget.LSIsValidTarget(Spells[SpellSlot.W].Range)
                    && !ObjectManager.Player.HasBuff("TwitchHideInShadows"))
                {
                    var prediction = Spells[SpellSlot.W].GetPrediction(wTarget);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Spells[SpellSlot.W].Cast(prediction.CastPosition);
                    }
                }
            }
        }

        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Twitch") return;

            CustomDamageIndicator.Initialize(Extensions.GetPoisonDamage);

            LoadSpells();
            LoadMenu();

            Spellbook.OnCastSpell += (sender, eventArgs) =>
            {
                if (eventArgs.Slot == SpellSlot.Recall && Spells[SpellSlot.Q].IsReady()
                    && getKeyBindItem(miscOptions, "com.itwitch.misc.recall"))
                {
                    Spells[SpellSlot.Q].Cast();
                    Utility.DelayAction.Add(
                        (int)(Spells[SpellSlot.Q].Delay + 300),
                        () => ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall));
                    eventArgs.Process = false;
                    return;
                }

                if (eventArgs.Slot == SpellSlot.R && getCheckBoxItem(miscOptions, "com.itwitch.misc.autoYo"))
                {
                    if (!HeroManager.Enemies.Any(x => ObjectManager.Player.LSDistance(x) <= Spells[SpellSlot.R].Range)) return;

                    if (Items.HasItem(ItemData.Youmuus_Ghostblade.Id))
                    {
                        Items.UseItem(ItemData.Youmuus_Ghostblade.Id);
                    }
                }

                if (getCheckBoxItem(miscOptions, "com.itwitch.misc.saveManaE") && eventArgs.Slot == SpellSlot.W)
                {
                    if (ObjectManager.Player.Mana <= Spells[SpellSlot.E].ManaCost + 10)
                    {
                        eventArgs.Process = false;
                    }
                }
            };

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            LSEvents.AfterAttack += AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

        }

        private static void AfterAttack(AfterAttackArgs args)
        {

        }

        public static void OnHarass()
        {
            if (getCheckBoxItem(harassOptions, "com.itwitch.harass.useW") && Spells[SpellSlot.W].IsReady())
            {
                var wTarget = TargetSelector.GetTarget(Spells[SpellSlot.W].Range, DamageType.Physical);
                if (wTarget.LSIsValidTarget(Spells[SpellSlot.W].Range))
                {
                    var prediction = Spells[SpellSlot.W].GetPrediction(wTarget);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Spells[SpellSlot.W].Cast(prediction.CastPosition);
                    }
                }
            }
        }

        #endregion

        #region Methods

        private static void OnDraw(EventArgs args)
        {
            CustomDamageIndicator.DrawingColor = System.Drawing.Color.DarkOliveGreen;
            CustomDamageIndicator.Enabled = getCheckBoxItem(drawOptions, "com.itwitch.drawing.eDamage");

            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawRRange"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[SpellSlot.R].Range, Color.BlueViolet);
            }

            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawERange"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[SpellSlot.E].Range, Color.BlueViolet);
            }

            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawQTime")
                && ObjectManager.Player.HasBuff("TwitchHideInShadows"))
            {
                var position = new Vector3(
                    ObjectManager.Player.Position.X,
                    ObjectManager.Player.Position.Y - 30,
                    ObjectManager.Player.Position.Z);
                position.DrawTextOnScreen(
                    "Stealth:  " + $"{ObjectManager.Player.GetRemainingBuffTime("TwitchHideInShadows"):0.0}",
                    Color.AntiqueWhite);
            }

            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawRTime")
                && ObjectManager.Player.HasBuff("TwitchFullAutomatic"))
            {
                ObjectManager.Player.Position.DrawTextOnScreen(
                    "Ultimate:  " + $"{ObjectManager.Player.GetRemainingBuffTime("TwitchFullAutomatic"):0.0}",
                    Color.AntiqueWhite);
            }

            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawEStacks"))
            {
                foreach (var source in
                    HeroManager.Enemies.Where(x => x.HasBuff("TwitchDeadlyVenom") && !x.IsDead && x.IsVisible))
                {
                    var position = new Vector3(source.Position.X, source.Position.Y + 10, source.Position.Z);
                    position.DrawTextOnScreen($"{"Stacks: " + source.GetPoisonStacks()}", Color.AntiqueWhite);
                }
            }

            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawEStackT"))
            {
                foreach (var source in
                    HeroManager.Enemies.Where(x => x.HasBuff("TwitchDeadlyVenom") && !x.IsDead && x.IsVisible))
                {
                    var position = new Vector3(source.Position.X, source.Position.Y - 30, source.Position.Z);
                    position.DrawTextOnScreen(
                        "Stack Timer:  " + $"{source.GetRemainingBuffTime("TwitchDeadlyVenom"):0.0}",
                        Color.AntiqueWhite);
                }
            }
        }
        
        private static void OnUpdate(EventArgs args)
        {
            if (getKeyBindItem(miscOptions, "com.itwitch.misc.recall"))
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall);
            }

            if (getCheckBoxItem(comboOptions, "com.itwitch.combo.useEKillable") && Spells[SpellSlot.E].IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(Spells[SpellSlot.E].Range) && (enemy.HasBuff("TwitchDeadlyVenom") || enemy.HasBuff("twitchdeadlyvenom"))))
                {
                    if (enemy.Health + enemy.AttackShield < EDamage(enemy))
                    {
                        Spells[SpellSlot.E].Cast();
                    }
                }
            }

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                OnCombo();
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                OnHarass();
            }
        }

        /* Doc7 - Twitch for calculations */

        public static int[] stack = { 0, 15, 20, 25, 30, 35 };
        public static int[] _base = { 0, 20, 35, 50, 65, 80 };

        private static float EDamage(Obj_AI_Base target)
        {
            var stacks = Stack(target);
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, _base[Spells[SpellSlot.E].Level] + stacks * (0.25f * ObjectManager.Player.FlatPhysicalDamageMod + 0.2f * ObjectManager.Player.FlatMagicDamageMod + stack[Spells[SpellSlot.E].Level]));
        }

        private static int Stack(Obj_AI_Base obj)
        {
            var Ec = 0;
            for (var t = 1; t < 7; t++)
            {
                if (ObjectManager.Get<Obj_GeneralParticleEmitter>().Any(s => s.Position.Distance(obj.ServerPosition) <= 175 && s.Name == "twitch_poison_counter_0" + t + ".troy"))
                {
                    Ec = t;
                }
            }
            return Ec;
        }
        #endregion
    }
}