using System;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

 namespace DZAwarenessAIO.Utility.MenuUtility
{
    static class MenuExtensions
    {
        /// <summary>
        /// Adds a bool to the menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="defaultValue">the default value for the item.</param>
        /// <returns></returns>
        public static void AddBool(this Menu menu, string name, string displayName, bool defaultValue = false)
        {
            menu.Add(name, new CheckBox(displayName, defaultValue));
        }

        /// <summary>
        /// Adds a slider.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static void AddSlider(this Menu menu, string name, string displayName, Tuple<int, int, int> values)
        {
            menu.Add(name, new Slider(displayName, values.Item1, values.Item2, values.Item3));
        }

        /// <summary>
        /// Adds a slider.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns></returns>
        public static void AddSlider(this Menu menu, string name, string displayName, int value, int min, int max)
        {
            menu.Add(name, new Slider(displayName, value, min, max));
        }


        /// <summary>
        /// Adds a keybind.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static void AddKeybind(this Menu menu, string name, string displayName, Tuple<uint, KeyBind.BindTypes> value)
        {
            menu.Add(name, new KeyBind(displayName, false, value.Item2, value.Item1));
        }

        /// <summary>
        /// Adds a text.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        public static void AddText(this Menu menu, string name, string displayName)
        {
            menu.AddLabel(displayName);
        }

        /// <summary>
        /// Adds a string list.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static void AddStringList(this Menu menu, string name, string displayName, string[] value, int index = 0)
        {
            menu.Add(name, new ComboBox(displayName, index, value));
        }
    }
}