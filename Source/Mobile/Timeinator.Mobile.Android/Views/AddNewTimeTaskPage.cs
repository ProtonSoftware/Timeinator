using Android.App;
using Android.OS;
using Android.Support.V4.View;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using System.Collections.Generic;
using Timeinator.Core;
using Timeinator.Mobile.Core;

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
            var fragments = new List<MvxFragmentInfo>
            {
                new MvxFragmentInfo
                {
                    FragmentType = typeof(AddNewTimeTaskFragment),
                    Title = TimeTaskType.Generic.ToString(),
                    ViewModel = viewModel
                },
                new MvxFragmentInfo
                {
                    FragmentType = typeof(AddNewTimeTaskFragment),
                    Title = TimeTaskType.Reading.ToString(),
                    ViewModel = viewModel
                }
            };

            // Get the view pager from this page
            var viewPager = FindViewById<ViewPager>(Resource.Id.viewPagerAdd);

            // Set the content as fragments using our adapter
            viewPager.Adapter = new MvxFragmentStatePagerAdapter(BaseContext, SupportFragmentManager, fragments);
        }
    }
}