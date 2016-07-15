using System;
using LeagueSharp;
using EloBuddy;

namespace GeassLib.Events
{
    public class OnLevel
    {
         public bool Enabled { get; set; }
         readonly int[] _abilitySequences;
         private int _lastLevel;
        public OnLevel(int[] sequence , bool enable = true)
        {
            if(!DelayHandler.Loaded)DelayHandler.Load();

            Enabled = enable;
            _abilitySequences = sequence;
            Game.OnUpdate += OnUpdate;
        }


        void OnUpdate(EventArgs args)
        {
            if (DelayHandler.CheckOnLevel())
            {
                DelayHandler.UseOnLevel();

                if (_lastLevel != Loader.Player.Level && Enabled)
                {
                    _lastLevel = Loader.Player.Level;
                    LevelUpSpells();
                }
            }
        }

        void LevelUpSpells()
        {
            var qL = Loader.Player.Spellbook.GetSpell(SpellSlot.Q).Level + _qOff;
            var wL = Loader.Player.Spellbook.GetSpell(SpellSlot.W).Level + _wOff;
            var eL = Loader.Player.Spellbook.GetSpell(SpellSlot.E).Level + _eOff;
            var rL = Loader.Player.Spellbook.GetSpell(SpellSlot.R).Level + _rOff;


            if (qL + wL + eL + rL >= Loader.Player.Level) return;

            int[] level = { 0, 0, 0, 0 };

            for (var i = 0; i < Loader.Player.Level; i++)
                level[_abilitySequences[i] - 1] = level[_abilitySequences[i] - 1] + 1;

            if (qL < level[0]) Loader.Player.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Loader.Player.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Loader.Player.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Loader.Player.Spellbook.LevelSpell(SpellSlot.R);
        }

#pragma warning disable RECS0122 // Initializing field with default value is redundant
        readonly int _qOff = 0;
        readonly int _wOff = 0;
        readonly int _eOff = 0;
        readonly int _rOff = 0;
#pragma warning restore RECS0122 // Initializing field with default value is redundant

    }
}
