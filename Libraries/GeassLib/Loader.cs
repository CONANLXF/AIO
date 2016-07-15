using System;
using LeagueSharp;
using EloBuddy;

namespace GeassLib
{
    public class Loader
    {
        public static AIHeroClient Player { get; set; }
        public static void Load()
        {
            AssemblyLoadTime = DateTime.Now;
            Player = ObjectManager.Player;
        }

        public static DateTime AssemblyLoadTime;

        public static float AssemblyTime() => (float)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
    }
}