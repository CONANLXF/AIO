#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Items/CoreItem.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

 namespace Activators.Items
{
    public class CoreItem
    {
        internal virtual int Id { get; set; }
        internal virtual int Priority { get; set; }
        internal virtual int Duration { get; set; }
        internal virtual bool Needed { get; set; }
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual float Range { get; set; }
        internal virtual MenuType[] Category { get; set; }
        internal virtual MapType[] Maps { get; set; }

        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent => Menu.Parent;
        public AIHeroClient Player => ObjectManager.Player;

        public Base.Champion Tar
        {
            get
            {
                return
                    Activator.Heroes.Where(
                        hero => hero.Player.IsEnemy && hero.Player.LSIsValidTarget(Range + 100) &&
                               !hero.Player.IsZombie).OrderBy(x => x.Player.LSDistance(Game.CursorPos)).FirstOrDefault();
            }
        }

        public static IEnumerable<CoreItem> PriorityList()
        {
            var hpi = from ii in Lists.Items
                      where LeagueSharp.Common.Items.CanUseItem(ii.Id) && ii.Needed
                      orderby ii.Menu["prior" + ii.Name].Cast<Slider>().CurrentValue descending
                      select ii;

            return hpi;
        }

        public bool IsReady()
        {
            var ready = LeagueSharp.Common.Items.CanUseItem(Id);
            return ready;
        }

        public int[] Excluded => new[] { 3090, 3157, 3137, 3139, 3140, 3222 };

        public void UseItem(bool combo = false)
        {
            Console.WriteLine("1");
            if (IsReady())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(80 - Priority * 10, () => Needed = true);
            }

            if (!combo || Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
            {
                if (PriorityList().Any() && Name == PriorityList().First().Name)
                {
                    if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Duration || Excluded.Any(ex => Id.Equals(ex)))
                    {
                        if (!Activator.Player.HasBuffOfType(BuffType.Invisibility))
                        {
                            if (LeagueSharp.Common.Items.UseItem(Id))
                            {
                                Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                                Activator.LastUsedDuration = Duration;
                                LeagueSharp.Common.Utility.DelayAction.Add(100, () => Needed = false);
                            }
                        }
                    }
                }
            }
        }

        public void UseItem(Obj_AI_Base target, bool combo = false)
        {
            if (IsReady())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(80 - Priority * 10, () => Needed = true);
            }

