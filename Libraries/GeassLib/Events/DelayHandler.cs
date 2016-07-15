using GeassLib.Humanizer;

namespace GeassLib.Events
{
    class DelayHandler
    {
        public static TickManager MyTicker = new TickManager();
        public static bool Loaded;
        public static void Load()
        {
            MyTicker.AddTick("GeassLib.OnLevel",50,100);
            Loaded = true;
        }

        public static bool CheckOnLevel()
        {
            return MyTicker.CheckTick("GeassLib.OnLevel");
        }

        public static void UseOnLevel()
        {
            MyTicker.UseTick("GeassLib.OnLevel");
        }
    }
}
