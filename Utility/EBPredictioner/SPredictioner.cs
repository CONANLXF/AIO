using System;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using System.Collections.Generic;
using EloBuddy.SDK.Spells;
using static EloBuddy.SDK.Spell;
using EloBuddy.SDK;
using System.Linq;

using TargetSelector = PortAIO.TSManager; namespace EBPredictioner
{
    internal class SPredictioner
    {
        internal static List<SpellInfo> _spells;
        public static Menu Config;
        private static int _lastChargeTime;

        internal static AIHeroClient MyHero
        {
            get { return Player.Instance; }
        }

        public static int getHitChance
        {
            get
            {
                return Config["SPREDHITC"].Cast<Slider>().CurrentValue;
            }
        }

        public static void Initialize()
        {
            _spells = EloBuddy.SDK.Spells.SpellDatabase.GetSpellInfoList(MyHero.BaseSkinName);

            #region Initialize Menu
            Config = MainMenu.AddMenu("EBPredictioner", "asdasdasdsad");
            Config.AddGroupLabel("Taken logic from AIMBot, huge credits to iCreative.");
            Config.Add("ENABLED", new CheckBox("Enabled"));
            //Config.Add("SPREDHITC", new ComboBox("HitChance", 0, "High", "Medium", "Low"));
            Config.Add("SPREDHITC", new Slider("HitChance", 60));
            Config.AddSeparator();

            #region Initialize Spells
            Config.AddGroupLabel("Skillshots");
            var slots = new HashSet<SpellSlot>();
            foreach (var info in _spells)
            {
                slots.Add(info.Slot);
            }
            foreach (var slot in slots)
            {
                if (Config[String.Format("{0}{1}", ObjectManager.Player.ChampionName, slot)] == null)
                {
                    Config.Add(String.Format("{0}{1}", ObjectManager.Player.ChampionName, slot), new CheckBox("Convert Spell " + slot.ToString()));
                }
            }
            #endregion
            #endregion

            #region Initialize Events
            Spellbook.OnCastSpell += EventHandlers.Spellbook_OnCastSpell;
            AIHeroClient.OnProcessSpellCast += EventHandlers.Obj_AI_Hero_OnProcessSpellCast;
            Game.OnTick += Game_OnTick;
            #endregion
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (MyHero.IsDead || !Config["ENABLED"].Cast<CheckBox>().CurrentValue || (!IsCharging && !Orbwalker.CanMove))
            {
                return;
            }
            if (_lastChargeTime == 0 && IsCharging)
            {
                _lastChargeTime = Core.GameTickCount;
            }
            else if (_lastChargeTime > 0 && !IsCharging)
            {
                _lastChargeTime = 0;
            }
        }

        public static void Cast(SpellSlot slot)
        {
            var first = _spells.FirstOrDefault(spell => spell.Slot == slot && (string.IsNullOrEmpty(spell.SpellName) || string.Equals(MyHero.Spellbook.GetSpell(slot).Name, spell.SpellName, StringComparison.CurrentCultureIgnoreCase)));
            if (first != null)
            {
                var allowedCollisionCount = int.MaxValue;
                if (first.Collisions.Contains(CollisionType.AiHeroClient))
                {
                    allowedCollisionCount = 0;
                }
                else if (first.Collisions.Contains(CollisionType.ObjAiMinion))
                {
                    allowedCollisionCount = -1;
                }
                var collidesWithWall = first.Collisions.Contains(CollisionType.YasuoWall);
                SpellBase spell = null;
                switch (first.Type)
                {
                    case EloBuddy.SDK.Spells.SpellType.Self:
                        spell = new SpellBase(slot, SpellType.Self, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Circle:
                        spell = new SpellBase(slot, SpellType.Circular, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Line:
                        spell = new SpellBase(slot, SpellType.Linear, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Cone:
                        spell = new SpellBase(slot, SpellType.Cone, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Ring:
                        break;
                    case EloBuddy.SDK.Spells.SpellType.MissileLine:
                        spell = new SpellBase(slot, SpellType.Linear, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.MissileAoe:
                        spell = new SpellBase(slot, SpellType.Circular, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                }
                if (spell != null)
                {
                    if (first.Chargeable)
                    {
                        if (IsCharging)
                        {
                            var percentageGrowth = Math.Min(1 / 1000f * (Core.GameTickCount - _lastChargeTime - first.CastRangeGrowthStartTime) / first.CastRangeGrowthDuration, 1);
                            spell.Range = (first.CastRangeGrowthMax - first.CastRangeGrowthMin) * percentageGrowth + first.CastRangeGrowthMin;
                            spell.ReleaseCast();
                        }
                        else
                        {
                            spell.StartCast();
                        }
                    }
                    else if (!IsCharging)
                    {
                        spell.Cast();
                    }
                }
            }
        }

        private static bool IsCharging
        {
            get { return MyHero.Spellbook.IsCharging; }
        }
    }
}
