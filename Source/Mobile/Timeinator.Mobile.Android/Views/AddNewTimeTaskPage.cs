
using Android.App;
using Android.OS;
using Android.Widget;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using Org.Adw.Library.Widgets.Discreteseekbar;
using System;
using Timeinator.Mobile.Core;
using static Android.App.TimePickerDialog;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(Label = "View for AddNewTimeTaskPageViewModel")]
    public class AddNewTimeTaskPage : MvxActivity, IOnTimeSetListener
    {
        /// <summary>
        /// Gets currently shown view model in this page
        /// </summary>
        private AddNewTimeTaskPageViewModel CurrentViewModel => BindingContext.DataContext as AddNewTimeTaskPageViewModel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddNewTimeTaskPage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Find the priority seekbar
            var prioritySeekBar = (DiscreteSeekBar)FindViewById(Resource.Id.prioritySeekBar);
            
            // Set initial progress value
            prioritySeekBar.Progress = CurrentViewModel.TaskPrioritySliderValue;

            // Listen out for future progress changes
            prioritySeekBar.ProgressChanged += PrioritySeekBar_ProgressChanged;

            // Find the time text view
            var timeText = (TextView)FindViewById(Resource.Id.taskTimeText);

            // Attach on-click event
            timeText.Click += TimeText_Click;
        }

        /// <summary>
        /// Fired when time text view is clicked
        /// Shows time picker dialog to the user
        /// </summary>
        private void TimeText_Click(object sender, System.EventArgs e)
        {
            // If user didn't check constant time flag...
            if (!CurrentViewModel.TaskHasConstantTime)
                // We shouldn't do anything
                return;

            // Get currently shown time 
            var currentTime = CurrentViewModel.TaskConstantTime;

            // Create the dialog
            var timePicker = new TimePickerDialog(this, this, currentTime.Hours, currentTime.Minutes, true);

            // Show it to the user
            timePicker.Show();
        }


        /// <summary>
        /// Fired when priority seek bar's progress changes
        /// It allows for manual binding, since Mvx one isn't working apparently
        /// </summary>
        private void PrioritySeekBar_ProgressChanged(object sender, DiscreteSeekBar.ProgressChangedEventArgs e)
        {
            // Get the seekbar itself
            var seekBar = sender as DiscreteSeekBar;

            // Set it's progress to view model 
            CurrentViewModel.TaskPrioritySliderValue = seekBar.Progress;
        }

        /// <summary>
        /// Fired whenever new time is being set in time picker dialog
        /// </summary>
        public void OnTimeSet(TimePicker view, int hourOfDay, int minute) => CurrentViewModel.TaskConstantTime = new TimeSpan(hourOfDay, minute, 0);
    }
}