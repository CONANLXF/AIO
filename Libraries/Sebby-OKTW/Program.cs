using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby;
using OneKeyToWin_AIO_Sebby.Champions;
using OneKeyToWin_AIO_Sebby.Core;
using SharpDX;
using Color = System.Drawing.Color;
using HitChance = SebbyLib.Prediction.HitChance;
using PredictionInput = SebbyLib.Prediction.PredictionInput;
using PredictionOutput = SebbyLib.Prediction.PredictionOutput;
using Spell = LeagueSharp.Common.Spell;
using SPrediction;
using EloBuddy.SDK.Spells;


namespace SebbyLib
{
    internal class Program
    {
        public static PredictionOutput DrawSpellPos;
        public static Spell Q, W, E, R, DrawSpell;
        public static float DrawSpellTime;
        public static int AIOmode;
        public static Menu Config;
        public static Obj_SpawnPoint enemySpawn;
        public static int HitChanceNum = 4, tickNum = 4, tickIndex;
        private static float dodgeTime = Game.Time;
        public static List<AIHeroClient> Enemies = new List<AIHeroClient>(), Allies = new List<AIHeroClient>();
        public static AIHeroClient jungler = ObjectManager.Player;
        public static bool Farm
        {
            get
            {
                return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                       Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
            }
        }

