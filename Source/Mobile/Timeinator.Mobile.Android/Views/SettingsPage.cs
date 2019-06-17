using Android.App;
using Android.OS;
using MvvmCross;
using MvvmCross.Binding.Binders;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using System.Linq;
using Timeinator.Core;

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

            // Create a fragment for every setting that will be shown in this page
            var settingHighestPriorityFragment = new SettingsFragment(LocalizationResource.SettingHighestPriorityName, LocalizationResource.SettingHighestPriorityDescription, SettingType.Toggleable);
            var settingRecalculateTasksFragment = new SettingsFragment(LocalizationResource.SettingRecalculateTasksName, LocalizationResource.SettingRecalculateTasksDescription, SettingType.Toggleable);
            var settingChangeLanguageFragment = new SettingsFragment(LocalizationResource.SettingChangeLanguageName, LocalizationResource.SettingChangeLanguageDescription, SettingType.MultipleValues);

            // Show them in the view
            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.settingHighestPriority, settingHighestPriorityFragment)
                .Replace(Resource.Id.settingRecalculateTasks, settingRecalculateTasksFragment)
                .Replace(Resource.Id.settingChangeLanguage, settingChangeLanguageFragment)
                .AddToBackStack(null)
                .Commit();
            FragmentManager.ExecutePendingTransactions();

            // Setup bindings for every fragment
            SetupBindingForSettingEntry(settingHighestPriorityFragment, "Checked HighestPrioritySetAsFirst");
            SetupBindingForSettingEntry(settingRecalculateTasksFragment, "Checked RecalculateTasksAfterBreak");
            SetupBindingForSettingEntry(settingChangeLanguageFragment, "ItemsSource LanguageItems; SelectedItem LanguageValue");
        }

        #region Private Helpers

        /// <summary>
        /// A helper that setups Mvx binding for specified setting fragment
        /// </summary>
        /// <param name="setting">The setting fragment to bind</param>
        /// <param name="bindingString">The Mvx-Bind string</param>
        private void SetupBindingForSettingEntry(SettingsFragment setting, string bindingString)
        {
            var binder = Mvx.IoCProvider.Resolve<IMvxBinder>();

            var bindingRecalculateTasks = binder.Bind(BindingContext.DataContext, setting.SettableView, bindingString).First();
            BindingContext.RegisterBinding(setting.SettableView, bindingRecalculateTasks);
        }

        #endregion
    }
}