using MvvmCross.ViewModels;
using System.Threading.Tasks;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for initial login page
    /// </summary>
    public class LoginPageViewModel : MvxViewModel
    {
        #region Commands

        /// <summary>
        /// The command to enter the application without logging in
        /// </summary>
        public ICommand EnterWithoutLoginCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoginPageViewModel()
        {
            // Create commands
            EnterWithoutLoginCommand = new RelayCommand(() => ChangePageWithoutLogin());
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Changes the page without users' logging in
        /// </summary>
        public void ChangePageWithoutLogin()
        {
            // Simply go to main task list page
            DI.Application.GoToPage(ApplicationPage.TasksList);
        }

        #endregion
    }
}
