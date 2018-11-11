using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for tasks session page
    /// </summary>
    public class SessionPageViewModel : BaseViewModel
    {
        #region PublicProperties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<TimeTaskViewModel> TaskItems { get; set; } = new ObservableCollection<TimeTaskViewModel>();
        /// <summary>
        /// Holds current task state
        /// </summary>
        public bool Paused => !DI.UserTimeHandler.TaskTimer.Enabled;

        #endregion

        #region Commands

        public ICommand StopCommand { get; private set; }
        public ICommand ExtendCommand { get; private set; }
        public ICommand ResumeCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SessionPageViewModel()
        {
            // Create commands
            StopCommand = new RelayCommand(() => DI.UserTimeHandler.StopTask());
            ResumeCommand = new RelayCommand(() => DI.UserTimeHandler.ResumeTask());
            ExtendCommand = new RelayCommand(() => DI.UserTimeHandler.ExtendTask());

            LoadTaskList();
        }

        #endregion

        /// <summary>
        /// Loads saved tasks from the <see cref="TimeTasksManager"/>
        /// </summary>
        public void LoadTaskList()
        {
            foreach (var e in DI.UserTimeHandler.DownloadSession())
                TaskItems.Add(DI.TimeTasksMapper.Map(e));
        }
    }
}
