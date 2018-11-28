using Dna;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Extension methods for the <see cref="FrameworkConstruction"/>
    /// </summary>
    public static class FrameworkConstructionExtensions
    {
        /// <summary>
        /// Injects the view models needed for Timeinator mobile application
        /// </summary>
        /// <param name="construction">Framework's construction</param>
        public static FrameworkConstruction AddTimeinatorViewModels(this FrameworkConstruction construction)
        {
            // Bind to a single instance of specified models
            construction.Services.AddSingleton<ApplicationViewModel>();
            construction.Services.AddSingleton<SettingsPageViewModel>();
            construction.Services.AddSingleton<TimeTasksMapper>();
            construction.Services.AddSingleton<IUIManager, UIManager>();
            construction.Services.AddSingleton<ITimeTasksRepository, TimeTasksRepository>();
            construction.Services.AddSingleton<ISettingsRepository, SettingsRepository>();

            // Bind to a scoped instance of specified models
            construction.Services.AddScoped<ITimeTasksService, TimeTasksService>();
            construction.Services.AddScoped<ITimeTasksManager, TimeTasksManager>();
            construction.Services.AddScoped<IUserTimeHandler, UserTimeHandler>();

            // Inject dependiencies into every page's view model
            construction.Services.AddTransient<TasksListPageViewModel>();
            construction.Services.AddTransient<TasksPreparationPageViewModel>();
            construction.Services.AddTransient<TasksSessionPageViewModel>();
            construction.Services.AddTransient<LoginPageViewModel>();
            construction.Services.AddTransient<AddNewTimeTaskViewModel>();

            // Return the construction for chaining
            return construction;
        }

        /// <summary>
        /// Injects the database for Pogodeo Timeinator application
        /// </summary>
        /// <param name="construction">Framework's construction</param>
        public static FrameworkConstruction AddDbContext(this FrameworkConstruction construction)
        {
            // Use Sqlite library
            construction.Services.AddEntityFrameworkSqlite();

            // Bind a db context to access in this application
            construction.Services.AddDbContext<TimeinatorMobileDbContext>();

            // Get the service provider
            var serviceProvider = construction.Services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                // Get the db service
                var db = scope.ServiceProvider.GetRequiredService<TimeinatorMobileDbContext>();
                // Make sure its created properly and do migrations
                db.Database.Migrate();
            }

            // Return the construction for chaining
            return construction;
        }
    }
}
