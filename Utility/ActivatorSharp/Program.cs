#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Activator/Program.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;
using LeagueSharp.Data.DataTypes;
using LeagueSharp.Data.Enumerations;

#region  namespaces © 2015
using LeagueSharp;
using LeagueSharp.Common;
using Activators.Base;
using Activators.Data;
using Activators.Handlers;
using Activators.Items;
using Activators.Spells;
using Activators.Summoners;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
#endregion

 namespace Activators
{
    internal class Activator
    {
        internal static Menu Origin;
        internal static AIHeroClient Player;
        internal static Random Rand;

        internal static int MapId;
        internal static int LastUsedTimeStamp;
        internal static int LastUsedDuration;

        internal static SpellSlot Smite;
        internal static bool SmiteInGame;
        internal static bool TroysInGame;
        internal static bool UseEnemyMenu, UseAllyMenu;

        public static List<Base.Champion> Heroes = new List<Base.Champion>();

        public static Menu zmenu, amenu, imenu, omenu, smenu, dmenu, cmenu;

        public static void Game_OnGameLoad()
        {
            try
            {
                Player = ObjectManager.Player;
                MapId = (int)LeagueSharp.Common.Utility.Map.GetMap().Type;
                Rand = new Random();

                GetSpellsInGame();
                GetSmiteSlot();
                GetGameTroysInGame();
                GetAurasInGame();
                GetHeroesInGame();
                GetComboDamage();

                Origin = MainMenu.AddMenu("Activator", "activator");

                cmenu = Origin.AddSubMenu("Cleansers", "cmenu");
                SubMenu(cmenu, false);
                GetItemGroup("Items.Cleansers").ForEach(t => NewItem((CoreItem) NewInstance(t), cmenu));

                dmenu = Origin.AddSubMenu("Defensives", "dmenu");
                SubMenu(dmenu, false);
                GetItemGroup("Items.Defensives").ForEach(t => NewItem((CoreItem) NewInstance(t), dmenu));

                smenu = Origin.AddSubMenu("Summoners", "smenu");
                GetItemGroup("Summoners").ForEach(t => NewSumm((CoreSum) NewInstance(t), smenu));
                SubMenu(smenu, true, true);

                omenu = Origin.AddSubMenu("Offensives", "omenu");
                SubMenu(omenu, true);
                GetItemGroup("Items.Offensives").ForEach(t => NewItem((CoreItem) NewInstance(t), omenu));

                imenu = Origin.AddSubMenu("Consumables", "imenu");
                GetItemGroup("Items.Consumables").ForEach(t => NewItem((CoreItem) NewInstance(t), imenu));

                amenu = Origin.AddSubMenu("Auto Spells", "amenu");
                SubMenu(amenu, false);
                GetItemGroup("Spells.Evaders").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));
                GetItemGroup("Spells.Shields").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));
                GetItemGroup("Spells.Health").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));
                GetItemGroup("Spells.Slows").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));
                GetItemGroup("Spells.Heals").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));

                zmenu = Origin.AddSubMenu("Misc/Settings", "settings");
                if (SmiteInGame)
                {
                    zmenu.AddGroupLabel("Drawings");
                    zmenu.Add("drawsmitet", new CheckBox("Draw Smite Text"));
                    zmenu.Add("drawfill", new CheckBox("Draw Smite Fill"));
                    zmenu.Add("drawsmite", new CheckBox("Draw Smite Range"));
                }
                zmenu.Add("acdebug", new CheckBox("Debug", false));
                zmenu.Add("autolevelup", new CheckBox("Auto Level Ultimate"));
                zmenu.Add("autotrinket", new CheckBox("Auto Upgrade Trinket", false));
                zmenu.Add("healthp", new ComboBox("Ally Priority:", 1, "Low HP", "Most AD/AP", "Most HP"));
                zmenu.Add("weightdmg", new Slider("Weight Income Damage (%)", 115, 100, 150));
                zmenu.Add("usecombo", new KeyBind("Combo (active)", false, KeyBind.BindTypes.HoldActive, 32));
                zmenu.AddGroupLabel("Spell Database");
                LoadSpellMenu(zmenu);

                // drawings
                Drawings.Init();

                // handlers
                Projections.Init();
                Trinkets.Init();

                // tracks dangerous or lethal buffs/auras
                Buffs.StartOnUpdate();

                // tracks gameobjects 
                Gametroys.StartOnUpdate();

                // on bought item
                Shop.OnBuyItem += Obj_AI_Base_OnPlaceItemInSlot;

                // on level up
                Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;

                Chat.Print("<b>Activator#</b> - Loaded!");

                // init valid auto spells
                foreach (var autospell in Lists.Spells)
                    if (Player.GetSpellSlot(autospell.Name) != SpellSlot.Unknown)
                        Game.OnUpdate += autospell.OnTick;

                // init valid summoners
                foreach (var summoner in Lists.Summoners)
                    if (summoner.Slot != SpellSlot.Unknown ||
                        summoner.ExtraNames.Any(x => Player.GetSpellSlot(x) != SpellSlot.Unknown))
                        Game.OnUpdate += summoner.OnTick;

                // find items (if F5)
                foreach (var item in Lists.Items)
                {
                    if (!LeagueSharp.Common.Items.HasItem(item.Id))
                    {
                        continue;
                    }

                    if (!Lists.BoughtItems.Contains(item))
                    {
                        Game.OnUpdate += item.OnTick;
                        Lists.BoughtItems.Add(item);
                        Chat.Print("<b>Activator#</b> - <font color=\"#FFF280\">" + item.Name + "</font> active!");
                    }
                }

                // Utility.DelayAction.Add(3000, CheckEvade);
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.OnGameLoad</font>");
            }
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!zmenu["autolevelup"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var hero = sender as AIHeroClient;
            if (hero == null || !hero.IsMe || Shop.IsOpen)
            {
                return;
            }

            if (hero.ChampionName == "Jayce" || 
                hero.ChampionName == "Udyr" || 
                hero.ChampionName == "Elise")
            {
                return;
            }

            switch (Player.Level)
            {
                case 6:
                case 11:
                case 16:
                    LeagueSharp.Common.Utility.DelayAction.Add(Rand.Next(250, 500) + Math.Max(30, Game.Ping),
                        () => { Player.Spellbook.LevelSpell(SpellSlot.R); });
                    break;
            }
        }

        private static void Obj_AI_Base_OnPlaceItemInSlot(Obj_AI_Base sender, ShopActionEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            var itemid = (int) args.Id;

            foreach (var item in Lists.Items)
            {
                if (item.Id == itemid)
                {
                    if (!Lists.BoughtItems.Contains(item))
                    {
                        Game.OnUpdate += item.OnTick;
                        Lists.BoughtItems.Add(item);
                        Chat.Print("<b>Activator#</b> - <font color=\"#FFF280\">" + item.Name + "</font> active!");
                    }
                }
            }
        }

        private static void NewItem(CoreItem item, Menu parent)
        {
            try
            {
                if (item.Maps.Contains((MapType) MapId) ||  item.Maps.Contains(MapType.Common))
                {
                    Lists.Items.Add(item.CreateMenu(parent));
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.NewItem</font>");
            }
        }

        private static void NewSpell(CoreSpell spell, Menu parent)
        {
            try
            {
                if (Player.GetSpellSlot(spell.Name) != SpellSlot.Unknown)
                    Lists.Spells.Add(spell.CreateMenu(parent));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.NewSpell</font>");
            }
        }

        private static void NewSumm(CoreSum summoner, Menu parent)
        {
            try
            {
                if (summoner.Name.Contains("smite") && SmiteInGame)
                    Lists.Summoners.Add(summoner.CreateMenu(parent));

                if (!summoner.Name.Contains("smite") && Player.GetSpellSlot(summoner.Name) != SpellSlot.Unknown)
                    Lists.Summoners.Add(summoner.CreateMenu(parent));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.NewSumm</font>");
            }
        }

        private static List<Type> GetItemGroup(string nspace)
        {
            try
            {
                var allowedTypes = new[] { typeof(CoreItem), typeof(CoreSpell), typeof(CoreSum) };

                return
                    Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(
                            t =>
                                t.IsClass && t.Namespace == "Activators." + nspace && !t.Name.Contains("Core") &&
                                allowedTypes.Any(x => x.IsAssignableFrom(t)))
                        .ToList();
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.GetItemGroup</font>");
                return null;
            }
        }

        private static void GetComboDamage()
        {
            foreach (KeyValuePair<string, List<DamageSpell>> entry in Damage.Spells)
            {
                if (entry.Key == Player.ChampionName)
                    foreach (DamageSpell spell in entry.Value)
                        Somedata.DamageLib.Add(spell.Damage, spell.Slot);
            }
        }

        private static void GetHeroesInGame()
        {
            foreach (var i in ObjectManager.Get<AIHeroClient>().Where(i => i.Team == Player.Team))
                Heroes.Add(new Base.Champion(i, 0));

            foreach (var i in ObjectManager.Get<AIHeroClient>().Where(i => i.Team != Player.Team))
                Heroes.Add(new Base.Champion(i, 0));
        }

        private static void GetSmiteSlot()
        {
            if (Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite"))
            {
                SmiteInGame = true;
                Smite = SpellSlot.Summoner1;
            }

            if (Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
            {
                SmiteInGame = true;
                Smite = SpellSlot.Summoner2;
            }
        }

        private static void GetGameTroysInGame()
        {
            foreach (var i in ObjectManager.Get<AIHeroClient>().Where(h => h.Team != Player.Team))
            {
                foreach (var item in Gametroydata.Troys.Where(x => x.ChampionName == i.ChampionName))
                {
                    TroysInGame = true;
                    Gametroy.Objects.Add(new Gametroy(i.ChampionName, item.Slot, item.Name, 0, false));
                    Console.WriteLine("Activator# - SpellList: " + item.Name + " added!");
                }
            }
        }

        private static void GetSpellsInGame()
        {
            foreach (var i in ObjectManager.Get<AIHeroClient>().Where(h => h.Team != Player.Team))
            {
                foreach (var item in Somedata.Spells.Where(x => x.ChampionName == i.ChampionName.ToLower()))
                {
                    Somedata.SomeSpells.Add(item);
                    Console.WriteLine("Activator# - SpellList: " + item.SDataName + " added!");
                }
            }

            LeagueSharp.Common.Utility.DelayAction.Add(1000, LoadSpellData);
        }

        private static void LoadSpellData()
        {
            try
            {
                foreach (var adata in Somedata.SomeSpells)
                {
                    foreach (
                        var entry in
                            LeagueSharp.Data.Data.Get<SpellDatabase>()
                                .Spells.Where(
                                    x => String.Equals(x.SpellName, adata.SDataName, StringComparison.CurrentCultureIgnoreCase))
                        )
                    {
                        adata.Delay = entry.Delay;
                        adata.Speed = entry.MissileSpeed;
                        adata.Range = entry.Range;
                        adata.Width = entry.Radius;
                        adata.SpellType = entry.SpellType;
                        adata.MissileName = entry.MissileSpellName;
                        adata.ExtraMissileNames = entry.ExtraMissileNames;
                        adata.SpellTags = entry.SpellTags;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">LeagueSharp.Data</font>");
            }
        }

        private static void GetAurasInGame()
        {
            foreach (var i in ObjectManager.Get<AIHeroClient>().Where(h => h.Team != Player.Team))
            {
                foreach (var aura in Buffdata.BuffList.Where(x => x.Champion == i.ChampionName && x.Champion != null))
                {
                    Buffdata.SomeAuras.Add(aura);
                    Console.WriteLine("Activator# - AuraList: " + aura.Name + " added!");
                }
            }

            foreach (var generalaura in Buffdata.BuffList.Where(x => string.IsNullOrEmpty(x.Champion)))
            {
                Buffdata.SomeAuras.Add(generalaura);
                Console.WriteLine("Activator# - AuraList: " + generalaura.Name + " added!");
            }
        }

        public static IEnumerable<Base.Champion> Allies()
        {
            switch (zmenu["healthp"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    return Heroes.Where(h => h.Player.IsAlly)
                        .OrderBy(h => h.Player.Health / h.Player.MaxHealth * 100);
                case 1:
                    return Heroes.Where(h => h.Player.IsAlly)
                        .OrderByDescending(h => h.Player.FlatPhysicalDamageMod + h.Player.FlatMagicDamageMod);
                case 2:
                    return Heroes.Where(h => h.Player.IsAlly)
                        .OrderByDescending(h => h.Player.Health);
            }

            return null;
        }

        private static void SubMenu(Menu parent, bool enemy, bool both = false)
        {
            parent.AddGroupLabel("Config");

            parent.Add(parent.UniqueMenuId + "clear", new CheckBox("Deselect [All]", false));

            foreach (var hero in both ? HeroManager.AllHeroes : enemy ? HeroManager.Enemies : HeroManager.Allies)
            {
                var side = hero.Team == Player.Team ? "[Ally]" : "[Enemy]";
                parent.Add(parent.UniqueMenuId + "useon" + hero.NetworkId, new CheckBox("Use for " + hero.ChampionName + " " + side));

                if (both)
                {
                    //mitem.Show(hero.IsAlly && UseAllyMenu || hero.IsEnemy && UseEnemyMenu);
                }
            }

            parent[parent.UniqueMenuId + "clear"].Cast<CheckBox>().OnValueChange += (sender, args) =>
            {
                if (args.NewValue)
                {
                    foreach (var hero in both
                        ? HeroManager.AllHeroes : enemy
                        ? HeroManager.Enemies : HeroManager.Allies)
                        parent[parent.UniqueMenuId + "useon" + hero.NetworkId].Cast<CheckBox>().CurrentValue = hero.IsMe; //.SetValue(hero.IsMe);

                    LeagueSharp.Common.Utility.DelayAction.Add(100, () => parent[parent.UniqueMenuId + "clear"].Cast<CheckBox>().CurrentValue = false);
                }
            };
        }

        private static void LoadSpellMenu(Menu parent)
        {
            foreach (var unit in Heroes.Where(h => h.Player.Team != Player.Team))
            {
                parent.AddGroupLabel(unit.Player.ChampionName);
                // new menu per spell
                foreach (var entry in Somedata.Spells)
                {
                    if (entry.ChampionName == unit.Player.ChampionName.ToLower())
                    {
                        parent.AddGroupLabel(entry.SDataName);

                        // activation parameters
                        parent.Add(entry.SDataName + "predict", new CheckBox("enabled"));
                        parent.Add(entry.SDataName + "danger", new CheckBox("danger", entry.HitType.Contains(HitType.Danger)));
                        parent.Add(entry.SDataName + "crowdcontrol", new CheckBox("crowdcontrol", entry.HitType.Contains(HitType.CrowdControl)));
                        parent.Add(entry.SDataName + "ultimate", new CheckBox("danger ultimate", entry.HitType.Contains(HitType.Ultimate)));
                        parent.Add(entry.SDataName + "forceexhaust", new CheckBox("force exhaust", entry.HitType.Contains(HitType.ForceExhaust)));

                        LeagueSharp.Common.Utility.DelayAction.Add(5000, () => parent[entry.SDataName + "predict"].Cast<CheckBox>().CurrentValue = entry.SpellTags.Contains(SpellTags.Damage) || entry.SpellTags.Contains(SpellTags.CrowdControl));
                    }
                }
                parent.AddSeparator();
            }
        }

        private static object NewInstance(Type type)
        {
            try
            {
                var target = type.GetConstructor(Type.EmptyTypes);
                var dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
                var il = dynamic.GetILGenerator();

                il.DeclareLocal(target.DeclaringType);
                il.Emit(OpCodes.Newobj, target);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                var method = (Func<object>) dynamic.CreateDelegate(typeof(Func<object>));
                return method();
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.NewInstance</font>");
                return null;
            }
        }
    }
}