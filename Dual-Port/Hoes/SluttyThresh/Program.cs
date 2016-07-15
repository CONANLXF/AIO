using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LCItems = LeagueSharp.Common.Items;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

 namespace Slutty_Thresh
{
    internal class SluttyThresh : MenuConfig
    {
        public const string ChampName = "Thresh";
        private static readonly AIHeroClient Player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        private static SpellSlot Ignite;
        private static SpellSlot FlashSlot;
        public static float FlashRange = 450f;

        public static Dictionary<string, string> channeledSpells = new Dictionary<string, string>();
        private static int elastattempt;
        private static int elastattemptin;

        public static void OnLoad()
        {
            if (Player.ChampionName != ChampName)
                return;

            Q = new Spell(SpellSlot.Q, 1080);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 500);
            R = new Spell(SpellSlot.R, 400);

            Q.SetSkillshot(0.4f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 50f, 2200f, false, SkillshotType.SkillshotCircle);

            FlashSlot = Player.GetSpellSlot("SummonerFlash");

            CreateMenuu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += ThreshInterruptableSpell;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnGapCloser;
            AIHeroClient.OnProcessSpellCast += Game_ProcessSpell;

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

        private static void ThrowLantern()
        {
            if (W.IsReady())
            {
                var NearAllies = Player.GetAlliesInRange(W.Range).Where(x => !x.IsMe).Where(x => !x.IsDead).Where(x => x.LSDistance(Player.Position) <= W.Range + 250).FirstOrDefault();
                if (NearAllies == null) return;
                W.Cast(NearAllies.Position);
            }
        }
        

        private static void Game_OnUpdate(EventArgs args)
        {
            var Etarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (getKeyBindItem(lanternMenu, "ThrowLantern"))
            {
                ThrowLantern();
            }

            if (getKeyBindItem(comboMenu, "FlayPush") || getKeyBindItem(comboMenu, "FlayPull"))
            {
                Orbwalker.MoveTo(Game.CursorPos);
            }

            if (getKeyBindItem(comboMenu, "FlayPush") && Etarget != null &&
                E.IsReady())
            {
                Push(Etarget);
            }

            if (getKeyBindItem(comboMenu, "FlayPull") && Etarget != null &&
                E.IsReady())
            {
                Pull(Etarget);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }

            AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target != null)
            {
                if (target.ChampionName == "Katarina")
                {
                    if (target.HasBuff("katarinaereduction"))
                    {
                        if (target.LSIsValidTarget(E.Range))
                        {
                            E.Cast(target.ServerPosition);
                            eattempt = Environment.TickCount;
                        }
                        if (Environment.TickCount - eattempt >= 90f + Game.Ping
                            && Q.IsReady())
                            Q.Cast(target.ServerPosition);
                    }
                }
            }


            if (getKeyBindItem(flashMenu, "qflash"))
                flashq();

            wcast();
            //  Itemusage();

        }

        //        private static void Itemusage()
        //        {
        //            var charm = Config.Item("charm").GetValue<bool>();
        //            var stun = Config.Item("stun").GetValue<bool>();
        //            var snare = Config.Item("snare").GetValue<bool>();
        //            var suppresion = Config.Item("suppression").GetValue<bool>();
        //            var taunt = Config.Item("taunt").GetValue<bool>();
        //
        //
        //            // var mikaelshp = Config.Item("mikaelshp").GetValue<Slider>().Value;
        //
        //            var mikael = ItemData.Mikaels_Crucible.GetItem();
        //            var locket = ItemData.Locket_of_the_Iron_Solari.GetItem();
        //            var mountain = ItemData.Face_of_the_Mountain.GetItem();
        //
        //            foreach (var hero in
        //                HeroManager.Allies.Where(x => !x.IsMe))
        //            {
        //                if (Config.Item("faceop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
        //                {
        //                    if (hero.HealthPercent <= Config.Item("facehp" + hero.ChampionName).GetValue<Slider>().Value)
        //                    {
        //                        if (hero.Distance(Player) >= 750f)
        //                            mountain.Cast(hero);
        //                    }
        //                }
        //            }
        //            foreach (var hero in
        //                HeroManager.Allies.Where(x => !x.IsMe))
        //            {
        //                if (Config.Item("locketop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
        //                {
        //                    if (hero.HealthPercent <= Config.Item("lockethp" + hero.ChampionName).GetValue<Slider>().Value)
        //                    {
        //                        if (hero.Distance(Player) >= 600)
        //                            locket.Cast();
        //                    }
        //                }
        //     }



