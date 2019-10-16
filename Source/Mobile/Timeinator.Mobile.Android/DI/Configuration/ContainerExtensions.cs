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
            // Bind to a single instance of specified models
            container.Register<ApplicationViewModel>(Lifestyle.Singleton);
            container.Register<SettingsPageViewModel>(Lifestyle.Singleton);
            container.Register<TimeTasksMapper>(Lifestyle.Singleton);
            container.Register<ITimeTasksRepository, TimeTasksRepository>(Lifestyle.Singleton);
            container.Register<ISettingsRepository, SettingsRepository>(Lifestyle.Singleton);
            container.Register<ITimeTasksService, TimeTasksService>(Lifestyle.Singleton);
            container.Register<ITimeTasksCalculator, TimeTasksCalculator>(Lifestyle.Singleton);
            container.Register<ISessionHandler, SessionHandler>(Lifestyle.Singleton);
            container.Register<IViewModelProvider, ViewModelProvider>(Lifestyle.Singleton);

            // Inject dependiencies into every page's view model
            container.Register<TasksListPageViewModel>(Lifestyle.Transient);
            container.Register<TasksTimePageViewModel>(Lifestyle.Transient);
            container.Register<TasksSummaryPageViewModel>(Lifestyle.Transient);
            container.Register<AlarmPageViewModel>(Lifestyle.Transient);
            container.Register<LoginPageViewModel>(Lifestyle.Transient);
            container.Register<AboutPageViewModel>(Lifestyle.Transient);
            container.Register<AddNewTimeTaskPageViewModel>(Lifestyle.Transient);
            container.Register<TasksSessionPageViewModel>(Lifestyle.Singleton);

            // Register our application's db context
            container.Register<TimeinatorMobileDbContext>(Lifestyle.Singleton);

            // Add Android-specific dependency injection implementations
            container.Register<IUIManager, UIManager>(Lifestyle.Singleton);
            container.Register<SessionNotificationService>(Lifestyle.Singleton);
            container.Register<IRingtonePlayer, RingtonePlayer>(Lifestyle.Singleton);

            // Return the container for chaining
            return container;
        }
    }
}