        public static bool Combo
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo); }
        }

        public static bool None
        {
            get
            {
                return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None);
            }
        }

        public static bool LaneClear
        {
            get { return (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)) && getCheckBoxItem("harassLaneclear"); }
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool IsJungler(AIHeroClient hero)
        {
            return hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite"));
        }

        public static bool getCheckBoxItem(string item)
        {
            return Config[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Config[item].Cast<Slider>().CurrentValue;
        }

        internal static List<SpellInfo> _spells;

        public static int getHitChance
        {
            get
            {
                return getSliderItem("HitChance");
            }
        }

        public static bool getKeyBindItem(string item)
        {
            return Config[item].Cast<KeyBind>().CurrentValue;
        }
        public static bool SPredictionLoad;
        public static void GameOnOnGameLoad()
        {
            _spells = EloBuddy.SDK.Spells.SpellDatabase.GetSpellInfoList(ObjectManager.Player.BaseSkinName);

            enemySpawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy);
            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E);
            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R);

            Config = MainMenu.AddMenu("OneKeyToWin AIO", "OneKeyToWin_AIO" + ObjectManager.Player.ChampionName);

            #region MENU ABOUT OKTW

            Config.Add("debug", new CheckBox("Debug", false));
            Config.Add("debugChat", new CheckBox("Debug Chat", false));
            Config.Add("print", new CheckBox("OKTW NEWS in chat"));

            #endregion

            Config.Add("AIOmode", new Slider("AIO mode (0 : Util & Champ | 1 : Only Champ | 2 : Only Util)", 0, 0, 2));
            AIOmode = getSliderItem("AIOmode");

            var slots = new HashSet<SpellSlot>();
            foreach (var info in _spells)
            {
                slots.Add(info.Slot);
            }

            Config.Add("PredictionMODE", new Slider("Pred Mode (0 : L# Pred | 1 : OKTW Pred | 2 : SPred | 3 : SDK | 4 : EB)", 0, 0, 4));
            Config.Add("HitChance", new Slider("AIO mode (0 : Very High | 1 : High | 2 : Medium)", 0, 0, 2));
            Config.Add("debugPred", new CheckBox("Draw Aiming OKTW© PREDICTION", false));
            Config.Add("harassLaneclear", new CheckBox("Skill-Harass in lane clear"));

            if (getSliderItem("PredictionMODE") == 2)
            {
                SPrediction.Prediction.Initialize(Config);
                SPredictionLoad = true;
            }
            else
            {
                Config.AddLabel("SPREDICTION NOT LOADED");
            }

            if (AIOmode != 2)
            {
                Config.Add("supportMode", new CheckBox("Support Mode", false));
                Config.Add("comboDisableMode", new CheckBox("Disable auto-attack in combo mode", false));
                Config.Add("manaDisable", new CheckBox("Disable mana manager in combo"));
                Config.Add("collAA", new CheckBox("Disable auto-attack if Yasuo wall collision"));

                #region LOAD CHAMPIONS

                switch (Player.ChampionName)
                {
                    case "Anivia":
                        PortAIO.Champion.Anivia.Anivia.LoadOKTW();
                        break;
                    case "Annie":
                        PortAIO.Champion.Annie.Program.LoadOKTW();
                        break;
                    case "Ashe":
                        PortAIO.Champion.Ashe.Program.LoadOKTW();
                        break;
                    case "Braum":
                        PortAIO.Champion.Braum.Program.LoadOKTW();
                        break;
                    case "Caitlyn":
                        new Caitlyn().LoadOKTW();
                        break;
                    case "Ekko":
                        PortAIO.Champion.Ekko.Program.LoadOKTW();
                        break;
                    case "Ezreal":
                        Ezreal.LoadOKTW();
                        break;
                    case "Graves":
                        Graves.LoadOKTW();
                        break;
                    case "Jayce":
                        OneKeyToWin_AIO_Sebby.Champions.Jayce.LoadOKTW();
                        break;
                    case "Jinx":
                        new Jinx().LoadOKTW();
                        break;
                    case "Karthus":
                        Karthus.LoadOKTW();
                        break;
                    case "Lucian":
                        Lucian.LoadOKTW();
                        break;
                    case "MissFortune":
                        MissFortune.LoadOKTW();
                        break;
                    case "Malzahar":
                        Malzahar.LoadOKTW();
                        break;
                    case "Orianna":
                        Orianna.LoadOKTW();
                        break;
                    case "Sivir":
                        Sivir.LoadOKTW();
                        break;
                    case "Twitch":
                        Twitch.LoadOKTW();
                        break;
                    case "Syndra":
                        new OneKeyToWin_AIO_Sebby.Champions.Syndra().LoadOKTW();
                        break;
                    case "Velkoz":
                        Velkoz.LoadOKTW();
                        break;
                    case "Xerath":
                        Xerath.LoadOKTW();
                        break;
                    case "Swain":
                        Swain.LoadOKTW();
                        break;
                    case "Urgot":
                        Urgot.LoadOKTW();
                        break;
                    case "Ahri":
                        Ahri.LoadOKTW();
                        break;
                    case "Thresh":
                        Thresh.LoadOKTW();
                        break;
                    case "Brand":
                        Brand.LoadOKTW();
                        break;
                    case "Blitzcrank":
                        Blitzcrank.LoadOKTW();
                        break;
                    case "Corki":
                        Corki.LoadOKTW();
                        break;
                    case "Darius":
                        Darius.LoadOKTW();
                        break;
                    case "Evelynn":
                        OneKeyToWin_AIO_Sebby.Champions.Evelynn.LoadOKTW();
                        break;
                    case "Jhin":
                        Jhin.LoadOKTW();
                        break;
                    case "Kindred":
                        Kindred.LoadOKTW();
                        break;
                    case "KogMaw":
                        OneKeyToWin_AIO_Sebby.KogMaw.LoadOKTW();
                        break;
                    case "Lux":
                        Lux.LoadOKTW();
                        break;
                    case "Morgana":
                        Morgana.LoadOKTW();
                        break;
                    case "Quinn":
                        Quinn.LoadOKTW();
                        break;
                    case "TwistedFate":
                        OneKeyToWin_AIO_Sebby.Champions.TwistedFate.LoadOKTW();
                        break;
                    case "Varus":
                        new OneKeyToWin_AIO_Sebby.Champions.Varus().LoadOKTW();
                        break;
                }
            }

            #endregion

            foreach (var hero in HeroManager.Enemies)
            {
                if (hero.IsEnemy && hero.Team != Player.Team)
                {
                    Enemies.Add(hero);
                    if (IsJungler(hero))
                        jungler = hero;
                }
            }

            foreach (var hero in HeroManager.Allies)
            {
                if (hero.IsAlly && hero.Team == Player.Team)
                    Allies.Add(hero);
            }

            if (AIOmode != 1)
            {
                new OKTWward().LoadOKTW();
                new OKTWtracker().LoadOKTW();
            }

            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            Drawing.OnDraw += OnDraw;
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Player.IsDead || getSliderItem("PredictionMODE") != 4 || (!IsCharging && !Orbwalker.CanMove))
            {
                return;
            }
            if (_lastChargeTime == 0 && IsCharging)
            {
                _lastChargeTime = Core.GameTickCount;
            }
            else if (_lastChargeTime > 0 && !IsCharging)
            {
                _lastChargeTime = 0;
            }
        }

        public static void debug(string msg)
        {
            if (getCheckBoxItem("debug"))
            {
                Console.WriteLine(msg);
            }
            if (getCheckBoxItem("debugChat"))
            {
                Chat.Print(msg);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            tickIndex++;

            if (tickIndex > 4)
                tickIndex = 0;

            if (!LagFree(0))
                return;
        }

        public static bool LagFree(int offset)
        {
            if (tickIndex == offset)
                return true;
            return false;
        }

        private static void OnDraw(EventArgs args)
        {
            if (AIOmode == 1)
                return;

            if (Game.Time - DrawSpellTime < 0.5 && getCheckBoxItem("debugPred") && getSliderItem("PredictionMODE") == 1)
            {
                if (DrawSpell.Type == SkillshotType.SkillshotLine)
                    OktwCommon.DrawLineRectangle(DrawSpellPos.CastPosition, Player.Position, (int)DrawSpell.Width, 1,
                        Color.DimGray);
                if (DrawSpell.Type == SkillshotType.SkillshotCircle)
                    Render.Circle.DrawCircle(DrawSpellPos.CastPosition, DrawSpell.Width, Color.DimGray, 1);

                drawText("Aiming " + DrawSpellPos.Hitchance,
                    Player.Position.LSExtend(DrawSpellPos.CastPosition, 400), Color.Gray);
            }
        }
        private static int _lastChargeTime;

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (AIOmode == 2)
                return;

            if (Combo && getCheckBoxItem("comboDisableMode"))
            {
                var t = (AIHeroClient)args.Target;
                if (4 * Player.LSGetAutoAttackDamage(t) < t.Health - OktwCommon.GetIncomingDamage(t) &&
                    !t.HasBuff("luxilluminatingfraulein") && !Player.HasBuff("sheen") && !Player.HasBuff("Mastery6261"))
                    args.Process = false;
            }

            if (!Player.IsMelee && OktwCommon.CollisionYasuo(Player.ServerPosition, args.Target.Position) &&
                getCheckBoxItem("collAA"))
            {
                args.Process = false;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && getCheckBoxItem("supportMode"))
            {
                if (args.Target.Type == GameObjectType.obj_AI_Minion) args.Process = false;
            }
        }

        public static void drawText(string msg, Vector3 Hero, Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - msg.Length * 5, wts[1] + weight, color, msg);
        }

        public static void Cast(SpellSlot slot)
        {
            var first = _spells.FirstOrDefault(spell => spell.Slot == slot && (string.IsNullOrEmpty(spell.SpellName) || string.Equals(Player.Spellbook.GetSpell(slot).Name, spell.SpellName, StringComparison.CurrentCultureIgnoreCase)));
            if (first != null)
            {
                var allowedCollisionCount = int.MaxValue;
                if (first.Collisions.Contains(CollisionType.AiHeroClient))
                {
                    allowedCollisionCount = 0;
                }
                else if (first.Collisions.Contains(CollisionType.ObjAiMinion))
                {
                    allowedCollisionCount = -1;
                }
                var collidesWithWall = first.Collisions.Contains(CollisionType.YasuoWall);
                SpellBase spell = null;
                switch (first.Type)
                {
                    case EloBuddy.SDK.Spells.SpellType.Self:
                        spell = new SpellBase(slot, SpellType.Self, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Circle:
                        spell = new SpellBase(slot, SpellType.Circular, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Line:
                        spell = new SpellBase(slot, SpellType.Linear, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Cone:
                        spell = new SpellBase(slot, SpellType.Cone, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Ring:
                        break;
                    case EloBuddy.SDK.Spells.SpellType.MissileLine:
                        spell = new SpellBase(slot, SpellType.Linear, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.MissileAoe:
                        spell = new SpellBase(slot, SpellType.Circular, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                }
                if (spell != null)
                {
                    if (first.Chargeable)
                    {
                        if (IsCharging)
                        {
                            var percentageGrowth = Math.Min(1 / 1000f * (Core.GameTickCount - _lastChargeTime - first.CastRangeGrowthStartTime) / first.CastRangeGrowthDuration, 1);
                            spell.Range = (first.CastRangeGrowthMax - first.CastRangeGrowthMin) * percentageGrowth + first.CastRangeGrowthMin;
                            spell.ReleaseCast();
                        }
                        else
                        {
                            spell.StartCast();
                        }
                    }
                    else if (!IsCharging)
                    {
                        spell.Cast();
                    }
                }
            }
        }

        private static bool IsCharging
        {
            get { return Player.Spellbook.IsCharging; }
        }

        public static void CastSpell(Spell QWER, Obj_AI_Base target)
        {
            if (!target.IsVisible || !target.IsHPBarRendered)
            {
                return;
            }
            if (getSliderItem("PredictionMODE") == 0)
            {
                if (getSliderItem("HitChance") == 0)
                {
                    var pred = QWER.GetPrediction(target);
                    if (pred.Hitchance >= LeagueSharp.Common.HitChance.VeryHigh)
                        QWER.Cast(pred.CastPosition);
                    return;
                }
                else if (getSliderItem("HitChance") == 1)
                {
                    var pred = QWER.GetPrediction(target);
                    if (pred.Hitchance >= LeagueSharp.Common.HitChance.High)
                        QWER.Cast(pred.CastPosition);
                    return;
                }
                else if (getSliderItem("HitChance") == 2)
                {
                    var pred = QWER.GetPrediction(target);
                    if (pred.Hitchance >= LeagueSharp.Common.HitChance.Medium)
                        QWER.Cast(pred.CastPosition);
                    return;
                }
                return;
            }

            if (getSliderItem("PredictionMODE") == 1)
            {
                SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (QWER.Type == SkillshotType.SkillshotCircle)
                {
                    CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                    aoe2 = true;
                }

                if (QWER.Width > 80 && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new SebbyLib.Prediction.PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if (getSliderItem("HitChance") == 0)
                {
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                        QWER.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }

                }
                else if (getSliderItem("HitChance") == 1)
                {
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                        QWER.Cast(poutput2.CastPosition);

                }
                else if (getSliderItem("HitChance") == 2)
                {
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                        QWER.Cast(poutput2.CastPosition);
                }
                if (Game.Time - DrawSpellTime > 0.5)
                {
                    DrawSpell = QWER;
                    DrawSpellTime = Game.Time;

                }
                DrawSpellPos = poutput2;
                return;
            }

            if (getSliderItem("PredictionMODE") == 2)
            {
                if (target is AIHeroClient && target.IsValid)
                {
                    var t = target as AIHeroClient;
                    if (getSliderItem("HitChance") == 0)
                    {
                        var pred = QWER.GetSPrediction(t);
                        if (pred.HitChance >= LeagueSharp.Common.HitChance.VeryHigh)
                            QWER.Cast(pred.CastPosition);
                        return;
                    }
                    else if (getSliderItem("HitChance") == 1)
                    {
                        var pred = QWER.GetSPrediction(t);
                        if (pred.HitChance >= LeagueSharp.Common.HitChance.High)
                            QWER.Cast(pred.CastPosition);
                        return;
                    }
                    else if (getSliderItem("HitChance") == 2)
                    {
                        var pred = QWER.GetSPrediction(t);
                        if (pred.HitChance >= LeagueSharp.Common.HitChance.Medium)
                            QWER.Cast(pred.CastPosition);
                        return;
                    }
                }
                else
                {
                    QWER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.High);
                }
                return;
            }

            if (getSliderItem("PredictionMODE") == 3)
            {
                SebbyLib.Movement.SkillshotType CoreType2 = SebbyLib.Movement.SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (QWER.Width > 80 && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new SebbyLib.Movement.PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = SebbyLib.Movement.Prediction.GetPrediction(predInput2);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if (getSliderItem("HitChance") == 0)
                {
                    if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.VeryHigh)
                        QWER.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= SebbyLib.Movement.HitChance.High)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }

                }
                else if (getSliderItem("HitChance") == 1)
                {
                    if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.High)
                        QWER.Cast(poutput2.CastPosition);

                }
                else if (getSliderItem("HitChance") == 2)
                {
                    if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.Medium)
                        QWER.Cast(poutput2.CastPosition);
                }
                return;
            }

            if (getSliderItem("PredictionMODE") == 4)
            {
                if (target is AIHeroClient && target.IsValid)
                {
                    var t = target as AIHeroClient;
                    Cast(QWER.Slot);
                }
                else
                {
                    Cast(QWER.Slot);
                }
            }
        }
    }
}