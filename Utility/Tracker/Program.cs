#region

using System;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;

#endregion

namespace Tracker
{
    internal class Program
    {
        public static Menu Config, cd, ward;
        public static void Game_OnGameLoad()
        {
            Config = MainMenu.AddMenu("Tracker", "Tracker");
            cd = Config.AddSubMenu("CD Tracker", "CD Tracker");
            cd.Add("TrackAllies", new CheckBox("Track Allies"));
            cd.Add("TrackEnemies", new CheckBox("Track Enemies"));

            ward = Config.AddSubMenu("Ward Tracker", "Ward Tracker");
            ward.Add("Details", new KeyBind("Show more info", false, KeyBind.BindTypes.HoldActive, 16));
            ward.AddLabel("Draw visible range of wards.");
            ward.Add("Enabled", new CheckBox("Enabled"));

            HbTracker.Game_OnGameLoad();
            WardTracker.Game_OnGameLoad();
        }
        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }
        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }
    }
}