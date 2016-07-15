namespace Valvrave_Sharp
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using EloBuddy;
    using Valvrave_Sharp.Core;
    using Valvrave_Sharp.Plugin;
    using EloBuddy.SDK.Menu;
    #endregion

    internal class Program
    {
        #region Constants

        internal const int FlashRange = 425, IgniteRange = 600, SmiteRange = 570;

        #endregion

        #region Static Fields

        internal static EloBuddy.SDK.Item Bilgewater, BotRuinedKing, Youmuu, Tiamat, Hydra, Titanic;

        internal static SpellSlot Flash = SpellSlot.Unknown, Ignite = SpellSlot.Unknown, Smite = SpellSlot.Unknown;

        internal static Menu _MainMenu;

        internal static AIHeroClient Player;

        internal static Spell Q, Q2, Q3, W, E, E2, R, R2;

        private static readonly Dictionary<string, Tuple<Func<object>, int>> Plugins =
            new Dictionary<string, Tuple<Func<object>, int>>
                {
                   // { "DrMundo", new Tuple<Func<object>, int>(() => new DrMundo(), 9) },
                   // { "Kennen", new Tuple<Func<object>, int>(() => new Kennen(), 6) },
                    { "LeeSin", new Tuple<Func<object>, int>(() => new LeeSin(), 10) },
                    //{ "Vladimir", new Tuple<Func<object>, int>(() => new Vladimir(), 7) },
                    { "Yasuo", new Tuple<Func<object>, int>(() => new Yasuo(), 3) },
                    { "Zed", new Tuple<Func<object>, int>(() => new Zed(), 3) }
};

        #endregion

        #region Methods

        private static void InitMenu(bool isSupport)
        {
            if (YasuoPro.YasuoMenu.ComboM != null || YasuoSharpV2.YasuoSharp.comboMenu != null)
            {
                _MainMenu = MainMenu.AddMenu("YasuoEvade", "YasEvadee");
            }
            else
            {
                _MainMenu = MainMenu.AddMenu("ValvraveSharp", "Valvrave Sharp");
            }
            if (isSupport)
            {
                Plugins[Player.ChampionName].Item1.Invoke();
            }
        }

        private static void InitItem()
        {
            Bilgewater = new EloBuddy.SDK.Item(ItemId.Bilgewater_Cutlass, 550);
            BotRuinedKing = new EloBuddy.SDK.Item(ItemId.Blade_of_the_Ruined_King, 550);
            Youmuu = new EloBuddy.SDK.Item(ItemId.Youmuus_Ghostblade, 0);
            Tiamat = new EloBuddy.SDK.Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new EloBuddy.SDK.Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Titanic = new EloBuddy.SDK.Item(3748, 0);
        }

        private static void InitSummonerSpell()
        {
            foreach (var smite in
                Player.Spellbook.Spells.Where(
                    i =>
                    (i.Slot == SpellSlot.Summoner1 || i.Slot == SpellSlot.Summoner2)
                    && i.Name.ToLower().Contains("smite")))
            {
                Smite = smite.Slot;
                break;
            }
            Ignite = Player.GetSpellSlot("SummonerDot");
            Flash = Player.GetSpellSlot("SummonerFlash");
        }

        public static void MainA()
        {
            Player = ObjectManager.Player;
            var isSupport = Plugins.ContainsKey(Player.ChampionName);
            InitMenu(isSupport);
            InitItem();
            InitSummonerSpell();
        }

        private static void PrintChat(string text)
        {
            Chat.Print("Valvrave Sharp => {0}", text);
        }

        #endregion
    }
}