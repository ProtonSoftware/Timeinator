
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Widget;
using MvvmCross;
using MvvmCross.Binding.Binders;
using MvvmCross.Binding.Extensions;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using System.Linq;
using Timeinator.Core;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.AndroidNative
{
    [MvxActivityPresentation]
    [Activity(Label = "View for TasksListPageViewModel")]
    public class TasksListPage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TasksListPage);

            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_top);
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Get this page view and data context
            var page = FindViewById(Resource.Id.taskListPage);
            var viewModel = BindingContext.DataContext as TasksListPageViewModel;
            var binder = Mvx.IoCProvider.Resolve<IMvxBinder>();

            // Find two bottom menu icons
            var settingsIcon = page.FindViewById(Resource.Id.settingsIcon);
            var aboutIcon = page.FindViewById(Resource.Id.aboutIcon);

            // Set their icons and texts accordingly (Maybe find XML way to do this in the future?)
            var textViewSettings = settingsIcon.FindViewById(Resource.Id.smallLabel) as TextView;

            // TODO: Find a better way, but for now, when the icon has already been changed from default name...
            if (textViewSettings.Text != "Name")
                // Don't do any operations to avoid multiple bindings
                return;

            textViewSettings.SetText(Resource.String.action_settings);
            var imageViewSettings = settingsIcon.FindViewById(Resource.Id.icon) as ImageView;
            imageViewSettings.SetImageResource(Resource.Drawable.ic_settings_black_24dp);

            var textViewAbout = aboutIcon.FindViewById(Resource.Id.smallLabel) as TextView;
            textViewAbout.SetText(Resource.String.action_about);
            var imageViewAbout = aboutIcon.FindViewById(Resource.Id.icon) as ImageView;
            imageViewAbout.SetImageResource(Resource.Drawable.ic_info_black_24dp);

            // Bind their click events to proper commands in view model
            var bindingSettings = binder.Bind(viewModel, settingsIcon, "Click OpenSettingsCommand").First();
            BindingContext.RegisterBinding(settingsIcon, bindingSettings);
            var bindingAbout = binder.Bind(viewModel, aboutIcon, "Click OpenAboutCommand").First();
            BindingContext.RegisterBinding(aboutIcon, bindingAbout);

            // Find task list container
            var recyclerView = (MvxRecyclerView)page.FindViewById(Resource.Id.taskList);

            // Allow item swiping
            var callback = new SwipeRecyclerViewItemCallback(recyclerView.GetAdapter());
            var itemTouchHelper = new ItemTouchHelper(callback);
            itemTouchHelper.AttachToRecyclerView(recyclerView);
            recyclerView.SetItemAnimator(new DefaultItemAnimator());

            // When full swipe on item happens...
            callback.OnSwipe += (position) => 
            {
                // Get view model of that item
                var removedVM = recyclerView.Adapter.ItemsSource.ElementAt(position);

                // Remove it from the list
                viewModel.DeleteTaskCommand.Execute(removedVM);
            };

            // For item single short clicks
            recyclerView.ItemClick = new RelayParameterizedCommand((s) =>
            {
                // Enable/disable it as if checkbox was clicked - provides better UX
                var itemVM = s as TimeTaskViewModel;

                // Handle it only when context menu is hidden
                if (itemVM.IsContextMenuVisible == false)
                    itemVM.IsEnabled ^= true;
            });

            // For item long clicks
            recyclerView.ItemLongClick = new RelayParameterizedCommand((s) => 
            {
                // Show context menu with edit/delete buttons
                var itemVM = s as TimeTaskViewModel;
                itemVM.IsContextMenuVisible ^= true;

                // Subscribe to context menu events so action can be performed after clicks
                itemVM.OnEditRequest -= (param) => viewModel.EditTaskCommand.Execute(param);
                itemVM.OnDeleteRequest -= (param) => viewModel.DeleteTaskCommand.Execute(param);
                itemVM.OnEditRequest += (param) => viewModel.EditTaskCommand.Execute(param);
                itemVM.OnDeleteRequest += (param) => viewModel.DeleteTaskCommand.Execute(param);
            });
        }
    }
}