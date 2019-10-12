using MvvmCross.ViewModels;
using System.Windows.Input;
using Timeinator.Core;

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
        /// The command to take a break in current session
        /// </summary>
        public ICommand StartBreakCommand { get; private set; }

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
            // Create commands
            StartBreakCommand = new RelayCommand(StartBreak);
            FinishCommand = new RelayCommand(FinishTask);

            // Get injected DI services
            mRingtonePlayer = ringtonePlayer;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Called whenever the view for this view model appears on screen
        /// </summary>
        public override void ViewAppeared()
        {
            // Do base stuff
            base.ViewAppeared();

            // Play the sound for this screen
            mRingtonePlayer.Play();
        }

        /// <summary>
        /// Called whenever the view for this view model appears on screen
        /// </summary>
        public override void ViewDisappearing()
        {
            // Do base stuff
            base.ViewDisappearing();

            // Stop the sound
            mRingtonePlayer.Stop();
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Starts the break in current session
        /// </summary>
        private void StartBreak()
        {
            // Get current session view model
            var viewModel = DI.GetInjectedPageViewModel<TasksSessionPageViewModel>();

            // Go back to session page
            DI.Application.GoToPage(ApplicationPage.TasksSession, viewModel);

            // Finish task
            viewModel.FinishTaskCommand.Execute(null);

            // Start the break
            viewModel.PauseCommand.Execute(null);
        }

        /// <summary>
        /// Finishes current task in the session
        /// </summary>
        private void FinishTask()
        {
            // Get current session view model
            var viewModel = DI.GetInjectedPageViewModel<TasksSessionPageViewModel>();

            // Go back to session page
            DI.Application.GoToPage(ApplicationPage.TasksSession, viewModel);

            // Finish task
            viewModel.FinishTaskCommand.Execute(null);
        }

        #endregion
    }
}
