using System;
using Firestorm_AIO.Bases;
using LeagueSharp;
using EloBuddy;

 namespace Firestorm_AIO.Champions.Anivia
{
    public class ObjManager
    {
        public static GameObject QObject;
        public static GameObject RObject;

        public static void Load()
        {
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "cryo_FlashFrost_Player_mis.troy")
            {
                QObject = sender;
            }

            if (sender.Name.Contains("cryo_storm"))
            {
                RObject = sender;
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "cryo_FlashFrost_Player_mis.troy")
            {
                QObject = null;
            }

            if (sender.Name.Contains("cryo_storm"))
            {
                RObject = null;
            }
        }
    }
}