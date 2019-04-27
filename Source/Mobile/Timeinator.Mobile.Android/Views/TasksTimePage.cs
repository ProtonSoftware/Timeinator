﻿using Android.App;
using Android.OS;
using Android.Widget;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using System;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(Label = "View for TasksTimePageViewModel",
              NoHistory = true)]
    public class TasksTimePage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TasksTimePage);
        }

        protected override void OnStart()
        {
            base.OnStop();

            // Find the timepicker on this page
            var timepicker = (TimePicker)FindViewById(Resource.Id.pickerSession);

            // Set default values (otherwise it would be current user time, which we don't want)
            timepicker.Hour = 0;
            timepicker.Minute = 0;
            timepicker.SetIs24HourView(new Java.Lang.Boolean(true));

            // Listen out for time changes
            timepicker.TimeChanged += Timepicker_TimeChanged;
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