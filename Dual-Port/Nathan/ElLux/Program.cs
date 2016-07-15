 namespace ElLux
{
    using LeagueSharp.Common;

    internal class Program
    {
        #region Methods

        public static void Init()
        {
            CustomEvents.Game.OnGameLoad += Lux.OnLoad;
        }

        #endregion
    }
}