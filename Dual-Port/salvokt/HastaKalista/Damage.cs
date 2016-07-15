using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;

using TargetSelector = PortAIO.TSManager; namespace HastaKalistaBaby
{
    class Damage
    {
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

        public static float GetEdamage(Obj_AI_Base target)
        {
            if (target.GetBuffCount("kalistaexpungemarker") > 0)
            {
                return EDamage(target);
            }
            return 0;
        }
    }
}
