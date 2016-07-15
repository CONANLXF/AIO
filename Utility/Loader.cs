using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;


namespace PortAIO.Utility
{
    class Loader
    {
        public static bool antiStealth { get { return Miscc["antiStealth"].Cast<CheckBox>().CurrentValue; } }
        public static bool limitedShat { get { return Miscc["limitedShat"].Cast<CheckBox>().CurrentValue; } }
        public static bool autoLevel { get { return Miscc["autoLevel"].Cast<CheckBox>().CurrentValue; } }
        public static bool chatLogger { get { return Miscc["chatLogger"].Cast<CheckBox>().CurrentValue; } }
        public static bool autoFF { get { return Miscc["autoFF"].Cast<CheckBox>().CurrentValue; } }
        public static bool urfSpell { get { return Miscc["urfSpell"].Cast<CheckBox>().CurrentValue; } }
        public static bool emoteSpammer { get { return Miscc["emoteSpammer"].Cast<CheckBox>().CurrentValue; } }
        public static bool pastingSharp { get { return Miscc["pastingSharp"].Cast<CheckBox>().CurrentValue; } }
        public static bool useActivator { get { return Miscc["activator"].Cast<CheckBox>().CurrentValue; } }
        public static bool cheat { get { return Miscc["cheat"].Cast<CheckBox>().CurrentValue; } }
        public static bool useTracker { get { return Miscc["tracker"].Cast<CheckBox>().CurrentValue; } }
        public static bool useRecall { get { return Miscc["recall"].Cast<CheckBox>().CurrentValue; } }
        public static bool useSkin { get { return Miscc["skin"].Cast<CheckBox>().CurrentValue; } }
        public static bool champOnly { get { return Miscc["champ"].Cast<CheckBox>().CurrentValue; } }
        public static bool utilOnly { get { return Miscc["util"].Cast<CheckBox>().CurrentValue; } }
        public static bool evade { get { return Miscc["evade"].Cast<CheckBox>().CurrentValue; } }
        public static bool godTracker { get { return Miscc["godTracker"].Cast<CheckBox>().CurrentValue; } }
        public static bool ping { get { return Miscc["ping"].Cast<CheckBox>().CurrentValue; } }
        public static bool human { get { return Miscc["human"].Cast<CheckBox>().CurrentValue; } }
        public static int soraka { get { return Miscc["soraka"].Cast<ComboBox>().CurrentValue; } }
        public static int poppy { get { return Miscc["poppy"].Cast<ComboBox>().CurrentValue; } }
        public static int kogmaw { get { return Miscc["kogmaw"].Cast<ComboBox>().CurrentValue; } }
        public static int lux { get { return Miscc["lux"].Cast<ComboBox>().CurrentValue; } }
        public static int leesin { get { return Miscc["leesin"].Cast<ComboBox>().CurrentValue; } }
        public static int leblanc { get { return Miscc["leblanc"].Cast<ComboBox>().CurrentValue; } }
        public static bool bubba { get { return Miscc["bubba"].Cast<CheckBox>().CurrentValue; } }
        public static int kalista { get { return Miscc["kalista"].Cast<ComboBox>().CurrentValue; } }
        public static bool gank { get { return Miscc["gank"].Cast<CheckBox>().CurrentValue; } }
        public static int diana { get { return Miscc["diana"].Cast<ComboBox>().CurrentValue; } }
        public static int ryze { get { return Miscc["ryze"].Cast<ComboBox>().CurrentValue; } }
        public static int draven { get { return Miscc["draven"].Cast<ComboBox>().CurrentValue; } }
        public static int cait { get { return Miscc["cait"].Cast<ComboBox>().CurrentValue; } }
        public static bool intro { get { return Miscc["intro"].Cast<CheckBox>().CurrentValue; } }
        public static int twitch { get { return Miscc["twitch"].Cast<ComboBox>().CurrentValue; } }
        public static int nidalee { get { return Miscc["nidalee"].Cast<ComboBox>().CurrentValue; } }
        public static int morgana { get { return Miscc["morgana"].Cast<ComboBox>().CurrentValue; } }
        public static int twistedfate { get { return Miscc["twistedfate"].Cast<ComboBox>().CurrentValue; } }
        public static int sona { get { return Miscc["sona"].Cast<ComboBox>().CurrentValue; } }
        public static int shaco { get { return Miscc["shaco"].Cast<ComboBox>().CurrentValue; } }
        public static int sion { get { return Miscc["sion"].Cast<ComboBox>().CurrentValue; } }
        public static int trundle { get { return Miscc["trundle"].Cast<ComboBox>().CurrentValue; } }
        public static int lucian { get { return Miscc["lucian"].Cast<ComboBox>().CurrentValue; } }
        public static int ashe { get { return Miscc["ashe"].Cast<ComboBox>().CurrentValue; } }
        public static int vayne { get { return Miscc["vayne"].Cast<ComboBox>().CurrentValue; } }
        public static int quinn { get { return Miscc["quinn"].Cast<ComboBox>().CurrentValue; } }
        public static int jayce { get { return Miscc["jayce"].Cast<ComboBox>().CurrentValue; } }
        public static int yasuo { get { return Miscc["yasuo"].Cast<ComboBox>().CurrentValue; } }
        public static int katarina { get { return Miscc["katarina"].Cast<ComboBox>().CurrentValue; } }
        public static int xerath { get { return Miscc["xerath"].Cast<ComboBox>().CurrentValue; } }
        public static int gragas { get { return Miscc["gragas"].Cast<ComboBox>().CurrentValue; } }
        public static int gangplank { get { return Miscc["gangplank"].Cast<ComboBox>().CurrentValue; } }
        public static int ezreal { get { return Miscc["ezreal"].Cast<ComboBox>().CurrentValue; } }
        public static int brand { get { return Miscc["brand"].Cast<ComboBox>().CurrentValue; } }
        public static int blitzcrank { get { return Miscc["blitzcrank"].Cast<ComboBox>().CurrentValue; } }
        public static int corki { get { return Miscc["corki"].Cast<ComboBox>().CurrentValue; } }
        public static int darius { get { return Miscc["darius"].Cast<ComboBox>().CurrentValue; } }
        public static int evelynn { get { return Miscc["evelynn"].Cast<ComboBox>().CurrentValue; } }
        public static int jhin { get { return Miscc["jhin"].Cast<ComboBox>().CurrentValue; } }
        public static int jax { get { return Miscc["jax"].Cast<ComboBox>().CurrentValue; } }
        public static int kindred { get { return Miscc["kindred"].Cast<ComboBox>().CurrentValue; } }
        public static int kayle { get { return Miscc["kayle"].Cast<ComboBox>().CurrentValue; } }
        public static int ekko { get { return Miscc["ekko"].Cast<ComboBox>().CurrentValue; } }
        public static int rumble { get { return Miscc["rumble"].Cast<ComboBox>().CurrentValue; } }
        public static int riven { get { return Miscc["riven"].Cast<ComboBox>().CurrentValue; } }
        public static int graves { get { return Miscc["Graves"].Cast<ComboBox>().CurrentValue; } }
        public static int ahri { get { return Miscc["ahri"].Cast<ComboBox>().CurrentValue; } }
        public static bool banwards { get { return Miscc["banwards"].Cast<CheckBox>().CurrentValue; } }
        public static bool antialistar { get { return Miscc["antialistar"].Cast<CheckBox>().CurrentValue; } }
        public static bool traptrack { get { return Miscc["traptrack"].Cast<CheckBox>().CurrentValue; } }
        public static int elise { get { return Miscc["elise"].Cast<ComboBox>().CurrentValue; } }
        public static int rengar { get { return Miscc["rengar"].Cast<ComboBox>().CurrentValue; } }
        public static int zed { get { return Miscc["zed"].Cast<ComboBox>().CurrentValue; } }
        public static int reksai { get { return Miscc["reksai"].Cast<ComboBox>().CurrentValue; } }
        public static int volibear { get { return Miscc["volibear"].Cast<ComboBox>().CurrentValue; } }
        public static int anivia { get { return Miscc["anivia"].Cast<ComboBox>().CurrentValue; } }
        public static int taliyah { get { return Miscc["taliyah"].Cast<ComboBox>().CurrentValue; } }
        public static int janna { get { return Miscc["janna"].Cast<ComboBox>().CurrentValue; } }
        public static int irelia { get { return Miscc["irelia"].Cast<ComboBox>().CurrentValue; } }
        public static int sivir { get { return Miscc["sivir"].Cast<ComboBox>().CurrentValue; } }
        public static int jarvan { get { return Miscc["jarvan"].Cast<ComboBox>().CurrentValue; } }
        public static int braum { get { return Miscc["braum"].Cast<ComboBox>().CurrentValue; } }
        public static int karma { get { return Miscc["karma"].Cast<ComboBox>().CurrentValue; } }
        public static int teemo { get { return Miscc["teemo"].Cast<ComboBox>().CurrentValue; } }
        public static int cassiopeia { get { return Miscc["cassiopeia"].Cast<ComboBox>().CurrentValue; } }
        public static int bard { get { return Miscc["bard"].Cast<ComboBox>().CurrentValue; } }
        public static int evadeCB { get { return Miscc["evadeCB"].Cast<ComboBox>().CurrentValue; } }
        public static int activatorCB { get { return Miscc["activatorCB"].Cast<ComboBox>().CurrentValue; } }
        public static int olaf { get { return Miscc["olaf"].Cast<ComboBox>().CurrentValue; } }
        public static int gnar { get { return Miscc["gnar"].Cast<ComboBox>().CurrentValue; } }
        public static int renekton { get { return Miscc["renekton"].Cast<ComboBox>().CurrentValue; } }
        public static int jinx { get { return Miscc["jinx"].Cast<ComboBox>().CurrentValue; } }
        public static int syndra { get { return Miscc["syndra"].Cast<ComboBox>().CurrentValue; } }
        public static int aatrox { get { return Miscc["aatrox"].Cast<ComboBox>().CurrentValue; } }
        public static int missfortune { get { return Miscc["missfortune"].Cast<ComboBox>().CurrentValue; } }
        public static bool reform { get { return Miscc["reform"].Cast<CheckBox>().CurrentValue; } }
        public static bool dzaware { get { return Miscc["dzaware"].Cast<CheckBox>().CurrentValue; } }
        public static int trackerCB { get { return Miscc["trackerCB"].Cast<ComboBox>().CurrentValue; } }
        public static int mundo { get { return Miscc["mundo"].Cast<ComboBox>().CurrentValue; } }
        public static bool feed { get { return Miscc["feed"].Cast<CheckBox>().CurrentValue; } }
        public static bool mes { get { return Miscc["mes"].Cast<CheckBox>().CurrentValue; } }
        public static bool dev { get { return Miscc["dev"].Cast<CheckBox>().CurrentValue; } }
        public static bool cursor { get { return Miscc["cursor"].Cast<CheckBox>().CurrentValue; } }
        public static int akali { get { return Miscc["akali"].Cast<ComboBox>().CurrentValue; } }
        public static int thresh { get { return Miscc["thresh"].Cast<ComboBox>().CurrentValue; } }
        public static int amumu { get { return Miscc["amumu"].Cast<ComboBox>().CurrentValue; } }
        public static int azir { get { return Miscc["azir"].Cast<ComboBox>().CurrentValue; } }
        public static int kassadin { get { return Miscc["kassadin"].Cast<ComboBox>().CurrentValue; } }
        public static int tristana { get { return Miscc["tristana"].Cast<ComboBox>().CurrentValue; } }
        public static int zac { get { return Miscc["zac"].Cast<ComboBox>().CurrentValue; } }
        public static int annie { get { return Miscc["annie"].Cast<ComboBox>().CurrentValue; } }
        public static int karthus { get { return Miscc["karthus"].Cast<ComboBox>().CurrentValue; } }
        public static bool condemn { get { return Miscc["condemn"].Cast<CheckBox>().CurrentValue; } }
        public static bool randomult { get { return Miscc["randomult"].Cast<CheckBox>().CurrentValue; } }
        public static int udyr { get { return Miscc["udyr"].Cast<ComboBox>().CurrentValue; } }
        public static int veigar { get { return Miscc["veigar"].Cast<ComboBox>().CurrentValue; } }
        public static int warwick { get { return Miscc["warwick"].Cast<ComboBox>().CurrentValue; } }
        public static int illaoi { get { return Miscc["illaoi"].Cast<ComboBox>().CurrentValue; } }
        public static int hecarim { get { return Miscc["hecarim"].Cast<ComboBox>().CurrentValue; } }
        public static bool predictioner { get { return Miscc["predictioner"].Cast<CheckBox>().CurrentValue; } }
        public static int predictionerCB { get { return Miscc["predictionerCB"].Cast<ComboBox>().CurrentValue; } }
        public static int urgot { get { return Miscc["urgot"].Cast<ComboBox>().CurrentValue; } }
        public static int varus { get { return Miscc["varus"].Cast<ComboBox>().CurrentValue; } }
        public static int malzahar { get { return Miscc["malzahar"].Cast<ComboBox>().CurrentValue; } }
        public static int orbwalkerCB { get { return Miscc["orbwalkerCB"].Cast<ComboBox>().CurrentValue; } }
        public static bool universalMinimap { get { return Miscc["universalMinimap"].Cast<CheckBox>().CurrentValue; } }

