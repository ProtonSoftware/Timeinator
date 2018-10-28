using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPage : BasePage
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksPage()
        {
            // Do default things
            InitializeComponent();

            // Set brand-new view model
            BindingContext = new TasksPageViewModel();
        }

        /// <summary>
        /// Constructor with additional view model to setup for this page
        /// </summary>
        public TasksPage(TasksPageViewModel viewModel)
        {
            // Do default things
            InitializeComponent();

            // Set specified view model
            BindingContext = viewModel ?? new TasksPageViewModel();
        }

        #endregion
    }
}
