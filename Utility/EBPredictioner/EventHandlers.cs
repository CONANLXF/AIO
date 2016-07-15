using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using ShineCommon;
using EloBuddy;
using EloBuddy.SDK;
using SebbyLib;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace EBPredictioner
{
    public class EventHandlers
    {
        private static bool[] handleEvent = { true, true, true, true };
        public static Menu menu;

        public static void Game_OnGameLoad(EventArgs args)
        {
            SPredictioner.Initialize();
            menu = SPredictioner.Config;
        }

        public static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                SpellSlot slot = ObjectManager.Player.GetSpellSlot(args.SData.Name);
                if (!ShineCommon.Utility.IsValidSlot(slot))
                    return;

                if (!handleEvent[(int)slot])
                {
                    if (SPredictioner._spells[(int)slot] != null)
                    {
                        handleEvent[(int)slot] = true;
                    }
                }
            }
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
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
        
        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                if (getCheckBoxItem(menu, "ENABLED") && (PortAIO.OrbwalkerManager.isComboActive || PortAIO.OrbwalkerManager.isHarassActive))
                {
                    if (menu[String.Format("{0}{1}", ObjectManager.Player.ChampionName, args.Slot)] == null)
                        return;

                    if (!ShineCommon.Utility.IsValidSlot(args.Slot))
                        return;

                    if (SPredictioner._spells[(int)args.Slot] == null)
                        return;

                    if (!getCheckBoxItem(menu, String.Format("{0}{1}", ObjectManager.Player.ChampionName, args.Slot)))
                        return;

                    if (handleEvent[(int)args.Slot])
                    {
                        args.Process = false;
                        handleEvent[(int)args.Slot] = false;
                        var enemy = TargetSelector.GetTarget(SPredictioner._spells[(int)args.Slot].Range, DamageType.Physical);

                        if (enemy != null)
                        {
                            args.Process = false;
                            SPredictioner.Cast(args.Slot);
                        }
                    }
                }
            }
        }
    }
}
