using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : BasePage
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsPage()
        {
            // Do default things
            InitializeComponent();

            // Set brand-new view model
            BindingContext = DI.GetInjectedPageViewModel<SettingsPageViewModel>();
        }

        /// <summary>
        /// Constructor with additional view model to setup for this page
        /// </summary>
        public SettingsPage(SettingsPageViewModel viewModel)
        {
            // Do default things
            InitializeComponent();

            // Set specified view model
            BindingContext = viewModel ?? DI.GetInjectedPageViewModel<SettingsPageViewModel>();
        }

        #endregion
    }
}
