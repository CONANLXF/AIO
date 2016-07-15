using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

 namespace MorganaE
{
    class Shield
    {

        private static List<AIHeroClient> priorAllyOrder { get; set; }
        private static List<AIHeroClient> hpAllyOrder { get; set; }
        private static int highestPriority { get; set; }
        private static float lowestHP { get; set; }

        public static void Initialize(Menu m)
        {
            AutoShield.Initialize();
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void CastShield(Obj_AI_Base target)
        {
            ObjectManager.Player.Spellbook.CastSpell(SpellSlot.E, target);
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || Player.Instance.IsRecalling()) return;

            if (args.Slot == SpellSlot.E && args.SData.Name.IndexOf("Condemn", StringComparison.CurrentCultureIgnoreCase) >= 0 && args.Target.Type == GameObjectType.AIHeroClient && args.Target.IsAlly && sender.BaseSkinName.Contains("Vayne"))
            {
                CastShield(args.Target as Obj_AI_Base);
            }

            if (args.Slot == SpellSlot.Q && args.SData.Name.IndexOf("Blind", StringComparison.CurrentCultureIgnoreCase) >= 0 && args.Target.Type == GameObjectType.AIHeroClient && args.Target.IsAlly && sender.BaseSkinName.Contains("Teemo"))
            {
                CastShield(args.Target as Obj_AI_Base);
            }

            priorAllyOrder = new List<AIHeroClient>();

            hpAllyOrder = new List<AIHeroClient>();

            highestPriority = 0;

            lowestHP = int.MaxValue;

            if (AutoShield.PriorMode == 1)
            {
                foreach (var slider in AutoShield.Sliders)
                {
                    if (slider.CurrentValue >= highestPriority)
                    {
                        highestPriority = slider.CurrentValue;

                        foreach (var ally in AutoShield.Heros.Where(ally => slider.VisibleName.Contains(ally.ChampionName)))
                        {
                            priorAllyOrder.Insert(0, ally);
                        }
                    }
                    else
                    {
                        foreach (var ally in AutoShield.Heros.Where(ally => slider.VisibleName.Contains(ally.ChampionName)))
                        {
                            priorAllyOrder.Add(ally);
                        }
                    }
                }

                foreach (var ally in priorAllyOrder.Where(ally => Player.Instance.IsInRange(ally, 750)))
                {
                    foreach (var shieldThisSpell in AutoShield.ShieldAllyList.Where(x => x.DisplayName.Contains(ally.ChampionName) && x.CurrentValue).SelectMany(shieldThisAlly => AutoShield.ShieldSpellList.Where(s => s.DisplayName.Contains(args.SData.Name) && s.CurrentValue)))
                    {
                        if (args.Target == ally)
                            CastShield(ally);
                        else
                        {
                            if (Prediction.Position.PredictUnitPosition(ally, 250)
                                .IsInRange(args.End,
                                    MissileDatabase.rangeRadiusDatabase[shieldThisSpell.DisplayName.Last(), 1]))
                            {
                                CastShield(ally);
                            }
                            else if (sender.IsFacing(ally) && Prediction.Position.PredictUnitPosition(ally, 250).IsInRange(sender, MissileDatabase.rangeRadiusDatabase[shieldThisSpell.DisplayName.Last(), 0]))
                            {
                                CastShield(ally);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var ally in EntityManager.Heroes.Allies)
                {
                    if (ally.Health <= lowestHP)
                    {
                        lowestHP = ally.Health;
                        hpAllyOrder.Insert(0, ally);
                    }
                    else
                        hpAllyOrder.Add(ally);
                }

                foreach (var ally in hpAllyOrder.Where(ally => Player.Instance.IsInRange(ally, 750)))
                {
                    foreach (var shieldThisSpell in AutoShield.ShieldAllyList.Where(a => a.DisplayName.Contains(ally.ChampionName) && a.CurrentValue).SelectMany(shieldThisAlly => AutoShield.ShieldSpellList.Where(s => s.DisplayName.Contains(args.SData.Name) && s.CurrentValue)))
                    {
                        if (args.Target == ally)
                            CastShield(ally);
                        else
                        {
                            if (Prediction.Position.PredictUnitPosition(ally, 250).IsInRange(args.End, MissileDatabase.rangeRadiusDatabase[shieldThisSpell.DisplayName.Last(), 1]))
                            {
                                CastShield(ally);
                            }
                            else if (sender.IsFacing(ally) && Prediction.Position.PredictUnitPosition(ally, 250).IsInRange(sender, MissileDatabase.rangeRadiusDatabase[shieldThisSpell.DisplayName.Last(), 0]))
                            {
                                CastShield(ally);
                            }
                        }
                    }
                }
            }
        }
    }

    public static class AutoShield
    {
        private static readonly ComboBox _priorMode;

        static AutoShield()
        {
            ShieldAllyList = new List<CheckBox>();
            ShieldSpellList = new List<CheckBox>();

            var m = MainMenu.AddMenu("Auto-Shield Settings", "auto-shield");
            m.AddLabel("Creds : houseparty36");
            foreach (var ally in EntityManager.Heroes.Allies)
            {
                ShieldAllyList.Add(m.Add("shield" + ally.ChampionName, new CheckBox($"Shield {ally.ChampionName} ({ally.Name})")));
            }

            m.AddSeparator(13);

            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                for (var i = 0; i <= 185; i++)
                {
                    if (MissileDatabase.missileDatabase[i, 2] == enemy.ChampionName)
                        ShieldSpellList.Add(m.Add(MissileDatabase.missileDatabase[i, 0] + i, new CheckBox($"Shield from {MissileDatabase.missileDatabase[i, 2]}'s {MissileDatabase.missileDatabase[i, 1]} ({MissileDatabase.missileDatabase[i, 0]})                                                 {i}")));
                }
            }

            m.AddSeparator(13);

            _priorMode = m.Add("autoShieldPriorMode", new ComboBox("AutoShield Priority Mode:", 0, "Lowest Health", "Priority Level"));
            m.AddSeparator(13);

            Sliders = new List<Slider>();
            Heros = new List<AIHeroClient>();

            foreach (var ally in EntityManager.Heroes.Allies)
            {
                var PrioritySlider = m.Add(ally.ChampionName, new Slider(string.Format("{0} Priority:", ally.ChampionName, ally.Name), 1, 1, EntityManager.Heroes.Allies.Count));
                m.AddSeparator(13);
                Sliders.Add(PrioritySlider);
                Heros.Add(ally);
            }
        }

        public static int PriorMode => _priorMode.SelectedIndex;

        public static List<Slider> Sliders { get; }

        public static List<AIHeroClient> Heros { get; }

        public static List<CheckBox> ShieldAllyList { get; }

        public static List<CheckBox> ShieldSpellList { get; }

        public static void Initialize()
        {
        }
    }

    public static class MissileDatabase
    {
        static MissileDatabase()
        {
            missileDatabase = new string[186, 3]
            {
                {"disabled/TestCubeRender", "Summoner", "Any"},
                {"AatroxQ", "Q", "Aatrox"},
                {"AatroxE", "E", "Aatrox"},
                {"AhriOrbMissile", "Q", "Ahri"},
                {"AhriSeduceMissile", "E", "Ahri"},
                {"AhriOrbofDeception2", "Q", "Ahri"},
                {"Pulverize", "Q", "Alistar"},
                {"CurseoftheSadMummy", "R", "Amumu"},
                {"SadMummyBandageToss", "Q", "Amumu"},
                {"FlashFrostSpell", "Q", "Anivia"},
                {"Incinerate", "W", "Annie"},
                {"InfernalGuardian", "R", "Annie"},
                {"EnchantedCrystalArrow", "R", "Ashe"},
                {"VolleyAttack", "W", "Ashe"},
                {"azirsoldiermissile", "Q", "Azir"},
                {"BardQMissile", "Q", "Bard"},
                {"RocketGrabMissile", "Q", "Blitzcrank"},
                {"BrandQMissile", "Q", "Brand"},
                {"BrandW", "W", "Brand"},
                {"braumrmissile", "R", "Braum"},
                {"BraumQMissile", "Q", "Braum"},
                {"CaitlynPiltoverPeacemaker", "Q", "Caitlyn"},
                {"CaitlynEntrapmentMissile", "E", "Caitlyn"},
                {"CassiopeiaPetrifyingGaze", "R", "Cassiopeia"},
                {"CassiopeiaQ", "Q", "Cassiopeia"},
                {"CassiopeiaMiasma", "W", "Cassiopeia"},
                {"FeralScream", "W", "Chogath"},
                {"Rupture", "Q", "Chogath"},
                {"MissileBarrageMissile2", "R", "Corki"},
                {"PhosphorusBombMissile", "Q", "Corki"},
                {"MissileBarrageMissile", "R", "Corki"},
                {"DariusAxeGrabCone", "E", "Darius"},
                {"DianaArc", "Q", "Diana"},
                {"InfectedCleaverMissile", "Q", "DrMundo"},
                {"DravenR", "R", "Draven"},
                {"DravenDoubleShotMissile", "E", "Draven"},
                {"ekkoqmis", "Q", "Ekko"},
                {"EkkoW", "W", "Ekko"},
                {"EkkoR", "R", "Ekko"},
                {"EliseHumanE", "E", "Elise"},
                {"EvelynnR", "R", "Evelynn"},
                {"EzrealMysticShotMissile", "Q", "Ezreal"},
                {"EzrealTrueshotBarrage", "R", "Ezreal"},
                {"EzrealEssenceFluxMissile", "W", "Ezreal"},
                {"FizzMarinerDoomMissile", "R", "Fizz"},
                {"GalioRighteousGust", "E", "Galio"},
                {"GalioResoluteSmite", "Q", "Galio"},
                {"GalioIdolOfDurand", "R", "Galio"},
                {"gnarbigq", "Q", "Gnar"},
                {"GnarR", "R", "Gnar"},
                {"gnarbigw", "W", "Gnar"},
                {"GnarQ", "Q", "Gnar"},
                {"GnarQMissileReturn", "Q", "Gnar"},
                {"GnarE", "E", "Gnar"},
                {"gnarbige", "E", "Gnar"},
                {"GragasQ", "Q", "Gragas"},
                {"GragasE", "E", "Gragas"},
                {"GragasR", "R", "Gragas"},
                {"GravesClusterShotAttack", "Q", "Graves"},
                {"GravesChargeShotShot", "R", "Graves"},
                {"HecarimUlt", "R", "Hecarim"},
                {"HeimerdingerWAttack2", "W", "Heimerdinger"},
                {"HeimerdingerWAttack2Ult", "W", "Heimerdinger"},
                {"HeimerdingerESpell", "E", "Heimerdinger"},
                {"heimerdingerespell_ult", "E", "Heimerdinger"},
                {"ireliatranscendentbladesspell", "R", "Irelia"},
                {"HowlingGaleSpell", "Q", "Janna"},
                {"JarvanIVDragonStrike", "Q", "JarvanIV"},
                {"JarvanIVDragonStrike2", "Q", "JarvanIV"},
                {"JarvanIVCataclysm", "R", "JarvanIV"},
                {"JayceShockBlastWallMis", "Q", "Jayce"},
                {"JayceShockBlastMis", "Q", "Jayce"},
                {"JinxR", "R", "Jinx"},
                {"JinxWMissile", "W", "Jinx"},
                {"kalistamysticshotmistrue", "Q", "Kalista"},
                {"KarmaQMissile", "Q", "Karma"},
                {"KarmaQMissileMantra", "Q", "Karma"},
                {"KarthusLayWasteA1", "Q", "Karthus"},
                {"RiftWalk", "R", "Kassadin"},
                {"ForcePulse", "E", "Kassadin"},
                {"KennenShurikenHurlMissile1", "Q", "Kennen"},
                {"KhazixWMissile", "W", "Khazix"},
                {"khazixwlong", "W", "Khazix"},
                {"KogMawQMis", "Q", "KogMaw"},
                {"KogMawVoidOozeMissile", "E", "KogMaw"},
                {"KogMawLivingArtillery", "R", "KogMaw"},
                {"LeblancSoulShackleM", "R Mimic", "Leblanc"},
                {"LeblancSoulShackle", "E", "Leblanc"},
                {"LeblancSlideM", "R Mimic", "Leblanc"},
                {"LeblancSlide", "W", "Leblanc"},
                {"BlindMonkQOne", "Q", "LeeSin"},
                {"LeonaSolarFlare", "R", "Leona"},
                {"FlashFrostSpell", "E", "Leona"},
                {"LissandraW", "W", "Lissandra"},
                {"LissandraQ", "Q", "Lissandra"},
                {"LucianW", "W", "Lucian"},
                {"LucianQ", "Q", "Lucian"},
                {"LuluQMissile", "Q", "Lulu"},
                {"LuluQMissileTwo", "Q", "Lulu"},
                {"LuxLightStrikeKugel", "E", "Lux"},
                {"LuxMaliceCannon", "R", "Lux"},
                {"LuxLightBindingMis", "Q", "Lux"},
                {"UFSlash", "R", "Malphite"},
                {"AlZaharCalloftheVoidMissile", "Q", "Malzahar"},
                {"MonkeyKingSpinToWin", "R", "MonkeyKing"},
                {"DarkBindingMissile", "Q", "Morgana"},
                {"NamiQ", "Q", "Nami"},
                {"NamiRMissile", "R", "Nami"},
                {"NautilusAnchorDragMissile", "Q", "Nautilus"},
                {"JavelinToss", "Q", "Nidalee"},
                {"NocturneDuskbringer", "Q", "Nocturne"},
                {"OlafAxeThrowCast", "Q", "Olaf"},
                {"OrianaIzunaCommand", "Q", "Orianna"},
                {"OrianaDetonateCommand", "R", "Orianna"},
                {"OrianaDissonanceCommand", "W", "Orianna"},
                {"PantheonE", "E", "Pantheon"},
                {"QuinnQMissile", "Q", "Quinn"},
                {"RekSaiQBurrowedMis", "E", "RekSai"},
                {"RengarEFinal", "E", "Rengar"},
                {"rivenizunablade", "R", "Riven"},
                {"RivenMartyr", "W", "Riven"},
                {"RumbleGrenade", "E", "Rumble"},
                {"RyzeQ", "Q", "Ryze"},
                {"SejuaniArcticAssault", "Q", "Sejuani"},
                {"SejuaniGlacialPrison", "R", "Sejuani"},
                {"ShenShadowDash", "E", "Shen"},
                {"ShyvanaFireball", "E", "Shyvana"},
                {"ShyvanaTransformCast", "R", "Shyvana"},
                {"SionEMissile", "E", "Sion"},
                {"SivirQMissile", "Q", "Sivir"},
                {"SivirQMissileReturn", "Q", "Sivir"},
                {"SkarnerFractureMissile", "E", "Skarner"},
                {"SonaR", "R", "Sona"},
                {"SorakaQ", "Q", "Soraka"},
                {"SorakaE", "E", "Soraka"},
                {"SwainShadowGrasp", "W", "Swain"},
                {"SyndraE", "E", "Syndra"},
                {"syndrawcast", "W", "Syndra"},
                {"SyndraQ", "Q", "Syndra"},
                {"tahmkenchqmissile", "Q", "TahmKench"},
                {"TalonRake", "W", "Talon"},
                {"ThreshQMissile", "Q", "Thresh"},
                {"ThreshEMissile1", "E", "Thresh"},
                {"SealFateMissile", "Q", "TwistedFate"},
                {"UrgotHeatseekingLineMissile", "Q", "Urgot"},
                {"UrgotPlasmaGrenadeBoom", "E", "Urgot"},
                {"VarusE", "E", "Varus"},
                {"VarusQMissile", "Q", "Varus"},
                {"VarusRMissile", "R", "Varus"},
                {"VeigarBalefulStrikeMis", "Q", "Veigar"},
                {"VeigarDarkMatter", "W", "Veigar"},
                {"VeigarEventHorizon", "E", "Veigar"},
                {"VelkozEMissile", "E", "Velkoz"},
                {"VelkozW", "W", "Velkoz"},
                {"VelkozQMissileSplit", "Q Splited", "Velkoz"},
                {"VelkozQMissile", "Q", "Velkoz"},
                {"ViQMissile", "Q", "Vi"},
                {"ViktorDeathRayMissile", "E1", "Viktor"},
                {"ViktorDeathRay3", "E3", "Viktor"},
                {"ViktorDeathRayMissile2", "E2", "Viktor"},
                {"ViktorGravitonField", "W", "Viktor"},
                {"VladimirHemoplague", "R", "Vladimir"},
                {"XerathArcaneBarrage2", "W", "Xerath"},
                {"xeratharcanopulse2", "Q", "Xerath"},
                {"xerathrmissilewrapper", "R", "Xerath"},
                {"XerathMageSpearMissile", "E", "Xerath"},
                {"YasuoQ3Mis", "Q Tornado", "Yasuo"},
                //{ "YasuoQ3Mis/disabled", "Q", "Yasuo" },
                {"yasuoq", "Q1", "Yasuo"},
                {"yasuoq2", "Q2", "Yasuo"},
                {"ZedQMissile", "Q", "Zed"},
                {"ZedPBAOEDummy", "E", "Zed"},
                {"ZiggsE", "E", "Ziggs"},
                {"ZiggsW", "W", "Ziggs"},
                {"ZiggsQSpell", "Q", "Ziggs"},
                {"ZiggsR", "R", "Ziggs"},
                {"ZileanQ", "Q", "Zilean"},
                {"ZyraEMissile", "E", "Zyra"},
                {"zyrapassivedeathmanager", "Passive", "Zyra"},
                {"ZyraQFissure", "Q", "Zyra"},
                {"ZyraBrambleZone", "R", "Zyra"},
                {"illaoiemis", "Q", "Illaoi"},
                {"illaoiemis", "E", "Illaoi"},
                {"GravesQLineMis", "Q", "Graves"},
                {"PoppyQ", "Q", "Poppy"},
                {"JhinWMissile", "W", "Jhin"},
                {"JhinRShotMis", "R", "Jhin"}
            };

            rangeRadiusDatabase = new int[186, 2]
            {
                {1600, 60},
                {650, 285},
                {1075, 100},
                {925, 100},
                {1000, 60},
                {925, 100},
                {365, 365},
                {560, 560},
                {1100, 80},
                {1250, 110},
                {625, 80}, // 10
                {600, 290},
                {12500, 130},
                {1150, 20},
                {850, 80},
                {950, 60},
                {1050, 70},
                {1100, 60},
                {1100, 250},
                {1250, 100},
                {1000, 100}, // 20
                {1300, 90},
                {950, 80},
                {825, 20},
                {600, 200},
                {850, 220},
                {650, 20},
                {950, 250},
                {1500, 40},
                {825, 270},
                {1300, 40}, // 30
                {570, 20},
                {850, 50},
                {1050, 60},
                {12500, 160},
                {1100, 130},
                {950, 60},
                {1600, 375},
                {1600, 375},
                {1100, 70},
                {650, 350}, // 40
                {1200, 60},
                {20000, 160},
                {1050, 80},
                {1275, 120},
                {1280, 120},
                {1040, 235},
                {600, 600},
                {1150, 90},
                {500, 500},
                {600, 100}, // 50
                {1185, 60},
                {1185, 60},
                {475, 150},
                {475, 100},
                {975, 250},
                {950, 200},
                {1050, 350},
                {1025, 60},
                {1000, 100},
                {1500, 300}, // 60
                {1500, 70},
                {1500, 70},
                {925, 135},
                {925, 135},
                {1200, 65},
                {1700, 120},
                {845, 80},
                {845, 120},
                {825, 350},
                {1170, 70}, // 70
                {1050, 70},
                {25000, 120},
                {1500, 60},
                {1200, 70},
                {1050, 90},
                {1050, 90},
                {875, 190},
                {700, 270},
                {700, 20},
                {1175, 50}, // 80
                {1100, 70},
                {1025, 70},
                {1125, 70},
                {1360, 120},
                {2200, 235},
                {960, 70},
                {960, 70},
                {725, 250},
                {1100, 60},
                {1200, 250}, // 90
                {975, 70},
                {725, 450},
                {825, 75},
                {1000, 80},
                {1140, 65},
                {925, 80},
                {925, 80},
                {1100, 340},
                {3500, 110},
                {1300, 70}, // 100
                {1000, 270},
                {900, 85},
                {300, 225},
                {1300, 80},
                {875, 200},
                {2750, 250},
                {1080, 90},
                {1500, 40},
                {1125, 60},
                {1000, 90}, // 110
                {2000, 80},
                {410, 410},
                {1825, 250},
                {650, 100},
                {1050, 80},
                {1500, 65},
                {1000, 70},
                {1100, 100},
                {650, 280},
                {950, 90}, // 120
                {900, 60},
                {900, 70},
                {1200, 110},
                {1600, 75},
                {950, 60},
                {1000, 160},
                {800, 80},
                {1275, 100},
                {1275, 100},
                {1000, 60}, // 130
                {1000, 150},
                {970, 260},
                {925, 275},
                {900, 200},
                {800, 140},
                {925, 220},
                {800, 210},
                {951, 90},
                {780, 75},
                {1200, 70}, // 140
                {1075, 110},
                {1450, 40},
                {1000, 60},
                {900, 250},
                {925, 235},
                {1600, 75},
                {1200, 100},
                {950, 70},
                {900, 225},
                {700, 425}, // 150
                {950, 225},
                {1100, 90},
                {900, 90},
                {1200, 90},
                {725, 90},
                {800, 80},
                {800, 80},
                {800, 80},
                {625, 300},
                {700, 375}, // 160
                {1100, 270},
                {1525, 80},
                {5600, 200},
                {1125, 60},
                {1150, 90},
                {1150, 90},
                //{ 1150, 90 },
                {550, 40},
                {550, 40},
                {925, 50},
                {290, 290}, // 170
                {2000, 235},
                {2000, 275},
                {850, 150},
                {5300, 550},
                {900, 250},
                {1150, 70},
                {1474, 80},
                {825, 260},
                {700, 525},
                {850, 100}, // 180
                {950, 50},
                {808, 40},
                {430, 100},
                {3000, 40},
                {3500, 80}
            };
        }

        public static string[,] missileDatabase { get; private set; }
        public static int[,] rangeRadiusDatabase { get; private set; }
    }
}
