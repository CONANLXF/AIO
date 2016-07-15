using System;
using EloBuddy;
using LeagueSharp.Common;

 namespace KarthusSharp
{
    class Program
    {
        public static Helper Helper;

        public static void Game_OnGameLoad()
        {
            Helper = new Helper();
            new Karthus();
        }
    }
}
