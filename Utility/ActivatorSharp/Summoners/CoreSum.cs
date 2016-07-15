#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Summoners/CoreSum.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Summoners
{
    public class CoreSum
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual string[] ExtraNames { get; set; }
        internal virtual float Range { get; set; }
        internal virtual int Duration { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent => Menu.Parent;
        public SpellSlot Slot => Player.GetSpellSlot(Name);
        public AIHeroClient Player => ObjectManager.Player;

        public CoreSum CreateMenu(Menu root)
        {
            try
            {
                Menu = root;//.AddSubMenu(DisplayName, "m" + Name);

                Menu.AddGroupLabel(DisplayName);

                if (!Name.Contains("smite") && !Name.Contains("teleport"))
                    Menu.Add("use" + Name, new CheckBox("Use " + DisplayName));
 
                if (Name == "summonersnowball")
                    Activator.UseEnemyMenu = true;

                if (Name == "summonerheal")
                {
                    Activator.UseAllyMenu = true;
                    Menu.Add("selflowhp" + Name + "pct", new Slider("Use on Hero HP % <=", 20));
                    Menu.Add("selfmuchhp" + Name + "pct", new Slider("Use on Hero Dmg Dealt % >=", 45));
                    Menu.Add("use" + Name + "tower", new CheckBox("Include Tower Damage", false));
                    Menu.Add("mode" + Name, new ComboBox("Mode: ", 1, "Always", "Combo"));
                    Menu.AddSeparator();
                }

                if (Name == "summonerboost")
                {
                    Activator.UseAllyMenu = true;
                    Menu.AddGroupLabel(DisplayName + " Buff Types");
                    Menu.Add(Name + "cstun", new CheckBox("Stuns"));
                    Menu.Add(Name + "ccharm", new CheckBox("Charms"));
                    Menu.Add(Name + "ctaunt", new CheckBox("Taunts"));
                    Menu.Add(Name + "cflee", new CheckBox("Flee/Fear"));
                    Menu.Add(Name + "csnare", new CheckBox("Snares"));
                    Menu.Add(Name + "cexh", new CheckBox("Exhaust", false));
                    Menu.Add(Name + "csupp", new CheckBox("Supression", false));
                    Menu.Add(Name + "csilence", new CheckBox("Silences", false));
                    Menu.Add(Name + "cpolymorph", new CheckBox("Polymorphs", false));
                    Menu.Add(Name + "cblind", new CheckBox("Blinds", false));
                    Menu.Add(Name + "cslow", new CheckBox("Slows", false));
                    Menu.Add(Name + "cpoison", new CheckBox("Poisons", false));
                    Menu.AddSeparator();

                    Menu.Add("use" + Name + "number", new Slider("Min Buffs to Use", 1, 1, 5));
                    Menu.AddSeparator();
                    Menu.Add("use" + Name + "time", new Slider("Min Durration to Use", 500, 250, 2000));
                    Menu.AddLabel("^ Will not use unless the buff durration (stun, snare, etc) last at least this long (ms, 500 = 0.5 seconds)");
                    Menu.AddSeparator();
                    Menu.Add("use" + Name + "od", new CheckBox("Use for Dangerous Only", false));
                    Menu.Add("use" + Name + "delay", new Slider("Activation Delay (in ms)", 150, 0, 500));
                    Menu.Add("mode" + Name, new ComboBox("Mode: ", 0, "Always", "Combo"));
                    Menu.AddSeparator();
                }

                if (Name == "summonerdot")
                {
                    Activator.UseEnemyMenu = true;
                    Menu.Add("idmgcheck", new Slider("Combo Damage Check %", 100, 1, 200));

                    switch (Player.ChampionName)
                    {
                        case "Ahri":
                            Menu.Add("ii" + Player.ChampionName, new CheckBox(Player.ChampionName + ": Check Charm", false));
                            break;
                        case "Cassiopeia":
                            Menu.Add("ii" + Player.ChampionName, new CheckBox(Player.ChampionName + ": Check Poison", false));
                            break;
                        case "Diana":
                            Menu.Add("ii" + Player.ChampionName, new CheckBox(Player.ChampionName + ": Check Moonlight?", false));
                            break;
                    }

                    Menu.Add("itu", new CheckBox("Dont Ignite Near Turret"));
                    Menu.Add("igtu", new Slider("-> Ignore after Level", 11, 1, 18));
                    Menu.Add("mode" + Name, new ComboBox("Mode: ", 0, "Killsteal", "Combo"));
                    Menu.AddSeparator();
                }

                if (Name == "summonermana")
                {
                    Activator.UseAllyMenu = true;
                    Menu.Add("selflowmp" + Name + "pct", new Slider("Minimum Mana % <=", 40));
                    Menu.AddSeparator();
                }

                if (Name == "summonerbarrier")
                {
                    Activator.UseAllyMenu = true;
                    Menu.Add("selflowhp" + Name + "pct", new Slider("Use on Hero HP % <=", 20));
                    Menu.Add("selfmuchhp" + Name + "pct", new Slider("Use on Hero Dmg Dealt % >=", 45));
                    Menu.Add("use" + Name + "ulti", new CheckBox("Use on Dangerous (Ultimates Only)"));
                    Menu.Add("f" + Name, new CheckBox("-> Force Barrier", false));
                    Menu.Add("use" + Name + "tower", new CheckBox("Include Tower Damage"));
                    Menu.Add("mode" + Name, new ComboBox("Mode: ", 1, "Always", "Combo"));
                    Menu.AddSeparator();
                }

                if (Name == "summonerexhaust")
                {
                    Activator.UseEnemyMenu = true;
                    Menu.Add("a" + Name + "pct", new Slider("Exhaust on ally HP %", 35));
                    Menu.Add("e" + Name + "pct", new Slider("Exhaust on enemy HP %", 45));
                    Menu.Add("use" + Name + "ulti", new CheckBox("Use on Dangerous (Utimates Only)"));
                    Menu.Add("f" + Name, new CheckBox("-> Force Exhaust"));
                    Menu.Add("mode" + Name, new ComboBox("Mode: ", 0, "Always", "Combo"));
                    Menu.AddSeparator();
                }

                if (Name == "summonersmite")
                {
                    Activator.UseEnemyMenu = true;
                    Menu.Add("usesmite", new KeyBind("Use Smite", true, KeyBind.BindTypes.PressToggle, 'M'));
                    Menu.Add("smiteskill", new CheckBox("Smite + Ability"));
                    Menu.Add("smitesmall", new CheckBox("Smite Small Camps"));
                    Menu.Add("smitelarge", new CheckBox("Smite Large Camps"));
                    Menu.Add("smitesuper", new CheckBox("Smite Epic Camps"));
                    Menu.Add("smitemode", new ComboBox("Smite Enemies: ", 1, "Killsteal", "Combo", "Nope"));
                    Menu.Add("savesmite", new CheckBox("Save a Smite Charge"));
                    Menu.AddSeparator();
                }

                if (Name == "summonerteleport")
                {
                    Activator.UseAllyMenu = true;
                    Menu.Add("telesound", new CheckBox("Enable Sound", false));
                    Menu.Add("telelowhp2", new CheckBox("Ping Low Health Allies", false));
                    Menu.Add("teleulthp2", new CheckBox("Ping Dangerous Activity", false));
                    Menu.AddSeparator();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("<font color=\"#FFF280\">Exception thrown at CoreSum.CreateMenu: </font>: " + e.Message);
            }

            return this;
        }

        public bool IsReady()
        {
            return Player.GetSpellSlot(Name).IsReady() || 
                ExtraNames.Any(exname => Player.GetSpellSlot(exname).IsReady());
        }

        public string[] Excluded = { "summonerexhaust" };

        public void UseSpell(bool combo = false)
        {
            if (!combo || Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
            {
                if (Excluded.Any(ex => Name.Equals(ex)) || // ignore limit
                    Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Name == "summonerexhaust" ? 0: Duration;
                    }
                }
            }
        }

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.zmenu["usecombo"].Cast<KeyBind>().CurrentValue)
            {
                if (Excluded.Any(ex => Name.Equals(ex)) || // ignore limit
                    Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration) 
                {
                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot, target);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Name == "summonerexhaust" ? 0 : Duration;
                    }
                }
            }
        }

        public virtual void OnDraw(EventArgs args)
        {

        }

        public virtual void OnTick(EventArgs args)
        {

        }
    }
}
