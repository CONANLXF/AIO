using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.Data.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Twitch
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The KillSteal E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getCheckBoxItem(Vars.EMenu, "killsteal"))
            {
                if (GameObjects.EnemyHeroes.Any(t => !Invulnerable.Check(t) && t.LSIsValidTarget(Vars.E.Range) && Vars.GetRealHealth(t) < EDamage(t)))
                {
                    Vars.E.Cast();
                }
            }
        }

        /* Doc7 - Twitch for calculations */

        public static int[] stack = { 0, 15, 20, 25, 30, 35 };
        public static int[] _base = { 0, 20, 35, 50, 65, 80 };

        private static float EDamage(Obj_AI_Base target)
        {
            var stacks = Stack(target);
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, _base[Vars.E.Level] + stacks * (0.25f * ObjectManager.Player.FlatPhysicalDamageMod + 0.2f * ObjectManager.Player.FlatMagicDamageMod + stack[Vars.E.Level]));
        }

        private static int Stack(Obj_AI_Base obj)
        {
            var Ec = 0;
            for (var t = 1; t < 7; t++)
            {
                if (ObjectManager.Get<Obj_GeneralParticleEmitter>().Any(s => s.Position.Distance(obj.ServerPosition) <= 175 && s.Name == "twitch_poison_counter_0" + t + ".troy"))
                {
                    Ec = t;
                }
            }
            return Ec;
        }
    }
}