using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using RandomUlt.Helpers;
using EloBuddy;

 namespace RandomUlt
{
    internal class Program
    {
        public static LastPositions positions;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public static void Main()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            positions = new LastPositions();
        }
    }
}