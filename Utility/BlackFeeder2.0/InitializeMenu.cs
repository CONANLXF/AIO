using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

 namespace BlackFeeder
{

    public class InitializeMenu
    {
        public static void Load()
        {
            Entry.Menu = MainMenu.AddMenu("BlackFeeder 2.0", "BlackFeeder");
            Entry.Menu.Add("Feeding.Activated", new CheckBox("Feeding Activated"));
            Entry.Menu.Add("Feeding.FeedMode", new ComboBox("Feeding Mode:", 0, "Middle Lane", "Bottom Lane", "Top Lane", "Random Lane"));

            Entry.feedingMenu = Entry.Menu.AddSubMenu("Feeding Options", "FeedingMenu");
            {
                Entry.feedingMenu.Add("Spells.Activated", new CheckBox("Spells Activated"));
                Entry.feedingMenu.Add("Messages.Activated", new CheckBox("Messages Activated"));
                Entry.feedingMenu.Add("Laugh.Activated", new CheckBox("Laugh Activated"));
                Entry.feedingMenu.Add("Items.Activated", new CheckBox("Items Activated"));
                Entry.feedingMenu.Add("Attacks.Disabled", new CheckBox("Disable auto attacks"));
            }

            Entry.miscMenu = Entry.Menu.AddSubMenu("Misc Options", "MiscMenu");
            {
                Entry.miscMenu.Add("Quit.Activated", new CheckBox("Quit on Game End"));
                Entry.miscMenu.Add("Surrender.Activated", new CheckBox("Auto Surrender Activated"));
            }
        }
    }
}