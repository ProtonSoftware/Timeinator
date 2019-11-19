using SimpleInjector;
using Timeinator.Mobile.Session;
using Timeinator.Mobile.DataAccess;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Extension methods for the dependency injection container
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Injects all the services needed for Timeinator mobile application
        /// </summary>
        /// <param name="container">DI container</param>
        public static Container AddTimeinatorServices(this Container container)
        {
            // Inject singleton services
            // The instance is created only once and re'used everytime
            container.Register<ApplicationViewModel>(Lifestyle.Singleton);
            container.Register<TimeTasksMapper>(Lifestyle.Singleton);
            container.Register<ITimeTasksRepository, TimeTasksRepository>(Lifestyle.Singleton);
            container.Register<ISettingsRepository, SettingsRepository>(Lifestyle.Singleton);
            container.Register<ITimeTasksService, TimeTasksService>(Lifestyle.Singleton);
            container.Register<ITimeTasksCalculator, TimeTasksCalculator>(Lifestyle.Singleton);
            container.Register<ISessionHandler, SessionHandler>(Lifestyle.Singleton);
            container.Register<IViewModelProvider, ViewModelProvider>(Lifestyle.Singleton);
            container.Register<ISettingsProvider, SettingsProvider>(Lifestyle.Singleton);
            container.Register<IUIManager, UIManager>(Lifestyle.Singleton);
            container.Register<SessionNotificationService>(Lifestyle.Singleton);
            container.Register<IRingtonePlayer, RingtonePlayer>(Lifestyle.Singleton);

            // Inject scoped services
            // The instance is created for every scope (in this mobile app case, this should work similar to singletons)

            // Inject transient services
            // The instance is created every single time it is requested in code
            container.Register<TasksListPageViewModel>(Lifestyle.Transient);
            container.Register<TasksTimePageViewModel>(Lifestyle.Transient);
            container.Register<TasksSummaryPageViewModel>(Lifestyle.Transient);
            container.Register<SettingsPageViewModel>(Lifestyle.Transient);
            container.Register<AlarmPageViewModel>(Lifestyle.Transient);
            container.Register<LoginPageViewModel>(Lifestyle.Transient);
            container.Register<AboutPageViewModel>(Lifestyle.Transient);
            container.Register<AddNewTimeTaskPageViewModel>(Lifestyle.Transient);
            container.Register<TasksSessionPageViewModel>(Lifestyle.Singleton);

            // Register our application's db context
            container.Register<TimeinatorMobileDbContext>(Lifestyle.Singleton);

            // Return the container for chaining
            return container;
        }
    }
}
