using MvvmCross.ViewModels;
using System.Threading.Tasks;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for main application state
    /// </summary>
    public class ApplicationViewModel : MvxViewModel
    {
        #region Public Properties

        /// <summary>
        /// Indicates which page is shown currently in the application
        /// </summary>
        public ApplicationPage CurrentPage { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Navigates the application to specified page
        /// And sets the initial view model for that page if provided
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="viewModel">The view model to set initially on page (if provided)</param>
        public async Task GoToPageAsync(ApplicationPage page, MvxViewModel viewModel = null)
        {
            // Change current page to specified one
            CurrentPage = page;

            // If view model wasnt provided
            if (viewModel == null)
                // Create new one for new page
                viewModel = page.GetViewModel();

            // Change the page on application
            await DI.UI.GoToViewModelPage(viewModel);
        }

        #endregion
    }
}
