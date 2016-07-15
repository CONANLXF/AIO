using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LCS_Janna.Plugins;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;
using TargetSelector = PortAIO.TSManager;
//using LCS_Janna.Plugins;

namespace LCS_Janna
{
    internal class Program
    {
        public static Menu Config, comboMenu, qsettings, esettings, rsettings;
        public static AIHeroClient Udyr = ObjectManager.Player;
        public static Spell Q, W, E, R;

        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };

        public static HitChance[] HitchanceArray =
        {
            HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh,
            HitChance.Immobile
        };

        public static string[] HighChamps =
        {
            "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
            "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
            "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
            "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
            "Zed", "Ziggs", "Kindred", "Jhin"
        };

        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[qsettings[menuName].Cast<ComboBox>().CurrentValue];
        }

        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Janna")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 120f, 900f, false, SkillshotType.SkillshotLine);

            SpellDatabase.InitalizeSpellDatabase();

            Config = MainMenu.AddMenu("LCS Series: Janna", "LCS Series: Janna");

            comboMenu = Config.AddSubMenu(":: Combo Settings", ":: Combo Settings");
            comboMenu.Add("q.combo", new CheckBox("Use (Q)"));
            comboMenu.Add("w.combo", new CheckBox("Use (W)"));

            qsettings = Config.AddSubMenu(":: Q Settings", ":: Q Settings");
            qsettings.Add("q.settings", new ComboBox("(Q) Mode :", 0, "Normal", "Q Hit x Target"));
            qsettings.Add("q.normal.hit.chance",
                new ComboBox("(Q) Hit Chance (Normal)", 2, "Low", "Medium", "High", "Very High", "Only Immobile"));
            qsettings.Add("q.hit.count", new Slider("(Q) Hit Enemy Count", 2, 1, 5));
            qsettings.Add("q.antigapcloser", new CheckBox("(Q) Anti-Gapcloser"));

            esettings = Config.AddSubMenu(":: E Settings", ":: E Settings");
            esettings.Add("Janna_AutoE", new CheckBox("Auto E"));
            esettings.AddGroupLabel(":: Protectable Skillshots");
            foreach (var spell in EntityManager.Heroes.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
            {
                esettings.Add(string.Format("e.protect.{0}", spell.SpellName), new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            esettings.AddSeparator();
            esettings.AddGroupLabel(":: Protectable Targetted Spells");
            foreach (var spell in EntityManager.Heroes.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted)))
            {
                esettings.Add(string.Format("e.protect.targetted.{0}", spell.SpellName), new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            esettings.AddSeparator();
            esettings.AddGroupLabel(":: Engage Spells");
            foreach (var spell in EntityManager.Heroes.Allies.SelectMany(ally => SpellDatabase.EscapeSpells.Where(p => p.ChampionName == ally.ChampionName)))
            {
                esettings.Add(string.Format("e.engage.{0}", spell.SpellName), new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            esettings.AddSeparator();
            esettings.AddGroupLabel(":: Whitelist");
            foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValid))
            {
                esettings.Add("e." + ally.ChampionName, new CheckBox("(E): " + ally.ChampionName, HighChamps.Contains(ally.ChampionName)));
            }
            esettings.AddSeparator();
            esettings.Add("turret.hp.percent", new Slider("Turret HP Percent", 10, 1, 99));
            esettings.Add("protect.carry.from.turret", new CheckBox("Protect Carry From Turrets"));
            esettings.Add("min.mana.for.e", new Slider("Min. Mana", 50, 1, 99));

            rsettings = Config.AddSubMenu(":: R Settings", ":: R Settings");
            rsettings.AddGroupLabel(":: Protectable Skillshots");
            foreach (var spell in EntityManager.Heroes.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
            {
                rsettings.Add(string.Format("r.protect.{0}", spell.SpellName), new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            rsettings.AddGroupLabel(":: Protectable Targetted Spells");
            foreach (var spell in EntityManager.Heroes.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted)))
            {
                rsettings.Add(string.Format("r.protect.targetted.{0}", spell.SpellName), new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            rsettings.Add("spell.damage.percent", new Slider("Min. Spell Damage Percentage", 10, 1, 99));

            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            AntiGapcloser.OnEnemyGapcloser += OnGapcloser;
            AIHeroClient.OnBasicAttack += AIHeroClient_OnBasicAttack;
            Obj_AI_Turret.OnBasicAttack += Obj_AI_Turret_OnBasicAttack;
            Game.OnUpdate += OnUpdate;
        }

        private static void Obj_AI_Turret_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (ObjectManager.Player.ManaPercent >= getSliderItem(esettings, "min.mana.for.e") && args.Target.IsValid<AIHeroClient>())
            {
                if (sender.IsEnemy && E.IsInRange((AIHeroClient)args.Target) && ((AIHeroClient)args.Target).IsAlly && getCheckBoxItem(esettings, "e." + ((AIHeroClient)args.Target).ChampionName) && getCheckBoxItem(esettings, "protect.carry.from.turret"))
                {
                    E.Cast((AIHeroClient)args.Target);
                }
            }

            if (ObjectManager.Player.ManaPercent >= getSliderItem(esettings, "min.mana.for.e"))
            {
                if (sender.IsAlly && E.IsInRange(sender) && sender.HealthPercent < getSliderItem(esettings, "turret.hp.percent"))
                {
                    E.Cast((AIHeroClient)args.Target);
                }
            }
        }

        private static void AIHeroClient_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!args.Target.IsValid<AIHeroClient>())
            {
                return;
            }
            if (ObjectManager.Player.ManaPercent >= getSliderItem(esettings, "min.mana.for.e"))
            {
                if (sender.IsEnemy && E.IsInRange((AIHeroClient)args.Target) && args.Target.IsAlly && getCheckBoxItem(esettings, "e." + ((AIHeroClient)args.Target).ChampionName))
                {
                    E.Cast((AIHeroClient)args.Target);
                }
            }
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

        private static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsEnemy && gapcloser.End.LSDistance(ObjectManager.Player.Position) < 200 &&
                gapcloser.Sender.LSIsValidTarget(Q.Range) && getCheckBoxItem(qsettings, "q.antigapcloser"))
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        public static AIHeroClient Player = ObjectManager.Player;

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (E.IsReady() && ObjectManager.Player.ManaPercent >= getSliderItem(esettings, "min.mana.for.e"))
            {
                if (esettings["e.engage." + args.SData.Name] != null)
                {
                    if (sender.IsValid<AIHeroClient>() && sender.IsAlly && getCheckBoxItem(esettings, "e.engage." + args.SData.Name) && getCheckBoxItem(esettings, "e." + sender.BaseSkinName) && E.IsInRange(sender) && !sender.IsDead && !sender.IsZombie && sender.IsValid)
                    {
                        E.CastOnUnit(sender);
                    }
                }

                if (esettings["e.protect." + args.SData.Name] != null)
                {
                    if (sender.IsValid<AIHeroClient>() && sender.IsEnemy && args.Target.IsValid<AIHeroClient>() && args.Target.IsAlly && E.IsInRange((AIHeroClient)args.Target) && getCheckBoxItem(esettings, "e.protect." + args.SData.Name) && getCheckBoxItem(esettings, "e." + sender.BaseSkinName))
                    {
                        E.Cast((AIHeroClient)args.Target);
                    }
                }

                if (esettings["e.protect.targetted." + args.SData.Name] != null)
                {
                    if (sender.IsValid<AIHeroClient>() && sender.IsEnemy && args.Target.IsValid<AIHeroClient>() && args.Target.IsAlly && E.IsInRange((AIHeroClient)args.Target) && getCheckBoxItem(esettings, "e.protect.targetted." + args.SData.Name) && getCheckBoxItem(esettings, "e." + sender.BaseSkinName))
                    {
                        E.Cast((AIHeroClient)args.Target);
                    }
                }
            }

            if (args.Target is Obj_AI_Minion || !(sender is AIHeroClient))
                return;

            if (getCheckBoxItem(esettings, "Janna_AutoE") && E.IsReady())
            {
                if (sender.IsEnemy)
                {
                    var StartPos = args.Start;
                    var EndPos = args.End;
                    var NonTRange = new EloBuddy.SDK.Geometry.Polygon.Rectangle(StartPos, EndPos, sender.BoundingRadius + 30);
                    var Target = EntityManager.Heroes.Allies.FirstOrDefault(f => f.Position.LSDistance(Player.Position) <= E.Range && NonTRange.IsInside(f.Position));
                    if (Target != null)
                    {
                        E.Cast(Target, true);
                        return;
                    }
                    if (args.Target != null && args.Target.Position.LSDistance(Player.Position) <= E.Range && args.Target is AIHeroClient)
                    {
                        var ShieldTarget = EntityManager.Heroes.Allies.FirstOrDefault(f => f.Position.LSDistance(args.Target.Position) <= 10);
                        E.Cast(ShieldTarget, true);
                        return;
                    }
                }
                if (sender.IsAlly && args.Target is AIHeroClient)
                {
                    if (sender.Position.LSDistance(Player.Position) <= E.Range && args.Target != null && args.SData.Name.ToLower().Contains("attack"))
                    {
                        E.CastOnUnit(sender, true);
                        return;
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                OnCombo();
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (target != null)
            {
                if (getCheckBoxItem(comboMenu, "w.combo") && W.IsReady())
                {
                    W.Cast(target);
                }
            }

            if (getCheckBoxItem(comboMenu, "q.combo"))
            {
                switch (getBoxItem(qsettings, "q.settings"))
                {
                    case 0:
                        foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => x.LSIsValidTarget(Q.Range)))
                        {
                            Q.CastIfHitchanceEquals(enemy, HikiChance("q.normal.hit.chance"));
                        }
                        break;
                    case 1:
                        if (ObjectManager.Player.CountEnemiesInRange(Q.Range) >= getSliderItem(qsettings, "q.hit.count"))
                        {
                            foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && Q.GetHitCount() >= getSliderItem(qsettings, "q.hit.count")))
                            {
                                Q.CastIfWillHit(enemy, getSliderItem(qsettings, "q.hit.count"));
                            }
                        }
                        break;
                }
            }
        }
    }
}