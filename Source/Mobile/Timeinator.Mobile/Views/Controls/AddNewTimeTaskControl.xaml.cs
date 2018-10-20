using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewTimeTaskControl : BasePage
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AddNewTimeTaskControl()
        {
            // Do default things
            InitializeComponent();

            // Set brand-new view model
            BindingContext = new TimeTaskViewModel();
        }

        /// <summary>
        /// Constructor with additional view model to setup for this page
        /// </summary>
        public AddNewTimeTaskControl(TimeTaskViewModel viewModel)
        {
            // Do default things
            InitializeComponent();

            // Set specified view model
            BindingContext = viewModel ?? new TimeTaskViewModel();
        }

        #endregion
    }
}
