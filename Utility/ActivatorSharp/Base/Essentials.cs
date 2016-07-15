#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Base/Essentials.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace Activators.Base
{
    public enum PrimaryRole
    {
        Unknown,
        Assassin,
        Fighter,
        Mage,
        Support,
        Marksman,
        Tank
    }

    internal class Essentials
    {

        /// <summary>
        /// Returns if the matched hero is valid and in the current game.
        /// </summary>
        /// <param name="heroname">The heroname.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool HeroInGame(string heroname)
        {
            return Activator.Heroes.Exists(x => x.Player.ChampionName == heroname && x.Player.IsEnemy);
        }

        /// <summary>
        /// Returns if the minion is an "Epic" minion (baron, dragon, etc)
        /// </summary>
        /// <param name="minion">The minion. </param>
        /// <returns></returns>
        public static bool IsEpicMinion(Obj_AI_Base minion)
        {
            var name = minion.Name;
            return minion is Obj_AI_Minion &&
                  (name.StartsWith("SRU_Baron") || name.StartsWith("SRU_Dragon") ||
                   name.StartsWith("SRU_RiftHerald") || name.StartsWith("TT_Spiderboss"));
        }

        /// <summary>
        /// Returns if the minion is a "Large" minion (Red Buff, Blue Buff, etc)
        /// </summary>
        /// <param name="minion">The minion. </param>
        /// <param name="notMini">Check if is mini. </param>
        /// <returns></returns>
        public static bool IsLargeMinion(Obj_AI_Base minion, bool notMini = true)
        {
            var name = minion.Name;
            return minion is Obj_AI_Minion && (notMini && !minion.Name.Contains("Mini")) &&
                   (name.StartsWith("SRU_Blue") || name.StartsWith("SRU_Red") || name.StartsWith("TT_NWraith1.1") ||
                    name.StartsWith("TT_NWraith4.1") || name.StartsWith("TT_NGolem2.1") || name.StartsWith("TT_NGolem5.1") ||
                    name.StartsWith("TT_NWolf3.1") || name.StartsWith("TT_NWolf6.1"));
        }

        /// <summary>
        /// Returns if the minion is a "Small" minion (Razorbeak, Krug, etc)
        /// </summary>
        /// <param name="minion">The minion. </param>
        /// <param name="notMini">Check if is mini. </param>
        /// <returns></returns>
        public static bool IsSmallMinion(Obj_AI_Base minion, bool notMini = true)
        {
            var name = minion.Name;
            return minion is Obj_AI_Minion && (notMini && !minion.Name.Contains("Mini")) &&
                  (name.StartsWith("SRU_Murkwolf") || name.StartsWith("SRU_Razorbeak") ||
                   name.StartsWith("SRU_Gromp") || name.StartsWith("SRU_Krug"));  
        }


        /// <summary>
        /// Will try to Reset income damage if target is not valid.
        /// </summary>
        /// <param name="hero">The hero to reset damage. </param>
        public static void ResetIncomeDamage(AIHeroClient hero)
        {
            foreach (var unit in Activator.Heroes.Where(x => x.Player.NetworkId == hero.NetworkId))
            {
                if (unit.IncomeDamage != 0 && unit.IncomeDamage.ToString().Contains("E")) // Check Expo
                {
                    unit.Attacker = null;
                    unit.IncomeDamage = 0;
                    unit.HitTypes.Clear();
                }

                if (unit.Player.IsZombie || unit.Immunity || !unit.Player.LSIsValidTarget(float.MaxValue, false))
                {
                    unit.Attacker = null;
                    unit.IncomeDamage = 0;
                    unit.HitTypes.Clear();
                }
            }
        }

        /// <summary>
        /// Returns the primary role of a hero.
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public static PrimaryRole GetRole(AIHeroClient hero)
        {
            var assassins = new[] // heroes who use sweepers
            {
                "Ahri", "Akali", "Annie", "Diana", "Ekko", "Elise", "Evelynn", "Fiddlesticks", "Fizz", "Gragas", "Kassadin", "Katarina",
                "Khazix", "Leblanc", "Lissandra", "MasterYi", "Nidalee", "Nocturne", "Rengar", "Shaco",
                "Syndra", "Talon", "Zed", "Kindred"
            };

            var fighters = new[] // heroes who may not upgrade trinket
            {
                "Aatrox", "Darius", "DrMundo", "Fiora", "Gangplank", "Garen", "Gnar", "Hecarim",
                "Illaoi", "Irelia", "Jax", "Jayce", "Kayle", "Kennen", "LeeSin", "Mordekaiser", "Nasus", "Olaf", "Pantheon",
                "RekSai", "Renekton", "Riven", "Rumble", "Shyvana", "Skarner", "Teemo", "Trundle", "Tryndamere", "Udyr", "Vi", "Vladimir",
                "Volibear", "Warwick", "Wukong", "XinZhao", "Yasuo", "Yorick"
            };

            var mages = new[] // mage heroes who may prefer farsight orb
            {
                "Anivia", "AurelionSol", "Azir", "Brand", "Cassiopeia", "Heimerdinger", "Karma",
                "Karthus", "Lux", "Malzahar", "Orianna", "Ryze", "Swain", "Twistedfate",
                "Veigar", "Velkoz", "Viktor", "Xerath", "Ziggs", "Taliyah"
            };

            var supports = new[]
            {
                "Alistar", "Bard", "Blitzcrank", "Braum", "Janna", "Leona", "Lulu", "Morgana", "Nami", "Nunu",
                "Sona", "Soraka", "TahmKench", "Taric", "Thresh",
                "Zilean", "Zyra"
            };

            var tanks = new[]
            {
                "Amumu", "Chogath", "Galio", "JarvanIV", "Malphite", "Maokai", "Nautilus",
                "Poppy", "Rammus", "Sejuani", "Shen", "Singed", "Sion", "Zac"
            };

            var marksmen = new[] // heroes that will 100% buy farsight orb
            {
                "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jhin", "Jinx", "Kalista",
                "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Tristana", "Twitch", "Urgot", "Varus",
                "Vayne"
            };

            if (assassins.Contains(hero.ChampionName))
            {
                return PrimaryRole.Assassin;
            }

            if (fighters.Contains(hero.ChampionName))
            {
                return PrimaryRole.Fighter;
            }

            if (mages.Contains(hero.ChampionName))
            {
                return PrimaryRole.Mage;
            }

            if (supports.Contains(hero.ChampionName))
            {
                return PrimaryRole.Support;
            }

            if (tanks.Contains(hero.ChampionName))
            {
                return PrimaryRole.Marksman;
            }

            if (marksmen.Contains(hero.ChampionName))
            {
                return PrimaryRole.Marksman;
            }

            return PrimaryRole.Unknown;
        }
    }
}
