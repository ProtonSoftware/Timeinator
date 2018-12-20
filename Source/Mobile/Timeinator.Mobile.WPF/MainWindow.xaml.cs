using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;
using Microsoft.Extensions.DependencyInjection;

namespace Timeinator.Mobile.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FormsApplicationPage
    {
        public MainWindow()
        {
            InitializeComponent();

            Forms.Init();
            LoadApplication(new Timeinator.Mobile.App());

            Dna.Framework.Construction.Services.AddSingleton<INotificationHandler, EmptyNotificationHandler>();
            Dna.Framework.Construction.Build();
        }
    }
}
