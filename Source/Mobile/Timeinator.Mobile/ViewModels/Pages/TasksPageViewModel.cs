using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for main tasks page
    /// </summary>
    public class TasksPageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The list of time tasks for today to show in this page
        /// </summary>
        public ObservableCollection<TimeTaskContext> TaskItems { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to show a modal to add new task to the list
        /// </summary>
        public ICommand AddNewTaskCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksPageViewModel()
        {
            // Create commands
            AddNewTaskCommand = new RelayCommand(() => DI.UI.ShowModalOnCurrentNavigation(new AddNewTimeTaskControl()));

            // TODO: Get tasks from a manager
        }

        #endregion
    }
}
