using Android.App;
using Android.OS;
using Android.Widget;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using System;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(NoHistory = true)]
    public class TasksTimePage : MvxAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TasksTimePage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
        }

        protected override void OnStart()
        {
            base.OnStop();

            // Find checkbox setting time picker mode
            var finishModeCheckBox = FindViewById<CheckBox>(Resource.Id.finishTimeMode);

            // Find the timepicker on this page
            var timepicker = FindViewById<TimePicker>(Resource.Id.pickerSession);

            // Set default values (otherwise it would be current user time, which we don't want)
            timepicker.SetIs24HourView(new Java.Lang.Boolean(true));

            // Listen for mode changed
            finishModeCheckBox.CheckedChange += CheckBox_CheckedChanged;
            CheckBox_CheckedChanged(finishModeCheckBox, null);

            // Listen out for time changes
            timepicker.TimeChanged += Timepicker_TimeChanged;
        }

        /// <summary>
        /// Fired when mode for time picker has been changed
        /// </summary>
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Get finished mode value
            var isFinishMode = (BindingContext.DataContext as TasksTimePageViewModel).FinishMode;

            // Get timepicker
            var timepicker = FindViewById<TimePicker>(Resource.Id.pickerSession);

            if (isFinishMode)
            {
                // Set time to Now
                timepicker.Hour = DateTime.Now.Hour;
                timepicker.Minute = DateTime.Now.Minute;
            }
            else
            {
                // Set time to 0
                timepicker.Hour = 0;
                timepicker.Minute = 0;
            }
        }

        /// <summary>
        /// Fired when timepicker's time changes
        /// It allows for manual binding, since Mvx one isn't working apparently (there is no easy way to bind TimeSpan to the value)
        /// </summary>
        private void Timepicker_TimeChanged(object sender, TimePicker.TimeChangedEventArgs e)
        {
            // Get the timepicker itself
            var timepicker = sender as TimePicker;

            // Get the current view model for this page
            var viewModel = BindingContext.DataContext as TasksTimePageViewModel;

            // Set it's time to view model 
            viewModel.UserTime = new TimeSpan(timepicker.Hour, timepicker.Minute, 0);
        }
    }
}