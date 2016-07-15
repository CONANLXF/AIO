using System.Drawing;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.Data.Enumerations;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Kalista
{
    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class Healthbars
    {
        /// <summary>
        ///     Loads the drawings.
        /// </summary>
        public static void Initialize()
        {
            Drawing.OnDraw += delegate
            {
                if (Vars.E.IsReady())
                {
                    if (!Vars.getCheckBoxItem(Vars.DrawingsMenu, "edmg"))
                    {
                        return;
                    }

                    ObjectManager.Get<Obj_AI_Base>().Where(
                        h =>
                            h.LSIsValidTarget() &&
                            Bools.IsPerfectRendTarget(h) &&
                            (h is AIHeroClient ||
                            Vars.JungleList.Contains(h.CharData.BaseSkinName))).ToList().ForEach(unit =>
                        {
                            /// <summary>
                            ///     Defines what HPBar Offsets it should display.
                            /// </summary>
                            var mobOffset = Vars.JungleHpBarOffsetList.FirstOrDefault(x => x.BaseSkinName.Equals(unit.CharData.BaseSkinName));

                            var width = (int)(Vars.JungleList.Contains(unit.CharData.BaseSkinName) ? mobOffset.Width : Vars.Width);
                            var height = (int)(Vars.JungleList.Contains(unit.CharData.BaseSkinName) ? mobOffset.Height : Vars.Height);
                            var xOffset = (int)(Vars.JungleList.Contains(unit.CharData.BaseSkinName) ? mobOffset.XOffset : Vars.XOffset);
                            var yOffset = (int)(Vars.JungleList.Contains(unit.CharData.BaseSkinName) ? mobOffset.YOffset : Vars.YOffset);

                            var barPos = unit.HPBarPosition;
                            {
                                barPos.X += xOffset;
                                barPos.Y += yOffset;
                            }

                            var drawEndXPos = barPos.X + width * (unit.HealthPercent / 100);
                            var drawStartXPos = barPos.X + (Vars.GetRealHealth(unit) >
                                (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E) +
                                (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E, DamageStage.Buff)
                                    ? width * (((Vars.GetRealHealth(unit) -
                                        ((float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E) +
                                         (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E, DamageStage.Buff))) / unit.MaxHealth * 100) / 100)
                                    : 0);

                            Drawing.DrawLine(
                                drawStartXPos,
                                barPos.Y,
                                drawEndXPos,
                                barPos.Y,
                                height,
                                Vars.GetRealHealth(unit) <
                                    (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E) +
                                    (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E, DamageStage.Buff)
                                        ? Color.Blue 
                                        : Color.Orange
                            );

                            Drawing.DrawLine(
                                drawStartXPos,
                                barPos.Y,
                                drawStartXPos,
                                barPos.Y + height + 1,
                                1,
                                Color.Lime
                            );
                        }
                    );
                }
            };
        }
    }
}
