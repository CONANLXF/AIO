using System;
using DZAwarenessAIO.Utility;
using DZAwarenessAIO.Utility.Logs;
using DZAwarenessAIO.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Geometry = DZAwarenessAIO.Utility.Geometry;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

 namespace DZAwarenessAIO.Modules.Ping
{
    /// <summary>
    /// The Ping Drawing base Class
    /// </summary>
    class PingTrackerBase : ModuleBase
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
                moduleMenu = RootMenu.AddSubMenu("Ping Tracker", "dz191.dza.ping");
                {
                    moduleMenu.AddBool("dz191.dza.ping.show", "Show name near pings");
                }
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("Ping_Base", e));
            }

        }

        /// <summary>
        /// Initializes the events.
        /// </summary>
        public override void InitEvents()
        {
            try
            {
                TacticalMap.OnPing += OnPing;
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("Ping_Base", e));
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Ping" /> event.
        /// </summary>
        /// <param name="args">The <see cref="GamePingEventArgs"/> instance containing the event data.</param>
        private void OnPing(TacticalMapPingEventArgs args)
        {
            if (!args.Source.IsMe && ShouldRun() && (args.Source is AIHeroClient))
            {
                var pingType = args.PingType;
                var srcHero = args.Source as AIHeroClient;
                if (pingType == PingCategory.Normal)
                {

                    var textObject = new Render.Text(
                        srcHero.ChampionName,
                        new Vector2(
                            Drawing.WorldToScreen(args.Position.To3D()).X,
                            Drawing.WorldToScreen(args.Position.To3D()).Y + 15), 17, SharpDX.Color.White)
                    {
                        PositionUpdate = () => new Vector2(
                            Drawing.WorldToScreen(args.Position.To3D()).X,
                            Drawing.WorldToScreen(args.Position.To3D()).Y + 30),
                        Centered = true
                    };
                    textObject.Add(0);
                    LeagueSharp.Common.Utility.DelayAction.Add(1000, () =>
                    {
                        textObject.Remove();
                    });
                }
            }
        }

        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <returns></returns>
        public override ModuleTypes GetModuleType()
        {
            return ModuleTypes.General;
        }

        /// <summary>
        /// Shoulds the module run.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldRun()
        {
            return moduleMenu["dz191.dza.ping.show"].Cast<CheckBox>().CurrentValue;
        }

        /// <summary>
        /// Called On Update
        /// </summary>
        public override void OnTick() { }
    }
}
