using System;
using System.Collections.Generic;
using Timeinator.Core;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Session
{
    /// <summary>
    /// The provider for every app setting
    /// </summary>
    public class SettingsProvider : ISettingsProvider
    {
        #region Private Members

        private readonly ISettingsRepository mSettingsRepository;
        private readonly IUIManager mUIManager;

        private string mLanguageValue = "Polski";

        #endregion

        #region Private Properties

        /// <summary>
        /// Allows to get the property of this class by simply calling its name
        /// </summary>
        /// <param name="propertyName">The name of the property to get/set</param>
        private object this[string propertyName]
        {
            get => GetType().GetProperty(propertyName).GetValue(this, null);
            set => GetType().GetProperty(propertyName).SetValue(this, value, null);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of all available languages that this app supports
        /// </summary>
        public List<string> Languages => new List<string> 
        {
            "Polski",
            "English",
            "Français"
        };

        /// <summary>
        /// If set to true, session time will be used as finishing timestamp, otherwise session time is just amount of time for session
        /// </summary>
        public bool SessionTimeAsFinishTime { get; private set; } = false;

        /// <summary>
        /// The tick rate in miliseconds that session timer will be based on
        /// </summary>
        public int TimerTickRate { get; private set; } = 1000;

        /// <summary>
        /// The minimum time in minutes required for a task in session
        /// </summary>
        public double MinimumTaskTime { get; private set; } = 0.1;

        /// <summary>
        /// The current selected value of app's language
        /// </summary>
        public string LanguageValue
        {
            get => mLanguageValue;
            private set
            {
                // Set new value
                mLanguageValue = value;

                // Change app's language based on that
                switch (Languages.IndexOf(mLanguageValue))
                {
                    case 1:
                        mUIManager.ChangeLanguage("en-US");
                        break;

                    case 2:
                        mUIManager.ChangeLanguage("fr-FR");
                        break;

                    // 0 or any not recognized index is default - Polish language
                    default:
                        mUIManager.ChangeLanguage("pl-PL");
                        break;
                }
            }
        }

        /// <summary>
        /// Indicates if dark mode is on
        /// </summary>
        public bool IsDarkModeOn { get; private set; } = false;

        /// <summary>
        /// If set to true, tasks with highest priority will be the first ones
        /// </summary>
        public bool HighestPrioritySetAsFirst { get; private set; } = true;

        /// <summary>
        /// If set to true, whenever break ends remaining tasks will be recalculated for remaining time
        /// </summary>
        public bool RecalculateTasksAfterBreak { get; private set; } = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsProvider(IUIManager uiManager, ISettingsRepository settingsRepository)
        {
            // Inject DI services
            mSettingsRepository = settingsRepository;
            mUIManager = uiManager;

            // Initially load all the setting configuration from database
            InitializeSettings();
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Updates specified setting with new value
        /// </summary>
        /// <param name="setting">The setting to set</param>
        /// <param name="shouldSaveToDatabase">Database flag, can be set to false if we don't want setting to be saved</param>
        public void SetSetting(SettingsPropertyInfo setting, bool shouldSaveToDatabase = true)
        {
            try
            {
                // Set its value to appropriate property
                this[setting.Name] = Convert.ChangeType(setting.Value, setting.Type);

                // If we should save new value to database
                if (shouldSaveToDatabase)
                {
                    // Do it
                    mSettingsRepository.SaveSetting(setting);
                }
            }
            // If something fails, that means the setting in database is broken
            // Therefore, default value of a property will be used and future changes will repair database failures
            // So no need to do anything after catching the exception
            catch { }
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Initializes settings with values that are currently saved in the database
        /// </summary>
        private void InitializeSettings()
        {
            // Get every setting from database
            var settingList = mSettingsRepository.GetAllSettings();

            // For each one...
            foreach (var setting in settingList)
            {
                // Set it's value to associated property
                SetSetting(setting, false);
            }
        }

        #endregion
    }
}
