#region

using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK;

#endregion

using TargetSelector = PortAIO.TSManager; namespace NechritoRiven.Core
{
   internal partial class Core
    {
        public static AttackableUnit QTarget;
        public static bool forceQ;
        public static bool forceW;
        public static bool forceR;
        public static bool forceR2;
        public static bool forceItem;
        public static float lastQ;
        public static float lastR;
        
        private static int Item
            =>
                Items.CanUseItem(3077) && Items.HasItem(3077)
                    ? 3077
                    : Items.CanUseItem(3074) && Items.HasItem(3074) ? 3074 : 0;

        public static void ForceW()
        {
            forceW = Spells.W.IsReady();
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => forceW = false);
        }

        public static void ForceQ(AttackableUnit target)
        {
            forceQ = true;
            QTarget = target;
        }
        public static void ForceSkill()
        {
            if (forceQ && qTarget != null && qTarget.LSIsValidTarget(Spells.E.Range + Player.BoundingRadius + 70) &&
                Spells.Q.IsReady())
                Spells.Q.Cast(qTarget.Position);
            if (forceW) Spells.W.Cast();
            if (forceR && Spells.R.Instance.Name == IsFirstR) Spells.R.Cast();
            if (forceItem && Items.CanUseItem(Item) && Items.HasItem(Item) && Item != 0) Items.UseItem(Item);
            if (forceR2 && Spells.R.Instance.Name == IsSecondR)
            {
                var target = TargetSelector.SelectedTarget;
                if (target != null) Spells.R.Cast(target.Position);
            }
        }
        public static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name.Contains("ItemTiamatCleave")) forceItem = false;
            if (args.SData.Name.Contains("RivenTriCleave")) forceQ = false;
            if (args.SData.Name.Contains("RivenMartyr")) forceW = false;
            if (args.SData.Name == IsFirstR) forceR = false;
            if (args.SData.Name == IsSecondR) forceR2 = false;
        }

        public static int WRange => Player.HasBuff("RivenFengShuiEngine")
          ? 330
          : 265;

        public static bool InWRange(Obj_AI_Base t) => t != null && t.LSIsValidTarget(WRange);

        public static bool InQRange(GameObject target)
        {
            return target != null && (Player.HasBuff("RivenFengShuiEngine")
                ? 330 >= Player.LSDistance(target.Position)
                : 265 >= Player.LSDistance(target.Position));
        }
        public static void ForceItem()
        {
            if (Items.CanUseItem(Item) && Items.HasItem(Item) && Item != 0) forceItem = true;
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => forceItem = false);
        }

        public static void ForceR()
        {
            forceR = Spells.R.IsReady() && Spells.R.Instance.Name == IsFirstR;
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => forceR = false);
        }

        public static void ForceR2()
        {
            forceR2 = Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR;
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => forceR2 = false);
        }
        public static void ForceCastQ(AttackableUnit target)
        {
            forceQ = true;
            qTarget = target;
        }

        public static void FlashW()
        {
            var target = TargetSelector.SelectedTarget;
            if (target != null && target.LSIsValidTarget() && !target.IsZombie)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(10, () => Player.Spellbook.CastSpell(Spells.Flash, target.Position));
            }
        }
    }
}