        public static Menu Miscc;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static List<string> RandomUltChampsList = new List<string>(new[] { "Ezreal", "Jinx", "Ashe", "Draven", "Gangplank", "Ziggs", "Lux", "Xerath" });
        public static List<string> BaseUltList = new List<string>(new[] { "Jinx", "Ashe", "Draven", "Ezreal", "Karthus" });
        public static List<string> Champion = new List<string>(new[] {
            "Soraka", // 0
            "KogMaw", // 1
            "LeeSin", // 2
            "Kalista", // 3
            "Diana", // 4
            "Caitlyn", // 5
            "Twitch", // 6
            "Nidalee", // 7
            "Lucian", // 8
            "Ashe", // 9
            "Vayne", // 10
            "Jayce", // 11
            "Yasuo", // 12
            "Katarina", // 13
            "Xerath", // 14
            "Gragas", // 15
            "Draven", // 16
            "Ezreal", // 17
            "Brand", // 18
            "Blitzcrank", //19
            "Corki", // 20
            "Darius", // 21
            "Evelynn", // 22
            "Jhin", //23
            "Kindred", // 24
            "Lux", //25
            "Morgana", //26
            "Quinn", //27
            "TwistedFate", // 28
            "Kayle", //29
            "Jax", // 30
            "Sion", // 31
            "Ryze", //32
            "Sona", // 33
            "Trundle", // 34
            "Gangplank", //35
            "Poppy", // 36
            "Shaco", // 37
            "Leblanc", // 38
            "Ekko", // 39
            "Rumble", // 40
            "Riven", // 41
            "Graves", // 42
            "Elise", // 43
            "Rengar", //44
            "Zed", //45
            "Ahri", //46
            "RekSai", //47
            "Volibear", //48
            "Anivia", //49
            "Taliyah", //50
            "Janna", //51
            "Irelia", //52
            "Sivir", //53
            "JarvanIV", // 54
            "Braum", //55
            "Karma", //56
            "Teemo", //57
            "Cassiopeia", //58
            "Bard", //59
            "Olaf", // 60
            "Gnar", //61
            "Renekton", //62
            "Jinx", //63
            "Syndra", //64
            "Aatrox", //65
            "MissFortune", //66
            "DrMundo", //67
            "Akali", //68
            "Thresh", //69
            "Amumu", //70
            "Azir", // 71
            "Kassadin", //72
            "Tristana", //73
            "Zac", //74
            "Annie", //75
            "Karthus", //76
            "Udyr", //77
            "Veigar", // 78
            "Warwick", //79
            "Illaoi", //80
            "Hecarim", //81
            "Urgot", // 82
            "Varus", //83
            "Malzahar", //84
        });

