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

            // Subscribe to an event to show task list when it is raised
            (BindingContext as TasksPageViewModel).TasksUIReady += TasksPage_TasksUIReady;
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

            // Subscribe to an event to show task list when it is raised
            (BindingContext as TasksPageViewModel).TasksUIReady += TasksPage_TasksUIReady;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Fired when view model is ready with its task list so we can show it in the UI here
        /// </summary>
        private void TasksPage_TasksUIReady()
        {
            // Get current view model state
            var currentVM = BindingContext as TasksPageViewModel;

            // For each of the task in the list...
            foreach (var task in currentVM.TaskItems)
                // Add it to the container
                TasksContainer.Children.Add(new TimeTaskControl(task));
        }

        #endregion
    }
}
