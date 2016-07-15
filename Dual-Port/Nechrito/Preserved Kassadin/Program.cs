using LeagueSharp.SDK;
using System;

 namespace Preserved_Kassadin
{
    class Program
    {
        public static void Load()
        {
            if (GameObjects.Player.ChampionName != "Kassadin") return;
            LoadAssembly.Load();
        }
    }
}
