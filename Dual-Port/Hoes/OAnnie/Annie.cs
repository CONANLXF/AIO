using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;

using TargetSelector = PortAIO.TSManager; namespace OAnnie
{
    internal class Annie : MenuConfig
    {
        public const string ChampName = "Annie";
        public static Spell Q, W, E, R;
        public static SpellSlot Ignite;
        public static SpellSlot FlashSlot;
        public static float FlashRange = 450f;
        public static AIHeroClient Player = ObjectManager.Player;

        /// <summary>
        ///     Passive Buff
        /// </summary>
        #region Passive buff
        public static int GetPassiveBuff
        {
            get
            {
                var data = Player.Buffs.FirstOrDefault(b => b.DisplayName == "Pyromania");
                // Does not use C# v6+ T_T
                // return data?.Count ?? 0;
                return data != null ? data.Count : 0;
            }
        }

        #endregion

        /// <summary>
        ///     When Game Loads
        /// </summary>
        /// <param name="args"></param>
        #region called when loaded
        public static void Load()
        {
            if (Player.ChampionName != ChampName)
                return;

            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 600);
            GlobalManager.DamageToUnit = GlobalManager.GetComboDamage;
            W.SetSkillshot(0.5f, 250f, float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.2f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            CreateMenu();
            FlashSlot = Player.GetSpellSlot("SummonerFlash");
            Ignite = Player.GetSpellSlot("SummonerDot");
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += DrawManager.OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            // CustomEvents.Unit.OnDash += Unit_OnDash;
            //  AntiGapcloser.OnEnemyGapcloser += OnGapCloser;
        }


        #endregion

        
        /// <summary>
        /// Gapcloser
        /// </summary>
        /// <param name="gapcloser"></param>
        #region GapCloser
        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.IsEnemy)
                return;
            var gap = GlobalManager.getCheckBoxItem(miscMenu, "miscMenu.qwgap");
            if (!gap)
                return;
            if (Player.HasBuff("pyromania_particle"))
            {
                if (Q.IsReady()
                    && Q.IsInRange(gapcloser.Start))
                {
                    Q.Cast(gapcloser.Start);
                }

                if (W.IsReady() && W.IsInRange(gapcloser.Start))
                {
                    W.Cast(gapcloser.Start);
                }
            }
        }

        #endregion



        /// <summary>
        /// Dash
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        #region ondash
        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var dash = GlobalManager.getCheckBoxItem(miscMenu, "miscMenu.qwdash");
            if (!dash)
                return;
            if (sender == null)
                return;
            if (!sender.IsEnemy)
                return;

            if (sender.NetworkId != target.NetworkId) return;


            if (Player.HasBuff("pyromania_particle"))
            {
                if (Q.IsReady()
                    && Q.IsInRange(sender.ServerPosition))
                {
                    Q.Cast(sender);
                }

                if (W.IsReady() && W.IsInRange(sender.ServerPosition))
                {
                    W.Cast(sender);
                }
            }
        }
         
         

        #endregion
         

        /// <summary>
        ///     E On auto attack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        #region Process spell
        private static void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!GlobalManager.getCheckBoxItem(comboMenu, "comboMenu.emenu.eaa")) return;
            if (sender.IsEnemy
                && sender.IsChampion()
                && args.SData.IsAutoAttack()
                && args.Target.IsMe)
            {
                E.Cast();
            }
        }

        #endregion
        
        /// <summary>
        ///     Every tick update
        /// </summary>
        /// <param name="args"></param>
        #region On Update
        private static void OnGameUpdate(EventArgs args)
        {
            /*
            foreach (var buff in Player.Buffs)
            {
                if (buff.Name.ToLower() != "odinplayerbuff"
                    || buff.Name.ToLower() != "kalistacoopstrikeally"
                    || buff.Name != "pyromania_marker")
                    Game.PrintChat(buff.Name.ToLower());
            }
             */

            if (Player.LSIsRecalling())
                return;

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo();
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Mixed();
            }

            if (PortAIO.OrbwalkerManager.isLastHitActive)
            {
                LastHit();
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Laneclear();
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                JungleClear();
            }
           
            var target = TargetSelector.GetTarget(Q.Range + 200, DamageType.Magical);
            if (target != null)
            {
                if (Player.LSDistance(target) > Q.Range &&
                    (( !Q.IsReady() || !W.IsReady() || !R.IsReady())) )
                {
                    PortAIO.OrbwalkerManager.SetAttack(false);
                }
                else
                {
                    PortAIO.OrbwalkerManager.SetAttack(true);
                }
            }

            Parostack();
            KillSteal();

            if (!GlobalManager.getKeyBindItem(tibbersMenu, "tibmove"))
            {
                Tibbers.Tibbersmove();
            }
            else if (GlobalManager.getKeyBindItem(tibbersMenu, "tibmove"))
            {
                MoveTibbers();
            }

            if (GlobalManager.getKeyBindItem(flashMenu, "comboMenu.flashmenu.flashr")
                || GlobalManager.getKeyBindItem(flashMenu, "comboMenu.flashmenu.flasher"))
            {
                TibbersFlash();
            }        
             
        }

        #endregion

        /// <summary>
        ///     Kill Steal
        /// </summary>
        #region Kill Steal
        private static void KillSteal()
        {
            var ks = GlobalManager.getCheckBoxItem(ksMenu, "killstealMenu.ks");
            var useq = GlobalManager.getCheckBoxItem(ksMenu, "killstealMenu.q");
            var usew = GlobalManager.getCheckBoxItem(ksMenu, "killstealMenu.w");
            var user = GlobalManager.getCheckBoxItem(ksMenu, "killstealMenu.r");

            if (!ks)
                return;

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
                return;

            if (useq && target.LSIsValidTarget(Q.Range) && Q.IsReady() && Q.GetDamage(target) >= target.Health)
            {
                Q.Cast(target);
            }

            if (usew && target.LSIsValidTarget(W.Range) && W.IsReady() && W.GetDamage(target) >= target.Health)
            {
                W.Cast(target);
            }

            if (user && target.LSIsValidTarget(R.Range) && R.IsReady() && R.GetDamage(target) >= target.Health)
            {
                Q.Cast(target);
            }
        }

        #endregion

        /// <summary>
        ///     passive stack
        /// </summary>
        #region Pyro Stacking
        private static void Parostack()
        {
            var usee = GlobalManager.getCheckBoxItem(comboMenu, "comboMenu.passivemanagement.e.stack");
            var usew = GlobalManager.getCheckBoxItem(comboMenu, "comboMenu.passivemanagement.e.stack");

            if (Player.HasBuff("pyromania_particle"))
                return;

            if (usee && E.IsReady())
            {
                E.Cast();
            }

            if (usew && W.IsReady())
            {
                W.Cast(Game.CursorPos);
            }
        }

        #endregion

        /// <summary>
        ///     jungleclear
        /// </summary>
        #region jungle clear
        private static void JungleClear()
        {
            var useq = GlobalManager.getCheckBoxItem(laneMenu, "clearMenu.jungleMenu.useq");
            var usew = GlobalManager.getCheckBoxItem(laneMenu, "clearMenu.jungleMenu.useq");
            var usee = GlobalManager.getCheckBoxItem(laneMenu, "clearMenu.jungleMenu.useq");

            var minionCount =
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All,
                    MinionTeam.Neutral).FirstOrDefault();

            if (minionCount == null)
                return;
            var minion = minionCount;

            if (useq && Q.IsReady() && minion.LSIsValidTarget(Q.Range))
            {
                Q.Cast(minion);
            }

            if (usew && W.IsReady() && minion.LSIsValidTarget(W.Range))
            {
                W.Cast(minion);
            }

            if (usee && E.IsReady() && minion.LSIsValidTarget(W.Range))
            {
                E.Cast();
            }
        }

        #endregion

        /// <summary>
        ///     Lane clear
        /// </summary>
        #region Lane Clear
        private static void Laneclear()
        {
            var useq = GlobalManager.getCheckBoxItem(laneMenu, "clearMenu.laneMenu.useq");
            var usestun = GlobalManager.getCheckBoxItem(laneMenu, "clearMenu.laneMenu.keepstun");
            var useql = GlobalManager.getCheckBoxItem(laneMenu, "clearMenu.laneMenu.useqlast");
            var usew = GlobalManager.getCheckBoxItem(laneMenu, "clearMenu.laneMenu.usew");
            var usewslider = GlobalManager.getSliderItem(laneMenu, "clearMenu.laneMenu.usewslider");
            var minMana = GlobalManager.getSliderItem(laneMenu, "clearMenu.laneMenu.manaslider");
            if (usestun && Player.HasBuff("pyromania_particle"))
                return;
            var minionCount =
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly).FirstOrDefault();

            if (minionCount == null)
                return;
            var minion = minionCount;
            var minionhp = minionCount.Health;


            if (useql && Q.IsReady() && minion.LSIsValidTarget(Q.Range) && minionhp <= Q.GetDamage(minion) && minionhp > Player.LSGetAutoAttackDamage(minion))
            {
                Q.Cast(minion);
            }

            if (useq && Q.IsReady() && minion.LSIsValidTarget(Q.Range))
            {
                Q.Cast(minion);
            }

            var wminion =
                MinionManager.GetMinions(Player.Position, W.Range, MinionTypes.All,
                    MinionTeam.NotAlly);
            if (wminion == null)
                return;

            var pred = W.GetLineFarmLocation(wminion);

            foreach (var mincount in wminion)
            {
                if (usew && W.IsReady() && minion.LSIsValidTarget(W.Range) && pred.MinionsHit >= usewslider &&
                    Player.ManaPercent >= minMana)
                {
                    W.Cast(mincount);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Last hit
        /// </summary>
        #region Last Hit
        private static void LastHit()
        {
            var useql = GlobalManager.getCheckBoxItem(laneMenu, "clearMenu.lastMenu.useqlast");
            var usestun = GlobalManager.getCheckBoxItem(laneMenu, "clearMenu.lastMenu.keepstun");
            if (usestun && Player.HasBuff("pyromania_particle"))
                return;
            var minionCount =
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly).FirstOrDefault();

            if (minionCount == null)
                return;

            var minion = minionCount;
            var minionhp = minion.Health;

            if (minionhp <= Q.GetDamage(minion) && useql && Q.IsReady())
            {
                Q.Cast(minion);
            }
        }

        #endregion

        /// <summary>
        ///     Tibber Flash Modes
        /// </summary>
        #region Flash
        private static void TibbersFlash()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var target = TargetSelector.GetTarget(R.Range + FlashRange, DamageType.Magical);

            if (target == null)
                return;

            if (!R.IsReady())
            {
                Combo();
            }

            var x = target.Position.LSExtend(LeagueSharp.Common.Prediction.GetPrediction(target, 1).UnitPosition, FlashRange);
            var predpos = R.GetPrediction(target);
            if (GlobalManager.getKeyBindItem(flashMenu, "comboMenu.flashmenu.flashr"))
            {
                if (Player.HasBuff("pyromania_particle"))
                {
                        Player.Spellbook.CastSpell(FlashSlot, x);
                        R.Cast(predpos.CastPosition);
                }
            }

            if (GlobalManager.getKeyBindItem(flashMenu, "comboMenu.flashmenu.flasher"))
            {
                if (GetPassiveBuff == 3)
                {
                        Player.Spellbook.CastSpell(FlashSlot, x);
                        E.Cast();
                }
                if (Player.HasBuff("pyromania_particle"))
                {
                    R.Cast(predpos.CastPosition);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Mixed Mode
        /// </summary>
        #region Mixed Mode
        private static void Mixed()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
                return;

            var useq = GlobalManager.getCheckBoxItem(harassMenu, "harrasMenu.useq");
            var usew = GlobalManager.getCheckBoxItem(harassMenu, "harrasMenu.usew");

            if (useq && Q.IsReady() && target.LSIsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }

            if (usew && W.IsReady() && target.LSIsValidTarget(W.Range))
            {
                Q.Cast(target);
            }
        }

        #endregion

        /// <summary>
        ///     Combo
        /// </summary>


        #region Combo
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
                return;

            var useq = GlobalManager.getCheckBoxItem(comboMenu, "comboMenu.useq");
            var usew = GlobalManager.getCheckBoxItem(comboMenu, "comboMenu.usew");
            var usee = GlobalManager.getCheckBoxItem(comboMenu, "comboMenu.usee");
            var user = GlobalManager.getCheckBoxItem(comboMenu, "comboMenu.user");
            var usersmart = GlobalManager.getCheckBoxItem(comboMenu, "comboMenu.user.smart");
            var useebefore = GlobalManager.getCheckBoxItem(comboMenu, "comboMenu.passivemanagement.e.before");
            var userslider = GlobalManager.getSliderItem(comboMenu, "comboMenu.user.Slider");

            if (Ignite.IsReady() && (target.Health <= Q.GetDamage(target) +Player.LSGetAutoAttackDamage(target)))
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }

            if (Q.IsReady() && target.LSIsValidTarget(Q.Range) && useq)
            {
                if (useebefore)
                {
                    if (GetPassiveBuff == 3 && E.IsReady() && !Player.HasBuff("summonerteleport"))
                    {
                        E.Cast();
                    }

                    if (!R.IsReady())
                    {
                        Q.Cast(target);
                    }
                    else
                    {
                        if (Player.HasBuff("pyromania_particles") && usersmart)
                            return;
                        Q.Cast(target);
                    }
                }

                if (!R.IsReady())
                {
                    Q.Cast(target);
                }
                else
                {
                    if (Player.HasBuff("pyromania_particles") && usersmart)
                        return;
                    Q.Cast(target);
                }
            }

            if (usee && E.IsReady() && !Player.HasBuff("pyromania_particles") && !Player.HasBuff("summonerteleport"))
            {
                switch (GlobalManager.getBoxItem(comboMenu, "comboMenu.emenu.emode"))
                {
                    case 0:
                        if (GetPassiveBuff == 3)
                            E.Cast();
                        break;

                    case 1:
                        E.Cast();
                        break;
                }
            }

            if (W.IsReady() && target.LSIsValidTarget(W.Range) && usew && !Player.HasBuff("summonerteleport"))
            {
                if (Player.HasBuff("pyromania_particles") && R.IsReady() && usersmart)
                    return;
                    W.Cast(target);
            }
            var rpred = R.GetPrediction(target, true);
            if (R.IsReady()
                && user && target.LSIsValidTarget(R.Range) && !Player.HasBuff("summonerteleport") && Player.HasBuff("pyromania_particle"))
            {
                if (rpred.AoeTargetsHitCount >= userslider)
                {
                    R.Cast(rpred.CastPosition);
                }
               if (target.Health <= Q.GetDamage(target) +W.GetDamage(target))
                {
                    R.Cast(target.Position);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Tibbers Configure
        /// </summary>
        #region Tibbers
        private static void MoveTibbers()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, Game.CursorPos);
        }

        #endregion
    }
}