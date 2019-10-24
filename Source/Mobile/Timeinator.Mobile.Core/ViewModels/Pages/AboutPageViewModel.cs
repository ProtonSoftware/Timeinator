using System;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The view model for about page
    /// </summary>
    public class AboutPageViewModel : BaseModalPageViewModel
    {
        #region Private Members

        private readonly IUIManager mUIManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// The version number string to show in the page
        /// </summary>
        public string VersionNumber { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AboutPageViewModel(IUIManager uiManager)
        {
            // Create commands
            GoBackCommand = new RelayCommand(ClosePage);

            // Get injected DI services
            mUIManager = uiManager;

            // Initialize the application assembly data
            InitializeAppData();
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

        #region Private Helpers

        /// <summary>
        /// Loads the data about this application's assembly such as version number/updates/etc. 
        /// </summary>
        private void InitializeAppData()
        {
            var currentAssembly = typeof(App).Assembly.GetName();
            VersionNumber = currentAssembly.Version.ToString(3);
        }

        #endregion
    }
}
