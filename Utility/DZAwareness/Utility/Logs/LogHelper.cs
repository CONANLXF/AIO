using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp;
using EloBuddy;

 namespace DZAwarenessAIO.Utility.Logs
{
    class LogHelper
    {
        /// <summary>
        /// The log items
        /// </summary>
        private static List<LogItem> logItems = new List<LogItem>();

        /// <summary>
        /// Gets the log path.
        /// </summary>
        /// <value>
        /// The log path.
        /// </value>
        private static String LogPath
        {
            get { return Path.Combine(Variables.WorkingDir, string.Format("[DZA] Log - {0} - {1}.txt", Game.GameId, DateTime.Now.ToString("dd_MM_yyyy"))); }
        }

        /// <summary>
        /// Called when the LogHelper is loaded
        /// </summary>
        public static void OnLoad()
        {
            Console.WriteLine(@"[DZA] >>> Logger loaded successfully!");
        }

        /// <summary>
        /// Adds an item to the log.
        /// </summary>
        /// <param name="logItem">The log item.</param>
        public static void AddToLog(LogItem logItem)
        {
            CreateDirectory();
            logItems.Add(logItem);
            SaveToFile(logItem);
        }

        /// <summary>
        /// Gets the logs.
        /// </summary>
        /// <returns></returns>
        public static List<LogItem> GetLogs()
        {
            return logItems;
        }

        /// <summary>
        /// Clears the logs.
        /// </summary>
        public static void ClearLogs()
        {
            logItems.Clear();
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        private static void CreateDirectory()
        {
            if (!Directory.Exists(Variables.WorkingDir))
            {
                Directory.CreateDirectory(Variables.WorkingDir);
            }

            if (!File.Exists(LogPath))
            {
                //File.Create(LogPath);
                InitLog();
            }
        }

        /// <summary>
        /// Initializes the log file.
        /// </summary>
        private static void InitLog()
        {
            var logString = string.Format("[{0} | {1} | {2} | Assembly Version. {3}] \r\n[Enemies: {4}] \r\n[Allies: {5}] \r\n \r\n", DateTime.Now.ToString("yyyy_MM_dd"),
                    Variables.GameRegion,
                    Variables.GameVersion,
                    Variables.AssemblyVersion,
                    Variables.EnemyTeam.Aggregate("", (current, en) => current + (" " + en)),
                    Variables.OwnTeam.Aggregate("", (current, en) => current + (" " + en)));
            try
            {
                File.AppendAllText(LogPath, logString);
            }
            catch
            {
                Console.WriteLine(@"[DZAwareness] >>> Exception: Cannot Write To Logs File.");
            }
        }

        /// <summary>
        /// Saves to the log file.
        /// </summary>
        /// <param name="logItem">The log item.</param>
        private static void SaveToFile(LogItem logItem)
        {
            try
            {
                File.AppendAllText(LogPath, logItem.GetLoggingString());
            }
            catch
            {
                Console.WriteLine(@"[DZAwareness] >>> Exception: Cannot Write To Logs File.");
            }
        }
    }
}
