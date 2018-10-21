﻿using Dna;
using Timeinator.Mobile.DataAccess;

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
        /// A shortcut to access the current implementation of <see cref="ITimeTasksRepository"/>
        /// </summary>
        public static ITimeTasksRepository TimeTasksRepository => Framework.Service<ITimeTasksRepository>();

        /// <summary>
        /// A shortcut to access the <see cref="TimeTasksManager"/>
        /// </summary>
        public static TimeTasksManager TimeTasksManager => Framework.Service<TimeTasksManager>();

        /// <summary>
        /// A shortcut to access the <see cref="TimeTasksMapper"/>
        /// </summary>
        public static TimeTasksMapper TimeTasksMapper => Framework.Service<TimeTasksMapper>();

        /// <summary>
        /// A shortcut to access the <see cref="UIManager"/>
        /// </summary>
        public static IUIManager UI => Framework.Service<IUIManager>();

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