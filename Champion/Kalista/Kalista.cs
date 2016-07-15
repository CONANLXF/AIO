using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DZLib.Modules;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using iKalistaReborn.Modules;
using iKalistaReborn.Utils;
using LeagueSharp.Common;
using TargetSelector = PortAIO.TSManager;
using SharpDX;

namespace iKalistaReborn
{
    internal class Kalista
    {

        public static Menu Menu;

        /// <summary>
        ///     The Modules
        /// </summary>
        public static readonly List<IModule> Modules = new List<IModule>
        {
            new AutoRendModule(),
            new JungleStealModule(),
            new AutoEModule(),
            new AutoELeavingModule(), new WallJumpModule()
        };

        private readonly List<Vector3[]> possibleJumpSpots = new List<Vector3[]>();

        public static float LastAutoAttack;
        public static Menu comboMenu, mixedMenu, laneclearMenu, jungleStealMenu, miscMenu, drawingMenu;

        public Kalista()
        {
            CreateMenu();
            SentinelManager.Initialize();
            LoadModules();
            PopulateList();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            CustomDamageIndicator.DamageToUnit = Helper.GetRendDamage;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Spellbook.OnCastSpell += (sender, args) =>
            {
                if (sender.Owner.IsMe && args.Slot == SpellSlot.Q && ObjectManager.Player.IsDashing())
                {
                    args.Process = false;
                }
            };

            if (PortAIO.OrbwalkerManager.isEBActive)
            {
                Orbwalker.OnUnkillableMinion += (Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args) =>
                {
                    var killableMinion = target as Obj_AI_Base;
                    if (killableMinion == null || !SpellManager.Spell[SpellSlot.E].IsReady() || ObjectManager.Player.HasBuff("summonerexhaust") || !killableMinion.HasRendBuff())
                    {
                        return;
                    }

                    if (getCheckBoxItem(laneclearMenu, "com.ikalista.laneclear.useEUnkillable") &&
                        killableMinion.IsMobKillable())
                    {
                        SpellManager.Spell[SpellSlot.E].Cast();
                    }
                };
            }
            else
            {
                PortAIO.Init.LSOrbwalker.OnNonKillableMinion += (AttackableUnit minion) =>
                {
                    var killableMinion = minion as Obj_AI_Base;
                    if (killableMinion == null || !SpellManager.Spell[SpellSlot.E].IsReady() || ObjectManager.Player.HasBuff("summonerexhaust") || !killableMinion.HasRendBuff())
                    {
                        return;
                    }

                    if (getCheckBoxItem(laneclearMenu, "com.ikalista.laneclear.useEUnkillable") &&
                        killableMinion.IsMobKillable())
                    {
                        SpellManager.Spell[SpellSlot.E].Cast();
                    }
                };
            }

            LSEvents.BeforeAttack += (BeforeAttackArgs args) =>
            {
                if (!getCheckBoxItem(miscMenu, "com.ikalista.misc.forceW")) return;

                args.Target = HeroManager.Enemies.FirstOrDefault(x => ObjectManager.Player.LSDistance(x) <= 600 && x.HasBuff("kalistacoopstrikemarkally"));
                if (args.Target != null)
                {
                    PortAIO.OrbwalkerManager.ForcedTarget(args.Target as Obj_AI_Base);
                }
            };
        }

