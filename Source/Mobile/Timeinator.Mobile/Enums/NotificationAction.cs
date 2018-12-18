namespace Timeinator.Mobile
{
    /// <summary>
    /// Actions supported by Main Activity
    /// </summary>
    public enum NotificationAction
    {
        /// <summary>
        /// Simply opens app
        /// </summary>
        DoNothing,

        /// <summary>
        /// Opens app and heads to session page
        /// </summary>
        GoToSession,

        /// <summary>
        /// Pauses current session
        /// </summary>
        PauseSession,

        /// <summary>
        /// Asks for next task in session
        /// </summary>
        NextSessionTask
    }
}
