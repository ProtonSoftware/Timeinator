using Microsoft.EntityFrameworkCore;
using MvvmCross.ViewModels;
using SimpleInjector;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile.Core
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
        public static Container Container { get; private set; }

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => Container.GetInstance<ApplicationViewModel>();

        /// <summary>
        /// A shortcut to access the <see cref="SettingsPageViewModel"/>
        /// </summary>
        public static SettingsPageViewModel Settings => Container.GetInstance<SettingsPageViewModel>();

        /// <summary>
        /// A shortcut to get appropriate view model for page with injected dependiencies by DI
        /// </summary>
        /// <typeparam name="T">Any view model that inherites <see cref="MvxViewModel"/></typeparam>
        public static T GetInjectedPageViewModel<T>() where T : MvxViewModel => Container.GetInstance<T>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets up the DI and binds initial view models to that
        /// </summary>
        public static void InitialSetup()
        {
            // Initialize brand-new DI container
            Container = new Container().AddTimeinatorServices();
        }

        /// <summary>
        /// Creates the database and applies all the migrations
        /// </summary>
        public static void MigrateDatabase()
        {
            // Migrate the database
            Container.GetInstance<TimeinatorMobileDbContext>().Database.Migrate();
        }

        #endregion
    }
}
