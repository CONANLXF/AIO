using System.Drawing;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace UniversalMinimapHack
{
    public class Menu
    {
        public static EloBuddy.SDK.Menu.Menu menu;

        public Menu()
        {
            menu = MainMenu.AddMenu("Universal MinimapHack", "UniversalMinimapHack");

            menu.AddGroupLabel("[Customize] : ");
            menu.Add("scale", new Slider("Icon Scale % (F5 to Reload)", 20)); //_slider
            menu.Add("opacity", new Slider("Icon Opacity % (F5 to Reload)", 70)); //_iconOpacity

            //menu.Add

            menu.AddGroupLabel("SS Timer : ");
            menu.Add("enableSS", new CheckBox("Enable")); // _ssTimerEnabler

            menu.AddGroupLabel("Extra : ");
            menu.Add("minSS", new Slider("Show after X seconds", 30, 1, 180)); // _ssTimerMin
            menu.Add("fallbackSS", new CheckBox("Fallback ping (local)", false)); // _ssFallbackPing
            menu.Add("minPingSS", new Slider("Ping after X seconds", 30, 5, 180)); // _ssTimerMinPing

            menu.AddGroupLabel("Customize : ");
            menu.Add("sizeSS", new Slider("SS Text Size (F5 to Reload)", 15)); //_ssTimerSize
            menu.Add("offsetSS", new Slider("SS Text Height", 15, -50, +50)); // _ssTimerOffset

            menu.AddGroupLabel("SS Circles : ");
            menu.Add("ssCircle", new CheckBox("Enable")); //_ssCircle
            menu.Add("ssCircleSize", new Slider("Max Circle Size", 7000, 500, 15000)); // _ssCircleSize
        }

        public float IconScale
        {
            get { return menu["scale"].Cast<Slider>().CurrentValue / 100f; }
        }

        public float IconOpacity
        {
            get { return menu["opacity"].Cast<Slider>().CurrentValue / 100f; }
        }

        public bool SSTimer
        {
            get { return menu["enableSS"].Cast<CheckBox>().CurrentValue; }
        }

        public int SSTimerSize
        {
            get { return menu["sizeSS"].Cast<Slider>().CurrentValue; }
        }

        public int SSTimerOffset
        {
            get { return menu["offsetSS"].Cast<Slider>().CurrentValue; }
        }

        public int SSTimerStart
        {
            get { return menu["minSS"].Cast<Slider>().CurrentValue; }
        }

        public bool Ping
        {
            get { return menu["fallbackSS"].Cast<CheckBox>().CurrentValue; }
        }

        public int MinPing
        {
            get { return menu["minPingSS"].Cast<Slider>().CurrentValue; }
        }

        public bool SSCircle
        {
            get { return menu["ssCircle"].Cast<CheckBox>().CurrentValue; }
        }


        public int SSCircleSize
        {
            get { return menu["ssCircleSize"].Cast<Slider>().CurrentValue; }
        }
    }
}