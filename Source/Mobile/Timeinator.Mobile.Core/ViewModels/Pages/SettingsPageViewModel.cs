using System.Collections.ObjectModel;
using System.ComponentModel;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The view model for application's settings page
    /// </summary>
    public class SettingsPageViewModel : BaseModalPageViewModel
    {
        #region Private Members

        private readonly ApplicationViewModel mApplicationViewModel;
        private readonly ISettingsProvider mSettingsProvider;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of possible languages in the app
        /// </summary>
        public ObservableCollection<string> LanguageItems { get; private set; }

        /// <summary>
        /// The language used in this application
        /// </summary>
        public string LanguageValue { get; set; }

        /// <summary>
        /// If set to true, tasks with highest priority will be the first ones
        /// </summary>
        public bool HighestPrioritySetAsFirst { get; set; } 

        /// <summary>
        /// If set to true, whenever break ends remaining tasks will be recalculated for remaining time
        /// </summary>
        public bool RecalculateTasksAfterBreak { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsPageViewModel(ISettingsProvider settingsProvider, ApplicationViewModel applicationViewModel)
        {
            // Create commands
            GoBackCommand = new RelayCommand(ClosePage);

            // Get injected DI services
            mApplicationViewModel = applicationViewModel;
            mSettingsProvider = settingsProvider;

            // Load current setting configuration from the provider
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
            // Go back to previous page
            // NOTE: It's important to reload the page here so UI potentially updates with new language
            mApplicationViewModel.GoToPage(ApplicationPage.TasksList);
        }

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

        #region Private Methods

        /// <summary>
        /// Initializes this view model state with values from setting provider
        /// </summary>
        private void InitializeSettings()
        {
            LanguageItems = new ObservableCollection<string>(mSettingsProvider.Languages);
            LanguageValue = mSettingsProvider.LanguageValue;
            HighestPrioritySetAsFirst = mSettingsProvider.HighestPrioritySetAsFirst;
            RecalculateTasksAfterBreak = mSettingsProvider.RecalculateTasksAfterBreak;
        }

        /// <summary>
        /// Fired everytime any of this view model's properties get changed
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
            mSettingsProvider.SetSetting(propertyInfo);
        }

        #endregion
    }
}
