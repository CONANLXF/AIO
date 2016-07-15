
using EloBuddy.SDK.Menu.Values;

 namespace NabbActivator
{
    /// <summary>
    ///     The managers class.
    /// </summary>
    internal class Managers
    {
        /// <summary>
        ///     Sets the minimum necessary health percent to use a health potion.
        /// </summary>
        public static int MinHealthPercent => Vars.SliderMenu["health"].Cast<Slider>().CurrentValue;

        /// <summary>
        ///     Sets the minimum necessary mana percent to use a mana potion.
        /// </summary>
        public static int MinManaPercent => Vars.SliderMenu["mana"].Cast<Slider>().CurrentValue;
    }
}