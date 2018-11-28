using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : BasePage
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoginPage()
        {
            // Do default things
            InitializeComponent();

            // Set brand-new view model
            BindingContext = DI.GetInjectedPageViewModel<LoginPageViewModel>();
        }

        /// <summary>
        /// Constructor with additional view model to setup for this page
        /// </summary>
        public LoginPage(LoginPageViewModel viewModel)
        {
            // Do default things
            InitializeComponent();

            // Set specified view model
            BindingContext = viewModel ?? DI.GetInjectedPageViewModel<LoginPageViewModel>();
        }

        #endregion
    }
}
