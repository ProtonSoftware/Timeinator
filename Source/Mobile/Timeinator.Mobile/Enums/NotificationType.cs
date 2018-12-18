namespace Timeinator.Mobile
{
    /// <summary>
    /// Types of notification supported by INotificationHandler
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Basic text with icon
        /// </summary>
        Message,

        /// <summary>
        /// Progress bar with text and icon
        /// </summary>
        Progress,

        /// <summary>
        /// Prompt dialog with icon
        /// </summary>
        Prompt
    }
}
