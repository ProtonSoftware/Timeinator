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
                    return new LoginPage(viewModel as LoginPageViewModel);

                case ApplicationPage.TasksList:
                    return new TasksListPage(viewModel as TasksListPageViewModel);

                case ApplicationPage.Settings:
                    return new SettingsPage(viewModel as SettingsPageViewModel);

                case ApplicationPage.About:
                    return new AboutPage(viewModel as AboutPageViewModel);

                case ApplicationPage.TasksPreparation:
                    return new TasksPreparationPage(viewModel as TasksPreparationPageViewModel);

                case ApplicationPage.TasksSession:
                    return new SessionPage(viewModel as TasksSessionPageViewModel);

                // If no page was found, return initial one
                default:
                    return new LoginPage(viewModel as LoginPageViewModel);
            }
        }
    }
}
