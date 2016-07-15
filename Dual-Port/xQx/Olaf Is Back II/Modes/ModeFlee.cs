using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace OlafxQx.Modes
{
    internal static class ModeFlee
    {
        public static Menu MenuLocal { get; private set; }

        public static void Init(Menu ParentMenu)
        {
            MenuLocal = ParentMenu.AddSubMenu("Flee", "Flee");
            {
                MenuLocal.Add("Flee.UseQ", new ComboBox("Q:", 1, "Off", "On"));
                MenuLocal.Add("Flee.Youmuu", new ComboBox("Item Youmuu:", 1, "Off", "On"));
                MenuLocal.Add("Flee.DrawMouse", new ComboBox("Draw Mouse Position:", 1, "Off", "On"));
            }

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += delegate(EventArgs args)
            {
                if (!PortAIO.OrbwalkerManager.isFleeActive)
                {
                    return;
                }

                if (MenuLocal["Flee.DrawMouse"].Cast<ComboBox>().CurrentValue == 1)
                {
                    Render.Circle.DrawCircle(Game.CursorPos, 150f, System.Drawing.Color.Red);
                }
            };
        }
        
        private static void OnUpdate(EventArgs args)
        {
            if (!PortAIO.OrbwalkerManager.isFleeActive)
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            PortAIO.OrbwalkerManager.SetAttack(!(PortAIO.OrbwalkerManager.isFleeActive));
            

            var t = TargetSelector.GetTarget(Champion.PlayerSpells.Q.Range, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                if (MenuLocal["Flee.UseQ"].Cast<ComboBox>().CurrentValue == 1 && Champion.PlayerSpells.Q.IsReady())
                {
                    Champion.PlayerSpells.CastQ(t, Champion.PlayerSpells.Q.Range);
                }

                if (MenuLocal["Flee.Youmuu"].Cast<ComboBox>().CurrentValue == 1 && Common.CommonItems.Youmuu.IsReady())
                {
                    Common.CommonItems.Youmuu.Cast();
                }
            }
        }
    }
}
