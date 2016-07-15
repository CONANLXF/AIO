using EloBuddy;
using LeagueSharp;

 namespace ExorAIO.Champions.Akali
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     Initializes the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Akali.OnUpdate;
            Obj_AI_Base.OnSpellCast += Akali.OnDoCast;
        }
    }
}