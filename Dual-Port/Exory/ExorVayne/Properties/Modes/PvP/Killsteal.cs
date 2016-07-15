using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Vayne
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
            ///     The Q KillSteal Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !GameObjects.Player.Spellbook.IsAutoAttacking &&
                Vars.getCheckBoxItem(Vars.QMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        t.LSIsValidTarget(Vars.Q.Range) &&
                        !t.LSIsValidTarget(Vars.AARange) &&
                        t.CountEnemyHeroesInRange(700f) <= 2 &&
                        Vars.GetRealHealth(t) <
                            GameObjects.Player.GetAutoAttackDamage(t) +
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.Q) +
                            (t.GetBuffCount("vaynesilvereddebuff") == 2
                                ? (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.W)
                                : 0)))
                {
                    Vars.Q.Cast(target.ServerPosition);
                    Orbwalker.ForcedTarget =(target);
                }
            }

            /// <summary>
            ///     The E KillSteal Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                !GameObjects.Player.Spellbook.IsAutoAttacking &&
                Vars.getCheckBoxItem(Vars.EMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        t.LSIsValidTarget(Vars.E.Range) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.E) +
                            (t.GetBuffCount("vaynesilvereddebuff") == 2
                                ? (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.W)
                                : 0)))
                {
                    Vars.E.CastOnUnit(target);
                }
            }
        }
    }
}