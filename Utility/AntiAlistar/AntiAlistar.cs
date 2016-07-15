 namespace AntiAlistar
{
    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    class AntiAlistar
    {
        #region Static Fields

        public static Spell ChampionSpell;

        public static SpellSlot FlashSlot;

        public static Menu Menu;

        public static string[] SupportedChampions = { "Vayne", "Ezreal", "Lucian", "Graves" };

        #endregion

        #region Public Methods and Operators

        /**
         *  1. Customizable hp percentage for flashing when no dashes available. - done
            2. Flash if X enemies close to alistar. - done
            3. Drawing that shows if Alistar is in range for combo - todo
            4. Drawing that shows if Alistar is in range for flash Q/flash W - todo 
            5. Anti alistar support for all possible adc champs (or more) dashes  - todo more
            6. Where to flash (backwards, teammates, etc) - half done
            Q = 365, W = 650f - ranges
         */
        public static Spell GetChampionSpell()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Vayne":
                    return new Spell(SpellSlot.E, 550f);
                case "Lucian":
                    return new Spell(SpellSlot.E, 475f);
                case "Ezreal":
                    return new Spell(SpellSlot.E, 475f);
                case "Graves":
                    return new Spell(SpellSlot.E, 425f);
            }

            return null;
        }

        public static Vector3 GetSelectedPosition(Vector3 pos, float range)
        {
            switch (getBoxItem(Menu, "com.antiali.flashPosition"))
            {
                case 0: // backwards
                    return ObjectManager.Player.ServerPosition.LSExtend(pos, -range);
                case 1: // teammates
                    var teammate = ObjectManager.Player.GetAlliesInRange(1500f).FirstOrDefault();
                    return ObjectManager.Player.Position.LSExtend(teammate?.Position ?? Game.CursorPos, range);
                case 2: // turret
                    var closestTurret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .FirstOrDefault(x => x.IsAlly && x.Health > 1 && x.LSDistance(ObjectManager.Player) < 1500f);
                    return ObjectManager.Player.Position.LSExtend(closestTurret?.Position ?? Game.CursorPos, range);
            }

            return ObjectManager.Player.ServerPosition.LSExtend(pos, -range);
        }

        private static bool AliInGame()
        {
            return HeroManager.Enemies.Any(h => !h.IsMe && h.ChampionName.Equals("Alistar"));
        }

    
        public static bool IsTargeted()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Vayne":
                    return true;
            }

            return false;
        }

        public static void DrawTextOnScreen(Vector3 location, string message, Color colour)
        {
            var world = Drawing.WorldToScreen(location);
            Drawing.DrawText(world[0] - message.Length * 5, world[1] - 200, colour, message);
        }

        private static void AlistarDrawing(EventArgs args)
        {
            var qRange = 365f;
            var wRange = 650f;
            var flashQRange = 450 + qRange;
            var flashWRange = 450 + wRange;
            if (!getCheckBoxItem(Menu, "com.antiali.drawRange")) return;
            var alistar = HeroManager.Enemies.FirstOrDefault(x => x.ChampionName == "Alistar");
            if (alistar == null) return;

            var position = new Vector3(
                ObjectManager.Player.Position.X, 
                ObjectManager.Player.Position.Y - 30, 
                ObjectManager.Player.Position.Z);

            if (ObjectManager.Player.LSDistance(alistar) <= wRange)
            {
                DrawTextOnScreen(position, "Alistar can W", Color.Red);
            }
            else if (ObjectManager.Player.LSDistance(alistar) <= flashWRange)
            {
                DrawTextOnScreen(position, "Alistar Can Flash W", Color.Red);
            }
            else if (ObjectManager.Player.LSDistance(alistar) <= flashQRange)
            {
                DrawTextOnScreen(position, "Alistar Can Flash Q", Color.Red);
            }
            else
            {
                DrawTextOnScreen(position, "Safe from Alistar!", Color.GreenYellow);
            }
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


        public static void OnLoad()
        {
            if (!AliInGame())
            {
                return;
            }

            FlashSlot = ObjectManager.Player.GetSpellSlot("summonerflash");

            if (SupportedChampions.Contains(ObjectManager.Player.ChampionName))
            {
                ChampionSpell = GetChampionSpell();
            }

            // Menu

            Menu = MainMenu.AddMenu("Anti Alistar", "com.antiali");
            Menu.Add("com.antiali.useFlash", new CheckBox("Use Flash", false));
            Menu.AddLabel("Will only use flash if spell is on cd");
            Menu.Add("com.antiali.flashPercent", new Slider("Health For Flash", 15, 0, 100));
            Menu.Add("com.antiali.flashAmount", new Slider("Flash if x enemies", 2, 1, 5));
            Menu.Add("com.antiali.useSpell", new CheckBox("Use Spell", false));
            Menu.AddLabel("Will dash / flash to pos if its not founud it will cast to cursor pos.");
            Menu.Add("com.antiali.flashPosition", new ComboBox("Flash Position", 0, "Backwards", "Teammates", "Towards Turret"));
            Menu.Add("com.antiali.drawRange", new CheckBox("Draw Text for ali range", false));


            // Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnGapcloser;
            Drawing.OnDraw += AlistarDrawing;
        }

        #endregion

        #region Methods

        private static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            if (ObjectManager.Player.Position.LSDistance(gapcloser.End) > 365f) return;

            if (gapcloser.Sender.IsEnemy && gapcloser.SkillType == GapcloserType.Targeted
                && gapcloser.Sender.ChampionName == "Alistar")
            {
                if (FlashSlot.IsReady()
                    && ObjectManager.Player.GetEnemiesInRange(1500f).Count
                    >= getSliderItem(Menu, "com.antiali.flashAmount")
                    && ObjectManager.Player.HealthPercent
                    < getSliderItem(Menu, "com.antiali.flashPercent"))
                {
                    if (SupportedChampions.Contains(ObjectManager.Player.ChampionName) && ChampionSpell.IsReady())
                    {
                        return;
                    }

                    ObjectManager.Player.Spellbook.CastSpell(
                        FlashSlot, 
                        GetSelectedPosition(gapcloser.Sender.Position, 450));
                }

                if (SupportedChampions.Contains(ObjectManager.Player.ChampionName))
                {
                    if (!getCheckBoxItem(Menu, "com.antiali.useSpell") || !ChampionSpell.IsReady()) return;

                    if (IsTargeted())
                    {
                        ChampionSpell.CastOnUnit(gapcloser.Sender);
                    }
                    else
                    {
                        var position = GetSelectedPosition(gapcloser.Sender.Position, ChampionSpell.Range);
                        ChampionSpell.Cast(position);
                    }
                }
            }
        }
        #endregion
    }
}