using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace NabbActivator
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     Sets the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Index.OnUpdate;
            Obj_AI_Base.OnSpellCast += Index.OnDoCast;
        }
    }
}