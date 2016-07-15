using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.SDK;

 namespace ExorAIO.Champions.Twitch
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     The methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Twitch.OnUpdate;
            Obj_AI_Base.OnSpellCast += Twitch.OnDoCast;
            Spellbook.OnCastSpell += Twitch.OnCastSpell;
            Orbwalker.OnPreAttack += Twitch.OnAction;
        }
    }
}