using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Timeinator.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeTaskControl : ContentControl
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTaskControl()
        {
            // Do default things
            InitializeComponent();

            var a = this.BindingContext;
        }

        #endregion
    }
}
