using MvvmCross.ViewModels;
using System.Diagnostics;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Extensions for <see cref="ApplicationPage"/>
    /// </summary>
    public static class ApplicationPageExtensions
    {
        /// <summary>
        /// Returns the brand-new view model to use on specified page
        /// </summary>
        /// <param name="page">The page to get view model for</param>
        public static MvxViewModel GetViewModel(this ApplicationPage page)
        {
            switch (page)
            {
                case ApplicationPage.Login:
                    return DI.GetInjectedPageViewModel<LoginPageViewModel>();

                case ApplicationPage.About:
                    return new AboutPageViewModel();

                case ApplicationPage.Settings:
                    return DI.GetInjectedPageViewModel<SettingsPageViewModel>();

                case ApplicationPage.TasksList:
                    return DI.GetInjectedPageViewModel<TasksListPageViewModel>();

                case ApplicationPage.TasksTime:
                    return DI.GetInjectedPageViewModel<TasksTimePageViewModel>();

                case ApplicationPage.TasksSummary:
                    return DI.GetInjectedPageViewModel<TasksSummaryPageViewModel>();

                case ApplicationPage.TasksSession:
                    return DI.GetInjectedPageViewModel<TasksSessionPageViewModel>();

                default:
                    // Alert developer of an issue
                    Debugger.Break();
                    return null;
            }
        }
    }
}
