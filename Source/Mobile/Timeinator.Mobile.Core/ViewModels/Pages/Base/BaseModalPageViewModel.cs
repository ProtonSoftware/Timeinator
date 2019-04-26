using MvvmCross.ViewModels;
using System.Windows.Input;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The base view model for any page that is show as modal and has return button
    /// </summary>
    public class BaseModalPageViewModel : MvxViewModel
    {
        #region Commands

        /// <summary>
        /// The command to close this modal page and go back to previous one
        /// </summary>
        public ICommand GoBackCommand { get; protected set; }

        #endregion
    }
}
