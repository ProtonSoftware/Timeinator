using Android.App;
using Android.OS;
using MvvmCross;
using MvvmCross.Binding.Binders;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using System.Linq;
using Timeinator.Core;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(Label = "View for SettingsPageViewModel")]
    public class SettingsPage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SettingsPage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
        }

        protected override void OnStart()
        {
            base.OnStart();

            var viewModel = BindingContext.DataContext as SettingsPageViewModel;
            var binder = Mvx.IoCProvider.Resolve<IMvxBinder>();

            // Create a fragment for every setting that will be shown in this page
            var settingHighestPriorityFragment = new SettingsCheckboxFragment(LocalizationResource.SettingHighestPriorityName, LocalizationResource.SettingHighestPriorityDescription);
            var settingRecalculateTasksFragment = new SettingsCheckboxFragment(LocalizationResource.SettingRecalculateTasksName, LocalizationResource.SettingRecalculateTasksDescription);

            // Show them in the view
            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.settingHighestPriority, settingHighestPriorityFragment)
                .Replace(Resource.Id.settingRecalculateTasks, settingRecalculateTasksFragment)
                .AddToBackStack(null)
                .Commit();
            FragmentManager.ExecutePendingTransactions();

            var bindingHighestPriority = binder.Bind(viewModel, settingHighestPriorityFragment.CheckBoxView, "Checked HighestPrioritySetAsFirst").First();
            BindingContext.RegisterBinding(settingHighestPriorityFragment.CheckBoxView, bindingHighestPriority);

            var bindingRecalculateTasks = binder.Bind(viewModel, settingRecalculateTasksFragment.CheckBoxView, "Checked RecalculateTasksAfterBreak").First();
            BindingContext.RegisterBinding(settingRecalculateTasksFragment.CheckBoxView, bindingRecalculateTasks);
        }
    }
}