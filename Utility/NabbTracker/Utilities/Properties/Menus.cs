using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;

using TargetSelector = PortAIO.TSManager; namespace NabbTracker
{
    using System.Drawing;
    using Color = SharpDX.Color;

    /// <summary>
    /// The menu class.
    /// </summary>
    class Menus
    {
        /// <summary>
        /// Builds the general Menu.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            /// The general Menu.
            /// </summary>
            Variables.Menu = MainMenu.AddMenu(Variables.MainMenuCodeName, Variables.MainMenuName);
            Variables.Menu.AddGroupLabel("Spell Tracker");
            Variables.Menu.Add("allies", new CheckBox("Enable Allies"));
            Variables.Menu.Add("enemies", new CheckBox("Enable Enemies"));
            Variables.Menu.AddGroupLabel("Ward Tracker");
            Variables.Menu.Add("ward", new CheckBox("Enable Ward Tracker"));
            Variables.Menu.AddGroupLabel("EXP Tracker");
            Variables.Menu.Add("me", new CheckBox("Enable Me"));
            Variables.Menu.Add("alliesEXP", new CheckBox("Enable Allies"));
            Variables.Menu.Add("enemiesEXP", new CheckBox("Enable Enemies"));
        }
    }
}
