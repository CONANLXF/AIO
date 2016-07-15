using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firestorm_AIO.Champions.Illaoi;
using Firestorm_AIO.Champions.Rumble;
using Firestorm_AIO.Champions.Yasuo;
using LeagueSharp;
using EloBuddy;
using Firestorm_AIO.Champions.Anivia;

using TargetSelector = PortAIO.TSManager; namespace Firestorm_AIO
{
    internal class Loader
    {
        public static void Load()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Yasuo":
                    new Yasuo().Load();
                    break;
                case "Rumble":
                    new Rumble().Load();
                    break;
                case "Illaoi":
                    new Illaoi().Load();
                    break;
                case "Anivia":
                    new Anivia().Load();
                    break;
                //TODO
                case "Gnar":
                    
                    break;
                case "Bard":

                    break;
                case "Ahri":

                    break;
                case "Ezreal":

                    break;
                default:
                    Chat.Print("Champion not supported in FireStorm AIO");
                    break;
            }
        }
    }
}
