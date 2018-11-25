using System.Threading.Tasks;
using Xamarin.Forms;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Manages all the UI stuff in this application
    /// </summary>
    public class TestUIManager : IUIManager
    {
        /// <summary>
        /// Shows specified page on top of the current navigation page
        /// </summary>
        /// <param name="page">The page to show as a modal</param>
        public async Task ShowModalOnCurrentNavigation(ContentPage page)
        {
            // Get current navigation page
            var navigation = (App.Current.MainPage as PageHost).Detail.Navigation;

            // Push a page on top of that
            await navigation.PushAsync(page, true);

            // Hide the menu afterwards
            HideMenu();
        }

        /// <summary>
        /// Hides the most recent modal from top of the page host
        /// </summary>
        /// <returns></returns>
        public async Task HideRecentModalFromCurrentNavigation()
        {
            // Get current navigation page
            var navigation = (App.Current.MainPage as PageHost).Detail.Navigation;

            // Pop a modal
            await navigation.PopAsync(true);

            // Hide the menu afterwards
            HideMenu();
        }

        /// <summary>
        /// Hides the menu on demand
        /// </summary>
        public void HideMenu() => (App.Current.MainPage as PageHost).IsPresented = false;

        /// <summary>
        /// Shows the popup to the user based on provided informations
        /// </summary>
        /// <param name="viewmodel">The provided properties of this popup to show</param>
        /// <returns>If the popup takes user response, true when user accepts and false when not
        ///          In case popup doesnt take any response from the user, always returns true when popup was shown succesfully</returns>
        public async Task<bool> DisplayPopupMessageAsync(PopupMessageViewModel viewmodel)
        {
            /*
            // If we dont want to get any user response...
            if (string.IsNullOrEmpty(viewmodel.AcceptButtonText))
            {
                // Just show the message with provided informations
                await (App.Current.MainPage as PageHost).DisplayAlert(viewmodel.Title, viewmodel.Message, viewmodel.CancelButtonText);

                // Return success afterwards
                return true;
            }

            // Otherwise, show response popup
            var response = await (App.Current.MainPage as PageHost).DisplayAlert(viewmodel.Title, viewmodel.Message, viewmodel.AcceptButtonText, viewmodel.CancelButtonText);
            */

            // And return user's response
            return await Task.Run(() => false);
        }

        public void ChangeLanguage(string langCode)
        {
            throw new System.NotImplementedException();
        }
    }
}
