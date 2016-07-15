using System.Linq;
using System.Drawing;
using EloBuddy;
using LeagueSharp.SDK;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace NabbActivator
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
                if (Vars.Smite.IsReady() &&
                    Vars.Smite.Slot != SpellSlot.Unknown)
                {
                    if (!Vars.DrawingsMenu["damage"].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }

                    GameObjects.Jungle.Where(
                        m =>
                            m.LSIsValidTarget() &&
                            !GameObjects.JungleSmall.Contains(m)).ToList().ForEach(unit =>
                        {
                            /// <summary>
                            ///     Defines what HPBar Offsets it should display.
                            /// </summary>
                            var mobOffset = Vars.JungleHpBarOffsetList.FirstOrDefault(x => x.BaseSkinName.Equals(unit.CharData.BaseSkinName));
                            
                            var barPos = unit.HPBarPosition;
                            {
                                barPos.X += mobOffset.XOffset;
                                barPos.Y += mobOffset.YOffset;
                            }

                            var drawStartXPos = barPos.X;
                            var drawEndXPos =
                                barPos.X + mobOffset.Width * (Vars.GetSmiteDamage / unit.MaxHealth * 100) / 100;

                            Drawing.DrawLine(
                                drawStartXPos,
                                barPos.Y,
                                drawEndXPos,
                                barPos.Y,
                                mobOffset.Height,
                                unit.Health < Vars.GetSmiteDamage
                                    ? Color.Blue 
                                    : Color.Orange
                            );

                            Drawing.DrawLine(
                                drawEndXPos + 1,
                                barPos.Y,
                                drawEndXPos + 1,
                                barPos.Y + mobOffset.Height + 1,
                                2,
                                Color.Lime
                            );
                        }
                    );
                }
            };
        }
    }
}