using System;
using DZAwarenessAIO.Utility;
using DZAwarenessAIO.Utility.Logs;
using DZAwarenessAIO.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;

 namespace DZAwarenessAIO.Modules.WardTracker
{
    /// <summary>
    /// The Ward Tracker base class
    /// </summary>
    class WardTrackerBase : ModuleBase
    {

        public static Menu moduleMenu;

        /// <summary>
        /// Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            try
            {
                var RootMenu = Variables.Menu;
                moduleMenu = RootMenu.AddSubMenu("Wards Tracker", "dz191.dza.ward");
                {
                    moduleMenu.AddBool("dz191.dza.ward.track", "Track wards");
                    moduleMenu.AddKeybind("dz191.dza.ward.extrainfo", "Show Extra informations", new Tuple<uint, KeyBind.BindTypes>('Z', KeyBind.BindTypes.HoldActive));
                    moduleMenu.AddStringList("dz191.dza.ward.type", "Drawing Type", new []{"Circle", "Polygon"}, 1);
                    moduleMenu.AddSlider("dz191.dza.ward.sides", "Sides of Polygon (Higher = Laggier)", new Tuple<int, int, int>(4, 3, 12));
                }
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("WardTracker_Base", e));
            }
        }

        /// <summary>
        /// Initializes the events.
        /// </summary>
        public override void InitEvents()
        {
            Obj_AI_Base.OnProcessSpellCast += WardDetector.OnProcessSpellCast;
            GameObject.OnCreate += WardDetector.OnCreate;
            GameObject.OnDelete += WardDetector.OnDelete;
            Drawing.OnDraw += WardDetector.OnDraw;
        }

        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <returns></returns>
        public override ModuleTypes GetModuleType()
        {
            return ModuleTypes.OnUpdate;
        }

        /// <summary>
        /// Determines whether or not the module should run.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldRun()
        {
            return moduleMenu["dz191.dza.ward.track"].Cast<CheckBox>().CurrentValue;
        }

        /// <summary>
        /// Called OnUpdate
        /// </summary>
        public override void OnTick()
        {
            WardDetector.OnTick();
        }
    }
}
