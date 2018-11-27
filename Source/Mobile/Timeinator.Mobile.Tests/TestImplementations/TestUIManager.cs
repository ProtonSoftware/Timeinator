using System.Threading.Tasks;
using Xamarin.Forms;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Test class that implements <see cref="IUIManager"/>
    /// </summary>
    public class TestUIManager : IUIManager
    {
        #region Base UI Implementation

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

        #endregion

        #region Testing Implementations

        public async Task<bool> DisplayPopupMessageAsync(PopupMessageViewModel viewmodel)
        {
            return await Task.Run(() => false);
        }

        public void ChangeLanguage(string langCode)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