        private void PopulateList()
        {
            // blue side wolves - left wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(2848, 6942, 53), new Vector3(3058, 6960, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(3064, 6962, 52), new Vector3(2809, 6936, 53) });

            // blue side wolves - left wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(2774, 6558, 57), new Vector3(3072, 6607, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(3074, 6608, 51), new Vector3(2755, 6523, 57) });

            // blue side wolves - left wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(3024, 6108, 57), new Vector3(3195, 6307, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(3200, 6243, 52), new Vector3(3022, 6111, 57) });

            // red side wolves - right wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(11772, 8856, 50), new Vector3(11513, 8762, 65) });
            possibleJumpSpots.Add(new[] { new Vector3(11572, 8706, 64), new Vector3(11817, 8903, 50) });

            // red side wolves - right wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(11772, 8206, 55), new Vector3(12095, 8281, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(12072, 8256, 52), new Vector3(11755, 8206, 55) });

            // red side wolves - right wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(11772, 7906, 52), new Vector3(12110, 7980, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(12072, 7906, 53), new Vector3(11767, 7900, 52) });

            // bottom bush wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(11410, 5526, 23), new Vector3(11647, 5452, 54) });
            possibleJumpSpots.Add(new[] { new Vector3(11646, 5452, 54), new Vector3(11354, 5511, 8) });

            // bottom bush wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(11722, 5058, 52), new Vector3(11345, 4813, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(11428, 4984, -71), new Vector3(11725, 5120, 52) });

            // bot bush wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(11772, 4608, -71), new Vector3(11960, 4802, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(11922, 4758, 51), new Vector3(11697, 4614, -71) });

            // top bush wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(3074, 10056, 54), new Vector3(3437, 10186, -66) });
            possibleJumpSpots.Add(new[] { new Vector3(3324, 10206, -65), new Vector3(2964, 10012, 54) });

            // top bush wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(3474, 9856, -65), new Vector3(3104, 9701, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(3226, 9752, 52), new Vector3(3519, 9833, -65) });

            // top bush wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(3488, 9414, 13), new Vector3(3224, 9440, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(3226, 9438, 51), new Vector3(3478, 9422, 16) });

            // mid wall - top side (top)
            possibleJumpSpots.Add(new[] { new Vector3(6524, 8856, -71), new Vector3(6685, 9116, 49) });
            possibleJumpSpots.Add(new[] { new Vector3(6664, 9002, 43), new Vector3(6484, 8804, -71) });

            // mid wall - top side (middle)
            possibleJumpSpots.Add(new[] { new Vector3(6874, 8606, -69), new Vector3(7095, 8727, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(7074, 8706, 52), new Vector3(6857, 8517, -71) });

            // mid wall - top side (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(7174, 8256, -33), new Vector3(7456, 8539, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(7422, 8406, 53), new Vector3(7100, 8159, -24) });

            // mid wall - bot side (top)
            possibleJumpSpots.Add(new[] { new Vector3(7658, 6512, 5), new Vector3(7378, 6298, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(7470, 6260, 52), new Vector3(7714, 6544, -1) });

            // mid wall - bot side (middle)
            possibleJumpSpots.Add(new[] { new Vector3(8034, 6198, -71), new Vector3(7813, 5938, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(7898, 6004, 51), new Vector3(8139, 6210, -71) });

            // mid wall - bot side (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(8222, 5808, 32), new Vector3(8412, 6081, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(8344, 6022, -71), new Vector3(8194, 5742, 42) });

            // baron wall
            possibleJumpSpots.Add(new[] { new Vector3(5774, 10656, 55), new Vector3(5355, 10657, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(5474, 10656, -71), new Vector3(5812, 10832, 55) });

            // baron entrance wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(4474, 10406, -71), new Vector3(4292, 10199, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(4292, 10270, -71), new Vector3(4480, 10437, -71) });

            // baron entrance wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(5074, 10006, -71), new Vector3(4993, 9706, -70) });
            possibleJumpSpots.Add(new[] { new Vector3(5000, 9754, -71), new Vector3(5083, 9998, -71) });

            // dragon wall
            possibleJumpSpots.Add(new[] { new Vector3(9322, 4358, -71), new Vector3(8971, 4284, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(9072, 4208, 53), new Vector3(9378, 4431, -71) });

            // dragon entrance wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(9812, 4918, -71), new Vector3(9803, 5249, -68) });
            possibleJumpSpots.Add(new[] { new Vector3(9822, 5158, -71), new Vector3(9751, 4884, -71) });

            // dragon entrance wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(10422, 4458, -71), new Vector3(10643, 4641, -68) });
            possibleJumpSpots.Add(new[] { new Vector3(10622, 4558, -71), new Vector3(10375, 4441, -71) });

            // top golllems wall
            possibleJumpSpots.Add(new[] { new Vector3(6524, 12006, 56), new Vector3(6553, 11666, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(6574, 11706, 53), new Vector3(6543, 12054, 56) });

            // bot gollems wall
            possibleJumpSpots.Add(new[] { new Vector3(8250, 2894, 51), new Vector3(8213, 3326, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(8222, 3158, 51), new Vector3(8282, 2741, 51) });

            // blue side bot tribush wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(9482, 2786, 49), new Vector3(9535, 3203, 55) });
            possibleJumpSpots.Add(new[] { new Vector3(9530, 3126, 59), new Vector3(9505, 2756, 49) });

            // blue side bot tribush wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(9772, 2758, 49), new Vector3(9862, 3111, 58) });
            possibleJumpSpots.Add(new[] { new Vector3(9872, 3066, 58), new Vector3(9815, 2673, 49) });

            // blue side bot tribush wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(10206, 2888, 49), new Vector3(10046, 2675, 49) });
            possibleJumpSpots.Add(new[] { new Vector3(10022, 2658, 49), new Vector3(10259, 2925, 49) });

            // red side toplane tribush wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(5274, 11806, 57), new Vector3(5363, 12185, 56) });
            possibleJumpSpots.Add(new[] { new Vector3(5324, 12106, 56), new Vector3(5269, 11725, 57) });

            // red side toplane tribush wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(5000, 11874, 57), new Vector3(5110, 12210, 56) });
            possibleJumpSpots.Add(new[] { new Vector3(5072, 12146, 56), new Vector3(4993, 11836, 57) });

            // red side toplane tribush wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(4624, 12006, 57), new Vector3(4825, 12307, 56) });
            possibleJumpSpots.Add(new[] { new Vector3(4776, 12224, 56), new Vector3(4605, 11970, 57) });

            // blue side razorbeak wall
            possibleJumpSpots.Add(new[] { new Vector3(7372, 5858, 52), new Vector3(7115, 5524, 55) });
            possibleJumpSpots.Add(new[] { new Vector3(7174, 5608, 58), new Vector3(7424, 5905, 52) });

            // blue side blue buff wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(3774, 7706, 52), new Vector3(3856, 7412, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(3828, 7428, 51), new Vector3(3802, 7743, 52) });

            // blue side blue buff wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(3424, 7408, 52), new Vector3(3422, 7759, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(3434, 7722, 52), new Vector3(3437, 7398, 52) });

            // blue side blue buff - right wall
            possibleJumpSpots.Add(new[] { new Vector3(4144, 8030, 50), new Vector3(4382, 8149, 48) });
            possibleJumpSpots.Add(new[] { new Vector3(4374, 8156, 48), new Vector3(4124, 8022, 50) });

            // blue side rock between blue buff/baron (left)
            possibleJumpSpots.Add(new[] { new Vector3(4664, 8652, -10), new Vector3(4624, 9010, -68) });
            possibleJumpSpots.Add(new[] { new Vector3(4662, 8896, -69), new Vector3(4672, 8519, 26) });

            // blue side rock between blue buff/baron (right)
            possibleJumpSpots.Add(new[] { new Vector3(3774, 9206, -14), new Vector3(4074, 9322, -67) });
            possibleJumpSpots.Add(new[] { new Vector3(4024, 9306, -68), new Vector3(3737, 9233, -8) });

            // red side blue buff wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(11022, 7208, 51), new Vector3(10904, 7521, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(11022, 7506, 52), new Vector3(11040, 7179, 51) });

            // red side blue buff wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(11440, 7208, 52), new Vector3(11449, 7517, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(11470, 7486, 52), new Vector3(11458, 7155, 52) });

            // red side rock between blue buff/dragon (left)
            possibleJumpSpots.Add(new[] { new Vector3(10172, 6208, 16), new Vector3(10189, 5922, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(10172, 5958, -71), new Vector3(10185, 6286, 29) });

            // red side rock between blue buff/dragon (right)
            possibleJumpSpots.Add(new[] { new Vector3(10722, 5658, -66), new Vector3(11049, 5660, -22) });
            possibleJumpSpots.Add(new[] { new Vector3(11022, 5658, -30), new Vector3(10665, 5662, -68) });

            // blue side top tribush wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(2574, 9656, 54), new Vector3(2800, 9596, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(2774, 9656, 53), new Vector3(2537, 9674, 54) });

            // blue side top tribush wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(2874, 9306, 51), new Vector3(2500, 9262, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(2598, 9272, 52), new Vector3(2884, 9291, 51) });

            // blue side wolves - right wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(4624, 5858, 51), new Vector3(4772, 5636, 50) });
            possibleJumpSpots.Add(new[] { new Vector3(4774, 5658, 50), new Vector3(4644, 5876, 51) });

            // blue side wolves - right wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(4924, 6158, 52), new Vector3(4869, 6452, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(4874, 6408, 51), new Vector3(4938, 6062, 51) });

            // blue razorbeak - left wall
            possibleJumpSpots.Add(new[] { new Vector3(6174, 5308, 49), new Vector3(5998, 5536, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(6024, 5508, 52), new Vector3(6199, 5286, 49) });

            // red side bottom tribush wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(12260, 5220, 52), new Vector3(12027, 5265, 54) });
            possibleJumpSpots.Add(new[] { new Vector3(12122, 5208, 54), new Vector3(12327, 5243, 52) });

            // red side bottom tribush wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(11972, 5558, 54), new Vector3(12343, 5498, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(12272, 5558, 53), new Vector3(11969, 5480, 55) });

            // red side razorbeak - rightdown wall
            possibleJumpSpots.Add(new[] { new Vector3(8672, 9606, 50), new Vector3(8831, 9384, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(8830, 9382, 52), new Vector3(8646, 9635, 50) });

            // red side wolves - left wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(10222, 9056, 50), new Vector3(10061, 9282, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(10072, 9306, 52), new Vector3(10193, 9052, 50) });

            // red side wolves - left wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(9972, 8506, 68), new Vector3(9856, 8831, 50) });
            possibleJumpSpots.Add(new[] { new Vector3(9872, 8756, 50), new Vector3(9967, 8429, 65) });

            // red size razorbeak - right wall
            possibleJumpSpots.Add(new[] { new Vector3(8072, 9806, 51), new Vector3(8369, 9807, 50) });
            possibleJumpSpots.Add(new[] { new Vector3(8372, 9806, 50), new Vector3(8066, 9796, 51) });

            // blue side base wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(4524, 3258, 96), new Vector3(4780, 3460, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(4774, 3408, 51), new Vector3(4463, 3260, 96) });

            // blue side base wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(3074, 4558, 96), new Vector3(3182, 4917, 54) });
            possibleJumpSpots.Add(new[] { new Vector3(3174, 4858, 54), new Vector3(3085, 4539, 96) });

            // red side base wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(11712, 10390, 91), new Vector3(11621, 10092, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(11622, 10106, 52), new Vector3(11735, 10430, 91) });

            // red base wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(10308, 11682, 91), new Vector3(9999, 11554, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(10022, 11556, 52), new Vector3(10321, 11664, 91) });
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        /// <summary>
        ///     This is where jeff creates his first Menu in a long time
        /// </summary>
        private void CreateMenu()
        {
            Menu = MainMenu.AddMenu("iKalista: Reborn", "com.ikalista");

            comboMenu = Menu.AddSubMenu("iKalista: Reborn - Combo", "com.ikalista.combo");
            comboMenu.Add("com.ikalista.combo.useQ", new CheckBox("Use Q", true));
            comboMenu.Add("com.ikalista.combo.useE", new CheckBox("Use E", true));
            comboMenu.Add("com.ikalista.combo.stacks", new Slider("Rend at X stacks", 10, 1, 20));
            comboMenu.Add("com.ikalista.combo.eLeaving", new CheckBox("Use E Leaving", true));
            comboMenu.Add("com.ikalista.combo.ePercent", new Slider("Min Percent for E Leaving", 50, 10, 100));
            comboMenu.Add("com.ikalista.combo.saveMana", new CheckBox("Save Mana for E", true));
            comboMenu.Add("com.ikalista.combo.saveAlly", new CheckBox("Save Ally With R", true));
            comboMenu.Add("com.ikalista.combo.balista", new CheckBox("Use Balista", true));
            comboMenu.Add("com.ikalista.combo.autoE", new CheckBox("Auto E Minion > Champion", true));
            comboMenu.Add("com.ikalista.combo.orbwalkMinions", new CheckBox("Orbwalk Minions in combo", true));
            comboMenu.Add("com.ikalista.combo.allyPercent", new Slider("Min Health % for Ally", 20, 10, 100));

            mixedMenu = Menu.AddSubMenu("iKalista: Reborn - Mixed", "com.ikalista.mixed");
            mixedMenu.Add("com.ikalista.mixed.useQ", new CheckBox("Use Q", true));
            mixedMenu.Add("com.ikalista.mixed.useE", new CheckBox("Use E", true));
            mixedMenu.Add("com.ikalista.mixed.stacks", new Slider("Rend at X stacks", 10, 1, 20));

            laneclearMenu = Menu.AddSubMenu("iKalista: Reborn - Laneclear", "com.ikalista.laneclear");
            laneclearMenu.Add("com.ikalista.laneclear.useQ", new CheckBox("Use Q", true));
            laneclearMenu.Add("com.ikalista.laneclear.qMinions", new Slider("Min Minions for Q", 3, 1, 10));
            laneclearMenu.Add("com.ikalista.laneclear.useE", new CheckBox("Use E", true));
            laneclearMenu.Add("com.ikalista.laneclear.eMinions", new Slider("Min Minions for E", 5, 1, 10));
            laneclearMenu.Add("com.ikalista.laneclear.useEUnkillable", new CheckBox("E Unkillable Minions", true));
            laneclearMenu.Add("com.ikalista.laneclear.eSiege", new CheckBox("Auto E Siege Minions", true));

            jungleStealMenu = Menu.AddSubMenu("iKalista: Reborn - Jungle Steal", "com.ikalista.jungleSteal");
            jungleStealMenu.Add("com.ikalista.jungleSteal.enabled", new CheckBox("Use Rend To Steal Jungle Minions", true));
            //foreach (var minion in JungleMinions)
            //{
            //jungleStealMenu.Add(minion.Key, new CheckBox(minion.Value, true));
            //}
            jungleStealMenu.Add("com.ikalista.jungleSteal.small", new CheckBox("Kill Small Minions", true));
            jungleStealMenu.Add("com.ikalista.jungleSteal.large", new CheckBox("Kill Large Minions", true));
            jungleStealMenu.Add("com.ikalista.jungleSteal.legendary", new CheckBox("Kill Legendary Minions", true));

            miscMenu = Menu.AddSubMenu("iKalista: Reborn - Misc", "com.ikalista.Misc");
            miscMenu.Add("com.ikalista.misc.reduceE", new Slider("Reduce Rend Damage", 90, 0, 300));
            miscMenu.Add("com.ikalista.misc.forceW", new CheckBox("Focus Enemy With W"));
            miscMenu.Add("com.ikalista.misc.exploit", new CheckBox("Exploit"));
            if (Game.MapId != GameMapId.SummonersRift)
            {
                miscMenu.AddLabel("Sentinel Manager is only on Summoners Rift, sorry.");
            }
            else
            {
                miscMenu.AddGroupLabel("Sentinel Manager (HellSing) :");
                miscMenu.Add("enabled", new CheckBox("Enabled"));
                miscMenu.Add("noMode", new CheckBox("Only use when no mode active"));
                miscMenu.Add("alert", new CheckBox("Alert when sentinel is taking damage"));
                miscMenu.Add("mana", new Slider("Minimum mana available when casting W ({0}%)", 40));
                miscMenu.AddLabel("Send to the following locations (no specific order):");
                miscMenu.Add("baron", new CheckBox("Baron (stuck bug usage)"));
                miscMenu.Add("dragon", new CheckBox("Dragon (stuck bug usage)"));
                miscMenu.Add("mid", new CheckBox("Mid lane brush"));
                miscMenu.Add("blue", new CheckBox("Blue buff"));
                miscMenu.Add("red", new CheckBox("Red buff"));
                SentinelManager.RecalculateOpenLocations();

                miscMenu["baron"].Cast<CheckBox>().OnValueChange += OnValueChange;
                miscMenu["dragon"].Cast<CheckBox>().OnValueChange += OnValueChange;
                miscMenu["mid"].Cast<CheckBox>().OnValueChange += OnValueChange;
                miscMenu["blue"].Cast<CheckBox>().OnValueChange += OnValueChange;
                miscMenu["red"].Cast<CheckBox>().OnValueChange += OnValueChange;
            }

            drawingMenu = Menu.AddSubMenu("iKalista: Reborn - Drawing", "com.ikalista.drawing");
            drawingMenu.Add("com.ikalista.drawing.spellRanges", new CheckBox("Draw Spell Ranges"));
            drawingMenu.Add("com.ikalista.drawing.spellRanges", new CheckBox("Draw Spell Ranges"));
            drawingMenu.Add("com.ikalista.drawing.junpSpots", new CheckBox("Draw Jump Spots", true));
            drawingMenu.Add("com.ikalista.drawing.eDamage", new CheckBox("Draw E Damage to Enemies"));//.SetValue(new Circle(true, Color.DarkOliveGreen)));
            drawingMenu.Add("com.ikalista.drawing.eDamageJ", new CheckBox("Draw E Damage to Jungle Minions"));//.SetValue(new Circle(true, Color.DarkOliveGreen)));
            drawingMenu.Add("com.ikalista.drawing.damagePercent", new CheckBox("Draw Percent Damage"));//.SetValue(new Circle(true, Color.DarkOliveGreen)));
        }


        private static void OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            SentinelManager.RecalculateOpenLocations();
        }

        private void LoadModules()
        {
            foreach (var module in Modules.Where(x => x.ShouldGetExecuted()))
            {
                try
                {
                    module.OnLoad();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error loading module: " + module.GetName() + " Exception: " + e);
                }
            }
        }

        /// <summary>
        ///     My names definatly jeffery.
        /// </summary>
        /// <param name="args">even more gay</param>
        private void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawingMenu, "com.ikalista.drawing.spellRanges"))
            {
                foreach (var spell in SpellManager.Spell.Values)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, System.Drawing.Color.DarkOliveGreen);
                }
            }

            if (getCheckBoxItem(drawingMenu, "com.ikalista.drawing.junpSpots")
                 && PortAIO.OrbwalkerManager.isCustomKeyActive)
            {
                foreach (var spot in possibleJumpSpots)
                {
                    var start = spot[0];
                    var end = spot[1];

                    if (ObjectManager.Player.Distance(start) <= 5000f
                        || ObjectManager.Player.Distance(end) <= 5000f && SpellManager.Spell[SpellSlot.Q].IsReady())
                    {
                        Drawing.DrawCircle(start, 100, System.Drawing.Color.Chartreuse);
                        Drawing.DrawCircle(end, 100, System.Drawing.Color.MediumSeaGreen);
                    }
                }
            }

            if (getCheckBoxItem(drawingMenu, "com.ikalista.drawing.damagePercent"))
            {
                foreach (var source in HeroManager.Enemies.Where(x => ObjectManager.Player.LSDistance(x) <= 2000f && !x.IsDead && x.IsHPBarRendered))
                {
                    var currentPercentage = Math.Round(Helper.GetRendDamage(source) * 100 / source.GetHealthWithShield(), 1);

                    Drawing.DrawText(Drawing.WorldToScreen(source.Position)[0], Drawing.WorldToScreen(source.Position)[1], currentPercentage >= 100 ? System.Drawing.Color.DarkOliveGreen : System.Drawing.Color.White, currentPercentage >= 100 ? "Killable With E" : "Current Damage: " + Math.Ceiling(Helper.GetRendDamage(source) * 100 / source.GetHealthWithShield()) + "%");
                }
            }
        }

        /// <summary>
        ///     The on process spell function
        /// </summary>
        /// <param name="sender">
        ///     The Spell Sender
        /// </param>
        /// <param name="args">
        ///     The Arguments
        /// </param>
        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "KalistaExpungeWrapper")
            {
                PortAIO.OrbwalkerManager.ResetAutoAttackTimer();
            }

            if (sender.Type == GameObjectType.obj_AI_Base && sender.IsEnemy && args.Target != null &&
                getCheckBoxItem(comboMenu, "com.ikalista.combo.saveAlly"))
            {
                var soulboundhero =
                    HeroManager.Allies.FirstOrDefault(
                        hero =>
                            hero.HasBuff("kalistacoopstrikeally") && args.Target.NetworkId == hero.NetworkId);

                if (soulboundhero != null &&
                    soulboundhero.HealthPercent < getSliderItem(comboMenu, "com.ikalista.combo.allyPercent"))
                {
                    SpellManager.Spell[SpellSlot.R].Cast();
                }
            }
        }

        /// <summary>
        ///     My Names Jeff
        /// </summary>
        /// <param name="args">gay</param>
        private void OnUpdate(EventArgs args)
        {
            PortAIO.OrbwalkerManager.ForcedTarget(null);

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                OnCombo();
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                OnMixed();
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                OnLaneclear();
            }

            //BALISTA
            if (getCheckBoxItem(comboMenu, "com.ikalista.combo.balista") && SpellManager.Spell[SpellSlot.R].IsReady())
            {
                var soulboundhero = HeroManager.Allies.FirstOrDefault(x => x.HasBuff("kalistacoopstrikeally") && x.IsAlly);
                if (soulboundhero?.ChampionName == "Blitzcrank")
                {
                    foreach (var unit in HeroManager.Enemies.Where(h => h.IsHPBarRendered && h.LSDistance(ObjectManager.Player.ServerPosition) > 700 && h.LSDistance(ObjectManager.Player.ServerPosition) < 1400))
                    {
                        if (unit.HasBuff("rocketgrab2"))
                        {
                            SpellManager.Spell[SpellSlot.R].Cast();
                        }
                    }
                }
            }

            foreach (var module in Modules.Where(x => x.ShouldGetExecuted()))
            {
                module.OnExecute();
            }
        }

        private void OnCombo()
        {
            if (getCheckBoxItem(miscMenu, "com.ikalista.misc.exploit"))
            {
                var target = TargetSelector.GetTarget(
                    ObjectManager.Player.AttackRange,
                    DamageType.Physical);
                if (target.IsValidTarget(ObjectManager.Player.AttackRange))
                {
                    if (Environment.TickCount - LastAutoAttack <= 250) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    if (Environment.TickCount - LastAutoAttack >= 50)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        LastAutoAttack = Environment.TickCount;
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "com.ikalista.combo.orbwalkMinions"))
            {
                var targets =
                    HeroManager.Enemies.Where(
                        x =>
                            ObjectManager.Player.LSDistance(x) <= SpellManager.Spell[SpellSlot.E].Range * 2 &&
                            x.LSIsValidTarget(SpellManager.Spell[SpellSlot.E].Range * 2));

                if (targets.Count(x => ObjectManager.Player.LSDistance(x) < Orbwalking.GetRealAutoAttackRange(x)) == 0)
                {
                    var minion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                    ObjectManager.Player.LSDistance(x) <= Orbwalking.GetRealAutoAttackRange(x) &&
                                    x.IsEnemy)
                            .OrderBy(x => x.Health)
                            .FirstOrDefault();
                    if (minion != null)
                    {
                        PortAIO.OrbwalkerManager.ForcedTarget(minion);
                    }
                }
            }

            if (!SpellManager.Spell[SpellSlot.Q].IsReady() || !getCheckBoxItem(comboMenu, "com.ikalista.combo.useQ"))
                return;

            if (getCheckBoxItem(comboMenu, "com.ikalista.combo.saveMana") && ObjectManager.Player.Mana < SpellManager.Spell[SpellSlot.E].ManaCost * 2)
            {
                return;
            }

            var spearTarget = TargetSelector.GetTarget(
                SpellManager.Spell[SpellSlot.Q].Range,
                DamageType.Physical);
            var prediction = SpellManager.Spell[SpellSlot.Q].GetPrediction(spearTarget);
            if (prediction.Hitchance >= HitChance.High
                && spearTarget.IsValidTarget(SpellManager.Spell[SpellSlot.Q].Range) && !ObjectManager.Player.IsDashing()
                && !ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                SpellManager.Spell[SpellSlot.Q].Cast(prediction.CastPosition);
            }
        }

        private void OnMixed()
        {
            if (SpellManager.Spell[SpellSlot.Q].IsReady() && getCheckBoxItem(mixedMenu, "com.ikalista.mixed.useQ"))
            {
                var target = TargetSelector.GetTarget(SpellManager.Spell[SpellSlot.Q].Range, DamageType.Physical);
                var prediction = SpellManager.Spell[SpellSlot.Q].GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High &&
                    target.LSIsValidTarget(SpellManager.Spell[SpellSlot.Q].Range))
                {
                    SpellManager.Spell[SpellSlot.Q].Cast(target);
                }
            }

            if (SpellManager.Spell[SpellSlot.E].IsReady() && getCheckBoxItem(mixedMenu, "com.ikalista.mixed.useE"))
            {
                foreach (
                    var source in
                        HeroManager.Enemies.Where(
                            x => x.IsValid && x.HasRendBuff() && SpellManager.Spell[SpellSlot.E].IsInRange(x)))
                {
                    if (source.IsRendKillable() ||
                        source.GetRendBuffCount() >= getSliderItem(mixedMenu, "com.ikalista.mixed.stacks"))
                    {
                        SpellManager.Spell[SpellSlot.E].Cast();
                    }
                }
            }
        }

