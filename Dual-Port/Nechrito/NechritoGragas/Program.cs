using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;
using EloBuddy.SDK;

 namespace Nechrito_Gragas
{
    class Program
    {
        public static readonly int[] BlueSmite = { 3706, 1400, 1401, 1402, 1403 };

        public static readonly int[] RedSmite = { 3715, 1415, 1414, 1413, 1412 };

        public static GameObject GragasQ;

        public static AIHeroClient Player => ObjectManager.Player;

        private static readonly HpBarIndicator Indicator = new HpBarIndicator();

        public static void OnGameLoad()
        {
            if (Player.ChampionName != "Gragas") return;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Gragas</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 3 (Date: 26/6-16)</font></b>");

            MenuConfig.LoadMenu();
            Spells.Initialise();

            Game.OnUpdate += OnTick;

            Obj_AI_Base.OnSpellCast += OnDoCast;

            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;

            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                GragasQ = null;
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                GragasQ = sender;
            }
        }
        
        private static void OnTick(EventArgs args)
        {
            SmiteJungle();
            SmiteCombo();
            Killsteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Mode.ComboLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Mode.JungleLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Mode.HarassLogic();
            }

        }
        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.Spellbook.IsAutoAttacking) return;

            if (args.Target is Obj_AI_Minion)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    var minions = MinionManager.GetMinions(Player.ServerPosition, 600);
                    {
                        if (minions == null)
                            return;

                        foreach (var m in minions)
                        {
                            if (Spells.E.IsReady() && MenuConfig.LaneE)
                            {
                                if (m.Health < Spells.E.GetDamage(m))
                                {
                                    Spells.E.Cast(GetCenterMinion());
                                }
                            }

                            if (Spells.Q.IsReady() && MenuConfig.LaneQ)
                            {
                                if (Program.GragasQ == null)
                                {
                                    Spells.Q.Cast(GetCenterMinion(), true);
                                }
                                if (Program.GragasQ != null && m.LSDistance(Program.GragasQ.Position) <= 250 && m.Health < Spells.Q.GetDamage(m))
                                {
                                    Spells.Q.Cast(true);
                                }
                            }
                            if (m.LSDistance(Player) <= 250f)
                            {
                                if (Spells.W.IsReady() && MenuConfig.LaneW)
                                {
                                    Spells.W.Cast();
                                }
                            }
                        }
                    }
                }
            }
        }


        public static Obj_AI_Base GetCenterMinion()
        {
            var minionposition = MinionManager.GetMinions(300 + Spells.Q.Range).Select(x => x.Position.LSTo2D()).ToList();
            var center = MinionManager.GetBestCircularFarmLocation(minionposition, 250, 300 + Spells.Q.Range);

            return center.MinionsHit >= 4
                ? MinionManager.GetMinions(1000).OrderBy(x => x.LSDistance(center.Position)).FirstOrDefault()
                : null;
        }
        private static void Killsteal()
        {
            if (Spells.Q.IsReady() && Spells.R.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells.Q.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells.Q.GetDamage(target) + Spells.Q.GetDamage(target))
                    {
                        var pos = Spells.R.GetSPrediction(target).CastPosition + 60;

                        Spells.Q.Cast(Mode.rpred(target));
                        Spells.R.Cast(Mode.rpred(target));
                    }
                }
            }

            if (Spells.Q.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells.Q.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells.Q.GetDamage(target))
                    {
                        var pos = Spells.Q.GetSPrediction(target).CastPosition;
                        Spells.Q.Cast(Mode.rpred(target));
                        Spells.Q.Cast(pos);
                    }
                }
            }

            if (Spells.E.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells.E.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells.E.GetDamage(target))
                    {
                        var pos = Spells.E.GetSPrediction(target).CastPosition;
                        Spells.E.Cast(pos);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead || !MenuConfig.prediction) return;
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);


            var Target = TargetSelector.SelectedTarget;

            if (Target != null && !Target.IsDead)
            {
                Render.Circle.DrawCircle(Mode.rpred(Target), 100, System.Drawing.Color.GhostWhite);
                Render.Circle.DrawCircle(Mode.qpred(Target), 100, System.Drawing.Color.Blue);
            }
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (MenuConfig.dind)
                {
                    var lethal = Spells.R.IsReady() && Dmg.IsLethal(enemy)
                        ? new ColorBGRA(0, 255, 0, 120)
                        : new ColorBGRA(255, 255, 0, 120);

                    Indicator.unit = enemy;
                    Indicator.drawDmg(Dmg.ComboDmg(enemy), lethal);
                }
            }
        }
        protected static void SmiteCombo()
        {
            if (BlueSmite.Any(id => Items.HasItem(id)))
            {
                Spells.Smite = Player.GetSpellSlot("s5_summonersmiteplayerganker");
                return;
            }

            if (RedSmite.Any(id => Items.HasItem(id)))
            {
                Spells.Smite = Player.GetSpellSlot("s5_summonersmiteduel");
                return;
            }

            Spells.Smite = Player.GetSpellSlot("summonersmite");
        }

        protected static void SmiteJungle()
        {
            foreach (var minion in MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral))
            {
                var damage = Player.Spellbook.GetSpell(Spells.Smite).State == SpellState.Ready
                    ? (float)Player.GetSummonerSpellDamage(minion, LeagueSharp.Common.Damage.SummonerSpell.Smite)
                    : 0;
                if (minion.LSDistance(Player.ServerPosition) <= 550)
                {
                    if ((minion.CharData.BaseSkinName.Contains("Dragon") || minion.CharData.BaseSkinName.Contains("Baron")))
                    {
                        if (damage >= minion.Health)
                            Player.Spellbook.CastSpell(Spells.Smite, minion);
                    }
                }

            }

        }
    }
}
