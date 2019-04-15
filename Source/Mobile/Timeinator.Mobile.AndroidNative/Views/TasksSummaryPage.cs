
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using MvvmCross.Binding.Extensions;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.AndroidNative
{
    [MvxActivityPresentation]
    [Activity(Label = "View for TasksSummaryPageViewModel",
              NoHistory = true)]
    public class TasksSummaryPage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TasksSummaryPage);
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Get this page view and data context
            var page = FindViewById(Resource.Id.taskSummaryPage);
            var viewModel = BindingContext.DataContext as TasksSummaryPageViewModel;

            // Find task list container
            var recyclerView = (MvxRecyclerView)page.FindViewById(Resource.Id.taskList);

            // Allow item movements
            var callback = new MovementRecyclerViewItemCallback(recyclerView.GetAdapter());
            var itemTouchHelper = new ItemTouchHelper(callback);
            itemTouchHelper.AttachToRecyclerView(recyclerView);
            recyclerView.SetItemAnimator(new DefaultItemAnimator());

            // When full swipe on item happens...
            callback.OnMovement += (posFrom, posTo) =>
            {
                // Get view model of that item
                var movedVM = recyclerView.Adapter.ItemsSource.ElementAt(posFrom);

                // TODO: Make some function in TasksSummaryPageViewModel to call here so the items can be reordered
            };
        }
    }
}