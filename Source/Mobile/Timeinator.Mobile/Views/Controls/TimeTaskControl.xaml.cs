using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeTaskControl : ContentView
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTaskControl()
        {
            // Do default things
            InitializeComponent();

            // Set brand-new view model
            BindingContext = new TimeTaskViewModel();
        }

        /// <summary>
        /// Constructor with additional view model to setup for this control
        /// </summary>
        public TimeTaskControl(TimeTaskViewModel viewModel)
        {
            // Do default things
            InitializeComponent();

            // Set specified view model
            BindingContext = viewModel ?? new TimeTaskViewModel();
        }

        #endregion
    }
}
