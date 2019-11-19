using SimpleInjector;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Dependency Injection container for this application
    /// </summary>
    public static class DI
    {
        #region Public Properties

        /// <summary>
        /// The underlying container from which the dependencies are retrieved
        /// </summary>
        public static Container Container { get; set; }

        #endregion
    }
}
