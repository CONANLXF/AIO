using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Champion;
using Color = SharpDX.Color;
using Leblanc.Common;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace Leblanc.Modes
{
    internal class ModeJump
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell W => Champion.PlayerSpells.W;
        private static LeagueSharp.Common.Spell W2 => Champion.PlayerSpells.W2;
        public static void Init(Menu ParentMenu)
        {

            MenuLocal = ParentMenu.AddSubMenu("W Auto Return Back", "MenuReturnW");
            {
                MenuLocal.Add("W.Return.Lasthist", new CheckBox("Last hit:"));
                MenuLocal.Add("W.Return.Freeze", new CheckBox("Freeze:"));
                MenuLocal.Add("W.Return.Laneclear", new CheckBox("Lane clear:"));
                MenuLocal.Add("W.Return.Harass", new CheckBox("Harass:"));
                MenuLocal.Add("W.Return.Combo", new CheckBox("Combo:", false));

                Game.OnUpdate += GameOnOnUpdate;
            }
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) &&
                MenuLocal["W.Return.Lasthist"].Cast<CheckBox>().CurrentValue)
            {
                if (W.StillJumped())
                {
                    W.Cast();
                }

                if (W2.StillJumped())
                {
                    W2.Cast();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) &&
                MenuLocal["W.Return.Freeze"].Cast<CheckBox>().CurrentValue)
            {
                if (W.StillJumped())
                {
                    W.Cast();
                }

                if (W2.StillJumped())
                {
                    W2.Cast();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) &&
                MenuLocal["W.Return.Laneclear"].Cast<CheckBox>().CurrentValue)
            {
                if (W.StillJumped())
                {
                    W.Cast();
                }

                if (W2.StillJumped())
                {
                    W2.Cast();
                }
            }


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                MenuLocal["W.Return.Harass"].Cast<CheckBox>().CurrentValue)
            {
                if (W.StillJumped())
                {
                    W.Cast();
                }

                if (W2.StillJumped())
                {
                    W2.Cast();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                MenuLocal["W.Return.Combo"].Cast<CheckBox>().CurrentValue)
            {
                if (W.StillJumped())
                {
                    Chat.Print("W 1");
                    W.Cast();
                }

                if (W2.StillJumped())
                {
                    Chat.Print("W 2");
                    W2.Cast();
                }
            }
        }
    }
}