using Dna;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Extension methods for the <see cref="FrameworkConstruction"/>
    /// </summary>
    public static class FrameworkConstructionExtensions
    {
        private static readonly Dictionary<MethodInfo, string> MethodInfoToUnitSuffix = new Dictionary<MethodInfo, string>
        {
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddYears), new[] { typeof(int) }), " years" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMonths), new[] { typeof(int) }), " months" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddDays), new[] { typeof(double) }), " days" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddHours), new[] { typeof(double) }), " hours" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMinutes), new[] { typeof(double) }), " minutes" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddSeconds), new[] { typeof(double) }), " seconds" }
        };

        private static readonly Dictionary<MethodInfo, string> SupportedMethods = new Dictionary<MethodInfo, string>
        {
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(double) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(float) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(int) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(long) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(sbyte) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(short) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(byte), typeof(byte) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(double), typeof(double) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(float), typeof(float) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(long), typeof(long) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(sbyte), typeof(sbyte) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(short), typeof(short) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(uint), typeof(uint) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(ushort), typeof(ushort) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(byte), typeof(byte) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(double), typeof(double) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(float), typeof(float) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(int), typeof(int) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(long), typeof(long) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(sbyte), typeof(sbyte) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(short), typeof(short) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(uint), typeof(uint) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(ushort), typeof(ushort) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Round), new[] { typeof(double) }), "round" },
            { typeof(Math).GetMethod(nameof(Math.Round), new[] { typeof(double), typeof(int) }), "round" }
        };

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
            construction.Services.AddSingleton<ITimeTasksRepository, TimeTasksRepository>();
            construction.Services.AddSingleton<ISettingsRepository, SettingsRepository>();

            // Bind to a scoped instance of specified models
            construction.Services.AddSingleton<ITimeTasksService, TimeTasksService>();
            construction.Services.AddSingleton<ITimeTasksCalculator, TimeTasksCalculator>();
            construction.Services.AddSingleton<ISessionTimer, SessionTimer>();

            // Inject dependiencies into every page's view model
            construction.Services.AddTransient<TasksListPageViewModel>();
            construction.Services.AddTransient<TasksTimePageViewModel>();
            construction.Services.AddTransient<TasksSummaryPageViewModel>();
            construction.Services.AddSingleton<TasksSessionPageViewModel>();
            construction.Services.AddTransient<AlarmPageViewModel>();
            construction.Services.AddTransient<LoginPageViewModel>();
            construction.Services.AddTransient<AboutPageViewModel>();
            construction.Services.AddTransient<AddNewTimeTaskPageViewModel>();

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
                // Make sure its created properly and do pending migrations
                db.Database.Migrate();
            }

            // Return the construction for chaining
            return construction;
        }
    }
}
