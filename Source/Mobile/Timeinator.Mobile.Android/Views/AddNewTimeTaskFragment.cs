using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Org.Adw.Library.Widgets.Discreteseekbar;
using System;
using Timeinator.Core;
using Timeinator.Mobile.Domain;
using static Android.App.TimePickerDialog;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The fragment view for every setting entry
    /// </summary>
    /// This fragment presentation doesn't matter for the fragment since we use outside binding and view model isn't needed
    /// But Mvx requires it, so it must be here
    [MvxFragmentPresentation(activityHostViewModelType: typeof(AddNewTimeTaskPageViewModel), 
                             fragmentContentId: Resource.Layout.SettingsCheckboxFragment)]
    public class AddNewTimeTaskFragment : MvxFragment, IOnTimeSetListener
    {
        #region Public Properties

        /// <summary>
        /// The instance of current view model for this fragment casted to the appropriate one
        /// </summary>
        public AddNewTimeTaskPageViewModel CurrentViewModel => ViewModel as AddNewTimeTaskPageViewModel;

        /// <summary>
        /// The type of this time task
        /// </summary>
        public TimeTaskType Type { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor that initializes this fragment with provided data
        /// </summary>
        /// <param name="type">The type of this time task</param>
        public AddNewTimeTaskFragment(TimeTaskType type)
        {
            // Get the type for this specific time task
            Type = type;
        }

        public AddNewTimeTaskFragment()
        {

        }

        #endregion

        #region Android View Methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Do base stuff (we dont need this view anyway)
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            // Setup binding context for the inflater not to cause errors with Mvx's controls
            this.EnsureBindingContextIsSet(inflater);

            // Based on task's type...
            switch (Type)
            {
                case TimeTaskType.Generic:
                    // Create the view for generic task fragment
                    view = this.BindingInflate(Resource.Layout.AddNewTimeTaskGenericFragment, container, false);
                    break;

                case TimeTaskType.Reading:
                    // Create the view for reading task fragment
                    view = this.BindingInflate(Resource.Layout.AddNewTimeTaskReadingFragment, container, false);
                    break;

                default:
                    // We shouldn't get there at all, so something went wrong in the code
                    throw new Exception("AddNewTimeTask - tried to create a fragment that does not exist!");
            }

            // Find main fragment view
            var pageView = view.FindViewById(Resource.Id.addTaskLayout);

            // Find the priority seekbar
            var prioritySeekBar = pageView.FindViewById<DiscreteSeekBar>(Resource.Id.prioritySeekBar);

            // Set initial progress value
            prioritySeekBar.Progress = CurrentViewModel.TaskPrioritySliderValue;

            // Everytime progress of the seekbar changes, update the view model 
            prioritySeekBar.ProgressChanged += (sender, args) => CurrentViewModel.TaskPrioritySliderValue = (sender as DiscreteSeekBar).Progress;

            // Find the time text view
            var timeText = pageView.FindViewById<TextView>(Resource.Id.taskTimeText);

            // Attach on-click event
            timeText.Click += TimeText_Click;

            // Return the final view
            return view;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Fired when time text view is clicked
        /// Shows time picker dialog to the user
        /// </summary>
        private void TimeText_Click(object sender, EventArgs e)
        {
            // If user didn't check constant time flag...
            if (!CurrentViewModel.TaskHasConstantTime)
                // We shouldn't do anything
                return;

            // Get currently shown time 
            var currentTime = CurrentViewModel.TaskConstantTime;

            // Create the dialog
            var timePicker = new TimePickerDialog(Context, this, currentTime.Hours, currentTime.Minutes, true);

            // Show it to the user
            timePicker.Show();
        }

        /// <summary>
        /// Fired whenever new time is being set in time picker dialog
        /// </summary>
        public void OnTimeSet(TimePicker view, int hourOfDay, int minute) => CurrentViewModel.TaskConstantTime = new TimeSpan(hourOfDay, minute, 0);

        #endregion
    }
}