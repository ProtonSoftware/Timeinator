namespace Timeinator.Mobile
{
    /// <summary>
    /// The extensions for application's icon types
    /// </summary>
    public static class IconExtensions
    {

        /// <summary>
        /// Converts <see cref="ApplicationIconType"/> to a Material Icon string
        /// </summary>
        /// <param name="type">The icon to convert</param>
        /// <returns>Material Icon string</returns>
        public static string ToMaterialFont(this ApplicationIconType icon)
        {
            // Return a Material Icon string based on the icon type
            switch (icon)
            {
                case ApplicationIconType.Settings:
                    return "\ue8b8";

                case ApplicationIconType.About:
                    return "\ue88e";

                case ApplicationIconType.Help:
                    return "\ue887";

                case ApplicationIconType.Cash:
                    return "\ue53e";

                // If none found, return null
                default:
                    return null;
            }
        }
    }
}
