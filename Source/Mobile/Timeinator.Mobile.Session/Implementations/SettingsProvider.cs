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

        private bool mSessionWasFinishTime = true;
        private int mTimerTickRate = 1000;
        private double mMinimumTaskTime = 0.1;
        private string mLanguageValue = "English";
        private bool mIsDarkModeOn = false;
        private bool mHighestPrioritySetAsFirst = true;
        private bool mRecalculateTasksAfterBreak = true;

        #endregion

        #region Interface

        /// <summary>
        /// Available languages names
        /// </summary>
        public List<string> Languages { get; private set; } = new List<string> {
            "English",
            "Polski",
            "Français"
        };

        /// <summary>
        /// Flag whether user started last session with a finish time
        /// </summary>
        public bool SessionWasFinishTime { get => mSessionWasFinishTime; 
            set
            {
                mSessionWasFinishTime = value;
                SaveSetting(nameof(SessionWasFinishTime));
            }
        }

        /// <summary>
        /// Rate of session timer (ms)
        /// </summary>
        public int TimerTickRate { get => mTimerTickRate;
            set
            {
                mTimerTickRate = value;
                SaveSetting(nameof(TimerTickRate));
            }
        }

        /// <summary>
        /// Minimum time required to start a task
        /// </summary>
        public double MinimumTaskTime { get => mMinimumTaskTime;
            set
            {
                mMinimumTaskTime = value;
                SaveSetting(nameof(MinimumTaskTime));
            }
        }

        /// <summary>
        /// Current language name
        /// </summary>
        public string LanguageValue { get => mLanguageValue;
            set
            {
                mLanguageValue = value;
                SaveSetting(nameof(LanguageValue));
            }
        }

        /// <summary>
        /// Dark mode enabled flag
        /// </summary>
        public bool IsDarkModeOn { get => mIsDarkModeOn;
            set
            {
                mIsDarkModeOn = value;
                SaveSetting(nameof(IsDarkModeOn));
            }
        }

        /// <summary>
        /// Should put highest priority task on top of session queue
        /// </summary>
        public bool HighestPrioritySetAsFirst { get => mHighestPrioritySetAsFirst;
            set
            {
                mHighestPrioritySetAsFirst = value;
                SaveSetting(nameof(HighestPrioritySetAsFirst));
            }
        }

        /// <summary>
        /// Should recalculate tasks after pausing or finishing early
        /// </summary>
        public bool RecalculateTasksAfterBreak { get => mRecalculateTasksAfterBreak;
            set
            {
                mRecalculateTasksAfterBreak = value;
                SaveSetting(nameof(RecalculateTasksAfterBreak));
            }
        }

        /// <summary>
        /// Update attribute by using name, value and type
        /// </summary>
        public void SetSetting(SettingsPropertyInfo setting)
        {
            this[setting.Name] = Convert.ChangeType(setting.Value, setting.Type);
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
        /// Update SettingRepository with current value
        /// </summary>
        private void SaveSetting(string name)
        {
            var setting = new SettingsPropertyInfo { Name = name, Type = this[name].GetType(), Value = this[name] };
            mSettingsRepository.SaveSetting(setting);
        }

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
