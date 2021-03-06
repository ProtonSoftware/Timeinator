﻿namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// Contains every possible page in this application
    /// </summary>
    public enum ApplicationPage
    {
        /// <summary>
        /// Initial login page
        /// </summary>
        Login,

        /// <summary>
        /// Main tasks list page
        /// </summary>
        TasksList,

        /// <summary>
        /// Tasks time page where user selects how much time he has for session
        /// </summary>
        TasksTime,

        /// <summary>
        /// Tasks summary page where session is calculated
        /// </summary>
        TasksSummary,

        /// <summary>
        /// Tasks session page
        /// </summary>
        TasksSession,

        /// <summary>
        /// Add (or edit) new task page
        /// </summary>
        AddNewTask,

        /// <summary>
        /// Settings page
        /// </summary>
        Settings,

        /// <summary>
        /// About page
        /// </summary>
        About,

        /// <summary>
        /// Alarm page
        /// </summary>
        Alarm
    }
}
