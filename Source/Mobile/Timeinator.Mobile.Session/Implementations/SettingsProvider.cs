using System;
using System.Collections.Generic;
using System.ComponentModel;
using Timeinator.Core;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Session
{
    /// <summary>
    /// The provider for every app setting
    /// </summary>
    public class SettingsProvider : ISettingsProvider, INotifyPropertyChanged
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
        /// Rate of session timer (ms)
        /// </summary>
        public int TimerTickRate { get; private set; } = 1000;

        /// <summary>
        /// Minimum time required to start a task
        /// </summary>
        public double MinimumTaskTime { get; private set; } = 0.1;

        /// <summary>
        /// Current language name
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
        /// Dark mode enabled flag
        /// </summary>
        public bool IsDarkModeOn { get; private set; } = false;

        /// <summary>
        /// Should put highest priority task on top of session queue
        /// </summary>
        public bool HighestPrioritySetAsFirst { get; private set; } = true;

        /// <summary>
        /// Should recalculate tasks after pausing or finishing early
        /// </summary>
        public bool RecalculateTasksAfterBreak { get; private set; } = true;

        /// <summary>
        /// An event that is fired everytime any setting property changes it's value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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

            // Hook to property changed event, so everytime settings are being changed, we save it to the database
            PropertyChanged += SettingValueChanged;

            // Initially load all the setting configuration from database
            InitializeSettings();
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Update attribute by using name, value and type
        /// </summary>
        public void SetSetting(SettingsPropertyInfo setting)
        {
            try
            {
                // Save its value to appropriate property
                this[setting.Name] = Convert.ChangeType(setting.Value, setting.Type);

                // Fire property changed event to inform everyone about the setting change
                PropertyChanged.Invoke(null, new PropertyChangedEventArgs(setting.Name));
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
                SetSetting(setting);
            }
        }

        /// <summary>
        /// Fired everytime any of setting properties get changed
        /// </summary>
        private void SettingValueChanged(object sender, PropertyChangedEventArgs e)
        {
            // Get changed property's name
            var propertyName = e.PropertyName;

            // Create new property info
            var propertyInfo = new SettingsPropertyInfo
            {
                Name = propertyName,
                Value = this[propertyName],
                Type = this[propertyName].GetType()
            };

            // Save it to the database
            mSettingsRepository.SaveSetting(propertyInfo);
        }

        #endregion
    }
}
