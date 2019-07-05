using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Timeinator.Core;
using MvvmCross.ViewModels;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for alarm page
    /// </summary>
    public class AlarmPageViewModel : MvxViewModel
    {
        #region Private Members

        private readonly IRingtonePlayer mRingtonePlayer;

        #endregion

        #region Commands

        /// <summary>
        /// The command to pause current task
        /// </summary>
        public ICommand PauseCommand { get; private set; }

        /// <summary>
        /// The command to finish current task and go for the next one
        /// </summary>
        public ICommand FinishCommand { get; private set; }

        #endregion

        #region Constructor 

        /// <summary>
        /// Default constructor
        /// </summary>
        public AlarmPageViewModel(IRingtonePlayer ringtonePlayer)
        {
            // Get injected DI services
            mRingtonePlayer = ringtonePlayer;
        }

        #endregion

        public void InitializeButtons(Action pause, Action next)
        {
            mRingtonePlayer.Play();
            PauseCommand = new RelayCommand(() => { mRingtonePlayer.Stop(); pause.Invoke(); DI.Application.GoToPage(ApplicationPage.TasksSession); });
            FinishCommand = new RelayCommand(() => { mRingtonePlayer.Stop(); next.Invoke(); DI.Application.GoToPage(ApplicationPage.TasksSession); });
        }
    }
}
