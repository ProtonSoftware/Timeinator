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
        public bool Paused { get; set; }

        #endregion

        #region Commands

        public ICommand BreakCommand { get; private set; }
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
            BreakCommand = new RelayCommand(() => DI.UserTimeHandler.StopTask());
            StopCommand = new RelayCommand(() => DI.UserTimeHandler.EndTask());
            //ExtendCommand = new RelayCommand(() => DI.UserTimeHandler.ExtendTask());
            ResumeCommand = new RelayCommand(() => DI.UserTimeHandler.ResumeTask());
            LoadTaskList();
            List<TimeTaskContext> cxts = new List<TimeTaskContext>();
            foreach (var task in TaskItems)
                cxts.Add(DI.TimeTasksMapper.ReverseMap(task));
            DI.UserTimeHandler.StartTimeHandler(cxts);
        }

        #endregion

        /// <summary>
        /// Loads saved tasks from the <see cref="TimeTasksManager"/>
        /// </summary>
        public void LoadTaskList()
        {
            // Get tasks from a manager
            foreach (var task in DI.TimeTasksManager.TaskContexts)
                TaskItems.Add(DI.TimeTasksMapper.Map(task));
        }
    }
}
