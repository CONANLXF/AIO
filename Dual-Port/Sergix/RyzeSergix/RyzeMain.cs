using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace RyzeAssembly
{
    class RyzeMain
    {
        private RyzeAssembly.Spells _spells;
        public RyzeAssembly.Menu _menu;
        private string tittle,version;
        private RyzeAssembly.Modes _modes;
        public RyzeAssembly.Modes Modes { get { return _modes; } }
        public string getName()
        {
            return tittle;
        }
        public string getVersion()
        {
            return version;
        }
        public RyzeAssembly.Spells Spells
        {
            get { return _spells; }
        }
        public RyzeAssembly.Menu Menu
        {
            get { return _menu; }
        }
        public AIHeroClient Hero
        {
            get
            {

                return ObjectManager.Player;
            }
        }
        public int GetPassiveBuff
        {
            get
            {
                var data = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack");
                if (data != null)
                {
                    return data.Count == -1 ? 0 : data.Count == 0 ? 1 : data.Count;
                }
                return 0;
            }
        }
        public RyzeMain()
        {

           tittle = "[Ryze]Ryze Updated June 2016";
            version = "1.0.0.0";
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private void OnLoad(EventArgs args)
        {
         
            if (Hero.ChampionName != "Ryze") return;
            Chat.Print(getName() + " load good luck ;) " + getVersion());
            _spells = new Spells();
            _modes = new Modes();
            _menu = new Menu();
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Drawing.OnDraw += draw;
            Game.OnUpdate += update;
        }

        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                //   if ((Game.Time - oldtime) > 0.4)
                //         {

                if (_modes.Functions != null)
                {
                    if (_modes.Functions[_modes.I] == "Q")
                        if (args.Slot == SpellSlot.Q)
                        {
                            _modes.Rev = true;
                        }
                    if (_modes.Functions[_modes.I] == "W")
                        if (args.Slot == SpellSlot.W)
                        {
                            _modes.Rev = true;
                        }
                    if (_modes.Functions[_modes.I] == "E")
                        if (args.Slot == SpellSlot.E)
                        {
                            _modes.Rev = true;
                        }
                    if (_modes.Functions[_modes.I] == "R")
                        if (args.Slot == SpellSlot.R)
                        {
                            _modes.Rev = true;
                        }

                }
                else
                {
                    if (args.Slot == SpellSlot.Q)
                    {
                        _modes.Qcast = false;
                    }
                    if (args.Slot == SpellSlot.W)
                    {
                        _modes.Qcast = true;
                    }
                    if (args.Slot == SpellSlot.E)
                    {
                        _modes.Qcast = true;
                    }
                    if (args.Slot == SpellSlot.R)
                    {
                        _modes.Qcast = true;
                    }
                }
        
            }

            //   }
        }
    

        private void update(EventArgs args)
        {
            _modes.Update(this);
        }

        private void draw(EventArgs args)
        {
            if (Hero.IsDead) return;
            if (RyzeAssembly.Menu._drawSettingsMenu["Draw Q Range"].Cast<CheckBox>().CurrentValue)
                if(_spells.Q.IsReady())
                Render.Circle.DrawCircle(Hero.Position, 900f, System.Drawing.Color.Blue, 2);
            if (RyzeAssembly.Menu._drawSettingsMenu["Draw W Range"].Cast<CheckBox>().CurrentValue)
                if (_spells.W.IsReady())
                    Render.Circle.DrawCircle(Hero.Position, 600f, System.Drawing.Color.Blue, 2);
            if (RyzeAssembly.Menu._drawSettingsMenu["Draw E Range"].Cast<CheckBox>().CurrentValue)
                if (_spells.E.IsReady())
                    Render.Circle.DrawCircle(Hero.Position, 600f, System.Drawing.Color.Blue, 2);
       
        }
    }
}
