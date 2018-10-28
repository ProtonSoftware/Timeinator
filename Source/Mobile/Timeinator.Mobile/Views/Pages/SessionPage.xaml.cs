﻿using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionPage : BasePage
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SessionPage()
        {
            // Do default things
            InitializeComponent();

            // Set brand-new view model
            BindingContext = new SessionPageViewModel();
        }

        /// <summary>
        /// Constructor with additional view model to setup for this page
        /// </summary>
        public SessionPage(SessionPageViewModel viewModel)
        {
            // Do default things
            InitializeComponent();

            // Set specified view model
            BindingContext = viewModel ?? new SessionPageViewModel();
        }

        #endregion
    }
}