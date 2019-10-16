using Microsoft.EntityFrameworkCore;
using MvvmCross.ViewModels;
using SimpleInjector;
using Timeinator.Mobile.DataAccess;
using Timeinator.Mobile.Domain;

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

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => Container.GetInstance<ApplicationViewModel>();

        /// <summary>
        /// A shortcut to access the <see cref="SettingsPageViewModel"/>
        /// </summary>
        public static SettingsPageViewModel Settings => Container.GetInstance<SettingsPageViewModel>();

        #endregion

        #region Public Methods

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
