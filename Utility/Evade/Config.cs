// Copyright 2014 - 2014 Esk0r
// Config.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

#endregion

 namespace EvadeSharp
{
    internal static class Config
    {
        public const bool PrintSpellData = false;
        public const bool TestOnAllies = false;
        public const int SkillShotsExtraRadius = 9;
        public const int SkillShotsExtraRange = 20;
        public const int GridSize = 10;
        public const int ExtraEvadeDistance = 15;
        public const int PathFindingDistance = 60;
        public const int PathFindingDistance2 = 35;

        public const int DiagonalEvadePointsCount = 7;
        public const int DiagonalEvadePointsStep = 20;

        public const int CrossingTimeOffset = 250;

        public const int EvadingFirstTimeOffset = 250;
        public const int EvadingSecondTimeOffset = 80;

        public const int EvadingRouteChangeTimeOffset = 250;

        public const int EvadePointChangeInterval = 300;
        public static int LastEvadePointChangeT = 0;

        public static Menu Menu, evadeSpells, skillShots, shielding, collision, drawings, misc;

        public static void CreateMenu()
        {
            Menu = MainMenu.AddMenu("Evade", "EvadeAASDASD");
            Menu.Add("Enabled", new KeyBind("Enabled", true, KeyBind.BindTypes.PressToggle, 'K'));
            Menu.Add("OnlyDangerous", new KeyBind("Dodge only dangerous", false, KeyBind.BindTypes.HoldActive, 32));

            evadeSpells = Menu.AddSubMenu("Evade spells", "evadeSpells");
            foreach (var spell in EvadeSpellDatabase.Spells)
            {
                if (evadeSpells["Enabled" + spell.Name] == null)
                {
                    evadeSpells.AddGroupLabel(spell.Name);
                    evadeSpells.Add("DangerLevel" + spell.Name, new Slider("Danger level", spell.DangerLevel, 5, 1));
                    if (spell.IsTargetted && spell.ValidTargets.Contains(SpellValidTargets.AllyWards))
                    {
                        evadeSpells.Add("WardJump" + spell.Name, new CheckBox("WardJump"));
                    }
                    evadeSpells.Add("Enabled" + spell.Name, new CheckBox("Enabled"));
                    evadeSpells.AddSeparator();
                }
            }

            skillShots = Menu.AddSubMenu("Skillshots", "Skillshots");
            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.Team != ObjectManager.Player.Team || Config.TestOnAllies)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (String.Equals(spell.ChampionName, hero.ChampionName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (skillShots["Enabled" + spell.MenuItemName] == null)
                            {
                                skillShots.AddGroupLabel(spell.MenuItemName);
                                skillShots.Add("DangerLevel" + spell.MenuItemName, new Slider("Danger level", spell.DangerValue, 5, 1));
                                skillShots.Add("IsDangerous" + spell.MenuItemName, new CheckBox("Is Dangerous", spell.IsDangerous));
                                skillShots.Add("Draw" + spell.MenuItemName, new CheckBox("Draw"));
                                skillShots.Add("Enabled" + spell.MenuItemName, new CheckBox("Enabled", !spell.DisabledByDefault));
                                skillShots.AddSeparator();
                            }
                        }
                    }
                }
            }

            shielding = Menu.AddSubMenu("Ally shielding", "Shielding");
            foreach (var ally in ObjectManager.Get<AIHeroClient>())
            {
                if (ally.IsAlly && !ally.IsMe)
                {
                    if (shielding["shield" + ally.ChampionName] == null)
                        shielding.Add("shield" + ally.ChampionName, new CheckBox("Shield " + ally.ChampionName));
                }
            }

            collision = Menu.AddSubMenu("Collision", "Collision");
            collision.Add("MinionCollision", new CheckBox("Minion collision", false));
            collision.Add("HeroCollision", new CheckBox("Hero collision", false));
            collision.Add("YasuoCollision", new CheckBox("Yasuo wall collision"));
            collision.Add("EnableCollision", new CheckBox("Enabled"));

            drawings = Menu.AddSubMenu("Drawings", "Drawings");
            drawings.AddLabel("Enabled spell color = White");//.SetValue(Color.White));
            drawings.AddLabel("Disabled spell color = Red");//.SetValue(Color.Red));
            drawings.AddLabel("Missile color = Lime Green");//.SetValue(Color.LimeGreen));
            drawings.Add("Border", new Slider("Border Width", 1, 1, 5));
            drawings.Add("EnableDrawings", new CheckBox("Enabled"));

            misc = Menu.AddSubMenu("Misc", "Misc");
            misc.Add("BlockSpells", new ComboBox("Block spells while evading", 1, "No", "Only dangerous", "Always"));
            misc.Add("DisableFow", new CheckBox("Disable fog of war dodging", false));
            misc.Add("ShowEvadeStatus", new CheckBox("Show Evade Status", false));
            if (ObjectManager.Player.CharData.BaseSkinName == "Olaf")
            {
                misc.Add("DisableEvadeForOlafR", new CheckBox("Automatic disable Evade when Olaf's ulti is active!"));
            }
        }
    }
}
