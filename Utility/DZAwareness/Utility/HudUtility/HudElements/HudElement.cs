 namespace DZAwarenessAIO.Utility.HudUtility.HudElements
{
    /// <summary>
    /// The Hud Element base class
    /// </summary>
    public abstract class HudElement
    {
        /// <summary>
        /// Called when the instance is loaded.
        /// </summary>
        public abstract void OnLoad();

        /// <summary>
        /// Raises the events.
        /// </summary>
        public abstract void RaiseEvents();

        /// <summary>
        /// Initializes the drawings.
        /// </summary>
        public abstract void InitDrawings();

        /// <summary>
        /// Removes the drawings.
        /// </summary>
        public abstract void RemoveDrawings();
    }
}
