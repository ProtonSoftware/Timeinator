using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Timeinator.Core;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Manages all the UI stuff in this application
    /// </summary>
    public class UIManager : IUIManager
    {
        #region Private Members

        /// <summary>
        /// The service from MVVMCross framework that allows page changing
        /// </summary>
        private IMvxNavigationService mNavigationService;

        /// <summary>
        /// The dispatcher that allows us to use main thread
        /// </summary>
        private IMvxMainThreadAsyncDispatcher mMainThreadDispatcher;

        /// <summary>
        /// The service for user dialogs library
        /// </summary>
        private IUserDialogs mUserDialogs;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UIManager()
        {
            mNavigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            mMainThreadDispatcher = Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>();
            mUserDialogs = Mvx.IoCProvider.Resolve<IUserDialogs>();
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Changes the current application's page to the one that is associated with specified view model
        /// </summary>
        /// <param name="viewModel">The view model for the page</param>
        public async Task GoToViewModelPage(MvxViewModel viewModel)
        {
            await mNavigationService.Navigate(viewModel);
        }

        /// <summary>
        /// Closes current page from the stack and as a result goes back to the previous one
        /// </summary>
        /// <param name="viewModel">The view model for the currently shown page that we want to close</param>
        /// <returns></returns>
        public async Task GoBackToPreviousPage(MvxViewModel currentVM)
        {
            await mNavigationService.Close(currentVM);
        }

        /// <summary>
        /// Shows the popup to the user based on provided informations
        /// </summary>
        /// <param name="viewmodel">The provided properties of this popup to show</param>
        /// <returns>If the popup takes user response, true when user accepts and false when not
        ///          In case popup doesnt take any response from the user, always returns true when popup was shown succesfully</returns>
        public async Task<bool> DisplayPopupMessageAsync(PopupMessageViewModel viewmodel)
        {
            // If we dont want to get any user response...
            if (string.IsNullOrEmpty(viewmodel.AcceptButtonText))
            {
                // Just show the message with provided informations
                await mUserDialogs.AlertAsync(viewmodel.Message, viewmodel.Title, viewmodel.CancelButtonText);

                // Return success afterwards
                return true;
            }

            // Otherwise, show response popup
            var response = await mUserDialogs.ConfirmAsync(viewmodel.Message, viewmodel.Title, viewmodel.AcceptButtonText, viewmodel.CancelButtonText);

            // And return user's response
            return response;
        }

        /// <summary>
        /// Takes the action on the main application's thread and executes it here
        /// </summary>
        /// <param name="action">The action to execute on main thread</param>
        /// <returns></returns>
        public async Task ExecuteOnMainThread(Action action)
        {
            await mMainThreadDispatcher.ExecuteOnMainThreadAsync(action);
        }

        /// <summary>
        /// Changes application's language
        /// </summary>
        /// <param name="langCode">The international code for the language to change to</param>
        public void ChangeLanguage(string langCode)
        {
            var culture = new CultureInfo(langCode);
            LocalizationResource.Culture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        #endregion
    }
}
