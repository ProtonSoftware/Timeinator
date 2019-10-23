using Timeinator.Mobile.Domain;
using System;
using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Session
{
    /// <summary>
    /// Manager for in app settings
    /// </summary>
    public class SettingsProvider : ISettingsProvider
    {
        #region Private Members

        private readonly ISettingsRepository mSettingsRepository;

        #endregion

        #region Interface

        /// <summary>
        /// Flag whether user started last session with a finish time
        /// </summary>
        public bool SessionWasFinishTime { get; private set; } = true;

        /// <summary>
        /// Rate of session timer (ms)
        /// </summary>
        public int TimerTickRate { get; private set; } = 1000;

        /// <summary>
        /// Minimum time required to start a task
        /// </summary>
        public double MinimumTaskTime { get; private set; } = 0.1;

        /// <summary>
        /// Available languages names
        /// </summary>
        public List<string> Languages { get; private set; } = new List<string> {
            "English",
            "Polski",
            "Français"
        };

        /// <summary>
        /// Current language name
        /// </summary>
        public string LanguageValue { get; set; } = "English";

        /// <summary>
        /// Dark mode enabled flag
        /// </summary>
        public bool IsDarkModeOn { get; set; } = false;

        /// <summary>
        /// Should put highest priority task on top of session queue
        /// </summary>
        public bool HighestPrioritySetAsFirst { get; set; } = true;

        /// <summary>
        /// Should recalculate tasks after pausing or finishing early
        /// </summary>
        public bool RecalculateTasksAfterBreak { get; set; } = true;

        /// <summary>
        /// Update SettingsRepository with new setting
        /// </summary>
        public void SaveSetting(SettingsPropertyInfo setting)
        {
            this[setting.Name] = Convert.ChangeType(setting.Value, setting.Type);
            mSettingsRepository.SaveSetting(setting);
        }

        #endregion

        #region Constructor

        public SettingsProvider(ISettingsRepository settingsRepository)
        {
            mSettingsRepository = settingsRepository;

            InitializeSettings();
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Allows to get the property of this view model by simply calling its name
        /// </summary>
        /// <param name="propertyName">The name of the property to get/set</param>
        private object this[string propertyName]
        {
            get => GetType().GetProperty(propertyName).GetValue(this, null);
            set => GetType().GetProperty(propertyName).SetValue(this, value, null);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Initializes this view model state with values that are currently saved in the database
        /// </summary>
        private void InitializeSettings()
        {
            // Get every setting from database
            var settingList = mSettingsRepository.GetAllSettings();

            // For each one...
            foreach (var setting in settingList)
            {
                try
                {
                    // Save its value to appropriate property
                    this[setting.Name] = Convert.ChangeType(setting.Value, setting.Type);
                }
                // If something fails, that means the setting in database is broken
                // Therefore, default value of a property will be used and future changes will repair database failures
                // So no need to do anything after catching the exception
                catch { }
            }
        }

        #endregion
    }
}
