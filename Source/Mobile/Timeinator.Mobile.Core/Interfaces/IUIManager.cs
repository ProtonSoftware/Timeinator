using MvvmCross.ViewModels;
using System;
using System.Threading.Tasks;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// Manages all the UI interactions in this application
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        /// Closes current page from the stack and as a result goes back to the previous one
        /// </summary>
        /// <param name="viewModel">The view model for the currently shown page that we want to close</param>
        Task GoBackToPreviousPage(MvxViewModel currentVM);

        /// <summary>
        /// Changes the current application's page to the one that is associated with specified view model
        /// </summary>
        /// <param name="viewModel">The view model for the page</param>
        Task GoToViewModelPage(MvxViewModel viewModel);

        /// <summary>
        /// Shows the popup to the user based on provided informations
        /// </summary>
        /// <param name="viewmodel">The provided properties of this popup to show</param>
        /// <returns>If the popup takes user response, true when user accepts and false when not
        ///          In case popup doesnt take any response from the user, always returns true when popup was shown succesfully</returns>
        Task<bool> DisplayPopupMessageAsync(PopupMessageViewModel viewmodel);

        /// <summary>
        /// Takes the action on the main application's thread and executes it here
        /// </summary>
        /// <param name="action">The action to execute on main thread</param>
        Task ExecuteOnMainThread(Action action);

        /// <summary>
        /// Changes application's language
        /// </summary>
        /// <param name="langCode">The international code for the language to change to</param>
        void ChangeLanguage(string langCode);
    }
}
