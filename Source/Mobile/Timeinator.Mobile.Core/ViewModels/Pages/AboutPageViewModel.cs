using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for about page
    /// </summary>
    public class AboutPageViewModel : BaseModalPageViewModel
    {
        #region Private Members

        private readonly IUIManager mUIManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AboutPageViewModel(IUIManager uiManager)
        {
            // Create commands
            GoBackCommand = new RelayCommand(ClosePage);

            // Get injected services
            mUIManager = uiManager;
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
    }
}
