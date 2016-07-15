using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace GeassLib.Functions
{
    public static class Menu
    {
        public static void AddBool(EloBuddy.SDK.Menu.Menu menu, string displayName, string name, bool value = true)
        {
            menu.Add(name, new CheckBox(displayName, value));
        }

        public static void AddSlider(EloBuddy.SDK.Menu.Menu menu, string displayName, string name, int startVal, int minVal = 0, int maxVal = 100)
        {
            menu.Add(name, new Slider(displayName, startVal, minVal, maxVal));
        }

        public static void AddKeyBind(EloBuddy.SDK.Menu.Menu menu, string displayName, string name, char key, KeyBind.BindTypes type)
        {
            menu.Add(name, new KeyBind(displayName, false, type, key));
        }

        public static void AddStringList(EloBuddy.SDK.Menu.Menu menu, string name, string displayName, string[] value, int index = 0)
        {
            menu.Add(name, new ComboBox(displayName, index, value));
        }
    }
}