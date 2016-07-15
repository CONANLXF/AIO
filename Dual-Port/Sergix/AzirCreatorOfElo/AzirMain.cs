using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace Azir_Creator_of_Elo
{
    internal class AzirMain
    {
        private Spells _spells;
        public Menu _menu;
        public AzirModes _modes;
        public Azir_Creator_of_Elo.Spells Spells
        {
            get { return _spells; }
        }
        public Azir_Creator_of_Elo.Menu Menu
        {
            get { return _menu; }
        }
        public SoldierManager soldierManager;
        public AzirMain()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
        public AIHeroClient Hero
        {
            get
            {
                return HeroManager.Player;
            }
        }
        private void OnLoad(EventArgs args)
        {
            if (Hero.ChampionName != "Azir") return;
            Chat.Print("<b><font color =\"#FF33D6\">Azir creator of Elo Loaded!");
            _menu = new AzirMenu("Azir Creator of Elo");
            soldierManager = new SoldierManager();
            _spells = new Spells();
            _modes = new AzirModes(this);
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Ondraw;
            //      walker = new AzirWalker(Menu.GetMenu.SubMenu("Orbwalker"));
        }

        public void Orbwalk(Vector3 pos)
        {
            Orbwalker.MoveTo(pos);
        }

        private void Ondraw(EventArgs args)
        {
            // var ts = TargetSelector.GetSelectedTarget();
            /* if (ts != null)
             {

                 var pos = Game.CursorPos.Extend(ts.Position, Game.CursorPos.Distance(ts.Position) - 250);
                 if (!ts.IsDead)
                     Drawing.DrawCircle(pos, 50, System.Drawing.Color.Red);
             }*/
            var drawControl = Menu._drawSettingsMenu["dcr"].Cast<CheckBox>().CurrentValue;
            var drawFleeMaxRange = Menu._drawSettingsMenu["dfr"].Cast<CheckBox>().CurrentValue;
            if (drawControl)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 925, System.Drawing.Color.GreenYellow);



            var drawLane = Menu._drawSettingsMenu["dsl"].Cast<CheckBox>().CurrentValue;
            int x = 0;
            if (drawLane)

                foreach (Obj_AI_Minion m in soldierManager.Soldiers)
                {
                    //m.tim
                    if (!m.IsDead)
                    {
                        foreach (AIHeroClient h in HeroManager.Enemies)
                        {
                            if (m.LSDistance(h) < 335)
                            {
                                x++;

                            }

                        }
                        if (x > 0)
                        {
                            Render.Circle.DrawCircle(m.Position, 315, System.Drawing.Color.GreenYellow);
                        }
                        else
                        {
                            Render.Circle.DrawCircle(m.Position, 315, System.Drawing.Color.PaleVioletRed);
                        }
                        var wts = Drawing.WorldToScreen(m.Position);
                        var wtssxt = Drawing.WorldToScreen(HeroManager.Player.ServerPosition);

                        if (m.Distance(HeroManager.Player) < 950)
                            Drawing.DrawLine(wts[0], wts[1], wtssxt[0], wtssxt[1], 5f, System.Drawing.Color.GreenYellow);
                        else
                            Drawing.DrawLine(wts[0], wts[1], wtssxt[0], wtssxt[1], 5f,
                                System.Drawing.Color.PaleVioletRed);
                    }
                }
            if (drawFleeMaxRange)
            {
                // var pos = HeroManager.Player.Position.Extend(Game.CursorPos, 450);

                Render.Circle.DrawCircle(Hero.Position, 1150 + 350, System.Drawing.Color.GreenYellow);
            }


        }


        private void OnUpdate(EventArgs args)
        {
            _modes.Update(this);


        }
    }
}
