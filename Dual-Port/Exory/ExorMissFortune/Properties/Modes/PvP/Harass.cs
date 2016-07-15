using System;
using System.Linq;
using ExorAIO.Utilities;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using Geometry = ExorAIO.Utilities.Geometry;

 namespace ExorAIO.Champions.MissFortune
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
        public static void Harass(EventArgs args)
        {
            /// <summary>
            ///     The Extended Q Mixed Harass Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.Q2Menu, "mixed")))
            {
                /// <summary>
                ///     Through enemy minions.
                /// </summary>
                foreach (var minion 
                    in from minion
                    in Targets.Minions.Where(
                        m =>
                            m.LSIsValidTarget(Vars.Q.Range) &&
                            Vars.getCheckBoxItem(Vars.Q2Menu, "mixedkill")
                                ? m.Health <
                                    (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)
                                : true)

                        let polygon = new Geometry.Sector(
                            (Vector2)minion.ServerPosition,
                            (Vector2)minion.ServerPosition.LSExtend(GameObjects.Player.ServerPosition, -(Vars.Q2.Range - Vars.Q.Range)),
                            40f * (float)Math.PI / 180f,
                            (Vars.Q2.Range - Vars.Q.Range)-50f)

                        let target = GameObjects.EnemyHeroes.FirstOrDefault(
                            t =>
                                !Invulnerable.Check(t) &&
                                t.LSIsValidTarget(Vars.Q2.Range-50f) &&
                                ((Vars.PassiveTarget.LSIsValidTarget() &&
                                    t.NetworkId == Vars.PassiveTarget.NetworkId) ||
                                    !Targets.Minions.Any(m => !polygon.IsOutside((Vector2)m.ServerPosition))) &&
                                    Vars.getCheckBoxItem(Vars.WhiteListMenu, (t.ChampionName.ToLower())))

                        where
                            target != null
                        where
                            !polygon.IsOutside((Vector2)target.ServerPosition) &&
                            !polygon.IsOutside(
                                (Vector2)Movement.GetPrediction(
                                    target,
                                    GameObjects.Player.Distance(target) / Vars.Q.Speed + Vars.Q.Delay).UnitPosition)

                    select minion)
                {
                    Vars.Q.CastOnUnit(minion);
                }
            }
        }
    }
}