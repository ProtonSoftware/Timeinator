using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeTaskControl : ParentedContentView
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTaskControl()
        {
            // Do default things
            InitializeComponent();
        }

        #endregion

        /// <summary>
        /// Fired when this control is tapped by a user
        /// </summary>
        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            // If buttons are hidden...
            if (ButtonContainer.Opacity == 0)
                // Animate them
                ButtonContainer.FadeTo(1);
            // Otherwise...
            else
                // Hide them
                ButtonContainer.FadeTo(0);
        }
    }
}