        //
        //
        //
        //            foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe))
        //            {
        //                if (Config.Item("healmikaels" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
        //                {
        //                    if (hero.HasBuffOfType(BuffType.Stun)
        //                        && stun ||
        //                        hero.HasBuffOfType(BuffType.Suppression)
        //                        && suppresion ||
        //                        hero.HasBuffOfType(BuffType.Taunt)
        //                        && taunt ||
        //                        hero.HasBuffOfType(BuffType.Charm)
        //                        && charm ||
        //                        hero.HasBuffOfType(BuffType.Snare)
        //                        && snare
        //                        || hero.HasBuffOfType(BuffType.CombatDehancer))
        //                    {
        //                        if (hero.Distance(Player) <= 750f)
        //                            mikael.Cast(hero);
        //                    }
        //                }
        //            }
        //        }


        private static void wcast()
        {
            if (Player.ManaPercent < getSliderItem(lanternMenu, "manalant"))
                return;
            // AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (!W.IsReady()) return;
            foreach (var hero in Player.GetAlliesInRange(950).Where(hero => !hero.IsDead &&
                                                                            hero.HealthPercent <=
                                                                            getSliderItem(lanternMenu, "hpsettings" +
                                                                                        hero.ChampionName)
                                                                            && hero.Distance(Player) <= 900))
                if (getBoxItem(lanternMenu, "healop" + hero.ChampionName) == 0)
                {
                    if (hero.Distance(Player) <= 900)
                    {
                        W.Cast(hero.Position);
                    }
                }
        }

        static void Pull(Obj_AI_Base target)
        {
            var pos = target.Position.LSExtend(Player.Position, Player.LSDistance(target.Position) + 200);
            E.Cast(pos);
        }

        static void Push(Obj_AI_Base target)
        {
            var pos = target.Position.LSExtend(Player.Position, Player.LSDistance(target.Position) - 200);
            E.Cast(pos);
        }

        private static void Combo()
        {
            Ignite = Player.GetSpellSlot("summonerdot");
            var qSpell = getCheckBoxItem(qMenu, "useQ");
            var q2Spell = getCheckBoxItem(qMenu, "useQ1");
            var q2Slider = getSliderItem(qMenu, "useQ2");
            var qrange1 = getSliderItem(qMenu, "qrange");
            var rslider = getSliderItem(comboMenu, "rslider");
            var rSpell = getCheckBoxItem(comboMenu, "useR");
            var eSpell = getCheckBoxItem(comboMenu, "useE");
            // var wSpell = Config.Item("useW").GetValue<bool>();

            AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null)
                return;

            if (target.HasBuff("threshQ")
                || (Player.LSDistance(target) <= 650 && E.IsReady()))
                PortAIO.OrbwalkerManager.SetAttack(false);
            else
                PortAIO.OrbwalkerManager.SetAttack(true);

            if (target.HasBuff("threshQ"))
            {
                lastbuff = Environment.TickCount;
            }
            if (Q.IsReady()
                && (E.IsReady() || ObjectManager.Player.GetSpell(SpellSlot.E).Cooldown <= 3000f)
                && qSpell
                && !target.HasBuff("threshQ")
                && target.LSIsValidTarget(Q.Range)
                && target.LSDistance(Player) >= qrange1)
            {
                Q.Cast(target);
                lastq = Environment.TickCount;
            }

            if (q2Spell
                && target.HasBuff("threshQ"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(q2Slider, () => Q.Cast());

            }

            switch (getBoxItem(comboMenu, "combooptions"))
            {
                case 0:
                    if (target.LSIsValidTarget(E.Range)
                        && eSpell
                        && !target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned
                        && Environment.TickCount - lastq >= 40 + Game.Ping)
                    {
                        E.Cast(target.ServerPosition);
                        elastattempt = Environment.TickCount;
                    }
                    break;

                case 1:
                    if (target.LSIsValidTarget(E.Range)
                        && Environment.TickCount - lastq >= 40 + Game.Ping
                        && eSpell)
                        E.Cast(target.Position.LSExtend(Player.ServerPosition,
                            Vector3.Distance(target.Position, Player.Position) + 400));
                    elastattemptin = Environment.TickCount;
                    break;
            }

            if (rSpell
                && Player.LSCountEnemiesInRange(R.Range - 30) >= rslider
                && ((Environment.TickCount - elastattempt > 180f + Game.Ping)
                    || (Environment.TickCount - elastattemptin > 180f + Game.Ping)))
                R.Cast();
        }


