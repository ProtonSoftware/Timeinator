using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPreparationPage : BasePage
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksPreparationPage()
        {
            // Do default things
            InitializeComponent();

            // Set brand-new view model
            BindingContext = DI.GetInjectedPageViewModel<TasksPreparationPageViewModel>();
        }

        /// <summary>
        /// Constructor with additional view model to setup for this page
        /// </summary>
        public TasksPreparationPage(TasksPreparationPageViewModel viewModel)
        {
            // Do default things
            InitializeComponent();

            // Set specified view model
            BindingContext = viewModel ?? DI.GetInjectedPageViewModel<TasksPreparationPageViewModel>();
        }

        #endregion
    }
}
