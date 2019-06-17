using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The fragment view for setting's checkbox entry
    /// </summary>
    public class SettingsCheckboxFragment : Fragment
    {
        #region Public Properties

        /// <summary>
        /// The check box view that is shown in this fragment
        /// Used to setup binding to the boolean in the outside activity's view model
        /// </summary>
        public CheckBox CheckBoxView { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor that initializes this entry with provided data
        /// </summary>
        /// <param name="name">The name of this setting</param>
        /// <param name="description">The description of this setting</param>
        public SettingsCheckboxFragment(string name, string description)
        {
            // Create a bundle that will be accessible later when view is created
            var bundle = new Bundle();
            
            // Put here all the data
            bundle.PutString("Name", name);
            bundle.PutString("Description", description);

            // Save it to the arguments
            Arguments = bundle;
        }

        #endregion

        #region Android View Methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.SettingsCheckboxFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Get all the views in the fragment
            var settingName = view.FindViewById(Resource.Id.settingName) as TextView;
            var settingDescription = view.FindViewById(Resource.Id.settingDescription) as TextView;
            CheckBoxView = view.FindViewById(Resource.Id.settingCheckbox) as CheckBox;

            // Set texts to provided data
            settingName.Text = Arguments.GetString("Name");
            settingDescription.Text = Arguments.GetString("Description");
        }

        #endregion
    }
}