using Xamarin.Forms;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Extensions for <see cref="ApplicationPage"/>
    /// </summary>
    public static class ApplicationPageExtensions
    {
        /// <summary>
        /// Returns actual application page based on enum
        /// </summary>
        /// <param name="page">The page as enum</param>
        /// <param name="viewModel">The view model to set initially on page (if provided)</param>
        /// <returns>The page as actual view</returns>
        public static Page ToApplicationPage(this ApplicationPage page, BaseViewModel viewModel = null)
        {
            // Based on provided page...
            switch (page)
            {
                case ApplicationPage.Login:
                    return new LoginPage(viewModel as LoginViewModel);

                case ApplicationPage.Tasks:
                    return new TasksPage(viewModel as TasksPageViewModel);

                case ApplicationPage.Settings:
                    return new SettingsPage(viewModel as SettingsViewModel);

                case ApplicationPage.About:
                    return new AboutPage(viewModel as AboutViewModel);

                // If no page was found, return initial one
                default:
                    return new LoginPage(viewModel as LoginViewModel);
            }
        }
    }
}
