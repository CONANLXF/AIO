using LeagueSharp.Common;

using TargetSelector = PortAIO.TSManager; namespace Humanizer
{
    internal static class Extensions
    {
        public static int TimeSince(this int time)
        {
            return Utils.TickCount - time;
        }
    }
}