        private static void flashq()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
                return;
            var x = target.Position.LSExtend(LeagueSharp.Common.Prediction.GetPrediction(target, 1).UnitPosition, FlashRange);
            switch (getBoxItem(flashMenu, "flashmodes"))
            {
                case 0:
                    Player.Spellbook.CastSpell(FlashSlot, x);
                    Q.Cast(x);
                    E.Cast(Player.Position);
                    break;

                /*
            case 1:
            E.Cast(Player.Position);
            Q.Cast(x);
            Player.Spellbook.CastSpell(FlashSlot, x);
                break;
                 */

                case 1:
                    Player.Spellbook.CastSpell(FlashSlot, x);
                    Q.Cast(x);
                    break;
            }
        }

        /*
        private static void Mixed()
        {
            throw new NotImplementedException();
        }
         */

        private static void LaneClear()
        {
            var elchSpell = getCheckBoxItem(laneMenu, "useelch");
            //  var elchSlider = Config.Item("elchslider").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (minionCount == null)
                return;

            foreach (var minion in minionCount)
            {
                if (elchSpell
                    && minion.LSIsValidTarget(E.Range)
                    && E.IsReady())
                {
                    E.Cast(minion.Position);
                }
            }
        }


        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly
                || gapcloser.Sender.IsMe)
                return;

            if (E.IsInRange(gapcloser.Start))
                E.Cast(Player.Position.LSExtend(gapcloser.Sender.Position, 400));
        }


        public static void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            //            if (!hero.IsMe)
            //                return;
            //           // var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            //            if ((args.SData.Name == "threshqinternal" || args.SData.Name == "ThreshQ")
            //                && Config.Item("autolantern").GetValue<bool>()
            //                && W.IsReady())
            //            {
            //                foreach (var heros in
            //                    HeroManager.Allies.Where(x => !x.IsMe
            //                                                  && x.Distance(Player) <= W.Range))
            //                {
            //                        W.Cast(heros.Position);
            //                }
            //            }
        }


        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (!sender.IsEnemy)
                return;

            if (sender.NetworkId == target.NetworkId)
            {
                if (E.IsReady()
                   && E.IsInRange(sender.ServerPosition))
                {
                    E.Cast(Player.Position.LSExtend(sender.Position, 400));
                }
            }

        }



        private static void ThreshInterruptableSpell(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady()
                && E.IsInRange(sender)
                && getCheckBoxItem(miscMenu, "useE2I"))
                E.Cast(sender.ServerPosition);
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (!getCheckBoxItem(drawMenu, "Draw"))
                return;

            var qDraw = getCheckBoxItem(drawMenu, "qDraw");
            var eDraw = getCheckBoxItem(drawMenu, "eDraw");
            var wDraw = getCheckBoxItem(drawMenu, "wDraw");
            var qfDraw = getCheckBoxItem(drawMenu, "qfDraw");

            if (qDraw
                && Q.Level > 0)
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Green);

            if (qfDraw
                && Q.IsReady()
                && FlashSlot.IsReady())
                Render.Circle.DrawCircle(Player.Position, 1440, Color.Red);

            if (wDraw
                && W.Level > 0)
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Black);

            if (eDraw
                && E.Level > 0)
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Gold);

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
                return;

            if (Q.IsReady()
                && FlashSlot.IsReady()
                && target.Distance(Player) <= Q.Range + 450
                && target.Distance(Player) >= Q.Range - 200)
            {
                var heroPosition = Drawing.WorldToScreen(Player.Position);
                var textDimension = Drawing.GetTextEntent("Stunnable!", 20);
                Drawing.DrawText(heroPosition.X - textDimension.Width, heroPosition.Y - textDimension.Height,
                    Color.DarkGreen, "Can Flash Q!");
            }
            else
            {
                var heroPosition = Drawing.WorldToScreen(Player.Position);
                var textDimension = Drawing.GetTextEntent("Stunnable!", 20);
                Drawing.DrawText(heroPosition.X - textDimension.Width, heroPosition.Y - textDimension.Height,
                    Color.Red, "Can't Flash Q!");
            }
        }

        public static int lastq { get; set; }

        public static int eattempt { get; set; }

        public static int lastbuff { get; set; }
    }
}