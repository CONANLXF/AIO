using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

using TargetSelector = PortAIO.TSManager; namespace D_Nidalee
{
    internal class Program
    {
        private const string ChampionName = "Nidalee";

        private static Spell Q, W, E, R, QC, WC, EC;

        private static readonly List<Spell> SpellList = new List<Spell>();

        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis, _zhonya, _archangel;

        private static SpellSlot IgniteSlot;

        private static Menu Config, comboMenu, Heal, items, harassMenu, laneMenu, jungleMenu, Misc, Drawings, smiteMenu;

        private static AIHeroClient Player;

        private static bool IsHuman;

        private static bool IsCougar;

        private static readonly float[] HumanQcd = { 6, 6, 6, 6, 6 };

        private static readonly float[] HumanWcd = { 12, 12, 12, 12, 12 };

        private static readonly float[] HumanEcd = { 13, 12, 11, 10, 9 };

        private static readonly float[] CougarQcd = { 5, 5, 5, 5, 5 };

        private static readonly float[] CougarWcd = { 5, 5, 5, 5, 5 };

        private static readonly float[] CougarEcd = { 5, 5, 5, 5, 5 };

        private static float _humQcd, _humWcd, _humEcd;

        private static float _spidQcd, _spidWcd, _spidEcd;

        private static float _humaQcd, _humaWcd, _humaEcd;

        private static float _spideQcd, _spideWcd, _spideEcd;

        private static SpellSlot _smiteSlot;

        private static Spell _smite;


        public static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (ObjectManager.Player.ChampionName != ChampionName) return;



            Q = new Spell(SpellSlot.Q, 1500f);
            W = new Spell(SpellSlot.W, 875f);
            E = new Spell(SpellSlot.E, 600f);
            QC = new Spell(SpellSlot.Q, 400f);
            WC = new Spell(SpellSlot.W, 375f);
            EC = new Spell(SpellSlot.E, 300f);
            R = new Spell(SpellSlot.R, 0);

            Q.SetSkillshot(0.25f, 40f, 1300, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.500f, 90f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            WC.SetSkillshot(0.50f, 400f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            EC.SetSkillshot(0.50f, (float)(15 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
            SpellList.Add(QC);
            SpellList.Add(WC);
            SpellList.Add(EC);

            _archangel = Utility.Map.GetMap().Type == Utility.Map.MapType.TwistedTreeline
                         || Utility.Map.GetMap().Type == Utility.Map.MapType.CrystalScar
                             ? new Items.Item(3048, float.MaxValue)
                             : new Items.Item(3040, float.MaxValue);

            if (Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite"))
            {
                _smite = new Spell(SpellSlot.Summoner1, 570f);
                _smiteSlot = SpellSlot.Summoner1;
            }
            else if (Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
            {
                _smite = new Spell(SpellSlot.Summoner2, 570f);
                _smiteSlot = SpellSlot.Summoner2;
            }

            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);
            _zhonya = new Items.Item(3157, float.MaxValue);


            IgniteSlot = Player.GetSpellSlot("SummonerDot");

            //D Nidalee;
            Config = MainMenu.AddMenu("D-Nidalee", "D-Nidalee");

            //Combo
            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseItemsignite", new CheckBox("Use Ignite"));
            comboMenu.Add("smitecombo", new CheckBox("Use Smite"));
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseRCombo", new CheckBox("Use R"));
            comboMenu.Add("UseQComboCougar", new CheckBox("Use Q Cougar"));
            comboMenu.Add("UseWComboCougar", new CheckBox("Use W Cougar"));
            comboMenu.Add("UseEComboCougar", new CheckBox("Use E Cougar"));
            comboMenu.Add("QHitCombo", new ComboBox("Q HitChange", 3, "Low", "Medium", "High", "Very High"));

            //Extra
            Heal = Config.AddSubMenu("Heal", "Heal");
            Heal.Add("MPPercent", new Slider("Mana percent", 400, 1, 100));
            Heal.Add("AutoSwitchform", new CheckBox("Auto Switch Forms"));
            Heal.Add("UseAutoE", new CheckBox("Use Heal(E)"));
            Heal.Add("HPercent", new Slider("Health percent", 400, 1, 100));
            Heal.Add("AllyUseAutoE", new CheckBox("Ally Use Heal(E)"));
            Heal.Add("AllyHPercent", new Slider("Ally Health percent", 400, 1, 100));

            items = Config.AddSubMenu("items", "items");
            items.AddLabel("Offensive");
            items.Add("Tiamat", new CheckBox("Use Tiamat"));
            items.Add("Hydra", new CheckBox("Use Hydra"));
            items.Add("Bilge", new CheckBox("Use Bilge"));
            items.Add("BilgeEnemyhp", new Slider("If Enemy Hp <", 85, 1, 100));
            items.Add("Bilgemyhp", new Slider("Or your Hp <", 85, 1, 100));
            items.Add("Blade", new CheckBox("Use Blade"));
            items.Add("BladeEnemyhp", new Slider("If Enemy Hp <", 85, 1, 100));
            items.Add("Blademyhp", new Slider("Or your Hp <", 85, 1, 100));
            items.AddLabel("Deffensive");
            items.Add("Omen", new CheckBox("Use Randuin Omen"));
            items.Add("Omenenemys", new Slider("Randuin if enemys>", 2, 1, 5));
            items.Add("lotis", new CheckBox("Use Iron Solari"));
            items.Add("lotisminhp", new Slider("Solari if Ally Hp<", 35, 1, 100));
            items.Add("Righteous", new CheckBox("Use Righteous Glory"));
            items.Add("Righteousenemys", new Slider("Righteous Glory if  Enemy >=", 2, 1, 5));
            items.Add("Righteousenemysrange", new Slider("Righteous Glory Range Check", 800, 400, 1400));
            items.Add("Zhonyas", new CheckBox("Use Zhonyas"));
            items.Add("Zhonyashp", new Slider("Use Zhonya's if HP%<", 20, 1, 100));
            items.Add("Archangel", new CheckBox("Seraph's Embrace"));
            items.Add("Archangelmyhp", new Slider("If My HP% <", 85, 1, 100));

            items.AddLabel("Potions");
            items.Add("usehppotions", new CheckBox("Use Healt potion/Refillable/Hunters/Corrupting/Biscuit"));
            items.Add("usepotionhp", new Slider("If Health % <", 35, 1, 100));
            items.Add("usemppotions", new CheckBox("Use Hunters/Corrupting/Biscuit"));
            items.Add("usepotionmp", new Slider("If Mana % <", 0, 1, 100));

            //Harass
            harassMenu = Config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q"));
            harassMenu.Add("UseWHarass", new CheckBox("Use W"));
            harassMenu.Add("QHitharass", new ComboBox("Q HitChange", 3, "Low", "Medium", "High", "Very High"));
            harassMenu.Add("harasstoggle", new KeyBind("AutoHarass (toggle)", false, KeyBind.BindTypes.PressToggle, 'G'));
            harassMenu.Add("Harrasmana", new Slider("Minimum Mana", 60, 1, 100));
            harassMenu.Add("Harass", new KeyBind("Harass key)", false, KeyBind.BindTypes.PressToggle, 'C'));

            laneMenu = Config.AddSubMenu("Lane Clear", "Lane Clear");
            laneMenu.AddLabel("LastHit");
            laneMenu.Add("UseQLH", new CheckBox("Use Q (Human)"));
            laneMenu.Add("lastmana", new Slider("Minimum Mana% >", 35, 1, 100));
            laneMenu.AddLabel("LaneClear");
            laneMenu.Add("UseQLane", new CheckBox("Use Q (Human)"));
            laneMenu.Add("UseWLane", new CheckBox("Use W (Human)"));
            laneMenu.Add("farm_E1", new CheckBox("Use E (Human)"));
            laneMenu.Add("UseQCLane", new CheckBox("Use Q (Cougar)"));
            laneMenu.Add("UseWCLane", new CheckBox("Use W (Cougar)"));
            laneMenu.Add("UseECLane", new CheckBox("Use E (Cougar)"));
            laneMenu.Add("Lane", new Slider("Minimum Mana", 60, 1, 100));
            laneMenu.Add("farm_R", new KeyBind("Auto Switch Forms(toggle))", false, KeyBind.BindTypes.PressToggle, 'C'));
            jungleMenu = Config.AddSubMenu("Jungle Clear", "Jungle Clear");
            jungleMenu.AddLabel("Jungle");
            jungleMenu.Add("UseQJungle", new CheckBox("Use Q (Human)"));
            jungleMenu.Add("UseWJungle", new CheckBox("Use W (Human)"));
            jungleMenu.Add("UseQCJungle", new CheckBox("Use Q (Cougar)"));
            jungleMenu.Add("UseWCJungle", new CheckBox("Use W (Cougar)"));
            jungleMenu.Add("UseECJungle", new CheckBox("Use E (Cougar)"));
            jungleMenu.Add("Switchungle", new CheckBox("Switch Forms"));
            jungleMenu.Add("junglemana", new Slider("Minimum Mana", 60, 1, 100));
            //Smite 
            smiteMenu = Config.AddSubMenu("Smite", "Smite");
            smiteMenu.Add("Usesmite", new KeyBind("Use Smite(toggle))", false, KeyBind.BindTypes.PressToggle, 'H'));
            smiteMenu.Add("Useblue", new CheckBox("Smite Blue Early"));
            smiteMenu.Add("manaJ", new Slider("Smite Blue Early if MP% <", 35, 1, 100));
            smiteMenu.Add("Usered", new CheckBox("Smite Red Early"));
            smiteMenu.Add("healthJ", new Slider("Smite Red Early if MP% <", 35, 1, 100));

            //Kill Steal
            Misc = Config.AddSubMenu("Misc", "Misc");
            Misc.Add("ActiveKs", new CheckBox("Use KillSteal"));
            Misc.Add("UseQKs", new CheckBox("Use Q"));
            Misc.Add("UseIgnite", new CheckBox("Use Ignite"));
            /*
                        //Damage after combo:
                        var dmgAfterComboItem = smiteMenu.Add("DamageAfterCombo", new CheckBox("Draw damage after combo"));
                        Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
                        Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
                        dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                            {
                                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                            };
            */

            //Drawings
            Drawings = Config.AddSubMenu("Drawings", "Drawings");
            Drawings.Add("DrawQ", new CheckBox("Draw Q(human)"));
            Drawings.Add("DrawW", new CheckBox("Draw W(human)"));
            Drawings.Add("DrawE", new CheckBox("Draw E(human)"));
            Drawings.Add("DrawWC", new CheckBox("Draw W(Cougar)"));
            Drawings.Add("DamageAfterCombo", new CheckBox("Draw damage after combo"));
            Drawings.Add("Drawsmite", new CheckBox("Draw Smite"));
            Drawings.Add("Drawharass", new CheckBox("Draw Auto Harass"));
            Drawings.Add("DrawCooldown", new CheckBox("Draw Cooldown"));


            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += OnCreateObj;
            GameObject.OnDelete += OnDeleteObj;
            //Game_OnGameEnd += Game_OnGameEnd;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
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
        
        private static void Game_OnGameUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target.LSIsValidTarget(Q.Range)) WC.Range = target.HasBuff("nidaleepassivehunted") ? 730 : 375;
            if (getCheckBoxItem(Heal, "UseAutoE"))
            {
                AutoE();
            }

            if (PortAIO.OrbwalkerManager.isFleeActive)
            {
                Escapeterino();
            }

            if (getCheckBoxItem(Heal, "AllyUseAutoE"))
            {
                AllyAutoE();
            }

            Cooldowns();

            Player = ObjectManager.Player;
            QC = new Spell(SpellSlot.Q, Player.AttackRange + 50);
            PortAIO.OrbwalkerManager.SetAttack(true);
            //            OrbwalkerLS.SetAttack(true);

            CheckSpells();
            if (PortAIO.OrbwalkerManager.isLastHitActive &&
                (100 * (Player.Mana / Player.MaxMana)) > getSliderItem(laneMenu, "lastmana"))
            {
                LastHit();
            }

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo();
            }

            if ((PortAIO.OrbwalkerManager.isHarassActive ||
                getKeyBindItem(harassMenu, "harasstoggle") &&
                (100 * (Player.Mana / Player.MaxMana)) > getSliderItem(harassMenu, "Harrasmana")))
            {
                Harass();
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Farm();
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Jungleclear();
            }

            if (getCheckBoxItem(Misc, "ActiveKs"))
            {
                KillSteal();
            }

            if (getKeyBindItem(smiteMenu, "Usesmite"))
            {
                Smiteuse();
            }

            Usepotion();
        }


        private static void Escapeterino()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (IsHuman)
            {
                if (R.IsReady())
                {
                    R.Cast();
                }
            }

            if (IsCougar && WC.IsReady())
                WC.Cast(Game.CursorPos);
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
            {
                return;
            }

            if (spell.Name.ToLower().Contains("takedown"))
            {
                // Chat.Print("reset");
                Utility.DelayAction.Add(450, PortAIO.OrbwalkerManager.ResetAutoAttackTimer);
            }

            /* if (sender.IsMe)
             {
                  Chat.Print("Spell name: " + args.SData.Name.ToString());
             }*/
            if (sender.IsMe && getCheckBoxItem(Drawings, "DrawCooldown"))
            {
                //Chat.Print("Spell name: " + args.SData.Name.ToString());
                if (IsHuman)
                {
                    if (args.SData.Name == "JavelinToss") _humQcd = Game.Time + CalculateCd(HumanQcd[Q.Level - 1]);
                    if (args.SData.Name == "Bushwhack") _humWcd = Game.Time + CalculateCd(HumanWcd[W.Level - 1]);
                    if (args.SData.Name == "PrimalSurge") _humEcd = Game.Time + CalculateCd(HumanEcd[E.Level - 1]);
                }
                else
                {
                    if (args.SData.Name == "Takedown") _spidQcd = Game.Time + CalculateCd(CougarQcd[QC.Level - 1]);
                    if (args.SData.Name == "Pounce") _spidWcd = Game.Time + CalculateCd(CougarWcd[WC.Level - 1]);
                    if (args.SData.Name == "Swipe") _spidEcd = Game.Time + CalculateCd(CougarEcd[EC.Level - 1]);
                }
            }
        }

        private static float CalculateCd(float time)
        {
            return time + (time * Player.PercentCooldownMod);
        }

        private static void Cooldowns()
        {
            _humaQcd = ((_humQcd - Game.Time) > 0) ? (_humQcd - Game.Time) : 0;
            _humaWcd = ((_humWcd - Game.Time) > 0) ? (_humWcd - Game.Time) : 0;
            _humaEcd = ((_humEcd - Game.Time) > 0) ? (_humEcd - Game.Time) : 0;
            _spideQcd = ((_spidQcd - Game.Time) > 0) ? (_spidQcd - Game.Time) : 0;
            _spideWcd = ((_spidWcd - Game.Time) > 0) ? (_spidWcd - Game.Time) : 0;
            _spideEcd = ((_spidEcd - Game.Time) > 0) ? (_spidEcd - Game.Time) : 0;
        }


        private static HitChance QHitChanceCombo()
        {
            switch (getBoxItem(comboMenu, "QHitCombo"))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        private static HitChance QHitChanceHarass()
        {
            switch (getBoxItem(harassMenu, "QHitharass"))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }

        private static void Smiteontarget()
        {
            if (_smite == null) return;
            var hero = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(570));
            var smiteDmg = Player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Smite);
            var usesmite = getCheckBoxItem(comboMenu, "smitecombo");
            if (Player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker" && usesmite &&
                ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready)
            {
                if (hero != null && (!hero.HasBuffOfType(BuffType.Stun) || !hero.HasBuffOfType(BuffType.Slow)))
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
                else if (hero != null && smiteDmg >= hero.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
            }

            if (Player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel" && usesmite &&
                ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready &&
                hero.LSIsValidTarget(570))
            {
                ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
            }
        }

        public static readonly string[] Smitetype =
        {
            "s5_summonersmiteplayerganker", "s5_summonersmiteduel", "s5_summonersmitequick", "itemsmiteaoe",
            "summonersmite"
        };

        private static int GetSmiteDmg()
        {
            int level = Player.Level;
            int index = Player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }

        //New map Monsters Name By SKO
        private static void Smiteuse()
        {
            var jungle = PortAIO.OrbwalkerManager.isLaneClearActive;
            if (ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) != SpellState.Ready) return;
            var useblue = getCheckBoxItem(smiteMenu, "Useblue");
            var usered = getCheckBoxItem(smiteMenu, "Usered");
            var health = (100 * (Player.Health / Player.MaxHealth)) < getSliderItem(smiteMenu, "healthJ");
            var mana = (100 * (Player.Mana / Player.MaxMana)) < getSliderItem(smiteMenu, "manaJ");
            string[] jungleMinions;
            if (Utility.Map.GetMap().Type == Utility.Map.MapType.TwistedTreeline)
            {
                jungleMinions = new string[] { "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
            }
            else
            {
                jungleMinions = new string[]
                                    {
                                        "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_RiftHerald",
                                        "SRU_Red", "SRU_Krug", "SRU_Dragon_Air", "SRU_Dragon_Water", "SRU_Dragon_Fire",
                                        "SRU_Dragon_Elder", "SRU_Baron"
                                    };
            }

            var minions = MinionManager.GetMinions(Player.Position, 1000, MinionTypes.All, MinionTeam.Neutral);
            if (minions.Count() > 0)
            {
                int smiteDmg = GetSmiteDmg();

                foreach (Obj_AI_Base minion in minions)
                {
                    if (Utility.Map.GetMap().Type == Utility.Map.MapType.TwistedTreeline &&
                        minion.Health <= smiteDmg &&
                        jungleMinions.Any(name => minion.Name.Substring(0, minion.Name.Length - 5).Equals(name)))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }

                    if (minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name)) &&
                        !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                    else if (jungle && useblue && mana && minion.Health >= smiteDmg &&
                             jungleMinions.Any(name => minion.Name.StartsWith("SRU_Blue")) &&
                             !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                    else if (jungle && usered && health && minion.Health >= smiteDmg &&
                             jungleMinions.Any(name => minion.Name.StartsWith("SRU_Red")) &&
                             !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                }
            }
        }


        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var itemsIgnite = getCheckBoxItem(comboMenu, "UseItemsignite");
            if (target == null) return;
            Smiteontarget();

            if (itemsIgnite && IgniteSlot != SpellSlot.Unknown &&
                Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(target) > target.Health)
                {
                    Player.Spellbook.CastSpell(IgniteSlot, target);
                }
            }

            if (Q.IsReady() && IsHuman && target.LSIsValidTarget(Q.Range) && getCheckBoxItem(comboMenu, "UseQCombo"))
            {
                var predictionq = Q.GetPrediction(target);
                if (predictionq.Hitchance >= QHitChanceCombo() && predictionq.CollisionObjects.Count == 0)
                    Q.Cast(predictionq.CastPosition);
            }

            if (W.IsReady() && IsHuman && target.LSIsValidTarget(W.Range) && getCheckBoxItem(comboMenu, "UseWCombo"))
            {
                W.Cast(target);
            }

            if (R.IsReady() && IsHuman && getCheckBoxItem(comboMenu, "UseRCombo"))
                if (Player.Distance(target) <= 325 || (target.HasBuff("nidaleepassivehunted") && target.LSIsValidTarget(WC.Range)))
                {
                    if (IsHuman)
                    {
                        R.Cast();
                    }

                    if (IsCougar)
                    {
                        if (WC.IsReady() && getCheckBoxItem(comboMenu, "UseWComboCougar") &&
                            target.LSIsValidTarget(WC.Range))
                        {
                            WC.Cast(target.ServerPosition);
                        }

                        if (EC.IsReady() && getCheckBoxItem(comboMenu, "UseEComboCougar") &&
                            target.LSIsValidTarget(EC.Range))
                        {
                            EC.Cast(target.ServerPosition);
                        }

                        if (QC.IsReady() && getCheckBoxItem(comboMenu, "UseQComboCougar") &&
                            target.LSIsValidTarget(QC.Range))
                        {
                            PortAIO.OrbwalkerManager.SetAttack(true);
                            //                        OrbwalkerLS.SetAttack(true);
                            QC.Cast();
                        }
                    }
                }

            if (IsCougar && Player.Distance(target) < 700)
            {
                if (IsHuman && R.IsReady())
                {
                    R.Cast();
                }

                if (IsCougar)
                {
                    if (WC.IsReady() && getCheckBoxItem(comboMenu, "UseWComboCougar") &&
                        target.LSIsValidTarget(WC.Range))
                    {
                        WC.Cast(target.ServerPosition);
                    }

                    if (EC.IsReady() && getCheckBoxItem(comboMenu, "UseEComboCougar") &&
                       target.LSIsValidTarget(EC.Range))
                    {
                        EC.Cast(target.ServerPosition);
                    }

                    if (QC.IsReady() && getCheckBoxItem(comboMenu, "UseQComboCougar") &&
                        target.LSIsValidTarget(QC.Range))
                    {
                        //                       OrbwalkerLS.SetAttack(true);
                        PortAIO.OrbwalkerManager.SetAttack(true);
                        QC.Cast();
                    }
                }
            }

            if (R.IsReady() && IsCougar && getCheckBoxItem(comboMenu, "UseRCombo") &&
                Player.LSDistance(target) > WC.Range)
            {
                R.Cast();
            }

            if (R.IsReady() && IsCougar && !QC.IsReady() && !WC.IsReady() && !EC.IsReady() && Q.IsReady()
                && getCheckBoxItem(comboMenu, "UseRCombo"))
            {
                R.Cast();
            }

            UseItemes();
        }

        private static float ComboDamage(Obj_AI_Base hero)
        {
            var dmg = 0d;
            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += Player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            }

            if (Items.HasItem(3153) && Items.CanUseItem(3153)) dmg += Player.GetItemDamage(hero, LeagueSharp.Common.Damage.DamageItems.Botrk);
            if (Items.HasItem(3144) && Items.CanUseItem(3144)) dmg += Player.GetItemDamage(hero, LeagueSharp.Common.Damage.DamageItems.Bilgewater);
            if (IsCougar)
            {
                if (QC.IsReady()) dmg += Player.GetSpellDamage(hero, SpellSlot.Q);
                if (EC.IsReady()) dmg += Player.GetSpellDamage(hero, SpellSlot.E);
                if (WC.IsReady()) dmg += Player.GetSpellDamage(hero, SpellSlot.W);
            }

            if (Q.IsReady() && !IsCougar)
                dmg += Player.GetSpellDamage(hero, SpellSlot.Q);
            return (float)dmg;
        }



        private static void UseItemes()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var iBilge = getCheckBoxItem(items, "Bilge");
                var iBilgeEnemyhp = hero.Health <=
                                    (hero.MaxHealth * (getSliderItem(items, "BilgeEnemyhp")) / 100);
                var iBilgemyhp = Player.Health <=
                                 (Player.MaxHealth * (getSliderItem(items, "Bilgemyhp")) / 100);
                var iBlade = getCheckBoxItem(items, "Blade");
                var iBladeEnemyhp = hero.Health <=
                                    (hero.MaxHealth * (getSliderItem(items, "BladeEnemyhp")) / 100);
                var iBlademyhp = Player.Health <=
                                 (Player.MaxHealth * (getSliderItem(items, "Blademyhp")) / 100);
                var iOmen = getCheckBoxItem(items, "Omen");
                var iOmenenemys = hero.CountEnemiesInRange(450) >= getSliderItem(items, "Omenenemys");
                var iTiamat = getCheckBoxItem(items, "Tiamat");
                var iHydra = getCheckBoxItem(items, "Hydra");
                var iRighteous = getCheckBoxItem(items, "Righteous");
                var iRighteousenemys =
                    hero.CountEnemiesInRange(getSliderItem(items, "Righteousenemysrange")) >=
                    getSliderItem(items, "Righteousenemys");
                var iZhonyas = getCheckBoxItem(items, "Zhonyas");
                var iZhonyashp = Player.Health <=
                                 (Player.MaxHealth * (getSliderItem(items, "Zhonyashp")) / 100);
                var iArchange = getCheckBoxItem(items, "Archangel");
                var iArchangelmyhp = Player.Health <=
                                     (Player.MaxHealth * (getSliderItem(items, "Archangelmyhp")) / 100);

                if (hero.LSIsValidTarget(450) && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady())
                {
                    _bilge.Cast(hero);
                }

                if (hero.LSIsValidTarget(450) && iBlade && (iBladeEnemyhp || iBlademyhp) && _blade.IsReady())
                {
                    _blade.Cast(hero);
                }

                if (iTiamat && _tiamat.IsReady() && hero.LSIsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (iHydra && _hydra.IsReady() && hero.LSIsValidTarget(_hydra.Range))
                {
                    _hydra.Cast();
                }

                if (iOmenenemys && iOmen && _rand.IsReady() && hero.LSIsValidTarget(450))
                {
                    Utility.DelayAction.Add(100, () => _rand.Cast());
                }

                if (iRighteousenemys && iRighteous && Items.HasItem(3800) && Items.CanUseItem(3800) &&
                    hero.LSIsValidTarget(getSliderItem(items, "Righteousenemysrange")))
                {
                    Items.UseItem(3800);
                }

                if (iZhonyas && iZhonyashp && hero.CountEnemiesInRange(1000) > 0)
                {
                    _zhonya.Cast();
                }

                if (iArchange && iArchangelmyhp && _archangel.IsReady() && Utility.LSCountEnemiesInRange(800) > 0)
                {
                    _archangel.Cast();
                }
            }

            var ilotis = getCheckBoxItem(items, "lotis");
            if (ilotis)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly || hero.IsMe))
                {
                    if (hero.Health <= (hero.MaxHealth * (getSliderItem(items, "lotisminhp")) / 100) &&
                        hero.Distance(Player.ServerPosition) <= _lotis.Range && _lotis.IsReady())
                        _lotis.Cast();
                }
            }
        }

        private static void Usepotion()
        {
            var mobs = MinionManager.GetMinions(
                Player.ServerPosition,
                Q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            var iusehppotion = getCheckBoxItem(items, "usehppotions");
            var iusepotionhp = Player.Health
                               <= (Player.MaxHealth * (getSliderItem(items, "usepotionhp")) / 100);
            var iusemppotion = getCheckBoxItem(items, "usemppotions");
            var iusepotionmp = Player.Mana
                               <= (Player.MaxMana * (getSliderItem(items, "usepotionmp")) / 100);
            if (Player.InFountain() || ObjectManager.Player.HasBuff("Recall")) return;

            if (Utility.LSCountEnemiesInRange(800) > 0
                || (mobs.Count > 0 && PortAIO.OrbwalkerManager.isLaneClearActive && _smite != null))
            {
                if (iusepotionhp && iusehppotion
                    && !(ObjectManager.Player.HasBuff("RegenerationPotion")
                         || ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlask")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
                         || ObjectManager.Player.HasBuff("ItemDarkCrystalFlask")))
                {
                    if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }

                    if (Items.HasItem(2003) && Items.CanUseItem(2003))
                    {
                        Items.UseItem(2003);
                    }

                    if (Items.HasItem(2031) && Items.CanUseItem(2031))
                    {
                        Items.UseItem(2031);
                    }

                    if (Items.HasItem(2032) && Items.CanUseItem(2032))
                    {
                        Items.UseItem(2032);
                    }

                    if (Items.HasItem(2033) && Items.CanUseItem(2033))
                    {
                        Items.UseItem(2033);
                    }
                }

                if (iusepotionmp && iusemppotion
                    && !(ObjectManager.Player.HasBuff("ItemDarkCrystalFlask")
                         || ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Items.HasItem(2041) && Items.CanUseItem(2041))
                    {
                        Items.UseItem(2041);
                    }

                    if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }

                    if (Items.HasItem(2032) && Items.CanUseItem(2032))
                    {
                        Items.UseItem(2032);
                    }

                    if (Items.HasItem(2033) && Items.CanUseItem(2033))
                    {
                        Items.UseItem(2033);
                    }
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target != null)
            {
                if (Q.IsReady() && IsHuman && target.IsValidTarget(Q.Range) &&
                    getCheckBoxItem(harassMenu, "UseQHarass"))
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.Hitchance >= QHitChanceHarass() && prediction.CollisionObjects.Count == 0)
                        Q.Cast(prediction.CastPosition);
                }

                if (W.IsReady() && IsHuman && target.IsValidTarget(W.Range) &&
                    getCheckBoxItem(harassMenu, "UseWHarass"))
                {
                    W.Cast(target);
                }
            }
        }


        private static void Farm()
        {
            var Humanq = getCheckBoxItem(laneMenu, "UseQLane");
            var Humanw = getCheckBoxItem(laneMenu, "UseWLane");
            var Cougarq = getCheckBoxItem(laneMenu, "UseQCLane");
            var Cougarw = getCheckBoxItem(laneMenu, "UseWCLane");
            var Cougare = getCheckBoxItem(laneMenu, "UseECLane");
            var lanemana = Player.Mana
                             >= (Player.MaxMana * (getSliderItem(laneMenu, "Lane")) / 100);
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 700, MinionTypes.All);

            if (allMinions.Count > 0)
            {
                var Minion = allMinions[0];
                if (IsCougar)
                {
                    if (QC.IsReady() && Cougarq && Minion.LSIsValidTarget(QC.Range))
                    {
                        QC.Cast();
                    }

                    if (EC.IsReady() && Cougare && Minion.LSIsValidTarget(EC.Range))
                    {
                        EC.Cast(Minion.ServerPosition);
                    }

                    foreach (var Minio in allMinions)
                    {
                        if (WC.IsReady() && Cougarw && Player.Distance(Minion) > 200f && Minion.LSIsValidTarget(WC.Range))
                        {
                            WC.Cast(Minio.ServerPosition);
                        }
                    }

                    if (getKeyBindItem(laneMenu, "farm_R") && lanemana && !QC.IsReady() && !WC.IsReady() && !EC.IsReady() && R.IsReady())
                    {
                        R.Cast();
                    }
                }

                if (IsHuman)
                {
                    if (Humanq && lanemana && Q.IsReady())
                    {
                        var prediction = Q.GetPrediction(Minion);
                        if (prediction.Hitchance >= HitChance.Medium) Q.Cast(Minion.ServerPosition);
                    }

                    if (Humanw && lanemana && W.IsReady())
                    {
                        var prediction = W.GetPrediction(Minion);
                        if (prediction.Hitchance >= HitChance.Medium) W.Cast(Minion.ServerPosition);
                    }

                    if (getKeyBindItem(laneMenu, "farm_R") && (!Q.IsReady() || !lanemana || !Humanq))
                    {
                        if (R.IsReady())
                        {
                            R.Cast();
                        }
                    }
                }

                if (E.IsReady() && IsHuman && !getKeyBindItem(laneMenu, "farm_R")
                         && getCheckBoxItem(laneMenu, "farm_E1")
                         && (100 * (Player.Mana / Player.MaxMana)) > getSliderItem(laneMenu, "Lane"))
                {
                    E.CastOnUnit(Player);
                }
            }
        }

        private static void Jungleclear()
        {
            var Humanq = getCheckBoxItem(jungleMenu, "UseQJungle");
            var Humanw = getCheckBoxItem(jungleMenu, "UseWJungle");
            var Cougarq = getCheckBoxItem(jungleMenu, "UseQCJungle");
            var Cougarw = getCheckBoxItem(jungleMenu, "UseWCJungle");
            var Cougare = getCheckBoxItem(jungleMenu, "UseECJungle");
            var Switch = getCheckBoxItem(jungleMenu, "Switchungle");
            var junglemana = Player.Mana
                             >= Player.MaxMana * getSliderItem(jungleMenu, "junglemana") / 100;
            var mobs = MinionManager.GetMinions(
                Player.ServerPosition,
                700,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (IsHuman)
                {
                    if (Humanq && !mob.Name.Contains("Mini") && junglemana && Q.IsReady())
                    {
                        var prediction = Q.GetPrediction(mob);
                        if (prediction.Hitchance >= HitChance.Low) Q.Cast(mob.ServerPosition);
                    }

                    if (Humanw && junglemana && W.IsReady() && !mob.Name.Contains("Mini"))
                    {
                        var prediction = W.GetPrediction(mob);
                        if (prediction.Hitchance >= HitChance.Medium) W.Cast(mob.ServerPosition);
                    }

                    if (Switch && (!Q.IsReady() || !Humanq) && (!W.IsReady() || !Humanw) || !junglemana)
                    {
                        if (R.IsReady())
                        {
                            R.Cast();
                        }
                    }
                }

                if (IsCougar)
                {
                    if (Cougarq && mob.LSIsValidTarget(QC.Range) && QC.IsReady())
                    {
                        QC.Cast();
                    }

                    foreach (var Minion in mobs)
                    {
                        WC.Range = Minion.HasBuff("nidaleepassivehunted") ? 730 : 375;
                        if (Cougarw && Minion.LSIsValidTarget(WC.Range) && WC.IsReady())
                        {
                            WC.Cast(Minion.ServerPosition);
                        }
                    }

                    if (Cougare && mob.IsValidTarget(EC.Range) && EC.IsReady())
                    {
                        EC.Cast(mob.ServerPosition);
                    }

                    if (Switch && junglemana && !QC.IsReady() && !WC.IsReady() && !EC.IsReady() && R.IsReady())
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void AutoE()
        {
            if (Player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready && Player.IsMe)
            {
                if (PortAIO.OrbwalkerManager.isFleeActive) return;
                var forms = getCheckBoxItem(Heal, "AutoSwitchform");
                var health = Player.Health
                             <= Player.MaxHealth * getSliderItem(Heal, "HPercent") / 100;
                var mana = Player.Mana >= Player.MaxMana * getSliderItem(Heal, "MPPercent") / 100;
                if (Player.HasBuff("Recall") || Player.InFountain()) return;
                if (E.IsReady() && health)
                {
                    if (IsHuman && mana)
                    {
                        Player.Spellbook.CastSpell(SpellSlot.E, Player);
                    }

                    if (IsCougar && R.IsReady() && mana && forms)
                    {
                        R.Cast();
                        Player.Spellbook.CastSpell(SpellSlot.E, Player);
                    }
                }
            }
        }



        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All);
            var useQ = getCheckBoxItem(laneMenu, "UseQLH");
            foreach (var minion in allMinions)
            {
                if (Q.IsReady() && IsHuman && useQ && minion.IsValidTarget(Q.Range) &&
                    minion.Health <= 0.95 * Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
            }
        }

        private static void AllyAutoE()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly && !hero.IsMe && hero.LSIsValidTarget(E.Range)))
            {
                var forms = getCheckBoxItem(Heal, "AutoSwitchform");
                var mana = Player.Mana >= Player.MaxMana * getSliderItem(Heal, "MPPercent") / 100;
                if (Player.HasBuff("Recall") || hero.HasBuff("Recall") || hero.InFountain()) return;
                if (E.IsReady() &&
                    hero.Health / hero.MaxHealth * 100 <= getSliderItem(Heal, "AllyHPercent") &&
                    Utility.LSCountEnemiesInRange(1200) > 0 &&
                    hero.LSIsValidTarget(E.Range))
                {
                    if (IsHuman && mana)
                    {
                        E.Cast(hero);
                    }

                    if (IsCougar && R.IsReady() && mana && forms)
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var igniteDmg = Player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
                var qhDmg = Player.GetSpellDamage(hero, SpellSlot.Q);

                if (hero.LSIsValidTarget(600) && getCheckBoxItem(Misc, "UseIgnite") &&
                    IgniteSlot != SpellSlot.Unknown &&
                    Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                {
                    if (igniteDmg > hero.Health)
                    {
                        Player.Spellbook.CastSpell(IgniteSlot, hero);
                    }
                }

                if (Q.IsReady() && hero.LSIsValidTarget(Q.Range) && IsHuman &&
                    getCheckBoxItem(Misc, "UseQKs"))
                {
                    var predictionq = Q.GetPrediction(hero);
                    if (hero.Health <= qhDmg && predictionq.Hitchance >= HitChance.High &&
                        predictionq.CollisionObjects.Count == 0)
                    {
                        Q.Cast(hero);
                    }
                }
            }
        }

        private static void CheckSpells()
        {
            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "JavelinToss" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "Bushwhack" ||
                Player.Spellbook.GetSpell(SpellSlot.E).Name == "PrimalSurge")
            {
                IsHuman = true;
                IsCougar = false;
            }
            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "Takedown" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "Pounce" ||
                Player.Spellbook.GetSpell(SpellSlot.E).Name == "Swipe")
            {
                IsHuman = false;
                IsCougar = true;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var harass = getKeyBindItem(harassMenu, "harasstoggle");
            var cat = Drawing.WorldToScreen(Player.Position);
            if (getCheckBoxItem(Drawings, "Drawharass"))
            {
                if (harass)
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.92f,
                        System.Drawing.Color.GreenYellow,
                        "Auto harass Enabled");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.92f,
                        System.Drawing.Color.OrangeRed,
                        "Auto harass Disabled");
            }

            if (getCheckBoxItem(Drawings, "Drawsmite") && _smite != null)
            {
                if (getKeyBindItem(smiteMenu, "Usesmite"))
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.88f,
                        System.Drawing.Color.GreenYellow,
                        "Smite Jungle On");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.88f,
                        System.Drawing.Color.OrangeRed,
                        "Smite Jungle Off");

                if (Player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker"
                    || Player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel")
                {
                    if (getCheckBoxItem(comboMenu, "smitecombo"))
                    {
                        Drawing.DrawText(
                            Drawing.Width * 0.02f,
                            Drawing.Height * 0.90f,
                            System.Drawing.Color.GreenYellow,
                            "Smite Target On");
                    }
                    else
                        Drawing.DrawText(
                            Drawing.Width * 0.02f,
                            Drawing.Height * 0.90f,
                            System.Drawing.Color.OrangeRed,
                            "Smite Target Off");
                }
            }

            if (IsHuman)
            {
                if (getCheckBoxItem(Drawings, "DrawQ"))
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? System.Drawing.Color.GreenYellow : System.Drawing.Color.OrangeRed);
                }

                if (getCheckBoxItem(Drawings, "DrawW"))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.GreenYellow);
                }

                if (getCheckBoxItem(Drawings, "DrawE"))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.GreenYellow);
                }
            }

            if (IsCougar)
            {
                if (getCheckBoxItem(Drawings, "DrawWC"))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, WC.Range, System.Drawing.Color.GreenYellow);
                }
            }

            if (getCheckBoxItem(Drawings, "DrawCooldown"))
            {
                if (!IsCougar)
                {
                    if (_spideQcd == 0) Drawing.DrawText(cat[0] - 60, cat[1], Color.White, "CQ Rdy");
                    else Drawing.DrawText(cat[0] - 60, cat[1], Color.Orange, "CQ: " + _spideQcd.ToString("0.0"));
                    if (_spideWcd == 0) Drawing.DrawText(cat[0] - 20, cat[1] + 30, Color.White, "CW Rdy");
                    else Drawing.DrawText(cat[0] - 20, cat[1] + 30, Color.Orange, "CW: " + _spideWcd.ToString("0.0"));
                    if (_spideEcd == 0) Drawing.DrawText(cat[0], cat[1], Color.White, "CE Rdy");
                    else Drawing.DrawText(cat[0], cat[1], Color.Orange, "CE: " + _spideEcd.ToString("0.0"));
                }
                else
                {
                    if (_humaQcd == 0) Drawing.DrawText(cat[0] - 60, cat[1], Color.White, "HQ Rdy");
                    else Drawing.DrawText(cat[0] - 60, cat[1], Color.Orange, "HQ: " + _humaQcd.ToString("0.0"));
                    if (_humaWcd == 0) Drawing.DrawText(cat[0] - 20, cat[1] + 30, Color.White, "HW Rdy");
                    else Drawing.DrawText(cat[0] - 20, cat[1] + 30, Color.Orange, "HW: " + _humaWcd.ToString("0.0"));
                    if (_humaEcd == 0) Drawing.DrawText(cat[0], cat[1], Color.White, "HE Rdy");
                    else Drawing.DrawText(cat[0], cat[1], Color.Orange, "HE: " + _humaEcd.ToString("0.0"));
                }
            }
        }

        private static void OnCreateObj(GameObject sender, EventArgs args)
        {
            //Recall
            if (!(sender is Obj_GeneralParticleEmitter)) return;
            var obj = (Obj_GeneralParticleEmitter)sender;
            if (obj != null && obj.IsMe && obj.Name == "TeleportHome")
            {

            }
        }

        private static void OnDeleteObj(GameObject sender, EventArgs args)
        {
            //Recall
            if (!(sender is Obj_GeneralParticleEmitter)) return;
            var obj = (Obj_GeneralParticleEmitter)sender;
            if (obj != null && obj.IsMe && obj.Name == "TeleportHome")
            {

            }
        }
    }
}


