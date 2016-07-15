using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;

 namespace D_Kayle
{
    internal class Program
    {
        private const string ChampionName = "Kayle";

        private static Spell _q, _w, _e, _r;

        private static readonly List<Spell> SpellList = new List<Spell>();

        private static SpellSlot _igniteSlot;

        private static Menu _config;

        public static Menu comboMenu, utilityMenu, itemMenu, clearMenu, miscMenu, drawMenu, smiteMenu, harassMenu;

        private static AIHeroClient _player;

        private static SpellSlot _smiteSlot;

        private static Spell _smite;

        private static Items.Item _rand, _lotis, _frostqueen, _mikael;

        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                Chat.Print("Please use Kayle~");
                return;
            }

            _q = new Spell(SpellSlot.Q, 650f);
            _w = new Spell(SpellSlot.W, 900f);
            _e = new Spell(SpellSlot.E, 675f);
            _r = new Spell(SpellSlot.R, 900f);

            if (_player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite"))
            {
                _smite = new Spell(SpellSlot.Summoner1, 570f);
                _smiteSlot = SpellSlot.Summoner1;
            }
            else if (_player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
            {
                _smite = new Spell(SpellSlot.Summoner2, 570f);
                _smiteSlot = SpellSlot.Summoner2;
            }

            SpellList.Add(_q);
            SpellList.Add(_w);
            SpellList.Add(_e);
            SpellList.Add(_r);

            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);
            _frostqueen = new Items.Item(3092, 800f);
            _mikael = new Items.Item(3222, 600f);

            _igniteSlot = _player.GetSpellSlot("SummonerDot");

            _config = MainMenu.AddMenu("D-Kayle", "D-Kayle");


            comboMenu = _config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseIgnitecombo", new CheckBox("Use Ignite", true));
            comboMenu.Add("smitecombo", new CheckBox("Use Smite in target", true));
            comboMenu.Add("UseQCombo", new CheckBox("Use Q", true));
            comboMenu.Add("UseWCombo", new CheckBox("Use W", true));
            comboMenu.Add("UseECombo", new CheckBox("Use E", true));


            itemMenu = _config.AddSubMenu("Items", "items");
            itemMenu.Add("frostQ", new CheckBox("Use Frost Queen's"));
            itemMenu.AddSeparator();
            itemMenu.Add("Omen", new CheckBox("Use Randuin Omen"));
            itemMenu.Add("Omenenemys", new Slider("Randuin if enemys >", 2, 1, 5));
            itemMenu.Add("lotis", new CheckBox("Use Iron Solari"));
            itemMenu.Add("lotisminhp", new Slider("Solari if Ally Hp <", 35, 1, 100));
            itemMenu.AddLabel("Cleanse");
            itemMenu.AddSeparator();
            itemMenu.Add("usemikael", new CheckBox("Use Mikael's to remove Debuffs"));
            itemMenu.Add("mikaelusehp", new Slider("Mikael's if Ally Hp <", 35, 1, 100));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly))
            {
                itemMenu.Add("mikaeluse" + hero.BaseSkinName, new CheckBox(hero.BaseSkinName));

            }
            itemMenu.Add("useqss", new CheckBox("Use QSS/Mercurial Scimitar/Dervish Blade"));
            itemMenu.Add("blind", new CheckBox("Blind"));
            itemMenu.Add("charm", new CheckBox("Charm"));
            itemMenu.Add("fear", new CheckBox("Fear"));
            itemMenu.Add("flee", new CheckBox("Flee"));
            itemMenu.Add("taunt", new CheckBox("Taunt"));
            itemMenu.Add("snare", new CheckBox("Snare"));
            itemMenu.Add("suppression", new CheckBox("Suppression"));
            itemMenu.Add("stun", new CheckBox("Stun"));
            itemMenu.Add("polymorph", new CheckBox("Polymorph"));
            itemMenu.Add("silence", new CheckBox("Silence"));
            itemMenu.Add("Cleansemode", new ComboBox("Use Cleanse", 1, "Always", "In Combo"));
            itemMenu.AddLabel("Potions");
            itemMenu.AddSeparator();
            itemMenu.Add("usehppotions", new CheckBox("Use Healt potion/Refillable/Hunters/Corrupting/Biscuit"));
            itemMenu.Add("usepotionhp", new Slider("If Health % <", 35, 1, 100));
            itemMenu.Add("usemppotions", new CheckBox("Use Hunters/Corrupting/Biscuit"));
            itemMenu.Add("usepotionmp", new Slider("If Mana % <", 35, 1, 100));


            utilityMenu = _config.AddSubMenu("Utilities", "utilities");
            utilityMenu.Add("onmeW", new CheckBox("W Self"));
            utilityMenu.Add("healper", new Slider("Self Health %", 40, 1, 100));
            utilityMenu.Add("onmeR", new CheckBox("R Self Use"));
            utilityMenu.Add("healper", new Slider("Self Health %", 40, 1, 100));
            utilityMenu.AddLabel("Use W Ally");
            utilityMenu.Add("allyW", new CheckBox("W Ally"));
            utilityMenu.Add("allyhealper", new Slider("Ally Health %", 40, 1, 100));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly && !hero.IsMe))
            {
                utilityMenu.Add("usewally" + hero.BaseSkinName, new CheckBox(hero.BaseSkinName));

            }
            utilityMenu.AddLabel("Use R Ally");
            utilityMenu.Add("allyR", new CheckBox("R Ally"));
            utilityMenu.Add("ultiallyHP", new Slider("Ally Health %", 40, 1, 100));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly && !hero.IsMe))
            {
                utilityMenu.Add("userally" + hero.BaseSkinName, new CheckBox(hero.BaseSkinName));

            }


            harassMenu = _config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q"));
            harassMenu.Add("UseEHarass", new CheckBox("Use E"));
            harassMenu.Add("harasstoggle", new KeyBind("AutoHarass (toggle)", false, KeyBind.BindTypes.PressToggle, 'L'));
            harassMenu.Add("Harrasmana", new Slider("Minimum Mana %", 60, 1, 100));


            clearMenu = _config.AddSubMenu("Farm", "Farm");
            clearMenu.AddGroupLabel("LaneClear");
            clearMenu.Add("UseQLane", new CheckBox("Use Q Lane"));
            clearMenu.Add("UseELane", new CheckBox("Use E Lane"));
            clearMenu.Add("Farmmana", new Slider("Minimum Mana %", 60, 1, 100));
            clearMenu.AddGroupLabel("Lasthit");
            clearMenu.Add("UseQLast", new CheckBox("Use Q Last"));
            clearMenu.Add("UseELast", new CheckBox("Use E Last"));
            clearMenu.Add("lasthitmana", new Slider("Minimum Mana %", 60, 1, 100));
            clearMenu.AddGroupLabel("Jungleclear");
            clearMenu.Add("UseQjungle", new CheckBox("Use Q Jungle"));
            clearMenu.Add("UseEjungle", new CheckBox("Use E Jungle"));
            clearMenu.Add("junglemana", new Slider("Minimum Mana %", 60, 1, 100));


            smiteMenu = _config.AddSubMenu("Smite", "Smite");
            smiteMenu.Add("Usesmite", new KeyBind("Use Smite (toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));
            smiteMenu.Add("Useblue", new CheckBox("Smite Blue Early"));
            smiteMenu.Add("manaJ", new Slider("Smite Blue Early if MP %", 35, 1, 100));
            smiteMenu.Add("Usered", new CheckBox("Smite Red Early"));
            smiteMenu.Add("healthJ", new Slider("Smite Red Early if HP %", 35, 1, 100));


            miscMenu = _config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("UseQKs", new CheckBox("Use Q KillSteal"));
            miscMenu.Add("UseIgnite", new CheckBox("Use Ignite KillSteal"));
            miscMenu.Add("GapCloserE", new CheckBox("Use Q to GapCloser"));
            miscMenu.Add("Escape", new KeyBind("Escapes key", false, KeyBind.BindTypes.HoldActive, 'Z'));
            miscMenu.Add("support", new CheckBox("Support Mode"));


            drawMenu = _config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q", false));
            drawMenu.Add("DrawW", new CheckBox("Draw W", false));
            drawMenu.Add("DrawE", new CheckBox("Draw E", false));
            drawMenu.Add("DrawR", new CheckBox("Draw R", false));
            drawMenu.Add("Drawharass", new CheckBox("Draw AutoHarass", true));
            drawMenu.Add("Drawsmite", new CheckBox("Draw smite", true));


            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;

            Chat.Print("<font color='#881df2'>D-Kayle By Diabaths </font>Loaded!");
            Chat.Print(
                "<font color='#f2f21d'>Do you like it???  </font> <font color='#ff1900'>Drop 1 Upvote in Database </font>");
            Chat.Print(
                "<font color='#f2f21d'>Buy me cigars </font> <font color='#ff1900'>ssssssssssmith@hotmail.com</font> (10) S");
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
            _player = ObjectManager.Player;

            if (getKeyBindItem(miscMenu, "Escape"))
            {
                Escape();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                    || getKeyBindItem(harassMenu, "harasstoggle"))
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(harassMenu, "Harrasmana"))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(clearMenu, "lasthitmana"))
            {
                Lasthit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(clearMenu, "Farmmana"))
            {
                Farm();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(clearMenu, "junglemana"))
            {
                JungleFarm();
            }

            Usepotion();
            if (getKeyBindItem(smiteMenu, "Usesmite"))
            {
                Smiteuse();
            }

            AutoW();
            AutoR();
            AllyR();
            AllyW();
            KillSteal();
            Usecleanse();
        }

        private static void UseItemes()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var iOmen = getCheckBoxItem(itemMenu, "Omen");
                var iOmenenemys = hero.LSCountEnemiesInRange(450) >= getSliderItem(itemMenu, "Omenenemys");
                var ifrost = getCheckBoxItem(itemMenu, "frostQ");

                if (ifrost && _frostqueen.IsReady() && hero.LSIsValidTarget(_frostqueen.Range))
                {
                    _frostqueen.Cast();
                }

                if (iOmenenemys && iOmen && _rand.IsReady() && hero.LSIsValidTarget(_rand.Range))
                {
                    _rand.Cast();
                }
            }

            var ilotis = getCheckBoxItem(itemMenu, "lotis");
            if (ilotis)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly || hero.IsMe))
                {
                    if (hero.Health <= (hero.MaxHealth * (getSliderItem(itemMenu, "lotisminhp")) / 100)
                        && hero.LSDistance(_player.ServerPosition) <= _lotis.Range && _lotis.IsReady())
                        _lotis.Cast();
                }
            }
        }


        private static void Usecleanse()
        {
            if (_player.IsDead
                || (getBoxItem(itemMenu, "Cleansemode") == 1)
                    && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return;
            if (Cleanse(_player) && getCheckBoxItem(itemMenu, "useqss"))
            {
                if (_player.HasBuff("zedulttargetmark"))
                {
                    if (Items.HasItem(3140) && Items.CanUseItem(3140)) Utility.DelayAction.Add(1000, () => Items.UseItem(3140));
                    else if (Items.HasItem(3139) && Items.CanUseItem(3139)) Utility.DelayAction.Add(1000, () => Items.UseItem(3139));
                    else if (Items.HasItem(3137) && Items.CanUseItem(3137)) Utility.DelayAction.Add(1000, () => Items.UseItem(3137));
                }
                else
                {
                    if (Items.HasItem(3140) && Items.CanUseItem(3140)) Items.UseItem(3140);
                    else if (Items.HasItem(3139) && Items.CanUseItem(3139)) Items.UseItem(3139);
                    else if (Items.HasItem(3137) && Items.CanUseItem(3137)) Items.UseItem(3137);
                }
            }
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => (hero.IsAlly || hero.IsMe)))
            {
                var usemikael = getCheckBoxItem(itemMenu, "usemikael");
                var mikaeluse = hero.Health
                                <= (hero.MaxHealth * (getSliderItem(itemMenu, "mikaelusehp") / 100));
                if (((Cleanse(hero) && usemikael) || mikaeluse)
                    && getCheckBoxItem(itemMenu, "mikaeluse" + hero.BaseSkinName) == true)
                {
                    if (_mikael.IsReady() && hero.LSDistance(_player.ServerPosition) <= _mikael.Range)
                    {
                        if (_player.HasBuff("zedulttargetmark")) Utility.DelayAction.Add(500, () => _mikael.Cast(hero));
                        else _mikael.Cast(hero);
                    }
                }
            }
        }

        private static bool Cleanse(AIHeroClient hero)
        {
            bool cc = false;
            if (getCheckBoxItem(itemMenu, "blind"))
            {
                if (hero.HasBuffOfType(BuffType.Blind))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "charm"))
            {
                if (hero.HasBuffOfType(BuffType.Charm))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "fear"))
            {
                if (hero.HasBuffOfType(BuffType.Fear))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "flee"))
            {
                if (hero.HasBuffOfType(BuffType.Flee))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "snare"))
            {
                if (hero.HasBuffOfType(BuffType.Snare))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "taunt"))
            {
                if (hero.HasBuffOfType(BuffType.Taunt))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "suppression"))
            {
                if (hero.HasBuffOfType(BuffType.Suppression))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "stun"))
            {
                if (hero.HasBuffOfType(BuffType.Stun))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "polymorph"))
            {
                if (hero.HasBuffOfType(BuffType.Polymorph))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "silence"))
            {
                if (hero.HasBuffOfType(BuffType.Silence))
                {
                    cc = true;
                }
            }

            return cc;
        }

        // princer007  Code
        private static int Getallies(float range)
        {
            int allies = 0;
            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>()) if (hero.IsAlly && !hero.IsMe && _player.LSDistance(hero) <= range) allies++;
            return allies;
        }

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Getallies(1000) > 0 && ((Obj_AI_Base)Orbwalker.LastTarget).IsMinion
                && /*args.Unit.IsMinion &&*/ getCheckBoxItem(miscMenu, "support"))
                args.Process = false;
        }

        private static void Smiteontarget()
        {
            if (_smite == null) return;
            var hero = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(570));
            var smiteDmg = _player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Smite);
            var usesmite = getCheckBoxItem(comboMenu, "smitecombo");
            if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker" && usesmite
                && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready)
            {
                if (hero != null && (!hero.HasBuffOfType(BuffType.Stun) || !hero.HasBuffOfType(BuffType.Slow)))
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
                else if (smiteDmg >= hero.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
            }

            if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel" && usesmite
                && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready && hero.LSIsValidTarget(570))
            {
                ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
            }
        }

        private static void AutoR()
        {
            if (_player.HasBuff("Recall") || _player.InFountain()) return;
            if (getCheckBoxItem(utilityMenu, "onmeR")
                && (_player.Health / _player.MaxHealth) * 100 <= getSliderItem(utilityMenu, "ultiSelfHP")
                && _r.IsReady() && Utility.LSCountEnemiesInRange(650) > 0)
            {
                _r.Cast(_player);
            }
        }

        private static void AllyR()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly && !hero.IsMe))
            {
                if (_player.HasBuff("Recall") || hero.InFountain()) return;
                if (!hero.LSIsValidTarget(_r.Range)) return;
                if (getCheckBoxItem(utilityMenu, "allyR")
                    && (hero.Health / hero.MaxHealth) * 100 <= getSliderItem(utilityMenu, "ultiallyHP")
                    && _r.IsReady() && Utility.LSCountEnemiesInRange(1000) > 0
                    && hero.LSDistance(_player.ServerPosition) <= _r.Range)
                    if (getCheckBoxItem(utilityMenu, "userally" + hero.BaseSkinName) == true)
                    {
                        _r.Cast(hero);
                    }
            }
        }

        private static void Usepotion()
        {
            var mobs = MinionManager.GetMinions(
                _player.ServerPosition,
                _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            var iusehppotion = getCheckBoxItem(itemMenu, "usehppotions");
            var iusepotionhp = _player.Health
                               <= (_player.MaxHealth * (getSliderItem(itemMenu, "usepotionhp")) / 100);
            var iusemppotion = getCheckBoxItem(itemMenu, "usemppotions");
            var iusepotionmp = _player.Mana
                               <= (_player.MaxMana * (getSliderItem(itemMenu, "usepotionmp")) / 100);
            if (_player.InFountain() || ObjectManager.Player.HasBuff("Recall")) return;

            if (ObjectManager.Player.LSCountEnemiesInRange(800) > 0
                || (mobs.Count > 0 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && _smite != null))
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

        private static float ComboDamage(Obj_AI_Base enemy)
        {
            var itemscheck = _player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker"
                             || _player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel";
            var damage = 0d;
            if (_igniteSlot != SpellSlot.Unknown && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready) damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            if (_q.IsReady()) damage += _player.LSGetSpellDamage(enemy, SpellSlot.Q);
            if (_e.IsReady() || ObjectManager.Player.HasBuff("JudicatorRighteousFury"))
            {
                damage += _player.LSGetSpellDamage(enemy, SpellSlot.E);
                damage = damage + _player.LSGetAutoAttackDamage(enemy, true) * 4;
            }

            if (itemscheck && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready)
            {
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Smite);
            }

            if (ObjectManager.Player.HasBuff("LichBane"))
            {
                damage += _player.BaseAttackDamage * 0.75 + _player.FlatMagicDamageMod * 0.5;
            }

            return (float)damage;
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(_r.Range + 200, DamageType.Magical);
            if (target != null)
            {
                Smiteontarget();
                UseItemes();

                if (target.LSIsValidTarget(600) && getCheckBoxItem(comboMenu, "UseIgnitecombo")
                    && _igniteSlot != SpellSlot.Unknown
                    && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                {
                    if (ComboDamage(target) > target.Health - 100)
                    {
                        _player.Spellbook.CastSpell(_igniteSlot, target);
                    }
                }

                if (getCheckBoxItem(comboMenu, "UseQCombo") && _q.IsReady() && target.LSIsValidTarget(_q.Range))
                {
                    _q.Cast(target);
                }

                if (getCheckBoxItem(comboMenu, "UseECombo") && _e.IsReady() && target.LSIsValidTarget(525))
                {
                    _e.Cast();
                }

                if (_w.IsReady() && getCheckBoxItem(comboMenu, "UseWCombo") && target.LSIsValidTarget(_w.Range)
                    && _player.LSDistance(target.Position) > _q.Range)
                {
                    _w.Cast(_player);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_q.IsReady() && gapcloser.Sender.LSIsValidTarget(_q.Range) && getCheckBoxItem(miscMenu, "GapCloserE"))
            {
                _q.Cast(gapcloser.Sender);
            }
        }

        private static void Escape()
        {
            var target = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (_player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready && _player.IsMe)
            {
                if (_w.IsReady() && Utility.LSCountEnemiesInRange(1200) > 0)
                {
                    _player.Spellbook.CastSpell(SpellSlot.W, _player);
                }
            }

            if (target.LSIsValidTarget(_q.Range) && _q.IsReady())
            {
                _q.Cast(target);
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            if (target.LSIsValidTarget(_q.Range) && _q.IsReady() && getCheckBoxItem(harassMenu, "UseQHarass"))
            {
                _q.Cast(target);
            }

            if (target.LSIsValidTarget(_q.Range) && _e.IsReady() && getCheckBoxItem(harassMenu, "UseEHarass")) _e.Cast();
        }


        private static void Farm()
        {
            var minions = MinionManager.GetMinions(_player.ServerPosition, _q.Range);
            foreach (var minion in minions)
            {
                if (getCheckBoxItem(clearMenu, "UseQLane") && _q.IsReady())
                {
                    if (minions.Count > 2)
                    {
                        _q.Cast(minion);

                    }

                    else
                        foreach (var minionQ in minions)
                            if (!Orbwalking.InAutoAttackRange(minion)
                                && minionQ.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.Q))
                                _q.Cast(minionQ);
                }

                if (getCheckBoxItem(clearMenu, "UseELane") && _e.IsReady())
                {
                    if (minions.Count > 4)
                    {
                        _e.Cast();

                    }
                }
            }
        }

        private static void Lasthit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var useQ = getCheckBoxItem(clearMenu, "UseQLast");
            var useE = getCheckBoxItem(clearMenu, "UseELast");
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast(minion);
                }

                if (_e.IsReady() && useE && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.E)
                    && allMinions.Count > 2)
                {
                    _e.Cast();
                }
            }
        }

        private static void JungleFarm()
        {
            var mobs = MinionManager.GetMinions(
                _player.ServerPosition,
                _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (getCheckBoxItem(clearMenu, "UseQjungle") && _q.IsReady() && mob.LSIsValidTarget(_q.Range)
                    && !mob.Name.Contains("Mini"))
                {
                    _q.Cast(mob);
                }

                if (getCheckBoxItem(clearMenu, "UseEjungle") && _e.IsReady() && mob.LSIsValidTarget(_q.Range))
                {
                    _e.Cast();
                }
            }
        }

        private static void AutoW()
        {
            if (_player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready && _player.IsMe)
            {
                if (_player.HasBuff("Recall") || _player.InFountain()) return;

                if (getCheckBoxItem(utilityMenu, "onmeW") && _w.IsReady()
                    && _player.Health <= (_player.MaxHealth * (getSliderItem(utilityMenu, "healper")) / 100))
                {
                    _player.Spellbook.CastSpell(SpellSlot.W, _player);
                }
            }
        }

        private static void AllyW()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly && !hero.IsMe))
            {
                if (_player.HasBuff("Recall") || hero.HasBuff("Recall") || hero.InFountain()) return;
                if (!hero.LSIsValidTarget(_w.Range)) return;
                if (getCheckBoxItem(utilityMenu, "allyW")
                    && (hero.Health / hero.MaxHealth) * 100 <= getSliderItem(utilityMenu, "allyhealper")
                    && _w.IsReady() && Utility.LSCountEnemiesInRange(1200) > 0
                    && hero.LSDistance(_player.ServerPosition) <= _w.Range)
                    if (getCheckBoxItem(utilityMenu, "usewally" + hero.BaseSkinName) == true)
                    {
                        _w.Cast(hero);
                    }
            }
        }

        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var igniteDmg = _player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
                var qhDmg = _player.LSGetSpellDamage(hero, SpellSlot.Q);

                if (hero.LSIsValidTarget(600) && getCheckBoxItem(miscMenu, "UseIgnite")
                    && _igniteSlot != SpellSlot.Unknown
                    && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                {
                    if (igniteDmg > hero.Health)
                    {
                        _player.Spellbook.CastSpell(_igniteSlot, hero);
                    }
                }

                if (_q.IsReady() && hero.LSIsValidTarget(_q.Range) && getCheckBoxItem(miscMenu, "UseQKs"))
                {
                    if (hero.Health <= qhDmg)
                    {
                        _q.Cast(hero);
                    }
                }
            }
        }

        public static readonly string[] Smitetype =
            {
                "s5_summonersmiteplayerganker", "s5_summonersmiteduel",
                "s5_summonersmitequick", "itemsmiteaoe", "summonersmite"
            };

        private static int GetSmiteDmg()
        {
            int level = _player.Level;
            int index = _player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }

        //New map Monsters Name By SKO
        private static void Smiteuse()
        {
            var jungle = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
            if (ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) != SpellState.Ready) return;
            var useblue = getCheckBoxItem(smiteMenu, "Useblue");
            var usered = getCheckBoxItem(smiteMenu, "Usered");
            var health = (100 * (_player.Health / _player.MaxHealth)) < getSliderItem(smiteMenu, "healthJ");
            var mana = (100 * (_player.Mana / _player.MaxMana)) < getSliderItem(smiteMenu, "manaJ");
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

            var minions = MinionManager.GetMinions(_player.Position, 1000, MinionTypes.All, MinionTeam.Neutral);
            if (minions.Count() > 0)
            {
                int smiteDmg = GetSmiteDmg();

                foreach (Obj_AI_Base minion in minions)
                {
                    if (Utility.Map.GetMap().Type == Utility.Map.MapType.TwistedTreeline && minion.Health <= smiteDmg
                        && jungleMinions.Any(name => minion.Name.Substring(0, minion.Name.Length - 5).Equals(name)))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }

                    if (minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name))
                        && !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                    else if (jungle && useblue && mana && minion.Health >= smiteDmg
                             && jungleMinions.Any(name => minion.Name.StartsWith("SRU_Blue"))
                             && !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                    else if (jungle && usered && health && minion.Health >= smiteDmg
                             && jungleMinions.Any(name => minion.Name.StartsWith("SRU_Red"))
                             && !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var harass = (getKeyBindItem(harassMenu, "harasstoggle"));

            if (getCheckBoxItem(drawMenu, "Drawharass"))
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

            if (getCheckBoxItem(drawMenu, "Drawsmite") && _smite != null)
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

                if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker"
                    || _player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel")
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

            if (getCheckBoxItem(drawMenu, "DrawQ") && _q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.GreenYellow);
            }

            if (getCheckBoxItem(drawMenu, "DrawW") && _w.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.GreenYellow);
            }

            if (getCheckBoxItem(drawMenu, "DrawE") && _e.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.GreenYellow);
            }

            if (getCheckBoxItem(drawMenu, "DrawR") && _r.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.GreenYellow);
            }
        }
    }
}


