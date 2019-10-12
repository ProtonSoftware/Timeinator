using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using System;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
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

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
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

            // When full move-over on item happens...
            callback.OnMovement += (posFrom, posTo) =>
            {
                // Set provided positons to parameter as tuple
                var parameter = new Tuple<int, int>(posFrom, posTo);

                // Reorder the task list
                viewModel.ReorderCommand.Execute(parameter);
            };

            // Launch SessionNotificationService
            DI.Container.GetInstance<SessionNotificationService>();
        }
    }
}