using System.Drawing;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace NabbActivator
{
    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class Drawings
    {
        /// <summary>
        ///     Loads the drawings.
        /// </summary>
        public static void Initialize()
        {
            Drawing.OnDraw += delegate
            {
                /// <summary>
                ///     Loads the Smite drawing.
                /// </summary>
                if (Vars.Smite.IsReady() &&
                    Vars.Smite.Slot != SpellSlot.Unknown &&
                    Vars.KeysMenu["smite"].Cast<KeyBind>().CurrentValue)
                {
                    if (Vars.DrawingsMenu["range"].Cast<CheckBox>().CurrentValue)
                    {
                        Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.Smite.Range, Color.Orange, 1);
                    }
                }
            };
        }
    }
}