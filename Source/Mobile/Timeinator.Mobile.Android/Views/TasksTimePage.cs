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
            var modecheckbox = FindViewById<CheckBox>(Resource.Id.finishTimeMode);

            // Listen for mode changed
            modecheckbox.Click += Modecheckbox_Click;

            // Find the timepicker on this page
            var timepicker = FindViewById<TimePicker>(Resource.Id.pickerSession);

            // Set default values (otherwise it would be current user time, which we don't want)
            timepicker.Hour = 0;
            timepicker.Minute = 0;
            timepicker.SetIs24HourView(new Java.Lang.Boolean(true));

            // Listen out for time changes
            timepicker.TimeChanged += Timepicker_TimeChanged;
        }

        /// <summary>
        /// Fired when mode for time picker has been changed
        /// </summary>
        private void Modecheckbox_Click(object sender, EventArgs e)
        {
            // Get checkbox
            var modecheckbox = sender as CheckBox;

            // Get timepicker
            var timepicker = FindViewById<TimePicker>(Resource.Id.pickerSession);

            if (modecheckbox.Checked)
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