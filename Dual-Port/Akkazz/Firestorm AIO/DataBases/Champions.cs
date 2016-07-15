using System;
using LeagueSharp;
using Firestorm_AIO.Enums;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace Firestorm_AIO.DataBases
{
    public static class Champions
    {
        private static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static Firestorm_AIO.Enums.Champion GetChampion(this AIHeroClient hero)
        {
            //Special Cases
            switch (hero.ChampionName)
            {
                case "Aurelion Sol":
                    return Firestorm_AIO.Enums.Champion.AurelionSol;
                case "Cho'Gath":
                    return Firestorm_AIO.Enums.Champion.ChoGath;
                case "Dr Mundo":
                    return Firestorm_AIO.Enums.Champion.DrMundo;
                case "Kha'Zix":
                    return Firestorm_AIO.Enums.Champion.KhaZix;
                case "Kog'Maw":
                    return Firestorm_AIO.Enums.Champion.KogMaw;
                case "Le Blanc":
                    return Firestorm_AIO.Enums.Champion.LeBlanc;
                case "Lee Sin":
                    return Firestorm_AIO.Enums.Champion.LeeSin;
                case "Master Yi":
                    return Firestorm_AIO.Enums.Champion.MasterYi;
                case "Miss Fortune":
                    return Firestorm_AIO.Enums.Champion.MissFortune;
                case "Tahm Kench":
                    return Firestorm_AIO.Enums.Champion.TahmKench;
                case "Rek'Sai":
                    return Firestorm_AIO.Enums.Champion.RekSai;
                case "Twisted Fate":
                    return Firestorm_AIO.Enums.Champion.TwistedFate;
                case "Jarvan IV":
                    return Firestorm_AIO.Enums.Champion.JarvanIV;

            }

            return hero.ChampionName.ToEnum<Firestorm_AIO.Enums.Champion>();
        }
    }
}