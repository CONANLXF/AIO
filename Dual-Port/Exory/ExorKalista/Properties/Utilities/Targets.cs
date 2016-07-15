using System.Collections.Generic;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Kalista
{
    /// <summary>
    ///     The settings class.
    /// </summary>
    internal class Targets
    {
        /// <summary>
        ///     The main hero target.
        /// </summary>
        public static AIHeroClient Target => TargetSelector.GetTarget(Vars.E.Range, DamageType.Physical);

        /// <summary>
        ///     The minions target.
        /// </summary>
        public static List<Obj_AI_Minion> Minions
            =>
                GameObjects.EnemyMinions.Where(
                    m =>
                        m.IsMinion() &&
                        m.LSIsValidTarget(Vars.E.Range)).ToList();

        /// <summary>
        ///     The jungle minion targets.
        /// </summary>
        public static List<Obj_AI_Minion> JungleMinions
            =>
                GameObjects.Jungle.Where(
                    m =>
                        m.LSIsValidTarget(Vars.E.Range) &&
                        (m.CharData.BaseSkinName.Equals("Sru_Crab") ||
                            !GameObjects.JungleSmall.Contains(m))).ToList();

        /// <summary>
        ///     The valid harassable heroes.
        /// </summary>
        public static List<AIHeroClient> Harass => GameObjects.EnemyHeroes.ToList().FindAll(t => Bools.IsPerfectRendTarget(t));
    }
}