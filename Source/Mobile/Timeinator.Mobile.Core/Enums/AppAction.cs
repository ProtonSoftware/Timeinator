﻿namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Actions supported by Main Activity
    /// </summary>
    public enum AppAction
    {
        /// <summary>
        /// Simply opens app
        /// </summary>
        DoNothing,

        /// <summary>
        /// Opens app and heads to session page
        /// </summary>
        GoToSession,

        /// <summary>
        /// Pauses current session
        /// </summary>
        PauseSession,

        /// <summary>
        /// Asks for next task in session
        /// </summary>
        NextSessionTask,

        /// <summary>
        /// Resumes paused session
        /// </summary>
        ResumeSession,

        /// <summary>
        /// Stop / cancel session
        /// </summary>
        Stop
    }
}
