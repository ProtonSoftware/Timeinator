using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for initial login page
    /// </summary>
    public class LoginPageViewModel : BasePageViewModel
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
            EnterWithoutLoginCommand = new RelayCommand(ChangePageWithoutLogin);

            // Set this page's title
            Title = "Zaloguj się";
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Changes the page without users' logging in
        /// </summary>
        public void ChangePageWithoutLogin()
        {
            // Simply change the page
            DI.Application.GoToPage(ApplicationPage.TasksList);
        }

        #endregion
    }
}
