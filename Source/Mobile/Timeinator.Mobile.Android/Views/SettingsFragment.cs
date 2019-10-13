using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;
using Timeinator.Core;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The fragment view for every setting entry
    /// </summary>
    /// This fragment presentation doesn't matter for the fragment since we use outside binding and view model isn't needed
    /// But Mvx requires it, so it must be here
    [MvxFragmentPresentation(activityHostViewModelType: typeof(SettingsPageViewModel), 
                             fragmentContentId: Resource.Layout.SettingsCheckboxFragment)]
    /// MvxFragment needs a view model, even if we don't use it, and since every view model is already taken by all the activities
    /// We use dummy not-used base view model as a replacement and it works as intended
    public class SettingsFragment : MvxFragment<BaseModalPageViewModel>
    {
        #region Public Properties

        /// <summary>
        /// Used to setup binding in the outside activity's view model
        /// </summary>
        public View SettableView { get; set; }

        /// <summary>
        /// The type of this setting
        /// </summary>
        public SettingType Type { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor that initializes this entry with provided data
        /// </summary>
        /// <param name="name">The name of this setting</param>
        /// <param name="description">The description of this setting</param>
        public SettingsFragment(string name, string description, SettingType type)
        {
            // Create a bundle that will be accessible later when view is created
            var bundle = new Bundle();
            
            // Put here all the data
            bundle.PutString("Name", name);
            bundle.PutString("Description", description);

            // Save it to the arguments
            Arguments = bundle;

            // Get the type for this specific setting
            Type = type;
        }

        #endregion

        #region Android View Methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Do base stuff
            base.OnCreateView(inflater, container, savedInstanceState);

            // Setup binding context for the inflater not to cause errors with Mvx's controls
            this.EnsureBindingContextIsSet(inflater);

            // Based on setting's type...
            switch (Type)
            {
                case SettingType.Toggleable:
                    // Return the view with checkbox to toggle values
                    return this.BindingInflate(Resource.Layout.SettingsCheckboxFragment, container, false);

                case SettingType.MultipleValues:
                    // Return the view with dropdown for user to pick one value from
                    return this.BindingInflate(Resource.Layout.SettingsDropdownFragment, container, false);

                default:
                    // We shouldn't get there at all, so something went wrong in the code
                    // Debugger.Break();
                    return null;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Get all the views in the fragment
            var settingName = view.FindViewById<TextView>(Resource.Id.settingName);
            var settingDescription = view.FindViewById<TextView>(Resource.Id.settingDescription);
            SettableView = view.FindViewById(Resource.Id.settingControl);

            // Set texts to provided data
            settingName.Text = Arguments.GetString("Name");
            settingDescription.Text = Arguments.GetString("Description");
        }

        #endregion
    }
}