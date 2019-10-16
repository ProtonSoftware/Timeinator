using MvvmCross.ViewModels;

namespace Timeinator.Mobile.Domain
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
        public static MvxViewModel GetViewModel(this ApplicationPage page, IViewModelProvider viewModelProvider)
        {
            switch (page)
            {
                case ApplicationPage.Login:
                    return viewModelProvider.GetInjectedPageViewModel<LoginPageViewModel>();

                case ApplicationPage.About:
                    return viewModelProvider.GetInjectedPageViewModel<AboutPageViewModel>();

                case ApplicationPage.Settings:
                    return viewModelProvider.GetInjectedPageViewModel<SettingsPageViewModel>();

                case ApplicationPage.AddNewTask:
                    return viewModelProvider.GetInjectedPageViewModel<AddNewTimeTaskPageViewModel>();

                case ApplicationPage.TasksList:
                    return viewModelProvider.GetInjectedPageViewModel<TasksListPageViewModel>();

                case ApplicationPage.TasksTime:
                    return viewModelProvider.GetInjectedPageViewModel<TasksTimePageViewModel>();

                case ApplicationPage.TasksSummary:
                    return viewModelProvider.GetInjectedPageViewModel<TasksSummaryPageViewModel>();

                case ApplicationPage.TasksSession:
                    return viewModelProvider.GetInjectedPageViewModel<TasksSessionPageViewModel>();

                case ApplicationPage.Alarm:
                    return viewModelProvider.GetInjectedPageViewModel<AlarmPageViewModel>();

                default:
                    // Alert developer of an issue
                    // Debugger.Break();
                    return null;
            }
        }
    }
}
