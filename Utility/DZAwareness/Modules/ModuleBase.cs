using DZAwarenessAIO.Utility;

 namespace DZAwarenessAIO.Modules
{
    /// <summary>
    /// The ModuleBase class
    /// </summary>
    abstract class ModuleBase
    {
        /// <summary>
        /// Called when the module is loaded.
        /// </summary>
        public void OnLoad()
        {
            CreateMenu();
            InitEvents();
        }

        /// <summary>
        /// Creates the menu.
        /// </summary>
        public abstract void CreateMenu();

        /// <summary>
        /// Initializes the events.
        /// </summary>
        public abstract void InitEvents();

        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <returns></returns>
        public abstract ModuleTypes GetModuleType();

        /// <summary>
        /// Should the module run.
        /// </summary>
        /// <returns></returns>
        public abstract bool ShouldRun();

        /// <summary>
        /// Called On Update
        /// </summary>
        public abstract void OnTick();
    }
}