            if (!combo || Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
            {
                if (PriorityList().Any() && Name == PriorityList().First().Name)
                {
                    if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Duration || Excluded.Any(ex => Id.Equals(ex)))
                    {
                        if (!Activator.Player.HasBuffOfType(BuffType.Invisibility))
                        {
                            if (LeagueSharp.Common.Items.UseItem(Id, target))
                            {
                                Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                                Activator.LastUsedDuration = Duration;
                                LeagueSharp.Common.Utility.DelayAction.Add(100, () => Needed = false);
                            }
                        }
                    }
                }
            }
        }

        public void UseItem(Vector3 pos, bool combo = false)
        {
            if (IsReady())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(80 - Priority * 10, () => Needed = true);
            }

            if (!combo || Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
            {
                if (PriorityList().Any() && Name == PriorityList().First().Name)
                {
                    if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Duration || Excluded.Any(ex => Id.Equals(ex)))
                    {
                        if (!Activator.Player.HasBuffOfType(BuffType.Invisibility))
                        {
                            if (LeagueSharp.Common.Items.UseItem(Id, pos))
                            {
                                Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                                Activator.LastUsedDuration = Duration;
                                LeagueSharp.Common.Utility.DelayAction.Add(100, () => Needed = false);
                            }
                        }
                    }
                }
            }
        }

        public CoreItem CreateMenu(Menu root)
        {
            try
            {
                Menu = root;//.AddSubMenu(Name, "m" + Name);
                Menu.AddGroupLabel(Name);
                Menu.Add("use" + Name, new CheckBox("Use " + DisplayName ?? Name));
                Menu.Add("prior" + Name, new Slider(DisplayName + " Priority [7 == Highest]", Priority, 1, 7));

                if (Category.Any(t => t == MenuType.SelfLowHP) && (Name.Contains("Pot") || Name.Contains("Flask") || Name.Contains("Biscuit")))
                {
                    Menu.Add("use" + Name + "cbat", new CheckBox("Use Only In Combat", false));
                }

                if (Category.Any(t => t == MenuType.EnemyLowHP))
                {
                    Menu.Add("enemylowhp" + Name + "pct", new Slider("Use on Enemy HP % <=", Name == "Botrk" || Name == "Cutlass" ? 65 : DefaultHP));
                }

                if (Category.Any(t => t == MenuType.SelfLowHP))
                    Menu.Add("selflowhp" + Name + "pct", new Slider("Use on Hero HP % <=", DefaultHP <= 35 || DefaultHP >= 90 ? (Name == "Botrk" || Name == "Cutlass" ? 65 : 35) : 55));

                if (Category.Any(t => t == MenuType.SelfMuchHP))
                    Menu.Add("selfmuchhp" + Name + "pct", new Slider("Use on Hero Dmg Dealt % >=", Duration == 101 ? 30 : 45));

                if (Category.Any(t => t == MenuType.SelfLowMP))
                    Menu.Add("selflowmp" + Name + "pct", new Slider("Use on Hero Mana % <=", DefaultMP));

                if (Category.Any(t => t == MenuType.SelfCount))
                    Menu.Add("selfcount" + Name, new Slider("Use On Enemy Near Count >=", 2, 1, 5));

                if (Category.Any(t => t == MenuType.SelfMinMP))
                    Menu.Add("selfminmp" + Name + "pct", new Slider("Minimum Mana %", 55));

                if (Category.Any(t => t == MenuType.SelfMinHP))
                    Menu.Add("selfminhp" + Name + "pct", new Slider("Minimum HP %", 55));

                if (Category.Any(t => t == MenuType.Zhonyas))
                {
                    Menu.Add("use" + Name + "norm", new CheckBox("Use on Dangerous (Spells)", false));
                    Menu.Add("use" + Name + "ulti", new CheckBox("Use on Dangerous (Ultimates Only)"));
                }

                if (Category.Any(t => t == MenuType.Cleanse))
                {
                    if (Id == 3222)
                    {
                        Menu.AddGroupLabel(Name + " Unique Buffs");
                        foreach (var b in Data.Buffdata.BuffList.Where(x => x.MenuName != null && (x.Cleanse || x.DoT)).OrderByDescending(x => x.DoT).ThenBy(x => x.Evade).ThenBy(x => x.MenuName))
                        {
                            string xdot = b.DoT && b.Cleanse ? "[Danger]" : (b.DoT ? "[DoT]" : "[Danger]");

                            if (b.Champion != null)
                                foreach (var ene in Activator.Heroes.Where(x => x.Player.IsEnemy && b.Champion == x.Player.ChampionName))
                                    Menu.Add(Name + b.Name + "cc", new CheckBox(b.MenuName + " " + xdot));
                            else
                                Menu.Add(Name + b.Name + "cc", new CheckBox(b.MenuName + " " + xdot));
                        }
                    }

                    Menu.AddGroupLabel(Name + " Buff Types");
                    Menu.Add(Name + "cexh", new CheckBox("Exhaust", true));
                    Menu.Add(Name + "csupp", new CheckBox("Supression"));
                    Menu.Add(Name + "cstun", new CheckBox("Stuns"));
                    Menu.Add(Name + "ccharm", new CheckBox("Charms"));
                    Menu.Add(Name + "ctaunt", new CheckBox("Taunts"));
                    Menu.Add(Name + "cflee", new CheckBox("Flee/Fear"));
                    Menu.Add(Name + "csnare", new CheckBox("Snares"));
                    Menu.Add(Name + "cpolymorph", new CheckBox("Polymorphs"));
                    Menu.Add(Name + "csilence", new CheckBox("Silences", false));
                    Menu.Add(Name + "cblind", new CheckBox("Blinds", false));
                    Menu.Add(Name + "cslow", new CheckBox("Slows", false));
                    if (Id == 3222)
                        Menu.Add(Name + "cpoison", new CheckBox("Poisons", false));

                    Menu.Add("use" + Name + "number", new Slider("Min Buffs to Use", DefaultHP / 5, 1, 5));
                    Menu.AddSeparator();
                    Menu.Add("use" + Name + "time", new Slider("Min Durration to Use", 500, 250, 2000));
                    Menu.AddLabel("^ Will not use unless the buff durration (stun, snare, etc) last at least this long (ms, 500 = 0.5 seconds)");
                    Menu.AddSeparator();
                    if (Id == 3222)
                    {
                        Menu.Add("use" + Name + "od", new CheckBox("Use for Unique Only", false));
                        Menu.Add("use" + Name + "dot", new Slider("Use for DoTs only if HP% <", 35));
                    }

                    Menu.Add("use" + Name + "delay", new Slider("Activation Delay (in ms)", 55, 0, 500));
                }

                if (Category.Any(t => t == MenuType.ActiveCheck))
                    Menu.Add("mode" + Name, new ComboBox("Mode: ", 0, "Always", "Combo"));

                Menu.AddSeparator();
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("<font color=\"#FFF280\">Exception thrown at CoreItem.CreateMenu: </font>: " + e.Message);
            }

            return this;
        }

        public virtual void OnTick(EventArgs args)
        {

        }
    }
}
