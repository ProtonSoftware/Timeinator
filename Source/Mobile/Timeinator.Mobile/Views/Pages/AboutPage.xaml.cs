using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : BasePage
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AboutPage()
        {
            // Do default things
            InitializeComponent();

            // Set brand-new view model
            BindingContext = new AboutPageViewModel();
        }

        /// <summary>
        /// Constructor with additional view model to setup for this page
        /// </summary>
        public AboutPage(AboutPageViewModel viewModel)
        {
            // Do default things
            InitializeComponent();

            // Set specified view model
            BindingContext = viewModel ?? new AboutPageViewModel();
        }

        #endregion
    }
}
