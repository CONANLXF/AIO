namespace iKalistaReborn.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;
    using TargetSelector = PortAIO.TSManager;

    using SharpDX;

    using Collision = LeagueSharp.Common.Collision;
    using EloBuddy.SDK;
    /// <summary>
    ///     The Helper class
    /// </summary>
    internal static class Helper
    {
        
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the list of minions currently between the source and target
        /// </summary>
        /// <param name="source">
        ///     The Source
        /// </param>
        /// <param name="targetPosition">
        ///     The Target Position
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        public static List<Obj_AI_Base> GetCollisionMinions(AIHeroClient source, Vector3 targetPosition)
        {
            var input = new PredictionInput
            {
                Unit = source,
                Radius = SpellManager.Spell[SpellSlot.Q].Width,
                Delay = SpellManager.Spell[SpellSlot.Q].Delay,
                Speed = SpellManager.Spell[SpellSlot.Q].Speed,
                CollisionObjects = new[] { CollisionableObjects.Minions }
            };

            return
                Collision.GetCollision(new List<Vector3> { targetPosition }, input)
                    .OrderBy(x => x.LSDistance(source))
                    .ToList();
        }

        /// <summary>
        ///     Gets the targets current health including shield damage
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetHealthWithShield(this Obj_AI_Base target)
        {
            var debuffer = 0f;

            /// <summary>
            ///     Gets the predicted reduction from Blitzcrank Shield.
            /// </summary>
            if (target is AIHeroClient)
            {
                if ((target as AIHeroClient).ChampionName.Equals("Blitzcrank") &&
                    !(target as AIHeroClient).HasBuff("BlitzcrankManaBarrierCD"))
                {
                    debuffer += target.Mana / 2;
                }
            }
            return target.Health + target.HPRegenRate + debuffer;
        }
        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
            =>
                target.Buffs.Find(
                    b => b.Caster.IsMe && b.IsValid && b.DisplayName.ToLowerInvariant() == "kalistaexpungemarker");

        /// <summary>
        ///     Gets the current <see cref="BuffInstance" /> Count of Expunge
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int GetRendBuffCount(this Obj_AI_Base target)
            => target.Buffs.Count(x => x.Name == "kalistaexpungemarker");

        public static float GetRendDamage(Obj_AI_Base target) => SpellManager.Spell[SpellSlot.E].GetDamage(target);

        /// <summary>
        ///     Checks if a target has the Expunge <see cref="BuffInstance" />
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasRendBuff(this Obj_AI_Base target) => target?.GetRendBuff() != null;

        /// <summary>
        ///     Checks if the given target has an invulnerable buff
        /// </summary>
        /// <param name="target1">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasUndyingBuff(this Obj_AI_Base target1)
        {
            var target = target1 as AIHeroClient;

            if (target == null) return false;

            // Tryndamere R
            if (target.ChampionName == "Tryndamere"
                && target.Buffs.Any(
                    b => b.Caster.NetworkId == target.NetworkId && b.IsValid && b.DisplayName == "Undying Rage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return true;
            }

            // TODO poppy
            return false;
        }

        /// <summary>
        ///     TODO The is mob killable.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsMobKillable(this Obj_AI_Base target) => IsRendKillable(target as Obj_AI_Minion);

        private static float EDamage(Obj_AI_Base target)
        {
            if ((target.IsMinion || target.IsMonster) && !(target is AIHeroClient))
            {
                int stacksMin = GetMinionStacks(target);

                var EDamageMinion = new float[] { 20, 30, 40, 50, 60 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] + (0.6 * ObjectManager.Player.FlatPhysicalDamageMod);

                if (stacksMin > 1)
                {
                    EDamageMinion += ((new float[] { 10, 14, 19, 25, 32 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] + (new float[] { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] * ObjectManager.Player.FlatPhysicalDamageMod)) * (stacksMin - 1));
                }

                return (float)ObjectManager.Player.CalcDamage(target, DamageType.Physical, EDamageMinion) * 0.9f;
            }
            if (target is AIHeroClient)
            {
                if (GetStacks(target) == 0) return 0;

                int stacksChamps = GetStacks(target);

                var EDamageChamp = new[] { 20, 30, 40, 50, 60 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] + (0.6 * ObjectManager.Player.FlatPhysicalDamageMod);

                if (stacksChamps > 1)
                {
                    EDamageChamp += ((new[] { 10, 14, 19, 25, 32 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] + (new[] { 0.2, 0.225, 0.25, 0.275, 0.3 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] * ObjectManager.Player.FlatPhysicalDamageMod)) * (stacksChamps - 1));
                }

                return (float)ObjectManager.Player.CalcDamage(target, DamageType.Physical, EDamageChamp);
            }
            return 0;
        }

        private static int GetMinionStacks(Obj_AI_Base minion)
        {
            int stacks = 0;
            foreach (var rendbuff in minion.Buffs.Where(x => x.Name.ToLower().Contains("kalistaexpungemarker")))
            {
                stacks = rendbuff.Count;
            }

            if (stacks == 0 || !minion.HasBuff("kalistaexpungemarker")) return 0;
            return stacks;
        }

        private static int GetStacks(Obj_AI_Base target)
        {
            int stacks = 0;

            if (target.HasBuff("kalistaexpungemarker"))
            {
                foreach (var rendbuff in target.Buffs.Where(x => x.Name.ToLower().Contains("kalistaexpungemarker")))
                {
                    stacks = rendbuff.Count;
                }
            }
            else
            {
                return 0;
            }
            return stacks;
        }

        /// <summary>
        ///     Checks if the given target is killable
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsRendKillable(this Obj_AI_Base target)
        {
            if (target.IsInvulnerable || !target.HasBuff("kalistaexpungemarker"))
            {
                return false;
            }

            return EDamage(target) >= GetHealthWithShield(target);
        }

        #endregion
    }
}