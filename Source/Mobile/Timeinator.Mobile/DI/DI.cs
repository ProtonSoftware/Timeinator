using Dna;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Dependency Injection container for this application
    /// </summary>
    public static class DI
    {
        #region Public Shortcuts

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => Framework.Service<ApplicationViewModel>();

        /// <summary>
        /// A shortcut to access the <see cref="SettingsPageViewModel"/>
        /// </summary>
        public static SettingsPageViewModel Settings => Framework.Service<SettingsPageViewModel>();

        /// <summary>
        /// A shortcut to get appropriate view model for page with injected dependiencies by DI
        /// </summary>
        /// <typeparam name="T">Any view model that inherites <see cref="BasePageViewModel"/></typeparam>
        public static T GetInjectedPageViewModel<T>() where T : BasePageViewModel => Framework.Service<T>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets up the DI and binds initial view models to that
        /// </summary>
        public static void InitialSetup() => Framework.Construct<DefaultFrameworkConstruction>()
                                                      .AddFileLogger()
                                                      .AddTimeinatorViewModels()
                                                      .AddDbContext()
                                                      .Build();

        #endregion
    }
}
