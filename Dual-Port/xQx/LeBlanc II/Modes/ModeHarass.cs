using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Champion;
using Color = SharpDX.Color;
using Leblanc.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

 namespace Leblanc.Modes
{

    internal class ModeHarass
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell Q => PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => PlayerSpells.E;
        private static AIHeroClient Target => TargetSelector.GetTarget(Q.Range * 2, DamageType.Magical);

        private static bool AutoReturnW => MenuLocal["Harass.UseW.Return"].Cast<CheckBox>().CurrentValue;

        private static int ToggleActive => MenuLocal["Toggle.Active"].Cast<ComboBox>().CurrentValue;

        public static void Init()
        {

            MenuLocal = Modes.ModeConfig.MenuConfig.AddSubMenu("Harass", "Harass");
            {
                MenuLocal.Add("Harass.UseQ", new CheckBox("Q:"));
                MenuLocal.Add("Harass.UseW", new ComboBox("W:", 2, "Off", "On", "On: After Q"));
                MenuLocal.Add("Harass.UseW.Return", new CheckBox("W: Auto Return"));
                MenuLocal.Add("Harass.UseE", new CheckBox("E:", false));

                MenuLocal.AddGroupLabel("Toggle Harass");
                {
                    MenuLocal.Add("Toggle.Active", new ComboBox("Active:", 2, "Just with Laneclear Mode", "Just with Lasthit Mode", "Both"));
                    MenuLocal.Add("Toggle.UseQ", new CheckBox("Q:", false));//.SetValue(false).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
                    MenuLocal.Add("Toggle.UseW", new CheckBox("W:", false));//.SetValue(false).SetFontStyle(FontStyle.Regular, W.MenuColor()));
                    MenuLocal.Add("Toggle.UseE", new CheckBox("E:", false));//.SetValue(false).SetFontStyle(FontStyle.Regular, E.MenuColor()));
                }
            }

            Game.OnUpdate += GameOnOnUpdate;
        }
        

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                ExecuteHarass();
            }

            if (Modes.ModeConfig.MenuKeys["Key.Harass1"].Cast<KeyBind>().CurrentValue)
            {
                ExecuteToggle();
            }
        }

        private static void CastW()
        {
            if (!W.CanCast(Target))
            {
                return;
            }
            PlayerSpells.CastW(Target, true);
            return;
        }
        private static void ExecuteHarass()
        {
            if (MenuLocal["Harass.UseQ"].Cast<CheckBox>().CurrentValue && Q.CanCast(Target))
            {
                PlayerSpells.CastQ(Target);
            }

            var harassUseW = MenuLocal["Harass.UseW"].Cast<ComboBox>().CurrentValue;

            if (harassUseW != 0 && W.CanCast(Target))
            {
                switch (harassUseW)
                {
                    case 1:
                        {
                            PlayerSpells.CastW(Target);
                            break;
                        }
                    case 2:
                        {
                            if (Target.HasMarkedWithQ())
                                PlayerSpells.CastW(Target);
                            break;
                        }
                }

            }

            if (W.StillJumped() && AutoReturnW)
            {
                W.Cast();
            }

            if (MenuLocal["Harass.UseE"].Cast<CheckBox>().CurrentValue && E.CanCast(Target))
            {
                PlayerSpells.CastE(Target);
            }
        }

        private static void ExecuteToggle()
        {
            if (ToggleActive == 0 && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                return;
            }

            if (ToggleActive == 1 && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                return;
            }

            if (ToggleActive == 2 && !(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)))
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                return;
            }

            if (MenuLocal["Toggle.UseQ"].Cast<CheckBox>().CurrentValue && Q.CanCast(Target))
            {
                PlayerSpells.CastQ(Target);
            }

            if (MenuLocal["Toggle.UseW"].Cast<CheckBox>().CurrentValue && W.CanCast(Target))
            {
                PlayerSpells.CastW(Target);
            }

            if (W.StillJumped() && AutoReturnW)
            {
                W.Cast();
            }

            if (MenuLocal["Toggle.UseE"].Cast<CheckBox>().CurrentValue && E.CanCast(Target))
            {
                PlayerSpells.CastE(Target);
            }
        }
    }
}