        public static void Menu()
        {
            Miscc = MainMenu.AddMenu("PortAIO Misc", "berbsicmisc");
            Miscc.AddGroupLabel("Options ");
            Miscc.Add("intro", new CheckBox("Load Intro?", true));
            Miscc.Add("resetorb", new CheckBox("Reset Orbwalker", false)).OnValueChange += Loader_OnValueChange;
            Miscc.AddSeparator();
            Miscc.AddGroupLabel("Champion Dual Port : ");
            if (Champion.Contains(ObjectManager.Player.ChampionName))
            {
                if (Player.ChampionName.Equals(Champion[0]))
                {
                    Miscc.Add("soraka", new ComboBox("Use addon for Soraka : ", 0, "Sophie Soraka", "ChallengerSeries"));
                }
                if (Player.ChampionName.Equals(Champion[1]))
                {
                    Miscc.Add("kogmaw", new ComboBox("Use addon for Kog'Maw : ", 0, "Sharpshooter", "ChallengerSeries", "OKTW", "Marksman II", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[2]))
                {
                    Miscc.Add("leesin", new ComboBox("Use addon for Lee Sin : ", 0, "ValvraveSharp", "El Lee Sin : Reborn", "FreshBooster"));
                }
                if (Player.ChampionName.Equals(Champion[3]))
                {
                    Miscc.Add("kalista", new ComboBox("Use addon for Kalista : ", 0, "Marksman II", "iKalista - Reborn", "ChallengerSeries", "HastaKalista", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[4]))
                {
                    Miscc.Add("diana", new ComboBox("Use addon for Diana : ", 0, "ElDiana", "Nechrito Diana", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[5]))
                {
                    Miscc.Add("cait", new ComboBox("Use addon for Caitlyn : ", 0, "OKTW", "ExorAIO", "ChallengerSeries", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[6]))
                {
                    Miscc.Add("twitch", new ComboBox("Use addon for Twitch : ", 0, "OKTW", "Infected Twitch", "iTwitch", "ExorAIO", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[7]))
                {
                    Miscc.Add("nidalee", new ComboBox("Use addon for Nidalee : ", 0, "Kurisu", "Nechrito", "D_Nidalee"));
                }
                if (Player.ChampionName.Equals(Champion[8]))
                {
                    Miscc.Add("lucian", new ComboBox("Use addon for Lucian : ", 0, "LCS Lucian", "ChallengerSeries", "iLucian", "Marksman II", "OKTW", "Hoola Lucian", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[9]))
                {
                    Miscc.Add("ashe", new ComboBox("Use addon for Ashe : ", 0, "OKTW", "ChallengerSeries", "Marksman II", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[10]))
                {
                    Miscc.Add("vayne", new ComboBox("Use addon for Vayne : ", 0, "Vayne 2.0", "VayneHunterReborn", "hi im gosu", "hVayne SDK", "ExorAIO", "Challenger Series", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[11]))
                {
                    Miscc.Add("jayce", new ComboBox("Use addon for Jayce : ", 0, "OKTW", "Hoe's Jayce"));
                }
                if (Player.ChampionName.Equals(Champion[12]))
                {
                    Miscc.Add("yasuo", new ComboBox("Use addon for Yasuo : ", 0, "ValvraveSharp", "YasuoPro", "GosuMechanics", "YasuoSharpV2", "Firestorm AIO"));
                }
                if (Player.ChampionName.Equals(Champion[13]))
                {
                    Miscc.Add("katarina", new ComboBox("Use addon for Katarina : ", 0, "Staberina", "e.Motion Katarina"));
                }
                if (Player.ChampionName.Equals(Champion[14]))
                {
                    Miscc.Add("xerath", new ComboBox("Use addon for Xerath : ", 0, "OKTW", "ElXerath"));
                }
                if (Player.ChampionName.Equals(Champion[15]))
                {
                    Miscc.Add("gragas", new ComboBox("Use addon for Gragas : ", 0, "Drunk Carry", "Nechrito"));
                }
                if (Player.ChampionName.Equals(Champion[16]))
                {
                    Miscc.Add("draven", new ComboBox("Use addon for Draven : ", 0, "Sharp Shooter/Exor", "Tyler1", "Marksman II", "ExorAIO", "MoonDraven"));
                }
                if (Player.ChampionName.Equals(Champion[17]))
                {
                    Miscc.Add("ezreal", new ComboBox("Use addon for Ezreal : ", 0, "OKTW", "iDzEzreal", "Marksman II", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[18]))
                {
                    Miscc.Add("brand", new ComboBox("Use addon for Brand : ", 0, "TheBrand", "OKTW", "yol0Brand"));
                }
                if (Player.ChampionName.Equals(Champion[19]))
                {
                    Miscc.Add("blitzcrank", new ComboBox("Use addon for Blitzcrank : ", 0, "FreshBooster", "OKTW", "KurisuBlitz"));
                }
                if (Player.ChampionName.Equals(Champion[20]))
                {
                    Miscc.Add("corki", new ComboBox("Use addon for Corki : ", 0, "ElCorki", "OKTW", "D-Corki", "Marksman II", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[21]))
                {
                    Miscc.Add("darius", new ComboBox("Use addon for Darius : ", 0, "ExorAIO", "OKTW", "KurisuDarius"));
                }
                if (Player.ChampionName.Equals(Champion[22]))
                {
                    Miscc.Add("evelynn", new ComboBox("Use addon for Evelynn : ", 0, "Evelynn#", "OKTW", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[23]))
                {
                    Miscc.Add("jhin", new ComboBox("Use addon for Jhin : ", 0, "Jhin Virtuoso", "OKTW", "hJhin", "Marksman II", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[24]))
                {
                    Miscc.Add("kindred", new ComboBox("Use addon for Kindred : ", 0, "Kindred Yin Yang", "OKTW", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[25]))
                {
                    Miscc.Add("lux", new ComboBox("Use addon for Lux : ", 0, "MoonLux", "OKTW", "ElLux", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[26]))
                {
                    Miscc.Add("morgana", new ComboBox("Use addon for Morgana : ", 0, "Kurisu Morgana", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[27]))
                {
                    Miscc.Add("quinn", new ComboBox("Use addon for Quinn : ", 0, "GFuel Quinn", "OKTW", "ExorAIO", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[28]))
                {
                    Miscc.Add("twistedfate", new ComboBox("Use addon for TwistedFate : ", 0, "Esk0r", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[29]))
                {
                    Miscc.Add("kayle", new ComboBox("Use addon for Kayle : ", 0, "SephKayle", "OKTW", "D_Kayle"));
                }
                if (Player.ChampionName.Equals(Champion[30]))
                {
                    Miscc.Add("jax", new ComboBox("Use addon for Jax : ", 0, "xQx Jax", "NoobJaxReloaded", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[31]))
                {
                    Miscc.Add("sion", new ComboBox("Use addon for Sion : ", 0, "UnderratedAIO", "SimpleSion"));
                }
                if (Player.ChampionName.Equals(Champion[32]))
                {
                    Miscc.Add("ryze", new ComboBox("Use addon for Ryze : ", 0, "ExorAIO", "ElEasy Ryze", "SluttyRyze", "Arcane Ryze", "Sergix Ryze", "HeavenStrikeRyze"));
                }
                if (Player.ChampionName.Equals(Champion[33]))
                {
                    Miscc.Add("sona", new ComboBox("Use addon for Sona : ", 0, "vSupport", "ElEasy Sona"));
                }
                if (Player.ChampionName.Equals(Champion[34]))
                {
                    Miscc.Add("trundle", new ComboBox("Use addon for Trundle : ", 0, "ElTrundle", "FastTrundle", "UnderratedAIO"));
                }
                if (Player.ChampionName.Equals(Champion[35]))
                {
                    Miscc.Add("gangplank", new ComboBox("Use addon for GangPlank : ", 0, "UnderratedAIO"));
                }
                if (Player.ChampionName.Equals(Champion[36]))
                {
                    Miscc.Add("poppy", new ComboBox("Use addon for Poppy : ", 0, "UnderratedAIO", "BadaoKingdom"));
                }
                if (Player.ChampionName.Equals(Champion[37]))
                {
                    Miscc.Add("shaco", new ComboBox("Use addon for Shaco : ", 0, "UnderratedAIO", "ChewyMoon's Shaco"));
                }
                if (Player.ChampionName.Equals(Champion[38]))
                {
                    Miscc.Add("leblanc", new ComboBox("Use addon for LeBlanc : ", 0, "PopBlanc", "xQx LeBlanc", "FreshBooster", "LeBlanc II", "Badao LeBlanc"));
                }
                if (Player.ChampionName.Equals(Champion[39]))
                {
                    Miscc.Add("Ekko", new ComboBox("Use addon for Ekko : ", 0, "OKTW", "ElEkko", "EkkoGod"));
                }
                if (Player.ChampionName.Equals(Champion[40]))
                {
                    Miscc.Add("Rumble", new ComboBox("Use addon for Rumble : ", 0, "Underrated Rumble", "ElRumble"));
                }
                if (Player.ChampionName.Equals(Champion[41]))
                {
                    Miscc.Add("Riven", new ComboBox("Use addon for Riven : ", 0, "NechritoRiven", "Heaven Strike Riven", "KurisuRiven", "Hoola Riven"));
                }
                if (Player.ChampionName.Equals(Champion[42]))
                {
                    Miscc.Add("Graves", new ComboBox("Use addon for Graves : ", 0, "OKTW", "D-Graves", "Marksman II", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[43]))
                {
                    Miscc.Add("Elise", new ComboBox("Use addon for Elise : ", 0, "GFuel Elise", "D-Elise", "GOD Elise"));
                }
                if (Player.ChampionName.Equals(Champion[44]))
                {
                    Miscc.Add("rengar", new ComboBox("Use addon for Rengar : ", 0, "ElRengar", "D-Rengar", "PrideStalker"));
                }
                if (Player.ChampionName.Equals(Champion[45]))
                {
                    Miscc.Add("zed", new ComboBox("Use addon for Zed : ", 0, "ValvraveSharp", "Ze-D is Back", "iDZed"));
                }
                if (Player.ChampionName.Equals(Champion[46]))
                {
                    Miscc.Add("ahri", new ComboBox("Use addon for Ahri : ", 0, "OKTW", "AhriSharp"));
                }
                if (Player.ChampionName.Equals(Champion[47]))
                {
                    Miscc.Add("reksai", new ComboBox("Use addon for RekSai : ", 0, "D-RekSai", "HeavenStrike"));
                }
                if (Player.ChampionName.Equals(Champion[48]))
                {
                    Miscc.Add("volibear", new ComboBox("Use addon for VoliBear : ", 0, "Underrated Voli", "VoliPower"));
                }
                if (Player.ChampionName.Equals(Champion[49]))
                {
                    Miscc.Add("anivia", new ComboBox("Use addon for Anivia : ", 0, "OKTW", "ExorAIO", "Firestorm AIO"));
                }
                if (Player.ChampionName.Equals(Champion[50]))
                {
                    Miscc.Add("taliyah", new ComboBox("Use addon for Taliyah : ", 0, "Taliyah", "TophSharp"));
                }
                if (Player.ChampionName.Equals(Champion[51]))
                {
                    Miscc.Add("janna", new ComboBox("Use addon for Janna : ", 0, "LCS Janna", "FreshBooster"));
                }
                if (Player.ChampionName.Equals(Champion[52]))
                {
                    Miscc.Add("irelia", new ComboBox("Use addon for Irelia : ", 0, "ChallengerSeries", "IreliaGOD", "Irelia II", "Irelia Reloaded"));
                }
                if (Player.ChampionName.Equals(Champion[53]))
                {
                    Miscc.Add("sivir", new ComboBox("Use addon for Sivir : ", 0, "OKTW", "ExorAIO", "iSivir", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[54]))
                {
                    Miscc.Add("jarvan", new ComboBox("Use addon for Jarvan : ", 0, "BrianSharp", "D_Jarvan"));
                }
                if (Player.ChampionName.Equals(Champion[55]))
                {
                    Miscc.Add("braum", new ComboBox("Use addon for Braum : ", 0, "OKTW", "FreshBooster"));
                }
                if (Player.ChampionName.Equals(Champion[56]))
                {
                    Miscc.Add("karma", new ComboBox("Use addon for Karma : ", 0, "Spirit Karma", "Esk0r Karma", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[57]))
                {
                    Miscc.Add("teemo", new ComboBox("Use addon for Teemo : ", 0, "Sharpshooter", "Swiftly Teemo", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[58]))
                {
                    Miscc.Add("cassiopeia", new ComboBox("Use addon for Cassiopeia : ", 0, "SAutoCarry", "Seph Cassio", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[59]))
                {
                    Miscc.Add("bard", new ComboBox("Use addon for Bard : ", 0, "Asuna Bard", "FreshBooster"));
                }
                if (Player.ChampionName.Equals(Champion[60]))
                {
                    Miscc.Add("olaf", new ComboBox("Use addon for Olaf : ", 0, "ExorAIO", "Olaf is Back", "UnderratedAIO Olaf"));
                }
                if (Player.ChampionName.Equals(Champion[61]))
                {
                    Miscc.Add("gnar", new ComboBox("Use addon for Gnar : ", 0, "SluttyGnar", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[62]))
                {
                    Miscc.Add("renekton", new ComboBox("Use addon for Renekton : ", 0, "ExorAIO", "UnderratedAIO"));
                }
                if (Player.ChampionName.Equals(Champion[63]))
                {
                    Miscc.Add("jinx", new ComboBox("Use addon for Jinx : ", 0, "OKTW", "Marksman II", "GENESIS Jinx", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[64]))
                {
                    Miscc.Add("syndra", new ComboBox("Use addon for Syndra : ", 0, "OKTW", "Kortatu Syndra"));
                }
                if (Player.ChampionName.Equals(Champion[65]))
                {
                    Miscc.Add("aatrox", new ComboBox("Use addon for Aatrox : ", 0, "BrianSharp", "KappaSeries"));
                }
                if (Player.ChampionName.Equals(Champion[66]))
                {
                    Miscc.Add("missfortune", new ComboBox("Use addon for MissFortune : ", 0, "OKTW", "Marksman II", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[67]))
                {
                    Miscc.Add("mundo", new ComboBox("Use addon for Dr.Mundo : ", 0, "Hestia's Mundo", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[68]))
                {
                    Miscc.Add("akali", new ComboBox("Use addon for Akali : ", 0, "xQx Akali", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[69]))
                {
                    Miscc.Add("thresh", new ComboBox("Use addon for Thresh : ", 0, "OKTW", "Thesh the Ruler", "Thresh the Chain Warden", "Slutty Thresh", "yol0 Thresh", "Dark Star Thresh", "Thresh by Asuvril"));
                }
                if (Player.ChampionName.Equals(Champion[70]))
                {
                    Miscc.Add("amumu", new ComboBox("Use addon for Amumu : ", 0, "Shine#", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[71]))
                {
                    Miscc.Add("azir", new ComboBox("Use addon for Azir : ", 0, "HeavenStrikeAzir", "Creator Of Elo"));
                }
                if (Player.ChampionName.Equals(Champion[72]))
                {
                    Miscc.Add("kassadin", new ComboBox("Use addon for Kassadin : ", 0, "Kassawin", "Preserved Kassadin"));
                }
                if (Player.ChampionName.Equals(Champion[73]))
                {
                    Miscc.Add("tristana", new ComboBox("Use addon for Tristana : ", 0, "ElTristana", "ExorAIO", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[74]))
                {
                    Miscc.Add("zac", new ComboBox("Use addon for Zac : ", 0, "Underrated Zac", "The Secret Flubber"));
                }
                if (Player.ChampionName.Equals(Champion[75]))
                {
                    Miscc.Add("annie", new ComboBox("Use addon for Annie : ", 0, "OKTW Annie", "OAnnie"));
                }
                if (Player.ChampionName.Equals(Champion[76]))
                {
                    Miscc.Add("karthus", new ComboBox("Use addon for Karthus : ", 0, "OKTW Karthus", "KarthusSharp"));
                }
                if (Player.ChampionName.Equals(Champion[77]))
                {
                    Miscc.Add("udyr", new ComboBox("Use addon for Udyr : ", 0, "D_Udyr", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[78]))
                {
                    Miscc.Add("veigar", new ComboBox("Use addon for Veigar : ", 0, "FreshBooster", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[79]))
                {
                    Miscc.Add("warwick", new ComboBox("Use addon for Warwick : ", 0, "Warwick | The Blood Hunter", "ExorAIO"));
                }
                if (Player.ChampionName.Equals(Champion[80]))
                {
                    Miscc.Add("illaoi", new ComboBox("Use addon for Illaoi : ", 0, "Tentacle Kitty", "Flowers Illaoi"));
                }
                if (Player.ChampionName.Equals(Champion[81]))
                {
                    Miscc.Add("hecarim", new ComboBox("Use addon for Hecarim : ", 0, "Justy Hecarim", "UnderratedAIO"));
                }
                if (Player.ChampionName.Equals(Champion[82]))
                {
                    Miscc.Add("urgot", new ComboBox("Use addon for Urgot : ", 0, "OKTW", "Marksman II"));
                }
                if (Player.ChampionName.Equals(Champion[83]))
                {
                    Miscc.Add("varus", new ComboBox("Use addon for Varus : ", 0, "ElVarus", "Marksman II", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[84]))
                {
                    Miscc.Add("malzahar", new ComboBox("Use addon for Malzahar : ", 0, "OKTW", "SurvivorMalzahar"));
                }
            }
            else
            {
                Miscc.AddLabel("This champion is not supported for these feature.");
            }
            Miscc.AddSeparator();
            Miscc.AddGroupLabel("Util Dual-Port :");
            Miscc.Add("evadeCB", new ComboBox("Which Evade?", 0, "ezEvade", "Evade#"));
            Miscc.Add("activatorCB", new ComboBox("Which Activator?", 0, "ElUtilitySuite", "NabbActivator", "Activator#"));
            Miscc.Add("trackerCB", new ComboBox("Which Tracker?", 0, "NabbTracker", "Tracker#"));
            Miscc.Add("predictionerCB", new ComboBox("Which Predictioner?", 0, "EB", "SDK", "OKTW", "SPred", "L#"));
            Miscc.Add("orbwalkerCB", new ComboBox("Which Orbwalk/TargetSelect?", 1, "NOPE", "L#"));
            Miscc.AddSeparator();
            Miscc.AddGroupLabel("Util Changes");
            Miscc.AddLabel("Please F5 after making any changes below >>");
            Miscc.Add("champ", new CheckBox("Champ only mode? (No utils will load)", false));
            Miscc.Add("util", new CheckBox("Util only mode? (No champs will load)", false));
            Miscc.AddSeparator();
            Miscc.Add("activator", new CheckBox("Enable Activator?"));
            Miscc.Add("tracker", new CheckBox("Enable Tracker?"));
            Miscc.Add("recall", new CheckBox("Enable Recall Tracker?"));
            Miscc.Add("skin", new CheckBox("Enable Skin Hack?", false));
            Miscc.AddSeparator();
            Miscc.Add("evade", new CheckBox("Enable Evade?", false));
            Miscc.Add("predictioner", new CheckBox("Enable Predictioner?", true));
            Miscc.Add("dzaware", new CheckBox("Enable DZAwareness?", false));
            Miscc.Add("godTracker", new CheckBox("Enable God Jungle Tracker?", false));
            Miscc.AddSeparator();
            Miscc.Add("human", new CheckBox("Enable Humanizer?", false));
            Miscc.Add("gank", new CheckBox("Enable GankAlerter?", false));
            Miscc.Add("cheat", new CheckBox("Enable TheCheater?", false));
            Miscc.Add("randomult", new CheckBox("Enable Random Ult?", false));
            Miscc.AddSeparator();
            Miscc.Add("banwards", new CheckBox("Enable Sebby BanWars?", false));
            Miscc.Add("antialistar", new CheckBox("Enable AntiAlistar?", false));
            Miscc.Add("traptrack", new CheckBox("Enable TrapTracker?", false));
            Miscc.Add("limitedShat", new CheckBox("Enable LimitedShat?", false));
            Miscc.AddSeparator();
            Miscc.Add("autoLevel", new CheckBox("Enable Auto Level?", false));
            Miscc.Add("chatLogger", new CheckBox("Enable Chat Logger?", false));
            Miscc.Add("autoFF", new CheckBox("Enable Auto FF?", false));
            Miscc.Add("urfSpell", new CheckBox("Enable URF Spam Speller?", false));
            Miscc.AddSeparator();
            Miscc.Add("pastingSharp", new CheckBox("Enable PastingSharp?", false));
            Miscc.Add("emoteSpammer", new CheckBox("Enable Emote Spammer?", false));
            Miscc.Add("antiStealth", new CheckBox("Enable Anti Stealth (ElUtil)?", false));
            Miscc.Add("reform", new CheckBox("Enable Toxic Player Reform Program?", false));
            Miscc.AddSeparator();
            Miscc.Add("ping", new CheckBox("Enable Ping Block?", false));
            Miscc.Add("feed", new CheckBox("Enable Black Feeder 2.0?", false));
            Miscc.Add("mes", new CheckBox("Enable Mastery Emote Spammer?", false));
            Miscc.Add("dev", new CheckBox("Enable Developer Sharp?", false));
            Miscc.AddSeparator();
            Miscc.Add("cursor", new CheckBox("Enable VCursor?", false));
            Miscc.Add("condemn", new CheckBox("Enable Asuna Condemn (Vayne Only)?", false));
            Miscc.Add("universalMinimap", new CheckBox("Enable UniversalMiniMapHack?", false));

            var credits = Miscc.AddSubMenu("Credits");
            credits.AddLabel("Nathan or jQuery");
            credits.AddLabel("Exory");
            credits.AddLabel("TheSaltyWaffle");
            credits.AddLabel("Nechrito");
            credits.AddLabel("AlphaGod");
            credits.AddLabel("Trees");
            credits.AddLabel("Kurisu");
            credits.AddLabel("Kortatu- Admin");
            credits.AddLabel("TheNinow");
            credits.AddLabel("Sebby");
            credits.AddLabel("iJava");
            credits.AddLabel("Shine");
            credits.AddLabel("ARSkid");
            credits.AddLabel("ILuvBananas");
            credits.AddLabel("imsosharp - fuck you but no fuck you because your scripts are cool/amazing");
            credits.AddLabel("karmapanda - L# version kappa");
            credits.AddLabel("Somaher");
            credits.AddLabel("TheKushStyle");
            credits.AddLabel("Brian");
            credits.AddLabel("xQx");
            credits.AddLabel("Hoes");
            credits.AddLabel("God");
            credits.AddLabel("Diabaths");
            credits.AddLabel("Beaving - Admin");
            credits.AddLabel("RussianBlue");
            credits.AddLabel("Badao");
            credits.AddLabel("Asuna");
            credits.AddLabel("Seph");
            credits.AddLabel("Soresu");
            credits.AddLabel("Hestia");
            credits.AddLabel("xcsoft");
            credits.AddLabel("legacy");
            credits.AddLabel("Hikigaya");
            credits.AddLabel("mathieu002");
            credits.AddLabel("BestSkarnerNA");
            credits.AddLabel("Justy");
            credits.AddLabel("spawny");
            credits.AddLabel("ChewyMoon");
            credits.AddLabel("Shegeki14");
            credits.AddLabel("trooperhdx");
            credits.AddLabel("StopMotionCuber");
            credits.AddLabel("Detuks");
            credits.AddLabel("Veto");
            credits.AddLabel("tulisan69");
            credits.AddLabel("trus");
            credits.AddLabel("Synx");
            credits.AddLabel("iMeh - Admin (a really nice guy - this guy makes L#'s customer service beautiful)");
            credits.AddLabel("Kyon");
            credits.AddLabel("Doug");
            credits.AddLabel("LuNi");
            credits.AddLabel("Berb");
            credits.AddLabel("Muse30");

            /*
            //Miscc.Add("orbwalker", new CheckBox("Enable L# Orbwalker (HIGHLY BETA)?", false));
            public static bool orbwalker { get { return Miscc["orbwalker"].Cast<CheckBox>().CurrentValue; } }
            public static bool VCursor { get { return Miscc["VCursor"].Cast<CheckBox>().CurrentValue; } }
            //Miscc.Add("VCursor", new CheckBox("Enable VCursor?", false));
            Miscc.Add("stream", new CheckBox("Enable StreamBuddy?", false));
            public static bool stream { get { return Miscc["stream"].Cast<CheckBox>().CurrentValue; } }
            public static bool baseUlt { get { return Miscc["baseUlt"].Cast<CheckBox>().CurrentValue; } }

            if (BaseUltList.Contains(ObjectManager.Player.ChampionName))
            {
                Miscc.Add("baseUlt", new CheckBox("Enable Base Ult?", false));
            }
            */
        }
        
        private static void Loader_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (args.NewValue)
            {
                PortAIO.OrbwalkerManager.SetAttack(true);
                PortAIO.OrbwalkerManager.SetMovement(true);
                Orbwalker.ForcedTarget =(null);
                Console.Clear();
                Miscc["resetorb"].Cast<CheckBox>().CurrentValue = false;
            }
        }
    }
}