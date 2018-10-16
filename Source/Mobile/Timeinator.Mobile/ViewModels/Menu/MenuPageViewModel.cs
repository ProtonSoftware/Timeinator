using System.Collections.Generic;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for side menu page
    /// </summary>
    public class MenuPageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// List of application menu items in this page such as settings/about etc.
        /// </summary>
        public List<MenuPageItemViewModel> ApplicationItems { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default contructor
        /// </summary>
        public MenuPageViewModel()
        {
            // Initialize menu items list
            ApplicationItems = new List<MenuPageItemViewModel>
            {
                new MenuPageItemViewModel { Page = ApplicationPage.Settings, Title = "Settings", Icon = ApplicationIconType.Settings },
                new MenuPageItemViewModel { Page = ApplicationPage.About, Title = "Settings", Icon = ApplicationIconType.About  }
            };
        }

        #endregion
    }
}
