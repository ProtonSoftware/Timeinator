using Android.App;
using Android.OS;
using Android.Widget;
using MvvmCross;
using MvvmCross.Binding.Binders;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using System.Linq;
using Timeinator.Core;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity]
    public class SettingsPage : MvxAppCompatActivity
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
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.settingHighestPriority, settingHighestPriorityFragment)
                .Replace(Resource.Id.settingRecalculateTasks, settingRecalculateTasksFragment)
                .Replace(Resource.Id.settingChangeLanguage, settingChangeLanguageFragment)
                .AddToBackStack(null)
                .Commit();
            SupportFragmentManager.ExecutePendingTransactions();

            // Setup bindings for every fragment
            SetupBindingForSettingEntry(settingHighestPriorityFragment, "Checked HighestPrioritySetAsFirst");
            SetupBindingForSettingEntry(settingRecalculateTasksFragment, "Checked RecalculateTasksAfterBreak");
            SetupBindingForSettingEntry(settingChangeLanguageFragment, "ItemsSource LanguageItems; SelectedItem LanguageValue");

            // For dropdown, make manual binding for selected item because automatic one doesn't work
            var initialValuePosition = (BindingContext.DataContext as SettingsPageViewModel).LanguageItems.IndexOf((BindingContext.DataContext as SettingsPageViewModel).LanguageValue);
            (settingChangeLanguageFragment.SettableView as MvxSpinner).ItemSelected += MvxSpinner_ItemSelected;
            (settingChangeLanguageFragment.SettableView as MvxSpinner).SetSelection(initialValuePosition);
        }

        public override void OnBackPressed() => (BindingContext.DataContext as SettingsPageViewModel).GoBackCommand.Execute(null);


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

        /// <summary>
        /// Updates the selected dropdown item in the view model
        /// </summary>
        private void MvxSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
            => (BindingContext.DataContext as SettingsPageViewModel).LanguageValue = (e.View as TextView).Text;

        #endregion
    }
}