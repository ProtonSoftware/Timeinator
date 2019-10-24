using Android.App;
using Android.OS;
using Android.Support.V4.View;
using DK.Ostebaronen.Droid.ViewPagerIndicator;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity]
    public class AddNewTimeTaskPage : MvxAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddNewTimeTaskPage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            // Get the view model for this page
            var viewModel = BindingContext.DataContext as AddNewTimeTaskPageViewModel;

            // Create the fragment pages for every question type
            // TODO: Make some configuration file to take info like that from one source
            var fragments = new List<MvxFragmentInfo>
            {
                new MvxFragmentInfo(TimeTaskType.Generic, viewModel),
                new MvxFragmentInfo(TimeTaskType.Reading, viewModel)
            };

            // Get the view pager from this page
            var viewPager = FindViewById<ViewPager>(Resource.Id.viewPagerAdd);

            // Set the content as fragments using our adapter
            viewPager.Adapter = new MvxFragmentStatePagerAdapter(BaseContext, SupportFragmentManager, fragments);

            // Get the icon indicator and associate it with view pager
            var indicator = FindViewById<IconPageIndicator>(Resource.Id.indicatorAdd);
            indicator.SetViewPager(viewPager);

            // Whenever current displayed fragment changes...
            indicator.PageSelected += (sender, args) =>
            {
                // Get the fragment that is currently displayed
                var selectedFragment = fragments.ElementAt(args.Position);

                // Change the task type based on that
                viewModel.TaskType = (TimeTaskType)Enum.Parse(typeof(TimeTaskType), selectedFragment.Title);
            };
        }
    }
}