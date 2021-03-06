﻿using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Widget;
using EverythingMe.AndroidUI.OverScroll;
using MvvmCross;
using MvvmCross.Binding.Binders;
using MvvmCross.Binding.Extensions;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using System.Linq;
using Timeinator.Core;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity]
    public class TasksListPage : MvxAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TasksListPage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
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
            var textViewSettings = settingsIcon.FindViewById<TextView>(Resource.Id.smallLabel);

            // TODO: Find a better way, but for now, when the icon has already been changed from default name...
            if (textViewSettings.Text != "Name")
                // Don't do any operations to avoid multiple bindings
                return;

            textViewSettings.SetText(LocalizationResource.Settings, TextView.BufferType.Normal);
            var imageViewSettings = settingsIcon.FindViewById<ImageView>(Resource.Id.icon);
            imageViewSettings.SetImageResource(Resource.Drawable.ic_settings_black_24dp);

            var textViewAbout = aboutIcon.FindViewById<TextView>(Resource.Id.smallLabel);
            textViewAbout.SetText(LocalizationResource.AboutUs, TextView.BufferType.Normal);
            var imageViewAbout = aboutIcon.FindViewById<ImageView>(Resource.Id.icon);
            imageViewAbout.SetImageResource(Resource.Drawable.ic_info_black_24dp);

            // Bind their click events to proper commands in view model
            var bindingSettings = binder.Bind(viewModel, settingsIcon, "Click OpenSettingsCommand").First();
            BindingContext.RegisterBinding(settingsIcon, bindingSettings);
            var bindingAbout = binder.Bind(viewModel, aboutIcon, "Click OpenAboutCommand").First();
            BindingContext.RegisterBinding(aboutIcon, bindingAbout);

            // Find task list container
            var recyclerView = page.FindViewById<MvxRecyclerView>(Resource.Id.taskList);

            // Subscribe to child view added event to populate tags list for task list items
            recyclerView.ChildViewAdded += RecyclerView_ChildViewAdded;

            // Allow item swiping
            var callback = new SwipeRecyclerViewItemCallback(recyclerView.GetAdapter());
            var itemTouchHelper = new ItemTouchHelper(callback);
            itemTouchHelper.AttachToRecyclerView(recyclerView);
            recyclerView.SetItemAnimator(new DefaultItemAnimator());

            // When full swipe on item happens...
            callback.OnSwipe += (position, removeDirection) => 
            {
                // Get view model of that item
                var swipedVM = recyclerView.Adapter.ItemsSource.ElementAt(position);

                // If the item was moved in the remove direction...
                if (removeDirection)
                    // Remove it from the list
                    viewModel.DeleteTaskCommand.Execute(swipedVM);

                // Otherwise, that means it was moved in edit direction
                else
                    // Show edit modal with item's view model injected
                    viewModel.EditTaskCommand.Execute(swipedVM);
            };

            // For item single short clicks
            recyclerView.ItemClick = new RelayParameterizedCommand((s) =>
            {
                // Enable/disable it as if checkbox was clicked - provides better UX
                var itemVM = s as ListTimeTaskItemViewModel;

                // Handle it only when context menu is hidden
                if (itemVM.IsContextMenuVisible == false)
                    itemVM.IsEnabled ^= true;
            });

            // For item long clicks
            recyclerView.ItemLongClick = new RelayParameterizedCommand((s) => 
            {
                // Show context menu with edit/delete buttons
                var itemVM = s as ListTimeTaskItemViewModel;
                itemVM.IsContextMenuVisible ^= true;

                // Subscribe to context menu events so action can be performed after clicks
                itemVM.OnEditRequest -= (param) => viewModel.EditTaskCommand.Execute(param);
                itemVM.OnDeleteRequest -= (param) => viewModel.DeleteTaskCommand.Execute(param);
                itemVM.OnEditRequest += (param) => viewModel.EditTaskCommand.Execute(param);
                itemVM.OnDeleteRequest += (param) => viewModel.DeleteTaskCommand.Execute(param);
            });

            // Allow overscrolling for the task list
            OverScrollDecoratorHelper.SetUpOverScroll(recyclerView, OverScrollDecoratorHelper.OrientationVertical);
        }

        #region Private Helpers

        /// <summary>
        /// Called whenever new task is added to the main task list
        /// Used to populate tags list in every item
        /// </summary>
        /// <param name="e">Event arguments, one of which is child's view</param>
        private void RecyclerView_ChildViewAdded(object sender, global::Android.Views.ViewGroup.ChildViewAddedEventArgs e)
        {
            // Get the actual view of the child
            var view = e.Child;

            // TODO: Decide if we do this or not
        }

        #endregion
    }
}