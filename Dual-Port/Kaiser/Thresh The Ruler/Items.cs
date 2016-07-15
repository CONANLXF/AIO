using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using LCItems = LeagueSharp.Common.Items;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

 namespace ThreshTherulerofthesoul
{
    class Items
    {
        public struct Item
        {
            public string Name;
            public int Id;
            public float Range;
        }

        static List<Item> ItemList = new List<Item>();
        private static Menu config = Program.config;
        public static Menu ItemMenu, BuffTypemenu, Allymenu;
        static Items()
        {
            #region Locket of the Iron Solari

            ItemList.Add(new Item
                {
                    Name = "Locket of the Iron Solari",
                    Id = 3190,
                    Range = 600f
                });

            #endregion

            #region Randuin's Omen

            ItemList.Add(new Item
                {
                    Name = "Randuin's Omen",
                    Id = 3143,
                    Range = 500f
                });

            #endregion

            #region Face of the Mountain

            ItemList.Add(new Item
            {
                Name = "Face of the Mountain",
                Id = 3401,
                Range = 750f
            });

            #endregion

            #region Mikael's Crucible

            ItemList.Add(new Item
            {
                Name = "Mikael's Crucible",
                Id = 3222,
                Range = 600f
            });

            #endregion
            //Console.WriteLine("Load");
        }

        public static void LoadItems()
        {
            #region ItemsMenu

            ItemMenu = config.AddSubMenu("Items", "Items");
            {
                ItemMenu.Add("UseItems", new KeyBind("Only Use Combo Key Press", false, KeyBind.BindTypes.HoldActive, 32));
                ItemMenu.Add("Use" + "Locket of the Iron Solari", new CheckBox("Use Locket"));
                ItemMenu.AddSeparator();
                ItemMenu.Add("Use" + "Randuin's Omen", new CheckBox("Use Randuin"));
                ItemMenu.Add("Randuin", new Slider("Use X Enemies In Range for randuin", 2, 1, 5));
                ItemMenu.Add("Use" + "Face of the Mountain", new CheckBox("Use FOM"));
                ItemMenu.Add("Use" + "Mikael's Crucible", new CheckBox("Use Mikaels"));

  
                BuffTypemenu = config.AddSubMenu("Buff Type", "Buff Type");
                {
                    BuffTypemenu.Add("blind", new CheckBox("Blind"));
                    BuffTypemenu.Add("charm", new CheckBox("Charm"));
                    BuffTypemenu.Add("fear", new CheckBox("Fear"));
                    BuffTypemenu.Add("flee", new CheckBox("Flee"));
                    BuffTypemenu.Add("snare", new CheckBox("Snare"));
                    BuffTypemenu.Add("taunt", new CheckBox("Taunt"));
                    BuffTypemenu.Add("suppression", new CheckBox("Suppression"));
                    BuffTypemenu.Add("stun", new CheckBox("Stun"));
                    BuffTypemenu.Add("polymorph", new CheckBox("Polymorph"));
                    BuffTypemenu.Add("silence", new CheckBox("Silence"));

                    }

               Allymenu = config.AddSubMenu("Use For Him", "Use For Him");
               {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
                  {
                    Allymenu.Add(hero.ChampionName, new CheckBox(hero.ChampionName));
                  }

                   }
                }

                #endregion
           

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Program.getKeyBindItem(ItemMenu, "UseItems"))
                return;

            foreach (var item in ItemList.
                Where(x => 
                    LCItems.HasItem(x.Id) &&
                    LCItems.CanUseItem(x.Id)))
            {
                if (Program.getCheckBoxItem(ItemMenu, "Use" + item.Name))
                {
                    UseItem(item.Id, item.Range);
                }
            }
        }

        private static void UseItem(int id, float range)
        {
            if (id == 3190)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>()
                    .Where(x => 
                        x.IsAlly &&
                        ObjectManager.Player.LSDistance(x.Position) <= range &&
                        !x.IsDead))
                {
                    if (hero.HpPercents() < 20)
                    {
                        LCItems.UseItem(id, hero);
                    }
                }
            }

            if (id == 3143)
            {
                var ReqValue = Program.getSliderItem(ItemMenu, "Randuin");
                
                if (HeroManager.Enemies.Where(x => x.LSIsValidTarget(range)).Count() >= ReqValue)
                {
                    LCItems.UseItem(3143);
                }
            }

            if (id == 3401)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>()
                    .Where(x => 
                        x.IsAlly &&
                        ObjectManager.Player.LSDistance(x.Position) <= range &&
                        !x.IsDead))
                {
                    if (hero.HpPercents() < 20)
                    {
                        LCItems.UseItem(id, hero);
                    }
                }
            }

            if (id == 3222)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>()
                    .Where(x => 
                        x.IsAlly &&
                        ObjectManager.Player.LSDistance(x.Position) <= range &&
                        !x.IsDead))
                {
                    if (Program.getCheckBoxItem(Allymenu, hero.ChampionName))
                    {
                        if (Program.getCheckBoxItem(BuffTypemenu, "blind") && hero.HasBuffOfType(BuffType.Blind))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (Program.getCheckBoxItem(BuffTypemenu, "charm") && hero.HasBuffOfType(BuffType.Charm))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (Program.getCheckBoxItem(BuffTypemenu, "fear") && hero.HasBuffOfType(BuffType.Fear))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (Program.getCheckBoxItem(BuffTypemenu, "flee") && hero.HasBuffOfType(BuffType.Flee))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (Program.getCheckBoxItem(BuffTypemenu, "snare") && hero.HasBuffOfType(BuffType.Snare))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (Program.getCheckBoxItem(BuffTypemenu, "taunt") && hero.HasBuffOfType(BuffType.Taunt))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (Program.getCheckBoxItem(BuffTypemenu, "suppression") && hero.HasBuffOfType(BuffType.Suppression))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (Program.getCheckBoxItem(BuffTypemenu, "stun") && hero.HasBuffOfType(BuffType.Stun))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (Program.getCheckBoxItem(BuffTypemenu, "polymorph") && hero.HasBuffOfType(BuffType.Polymorph))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (Program.getCheckBoxItem(BuffTypemenu, "silence") && hero.HasBuffOfType(BuffType.Silence))
                        {
                            LCItems.UseItem(id, hero);
                        }
                    }
                }
            }
        }
    }
}
