#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Spells/CoreSpell.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Linq;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Spells
{
    public class CoreSpell
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual float Range { get; set; }
        internal virtual MenuType[] Category { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent => Menu.Parent;
        public AIHeroClient Player => ObjectManager.Player;

        public AIHeroClient LowTarget
        {
            get
            {
                return ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.LSIsValidTarget(Range))
                    .OrderBy(ene => ene.Health/ene.MaxHealth*100).First();
            }
        }

        public CoreSpell CreateMenu(Menu root)
        {
            try
            {
                if (Player.GetSpellSlot(Name) == SpellSlot.Unknown)
                    return null;

                Menu = root;//.AddSubMenu(DisplayName, "m" + Name);
                Menu.AddGroupLabel(DisplayName);

                Menu.Add("use" + Name, new CheckBox("Use " + DisplayName, false));

                if (Category.Any(t => t == MenuType.Stealth))
                    Menu.Add("Stealth" + Name + "pct", new CheckBox("Use on Stealth"));

                if (Category.Any(t => t == MenuType.SlowRemoval))
                    Menu.Add("use" + Name + "sr", new CheckBox("Use on Slows"));

                if (Category.Any(t => t == MenuType.EnemyLowHP)) 
                    Menu.Add("enemylowhp" + Name + "pct", new Slider("Use on Enemy HP % <=", DefaultHP));

                if (Category.Any(t => t == MenuType.SelfLowHP))
                    Menu.Add("selflowhp" + Name + "pct", new Slider("Use on Hero HP % <=", DefaultHP));

                if (Category.Any(t => t == MenuType.SelfMuchHP))
                    Menu.Add("selfmuchhp" + Name + "pct", new Slider("Use on Hero Dmg Dealt % >=", 25));

                if (Category.Any(t => t == MenuType.SelfLowMP))
                    Menu.Add("selflowmp" + Name + "pct", new Slider("Use on Hero Mana % <=", DefaultMP));

                if (Category.Any(t => t == MenuType.SelfCount))
                    Menu.Add("selfcount" + Name, new Slider("Use on # Near Hero >=", 3, 1, 5));

                if (Category.Any(t => t == MenuType.SelfMinMP))
                    Menu.Add("selfminmp" + Name + "pct", new Slider("Minimum Mana/Energy %", 40));

                if (Category.Any(t => t == MenuType.SelfMinHP))
                    Menu.Add("selfminhp" + Name + "pct", new Slider("Minimum HP %", 40));

                if (Category.Any(t => t == MenuType.SpellShield))
                {
                    Menu.Add("ss" + Name + "all", new CheckBox("Use on Any Spell", false));
                    Menu.Add("ss" + Name + "cc", new CheckBox("Use on Crowd Control"));
                }

                if (Category.Any(t => t == MenuType.Zhonyas))
                {
                    Menu.Add("use" + Name + "norm", new CheckBox("Use on Dangerous (Spells)", false));
                    Menu.Add("use" + Name + "ulti", new CheckBox("Use on Dangerous (Ultimates Only)"));
                }

                if (Category.Any(t => t == MenuType.ActiveCheck))
                    Menu.Add("mode" + Name, new ComboBox("Mode: ", 0, "Always", "Combo"));
                Menu.AddSeparator();
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("<font color=\"#FFF280\">Exception thrown at CoreSpell.CreateMenu: </font>: " + e.Message);
            }

            return this;
        }

        public void CastOnBestTarget(AIHeroClient primary, bool nonhero = false)
        {
            if (LowTarget != null)
            {
                if (!Player.LSIsRecalling() &&
                    !Player.Spellbook.IsChanneling &&
                    !Player.IsChannelingImportantSpell())
                {
                    if (Player.Spellbook.CastSpell(Player.GetSpellSlot(Name), LowTarget))
                    {
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = 100;
                    }
                }
            }
        }

        public bool IsReady()
        {
            var ready = Player.GetSpellSlot(Name).IsReady();
            return ready;
        }

        public void UseSpell(bool combo = false)
        {
            if (!combo || Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (!Player.LSIsRecalling() &&
                        !Player.Spellbook.IsChanneling && 
                        !Player.IsChannelingImportantSpell() &&
                        !Player.Spellbook.IsCastingSpell)
                    {
                        if (Player.Spellbook.CastSpell(Player.GetSpellSlot(Name)))
                        {
                            Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                            Activator.LastUsedDuration = 100;
                        }
                    }
                }
            }
        }

        public void UseSpellTowards(Vector3 targetpos, bool combo = false)
        {
            if (!combo || Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (!Player.LSIsRecalling() &&
                        !Player.Spellbook.IsChanneling &&
                        !Player.IsChannelingImportantSpell() &&
                        !Player.Spellbook.IsCastingSpell)
                    {
                        if (Player.Spellbook.CastSpell(Player.GetSpellSlot(Name), targetpos))
                        {
                            Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                            Activator.LastUsedDuration = 100;  
                        }
                    }
                }
            }
        }

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (!Player.LSIsRecalling() &&
                        !Player.Spellbook.IsChanneling &&
                        !Player.IsChannelingImportantSpell() &&
                        !Player.Spellbook.IsCastingSpell)
                    {
                        if (Player.Spellbook.CastSpell(Player.GetSpellSlot(Name), target))
                        {
                            Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                            Activator.LastUsedDuration = 100;                
                        }
                    }
                }
            }
        }

        public virtual void OnTick(EventArgs args)
        {
        }
    }
}
