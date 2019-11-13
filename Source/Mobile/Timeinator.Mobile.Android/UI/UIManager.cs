using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Timeinator.Core;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Manages all the UI interactions in this application
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
            // Inject all dependiences from Mvx default DI container
            mNavigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            mMainThreadDispatcher = Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>();
            mUserDialogs = Mvx.IoCProvider.Resolve<IUserDialogs>();
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Implements <see cref="IUIManager.GoToViewModelPage(MvxViewModel)"/>
        /// </summary>
        public async Task GoToViewModelPage(MvxViewModel viewModel) => await mNavigationService.Navigate(viewModel);

        /// <summary>
        /// Implements <see cref="IUIManager.GoBackToPreviousPage(MvxViewModel)"/>
        /// </summary>
        public async Task GoBackToPreviousPage(MvxViewModel currentVM) => await mNavigationService.Close(currentVM);
        
        /// <summary>
        /// Implements <see cref="IUIManager.DisplayPopupMessageAsync(PopupMessageViewModel)"/>
        /// </summary>
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
        /// Implements <see cref="IUIManager.ExecuteOnMainThread(Action)"/>
        /// </summary>
        public async Task ExecuteOnMainThread(Action action) => await mMainThreadDispatcher.ExecuteOnMainThreadAsync(action);

        /// <summary>
        /// Implements <see cref="IUIManager.ChangeLanguage(string)"/>
        /// </summary>
        public void ChangeLanguage(string langCode)
        {
            // Create new culture based on provided code
            var culture = new CultureInfo(langCode);

            // And set it in every possible place so the whole application is now in different language
            LocalizationResource.Culture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        #endregion
    }
}
