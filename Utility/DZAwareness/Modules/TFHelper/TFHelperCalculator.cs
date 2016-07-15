using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

 namespace DZAwarenessAIO.Modules.TFHelper
{
    class TFHelperCalculator
    {
        /// <summary>
        /// Gets the ally team strength.
        /// </summary>
        /// <returns></returns>
        public static float GetAllyStrength()
        {
            if (ObjectManager.Player.IsDead || (TFHelperVariables.AlliesClose.Any() && !TFHelperVariables.EnemiesClose.ToList().Any()))
            {
                return 0;
            }

            return 1 - GetEnemyStrength();
        }

        /// <summary>
        /// Gets the enemy team strength.
        /// </summary>
        /// <returns></returns>
        public static float GetEnemyStrength()
        {
            var allyDamage = TFHelperVariables.AlliesClose.ToList().Sum(s => GetHeroAvgDamage(s, TFHelperVariables.EnemiesClose.ToList()));
            var enemyDamage = TFHelperVariables.EnemiesClose.ToList().Sum(s => GetHeroAvgDamage(s, TFHelperVariables.AlliesClose.ToList()));

            if (enemyDamage <= 0 && allyDamage >= 0)
            {
                return 0;
            }

            if (ObjectManager.Player.IsDead || (TFHelperVariables.AlliesClose.Any() && !TFHelperVariables.EnemiesClose.ToList().Any()))
            {
                return 0;
            }

            return (enemyDamage / allyDamage) <= 1 ? (enemyDamage / allyDamage) : (allyDamage / enemyDamage);
        }

        /// <summary>
        /// Gets the text to be shown in the hud.
        /// </summary>
        /// <returns></returns>
        public static string GetText()
        {
            if ((TFHelperVariables.AlliesClose.Any() && !TFHelperVariables.EnemiesClose.ToList().Any()))
            {
                if (ObjectManager.Player.IsDead)
                {
                    return "You kinda suck!";
                }

                return "No enemy around";
            }

            return !ObjectManager.Player.IsDead ?
                $"{TFHelperVariables.AlliesClose.Count()}v{TFHelperVariables.EnemiesClose.Count()}: {(GetAllyStrength() > GetEnemyStrength() ? "Ally" : "Enemy")} will win"
                : string.Format("You kinda suck!");
        }

        /// <summary>
        /// Gets the hero average damage to the opposing team.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="Enemies">The enemies.</param>
        /// <returns></returns>
        public static float GetHeroAvgDamage(AIHeroClient player, List<AIHeroClient> Enemies)
        {
            var totalEnemies = Enemies.Count();
            if (totalEnemies == 0)
            {
                return -1;
            }
            var AADamage = Enemies.Aggregate(0, (current, s) => (int) (current + player.LSGetAutoAttackDamage(s) * 2));
            var QDamage = Enemies.Aggregate(0, (current, s) => (int)(current + (player.GetSpell(SpellSlot.Q).IsReady() ? player.LSGetSpellDamage(s, SpellSlot.Q) : 0f)));
            var WDamage = Enemies.Aggregate(0, (current, s) => (int)(current + (player.GetSpell(SpellSlot.W).IsReady() ? player.LSGetSpellDamage(s, SpellSlot.W) : 0f)));
            var EDamage = Enemies.Aggregate(0, (current, s) => (int)(current + (player.GetSpell(SpellSlot.E).IsReady() ? player.LSGetSpellDamage(s, SpellSlot.E) : 0f)));
            var RDamage = Enemies.Aggregate(0, (current, s) => (int)(current + (player.GetSpell(SpellSlot.R).IsReady() ? player.LSGetSpellDamage(s, SpellSlot.R) : 0f)));
            
            var itemsDamage = 0f;

            foreach (var item in player.InventoryItems)
            {
                foreach (var hero in Enemies)
                {
                    var itemID = item.Id;
                    switch (itemID)
                    {
                        case ItemId.Bilgewater_Cutlass:
                            itemsDamage +=
                                (float) player.GetItemDamage(hero, Damage.DamageItems.Bilgewater);
                            break;
                        case ItemId.Blade_of_the_Ruined_King:
                            itemsDamage += (float) player.GetItemDamage(hero, Damage.DamageItems.Botrk);
                            break;
                        case ItemId.Hextech_Gunblade:
                            itemsDamage += (float) player.GetItemDamage(hero, Damage.DamageItems.Hexgun);
                            break;
                        case ItemId.Frost_Queens_Claim:
                            itemsDamage +=
                                (float) player.GetItemDamage(hero, Damage.DamageItems.FrostQueenClaim);
                            break;
                        case ItemId.Tiamat_Melee_Only:
                            itemsDamage += player.IsMelee
                                ? (float) player.GetItemDamage(hero, Damage.DamageItems.Tiamat)
                                : 0f;
                            break;
                        case ItemId.Ravenous_Hydra_Melee_Only:
                            itemsDamage += player.IsMelee
                                ? (float) player.GetItemDamage(hero, Damage.DamageItems.Hydra)
                                : 0f;
                            break;
                        case ItemId.Liandrys_Torment:
                            itemsDamage +=
                                (float) player.GetItemDamage(hero, Damage.DamageItems.LiandrysTorment);
                            break;
                    }
                }
                
            }

            var totalDamage = AADamage + QDamage + WDamage + EDamage + RDamage + itemsDamage;

            return (float) totalDamage / totalEnemies;
        }
    }
}
