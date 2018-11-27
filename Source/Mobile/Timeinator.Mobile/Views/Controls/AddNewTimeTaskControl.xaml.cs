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
            BindingContext = DI.GetInjectedPageViewModel<AddNewTimeTaskViewModel>();
        }

        /// <summary>
        /// Constructor with additional view model to setup for this page
        /// </summary>
        public AddNewTimeTaskControl(AddNewTimeTaskViewModel viewModel)
        {
            // Do default things
            InitializeComponent();

            // Set specified view model
            BindingContext = viewModel ?? DI.GetInjectedPageViewModel<AddNewTimeTaskViewModel>();

            // View model was provided, so we are editing task instead of adding new one
            ConfirmButton.Text = "Potwierdź zmiany";
        }

        #endregion
    }
}
