﻿using Dna;
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
        /// Injects the view models needed for Pogodeo mobile application
        /// </summary>
        /// <param name="construction">Framework's construction</param>
        public static FrameworkConstruction AddTimeinatorViewModels(this FrameworkConstruction construction)
        {
            // Bind to a single instance of Application view model
            construction.Services.AddSingleton<ApplicationViewModel>();

            // Bind to a single instance of Time Tasks repository
            construction.Services.AddSingleton<ITimeTasksRepository, TimeTasksRepository>();

            // Bind to a single instance of UIManager
            construction.Services.AddSingleton<IUIManager, UIManager>();

            // Return the construction for chaining
            return construction;
        }

        /// <summary>
        /// Injects the database for Pogodeo mobile application
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
                // Make sure its created properly
                db.Database.EnsureCreated();
                // Do migrations
                db.Database.Migrate();
            }

            // Return the construction for chaining
            return construction;
        }
    }
}
