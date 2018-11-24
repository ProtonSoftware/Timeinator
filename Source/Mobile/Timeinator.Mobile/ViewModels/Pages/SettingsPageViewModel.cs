namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for application's settings page
    /// </summary>
    public class SettingsPageViewModel : BasePageViewModel
    {
        #region Private Members

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
                        DI.UI.ChangeLanguage("en-US");
                        break;

                    // 0 or any not found index is default - Polish language
                    default:
                        DI.UI.ChangeLanguage("pl-PL");
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
    }
}