        private void OnLaneclear()
        {
            if (getCheckBoxItem(laneclearMenu, "com.ikalista.laneclear.useQ"))
            {
                var minions = MinionManager.GetMinions(SpellManager.Spell[SpellSlot.Q].Range).ToList();
                if (minions.Count < 0)
                    return;

                foreach (var minion in minions.Where(x => x.Health <= SpellManager.Spell[SpellSlot.Q].GetDamage(x)))
                {
                    var killableMinions = Helper.GetCollisionMinions(ObjectManager.Player,
                        ObjectManager.Player.ServerPosition.LSExtend(
                            minion.ServerPosition,
                            SpellManager.Spell[SpellSlot.Q].Range))
                        .Count(
                            collisionMinion =>
                                collisionMinion.Health
                                <= ObjectManager.Player.GetSpellDamage(collisionMinion, SpellSlot.Q));

                    if (killableMinions >= getSliderItem(laneclearMenu, "com.ikalista.laneclear.qMinions"))
                    {
                        SpellManager.Spell[SpellSlot.Q].Cast(minion.ServerPosition);
                    }
                }
            }
            if (getCheckBoxItem(laneclearMenu, "com.ikalista.laneclear.useE"))
            {
                var minions = MinionManager.GetMinions(SpellManager.Spell[SpellSlot.E].Range).ToList();
                if (minions.Count < 0)
                    return;

                var siegeMinion =
                    minions.FirstOrDefault(x => x.CharData.BaseSkinName == "MinionSiege" && x.IsRendKillable());

                if (getCheckBoxItem(laneclearMenu, "com.ikalista.laneclear.eSiege") && siegeMinion != null)
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }

                var count =
                    minions.Count(
                        x => SpellManager.Spell[SpellSlot.E].CanCast(x) && x.IsMobKillable());

                if (count >= getSliderItem(laneclearMenu, "com.ikalista.laneclear.eMinions") &&
                    !ObjectManager.Player.HasBuff("summonerexhaust"))
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }
            }
        }
    }
}
