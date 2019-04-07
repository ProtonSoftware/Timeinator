
using Android.App;
using Android.OS;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using Org.Adw.Library.Widgets.Discreteseekbar;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.AndroidNative
{
    [MvxActivityPresentation]
    [Activity(Label = "View for AddNewTimeTaskPageViewModel")]
    public class AddNewTimeTaskPage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddNewTimeTaskPage);
        }

        protected override void OnStart()
        {
            base.OnStop();

            // Find the priority seekbar and listen out for progress changes
            var prioritySeekBar = (DiscreteSeekBar)FindViewById(Resource.Id.prioritySeekBar);
            prioritySeekBar.ProgressChanged += PrioritySeekBar_ProgressChanged;
        }


        /// <summary>
        /// Fired when priority seek bar's progress changes
        /// It allows for manual binding, since Mvx one isn't working apparently
        /// </summary>
        private void PrioritySeekBar_ProgressChanged(object sender, DiscreteSeekBar.ProgressChangedEventArgs e)
        {
            // Get the seekbar itself
            var seekBar = sender as DiscreteSeekBar;

            // Get the current view model for this page
            var viewModel = BindingContext.DataContext as AddNewTimeTaskPageViewModel;

            // Set it's progress to view model 
            viewModel.TaskPrioritySliderValue = seekBar.Progress;
        }
    }
}