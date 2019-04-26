using System;
using System.ComponentModel;
using Timeinator.Core;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for application's settings page
    /// </summary>
    public class SettingsPageViewModel : BaseModalPageViewModel
    {
        #region Private Members

        private readonly IUIManager mUIManager;
        private readonly ISettingsRepository mSettingsRepository;

        /// <summary>
        /// Index of the language used in this application
        /// 0 - Polish
        /// 1 - English
        /// </summary>
        private int mLanguageIndex = 0;

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

        #region Public Properties
        
        /// <summary>
        /// Index of the language used in this application
        /// 0 - Polish
        /// 1 - English
        /// </summary>
        public int LanguageIndex
        {
            get => mLanguageIndex;
            set
            {
                // Set new value
                mLanguageIndex = value;

                // Change app's language based on that
                switch (mLanguageIndex)
                {
                    case 1:
                        mUIManager.ChangeLanguage("en-US");
                        break;

                    // 0 or any not found index is default - Polish language
                    default:
                        mUIManager.ChangeLanguage("pl-PL");
                        break;
                }
            }
        }

        /// <summary>
        /// Indicates if dark mode is set 
        /// </summary>
        public bool IsDarkModeOn { get; set; } = false;

        /// <summary>
        /// If set to true, tasks with highest priority will be the first ones
        /// </summary>
        public bool HighestPrioritySetAsFirst { get; set; } = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsPageViewModel(IUIManager uiManager, ISettingsRepository settingsRepository)
        {
            // Create commands
            GoBackCommand = new RelayCommand(ClosePage);

            // Get injected DI services
            mUIManager = uiManager;
            mSettingsRepository = settingsRepository;

            // Load initial settings configuration from database
            InitializeSettings();

            // Hook to property changed event, so everytime settings are being changed, we save it to the database
            PropertyChanged += SettingValueChanged;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Closes this page and goes back to previous one
        /// </summary>
        private void ClosePage()
        {
            // Close this page
            mUIManager.GoBackToPreviousPage(this);
        }

        #endregion

        #region Private Methods

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

        /// <summary>
        /// Fired everytime any of this view model's properties get changed
        /// </summary>
        private void SettingValueChanged(object sender, PropertyChangedEventArgs e)
        {
            // Get changed property's name
            var propertyName = e.PropertyName;

            // Get its type and value based on that
            var propertyValue = this[propertyName];
            var propertyType = this[propertyName].GetType();

            // Create new property info
            var propertyInfo = new SettingsPropertyInfo
            {
                Name = propertyName,
                Type = propertyType,
                Value = propertyValue
            };

            // Save it to the database
            mSettingsRepository.SaveSetting(propertyInfo);
        }

        #endregion
    }
}
